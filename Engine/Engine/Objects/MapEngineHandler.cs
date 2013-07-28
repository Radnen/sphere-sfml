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
        private static TextureAtlas _tileAtlas;
        private static Sprite _tiles;
        private static bool _ended = false;
        private static int _fps = 0;

        private static RenderTexture[] _layertex;
        private static Sprite[] _layer_sprites;

        private static CompiledMethod _updateScript;
        private static CompiledMethod _renderScript;
        private static string[] _layerScripts;

        private static string camera_ent = "";
        private static string input_ent = "";

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
            return input_ent != "";
        }

        private static bool IsCameraAttached()
        {
            return camera_ent != "";
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
            //SetMapEngineFrameRate(fps);

            View v = new View(Program._window.GetView());
            filename = GlobalProps.BasePath + "/maps/" + filename;
            LoadMap(filename);

            while (!_ended) {
                if (_updateScript != null)
                    _updateScript.Execute();

                UpdateMapEngine();
                RenderMap();

                View camera = new View(Program._window.GetView());
                Program._window.SetView(v);
                if (_renderScript != null)
                    _renderScript.Execute();
                Program._window.SetView(camera);

                Program.FlipScreen();
            }

            Program._window.SetView(v);
            Program.SetFrameRate(Program.GetFrameRate());
        }

        private static void LoadMap(string filename)
        {
            _map = new Map();
            _map.Load(filename);
 
            if (_tileAtlas == null)
                _tileAtlas = new TextureAtlas(1024);

            Image[] sources = new Image[_map.Tileset.Tiles.Count];
            for (var i = 0; i < _map.Tileset.Tiles.Count; ++i)
                sources[i] = _map.Tileset.Tiles[i].Graphic;

            _tileAtlas.Update(sources);
            _tiles = new Sprite(_tileAtlas.Texture);

            _layerScripts = new string[_map.Layers.Count];
            _layerScripts.Initialize();

            _layertex = new RenderTexture[_map.Layers.Count];
            _layer_sprites = new Sprite[_map.Layers.Count];
            for (var i = 0; i < _map.Layers.Count; ++i) {
                RenderTexture tex = new RenderTexture((uint)(_map.Layers[i].Width * _map.Tileset.TileWidth),
                                                      (uint)(_map.Layers[i].Height * _map.Tileset.TileHeight));

                DrawTiles(tex, _map.Layers[i]);
                tex.Display();

                _layer_sprites[i] = new Sprite(tex.Texture);
                _layertex[i] = tex;
            }

            Vector2f def_pos = new Vector2f(_map.StartX, _map.StartY);
            foreach (Person p in PersonManager.People.Values)
            {
                if (p.Position.X < 0 || p.Position.Y < 0)
                    p.Position = def_pos;
            }
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

            foreach (Person p in PersonManager.People.Values)
                p.UpdateCommandQueue();

            if (IsCameraAttached())
            {
                View v = Program._window.GetView();
                SetCameraX(PersonManager.GetPersonX(camera_ent) - (int)(v.Size.X / 2));
                SetCameraY(PersonManager.GetPersonY(camera_ent) - (int)(v.Size.Y / 2));
            }
        }

        private static void RenderMap()
        {
            for (var i = 0; i < _layer_sprites.Length; ++i)
            {
                Program._window.Draw(_layer_sprites[i]);
                if (i == _map.StartLayer)
                    DrawPersons();
            }
        }

        private static void ExitMapEngine()
        {
            _ended = true;
        }

        private static void DrawTiles(RenderTexture layer_tex, Layer layer)
        {
            Vector2f position = new Vector2f();
            int sw = Program.GetScreenWidth();
            int sh = Program.GetScreenHeight();
            int tw = _map.Tileset.TileWidth;
            int th = _map.Tileset.TileHeight;

            for (int y = 0; y < layer.Height; ++y)
            {
                position.Y = y * th;
                for (int x = 0; x < layer.Width; ++x)
                {
                    position.X = x * th;
                    short t = layer.GetTile(x, y);
                    if (t < 0) continue;
                    _tiles.Position = position;
                    _tiles.TextureRect = _tileAtlas.Sources[t];
                    layer_tex.Draw(_tiles);
                }
            }
        }

        private static void DrawPersons()
        {
            foreach (Person p in PersonManager.People.Values)
                p.Draw();
        }

        private static void SetCameraX(int x) {
            View v = Program._window.GetView();
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
            v.Center = new Vector2f(v.Center.X, v.Size.Y / 2 + y);
            Program._window.SetView(v);
        }

        private static int GetCameraY()
        {
            View v = Program._window.GetView();
            return (int)(v.Center.Y - v.Size.Y / 2);
        }

        private static void SetUpdateScript(string code)
        {
            _updateScript = new CompiledMethod(Program._engine, code);
        }

        private static void SetRenderScript(string code)
        {
            _renderScript = new CompiledMethod(Program._engine, code);
        }

        private static void SetLayerRenderer(int layer, string code)
        {
            if (layer < _layerScripts.Length)
                _layerScripts[layer] = code;
        }
    }
}

