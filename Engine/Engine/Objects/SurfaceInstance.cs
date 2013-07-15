using System;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class SurfaceInstance : ObjectInstance
    {
        private Image _image;
        private Texture _tex;
        private bool _changed;
        private Sprite _sprite;

        public SurfaceInstance(ObjectInstance proto, int width, int height, Color bg_color)
            : base(proto)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Width must be > 0.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Height must be > 0.");

            _image = new Image((uint)width, (uint)height, bg_color);
            Init();
        }

        public SurfaceInstance(ObjectInstance proto, Image copy, bool clone = true)
            : base(proto)
        {
            _image = (clone) ? new Image(copy) : copy;
            Init();
        }

        public SurfaceInstance(ObjectInstance proto, string filename)
            : base(proto)
        {
            _image = new Image(filename);
            Init();
        }

        private void Init() {
            PopulateFunctions();
            _tex = new Texture(_image);
            _sprite = new Sprite(_tex);

            DefineProperty("width", new PropertyDescriptor((int)_image.Size.X, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor((int)_image.Size.Y, PropertyAttributes.Sealed), true);
        }

        public Image GetImageRef()
        {
            return _image;
        }

        [JSFunction(Name = "blit")]
        public void Blit(double x, double y)
        {
            if (_changed)
                _tex.Update(_image);

            _sprite.Position = new Vector2f((float)x, (float)y);
            Program._window.Draw(_sprite);
        }

        [JSFunction(Name = "blitSurface")]
        public void BlitSurface(int x, int y, SurfaceInstance surf)
        {
            _image.Copy(surf.GetImageRef(), (uint)x, (uint)y);
            _changed = true;
        }

        [JSFunction(Name = "setPixel")]
        public void SetPixel(int x, int y, ColorInstance color)
        {
            _image.SetPixel((uint)x, (uint)y, color.GetColor());
            _changed = true;
        }

        [JSFunction(Name = "replaceColor")]
        public void ReplaceColor(ColorInstance colorA, ColorInstance colorB)
        {
            _image.ReplaceColor(colorA.GetColor(), colorB.GetColor());
            _changed = true;
        }

        [JSFunction(Name = "resize")]
        public void Resize(int width, int height)
        {
            Image copy = new Image((uint)width, (uint)height, new Color(0, 0, 0, 0));
            copy.Copy(_image, 0, 0, new IntRect(0, 0, Math.Min(width, (int)_image.Size.X), Math.Min(height, (int)_image.Size.Y)));
            _image.Dispose();
            _image = copy;
            _changed = true;
        }

        [JSFunction(Name = "getPixel")]
        public ColorInstance GetPixel(int x, int y)
        {
            Color col = _image.GetPixel((uint)x, (uint)y);
            return new ColorInstance(Program._engine.Object.InstancePrototype, col);
        }

        [JSFunction(Name = "flipHorizontally")]
        public void FlipHorizontally()
        {
            _image.FlipHorizontally();
            _changed = true;
        }

        [JSFunction(Name = "flipVertically")]
        public void FlipVertically()
        {
            _image.FlipVertically();
            _changed = true;
        }

        [JSFunction(Name = "createImage")]
        public ImageInstance CreateImage()
        {
            if (_changed)
                _tex.Update(_image);

            return new ImageInstance(Program._engine.Object.InstancePrototype, _tex);
        }

        [JSFunction(Name = "clone")]
        public SurfaceInstance Clone()
        {
            return new SurfaceInstance(Program._engine.Object.InstancePrototype, _image);
        }

        [JSFunction(Name = "cloneSection")]
        public SurfaceInstance CloneSection(int x, int y, int w, int h)
        {
            using (Image image = new Image((uint)w, (uint)h))
            {
                image.Copy(_image, 0, 0, new IntRect(x, y, w, h));
                return new SurfaceInstance(Program._engine.Object.InstancePrototype, image);
            }
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object surface]";
        }
    }
}

