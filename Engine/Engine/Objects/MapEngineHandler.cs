using System;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;

namespace Engine.Objects
{
    public static class MapEngineHandler
    {
        private static Map _map;
        private static TextureAtlas _tileatlas;
        private static bool _ended = false, _toggled;
        private static int _fps = 0;
        private static double _delta = 0;

        private static Vertex[] _cutout;
        private static RenderStates _layerstates;
        private static List<Vertex[]> _layerverts;

        private static List<TileAnimHandler> _tileanims;
        private static FunctionScript _updatescript;
        private static FunctionScript _renderscript;
        private static FunctionScript[] _renderers;
        private static FastTextureAtlas _fastatlas;

        private static string camera_ent = "";
        private static string input_ent = "";

        static MapEngineHandler()
        {
            _tileanims = new List<TileAnimHandler>();
            _tileatlas = new TextureAtlas(1024);
        }

        public static void BindToEngine(ScriptEngine engine)
        {
            engine.SetGlobalFunction("MapEngine", new Action<string, int>(MapEngine));
            engine.SetGlobalFunction("ExitMapEngine", new Action(ExitMapEngine));
            engine.SetGlobalFunction("ChangeMap", new Action<string>(LoadMap));
            engine.SetGlobalFunction("IsMapEngineRunning", new Func<bool>(IsMapEngineRunning));
            engine.SetGlobalFunction("SetCameraX", new Action<int>(SetCameraX));
            engine.SetGlobalFunction("SetCameraY", new Action<int>(SetCameraY));
            engine.SetGlobalFunction("GetCameraX", new Func<int>(GetCameraX));
            engine.SetGlobalFunction("GetCameraY", new Func<int>(GetCameraY));
            engine.SetGlobalFunction("SetUpdateScript", new Action<string>(SetUpdateScript));
            engine.SetGlobalFunction("SetRenderScript", new Action<string>(SetRenderScript));
            engine.SetGlobalFunction("SetLayerRenderer", new Action<int, string>(SetLayerRenderer));
            engine.SetGlobalFunction("GetMapEngineFrameRate", new Func<int>(GetMapEngineFrameRate));
            engine.SetGlobalFunction("SetMapEngineFrameRate", new Action<int>(SetMapEngineFrameRate));
            engine.SetGlobalFunction("AttachInput", new Action<string>(AttachInput));
            engine.SetGlobalFunction("AttachCamera", new Action<string>(AttachCamera));
            engine.SetGlobalFunction("DetachInput", new Action(DetachInput));
            engine.SetGlobalFunction("DetachCamera", new Action(DetachCamera));
            engine.SetGlobalFunction("GetInputPerson", new Func<string>(GetInputPerson));
            engine.SetGlobalFunction("GetCameraPerson", new Func<string>(GetCameraPerson));
            engine.SetGlobalFunction("IsInputAttached", new Func<bool>(IsInputAttached));
            engine.SetGlobalFunction("IsCameraAttached", new Func<bool>(IsCameraAttached));
            engine.SetGlobalFunction("UpdateMapEngine", new Action(UpdateMapEngine));
            engine.SetGlobalFunction("RenderMap", new Action(RenderMap));
            engine.SetGlobalFunction("GetTileWidth", new Func<int>(GetTileWidth));
            engine.SetGlobalFunction("GetTileHeight", new Func<int>(GetTileHeight));
            engine.SetGlobalFunction("GetLayerWidth", new Func<int, int>(GetLayerWidth));
            engine.SetGlobalFunction("GetLayerHeight", new Func<int, int>(GetLayerHeight));
        }

        private static void AttachInput(string name)
        {
            input_ent = name;
        }

        private static void AttachCamera(string name)
        {
            camera_ent = name;
        }

        private static string GetInputPerson()
        {
            return input_ent;
        }

        private static string GetCameraPerson()
        {
            return camera_ent;
        }

        private static bool IsInputAttached()
        {
            return input_ent != "" && PersonManager.DoesPersonExist(input_ent);
        }

        private static bool IsCameraAttached()
        {
            return camera_ent != "" && PersonManager.DoesPersonExist(camera_ent);
        }

        private static void DetachInput()
        {
            input_ent = "";
        }

        private static void DetachCamera()
        {
            camera_ent = "";
        }

        private static bool IsMapEngineRunning()
        {
            return !_ended;
        }

        private static int GetMapEngineFrameRate()
        {
            return _fps;
        }

        private static void SetMapEngineFrameRate(int rate)
        {
            Program._window.SetFramerateLimit((uint)rate);
            _delta = 1000.0 / rate; // for use later on...
            _fps = rate;
        }

        public static int GetLayerWidth(int layer)
        {
            return _map.Layers[layer].Width;
        }

        public static int GetLayerHeight(int layer)
        {
            return _map.Layers[layer].Height;
        }

