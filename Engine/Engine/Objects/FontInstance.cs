using System;
using System.IO;
using System.Text;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.System;

namespace Engine.Objects
{
    public class FontInstance : ObjectInstance
    {
        private const int SIZE = 512; // atlas size

        TextureAtlas _atlas = new TextureAtlas(SIZE);
        Image[] _glyphs;
        uint _height = 0;
        short _version = 0;
        bool _updated = false;
        Color _color = Color.White;

        public Image GetGlyph(int num) {
            return _glyphs[num];
        }

        public FontInstance(ScriptEngine parent)
            : base (parent.Object.InstancePrototype)
        {
            PopulateFunctions();
        }

        public FontInstance(ScriptEngine parent, string filename)
            : this (parent)
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
        }

        /// <summary>
        /// Tries to update the atlas if any graphical changes were made to this font.
        /// </summary>
        private void CheckUpdate()
        {
            if (_updated)
            {
                _atlas.Update(_glyphs);
                _updated = false;
            }
        }

        [JSFunction(Name = "drawText")]
        public void DrawText(double x, double y, string text)
        {
            DrawText(Program.Batch, x, y, text);
        }

        public void DrawText(SpriteBatch batch, double x, double y, string text)
        {
            CheckUpdate();
            FloatRect dest = new FloatRect((float)x, (float)y, 0, (float)_height);

            for (var i = 0; i < text.Length; ++i)
            {
                IntRect src = _atlas.Sources[text[i]];
                dest.Width = src.Width;
                dest.Height = src.Height;
                batch.Add(_atlas.Texture, src, dest, _color);
                dest.Left += src.Width;
            }
        }

        [JSFunction(Name = "drawZoomedText")]
        public void DrawZoomedText(double x, double y, double zoom, string text)
        {
            CheckUpdate();
            FloatRect dest = new FloatRect((float)x, (float)y, 0, (float)(_height * zoom));

            for (var i = 0; i < text.Length; ++i)
            {
                IntRect src = _atlas.Sources[text[i]];
                dest.Width = (float)(src.Width * zoom);
                dest.Height = src.Height;
                Program.Batch.Add(_atlas.Texture, src, dest, _color);
                dest.Left += (int)(src.Width * zoom);
            }
        }

        [JSFunction(Name = "drawTextBox")]
        public void DrawTextBox(double x, double y, double w, double h, int spacing, string text)
        {
            CheckUpdate();
            ArrayInstance array = Wrap(text, (int)w);
            spacing += (int)_height;
            for (var i = 0; i < array.Length; ++i) {
                DrawText(x, y + i * spacing, (string)array[i]);
            }
        }

        [JSFunction(Name = "setCharacterImage")]
        public void SetCharacterImage(int ch, ImageInstance image)
        {
            _glyphs[ch].Dispose();
            _glyphs[ch] = image.GetImage();
            _updated = true;
        }

        [JSFunction(Name = "getCharacterImage")]
        public ImageInstance GetCharacterImage(int ch)
        {
            return new ImageInstance(Engine, new Texture(_glyphs[ch]), false);
        }

        private StringBuilder word = new StringBuilder(), current = new StringBuilder();

        [JSFunction(Name = "wordWrapString")]
        public ArrayInstance Wrap(string text, int width)
        {
            word.Clear(); current.Clear();
            ArrayInstance array = Program._engine.Array.New();
            int x = 0, word_w = 0;
            int space_w = GetStringWidth(" "), tab_w = GetStringWidth("   ");

            for (var i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                if (c == ' ' || c == '\t')
                {
                    int white_w = (c == ' ') ? space_w : tab_w;
                    string ws = c == ' ' ? " " : "   ";
                    if (x + word_w + white_w > width)
                    {
                        array.Push(current.ToString()); // Push current line...
                        current.Clear().Append(word).Append(ws);
                        x = word_w + white_w;
                    }
                    else
                    {
                        current.Append(word).Append(ws); // ... else add it to current line.
                        x += word_w + white_w;
                    }

                    word.Clear();
                    word_w = 0;
                }
                else if (c == '\n')
                {
                    array.Push(current.Append(word).ToString()); // On \n just push current line.
                    current.Clear();
                    word.Clear();
                    word_w = x = 0;
                }
                else
                {
                    int char_w = _atlas.Sources[c].Width;
                    if (word_w + char_w > width && x == 0) // break up long lines.
                    {
                        array.Push(current.Append(word).ToString());
                        current.Clear();
                        word.Clear();
                        word_w = 0;
                    }
                    else if (x + word_w + char_w > width)
                    {
                        array.Push(current.ToString());
                        current.Clear();
                        x = 0;
                    }

                    word.Append(c);   // Append character ...
                    word_w += char_w; // ... and add to consumed width.
                }
            }
            array.Push(current.Append(word).ToString());
            return array;
        }

        [JSFunction(Name = "getStringWidth")]
        public int GetStringWidth(string text)
        {
            int w = 0;
            for (var i = 0; i < text.Length; ++i)
                w += _atlas.Sources[text[i]].Width;
            return w;
        }

        [JSFunction(Name = "getStringHeight")]
        public int GetStringHeight(string text, int width)
        {
            return (int)(Wrap(text, width).Length * _height);
        }

        [JSFunction(Name = "setColorMask")]
        public void SetColorMask(ColorInstance color)
        {
            _color = color.Color;
        }

        [JSFunction(Name = "getColorMask")]
        public ColorInstance GetColorMask()
        {
            return new ColorInstance(Engine, _color);
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
            FontInstance font = new FontInstance(Engine);
            font._atlas = new TextureAtlas(_atlas.Size);
            font._glyphs = new Image[_glyphs.Length];
            for (var i = 0; i < _glyphs.Length; ++i)
                font._glyphs[i] = new Image(_glyphs[i]);
            font._height = _height;
            font._version = _version;
            font._updated = true;
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

