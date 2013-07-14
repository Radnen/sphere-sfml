﻿using System;
using SFML.Window;
using SFML.Graphics;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    public class ImageInstance : ObjectInstance
    {
        private static readonly Vector2f VECT_1 = new Vector2f(1, 1);
        private static readonly Vector2f VECT_0 = new Vector2f(0, 0);

        private Texture _image;
        private Sprite _sprite;
        private RectangleShape _shape = new RectangleShape();
        private RenderStates _state = new RenderStates();

		public ImageInstance(ObjectInstance proto, string filename)
            : base(proto)
        {
            _image = new Texture(filename);
            _sprite = new Sprite(_image);
            PopulateFunctions();
            
            _state.Texture = _image;
            _state.BlendMode = BlendMode.Alpha;
            _state.Transform = Transform.Identity;

            DefineProperty("width", new PropertyDescriptor(_image.Size.X, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor(_image.Size.Y, PropertyAttributes.Sealed), true);
        }

        public ImageInstance(ObjectInstance proto, Texture copy)
            : base(proto)
        {
            _image = new Texture(copy);
            _sprite = new Sprite(_image);
            PopulateFunctions();
            
            _state.Texture = _image;
            _state.BlendMode = BlendMode.Alpha;
            _state.Transform = Transform.Identity;
            
            DefineProperty("width", new PropertyDescriptor(_image.Size.X, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor(_image.Size.Y, PropertyAttributes.Sealed), true);
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object image]";
        }

        [JSFunction(Name = "save")]
        public void Save(string filename)
        {
            _image.CopyToImage().SaveToFile(GlobalProps.BasePath + "\\images\\" + filename);
        }

        [JSFunction(Name = "blit")]
        public void Blit(double x, double y, int mode)
        {
			_sprite.Position = new Vector2f((float)x, (float)y);
            _sprite.Color = Color.White;
            _sprite.Scale = VECT_1;
            _sprite.Rotation = 0;
            _sprite.Origin = VECT_0;

            BlendMode before = _state.BlendMode;
            switch (mode)
            {
                case 0:
                    _state.BlendMode = BlendMode.Alpha;
                    break;
                case 4:
                    _state.BlendMode = BlendMode.Add;
                    break;
                case 6:
                    _state.BlendMode = BlendMode.Multiply;
                    break;
            }

            Program._window.Draw(_sprite, _state);
            _state.BlendMode = before;
        }

        [JSFunction(Name = "blitMask")]
		public void BlitMask(double x, double y, ColorInstance color)
		{
			_sprite.Position = new Vector2f((float)x, (float)y);
            _sprite.Color = color.GetColor();
            _sprite.Scale = VECT_1;
            _sprite.Rotation = 0;
            _sprite.Origin = VECT_0;

            Program._window.Draw(_sprite);
		}

        [JSFunction(Name = "zoomBlit")]
        public void ZoomBlit(double x, double y, double z) {
            _sprite.Position = new Vector2f((float)x, (float)y);
            _sprite.Color = Color.White;
            _sprite.Scale = new Vector2f((float)z, (float)z);
            _sprite.Rotation = 0;
            _sprite.Origin = VECT_0;

            Program._window.Draw(_sprite);
        }

        [JSFunction(Name = "zoomBlitMask")]
        public void ZoomBlitMask(double x, double y, double z, ColorInstance color) {
            _sprite.Position = new Vector2f((float)x, (float)y);
            _sprite.Color = color.GetColor();
            _sprite.Scale = new Vector2f((float)z, (float)z);
            _sprite.Rotation = 0;
            _sprite.Origin = VECT_0;

            Program._window.Draw(_sprite);
        }

        [JSFunction(Name = "rotateBlit")]
        public void RotateBlit(double x, double y, double r) {
            _sprite.Position = new Vector2f((float)(x + _image.Size.X / 2), (float)(y + _image.Size.Y / 2));
            _sprite.Color = Color.White;
            _sprite.Scale = VECT_1;
            _sprite.Rotation = (float)(r / Math.PI) * 180.0f;
            _sprite.Origin = new Vector2f((float)_image.Size.X / 2, (float)_image.Size.Y / 2);

            Program._window.Draw(_sprite);
        }

        [JSFunction(Name = "rotateBlitMask")]
        public void RotateBlitMask(double x, double y, double r, ColorInstance color) {
            _sprite.Position = new Vector2f((float)(x + _image.Size.X / 2), (float)(y + _image.Size.Y / 2));
            _sprite.Color = color.GetColor();
            _sprite.Scale = VECT_1;
            _sprite.Rotation = (float)(r / Math.PI) * 180.0f;
            _sprite.Origin = new Vector2f((float)_image.Size.X / 2, (float)_image.Size.Y / 2);

            Program._window.Draw(_sprite);
        }

        [JSFunction(Name = "transformBlit")]
        public void TransformBlit(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            Vertex[] array = new Vertex[4];

            array[0] = new Vertex(new Vector2f((float)x1, (float)y1), new Vector2f(0, 0));
            array[1] = new Vertex(new Vector2f((float)x2, (float)y2), new Vector2f((float)_image.Size.X, 0));
            array[2] = new Vertex(new Vector2f((float)x3, (float)y3), new Vector2f((float)_image.Size.X, (float)_image.Size.Y));
            array[3] = new Vertex(new Vector2f((float)x4, (float)y4), new Vector2f(0, (float)_image.Size.Y));

            Program._window.Draw(array, PrimitiveType.Quads, _state);
        }

        [JSFunction(Name = "transformBlitMask")]
        public void TransformBlitMask(ObjectInstance ul, ObjectInstance ur, ObjectInstance lr, ObjectInstance ll, ColorInstance color)
        {
            Vertex[] array = new Vertex[4];
            Color c = color.GetColor();

            array[0] = new Vertex(GlobalPrimitives.GetVector(ul), c, new Vector2f(0, 0));
            array[1] = new Vertex(GlobalPrimitives.GetVector(ur), c, new Vector2f((float)_image.Size.X, 0));
            array[2] = new Vertex(GlobalPrimitives.GetVector(lr), c, new Vector2f((float)_image.Size.X, (float)_image.Size.Y));
            array[3] = new Vertex(GlobalPrimitives.GetVector(ll), c, new Vector2f(0, (float)_image.Size.Y));

            Program._window.Draw(array, PrimitiveType.Quads, _state);
        }

        [JSFunction(Name = "createSurface")]
        public SurfaceInstance CreateSurface()
        {
            return new SurfaceInstance(Program._engine.Object.InstancePrototype, _image.CopyToImage());
        }

        [JSFunction(Name = "clone")]
        public ImageInstance Clone()
        {
            return new ImageInstance(Program._engine.Object.InstancePrototype, _image);
        }
    }
}
