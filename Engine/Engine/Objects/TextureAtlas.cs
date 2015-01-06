using System;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class TextureAtlas
    {
        private Image _canvas;
        private bool _modified;

        public uint Size { get; private set; }
        public Texture Texture { get; private set; }
        public IntRect[] Sources { get; private set; }

        public TextureAtlas(uint size)
        {
            Size = size;
            _canvas = new Image(size, size, new Color(0, 0, 0, 0));
            Texture = new Texture(_canvas);
        }

        public TextureAtlas(TextureAtlas copy)
        {
            Size = copy.Size;
            _canvas = new Image(copy._canvas);
            Texture = new Texture(_canvas);
            Sources = new IntRect[copy.Sources.Length];
            copy.Sources.CopyTo(Sources, 0);
        }

        /// <summary>
        /// Packs the images in the image list into a single texture.
        /// </summary>
        /// <param name="sources">Sources.</param>
        public void Update(Image[] images, bool clear = false)
        {
            if (clear)
                _canvas.Clear(new Color(0, 0, 0, 0));

            Sources = new IntRect[images.Length];
            uint x = 0, y = 0, height = 0;

            // construct a packed atlas:
            for (var i = 0; i < images.Length; ++i)
            {
                uint width = images[i].Size.X;

                if (x + width >= Size)
                {
                    x = 0;
                    y += height;
                }

                height = Math.Max(height, images[i].Size.Y);

                Sources[i] = new IntRect((int)x, (int)y, (int)width, (int)height);
                _canvas.Copy(images[i], x, y);

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

        /// <summary>
        /// This will clip the image, replacing the atlas graphic.
        /// In order to change the physical bounds, see <see cref="update(image[])"/>.
        /// </summary>
        public void SetImageAt(uint index, Image img)
        {
            IntRect dest = Sources[index];
            int width = Math.Min(dest.Width, (int)img.Size.X);
            int height = Math.Min(dest.Height, (int)img.Size.Y);
            IntRect sourceRect = new IntRect(0, 0, width, height);
            _canvas.Copy(img, (uint)dest.Left, (uint)dest.Top, sourceRect);
            _modified = true;
        }

        public void Refresh()
        {
            if (_modified)
            {
                Texture.Update(_canvas);
                _modified = false;
            }
        }
    }
}

