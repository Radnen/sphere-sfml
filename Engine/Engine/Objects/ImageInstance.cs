using System;
using SFML.Window;
using SFML.Graphics;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    public class ImageConstructor : ClrFunction
    {
        private RenderWindow _window;

        public ImageConstructor(ScriptEngine engine, RenderWindow window)
            : base(engine.Function.InstancePrototype, "Image", new ImageInstance(engine.Object.InstancePrototype))
        {
            _window = window;
        }

        [JSConstructorFunction]
        public ImageInstance Construct(string filename)
        {
            return new ImageInstance(InstancePrototype, filename, _window);
        }
    }

    public class ImageInstance : ObjectInstance
    {
        private Texture _image;
		private Sprite _sprite;
		private RenderWindow _parent_window;

        public ImageInstance(ObjectInstance proto)
            : base(proto)
        {
        }

		public ImageInstance(ObjectInstance proto, string filename, RenderWindow parent)
            : base(proto)
		{
            _image = new Texture(GlobalProps.BasePath + "\\images\\" + filename);
            _sprite = new Sprite(_image);
            _parent_window = parent;
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
            _sprite.Scale = new Vector2f(1, 1);
            _parent_window.Draw(_sprite);
        }

        [JSFunction(Name = "blitMask")]
		public void BlitMask(double x, double y, ColorInstance color)
		{
			_sprite.Position = new Vector2f((float)x, (float)y);
            _sprite.Color = color.GetColor();
			_parent_window.Draw(_sprite);
		}

        [JSFunction(Name = "zoomBlit")]
        public void zoomBlit(double x, double y, double z) {
            _sprite.Position = new Vector2f((float)x, (float)y);
            _sprite.Scale = new Vector2f((float)z, (float)z);
            _parent_window.Draw(_sprite);
        }

        // TODO: implement in GL
        [JSFunction(Name = "transformBlit")]
        public void TransformBlit(double x1, double y1, double x2, double y2)
        {
            //_sprite.Transform.(new FloatRect(x1, x2, (x2 - x1), (y2 - y1)));
            _parent_window.Draw(_sprite);
        }
    }
}
