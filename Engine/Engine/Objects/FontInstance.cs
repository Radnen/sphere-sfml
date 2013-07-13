using System;
using System.IO;
using SFML.Graphics;
using SFML.Window;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    public class FontInstance : ObjectInstance
    {
        private const int SIZE = 256; // atlas size

        Texture _fontAtlas;
        Sprite _fontSprite;
        IntRect[] _sources;
        uint _height = 0;
        short _version = 0;

        public FontInstance(ObjectInstance proto)
            : base (proto)
        {
            PopulateFunctions();
        }

        public FontInstance(ObjectInstance proto, string filename)
            : this (proto)
        {
            ReadFromFile(filename);
        }

        private void ReadFromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException();

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                if (!new String(reader.ReadChars(4)).Equals(".rfn"))
                    throw new FontException(filename, "Invalid file header.");
                _version = reader.ReadInt16();
                int num_chars = reader.ReadInt16();
                reader.ReadBytes(248);

                uint x = 0, y = 0;

                // construct a packed atlas:
                Image canvas = new Image(SIZE, SIZE, Color.Red);
                _sources = new IntRect[num_chars];
                for (var i = 0; i < num_chars; ++i)
                {
                    short width = reader.ReadInt16();

                    if (x + width >= SIZE)
                    {
                        x = 0;
                        y += _height;
                    }

                    short height = reader.ReadInt16();
                    _height = Math.Max(_height, (uint)height);
                    reader.ReadBytes(28);

                    byte[] pixels = reader.ReadBytes(width * height * 4);
                    Image glyph = new Image((uint)width, (uint)height, pixels);
                    _sources[i] = new IntRect((int)x, (int)y, width, height);
                    canvas.Copy(glyph, x, y);

                    if (x + width >= SIZE)
                    {
                        x = 0;
                        y += _height;
                    }
                    else
                        x += (uint)width;
                }

                _fontAtlas = new Texture(canvas);
                _fontSprite = new Sprite(_fontAtlas);
                canvas.Dispose();
            }
        }

        [JSFunction(Name = "drawText")]
        public void DrawText(double x, double y, string text)
        {
            Vector2f start = new Vector2f((float)x, (float)y);
            Vector2f offset = new Vector2f(0, 0);

            for (var i = 0; i < text.Length; ++i)
            {
                int ch = (int)text[i];

                _fontSprite.TextureRect = _sources[ch];
                _fontSprite.Position = start + offset;
                offset.X += _sources[ch].Width;
                Program._window.Draw(_fontSprite);
            }
        }

        [JSFunction(Name = "drawTextBox")]
        public void DrawTextBox(double x, double y, double w, double h, int spacing, string text)
        {
            ArrayInstance array = Wrap(text, (int)w);
            spacing += (int)_height;
            for (var i = 0; i < array.Length; ++i) {
                DrawText(x, y + i * spacing, (string)array[i.ToString()]);
            }
        }

        private int GetNextWordLength(ref string text, int pos)
        {
            int stop = text.IndexOf(" ", pos + 1) - pos;
            string word;
            if (stop < 0)
                word = text.Substring(pos, text.Length - pos);
            else
                word = text.Substring(pos, stop);
            return GetStringWidth(word);
        }

        [JSFunction(Name = "wordWrapString")]
        public ArrayInstance Wrap(string text, int width)
        {
            ArrayInstance array = Program._engine.Array.New();
            string word = "", current = "";
            int x = 0, word_w = 0;
            int space_w = GetStringWidth(" "), tab_w = GetStringWidth("   ");

            for (var i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                if (c == ' ' || c == '\t')
                {
                    int white_w = (c == ' ') ? space_w : tab_w;
                    if (x + word_w + white_w > width)
                    {
                        array.Push(current); // Push current line...
                        current = word + " ";
                        x = word_w + white_w;
                    }
                    else
                    {
                        current += word + " "; // else add it to current line.
                        x += word_w + white_w;
                    }

                    word = "";
                    word_w = 0;
                }
                else if (c == '\n')
                {
                    array.Push(current + word); // On \n just push current line,
                    current = word = "";        // and clear it.
                    word_w = 0;
                    x = 0;
                }
                else
                {
                    int char_w = _sources[c].Width;
                    if (word_w + char_w > width && x == 0) // break up long lines
                    {
                        array.Push(current + word);
                        current = word = "";
                        word_w = 0;
                    }
                    else if (x + word_w + char_w > width)
                    {
                        array.Push(current);
                        current = "";
                        x = 0;
                    }

                    word += c;        // Append character...
                    word_w += char_w; // and add to consumed width.
                }
            }
            array.Push(current + word);
            return array;
        }

        [JSFunction(Name = "getStringWidth")]
        public int GetStringWidth(string text)
        {
            int w = 0;
            for (var i = 0; i < text.Length; ++i)
            {
                w += _sources[(int)text[i]].Width;
            }
            return w;
        }

        [JSFunction(Name = "setColorMask")]
        public void SetColorMask(ColorInstance color)
        {
            _fontSprite.Color = color.GetColor();
        }

        [JSFunction(Name = "getColorMask")]
        public ColorInstance GetColorMask()
        {
            return new ColorInstance(Program._engine.Object.InstancePrototype, _fontSprite.Color);
        }

        [JSFunction(Name = "getHeight")]
        public int GetHeight()
        {
            return (int)_height;
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object font]";
        }

        [JSFunction(Name = "clone")]
        public FontInstance Clone()
        {
            FontInstance font = new FontInstance(Program._engine.Object.InstancePrototype);
            font._fontAtlas = new Texture(_fontAtlas);
            font._fontSprite = new Sprite(font._fontAtlas);
            font._height = _height;
            font._sources = (IntRect[])_sources.Clone();
            font._version = _version;
            return font;
        }

        [JSFunction(Name = "save")]
        public void Save(string filename)
        {
            filename = GlobalProps.BasePath + "\\images\\" + filename;
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filename)))
            {
                writer.Write(".rfn".ToCharArray());
                writer.Write(_version);
                writer.Write(new Byte[248]);

                Image canvas = _fontAtlas.CopyToImage();

                for (var i = 0; i < _sources.Length; ++i)
                {
                    writer.Write((short)_sources[i].Width);
                    writer.Write((short)_sources[i].Height);
                    writer.Write(new Byte[28]);

                    Image portion = new Image((uint)_sources[i].Width, (uint)_sources[i].Height);
                    portion.Copy(canvas, 0, 0, _sources[i]);

                    writer.Write(portion.Pixels);
                    portion.Dispose();
                }

                canvas.Dispose();
            }
        }
    }

    public class FontException : Exception
    {
        public FontException(string filename, string msg)
            : base(string.Format("Invalid Font at {0}: {1}", filename, msg))
        {
        }
    }
}

