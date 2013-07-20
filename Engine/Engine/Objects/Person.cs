using System;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class Person
    {
        SpritesetInstance _innerSS;
        TextureAtlas _innerAtlas;
        Sprite _sprite;
        IntRect _base;

        public Person(string name, SpritesetInstance spriteset, bool destroy)
        {
            Name = name;
            DestroyOnMap = destroy;
            Position = new Vector2f(0, 0);
            _innerSS = spriteset;
            _innerAtlas = new TextureAtlas(1024);
            _base = _innerSS.GetBase();

            ArrayInstance s_images = _innerSS["images"] as ArrayInstance;
            Image[] images = new Image[s_images.Length];
            for (var i = 0; i < s_images.Length; ++i)
                images[i] = ((ImageInstance)s_images[i]).GetImage(false);

            _innerAtlas.Update(images);
            _sprite = new Sprite(_innerAtlas.Texture);
        }

        public string Name { get; set; }
        public string Direction { get; set; }
        public bool Visible { get; set; }
        public bool DestroyOnMap { get; set; }
        public Vector2f Position { get; set; }

        public void Draw(int x, int y)
        {
            _sprite.TextureRect = _innerAtlas.Sources[0];
            _sprite.Position = new Vector2f(x - _base.Left, y - _base.Top);
            Program._window.Draw(_sprite);
        }
    }
}

