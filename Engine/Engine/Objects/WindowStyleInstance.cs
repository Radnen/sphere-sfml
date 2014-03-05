using System;
using System.IO;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class WindowStyleInstance : ObjectInstance
    {
        /// <summary>
        /// nested class for working with colors
        /// </summary>
        private class RGBA
        {
            Color _rgba;

            public RGBA(BinaryReader reader)
            {
                ReadData(reader);
            }

            public void ReadData(BinaryReader reader)
            {
                _rgba = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            }

            public void SaveData(BinaryWriter writer)
            {
                writer.Write(new byte[4] { _rgba.R, _rgba.G, _rgba.B, _rgba.A });
            }
        }

        Texture[] _textures = new Texture[5];
        Sprite[] _sprites = new Sprite[5];
        short _version;
        byte _edgeWidth;
        byte _backgroundMode;
        RGBA[] _edgeColors = new RGBA[4];
        byte[] _edgeOffsets = new byte[4];
        Color _color = Color.White;
        TextureAtlas _atlas = new TextureAtlas(64); // find optimal value

        public WindowStyleInstance(ScriptEngine parent)
            : base(parent.Object.InstancePrototype)
        {
            this["color"] = new ColorInstance(Engine, Color.White);
            PopulateFunctions();
        }

        public WindowStyleInstance(ScriptEngine parent, string filename)
            : this(parent)
        {
            ReadFromFile(filename);
        }

        private void ReadFromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException();

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                if (!new String(reader.ReadChars(4)).Equals(".rws"))
                    throw new WindowStyleException(filename, "Invalid file header.");
                _version = reader.ReadInt16();
                _edgeWidth = reader.ReadByte();
                _backgroundMode = reader.ReadByte();
                for (int i = 0; i < _edgeColors.Length; ++i) {
                    _edgeColors[i] = new RGBA(reader);
                }
                _edgeOffsets = reader.ReadBytes(4);
                reader.ReadBytes(36); // reserved

                switch (_version)
                {
                    case 2:
                        Image[] images = new Image[4];
                        for (int i = 0; i < 9; ++i)
                        {
                            int width = reader.ReadInt16();
                            int height = reader.ReadInt16();
                            if (i < 7 && i % 2 == 0)
                                images[i / 2] = new Image((uint)width, (uint)height, reader.ReadBytes(width * height * 4));
                            else
                            {
                                int index = (i == 8 ? i : i - 1) / 2;
                                _textures[index] = new Texture((uint)width, (uint)height);
                                _textures[index].Update(reader.ReadBytes(width * height * 4));
                                _sprites[index] = new Sprite(_textures[index]);
                                _textures[index].Repeated = true;
                            }
                        }
                        _atlas.Update(images);
                        break;
                }
            }
        }

        [JSFunction(Name = "drawWindow")]
        public void DrawWindow(double dx, double dy, double dwidth, double dheight)
        {
            float x = (float)dx, y = (float)dy;
            int width = (int)dwidth, height = (int)dheight;

            // sprite-batch the corners:
            AddTileC(0, x - _atlas.Sources[0].Width, y - _atlas.Sources[0].Height);
            AddTileC(1, x + width, y - _atlas.Sources[1].Height);
            AddTileC(2, x + width, y + height);
            AddTileC(3, x - _atlas.Sources[3].Width, y + height);
            Program.Batch.Flush();

            // then use u, v repeating on the rest (non-batched):
            IntRect clip = new IntRect(0, 0, width, height);

            _sprites[4].Position = new Vector2f(x, y);
            _sprites[4].TextureRect = clip;

            _sprites[0].Position = new Vector2f(x, y - (float)_textures[0].Size.Y);
            clip.Height = (int)_textures[0].Size.Y;
            _sprites[0].TextureRect = clip;
            clip.Height = (int)_textures[2].Size.Y;
            _sprites[2].Position = new Vector2f(x, y + height);
            _sprites[2].TextureRect = clip;

            clip.Height = height;
            clip.Width = (int)_textures[1].Size.X;
            _sprites[1].Position = new Vector2f(x + width, y);
            _sprites[1].TextureRect = clip;
            _sprites[3].Position = new Vector2f(x - (float)_textures[3].Size.X, y);
            clip.Width = (int)_textures[3].Size.X;
            _sprites[3].TextureRect = clip;

            Program._window.Draw(_sprites[0]);
            Program._window.Draw(_sprites[1]);
            Program._window.Draw(_sprites[2]);
            Program._window.Draw(_sprites[3]);
            Program._window.Draw(_sprites[4]);
        }

        public void AddTileC(int i, float x, float y)
        {
            IntRect source = _atlas.Sources[i];
            FloatRect dest = new FloatRect(x, y, source.Width, source.Height);
            Program.Batch.Add(_atlas.Texture, source, dest, _color);
        }

        [JSFunction(Name = "getColorMask")]
        public ColorInstance GetColorMask()
        {
            return this["color"] as ColorInstance;
        }

        [JSFunction(Name = "setColorMask")]
        public void SetColorMask(ColorInstance color)
        {
            this["color"] = color;
            _color = color.GetColor();
            foreach (Sprite s in _sprites) s.Color = _color;
        }

        [JSFunction(Name = "clone")]
        public WindowStyleInstance Clone()
        {
            WindowStyleInstance wind = new WindowStyleInstance(Engine);
            wind["color"] = new ColorInstance(Engine, (ColorInstance)this["color"]);
            wind._backgroundMode = _backgroundMode;
            _edgeColors.CopyTo(wind._edgeColors, 0);
            _edgeOffsets.CopyTo(wind._edgeOffsets, 0);
            wind._edgeWidth = _edgeWidth;
            for (var i = 0; i < _textures.Length; ++i)
            {
                wind._textures[i] = new Texture(_textures[i]);
                wind._sprites[i] = new Sprite(wind._textures[i]);
            }
            wind._version = _version;
            return wind;
        }

        [JSFunction(Name = "save")]
        public void Save(string filename)
        {
            string root = GlobalProps.BasePath + "/windowstyles";
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(root + "/" + filename)))
            {
                writer.Write(".rws".ToCharArray());
                writer.Write(_version);
                writer.Write(_edgeWidth);
                writer.Write(_backgroundMode);
                foreach (RGBA col in _edgeColors)
                    col.SaveData(writer);
                writer.Write(_edgeOffsets);
                writer.Write(new Byte[36]);

                foreach (Texture tex in _textures)
                {
                    writer.Write((short)tex.Size.X);
                    writer.Write((short)tex.Size.Y);
                    using (Image img = tex.CopyToImage())
                    {
                        writer.Write(img.Pixels);
                    }
                }
            }
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object windowstyle]";
        }
    }

    public class WindowStyleException : Exception
    {
        public WindowStyleException(string filename, string msg)
            : base(string.Format("Invalid WindowStyle at {0}: {1}", filename, msg))
        {
        }
    }
}

