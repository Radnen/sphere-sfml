using System;
using System.IO;
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
        public static RenderStates _states;
        public static ScriptEngine _engine = null;

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
                string code = ReadCodeFile(filename);
                RunCode(code);
                RunCode("game();");
            }
            else
            {
                Console.WriteLine("Invalid script file in game.sgm");
            }
        }

        static bool InitEngine()
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

            GlobalProps.BasePath = Path.GetDirectoryName(_game.FileName);

            ContextSettings settings = new ContextSettings(32, 0, 0);
            _window = new RenderWindow(new VideoMode((uint)320, (uint)240, 32), "Game", Styles.Titlebar | Styles.Close, settings);
            //_window.SetVerticalSyncEnabled(true);

            _window.Closed += window_Closed;
            GlobalPrimitives.window = _window;

            _engine = GetSphereEngine();

            return true;
        }

        public static ScriptEngine GetSphereEngine()
        {
            ScriptEngine engine = new ScriptEngine();

            // The glorious Sphere game API :)
            engine.SetGlobalFunction("FlipScreen", new Action(FlipScreen));
            engine.SetGlobalFunction("GetScreenWidth", new Func<int>(GetScreenWidth));
            engine.SetGlobalFunction("GetScreenHeight", new Func<int>(GetScreenHeight));
            engine.SetGlobalFunction("RequireScript", new Action<string>(RequireScript));
            engine.SetGlobalFunction("Print", new Action<string>(Print));
            engine.SetGlobalFunction("Exit", new Action(Exit));
            engine.SetGlobalFunction("CreateColor", new Func<int, int, int, int, ColorInstance>(CreateColor));
            engine.SetGlobalFunction("LoadImage", new Func<string, ImageInstance>(LoadImage));
            engine.SetGlobalFunction("Rectangle", new Action<double, double, double, double, ColorInstance>(GlobalPrimitives.Rectangle));
            engine.SetGlobalFunction("Triangle", new Action<double, double, double, double, double, double, ColorInstance>(GlobalPrimitives.Triangle));
            engine.SetGlobalFunction("OutlinedRectangle", new Action<double, double, double, double, ColorInstance, double>(GlobalPrimitives.OutlinedRectangle));
            engine.SetGlobalFunction("GradientRectangle", new Action<double, double, double, double, ColorInstance, ColorInstance, ColorInstance, ColorInstance>(GlobalPrimitives.GradientRectangle));
            engine.SetGlobalFunction("FilledCircle", new Action<double, double, double, ColorInstance>(GlobalPrimitives.FilledCircle));
            engine.SetGlobalFunction("Line", new Action<double, double, double, double, ColorInstance>(GlobalPrimitives.Line));
            engine.SetGlobalFunction("LineSeries", new Action<ArrayInstance, ColorInstance>(GlobalPrimitives.LineSeries));
            engine.SetGlobalFunction("Point", new Action<double, double, ColorInstance>(GlobalPrimitives.Point));
            engine.SetGlobalFunction("PointSeries", new Action<ArrayInstance, ColorInstance>(GlobalPrimitives.PointSeries));
            engine.SetGlobalFunction("GradientLine", new Action<double, double, double, double, ColorInstance, ColorInstance>(GlobalPrimitives.GradientLine));
            engine.SetGlobalFunction("CreateSurface", new Func<int, int, ColorInstance, SurfaceInstance>(CreateSurface)); 

            return engine;
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

        public static void SetupTest()
        {
            if (_window != null)
                return;

            _window = new RenderWindow(new VideoMode(320, 240), "Test Window", Styles.Default);
            _engine = GetSphereEngine();
        }

        static string ReadCodeFile(string filename)
		{
			StreamReader reader = null;
			string code = "";

			try
			{
				reader = new StreamReader(GlobalProps.BasePath + "/scripts/" + filename);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: could not load file " + filename + " - (" + e.Message + ")");
			}
			finally
			{
				if (reader != null)
				{
					code = reader.ReadToEnd();
					Console.WriteLine("Success: read in " + filename);
				}
			}

			return code;
		}

		static void Exit()
		{
			_window.Close();

            // Sadly, it's the only way to kill a while(true){}; in the JS environment.
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            proc.Kill();
		}

        static void Print(object obj)
        {
            Console.WriteLine(obj);
        }

		static ColorInstance CreateColor(int r, int g, int b, int a = 255)
		{
            return new ColorInstance(_engine.Object.InstancePrototype, r, g, b, a);
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
            return (int)_window.Size.X;
        }

        static int GetScreenHeight()
        {
            return (int)_window.Size.Y;
        }

        static ImageInstance LoadImage(string filename)
        {
            return new ImageInstance(_engine.Object.InstancePrototype, filename, _window);
        }

        static SurfaceInstance CreateSurface(int w, int h, ColorInstance color)
        {
            return new SurfaceInstance(_engine.Object.InstancePrototype, w, h, color.GetColor(), _window);
        }

        static void RunCode(string code)
		{
			try
			{
                _engine.Execute(code);
			}
			catch (JavaScriptException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

        static void RequireScript(string filename)
        {
            string code = ReadCodeFile(filename);
            RunCode(code);
        }

        static void window_Closed(object sender, EventArgs e)
		{
			Window window = sender as Window;
            if (window != null)
                Exit();
		}
    }
}
