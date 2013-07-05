using System;
using System.IO;
using SFML;
using SFML.Graphics;
using SFML.Window;
using Noesis.Javascript;

namespace Engine
{
    static class Program
    {
        static RenderWindow _window = null;
        static JavascriptContext _ctxt = null;

        static void Main()
        {
            _window = new RenderWindow(new VideoMode(640, 480, 32), "Game", Styles.Titlebar | Styles.Close, new ContextSettings(32, 0));
			_window.SetVerticalSyncEnabled(true);

            _ctxt = new JavascriptContext();
			_ctxt.SetParameter("FlipScreen", new Action(FlipScreen));
			_ctxt.SetParameter("RequireScript", new Action<string>(RequireScript));
			_ctxt.SetParameter("Print", new Action<string>(Print));
			_ctxt.SetParameter("Exit", new Action(Exit));
			_ctxt.SetParameter("LoadImage", new Func<string, object>(LoadImage));

            string code = ReadCodeFile("test.js"); // TODO: read this in from a .sgm
            code += "game();";

            _window.Closed += window_Closed;

            if (_window.IsOpen()) { RunCode(code); }
        }

        // TODO: make this not assume 'Startup' dir.
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
		}

        static void Print(object obj)
        {
            Console.WriteLine(obj);
        }

        static void FlipScreen()
        {
            _window.Display();
            _window.DispatchEvents();
        }

        static object LoadImage(string filename)
        {
			return new Engine.Objects.ImageWrapper (filename, _window);
        }

        static void RunCode(string code)
		{
			try
			{
				_ctxt.Run(code);
			}
			catch (JavascriptException ex)
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
			{
				window.Close();
			}
		}
    }
}
