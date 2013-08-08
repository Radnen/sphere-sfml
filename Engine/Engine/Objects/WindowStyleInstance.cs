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

        Texture[] _textures = new Texture[9];
        Sprite[] _sprites = new Sprite[9];
        short _version;
        byte _edgeWidth;
        byte _backgroundMode;
        RGBA[] _edgeColors = new RGBA[4];
        byte[] _edgeOffsets = new byte[4];

        public WindowStyleInstance(ScriptEngine parent)
            : base(parent)
        {
            this["color"] = new ColorInstance(Program._engine);
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
                        for (int i = 0; i < 9; ++i)
                        {
                            short width = reader.ReadInt16();
                            short height = reader.ReadInt16();
                            _textures[i] = new Texture((uint)width, (uint)height);
                            _textures[i].Update(reader.ReadBytes(width * height * 4));
                            _sprites[i] = new Sprite(_textures[i]);
                            _textures[i].Repeated = true;
                        }
                        break;
                }
            }
        }

        [JSFunction(Name = "drawWindow")]
        public void DrawWindow(double dx, double dy, double dwidth, double dheight)
        {
            float x = (float)dx, y = (float)dy;
            int width = (int)dwidth, height = (int)dheight;
            _sprites[0].Position = new Vector2f(x - (float)_textures[0].Size.X, y - (float)_textures[0].Size.Y);
            _sprites[1].Position = new Vector2f(x, y - (float)_textures[1].Size.Y);
            _sprites[1].TextureRect = new IntRect(0, 0, width, (int)_textures[1].Size.Y);
            _sprites[2].Position = new Vector2f(x + width, y - (float)_textures[2].Size.Y);
            _sprites[3].Position = new Vector2f(x + width, y);
            _sprites[3].TextureRect = new IntRect(0, 0, (int)_textures[3].Size.X, height);
            _sprites[4].Position = new Vector2f(x + width, y + height);
            _sprites[5].Position = new Vector2f(x, y + height);
            _sprites[5].TextureRect = new IntRect(0, 0, width, (int)_textures[5].Size.Y);
            _sprites[6].Position = new Vector2f(x - (float)_textures[6].Size.X, y + height);
            _sprites[7].Position = new Vector2f(x - (float)_textures[7].Size.X, y);
            _sprites[7].TextureRect = new IntRect(0, 0, (int)_textures[7].Size.X, height);
            _sprites[8].Position = new Vector2f(x, y);
            _sprites[8].TextureRect = new IntRect(0, 0, width, height);

            foreach (Sprite s in _sprites) { Program._window.Draw(s); }
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
            Color c = color.GetColor();
            foreach (Sprite s in _sprites)
                s.Color = c;
        }

        [JSFunction(Name = "clone")]
        public WindowStyleInstance Clone()
        {
            WindowStyleInstance wind = new WindowStyleInstance(Engine);
            wind["color"] = new ColorInstance(Program._engine, (ColorInstance)this["color"]);
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
            filename = GlobalProps.BasePath + "\\windowstyles\\" + filename;
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filename)))
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

