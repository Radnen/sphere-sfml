using System;
using SFML.Window;
using SFML.Graphics;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    public class ImageInstance : ObjectInstance
    {
        private Texture _image;
		private Sprite _sprite;
		private RenderWindow _window;

		public ImageInstance(ObjectInstance proto, string filename, RenderWindow parent)
            : base(proto)
		{
            _image = new Texture(GlobalProps.BasePath + "\\images\\" + filename);
            _sprite = new Sprite(_image);
            _window = parent;
            PopulateFunctions();
            DefineProperty("width", new PropertyDescriptor(_image.Size.X, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor(_image.Size.Y, PropertyAttributes.Sealed), true);
		}

        public ImageInstance(ObjectInstance proto, Texture copy, RenderWindow parent)
            : base(proto)
        {
            _image = new Texture(copy);
            _sprite = new Sprite(_image);
            _window = parent;
            PopulateFunctions();
            DefineProperty("width", new PropertyDescriptor(_image.Size.X, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor(_image.Size.Y, PropertyAttributes.Sealed), true);
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object image]";
        }

        [JSFunction(Name = "blit")]
        public void Blit(double x, double y)
        {
			_sprite.Position = new Vector2f((float)x, (float)y);
            _sprite.Color = Color.White;
            _sprite.Scale = new Vector2f(1, 1);
            _sprite.Rotation = 0;
            _window.Draw(_sprite);
        }

        [JSFunction(Name = "blitMask")]
		public void BlitMask(double x, double y, ColorInstance color)
		{
			_sprite.Position = new Vector2f((float)x, (float)y);
            _sprite.Color = color.GetColor();
            _sprite.Scale = new Vector2f(1, 1);
            _sprite.Rotation = 0;
			_window.Draw(_sprite);
		}

        [JSFunction(Name = "zoomBlit")]
        public void ZoomBlit(double x, double y, double z) {
            _sprite.Position = new Vector2f((float)x, (float)y);
            _sprite.Color = Color.White;
            _sprite.Scale = new Vector2f((float)z, (float)z);
            _sprite.Rotation = 0;
            _window.Draw(_sprite);
        }

        [JSFunction(Name = "rotateBlit")]
        public void RotateBlit(double x, double y, double r) {
            _sprite.Position = new Vector2f((float)x, (float)y);
            _sprite.Color = Color.White;
            _sprite.Scale = new Vector2f(1, 1);
            _sprite.Rotation = (float)(r / Math.PI) * 180.0f;
            _window.Draw(_sprite);
        }

        [JSFunction(Name = "createSurface")]
        public SurfaceInstance CreateSurface()
        {
            return new SurfaceInstance(Program._engine.Object.InstancePrototype, _image.CopyToImage(), _window);
        }

        [JSFunction(Name = "clone")]
        public ImageInstance Clone()
        {
            return new ImageInstance(Program._engine.Object.InstancePrototype, _image, _window);
        }
    }
}
