﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Engine.Objects;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Engine
{
    static class Program
    {
        public static RenderWindow _window = null;
        public static ScriptEngine _engine = null;
        public static SpriteBatch Batch;
        public static IntRect _clipper = new IntRect(0, 0, 0, 0);

        private static int _internal_fps = 0;
        public static bool Scaled { get; set; }

        static GameFile _game = new GameFile();
        static string _name = "";
        public static bool FullScreen { get; set; }

        static void Main(string[] args)
        {
            string filename;
            if (args.Length == 1)
            {
                if (args[0] == "-compliance")
                {
                    Console.WriteLine("Sphere Objects: 100%");
                    Console.WriteLine("File IO: 100%");
                    Console.WriteLine("Networking: 100%");
                    Console.WriteLine("Map Engine: 75%");
                    Console.WriteLine("Other: 90%");
                    return;
                }
                else if (args[0] == "-games")
                {
                    Console.WriteLine("List of games");
                    ListGames();
                    return;
                }
                else if (args[0].EndsWith(".sgm"))
                {
                    if (!_game.ReadFile(args[0]))
                    {
                        Console.WriteLine("Faliled to load game: " + args[0]);
                        return;
                    }
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
            else if (args.Length == 2)
            {
                filename = args[1];
                if (!filename.EndsWith("game.sgm"))
                    filename += "/game.sgm";

                if (args[0] != "-game" || !_game.ReadFile(filename))
                {
                    Console.WriteLine("Failed to load game: " + filename);
                    Console.WriteLine("Useage: ");
                    Console.WriteLine("    -game <directory>  | Loads the game running at that directory.");
                    return;
                }
            }
            else if (!_game.ReadFile("startup/game.sgm"))
            {
                Console.WriteLine("Faliled to find game.sgm in startup/");
                return;
            }

            if (!InitEngine())
                return;

            _game.TryGetData("name", out _name);
            if (_game.TryGetData("script", out filename))
            {
                GlobalScripts.RequireScript(filename);
                GlobalScripts.RunCode(new StringScriptSource("game();", filename));
            }
            else
                Console.WriteLine("Invalid script file in game.sgm");
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

        public static bool InitWindow(Styles style)
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

            if (Scaled)
            {
                width *= 2;
                height *= 2;
            }
            _clipper.Width = width;
            _clipper.Height = height;

            GlobalProps.BasePath = Path.GetDirectoryName(_game.FileName);

            if (!_game.TryGetData("name", out GlobalProps.GameName))
            {
                Console.WriteLine("No name set in game.sgm.");
                return false;
            }

            if (style == Styles.Fullscreen && (width < 640 || height < 480))
            {
                width = 640;
                height = 480;
            }

            _window = new RenderWindow(new VideoMode((uint)width, (uint)height), GlobalProps.GameName, style);

            if (Scaled)
            {
                View v = _window.GetView();
                v.Size = new Vector2f(GlobalProps.Width, GlobalProps.Height);
                v.Center = new Vector2f(GlobalProps.Width / 2, GlobalProps.Height / 2);
                _window.SetView(v);
            }

            _window.SetMouseCursorVisible(false);
            GlobalInput.AddWindowHandlers(_window);
            Program._window.SetFramerateLimit((uint)_internal_fps);
            Program._window.SetMouseCursorVisible(false);

            GlobalPrimitives.Target = _window;
            Batch = new SpriteBatch(_window);

            FindIcon();
            return true;
        }

        public static ObjectInstance CreateObject()
        {
            return ObjectConstructor.Create(_engine, _engine.Object.InstancePrototype);
        }

        private static void SetGlobalConst(ScriptEngine engine, params string[] values) {
            for (int i = 0; i < values.Length; ++i) {
                engine.SetGlobalValue(values[i], i);
            }
        }

        public static ScriptEngine GetSphereEngine()
        {
            ScriptEngine engine = new ScriptEngine();
            // The glorious Sphere game API :)
            engine.SetGlobalFunction("Abort", new Action<string>(Abort));
            engine.SetGlobalFunction("GetVersion", new Func<double>(GetVersion));
            engine.SetGlobalFunction("GetVersionString", new Func<string>(GetVersionString));
            engine.SetGlobalFunction("GetExtensions", new Func<ArrayInstance>(GetExtensions));
            engine.SetGlobalFunction("FlipScreen", new Action(FlipScreen));
            engine.SetGlobalFunction("GetScreenWidth", new Func<int>(GetScreenWidth));
            engine.SetGlobalFunction("GetScreenHeight", new Func<int>(GetScreenHeight));
            engine.SetGlobalFunction("Print", new Action<string>(Print));
            engine.SetGlobalFunction("GarbageCollect", new Action(GarbageCollect));
            engine.SetGlobalFunction("Exit", new Action(Exit));
            engine.SetGlobalFunction("CreateColor", new Func<int, int, int, int, ColorInstance>(CreateColor));
            engine.SetGlobalFunction("LoadImage", new Func<string, ImageInstance>(LoadImage));
            engine.SetGlobalFunction("LoadSound", new Func<string, SoundInstance>(LoadSound));
            engine.SetGlobalFunction("LoadSurface", new Func<string, ObjectInstance>(LoadSurface));
            engine.SetGlobalFunction("LoadWindowStyle", new Func<string, WindowStyleInstance>(LoadWindowStyle));
            engine.SetGlobalFunction("LoadSpriteset", new Func<string, SpritesetInstance>(LoadSpriteset));
            engine.SetGlobalFunction("CreateSpriteset", new Func<int, int, int, int, int, SpritesetInstance>(CreateSpriteset));
            engine.SetGlobalFunction("LoadFont", new Func<string, FontInstance>(LoadFont));
            engine.SetGlobalFunction("GetSystemFont", new Func<FontInstance>(GetSystemFont));
            engine.SetGlobalFunction("GetSystemWindowStyle", new Func<WindowStyleInstance>(GetSystemWindowStyle));
            engine.SetGlobalFunction("GetSystemArrow", new Func<ImageInstance>(GetSystemArrow));
            engine.SetGlobalFunction("GetSystemUpArrow", new Func<ImageInstance>(GetSystemUpArrow));
            engine.SetGlobalFunction("GetSystemDownArrow", new Func<ImageInstance>(GetSystemDownArrow));
            engine.SetGlobalFunction("Triangle", new Action<double, double, double, double, double, double, ColorInstance>(GlobalPrimitives.Triangle));
            engine.SetGlobalFunction("GradientTriangle", new Action<ObjectInstance, ObjectInstance, ObjectInstance, ColorInstance, ColorInstance, ColorInstance>(GlobalPrimitives.GradientTriangle));
            engine.SetGlobalFunction("Rectangle", new Action<double, double, double, double, ColorInstance>(GlobalPrimitives.Rectangle));
            engine.SetGlobalFunction("OutlinedRectangle", new Action<double, double, double, double, ColorInstance, double>(GlobalPrimitives.OutlinedRectangle));
            engine.SetGlobalFunction("GradientRectangle", new Action<double, double, double, double, ColorInstance, ColorInstance, ColorInstance, ColorInstance>(GlobalPrimitives.GradientRectangle));
            engine.SetGlobalFunction("OutlinedCircle", new Action<double, double, double, ColorInstance, double>(GlobalPrimitives.OutlinedCircle));
            engine.SetGlobalFunction("GradientCircle", new Action<double, double, double, ColorInstance, ColorInstance, bool>(GlobalPrimitives.GradientCircle));
            engine.SetGlobalFunction("FilledCircle", new Action<double, double, double, ColorInstance>(GlobalPrimitives.FilledCircle));
            engine.SetGlobalFunction("Line", new Action<double, double, double, double, ColorInstance>(GlobalPrimitives.Line));
            engine.SetGlobalFunction("LineSeries", new Action<ArrayInstance, ColorInstance>(GlobalPrimitives.LineSeries));
            engine.SetGlobalFunction("Point", new Action<double, double, ColorInstance>(GlobalPrimitives.Point));
            engine.SetGlobalFunction("PointSeries", new Action<ArrayInstance, ColorInstance>(GlobalPrimitives.PointSeries));
            engine.SetGlobalFunction("Polygon", new Action<ArrayInstance, ColorInstance, bool>(GlobalPrimitives.Polygon));
            engine.SetGlobalFunction("GradientLine", new Action<double, double, double, double, ColorInstance, ColorInstance>(GlobalPrimitives.GradientLine));
            engine.SetGlobalFunction("ApplyColorMask", new Action<ColorInstance>(GlobalPrimitives.ApplyColorMask));
            engine.SetGlobalFunction("CreateSurface", new Func<int, int, ColorInstance, ObjectInstance>(CreateSurface)); 
            engine.SetGlobalFunction("GrabImage", new Func<int, int, int, int, ImageInstance>(GrabImage));
            engine.SetGlobalFunction("GrabSurface", new Func<int, int, int, int, SurfaceInstance>(GrabSurface));
            engine.SetGlobalFunction("SetFrameRate", new Action<int>(SetFrameRate));
            engine.SetGlobalFunction("GetFrameRate", new Func<int>(GetFrameRate));
            engine.SetGlobalFunction("GetTime", new Func<double>(GetTime));
            engine.SetGlobalFunction("BlendColors", new Func<ColorInstance, ColorInstance, ColorInstance>(BlendColors));
            engine.SetGlobalFunction("BlendColorsWeighted", new Func<ColorInstance, ColorInstance, double, ColorInstance>(BlendColorsWeighted));
            engine.SetGlobalFunction("IsKeyPressed", new Func<int, bool>(GlobalInput.IsKeyPressed));
            engine.SetGlobalFunction("IsAnyKeyPressed", new Func<bool>(GlobalInput.IsAnyKeyPressed));
            engine.SetGlobalFunction("IsMouseButtonPressed", new Func<int, bool>(GlobalInput.IsMouseButtonPressed));
            engine.SetGlobalFunction("GetMouseWheelEvent", new Func<int>(GlobalInput.GetMouseWheelEvent));
            engine.SetGlobalFunction("GetNumMouseWheelEvents", new Func<int>(GlobalInput.GetNumMouseWheelEvents));
            engine.SetGlobalFunction("IsJoystickButtonPressed", new Func<int, int, bool>(GlobalInput.IsJoystickButtonPressed));
            engine.SetGlobalFunction("GetJoystickAxis", new Func<int, int, double>(GlobalInput.GetJoystickAxis));
            engine.SetGlobalFunction("GetNumJoystickAxes", new Func<int, int>(GlobalInput.GetNumJoystickAxes));
            engine.SetGlobalFunction("AreKeysLeft", new Func<bool>(GlobalInput.AreKeysLeft));
            engine.SetGlobalFunction("GetKey", new Func<int>(GlobalInput.GetKey));
            engine.SetGlobalFunction("GetKeyString", new Func<int, bool, string>(GlobalInput.GetKeyString));
            engine.SetGlobalFunction("SetTalkActivationKey", new Action<int>(GlobalInput.SetTalkActivationKey));
            engine.SetGlobalFunction("SetTalkActivationButton", new Action<int>(GlobalInput.SetTalkActivationButton));
            engine.SetGlobalFunction("GetTalkActivationKey", new Func<int>(GlobalInput.GetTalkActivationKey));
            engine.SetGlobalFunction("GetTalkActivationButton", new Func<int>(GlobalInput.GetTalkActivationButton));
            engine.SetGlobalFunction("GetMouseX", new Func<int>(GlobalInput.GetMouseX));
            engine.SetGlobalFunction("GetMouseY", new Func<int>(GlobalInput.GetMouseY));
            engine.SetGlobalFunction("SetMouseX", new Action<int>(GlobalInput.SetMouseX));
            engine.SetGlobalFunction("SetMouseY", new Action<int>(GlobalInput.SetMouseY));
            engine.SetGlobalFunction("SetMousePosition", new Action<int, int>(GlobalInput.SetMousePosition));
            engine.SetGlobalFunction("BindKey", new Action<int, string, string>(GlobalInput.BindKey));
            engine.SetGlobalFunction("UnbindKey", new Action<int>(GlobalInput.UnbindKey));
            engine.SetGlobalFunction("GetNumJoysticks", new Func<int>(GlobalInput.GetNumJoySticks));
            engine.SetGlobalFunction("GetNumJoystickButtons", new Func<int, int>(GlobalInput.GetNumJoyStickButtons));
            engine.SetGlobalFunction("OpenFile", new Func<string, FileInstance>(OpenFile));
            engine.SetGlobalFunction("OpenRawFile", new Func<string, bool, RawFileInstance>(OpenRawFile));
            engine.SetGlobalFunction("CreateByteArray", new Func<int, ByteArrayInstance>(CreateByteArray));
            engine.SetGlobalFunction("CreateByteArrayFromString", new Func<string, ByteArrayInstance>(CreateByteArrayFromString));
            engine.SetGlobalFunction("CreateStringFromByteArray", new Func<ByteArrayInstance, string>(CreateStringFromByteArray));
            engine.SetGlobalFunction("CreateDirectory", new Action<string>(CreateDirectory));
            engine.SetGlobalFunction("GetDirectoryList", new Func<string, ArrayInstance>(GetDirectoryList));
            engine.SetGlobalFunction("GetFileList", new Func<string, ArrayInstance>(GetFileList));
            engine.SetGlobalFunction("RemoveDirectory", new Action<string>(RemoveDirectory));
            engine.SetGlobalFunction("RemoveFile", new Action<string>(RemoveFile));
            engine.SetGlobalFunction("Rename", new Func<string, string, bool>(Rename));
            engine.SetGlobalFunction("HashFromFile", new Func<string, bool, string>(HashFromFile));
            engine.SetGlobalFunction("HashByteArray", new Func<ByteArrayInstance, bool, string>(HashByteArray));
            engine.SetGlobalFunction("CreateStringFromCode", new Func<int, string>(CreateStringFromCode));
            engine.SetGlobalFunction("SetScaled", new Action<bool>(SetScaled));
            engine.SetGlobalFunction("GetGameList", new Func<ArrayInstance>(GetGameList));
            engine.SetGlobalFunction("SetClippingRectangle", new Action<int, int, int, int>(SetClippingRectangle));
            engine.SetGlobalFunction("GetClippingRectangle", new Func<ObjectInstance>(GetClippingRectangle));
            engine.SetGlobalFunction("ListenOnPort", new Func<int, object>(ListenOnPort));
            engine.SetGlobalFunction("OpenAddress", new Func<string, int, object>(OpenAddress));
            engine.SetGlobalFunction("GetLocalAddress", new Func<string>(GetLocalAddress));
            engine.SetGlobalFunction("GetLocalName", new Func<string>(GetLocalName));
            engine.SetGlobalFunction("SettFullScreen", new Action<bool>(SetFullScreen));
            engine.SetGlobalFunction("IsFullScreen", new Func<bool>(IsFullScreen));
            engine.SetGlobalFunction("LineIntersects", new Func<ObjectInstance, ObjectInstance, ObjectInstance, ObjectInstance, bool>(LineIntersects));
            engine.SetGlobalValue("BinaryHeap", new BinHeapConstructor(engine));
            engine.SetGlobalValue("XmlFile", new XMLDocConstructor(engine));

            GlobalScripts.BindToEngine(engine);
            PersonManager.BindToEngine(engine);
            MapEngineHandler.BindToEngine(engine);
            ParticleEngine.BindToEngine(engine);

            // keys:
            Array a = Enum.GetValues(typeof(Keyboard.Key));
            string[] n = Enum.GetNames(typeof(Keyboard.Key));
            Dictionary<string, string> keymaps = new Dictionary<string, string>();
            keymaps["RETURN"] = "ENTER";
            keymaps["RSHIFT"] = "SHIFT";
            keymaps["RCONTROL"] = "CTRL";
            keymaps["RALT"] = "ALT";
            keymaps["BACK"] = "BACKSPACE";
            keymaps["SUBTRACT"] = "MINUS";
            keymaps["EQUAL"] = "EQUALS";
            for (var i = 0; i < a.Length; ++i)
            {
                string key = n[i].ToUpper();
                if (keymaps.ContainsKey(key))
                    key = keymaps[key];
                else if (key.StartsWith("NUM"))
                    key = key[3].ToString();

                engine.SetGlobalValue("KEY_" + key, (int)a.GetValue(i));
            }

            SetGlobalConst(engine, "BLEND", "REPLACE", "RGB_ONLY", "ALPHA_ONLY", "ADD", "SUBTRACT", "MULTPLY", "AVERAGE");

            engine.SetGlobalValue("MOUSE_LEFT", (int)Mouse.Button.Left);
            engine.SetGlobalValue("MOUSE_RIGHT", (int)Mouse.Button.Right);
            engine.SetGlobalValue("MOUSE_MIDDLE", (int)Mouse.Button.Middle);
            engine.SetGlobalValue("MOUSE_EX1", (int)Mouse.Button.XButton1);
            engine.SetGlobalValue("MOUSE_EX2", (int)Mouse.Button.XButton2);


            engine.SetGlobalValue("COMMAND_WAIT", 0);
            engine.SetGlobalValue("COMMAND_ANIMATE", 1);
            engine.SetGlobalValue("COMMAND_FACE_NORTH", 2);
            engine.SetGlobalValue("COMMAND_FACE_NORTHEAST", 3);
            engine.SetGlobalValue("COMMAND_FACE_EAST", 4);
            engine.SetGlobalValue("COMMAND_FACE_SOUTHEAST", 5);
            engine.SetGlobalValue("COMMAND_FACE_SOUTH", 6);
            engine.SetGlobalValue("COMMAND_FACE_SOUTHWEST", 7);
            engine.SetGlobalValue("COMMAND_FACE_WEST", 8);
            engine.SetGlobalValue("COMMAND_FACE_NORTHWEST", 9);
            engine.SetGlobalValue("COMMAND_MOVE_NORTH", 10);
            engine.SetGlobalValue("COMMAND_MOVE_EAST", 11);
            engine.SetGlobalValue("COMMAND_MOVE_SOUTH", 12);
            engine.SetGlobalValue("COMMAND_MOVE_WEST", 13);

            SetGlobalConst(engine, "SCRIPT_ON_ENTER_MAP", "SCRIPT_ON_LEAVE_MAP", "SCRIPT_ON_LEAVE_MAP_NORTH",
                           "SCRIPT_ON_LEAVE_MAP_EAST", "SCRIPT_ON_LEAVE_MAP_SOUTH", "SCRIPT_ON_LEAVE_MAP_WEST");

            SetGlobalConst(engine, "SCRIPT_ON_CREATE", "SCRIPT_ON_DESTROY", "SCRIPT_ON_ACTIVATE_TOUCH",
                           "SCRIPT_ON_ACTIVATE_TALK", "SCRIPT_COMMAND_GENERATOR");

            SetGlobalConst(engine, "JOYSTICK_AXIS_X", "JOYSTICK_AXIS_Y", "JOYSTICK_AXIS_Z", "JOYSTICK_AXIS_R");

            return engine;
        }

        public static void SetupTestEnvironment()
        {
            if (_window != null)
                return;

            _window = new RenderWindow(new VideoMode(320, 240), "Test Window", Styles.Default);
            _window.Clear(Color.Black);
            _window.Display();
            _engine = GetSphereEngine();
        }

        public static void Exit()
        {
            _window.Close();

            // Sadly, it's the only way to kill a while(true){}; in the JS environment.
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            proc.Kill();
        }

        public static void SetScaled(bool v) {
            Scaled = v;
            if (_window != null)
            {
                GlobalInput.RemoveWindowHandlers(_window);
                _window.Close();
            }

            Program.InitWindow(Styles.Default);
            if (MapEngineHandler.FPSToggle)
            {
                MapEngineHandler.ToggleFPSThrottle();
                MapEngineHandler.ToggleFPSThrottle();
            }
        }

        public static bool IsFullScreen()
        {
            return FullScreen;
        }

        public static void SetFullScreen(bool value)
        {
            if (_window != null)
            {
                GlobalInput.RemoveWindowHandlers(_window);
                _window.Close();
            }

            FullScreen = value;
            var style = (FullScreen) ? Styles.Fullscreen : Styles.Default;

            Program.InitWindow(style);
            if (MapEngineHandler.FPSToggle)
            {
                MapEngineHandler.ToggleFPSThrottle();
                MapEngineHandler.ToggleFPSThrottle();
            }
        }

        public static void SetFrameRate(int fps)
        {
            _window.SetFramerateLimit((uint)fps);
            _internal_fps = fps;
        }

        public static int GetFrameRate()
        {
            return _internal_fps;
        }

        public static ColorInstance BlendColors(Color c1, Color c2)
        {
            return BlendColorsWeighted(c1, c2, 0.5);
        }

        static ColorInstance BlendColors(ColorInstance c1, ColorInstance c2)
        {
            return BlendColorsWeighted(c1, c2, 0.5);
        }

        static string CreateStringFromCode(int code)
        {
            return ((char)code).ToString();
        }

        static void SetClippingRectangle(int x, int y, int w, int h)
        {
            float sh = GetScreenHeight();
            float sw = GetScreenWidth();
            _clipper = new IntRect(x, y, w, h);
            View v = _window.GetView();
            v.Reset(new FloatRect(x, y, w, h));
            v.Viewport = new FloatRect(x / sw, y / sh, w / sw, h / sh);
            _window.SetView(v);
        }

        static ObjectInstance GetClippingRectangle()
        {
            ObjectInstance o = CreateObject();
            o["x"] = _clipper.Left;
            o["y"] = _clipper.Top;
            o["width"] = _clipper.Width;
            o["height"] = _clipper.Height;
            return o;
        }

        public static ColorInstance BlendColorsWeighted(Color c1, Color c2, double w)
        {
            int R = (int)(c1.R * w + c2.R * (1 - w));
            int G = (int)(c1.G * w + c2.G * (1 - w));
            int B = (int)(c1.B * w + c2.B * (1 - w));
            int A = (int)(c1.A * w + c2.A * (1 - w));
            return new ColorInstance(_engine, R, G, B, A);
        }

        public static ColorInstance BlendColorsWeighted(ColorInstance c1, ColorInstance c2, double w)
        {
            int R = (int)((int)c1["red"] * w + (int)c2["red"] * (1 - w));
            int G = (int)((int)c1["green"] * w + (int)c2["green"] * (1 - w));
            int B = (int)((int)c1["blue"] * w + (int)c2["blue"] * (1 - w));
            int A = (int)((int)c1["alpha"] * w + (int)c2["alpha"] * (1 - w));
            return new ColorInstance(_engine, R, G, B, A);
        }

        static void Print(object obj)
        {
            Console.WriteLine(obj);
        }

        public static double GetTime()
        {
            return Math.Floor(DateInstance.Now());
        }

        static object OpenAddress(string address, int port)
        {
            SocketInstance instance = null;

            try
            {
                instance = new SocketInstance(_engine, address, port);
            }
            catch (Exception) { /* Swallow Invalid Connection Attempts */ }

            return instance;
        }

        static object ListenOnPort(int port)
        {
            SocketInstance instance = null;

            

            return instance;
        }

        static string GetLocalName()
        {
            return System.Net.Dns.GetHostName();
        }

        static string GetLocalAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (System.Net.IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        static ColorInstance CreateColor(int r, int g, int b, [DefaultParameterValue(255)] int a = 255)
        {
            return new ColorInstance(_engine, r, g, b, a);
        }

        static ImageInstance GrabImage(int x, int y, int w, int h)
        {
            Batch.Flush();
            w *= (Scaled ? 2 : 1);
            h *= (Scaled ? 2 : 1);
            Texture tex = new Texture((uint)w, (uint)h);
            tex.Update(_window, (uint)x, (uint)y);
            return new ImageInstance(_engine, tex, false);
        }

        static SurfaceInstance GrabSurface(int x, int y, int w, int h)
        {
            Batch.Flush();
            x *= (Scaled ? 2 : 1);
            y *= (Scaled ? 2 : 1);
            w *= (Scaled ? 2 : 1);
            h *= (Scaled ? 2 : 1);
            using (Image window = _window.Capture())
            {
                Image section = new Image((uint)w, (uint)h);
                section.Copy(window, 0, 0, new IntRect(x, y, w, h));
                return new SurfaceInstance(_engine, section.Pixels, w, h);
            }
        }

        static DateTime _start = DateTime.Now;
        static int _fps = 0;
        public static void FlipScreen()
        {
            Batch.Flush();

            _window.DispatchEvents();
            _window.Display();
            _window.Clear();

            _fps++;
            if ((DateTime.Now - _start).Seconds >= 1)
            {
                _window.SetTitle(_name + " (FPS: " + _fps + ")");
                _fps = 0;
                _start = DateTime.Now;
            }
        }

        public static int GetScreenWidth()
        {
            return GlobalProps.Width;
        }

        public static int GetScreenHeight()
        {
            return GlobalProps.Height;
        }

        public static void Abort(string msg)
        {
            ShowAbortScreen(msg);
            Exit();
        }

        static bool LineIntersects(ObjectInstance a1, ObjectInstance b1, ObjectInstance a2, ObjectInstance b2)
        {
            Vector2f start1 = GlobalPrimitives.GetVector(a1);
            Vector2f end1 = GlobalPrimitives.GetVector(b1);
            Vector2f start2 = GlobalPrimitives.GetVector(a2);
            Vector2f end2 = GlobalPrimitives.GetVector(b2);

            return Line.Intersects(new Line(start1, end1), new Line(start2, end2));
        }

        static double GetVersion()
        {
            return 1.5;
        }

        static string GetVersionString()
        {
            return "v1.5 (compatible; Sphere-SFML 0.90)";
        }

        static ArrayInstance GetExtensions()
        {
            return Program._engine.Array.New(new[] {
                "sphere-legacy-api",
                "sphere-sfml",
                "set-script-function"
            });
        }

        static void GarbageCollect()
        {
            GC.Collect();
        }

        static void FindIcon()
        {
            Image icon = null;

            if (File.Exists(GlobalProps.BasePath + "/icon.png"))
                icon = new Image(GlobalProps.BasePath + "/icon.png");
            else if (File.Exists(GlobalProps.EnginePath + "/icon.png"))
                icon = new Image(GlobalProps.EnginePath + "/icon.png");

            if (icon != null)
            {
                _window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);
                icon.Dispose();
            }
        }

        static WindowStyleInstance GetSystemWindowStyle()
        {
            return new WindowStyleInstance(_engine, GlobalProps.EnginePath + "/system/system.rws");
        }

        public static FontInstance GetSystemFont()
        {
            return new FontInstance(_engine, GlobalProps.EnginePath + "/system/system.rfn");
        }

        static ImageInstance GetSystemArrow()
        {
            return new ImageInstance(_engine, GlobalProps.EnginePath + "/system/pointer.png");
        }

        static ImageInstance GetSystemUpArrow()
        {
            return new ImageInstance(_engine, GlobalProps.EnginePath + "/system/up_arrow.png");
        }

        static ImageInstance GetSystemDownArrow()
        {
            return new ImageInstance(_engine, GlobalProps.EnginePath + "/system/down_arrow.png");
        }

        /// <summary>
        /// Sandboxes the game.sgm, allows to peek a level behind.
        /// </summary>
        /// <returns>The sphere path.</returns>
        /// <param name="path">Path.</param>
        /// <param name="root">Root.</param>
        public static string ParseSpherePath(string path, string root)
        {
            if (path.StartsWith("~") || path.StartsWith("."))
                return GlobalProps.BasePath + "/" + path.Substring(path.StartsWith("..") ? 3 : 2);
            else if (root == "")
                return GlobalProps.BasePath + "/" + path;
            else
                return GlobalProps.BasePath + "/" + root + "/" + path;
        }

        static ImageInstance LoadImage(string filename)
        {
            return new ImageInstance(_engine, ParseSpherePath(filename, "images"));
        }

        static SoundInstance LoadSound(string filename)
        {
            return new SoundInstance(_engine, ParseSpherePath(filename, "sounds"));
        }

        static HWSurfaceInstance LoadSurface(string filename)
        {
            return new HWSurfaceInstance(_engine, ParseSpherePath(filename, "images"));
        }

        static WindowStyleInstance LoadWindowStyle(string filename)
        {
            return new WindowStyleInstance(_engine, ParseSpherePath(filename, "windowstyles"));
        }

        static SpritesetInstance LoadSpriteset(string filename)
        {
            return AssetManager.GetSpriteset(filename);
        }

        static SpritesetInstance CreateSpriteset(int width, int height, int i, int d, int f)
        {
            return new SpritesetInstance(_engine, width, height, i, d, f);
        }

        static FontInstance LoadFont(string filename)
        {
            return new FontInstance(_engine, ParseSpherePath(filename, "fonts"));
        }

        static HWSurfaceInstance CreateSurface(int w, int h, ColorInstance color)
        {
            return new HWSurfaceInstance(_engine, w, h, color.Color);
        }

        static FileInstance OpenFile(string filename)
        {
            return new FileInstance(_engine, ParseSpherePath(filename, "save"));
        }

        static RawFileInstance OpenRawFile(string filename, [DefaultParameterValue(false)] bool writeable = false)
        {
            return new RawFileInstance(_engine, ParseSpherePath(filename, "other"), writeable);
        }

        static ByteArrayInstance CreateByteArray(int size)
        {
            return new ByteArrayInstance(_engine, new byte[size]);
        }

        static ByteArrayInstance CreateByteArrayFromString(string content)
        {
            return new ByteArrayInstance(_engine, content);
        }

        static string CreateStringFromByteArray(ByteArrayInstance array)
        {
            byte[] data = array.GetBytes();
            string result = "";
            for (var i = 0; i < data.Length; ++i) result += (char)data[i];
            return result;
        }

        static ArrayInstance GetFileList([DefaultParameterValue("")] string filepath = "")
        {
            if (string.IsNullOrEmpty(filepath))
                filepath = GlobalProps.BasePath + "/save/";
            else
                filepath = ParseSpherePath(filepath, "");

            if (!Directory.Exists(filepath))
                return _engine.Array.New();

            string[] files = Directory.GetFiles(filepath);
            string[] names = new string[files.Length];
            for (var i = 0; i < names.Length; ++i)
                names[i] = Path.GetFileName(files[i]);
            return _engine.Array.New(names);
        }

        static void RemoveDirectory(string filepath)
        {
            if (Directory.Exists(filepath))
                Directory.Delete(filepath);
        }

        static void RemoveFile(string filepath)
        {
            if (File.Exists(filepath))
                File.Delete(filepath);
        }

        static ArrayInstance GetDirectoryList([DefaultParameterValue("")] string filepath = "")
        {
            if (string.IsNullOrEmpty(filepath))
                filepath = GlobalProps.BasePath;
            else
                filepath = ParseSpherePath(filepath, "");

            if (!Directory.Exists(filepath))
                return _engine.Array.New();

            string[] dirs = Directory.GetDirectories(filepath);
            string[] names = new string[dirs.Length];
            for (var i = 0; i < names.Length; ++i)
                names[i] = Path.GetFileName(dirs[i]);
            return _engine.Array.New(names);
        }

        static void CreateDirectory(string fullname)
        {
            if (string.IsNullOrEmpty(fullname))
                fullname = GlobalProps.BasePath + "/" + fullname;
            Directory.CreateDirectory(fullname);
        }

        static ArrayInstance GetGameList()
        {
            string directory = GlobalProps.EnginePath + "/games";
            ArrayInstance array = _engine.Array.New();
            if (!Directory.Exists(directory)) return array;

            string[] files = Directory.GetDirectories(directory);
            for (uint i = 0; i < files.Length; ++i) {
                GameFile file = new GameFile();
                if (!file.ReadFile(files[i] + "/game.sgm")) continue;
                string name;

                ObjectInstance obj = CreateObject();
                obj["name"] = (file.TryGetData("name", out name)) ? name : "";
                obj["description"] = (file.TryGetData("description", out name)) ? name : "";
                obj["author"] = (file.TryGetData("author", out name)) ? name : "";
                obj["directory"] = Path.GetFileName(files[i]);

                ArrayInstance.Push(array, obj);
            }

            return array;
        }

        static string HashFromFile(string filename, [DefaultParameterValue(false)] bool sha = false)
        {
            if (!File.Exists(filename))
                return "";

            HashAlgorithm hash;
            if (sha)
                hash = SHA1.Create();
            else
                hash = MD5.Create();

            using (hash)
            {
                return GetHash(hash, File.ReadAllBytes(filename));
            }
        }

        static string GetHash(HashAlgorithm algorithm, byte[] content)
        {
            byte[] data = algorithm.ComputeHash(content);
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            for (int i = 0; i < data.Length; i++)
                builder.Append(data[i].ToString("x2"));

            // Return the hexadecimal string. 
            return builder.ToString();
        }

        static string HashByteArray(ByteArrayInstance array, [DefaultParameterValue(false)] bool sha = false)
        {
            HashAlgorithm hash;
            if (sha)
                hash = SHA1.Create();
            else
                hash = MD5.Create();

            using (hash)
            {
                return GetHash(hash, array.GetBytes());
            }
        }

        static bool Rename(string file1, string file2)
        {
            if (!File.Exists(file1) || File.Exists(file2))
                return false;
            File.Move(file1, file2);
            return true;
        }

        public static void ShowAbortScreen(string message)
        {
            FontInstance font = GetSystemFont();
            _window.SetView(MapEngineHandler.DefaultView);
            var done = false;
            while (GlobalInput.AreKeysLeft()) { GlobalInput.GetKey(); FlipScreen(); }
            while (!done)
            {
                font.DrawTextBox(0, 0, GlobalProps.Width, GlobalProps.Height, 0, message);
                FlipScreen();

                //while (GlobalInput.AreKeysLeft())
                    //done = GlobalInput.GetKey() >= 0;
            }
        }
    }
}
