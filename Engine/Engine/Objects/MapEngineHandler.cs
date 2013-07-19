using System;
using Jurassic;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public static class MapEngineHandler
    {
        private static Map _map;
        private static TextureAtlas _tileAtlas;
        private static Sprite _tiles;
        private static bool _ended = false;

        private static RenderTexture[] _layertex;
        private static Sprite[] _layer_sprites;

        private static string _updateScript = "";
        private static string _renderScript = "";
        private static string[] _layerScripts;

        public static void BindToEngine(ScriptEngine engine)
        {
            engine.SetGlobalFunction("MapEngine", new Action<string, int>(MapEngine));
            engine.SetGlobalFunction("ChangeMap", new Action<string>(LoadMap));
            engine.SetGlobalFunction("IsMapEngineRunning", new Func<bool>(IsMapEngineRunning));
            engine.SetGlobalFunction("SetCameraX", new Action<int>(SetCameraX));
            engine.SetGlobalFunction("SetCameraY", new Action<int>(SetCameraY));
            engine.SetGlobalFunction("SetUpdateScript", new Action<string>(SetUpdateScript));
            engine.SetGlobalFunction("SetRenderScript", new Action<string>(SetRenderScript));
            engine.SetGlobalFunction("SetLayerRenderer", new Action<int, string>(SetLayerRenderer));
        }

        private static bool IsMapEngineRunning()
        {
            return !_ended;
        }

        private static void MapEngine(string filename, int fps = 60)
        {
            //Program._window.SetFramerateLimit((uint)fps);

            filename = GlobalProps.BasePath + "/maps/" + filename;
            LoadMap(filename);

            while (!_ended) {
                for (var i = 0; i < _layer_sprites.Length; ++i)
                    Program._window.Draw(_layer_sprites[i]);

                Program._engine.Execute(_renderScript);
                Program.FlipScreen();
            }
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

        private static void SetCameraX(int x) {
            View v = Program._window.GetView();
            v.Move(new Vector2f(x, 0));
            Program._window.SetView(v);
        }

        private static void SetCameraY(int y) {
            View v = Program._window.GetView();
            v.Move(new Vector2f(0, y));
            Program._window.SetView(v);
        }

        private static void SetUpdateScript(string code)
        {
            _updateScript = code;
        }

        private static void SetRenderScript(string code)
        {
            _renderScript = code;
        }

        private static void SetLayerRenderer(int layer, string code)
        {
            if (layer < _layerScripts.Length)
                _layerScripts[layer] = code;
        }
    }
}