        public static int GetTileWidth()
        {
            return _map.Tileset.TileWidth;
        }

        public static int GetTileHeight()
        {
            return _map.Tileset.TileHeight;
        }

        private static void MapEngine(string filename, [DefaultParameterValue(60)] int fps = 60)
        {
            _ended = false;
            SetMapEngineFrameRate(fps);

            double time = Program.GetTime();
            View v = new View(Program._window.GetView());
            filename = GlobalProps.BasePath + "/maps/" + filename;
            LoadMap(filename);
            Console.WriteLine(Program.GetTime() - time);

            while (!_ended)
            {
                if (_updatescript != null)
                    _updatescript.Execute();

                UpdateMapEngine();
                RenderMap();

                View camera = new View(Program._window.GetView());
                Program._window.SetView(v);
                if (_renderscript != null)
                    _renderscript.Execute();
                Program._window.SetView(camera);

                Program.FlipScreen();
            }

            Program._window.SetView(v);
            Program.SetFrameRate(Program.GetFrameRate());
        }

        /// <summary>
        /// Toggles the FPS throttle.
        /// </summary>
        public static void ToggleFPSThrottle()
        {
            _toggled = !_toggled;
            if (_toggled)
                Program._window.SetFramerateLimit(0);
            else
                Program._window.SetFramerateLimit((uint)_fps);
        }

        private static void LoadMap(string filename)
        {
            PersonManager.RemoveNonEssential();
            _map = new Map();
            _map.Load(filename);

            Vector2f start_pos = new Vector2f(_map.StartX, _map.StartY);
            foreach (Person p in PersonManager.People)
            {
                p.Position = start_pos;
                p.Layer = _map.StartLayer;
            }

            AddPersons();

            ConstructFastAtlas();
            ParseAnimations();
            UpdateCutout();

            _renderers = new FunctionScript[_map.Layers.Count];

            Tuple<List<Vertex[]>, RenderStates> tuple;
            tuple = _map.GetTileMap(_tileatlas);

            _layerstates = tuple.Item2;
            _layerstates.Texture = _fastatlas.RenderTexture.Texture;
            _layerverts = tuple.Item1;
        }

        /// <summary>
        /// Adds the persons in this map instance to the person handler.
        /// </summary>
        private static void AddPersons()
        {
            foreach (Entity e in _map.Entities)
            {
                if (e.Type == Entity.EntityType.Person) {
                    PersonManager.CreatePerson(e);
                    PersonManager.CallPersonScript(e.Name, (int)PersonScripts.Create);
                }
            }
        }

        /// <summary>
        /// Turns the tileset into a tile atlas and then put it into
        /// a wrapper for fast tetxure manipulation.
        /// </summary>
        private static void ConstructFastAtlas()
        {
            _tileatlas.Update(_map.Tileset.GetImageArray());
            _fastatlas = new FastTextureAtlas(_tileatlas);
        }

        /// <summary>
        /// Goes through the tiles and finds those that animate and
        /// Puts their index references into an easily managed class.
        /// </summary>
        private static void ParseAnimations()
        {
            _tileanims.Clear();

            int count = _map.Tileset.Tiles.Count;
            List<int> handled = new List<int>(); // tile anims can be cyclical, this stops it.
            for (var i = 0; i < count; ++i)
            {
                Tile t = _map.Tileset.Tiles[i];
                if (!t.Animated)
                    continue;

                handled.Clear();
                var animated = t.Animated;
                var handler = new TileAnimHandler(_fastatlas);
                var index = i;

                // If we found a tile animation, go through it's 
                // 'linked list' of next-tile animations.

                while (animated && !handled.Contains(index))
                {
                    handled.Add(index);
                    handler.AddTile(index, _map.Tileset.Tiles[index].Delay);
                    index = _map.Tileset.Tiles[index].NextAnim;
                    animated = _map.Tileset.Tiles[index].Animated;
                }
                _tileanims.Add(handler);
            }
        }

        /// <summary>
        /// Updates the max num of vertices that the size of the screen
        /// can handle.
        /// </summary>
        private static void UpdateCutout()
        {
            int w = GlobalProps.Width / _map.Tileset.TileWidth + 1;
            int h = GlobalProps.Height / _map.Tileset.TileHeight + 1;
            int length = w * h * 4;
            if (_cutout == null)
                _cutout = new Vertex[length];
            else if (_cutout.Length != length)
                Array.Resize(ref _cutout, length);
        }

