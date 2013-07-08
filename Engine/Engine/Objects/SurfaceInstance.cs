using System;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class SurfaceInstance : ObjectInstance
    {
        private RenderWindow _window;
        private Image _image;
        private Texture _tex;
        private bool _changed = false;
        private Sprite _sprite;

        public SurfaceInstance(ObjectInstance proto, int width, int height, Color bg_color, RenderWindow window)
            : base(proto)
        {
            PopulateFunctions();

            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Width must be > 0.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Height must be > 0.");

            _image = new Image((uint)width, (uint)height, bg_color);
            _tex = new Texture(_image);
            _sprite = new Sprite(_tex);
            _window = window;

            DefineProperty("width", new PropertyDescriptor(width, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor(height, PropertyAttributes.Sealed), true);
        }

        public SurfaceInstance(ObjectInstance proto, Image copy, RenderWindow window)
            : base(proto)
        {
            PopulateFunctions();

            _image = new Image(copy);
            _tex = new Texture(_image);
            _sprite = new Sprite(_tex);
            _window = window;

            DefineProperty("width", new PropertyDescriptor((int)_image.Size.X, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor((int)_image.Size.Y, PropertyAttributes.Sealed), true);
        }

        [JSFunction(Name = "blit")]
        public void Blit(double x, double y)
        {
            if (_changed)
                _tex.Update(_image);

            _sprite.Position = new Vector2f((float)x, (float)y);
            _window.Draw(_sprite);
        }

        [JSFunction(Name = "setPixel")]
        public void SetPixel(int x, int y, ColorInstance color)
        {
            _image.SetPixel((uint)x, (uint)y, color.GetColor());
            _changed = true;
        }

        [JSFunction(Name = "getPixel")]
        public ColorInstance GetPixel(int x, int y)
        {
            Color col = _image.GetPixel((uint)x, (uint)y);
            return new ColorInstance(Program._engine.Object.InstancePrototype, col);
        }

        [JSFunction(Name = "createImage")]
        public ImageInstance CreateImage()
        {
            if (_changed)
                _tex.Update(_image);

            return new ImageInstance(Program._engine.Object.InstancePrototype, _tex, _window);
        }

        [JSFunction(Name = "clone")]
        public SurfaceInstance Clone()
        {
            return new SurfaceInstance(Program._engine.Object.InstancePrototype, _image, _window);
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object surface]";
        }
    }
}

