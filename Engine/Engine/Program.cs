using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using SFML;
using SFML.Graphics;
using SFML.Window;
using Jurassic;
using Jurassic.Library;
using Engine.Objects;

namespace Engine
{
    static class Program
    {
        public static RenderWindow _window = null;
        public static ScriptEngine _engine = null;
        private static Dictionary<int, bool> _keyCache = new Dictionary<int, bool>();
        private static Queue<int> _keyQueue = new Queue<int>(10);

        private static bool _fullscreen;
        private static int _internal_fps = 0;
        private static bool _scaled = true;

        static GameFile _game = new GameFile();

        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0] == "-compliance")
                {
                    Console.WriteLine("Sphere v1.5 subset compliance.");
                    return;
                }
                else if (args[0] == "-games")
                {
                    Console.WriteLine("List of games");
                    ListGames();
                    return;
                }
                else
                {
                    Console.WriteLine("Useage: ");
                    Console.WriteLine("    -game <directory>  | Loads the game running at that directory.");
                    Console.WriteLine("    -compliance        | See what Sphere version this build conforms to.");
                    Console.WriteLine("    -games             | Lists games local to this executable.");
                    return;
                }
            }
            if (args.Length == 2)
            {
                if (args[0] != "-game" || !_game.ReadFile(args[1]))
                {
                    Console.WriteLine("Failed to load game: " + args[1]);
                    return;
                }
            }
            else
            {
                if (!_game.ReadFile("startup/game.sgm"))
                {
                    Console.WriteLine("Faliled to find game.sgm in startup/");
                    return;
                }
            }

            if (!InitEngine())
                return;

            string filename;
            if (_game.TryGetData("script", out filename))
            {
                RequireScript(filename);
                RunCode("game();");
            }
            else
            {
                Console.WriteLine("Invalid script file in game.sgm");
            }
        }

        static void ListGames()
        {
            if (Directory.Exists("games"))
            {
                string[] files = Directory.GetFileSystemEntries("games");
                foreach (string s in files)
                {
                    GameFile file = new GameFile();
                    if (file.ReadFile(s + "/game.sgm")) {
                        string name, author, desc;
                        file.TryGetData("name", out name);
                        file.TryGetData("author", out author);
                        file.TryGetData("description", out desc);
                        Console.WriteLine(string.Format("{0} by {1}, \"{2}\"", name, author, desc));
                    }
                }
            }
        }

        static bool InitEngine()
        {
            bool ret = InitWindow(Styles.Default);
            _engine = GetSphereEngine();
            return ret;
        }

        static bool InitWindow(Styles style)
        {
            int width, height;
            if (!_game.TryGetData("screen_width", out width))
            {
                Console.WriteLine("No screen width set in game.sgm.");
                return false;
            }
            if (!_game.TryGetData("screen_height", out height))
            {
                Console.WriteLine("No screen height set in game.sgm.");
                return false;
            }

            if (width <= 0 || height <= 0)
            {
                Console.WriteLine("Invalid width and height in game.sgm.");
                return false;
            }

            GlobalProps.Width = width;
            GlobalProps.Height = height;

            if (_scaled)
            {
                width *= 2;
                height *= 2;
            }

            GlobalProps.BasePath = Path.GetDirectoryName(_game.FileName);

            if (!_game.TryGetData("name", out GlobalProps.GameName))
            {
                Console.WriteLine("No name set in game.sgm.");
                return false;
            }

            _window = new RenderWindow(new VideoMode((uint)width, (uint)height, 32), "", style);

            if (_scaled)
            {
                View v = _window.GetView();
                v.Size = new Vector2f(GlobalProps.Width, GlobalProps.Height);
                v.Center = new Vector2f(GlobalProps.Width / 2, GlobalProps.Height / 2);
                _window.SetView(v);
            }

            _window.Closed += window_Closed;
            _window.KeyPressed += window_KeyPressed;
            _window.KeyReleased += window_KeyReleased;
            _window.SetMouseCursorVisible(false);

            GlobalPrimitives.window = _window;
            return true;
        }

        public static void ToggleFullScreen()
        {
            if (_window != null) {
                _window.Closed -= window_Closed;
                _window.KeyPressed -= window_KeyPressed;
                _window.KeyReleased -= window_KeyReleased;
                _window.Close();
            }

            _fullscreen = !_fullscreen;
            var style = (_fullscreen) ? Styles.Fullscreen : Styles.Default;

            InitWindow(style);
            _window.SetFramerateLimit((uint)_internal_fps);
            _window.SetMouseCursorVisible(false);
        }

        public static void window_KeyPressed(object sender, KeyEventArgs e) {
            _keyCache[(int)e.Code] = true;

            _keyQueue.Enqueue((int)e.Code);

            if (e.Code == Keyboard.Key.F10)
                ToggleFullScreen();
        }

        public static void window_KeyReleased(object sender, KeyEventArgs e) {
            _keyCache[(int)e.Code] = false;
        }

        public static ScriptEngine GetSphereEngine()
        {
            ScriptEngine engine = new ScriptEngine();

            // The glorious Sphere game API :)
            engine.SetGlobalFunction("Abort", new Action<string>(Abort));
            engine.SetGlobalFunction("GetVersion", new Func<double>(GetVersion));
            engine.SetGlobalFunction("GetVersionString", new Func<string>(GetVersionString));
            engine.SetGlobalFunction("FlipScreen", new Action(FlipScreen));
            engine.SetGlobalFunction("GetScreenWidth", new Func<int>(GetScreenWidth));
            engine.SetGlobalFunction("GetScreenHeight", new Func<int>(GetScreenHeight));
            engine.SetGlobalFunction("RequireScript", new Action<string>(RequireScript));
            engine.SetGlobalFunction("Print", new Action<string>(Print));
            engine.SetGlobalFunction("Exit", new Action(Exit));
            engine.SetGlobalFunction("CreateColor", new Func<int, int, int, int, ColorInstance>(CreateColor));
            engine.SetGlobalFunction("LoadImage", new Func<string, ImageInstance>(LoadImage));
            engine.SetGlobalFunction("LoadWindowStyle", new Func<string, WindowStyleInstance>(LoadWindowStyle));
            engine.SetGlobalFunction("LoadFont", new Func<string, FontInstance>(LoadFont));
            engine.SetGlobalFunction("GetSystemFont", new Func<FontInstance>(GetSystemFont));
            engine.SetGlobalFunction("GetSystemWindowStyle", new Func<WindowStyleInstance>(GetSystemWindowStyle));
            engine.SetGlobalFunction("GetSystemArrow", new Func<ImageInstance>(GetSystemArrow));
            engine.SetGlobalFunction("GetSystemUpArrow", new Func<ImageInstance>(GetSystemUpArrow));
            engine.SetGlobalFunction("GetSystemDownArrow", new Func<ImageInstance>(GetSystemDownArrow));
            engine.SetGlobalFunction("Rectangle", new Action<double, double, double, double, ColorInstance>(GlobalPrimitives.Rectangle));
            engine.SetGlobalFunction("Triangle", new Action<double, double, double, double, double, double, ColorInstance>(GlobalPrimitives.Triangle));
            engine.SetGlobalFunction("GradientTriangle", new Action<ObjectInstance, ObjectInstance, ObjectInstance, ColorInstance, ColorInstance, ColorInstance>(GlobalPrimitives.GradientTriangle));
            engine.SetGlobalFunction("OutlinedRectangle", new Action<double, double, double, double, ColorInstance, double>(GlobalPrimitives.OutlinedRectangle));
            engine.SetGlobalFunction("GradientRectangle", new Action<double, double, double, double, ColorInstance, ColorInstance, ColorInstance, ColorInstance>(GlobalPrimitives.GradientRectangle));
            engine.SetGlobalFunction("FilledCircle", new Action<double, double, double, ColorInstance>(GlobalPrimitives.FilledCircle));
            engine.SetGlobalFunction("Line", new Action<double, double, double, double, ColorInstance>(GlobalPrimitives.Line));
            engine.SetGlobalFunction("LineSeries", new Action<ArrayInstance, ColorInstance>(GlobalPrimitives.LineSeries));
            engine.SetGlobalFunction("Point", new Action<double, double, ColorInstance>(GlobalPrimitives.Point));
            engine.SetGlobalFunction("PointSeries", new Action<ArrayInstance, ColorInstance>(GlobalPrimitives.PointSeries));
            engine.SetGlobalFunction("Polygon", new Action<ArrayInstance, ColorInstance, bool>(GlobalPrimitives.Polygon));
            engine.SetGlobalFunction("GradientLine", new Action<double, double, double, double, ColorInstance, ColorInstance>(GlobalPrimitives.GradientLine));
            engine.SetGlobalFunction("CreateSurface", new Func<int, int, ColorInstance, SurfaceInstance>(CreateSurface)); 
            engine.SetGlobalFunction("GrabImage", new Func<ImageInstance>(GrabImage));
            engine.SetGlobalFunction("IsKeyPressed", new Func<int, bool>(IsKeyPressed));
            engine.SetGlobalFunction("SetFrameRate", new Action<int>(SetFrameRate));
            engine.SetGlobalFunction("GetFrameRate", new Func<int>(GetFrameRate));
            engine.SetGlobalFunction("GetRealFrameRate", new Func<int>(GetRealFrameRate));
            engine.SetGlobalFunction("GetTime", new Func<double>(GetTime));
            engine.SetGlobalFunction("BlendColors", new Func<ColorInstance, ColorInstance, ColorInstance>(BlendColors));
            engine.SetGlobalFunction("BlendColorsWeighted", new Func<ColorInstance, ColorInstance, double, ColorInstance>(BlendColorsWeighted));
            engine.SetGlobalFunction("ApplyColorMask", new Action<ColorInstance>(GlobalPrimitives.ApplyColorMask));
            engine.SetGlobalFunction("IsMapEngineRunning", new Func<bool>(IsMapEngineRunning));
            engine.SetGlobalFunction("AreKeysLeft", new Func<bool>(AreKeysLeft));
            engine.SetGlobalFunction("GetKey", new Func<int>(GetKey));
            engine.SetGlobalFunction("GetMouseX", new Func<int>(GetMouseX));
            engine.SetGlobalFunction("GetMouseY", new Func<int>(GetMouseY));
            engine.SetGlobalFunction("SetMouseX", new Action<int>(SetMouseX));
            engine.SetGlobalFunction("SetMouseY", new Action<int>(SetMouseY));

            // keys:
            Array a = Enum.GetValues(typeof(Keyboard.Key));
            string[] n = Enum.GetNames(typeof(Keyboard.Key));
            for (var i = 0; i < a.Length; ++i)
            {
                string key = n[i].ToUpper();
                if (key == "RETURN")
                    key = "ENTER";
                if (key == "RSHIFT")
                    key = "SHIFT";
                if (key == "RCONTROL")
                    key = "CTRL";
                if (key == "RALT")
                    key = "ALT";
                if (key.StartsWith("NUM"))
                    key = key[3].ToString();
                engine.SetGlobalValue("KEY_" + key, (int)a.GetValue(i));
            }
            engine.SetGlobalValue("BLEND", 0);
            engine.SetGlobalValue("REPLACE", 1);
            engine.SetGlobalValue("RGB_ONLY", 2);
            engine.SetGlobalValue("ALPHA_ONLY", 3);
            engine.SetGlobalValue("ADD", 4);
            engine.SetGlobalValue("SUBTRACT", 5);
            engine.SetGlobalValue("MULTIPLY", 6);
            engine.SetGlobalValue("AVERAGE", 7);

            return engine;
        }

        public static void SetupTestEnvironment()
        {
            if (_window != null)
                return;

            _window = new RenderWindow(new VideoMode(320, 240), "Test Window", Styles.Default);
            _engine = GetSphereEngine();
        }

		static void Exit()
		{
			_window.Close();

            // Sadly, it's the only way to kill a while(true){}; in the JS environment.
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            proc.Kill();
		}

        static void SetFrameRate(int fps)
        {
            _window.SetFramerateLimit((uint)fps);
            _internal_fps = fps;
        }

        static int GetFrameRate()
        {
            return _internal_fps;
        }

        static bool IsMapEngineRunning()
        {
            return false;
        }

        static int GetRealFrameRate()
        {
            return _fps;
        }

        static ColorInstance BlendColors(ColorInstance c1, ColorInstance c2)
        {
            return BlendColorsWeighted(c1, c2, 0.5);
        }

        static ColorInstance BlendColorsWeighted(ColorInstance c1, ColorInstance c2, double w)
        {
            int R = (int)((int)c1["red"] * w + (int)c2["red"] * (1 - w));
            int G = (int)((int)c1["green"] * w + (int)c2["green"] * (1 - w));
            int B = (int)((int)c1["blue"] * w + (int)c2["blue"] * (1 - w));
            int A = (int)((int)c1["alpha"] * w + (int)c2["alpha"] * (1 - w));
            return new ColorInstance(_engine.Object.InstancePrototype, R, G, B, A);
        }

        static void Print(object obj)
        {
            Console.WriteLine(obj);
        }

        static double GetTime()
        {
            return DateInstance.Now();
        }

        static bool IsKeyPressed(int code)
        {
            return (_keyCache.ContainsKey(code)) ? _keyCache[code] : false;
        }

        static bool AreKeysLeft()
        {
            return _keyQueue.Count > 0;
        }

        static int GetKey()
        {
            while (_keyQueue.Count == 0)
            {
                _window.DispatchEvents();
                _window.Clear(Color.Cyan);
            }
            return _keyQueue.Dequeue();
        }

        static int GetMouseX()
        {
            return _window.InternalGetMousePosition().X;
        }

        static int GetMouseY()
        {
            return _window.InternalGetMousePosition().Y;
        }

        static void SetMouseX(int new_x)
        {
            int y = GetMouseY();
            _window.InternalSetMousePosition(new Vector2i(new_x, y));
        }

        static void SetMouseY(int new_y)
        {
            int x = GetMouseX();
            _window.InternalSetMousePosition(new Vector2i(x, new_y));
        }

		static ColorInstance CreateColor(int r, int g, int b, int a = 255)
		{
            return new ColorInstance(_engine.Object.InstancePrototype, r, g, b, a);
		}

        static ImageInstance GrabImage()
        {
            Image window = _window.Capture();
            return new ImageInstance(_engine.Object.InstancePrototype, new Texture(window));
        }

        static DateTime _start = DateTime.Now;
        static int _fps = 0;
        static void FlipScreen()
        {
            _window.DispatchEvents();
            _window.Display();
            _window.Clear();

            _fps++;
            if ((DateTime.Now - _start).Seconds >= 1)
            {
                _window.SetTitle("FPS: " + _fps);
                _fps = 0;
                _start = DateTime.Now;
            }
        }

        static int GetScreenWidth()
        {
            return GlobalProps.Width;
        }

        static int GetScreenHeight()
        {
            return GlobalProps.Height;
        }

        static void Abort(string msg)
        {
            Print(msg);
            Exit();
        }

        static double GetVersion()
        {
            return 1.55;
        }

        static string GetVersionString()
        {
            return "v1.55";
        }

        static WindowStyleInstance GetSystemWindowStyle()
        {
            return new WindowStyleInstance(_engine.Object.InstancePrototype, "system/system.rws");
        }

        static FontInstance GetSystemFont()
        {
            return new FontInstance(_engine.Object.InstancePrototype, "system/system.rfn");
        }

        static ImageInstance GetSystemArrow()
        {
            return new ImageInstance(_engine.Object.InstancePrototype, "system/pointer.png");
        }

        static ImageInstance GetSystemUpArrow()
        {
            return new ImageInstance(_engine.Object.InstancePrototype, "system/up_arrow.png");
        }

        static ImageInstance GetSystemDownArrow()
        {
            return new ImageInstance(_engine.Object.InstancePrototype, "system/down_arrow.png");
        }

        static ImageInstance LoadImage(string filename)
        {
            return new ImageInstance(_engine.Object.InstancePrototype,
                                     GlobalProps.BasePath + "/images/" + filename);
        }

        static WindowStyleInstance LoadWindowStyle(string filename)
        {
            return new WindowStyleInstance(_engine.Object.InstancePrototype,
                                           GlobalProps.BasePath + "/windowstyles/" + filename);
        }

        static FontInstance LoadFont(string filename)
        {
            return new FontInstance(_engine.Object.InstancePrototype,
                                    GlobalProps.BasePath + "/fonts/" + filename);
        }

        static SurfaceInstance CreateSurface(int w, int h, ColorInstance color)
        {
            return new SurfaceInstance(_engine.Object.InstancePrototype, w, h, color.GetColor());
        }

        static void RequireScript(string filename)
        {
            try
            {
                System.Text.Encoding ISO_8859_1 = System.Text.Encoding.GetEncoding("iso-8859-1");
                _engine.ExecuteFile(GlobalProps.BasePath + "/scripts/" + filename, ISO_8859_1);
            }
            catch (JavaScriptException ex)
            {
                Console.WriteLine(string.Format("Script error in \'{0}\', line: {1}\n{2}", ex.SourcePath, ex.LineNumber, ex.Message));
            }
        }

        static void RunCode(string code)
        {
            try
            {
                _engine.Execute(code);
            }
            catch (JavaScriptException ex)
            {
                Console.WriteLine(string.Format("Script error in \'{0}\', line: {1}\n{2}", ex.SourcePath, ex.LineNumber, ex.Message));
            }
        }

        static void window_Closed(object sender, EventArgs e)
		{
			Window window = sender as Window;
            if (window != null)
                Exit();
		}
    }
}
