using System;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class TextureAtlas
    {
        private Image _canvas;
        public uint _size { get; private set; }
        public Texture Texture { get; private set; }
        public IntRect[] Sources { get; private set; }
        private bool _modified = false; 

        public TextureAtlas(uint size)
        {
            _size = size;
            _canvas = new Image(size, size, new Color(0, 0, 0, 0));
            Texture = new Texture(_canvas);
        }

        public TextureAtlas(TextureAtlas copy)
        {
            _size = copy._size;
            _canvas = new Image(copy._canvas);
            Texture = new Texture(_canvas);
            Sources = new IntRect[copy.Sources.Length];
            copy.Sources.CopyTo(Sources, 0);
        }

        public void Update(Image[] sources)
        {
            _canvas.Clear(new Color(0, 0, 0, 0));

            Sources = new IntRect[sources.Length];
            uint x = 0, y = 0, height = 0;

            // construct a packed atlas:
            for (var i = 0; i < sources.Length; ++i)
            {
                uint width = sources[i].Size.X;

                if (x + width >= _size)
                {
                    x = 0;
                    y += height;
                }

                height = Math.Max(height, sources[i].Size.Y);

                Sources[i] = new IntRect((int)x, (int)y, (int)width, (int)height);
                _canvas.Copy(sources[i], x, y);

                x += width;
            }

            Texture.Update(_canvas);
        }

        public Image GetImageAt(uint index)
        {
            Image image = new Image((uint)Sources[index].Width, (uint)Sources[index].Height);
            image.Copy(_canvas, 0, 0, Sources[index]);
            return image;
        }

        public void SetImageAt(uint index, Image img)
        {
            IntRect dest = Sources[index];
            _canvas.Copy(img, (uint)dest.Left, (uint)dest.Top);
            _modified = true;
        }

        public void Update()
        {
            if (_modified)
            {
                Texture.Update(_canvas);
                _modified = false;
            }
        }
    }
}

