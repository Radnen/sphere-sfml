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
        static RenderWindow _window = null;
        static ScriptEngine _engine = null;

        static void Main()
        {
            Console.WriteLine("Working Directory:" + System.IO.Directory.GetCurrentDirectory());

            _window = new RenderWindow(new VideoMode(320, 240, 32), "Game", Styles.Titlebar | Styles.Close, new ContextSettings(32, 0));
            //_window.SetVerticalSyncEnabled(true);
            _window.Closed += window_Closed;
            GlobalPrimitives.window = _window;

            _engine = new ScriptEngine();

            _engine.SetGlobalFunction("FlipScreen", new Action(FlipScreen));
            _engine.SetGlobalFunction("GetScreenWidth", new Func<int>(GetScreenWidth));
            _engine.SetGlobalFunction("GetScreenHeight", new Func<int>(GetScreenHeight));
            _engine.SetGlobalFunction("RequireScript", new Action<string>(RequireScript));
            _engine.SetGlobalFunction("Print", new Action<string>(Print));
            _engine.SetGlobalFunction("Exit", new Action(Exit));
            _engine.SetGlobalFunction("CreateColor", new Func<int, int, int, int, ColorInstance>(CreateColor));
            _engine.SetGlobalFunction("LoadImage", new Func<string, ImageInstance>(LoadImage));
            _engine.SetGlobalFunction("Rectangle", new Action<double, double, double, double, ColorInstance>(Rectangle));
            _engine.SetGlobalFunction("Triangle", new Action<double, double, double, double, double, double, ColorInstance>(Triangle));
            _engine.SetGlobalFunction("OutlinedRectangle", new Action<double, double, double, double, ColorInstance, double>(OutlinedRectangle));
            _engine.SetGlobalFunction("GradientRectangle", new Action<double, double, double, double, ColorInstance, ColorInstance, ColorInstance, ColorInstance>(GradientRectangle));
            _engine.SetGlobalFunction("FilledCircle", new Action<double, double, double, ColorInstance>(FilledCircle));
            _engine.SetGlobalFunction("Line", new Action<double, double, double, double, ColorInstance>(Line));
            _engine.SetGlobalFunction("GradientLine", new Action<double, double, double, double, ColorInstance, ColorInstance>(GradientLine));

            string code = ReadCodeFile("test.js"); // TODO: read this in from a .sgm

            RunCode(code);

            RunCode("game();");
        }

        // TODO: engine should change GlobalProps.BasePath to loaded gamepath.
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

        static void Rectangle(double x, double y, double w, double h, ColorInstance color)
        {
            GlobalPrimitives.Rectangle((float)x, (float)y, (float)w, (float)h, color.GetColor());
        }

        static void Line(double x1, double y1, double x2, double y2, ColorInstance color)
        {
            GlobalPrimitives.Line((float)x1, (float)y1, (float)x2, (float)y2, color.GetColor());
        }

        static void GradientLine(double x1, double y1, double x2, double y2, ColorInstance color1, ColorInstance color2)
        {
            GlobalPrimitives.GradientLine((float)x1, (float)y1, (float)x2, (float)y2, color1.GetColor(), color2.GetColor());
        }

        static void Triangle(double x1, double y1, double x2, double y2, double x3, double y3, ColorInstance color)
        {
            GlobalPrimitives.Triangle((float)x1, (float)y1, (float)x2, (float)y2, (float)x3, (float)y3, color.GetColor());
        }

        static void OutlinedRectangle(double x, double y, double w, double h, ColorInstance color, double thickness = 1.0f)
        {
            GlobalPrimitives.OutlinedRectangle((float)x, (float)y, (float)w, (float)h, color.GetColor(), (float)thickness);
        }

        static void GradientRectangle(double x, double y, double width, double height, ColorInstance color1, ColorInstance color2, ColorInstance color3, ColorInstance color4)
        {
            GlobalPrimitives.GradientRectangle((float)x, (float)y, (float)width, (float)height, color1.GetColor(),
                                               color2.GetColor(), color3.GetColor(), color4.GetColor());
        }

        static void FilledCircle(double x, double y, double radius, ColorInstance color)
        {
            GlobalPrimitives.FilledCircle((float)x, (float)y, (float)radius, color.GetColor());
        }

        static ImageInstance LoadImage(string filename)
        {
            return new ImageInstance(_engine.Object.InstancePrototype, filename, _window);
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
