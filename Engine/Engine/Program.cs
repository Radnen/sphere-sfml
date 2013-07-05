using System;
using System.IO;
using SFML;
using SFML.Window;
using Noesis.Javascript;

namespace Engine
{
    static class Program
    {
        static Window window = null;
        static JavascriptContext ctxt = null;

        static void Main()
        {
            window = new Window(new VideoMode(640, 480, 32), "Game", Styles.Titlebar | Styles.Close, new ContextSettings(32, 0));
            window.SetVerticalSyncEnabled(true);

            ctxt = new JavascriptContext();
            ctxt.SetParameter("FlipScreen", new Action(FlipScreen));
            ctxt.SetParameter("RequireScript", new Action<string>(RequireScript));
            ctxt.SetParameter("Print", new Action<string>(Print));

            string code = ReadCodeFile("test.js"); // TODO: read this in from a .sgm
            code += "game();";

            window.Closed += window_Closed;

            if (window.IsOpen()) { RunCode(code); }
        }

        // TODO: make this not assume 'Startup' dir.
        static string ReadCodeFile(string filename)
        {
            StreamReader reader = null;
            string code = "";

            try
            {
                reader = new StreamReader("Startup/scripts/" + filename);
            }
            catch (Exception e) { Console.WriteLine("Error: could not load file " + filename + " - (" + e.Message + ")"); }
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

        static void Print(object obj)
        {
            Console.WriteLine(obj);
        }

        static void FlipScreen()
        {
            window.Display();
            window.DispatchEvents();
        }

        static void RunCode(string code)
        {
            try
            {
                ctxt.Run(code);
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
