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

        Queue<PersonCommand> _commandQueue = new Queue<PersonCommand>();

        string _direction;
        int _frame, _image, _delay, _toNextDelay;

        public Person(string name, SpritesetInstance spriteset, bool destroy)
        {
            Name = name;
            DestroyOnMap = destroy;
            Position = new Vector2f(-1, -1);
            Mask = new Color(255, 255, 255);
            Speed = new Vector2f(1, 1);
            _innerSS = spriteset;

            Data = Program.CreateObject();
            _innerAtlas = new TextureAtlas(1024);
            _base = _innerSS.GetBase();

            ArrayInstance s_images = _innerSS["images"] as ArrayInstance;
            Image[] images = new Image[s_images.Length];
            for (var i = 0; i < s_images.Length; ++i)
                images[i] = ((ImageInstance)s_images[i]).GetImage();

            _innerAtlas.Update(images);
            _sprite = new Sprite(_innerAtlas.Texture);

            Direction = "south";
        }

        public string Name { get; set; }
        public bool Visible { get; set; }
        public bool DestroyOnMap { get; set; }
        public Vector2f Offset { get; set; }
        public Vector2f Position { get; set; }
        public Vector2f Speed { get; set; }
        public Color Mask { get; set; }
        public int FrameRevert { get; set; }
        public int Layer { get; set; }
        public ObjectInstance Data { get; set; }

        public ObjectInstance Base
        {
            get
            {
                return _innerSS.CreateBase();
            }
        }

        public int Frame
        {
            get
            {
                return _frame;
            }
            set
            {
                ObjectInstance dir = GetDirection(_direction);
                ArrayInstance frames = dir["frames"] as ArrayInstance;
                _frame = (int)(value % frames.Length);
                ObjectInstance frame = frames[_frame] as ObjectInstance;
                if (frame != null)
                {
                    _image = (int)frame["index"];
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
                    Frame = Frame;
                }
            }
        }

        private FloatRect GetBounds()
        {
            IntRect b = _innerSS.GetBase();
            float x = b.Left + Position.X;
            float y = b.Top + Position.Y;
            return new FloatRect(x, y, b.Width, b.Height);
        }

        public void CheckObstructions(Layer layer, Tileset tiles)
        {
            FloatRect a = GetBounds();
            Vector2f point = new Vector2f();
            int t = -1;

            int sx = (int)Math.Floor(Position.X / tiles.TileWidth);
            int sy = (int)Math.Floor(Position.Y / tiles.TileHeight);

            for (int x = sx; x < sx+3; ++x)
            {
                point.X = x * 16;
                for (int y = sy; y < sy+3; ++y)
                {
                    if ((t = layer.GetTile(x, y)) < 0)
                        continue;
                    Tile tile = tiles.Tiles[t];
                }
            }
        }

        public void Draw()
        {
            _sprite.TextureRect = _innerAtlas.Sources[_image];
            _sprite.Position = new Vector2f(Position.X - _base.Left + Offset.X, Position.Y - _base.Top + Offset.Y);
            _sprite.Color = Mask;
            Program._window.Draw(_sprite);
        }

        public void QueueCommand(int command, bool imm)
        {
            _commandQueue.Enqueue(new PersonCommand(command, imm));
        }

        public void UpdateCommandQueue()
        {
            bool imm = true;

            while (imm)
            {
                if (_commandQueue.Count == 0)
                    break;

                PersonCommand c = _commandQueue.Dequeue();
                Commands command = (Commands)c.command;
                imm = c.immediate;

                bool update = ((command == Commands.Animate || (int)command > 9) && !imm);

                Vector2f move = new Vector2f();
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
                    case Commands.MoveNorth:
                        move.Y = -1;
                        break;
                    case Commands.MoveEast:
                        move.X = 1;
                        break;
                    case Commands.MoveSouth:
                        move.Y = 1;
                        break;
                    case Commands.MoveWest:
                        move.X = -1;
                        break;
                }

                move.X *= Speed.X;
                move.Y *= Speed.Y;
                Position += move;

                _toNextDelay += (update ? 1 : 0);
                if (_toNextDelay == _delay)
                {
                    Frame = Frame + 1;
                    _toNextDelay = 0;
                }
            }
        }

        public bool IsQueueEmpty()
        {
            return _commandQueue.Count == 0;
        }

        public void ClearComands()
        {
            _commandQueue.Clear();
        }

        private ObjectInstance GetDirection(string name)
        {
            ArrayInstance directions = _innerSS["directions"] as ArrayInstance;
            for (int i = 0; i < directions.Length; ++i)
            {
                ObjectInstance dir = directions[i] as ObjectInstance;
                if (dir["name"].Equals(name))
                    return dir;
            }
            return null;
        }
    }
}

