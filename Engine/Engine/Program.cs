using System;
using SFML;
using SFML.Window;

namespace Engine
{
    static class Program
    {
        static void Main()
        {
            Window window = new Window(new VideoMode(640, 480, 32), "Game", Styles.Titlebar | Styles.Close, new ContextSettings(32, 0));
            window.SetVerticalSyncEnabled(true);

            window.Closed += window_Closed;

            while (window.IsOpen())
            {
                window.DispatchEvents();
                window.Display();
            }
        }

        static void window_Closed(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window != null) window.Close();
        }
    }
}