        private static void UpdateMapEngine()
        {
            if (IsInputAttached() && PersonManager.IsCommandQueueEmpty(input_ent))
            {
                int x = (int)(Joystick.GetAxisPosition(0, Joystick.Axis.X) + 0.5f);
                int y = (int)(Joystick.GetAxisPosition(0, Joystick.Axis.Y) + 0.5f);

                if (GlobalInput.IsKeyPressed((int)Keyboard.Key.Up))
                    y = -1;
                if (GlobalInput.IsKeyPressed((int)Keyboard.Key.Down))
                    y = 1;
                if (GlobalInput.IsKeyPressed((int)Keyboard.Key.Left))
                    x = -1;
                if (GlobalInput.IsKeyPressed((int)Keyboard.Key.Right))
                    x = 1;

                switch (x + y * 3) {
                    case -4: // nw
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.FaceNorth, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveWest, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveNorth, false);
                        break;
                    case -3: // n
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.FaceNorth, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveNorth, false);
                        break;
                    case -2: // ne
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.FaceNorth, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveEast, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveNorth, false);
                        break;
                    case -1: // w
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.FaceWest, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveWest, false);
                        break;
                    case 1:  // e
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.FaceEast, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveEast, false);
                        break;
                    case 2:  // sw
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.FaceSouth, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveWest, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveSouth, false);
                        break;
                    case 3: // s
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.FaceSouth, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveSouth, false);
                        break;
                    case 4: // se
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.FaceSouth, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveEast, true);
                        PersonManager.QueuePersonCommand(input_ent, (int)Commands.MoveSouth, false);
                        break;
                }
            }

            foreach (Person p in PersonManager.People)
                p.UpdateCommandQueue();

            if (IsCameraAttached())
            {
                View v = Program._window.GetView();
                SetCameraX(PersonManager.GetPersonX(camera_ent) - (int)(v.Size.X / 2));
                SetCameraY(PersonManager.GetPersonY(camera_ent) - (int)(v.Size.Y / 2));
            }
        }

        private static void CutoutVerts(Vertex[] in_verts, int x, int y, int scan)
        {
            scan <<= 2;
            y /= _map.Tileset.TileHeight;
            x /= _map.Tileset.TileWidth;

            Vertex[] local = _cutout;
            int h = GlobalProps.Height / _map.Tileset.TileHeight + 1;
            int length = (GlobalProps.Width / _map.Tileset.TileWidth + 1) << 2;
            int offset = (x << 2) + y * scan;
            int height = offset + h * scan;
            int index = 0;

            for (var i = offset; i < height; i += scan)
            {
                Array.Copy(in_verts, i, local, index, length);
                index += length;
            }
        }

        private static void RenderMap()
        {
            foreach (TileAnimHandler h in _tileanims)
                h.Animate();

            _fastatlas.Refresh();

            int length = _map.Layers.Count;
            for (var i = 0; i < length; ++i)
            {
                CutoutVerts(_layerverts[i], GetCameraX(), GetCameraY(), _map.Layers[i].Width);
                Program._window.Draw(_cutout, PrimitiveType.Quads, _layerstates);
                DrawPersons(i);
                if (_renderers[i] != null)
                    _renderers[i].Execute();
            }
        }

        private static void ExitMapEngine()
        {
            _ended = true;
        }

        /// <summary>
        /// Draws the persons for the requested layer.
        /// </summary>
        /// <param name="layer">Layer.</param>
        private static void DrawPersons(int layer)
        {
            foreach (Person p in PersonManager.People)
                if (p.Layer == layer)
                    p.Draw();
        }

        private static void SetCameraX(int x) {
            View v = Program._window.GetView();
            if (x < 0 || v.Size.X + x >= _map.Layers[0].Width * _map.Tileset.TileWidth)
                return;
            v.Center = new Vector2f(v.Size.X / 2 + x, v.Center.Y);
            Program._window.SetView(v);
        }

        private static int GetCameraX()
        {
            View v = Program._window.GetView();
            return (int)(v.Center.X - v.Size.X / 2);
        }

        private static void SetCameraY(int y) {
            View v = Program._window.GetView();
            if (y < 0 || v.Size.Y + y >= _map.Layers[0].Height * _map.Tileset.TileHeight)
                return;
            v.Center = new Vector2f(v.Center.X, v.Size.Y / 2 + y);
            Program._window.SetView(v);
        }

        private static int GetCameraY()
        {
            View v = Program._window.GetView();
            return (int)(v.Center.Y - v.Size.Y / 2);
        }

        private static void SetUpdateScript(object code)
        {
            if (code == null || (code is string && (string)code == ""))
                _updatescript = null;
            else
                _updatescript = new FunctionScript(code);
        }

        private static void SetRenderScript(object code)
        {
            if (code == null || (code is string && (string)code == ""))
                _renderscript = null;
            else
                _renderscript = new FunctionScript(code);
        }

        private static void SetLayerRenderer(int layer, object code)
        {
            if (layer > 0 && layer < _renderers.Length)
            {
                if (code == null || (code is string && (string)code == ""))
                    _renderers[layer] = null;
                else
                    _renderers[layer] = new FunctionScript(code);
            }
        }
    }
}

