using System;
using System.Collections.Generic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.System;

namespace Engine.Objects
{
    public class Person
    {
#if(DEBUG)
        private static ColorInstance _debug1, _debug2;
#endif

        SpritesetInstance _innerSS;
        Sprite _sprite;
        IntRect _base;

        Queue<PersonCommand> _commandQueue = new Queue<PersonCommand>();

        string _direction;
        int _frame, _image, _delay, _toNextDelay, _revert;

        public Person(string name, SpritesetInstance spriteset, bool destroy)
        {
#if(DEBUG)
            if (_debug1 == null)
                _debug1 = new ColorInstance(Program._engine, Color.Magenta);
            if (_debug2 == null)
                _debug2 = new ColorInstance(Program._engine, Color.Red);
#endif

            Name = name;
            DestroyOnMap = destroy;
            Speed = new Vector2f(1, 1);
            _innerSS = spriteset;

            Data = Program.CreateObject();
            _base = _innerSS.GetBase();

            _sprite = new Sprite(_innerSS.TextureAtlas.Texture);
            Mask = new Color(255, 255, 255);

            Scripts = new FunctionScript[5];

            string d = GetDirectionAt(0);
            if (!String.IsNullOrEmpty(d))
                Direction = d;
        }

        public string Name { get; set; }
        public bool Visible { get; set; }
        public bool DestroyOnMap { get; set; }
        public Vector2f Offset { get; set; }
        public Vector2f Position { get; set; }
        public Vector2f Speed { get; set; }
        public bool IgnorePersons { get; set; }
        public bool IgnoreTiles { get; set; }

        public Color Mask
        {
            get { return _sprite.Color; }
            set { _sprite.Color = value; }
        }

        public int FrameRevert { get; set; }
        public int Layer { get; set; }
        public ObjectInstance Data { get; set; }
        public FunctionScript[] Scripts { get; private set; }

        public void CallScript(PersonScripts script)
        {
            FunctionScript code = Scripts[(int)script];
            if (code != null) code.Execute();
        }

        public void SetScript(PersonScripts script, object instance)
        {
            if (instance == null || instance.ToString() == "")
                Scripts[(int)script] = null;
            else
                Scripts[(int)script] = new FunctionScript(instance);
        }

        public ObjectInstance Base
        {
            get
            {
                return _innerSS.CreateBase();
            }
        }

        public SpritesetInstance Spriteset
        {
            get { return _innerSS; }
            set
            {
                _innerSS = value;
                _base = _innerSS.GetBase();
                _sprite = new Sprite(_innerSS.TextureAtlas.Texture);
                Direction = Direction;
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
                _image = (int)frame["index"];
                _delay = (int)frame["delay"];
                _sprite.TextureRect = _innerSS.TextureAtlas.Sources[_image];
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
                    Frame = Frame; // trick to readjust the frame. ;)
                }
            }
        }

        public Line[] GetBounds()
        {
            return _innerSS.GetLineBase();
        }

        public bool CheckObstructions(ref Vector2f pos, ref Vector2f tileOffset, Tile tile)
        {
            Line[] baselines = _innerSS.GetLineBase();
            Vector2f my_pos = pos - new Vector2f(_base.Left, _base.Top);
            foreach (Line b in baselines)
            {
                var lineA = b.Offset(my_pos);
                foreach (Line l in tile.Obstructions)
                {
                    var lineB = l.Offset(tileOffset);
                    if (Line.Intersects(lineA, lineB))
                        return true;
                }
            }
            return false;
        }

        public bool CheckObstructions(ref Vector2f pos, Person other)
        {
            if (other.IgnorePersons || other.Layer != Layer)
                return false;

            Line[] baselines1 = _innerSS.GetLineBase();
            Line[] baselines2 = other._innerSS.GetLineBase();
            Vector2f pos1 = pos - new Vector2f(_base.Left, _base.Top);
            Vector2f pos2 = other.Position - new Vector2f(other._base.Left, other._base.Top);

            foreach (Line l1 in baselines1)
            {
                Line A = l1.Offset(pos1);
                foreach (Line l2 in baselines2)
                {
                    if (Line.Intersects(A, l2.Offset(pos2)))
                        return true;
                }
            }

            return false;
        }

        public bool CheckObstructions(ref Vector2f position, Line line)
        {
            Line[] baselines = _innerSS.GetLineBase();
            Vector2f pos = position - new Vector2f(_base.Left, _base.Top);
            foreach (Line b in baselines)
            {
                var baseline = b.Offset(pos);
                if (Line.Intersects(baseline, line))
                    return true;
            }
            return false;
        }

        public void Draw()
        {
            _sprite.Position = new Vector2f(Position.X - _base.Left + Offset.X, Position.Y - _base.Top + Offset.Y);
            Program._window.Draw(_sprite);
#if(DEBUG)
            Line[] lines = _innerSS.GetLineBase();
            double x = Position.X;
            double y = Position.Y;
            double w = BaseWidth + 1;
            double h = BaseHeight + 1;

            GlobalPrimitives.OutlinedRectangle(((int)(x+lines[0].Start.X/2)/16)*16, ((int)(y+ lines[0].Start.Y/2)/16)*16, w, h, _debug2);
            GlobalPrimitives.OutlinedRectangle(x, y, w, h, _debug1);
#endif
        }

        public int BaseWidth { get { return _base.Width; } }

        public int BaseHeight { get { return _base.Height; } }

        public void QueueCommand(int command, bool imm)
        {
            _commandQueue.Enqueue(new PersonCommand(command, imm));
        }

        public void QueueScript(object script, bool imm)
        {
            _commandQueue.Enqueue(new PersonCommand((int)Commands.Wait, imm, script));
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

                if (c.script != null) {
                    PersonManager.CurrentPerson = Name;
                    c.script.Execute();
                    continue;
                }

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
                        move.Y = -Speed.Y;
                        break;
                    case Commands.MoveEast:
                        move.X = Speed.X;
                        break;
                    case Commands.MoveSouth:
                        move.Y = Speed.Y;
                        break;
                    case Commands.MoveWest:
                        move.X = -Speed.X;
                        break;
                }

                Position += move;
                if (IsObstructedAt(Position))
                {
                    Position -= move;
                }

                _revert = 0;
                _toNextDelay += (update ? 1 : 0);
                if (_toNextDelay >= _delay)
                {
                    Frame = Frame + 1;
                    _toNextDelay = 0;
                }
            }

            if (FrameRevert > 0) {
                _revert++;
                if (_revert >= FrameRevert) {
                    Frame = 0;
                    _revert = 0;
                }
            }
        }

        public bool IsObstructedAt(Vector2f pos)
        {
            return (!IgnoreTiles && MapEngineHandler.CheckTileObstruction(ref pos, this)) ||
                (!IgnorePersons && PersonManager.CheckPersonObstructions(ref pos, this)) ||
                (MapEngineHandler.CheckLineObstruction(ref pos, this));
        }

        public bool IsQueueEmpty()
        {
            return _commandQueue.Count == 0;
        }

        public void ClearComands()
        {
            _commandQueue.Clear();
        }

        private string GetDirectionAt(int index)
        {
            ArrayInstance directions = _innerSS["directions"] as ArrayInstance;
            if (index < 0 || index >= directions.Length)
                return "";
            ObjectInstance dir = directions[index] as ObjectInstance;
            if (dir != null)
                return (string)dir["name"];
            else
                return "";
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

