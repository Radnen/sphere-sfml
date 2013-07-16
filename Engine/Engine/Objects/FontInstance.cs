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
        private const int SIZE = 512; // atlas size
        private static readonly Vector2f VECT_1 = new Vector2f(1, 1); 

        TextureAtlas _atlas = new TextureAtlas(SIZE);
        Sprite _fontSprite;
        Image[] _glyphs;
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

                _glyphs = new Image[num_chars];
                for (var i = 0; i < num_chars; ++i)
                {
                    short width = reader.ReadInt16();
                    short height = reader.ReadInt16();
                    _height = Math.Max(_height, (uint)height);
                    reader.ReadBytes(28);

                    byte[] pixels = reader.ReadBytes(width * height * 4);
                    _glyphs[i] = new Image((uint)width, (uint)height, pixels);
                }

                _atlas.Update(_glyphs);
            }

            _fontSprite = new Sprite(_atlas.Texture);
        }

        [JSFunction(Name = "drawText")]
        public void DrawText(double x, double y, string text)
        {
            Vector2f start = new Vector2f((float)x, (float)y);
            Vector2f offset = new Vector2f(0, 0);

            for (var i = 0; i < text.Length; ++i)
            {
                int ch = (int)text[i];

                _fontSprite.TextureRect = _atlas.Sources[ch];
                _fontSprite.Scale = VECT_1;
                _fontSprite.Position = start + offset;
                offset.X += _atlas.Sources[ch].Width;
                Program._window.Draw(_fontSprite);
            }
        }

        [JSFunction(Name = "drawZoomedText")]
        public void DrawZoomedText(double x, double y, double zoom, string text)
        {
            Vector2f start = new Vector2f((float)x, (float)y);
            Vector2f offset = new Vector2f(0, 0);
            Vector2f scale = new Vector2f((float)zoom, (float)zoom);

            for (var i = 0; i < text.Length; ++i)
            {
                int ch = (int)text[i];

                _fontSprite.TextureRect = _atlas.Sources[ch];
                _fontSprite.Scale = scale;
                _fontSprite.Position = start + offset;
                offset.X += _atlas.Sources[ch].Width * scale.X;
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

        [JSFunction(Name = "setCharacterImage")]
        public void SetCharacterImage(int ch, ImageInstance image)
        {
            _glyphs[ch] = image.GetImage();
            _atlas.Update(_glyphs);
        }

        [JSFunction(Name = "getCharacterImage")]
        public ImageInstance GetCharacterImage(int ch)
        {
            return new ImageInstance(Program._engine.Object.InstancePrototype,
                                     new Texture(_atlas.GetImageAt((uint)ch)));
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
                    int char_w = _atlas.Sources[c].Width;
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
                w += _atlas.Sources[(int)text[i]].Width;
            }
            return w;
        }

        [JSFunction(Name = "getStringHeight")]
        public int GetStringHeight(string text, int width)
        {
            ArrayInstance array = Wrap(text, width);
            return (int)(array.Length * _height);
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
            font._atlas = new TextureAtlas(_atlas);
            font._fontSprite = new Sprite(font._atlas.Texture);
            font._glyphs = new Image[_glyphs.Length];
            for (var i = 0; i < _glyphs.Length; ++i)
                font._glyphs[i] = new Image(_glyphs[i]);
            font._height = _height;
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

                using (Image canvas = _atlas.Texture.CopyToImage())
                {
                    for (uint i = 0; i < _atlas.Sources.Length; ++i)
                    {
                        writer.Write((short)_atlas.Sources[i].Width);
                        writer.Write((short)_atlas.Sources[i].Height);
                        writer.Write(new Byte[28]);

                        using (Image portion = _atlas.GetImageAt(i))
                        {
                            writer.Write(portion.Pixels);
                        }
                    }
                }
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

