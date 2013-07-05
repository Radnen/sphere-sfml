using System;
using SFML.Window;
using SFML.Graphics;

namespace Engine.Objects
{
    public class ImageWrapper
    {
        private Texture _image;
		private Sprite _sprite;
		private RenderWindow _parent_window;

		public ImageWrapper(string filename, RenderWindow parent)
		{
			_image = new Texture(GlobalProps.BasePath + "\\images\\" + filename);
            _sprite = new Sprite(_image);
			_parent_window = parent;
		}

        public void blit(float x, float y)
        {
			_sprite.Position = new Vector2f(x, y);
            _parent_window.Draw(_sprite);
        }

		public void blitMask(float x, float y, Color color)
		{
			_sprite.Position = new Vector2f(x, y);
			_sprite.Color = color;
			_parent_window.Draw(_sprite);
		}
    }
}
