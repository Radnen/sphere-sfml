using System;
using SFML.Window;

namespace Engine.Objects
{
    public class Person
    {
        SpritesetInstance _innerSS;

        public Person(string name, SpritesetInstance spriteset, bool destroy)
        {
            Name = name;
            DestroyOnMap = destroy;
            Position = new Vector2f(0, 0);
            _innerSS = spriteset;
        }

        public string Name { get; set; }
        public bool Visible { get; set; }
        public bool DestroyOnMap { get; set; }
        public Vector2f Position { get; set; }
    }
}

