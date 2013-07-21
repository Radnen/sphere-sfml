using System;
using System.Collections.Generic;
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

        Queue<int> _commandQueue = new Queue<int>();

        string _direction;
        int _frame, _image, _delay, _toNextDelay;

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

            Direction = "south";
        }

        public string Name { get; set; }
        public bool Visible { get; set; }
        public bool DestroyOnMap { get; set; }
        public Vector2f Position { get; set; }

        public int Frame
        {
            get
            {
                return _frame;
            }
            set
            {
                _frame = value;
                ObjectInstance dir = GetDirection(_direction);
                ArrayInstance frames = dir["frames"] as ArrayInstance;
                ObjectInstance frame = frames[value] as ObjectInstance;
                if (frame != null)
                {
                    _image = (int)((int)frame["index"] % frames.Length);
                    _delay = (int)frame["delay"];
                }
            }
        }

        public string Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                ObjectInstance direction = GetDirection(value);
                if (direction != null)
                {
                    _direction = value;
                    Frame = (int)((ArrayInstance)direction["frames"])[0];
                }
            }
        }

        public void Draw(int x, int y)
        {
            _sprite.TextureRect = _innerAtlas.Sources[_image];
            _sprite.Position = new Vector2f(x - _base.Left, y - _base.Top);
            Program._window.Draw(_sprite);
        }

        public void QueueCommand(Commands command)
        {
            _commandQueue.Enqueue((int)command);
        }

        public void UpdateCommandQueue()
        {
            Commands command = (Commands)_commandQueue.Dequeue();
            bool update = (command == Commands.Animate || (int)command > 9);

            switch (command)
            {
                case Commands.FaceNorth:
                    Direction = "north";
                    break;
                case Commands.FaceEast:
                    Direction = "east";
                    break;
                case Commands.FaceSouth:
                    Direction = "south";
                    break;
                case Commands.FaceWest:
                    Direction = "west";
                    break;
            }

            _toNextDelay += (update ? 1 : 0);
            if (_toNextDelay == _delay)
            {
                Frame = Frame + 1;
                _toNextDelay = 0;
            }
        }

        private ObjectInstance GetDirection(string name)
        {
            ArrayInstance directions = _innerSS["directions"] as ArrayInstance;
            for (var i = 0; i < directions.Length; ++i) {
                if ((string)((ObjectInstance)directions[i])["name"] == name)
                    return directions[i] as ObjectInstance;
            }
            return null;
        }
    }
}

