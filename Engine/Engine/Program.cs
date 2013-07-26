using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
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
        public static IntRect _clipper = new IntRect(0, 0, 0, 0);

        private static int _internal_fps = 0;
        private static bool SCALED = false;
        private static readonly bool DEBUG = false;

        static GameFile _game = new GameFile();

        static void Main(string[] args)
        {
            string filename;
            if (args.Length == 1)
            {
                if (args[0] == "-compliance")
                {
                    Console.WriteLine("Sphere Objects: 100%");
                    Console.WriteLine("File IO: 80%");
                    Console.WriteLine("Networking: 0%");
                    Console.WriteLine("Map Engine: 10%");
                    Console.WriteLine("Other: 85%");
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

            if (_game.TryGetData("script", out filename))
            {
                GlobalScripts.RequireScript(filename);
                GlobalScripts.RunCode(new StringScriptSource("game();", filename));
                GlobalScripts.RunCode(new StringScriptSource("GetKey();", filename));
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

            if (SCALED)
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

            _window = new RenderWindow(new VideoMode((uint)width, (uint)height), GlobalProps.GameName, style);
            _clipper.Width = (int)width;
            _clipper.Height = (int)height;

            if (SCALED)
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

            GlobalPrimitives.window = _window;
            FindIcon();
            return true;
        }

        public static ObjectInstance CreateObject()
        {
            return ObjectConstructor.Create(_engine, _engine.Object.InstancePrototype);
        }

        public static ScriptEngine GetSphereEngine()
        {
            ScriptEngine engine = new ScriptEngine();
            engine.EnableDebugging = DEBUG;

            // The glorious Sphere game API :)
            engine.SetGlobalFunction("Abort", new Action<string>(Abort));
            engine.SetGlobalFunction("GetVersion", new Func<double>(GetVersion));
            engine.SetGlobalFunction("GetVersionString", new Func<string>(GetVersionString));
            engine.SetGlobalFunction("FlipScreen", new Action(FlipScreen));
            engine.SetGlobalFunction("GetScreenWidth", new Func<int>(GetScreenWidth));
            engine.SetGlobalFunction("GetScreenHeight", new Func<int>(GetScreenHeight));
            engine.SetGlobalFunction("Print", new Action<string>(Print));
            engine.SetGlobalFunction("Exit", new Action(Exit));
            engine.SetGlobalFunction("CreateColor", new Func<int, int, int, int, ColorInstance>(CreateColor));
            engine.SetGlobalFunction("LoadImage", new Func<string, ImageInstance>(LoadImage));
            engine.SetGlobalFunction("LoadSound", new Func<string, SoundInstance>(LoadSound));
            engine.SetGlobalFunction("LoadSurface", new Func<string, SurfaceInstance>(LoadSurface));
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
            engine.SetGlobalFunction("OutlinedCircle", new Action<double, double, double, ColorInstance>(GlobalPrimitives.OutlinedCircle));
            engine.SetGlobalFunction("FilledCircle", new Action<double, double, double, ColorInstance>(GlobalPrimitives.FilledCircle));
            engine.SetGlobalFunction("Line", new Action<double, double, double, double, ColorInstance>(GlobalPrimitives.Line));
            engine.SetGlobalFunction("LineSeries", new Action<ArrayInstance, ColorInstance>(GlobalPrimitives.LineSeries));
            engine.SetGlobalFunction("Point", new Action<double, double, ColorInstance>(GlobalPrimitives.Point));
            engine.SetGlobalFunction("PointSeries", new Action<ArrayInstance, ColorInstance>(GlobalPrimitives.PointSeries));
            engine.SetGlobalFunction("Polygon", new Action<ArrayInstance, ColorInstance, bool>(GlobalPrimitives.Polygon));
            engine.SetGlobalFunction("GradientLine", new Action<double, double, double, double, ColorInstance, ColorInstance>(GlobalPrimitives.GradientLine));
            engine.SetGlobalFunction("ApplyColorMask", new Action<ColorInstance>(GlobalPrimitives.ApplyColorMask));
            engine.SetGlobalFunction("CreateSurface", new Func<int, int, ColorInstance, SurfaceInstance>(CreateSurface)); 
            engine.SetGlobalFunction("GrabImage", new Func<int, int, int, int, ImageInstance>(GrabImage));
            engine.SetGlobalFunction("GrabSurface", new Func<int, int, int, int, SurfaceInstance>(GrabSurface));
            engine.SetGlobalFunction("SetFrameRate", new Action<int>(SetFrameRate));
            engine.SetGlobalFunction("GetFrameRate", new Func<int>(GetFrameRate));
            engine.SetGlobalFunction("GetTime", new Func<double>(GetTime));
            engine.SetGlobalFunction("BlendColors", new Func<ColorInstance, ColorInstance, ColorInstance>(BlendColors));
            engine.SetGlobalFunction("BlendColorsWeighted", new Func<ColorInstance, ColorInstance, double, ColorInstance>(BlendColorsWeighted));
            engine.SetGlobalFunction("IsKeyPressed", new Func<int, bool>(GlobalInput.IsKeyPressed));
            engine.SetGlobalFunction("IsAnyKeyPressed", new Func<int, bool>(GlobalInput.IsKeyPressed));
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
            engine.SetGlobalFunction("HashFromFile", new Func<string, string>(HashFromFile));
            engine.SetGlobalFunction("HashByteArray", new Func<ByteArrayInstance, string>(HashByteArray));
            engine.SetGlobalFunction("CreateStringFromCode", new Func<int, string>(CreateStringFromCode));
            engine.SetGlobalFunction("SetScaled", new Action<bool>(SetScaled));
            engine.SetGlobalFunction("GetGameList", new Func<ArrayInstance>(GetGameList));
            engine.SetGlobalFunction("SetClippingRectangle", new Action<int, int, int, int>(SetClippingRectangle));
            engine.SetGlobalFunction("GetClippingRectangle", new Func<ObjectInstance>(GetClippingRectangle));
            GlobalScripts.BindToEngine(engine);
            PersonManager.BindToEngine(engine);
            MapEngineHandler.BindToEngine(engine);

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
                if (key == "BACK")
                    key = "BACKSPACE";
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

            engine.SetGlobalValue("SCRIPT_ON_ENTER_MAP", 0);
            engine.SetGlobalValue("SCRIPT_ON_LEAVE_MAP", 1);
            engine.SetGlobalValue("SCRIPT_ON_LEAVE_MAP_NORTH", 2);
            engine.SetGlobalValue("SCRIPT_ON_LEAVE_MAP_EAST", 3);
            engine.SetGlobalValue("SCRIPT_ON_LEAVE_MAP_SOUTH", 4);
            engine.SetGlobalValue("SCRIPT_ON_LEAVE_MAP_WEST", 5);

            engine.SetGlobalValue("SCRIPT_ON_CREATE", 0);
            engine.SetGlobalValue("SCRIPT_ON_DESTROY", 1);
            engine.SetGlobalValue("SCRIPT_ON_ACTIVATE_TOUCH", 2);
            engine.SetGlobalValue("SCRIPT_ON_ACTIVATE_TALK", 3);
            engine.SetGlobalValue("SCRIPT_COMMAND_GENERATOR", 4);

            return engine;
        }

        public static void SetupTestEnvironment()
        {
            if (_window != null)
                return;

            _window = new RenderWindow(new VideoMode(320, 240), "Test Window", Styles.Default);
            _engine = GetSphereEngine();
        }

		public static void Exit()
		{
			_window.Close();

            // Sadly, it's the only way to kill a while(true){}; in the JS environment.
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            proc.Kill();
		}

        private static void SetScaled(bool v) {
            SCALED = v;
            if (_window != null)
            {
                GlobalInput.RemoveWindowHandlers(_window);
                _window.Close();
            }

            Program.InitWindow(Styles.Default);
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
            return DateInstance.Now();
        }

        static ColorInstance CreateColor(int r, int g, int b, [DefaultParameterValue(255)] int a = 255)
		{
            return new ColorInstance(_engine, r, g, b, a);
		}

        static ImageInstance GrabImage(int x, int y, int w, int h)
        {
            x *= (SCALED ? 2 : 1);
            y *= (SCALED ? 2 : 1);
            w *= (SCALED ? 2 : 1);
            h *= (SCALED ? 2 : 1);
            Texture tex = new Texture((uint)w, (uint)h);
            tex.Update(_window);
            return new ImageInstance(_engine, tex, false);
        }

        static SurfaceInstance GrabSurface(int x, int y, int w, int h)
        {
            x *= (SCALED ? 2 : 1);
            y *= (SCALED ? 2 : 1);
            w *= (SCALED ? 2 : 1);
            h *= (SCALED ? 2 : 1);
            using (Image window = _window.Capture())
            {
                Image section = new Image((uint)w, (uint)h);
                section.Copy(window, 0, 0, new IntRect(x, y, w, h));
                return new SurfaceInstance(_engine, section, false);
            }
        }

        static DateTime _start = DateTime.Now;
        static int _fps = 0;
        public static void FlipScreen()
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

        static FontInstance GetSystemFont()
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

        static SurfaceInstance LoadSurface(string filename)
        {
            return new SurfaceInstance(_engine, ParseSpherePath(filename, "images"));
        }

        static WindowStyleInstance LoadWindowStyle(string filename)
        {
            return new WindowStyleInstance(_engine, ParseSpherePath(filename, "windowstyles"));
        }

        static SpritesetInstance LoadSpriteset(string filename)
        {
            return new SpritesetInstance(_engine, ParseSpherePath(filename, "spritesets"));
        }

        static SpritesetInstance CreateSpriteset(int width, int height, int i, int d, int f)
        {
            return new SpritesetInstance(_engine, width, height, i, d, f);
        }

        static FontInstance LoadFont(string filename)
        {
            return new FontInstance(_engine, ParseSpherePath(filename, "fonts"));
        }

        static SurfaceInstance CreateSurface(int w, int h, ColorInstance color)
        {
            return new SurfaceInstance(_engine, w, h, color.GetColor());
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
            System.Text.StringBuilder builder = new System.Text.StringBuilder(data.Length);
            for (var i = 0; i < data.Length; ++i)
                    builder.Append((char)data[i]);
            return builder.ToString();
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
            string[] files = Directory.GetDirectories(GlobalProps.EnginePath + "/games");
            ObjectInstance[] names = new ObjectInstance[files.Length]; 
            for (var i = 0; i < files.Length; ++i) {
                GameFile file = new GameFile();
                file.ReadFile(files[i] + "/game.sgm");
                string name;
                names[i] = CreateObject();
                names[i]["name"] = (file.TryGetData("name", out name)) ? name : "";
                names[i]["description"] = (file.TryGetData("description", out name)) ? name : "";
                names[i]["author"] = (file.TryGetData("author", out name)) ? name : "";
                names[i]["directory"] = Path.GetFileName(files[i]);
            }
            return _engine.Array.New(names);
        }

        static string HashFromFile(string filename)
        {
            if (!File.Exists(filename))
                return "";

            using (MD5 mdHash = MD5.Create())
            {
                return GetMd5Hash(mdHash, File.ReadAllBytes(filename));
            }
        }

        static string GetMd5Hash(MD5 md5Hash, byte[] content)
        {
            byte[] data = md5Hash.ComputeHash(content);
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            for (int i = 0; i < data.Length; i++)
                builder.Append(data[i].ToString("x2"));

            // Return the hexadecimal string. 
            return builder.ToString();
        }

        static string HashByteArray(ByteArrayInstance array)
        {
            using (MD5 mdHash = MD5.Create())
            {
                return GetMd5Hash(mdHash, array.GetBytes());
            }
        }

        static bool Rename(string file1, string file2)
        {
            if (!File.Exists(file1) || File.Exists(file2))
                return false;
            File.Move(file1, file2);
            return true;
        }
    }
}
