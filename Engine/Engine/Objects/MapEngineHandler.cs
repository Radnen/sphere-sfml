using System;
using System.Collections.Generic;
using System.Diagnostics;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public static class MapEngineHandler
    {
        private static Map _map;

        public static Map Map { get { return _map; } }

        private static TextureAtlas _tileatlas;
        private static bool _ended = true, _toggled, _talkheld;
        private static int _fps = 0;
        private static double _delta = 0;

        private static View _cameraView;
        private static Vector2f _camera;
        private static Vertex[] _cutout;
        private static RenderStates _layerstates;
        private static List<Vertex[]> _layerverts;

        private static List<TileAnimHandler> _tileanims;
        private static FunctionScript _updatescript;
        private static FunctionScript _renderscript;
        private static FunctionScript[] _renderers;
        private static FastTextureAtlas _fastatlas;
        private static List<Entity> _triggers;
        //private static List<Zone> _zones;
        private static FunctionScript[] _scripts;
        private static FunctionScript[] _defscripts;

        private static string camera_ent = "";
        private static string input_ent = "";
        private static string _current = "";
        private static int _mask_frames = 0, _frames = 0;
        private static int _target_alpha = 0;
        private static ColorInstance _mask = null;
        private static Entity _last_trigger = null; // for one trigger at a time.

        static MapEngineHandler()
        {
            _tileanims = new List<TileAnimHandler>();
            _tileatlas = new TextureAtlas(1024);
            _triggers = new List<Entity>();
            _scripts = new FunctionScript[6];
            _defscripts = new FunctionScript[6];
        }

        public static void BindToEngine(ScriptEngine engine)
        {
            engine.SetGlobalFunction("MapEngine", new Action<string, int>(MapEngine));
            engine.SetGlobalFunction("ExitMapEngine", new Action(ExitMapEngine));
            engine.SetGlobalFunction("ChangeMap", new Action<string>(ChangeMap));
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
            engine.SetGlobalFunction("SetDefaultMapScript", new Action<int, object>(SetDefaultMapScript));
            engine.SetGlobalFunction("CallDefaultMapScript", new Action<int>(CallDefaultMapScript));
            engine.SetGlobalFunction("GetCurrentMap", new Func<string>(GetCurrentMap));
            engine.SetGlobalFunction("GetTile", new Func<int, int, int, int>(GetTile));
            engine.SetGlobalFunction("SetTile", new Action<int, int, int, int>(SetTile));
            engine.SetGlobalFunction("GetNumTiles", new Func<int>(GetNumTiles));
            engine.SetGlobalFunction("GetNumLayers", new Func<int>(GetNumLayers));
            engine.SetGlobalFunction("GetLayerName", new Func<int, string>(GetLayerName));
            engine.SetGlobalFunction("GetTileName", new Func<int, string>(GetTileName));
            engine.SetGlobalFunction("GetTileDelay", new Func<int, int>(GetTileDelay));
            engine.SetGlobalFunction("SetLayerVisible", new Action<int, bool>(SetLayerVisible));
            engine.SetGlobalFunction("MapToScreenX", new Func<int, int, int>(MapToScreenX));
            engine.SetGlobalFunction("MapToScreenY", new Func<int, int, int>(MapToScreenY));
            engine.SetGlobalFunction("ScreenToMapX", new Func<int, int, int>(ScreenToMapX));
            engine.SetGlobalFunction("ScreenToMapY", new Func<int, int, int>(ScreenToMapY));
            engine.SetGlobalFunction("IsTriggerAt", new Func<double, double, int, bool>(IsTriggerAt));
            engine.SetGlobalFunction("ExecuteTrigger", new Action<double, double, int>(ExecuteTrigger));
            engine.SetGlobalFunction("SetColorMask", new Action<ColorInstance, int>(SetColorMask));
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
            //Program._window.SetFramerateLimit((uint)rate);
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
            DateTime time = DateTime.Now;
            SetMapEngineFrameRate(fps);

            _cameraView = new View(GetDefaultView());
            _current = filename;

            // It seems Sphere keeps non-essential npc's created prior to MapEngine.
            LoadMap(Program.ParseSpherePath(filename, "maps"), false, true);

            Console.WriteLine((DateTime.Now - time).TotalMilliseconds);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (!_ended)
            {
                time = DateTime.Now;
                while ((DateTime.Now - time).TotalMilliseconds <= _delta)
                {
                }

                if (_updatescript != null)
                    _updatescript.Execute();

                UpdateMapEngine();
                RenderMap();

                Program.FlipScreen();

                var keyheld = GlobalInput.IsKeyPressed(GlobalInput.TalkKey);
                if (!_talkheld && keyheld)
                {
                    DoTalk();
                    _talkheld = true;
                }
                if (!keyheld)
                    _talkheld = false;
            }

            PersonManager.RemoveNonEssential();
            Program.SetFrameRate(Program.GetFrameRate());
        }

        private static View _defaultView;
        public static View GetDefaultView()
        {
            if (_defaultView == null)
            {
                Vector2f size = new Vector2f(GlobalProps.Width, GlobalProps.Height);
                Vector2f center = new Vector2f(GlobalProps.Width / 2, GlobalProps.Height / 2);
                _defaultView = new View(center, size);
            }
            return _defaultView;
        }

        public static void SetColorMask(ColorInstance color, int frames)
        {
            Color c = color.GetColor();
            if (c.Equals(Color.White))
                _mask = null;
            else
            {
                _mask = color;
                _target_alpha = c.A;
                _mask_frames = frames;
            }
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

        public static void DoTalk()
        {
            if (!IsInputAttached())
                return;
            string person = PersonManager.GetClosest(input_ent);
            if (person != null)
                PersonManager.CallPersonScript(person, (int)PersonScripts.Talk);
        }

        private static void ChangeMap(string filename)
        {
            _current = filename;
            LoadMap(Program.ParseSpherePath(filename, "maps"), true, false);
        }

        /// <summary>
        /// Loads the map, and optionally removes those that may die when changing map.
        /// </summary>
        /// <param name="filename">Filename.</param>
        /// <param name="remove_people">If set to <c>true</c> remove people.</param>
        private static void LoadMap(string filename, bool removePeople, bool useBase)
        {
            if (removePeople)
                PersonManager.RemoveNonEssential();

            _map = new Map();

            if (!_map.Load(filename))
                throw new System.IO.FileNotFoundException("Could not locate map file.", filename);

            SetCameraX(_map.StartX);
            SetCameraY(_map.StartY);

            AddScripts();

            // do this before AddPersons() to reset essential NPC's to map defaults.
            Vector2f start_pos = new Vector2f(_map.StartX, _map.StartY);
            foreach (Person p in PersonManager.People)
            {
                Vector2f basevect = new Vector2f(p.BaseWidth / 2, p.BaseHeight / 2);
                p.Position = useBase ? start_pos - basevect : start_pos;
                p.Layer = _map.StartLayer;
            }

            // add non-essential NPC's to the map.
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

            CallDefaultMapScript((int)MapScripts.Enter);
            CallLocalMapScript(MapScripts.Enter);
        }

        /// <summary>
        /// Adds the persons and triggers in this map instance to the person handler.
        /// </summary>
        private static void AddPersons()
        {
            _triggers.Clear();
            foreach (Entity e in _map.Entities)
            {
                if (e.Type == Entity.EntityType.Person) {
                    PersonManager.CreatePerson(e);
                    PersonManager.CallPersonScript(e.Name, (int)PersonScripts.Create);
                }
                else if (e.Type == Entity.EntityType.Trigger) {
                    _triggers.Add(e);
                }
            }
        }

        /// <summary>
        /// Adds map scripts to this map engine instance.
        /// </summary>
        private static void AddScripts()
        {
            for (var i = 3; i < _map.Scripts.Count; ++i)
            {
                if (!String.IsNullOrEmpty(_map.Scripts[i]))
                    _scripts[i - 3] = new FunctionScript(_map.Scripts[i]);
                else
                    _scripts[i - 3] = null;
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

#if(DEBUG)
            ColorInstance color = new ColorInstance(Program._engine, Color.Magenta);
            for (int i = 0; i < _map.Tileset.Tiles.Count; ++i) {
                Tile t = _map.Tileset.Tiles[i];
                foreach (Line l in t.Obstructions)
                    _fastatlas.Line((uint)i, l);
            }
#endif
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
            foreach (TileAnimHandler h in _tileanims)
                h.Animate();

            _fastatlas.Refresh();

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

            for (var i = 0; i < PersonManager.People.Count; ++i)
                PersonManager.People[i].UpdateCommandQueue();

            PersonManager.OrderPeople();

            if (IsCameraAttached())
            {
                SetCameraX(PersonManager.GetPersonX(camera_ent));
                SetCameraY(PersonManager.GetPersonY(camera_ent));
            }

            if (IsInputAttached())
            {
                int px = PersonManager.GetPersonX(input_ent);
                int py = PersonManager.GetPersonY(input_ent);
                Entity trigger = GetTriggerAt(px, py);
                if (trigger != _last_trigger)
                {
                    if (trigger != null)
                        trigger.ExecuteTrigger();
                    _last_trigger = trigger;
                }
            }
        }

        private static Entity GetTriggerAt(double x, double y)
        {
            x = (int)x / _map.Tileset.TileWidth;
            y = (int)y / _map.Tileset.TileHeight;
            foreach (Entity e in _triggers)
            {
                int tx = e.X / _map.Tileset.TileWidth;
                int ty = e.Y / _map.Tileset.TileHeight;
                if (tx == x && ty == y)
                    return e;
            }
            return null;
        }

        private static bool IsTriggerAt(double x, double y, int layer)
        {
            Entity trigger = GetTriggerAt(x, y);
            return trigger != null;
        }

        private static void ExecuteTrigger(double x, double y, int layer)
        {
            Entity trigger = GetTriggerAt(x, y);
            if (trigger != null)
                trigger.ExecuteTrigger();
        }

        private static void CutoutVerts(Vertex[] in_verts, int x, int y, int scan)
        {
            if (x < 0 || y < 0)
                return;

            scan <<= 2;
            y /= _map.Tileset.TileHeight;
            x /= _map.Tileset.TileWidth;

            // initialize some math to make stripping easier.
            Vertex[] local = _cutout;
            int h = GlobalProps.Height / _map.Tileset.TileHeight + 1;
            int length = (GlobalProps.Width / _map.Tileset.TileWidth + 1) << 2;
            int offset = (x << 2) + y * scan;
            int height = offset + h * scan;
            int index = 0, i = 0;

            // strip out slices of the view at a time.
            for (i = offset; i + length < in_verts.Length && index < local.Length && i < height; i += scan)
            {
                Array.Copy(in_verts, i, local, index, length);
                index += length;
            }

            // check to see if we haven't; else pad out the rest. (small maps are affected by this).
            if (i < in_verts.Length && index < local.Length)
                Array.Copy(in_verts, i, local, index, in_verts.Length - i);
        }

        private static void DrawVerts(Vertex[] in_verts, int x, int y, uint scan)
        {
            if (x < 0 || y < 0)
                return;

            scan <<= 2;
            y /= _map.Tileset.TileHeight;
            x /= _map.Tileset.TileWidth;

            uint height = (uint)(GlobalProps.Height / _map.Tileset.TileHeight + 1) * scan;
            uint length = (uint)((GlobalProps.Width / _map.Tileset.TileWidth + 1) << 2);
            uint offset = (uint)((x << 2) + y * scan), i = 0;

            for (; i < height; i += scan)
            {
                if (offset + i > in_verts.Length) break;
                Program._window.Draw(in_verts, offset + i, length, PrimitiveType.Quads, _layerstates);
            }

            // draw the last line:
            if (offset + i > in_verts.Length)
                Program._window.Draw(in_verts, offset + i, (uint)in_verts.Length - (offset + i), PrimitiveType.Quads, _layerstates);
        }

        private static void RenderMap()
        {
            Program._window.SetView(_cameraView);

            Vector2f camera = GetClampedCamera();
            int length = _map.Layers.Count;
            for (var i = 0; i < length; ++i)
            {
                if (_map.Layers[i].Visible)
                {
                    DrawVerts(_layerverts[i], (int)camera.X, (int)camera.Y, (uint)_map.Layers[i].Width);
                    //CutoutVerts(_layerverts[i], (int)camera.X, (int)camera.Y, _map.Layers[i].Width);
                    //Program._window.Draw(_cutout, PrimitiveType.Quads, _layerstates);
                }
                DrawPersons(i);
                if (_renderers[i] != null)
                    _renderers[i].Execute();
            }

            Program._window.SetView(GetDefaultView());

            if (_mask != null && _frames != _mask_frames)
            {
                _mask["alpha"] = (int)(((float)_frames / _mask_frames) * _target_alpha);
                GlobalPrimitives.ApplyColorMask(_mask);
                _frames++;
                if (_frames == _mask_frames)
                    _mask["alpha"] = _target_alpha;
            }

            if (_renderscript != null)
                _renderscript.Execute();
        }

        public static bool CheckTileObstruction(ref Vector2f position, Person person)
        {
            int tw = _map.Tileset.TileWidth, th = _map.Tileset.TileHeight;
            int sx = (int)position.X / tw + 2;
            int sy = (int)position.Y / th + 2;
            Vector2f pos = new Vector2f();
            for (var y = sy - 2; y < sy; ++y)
            {
                pos.Y = y * th;
                for (var x = sx - 2; x < sx; ++x)
                {
                    pos.X = x * tw;
                    int t = _map.Layers[person.Layer].GetTile(x, y);
                    if (t >= 0 && person.CheckObstructions(ref position, ref pos, _map.Tileset.Tiles[t]))
                        return true;
                }
            }
            return false;
        }

        public static bool CheckLineObstruction(ref Vector2f position, Person person)
        {
            foreach (Segment s in _map.Layers[person.Layer].Segments)
            {
                if (person.CheckObstructions(ref position, s.Line))
                    return true;
            }
            return false;
        }

        private static void ExitMapEngine()
        {
            _ended = true;
        }

        private static string GetCurrentMap()
        {
            return _current;
        }

        private static void SetDefaultMapScript(int type, object code)
        {
            if (code == null || (code is string && code.Equals("")))
                _defscripts[type] = null;
            else
                _defscripts[type] = new FunctionScript(code);
        }

        private static void CallDefaultMapScript(int type)
        {
            if (_defscripts[type] != null)
                _defscripts[type].Execute();
        }

        private static void CallLocalMapScript(MapScripts type)
        {
            if (_scripts[(int)type] != null)
                _scripts[(int)type].Execute();
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

        private static Vector2f GetClampedCamera()
        {
            Vector2f v = Program._window.GetView().Center;
            v.X -= GlobalProps.Width / 2;
            v.Y -= GlobalProps.Height / 2;
            return v;
        }

        private static void SetCameraX(int x) {
            _camera.X = x;
            int w = (_map.Layers[_map.StartLayer].Width) * _map.Tileset.TileWidth;
            int w2 = GlobalProps.Width / 2;
            if (x < w2)
                _cameraView.Center = new Vector2f(w2, _cameraView.Center.Y);
            else if (x >= w - w2)
                _cameraView.Center = new Vector2f(w - w2, _cameraView.Center.Y);
            else
                _cameraView.Center = new Vector2f(x, _cameraView.Center.Y);
        }

        private static int GetCameraX()
        {
            return (int)_camera.X;
        }

        private static void SetCameraY(int y) {
            _camera.Y = y;
            int h = _map.Layers[0].Height * _map.Tileset.TileHeight;
            int h2 = GlobalProps.Height / 2;
            if (y < h2)
                _cameraView.Center = new Vector2f(_cameraView.Center.X, h2);
            else if (y >= h - h2)
                _cameraView.Center = new Vector2f(_cameraView.Center.X, h - h2);
            else
                _cameraView.Center = new Vector2f(_cameraView.Center.X, y);
        }

        private static int GetCameraY()
        {
            return (int)_camera.Y;
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

        private static int GetNumLayers()
        {
            return _map.Layers.Count;
        }

        private static void SetLayerVisible(int layer, bool visible)
        {
            _map.Layers[layer].Visible = visible;
        }

        private static int GetTile(int x, int y, int layer)
        {
            if (layer < 0 || layer >= _map.Layers.Count)
                throw new MapEngineException("RangeError", string.Format("Invalid layer: {0}", layer));
            var tile = _map.Layers[layer].GetTile(x, y);
            if (tile < 0 || tile >= _map.Tileset.Tiles.Count)
                throw new MapEngineException("RangeError", string.Format("Invalid (x, y) coords: ({0},{1})", x, y));
            return tile;
        }

        private static void SetTile(int x, int y, int layer, int tile)
        {
            _map.Layers[layer].SetTile(x, y, (short)tile);

            Tuple<List<Vertex[]>, RenderStates> tuple;
            tuple = _map.GetTileMap(_tileatlas);

            _layerstates = tuple.Item2;
            _layerstates.Texture = _fastatlas.RenderTexture.Texture;
            _layerverts = tuple.Item1;
        }

        private static string GetTileName(int tile)
        {
            return _map.Tileset.Tiles[tile].Name;
        }

        private static int GetTileDelay(int tile)
        {
            return _map.Tileset.Tiles[tile].Delay;
        }

        private static int GetNumTiles()
        {
            return _map.Tileset.Tiles.Count;
        }

        private static string GetLayerName(int layer)
        {
            return _map.Layers[layer].Name;
        }

        private static int MapToScreenX(int layer, int x)
        {
            return x - GetCameraX() + GlobalProps.Width / 2;
        }

        private static int MapToScreenY(int layer, int y)
        {
            return y - GetCameraY() + GlobalProps.Height / 2;
        }

        private static int ScreenToMapX(int layer, int x)
        {
            return x + GetCameraX() - GlobalProps.Width / 2;
        }

        private static int ScreenToMapY(int layer, int y)
        {
            return y + GetCameraY() - GlobalProps.Height / 2;
        }
    }
}

