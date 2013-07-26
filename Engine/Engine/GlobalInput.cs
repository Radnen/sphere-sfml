using System;
using System.Collections.Generic;
using SFML.Window;

namespace Engine
{
    public static class GlobalInput
    {
        private static Queue<int> _keyQueue = new Queue<int>(10);
        private static Queue<int> _wheelQueue = new Queue<int>(10);
        private static bool _fullscreen, _anyKey;
        private static Dictionary<int, Tuple<string, string>> _boundKeys = new Dictionary<int, Tuple<string, string>>();

        public static int TalkKey { get; private set; }
        public static int TalkButton { get; private set; }

        static GlobalInput()
        {
            TalkKey = (int)Keyboard.Key.Space;
        }

        public static void window_KeyPressed(object sender, KeyEventArgs e) {
            _anyKey = true;
            _keyQueue.Enqueue((int)e.Code);

            if (e.Code == Keyboard.Key.F10)
                ToggleFullScreen();
        }

        public static void window_KeyReleased(object sender, KeyEventArgs e) {
            _anyKey = false;
        }

        public static void window_Closed(object sender, EventArgs e)
        {
            Program.Exit();
        }

        public static void AddWindowHandlers(Window wind)
        {
            wind.Closed += window_Closed;
            wind.KeyPressed += window_KeyPressed;
            wind.KeyReleased += window_KeyReleased;
            wind.MouseWheelMoved += window_MouseWheel;
        }

        static void window_MouseWheel (object sender, MouseWheelEventArgs e)
        {
            _wheelQueue.Enqueue(e.Delta);
        }

        public static void RemoveWindowHandlers(Window wind)
        {
            wind.Closed -= window_Closed;
            wind.KeyPressed -= window_KeyPressed;
            wind.KeyReleased -= window_KeyReleased;
            wind.MouseWheelMoved -= window_MouseWheel;
        }

        public static void ToggleFullScreen()
        {
            var wind = Program._window;
            if (wind != null) {
                RemoveWindowHandlers(wind);
                wind.Close();
            }

            _fullscreen = !_fullscreen;
            var style = (_fullscreen) ? Styles.Fullscreen : Styles.Default;

            Program.InitWindow(style);
        }

        public static bool IsKeyPressed(int code)
        {
            return Keyboard.IsKeyPressed((Keyboard.Key)code);
        }

        public static bool IsAnyKeyPressed(int code)
        {
            return _anyKey;
        }

        public static bool IsMouseButtonPressed(int code)
        {
            return Mouse.IsButtonPressed((Mouse.Button)code);
        }

        public static int GetMouseWheelEvent()
        {
            return _wheelQueue.Dequeue();
        }

        public static int GetNumMouseWheelEvents()
        {
            return _wheelQueue.Count;
        }

        public static bool IsJoystickButtonPressed(int id, int code)
        {
            return Joystick.IsButtonPressed((uint)id, (uint)code);
        }

        public static double GetJoystickAxis(int id, int axis_id)
        {
            return Joystick.GetAxisPosition((uint)id, (Joystick.Axis)axis_id);
        }

        public static int GetNumJoystickAxes(int id)
        {
            int count = 0;
            Array a = Enum.GetValues(typeof(Joystick.Axis));
            foreach (object axis in a)
            {
                if (Joystick.HasAxis((uint)id, (Joystick.Axis)axis))
                    count++;
            }
            return count;
        }

        public static bool AreKeysLeft()
        {
            return _keyQueue.Count > 0;
        }

        public static int GetKey()
        {
            while (_keyQueue.Count == 0)
            {
                Program._window.DispatchEvents();
            }
            return _keyQueue.Dequeue();
        }

        public static int GetMouseX()
        {
            return Program._window.InternalGetMousePosition().X;
        }

        public static int GetMouseY()
        {
            return Program._window.InternalGetMousePosition().Y;
        }

        public static void SetMouseX(int new_x)
        {
            int y = GetMouseY();
            Program._window.InternalSetMousePosition(new Vector2i(new_x, y));
        }

        public static void SetMouseY(int new_y)
        {
            int x = GetMouseX();
            Program._window.InternalSetMousePosition(new Vector2i(x, new_y));
        }

        public static void SetTalkActivationKey(int key)
        {
            TalkKey = key;
        }

        public static void SetTalkActivationButton(int btn)
        {
            TalkButton = btn;
        }

        public static int GetTalkActivationKey()
        {
            return TalkKey;
        }

        public static int GetTalkActivationButton()
        {
            return TalkButton;
        }

        public static void SetMousePosition(int x, int y)
        {
            Program._window.InternalSetMousePosition(new Vector2i(x, y));
        }

        public static void BindKey(int code, string js_down, string js_up)
        {
            _boundKeys[code] = new Tuple<string, string>(js_down, js_up);
        }

        public static void UnbindKey(int code)
        {
            _boundKeys.Remove(code);
        }

        public static int GetNumJoySticks()
        {
            return (int)Joystick.Count;
        }

        public static int GetNumJoyStickButtons(int id)
        {
            return (int)Joystick.ButtonCount;
        }

        public static string GetKeyString(int key, bool shift)
        {
            if (key >= (int)Keyboard.Key.A && key <= (int)Keyboard.Key.Z)
            {
                return ((char)(key + (shift ? 65 : 97))).ToString();
            }
            else if (key >= (int)Keyboard.Key.Num0 && key <= (int)Keyboard.Key.Num9)
            {
                if (!shift)
                    return ((char)(key + 22)).ToString();
                else
                {
                    switch ((Keyboard.Key)key)
                    {
                        case Keyboard.Key.Num0:
                            return ")";
                        case Keyboard.Key.Num1:
                            return "!";
                        case Keyboard.Key.Num2:
                            return "@";
                        case Keyboard.Key.Num3:
                            return "#";
                        case Keyboard.Key.Num4:
                            return "$";
                        case Keyboard.Key.Num5:
                            return "%";
                        case Keyboard.Key.Num6:
                            return "^";
                        case Keyboard.Key.Num7:
                            return "&";
                        case Keyboard.Key.Num8:
                            return "*";
                        case Keyboard.Key.Num9:
                            return "(";
                    }
                }
            }
            else
            {
                switch ((Keyboard.Key)key)
                {
                    case Keyboard.Key.Add:
                        return "+";
                    case Keyboard.Key.BackSlash:
                        return (shift) ? "|" : "\\";
                    case Keyboard.Key.Comma:
                        return (shift) ? "<" : ",";
                    case Keyboard.Key.Dash:
                        return (shift) ? "_" : "-";
                    case Keyboard.Key.Divide:
                        return "/";
                    case Keyboard.Key.Equal:
                        return (shift) ? "+" : "=";
                    case Keyboard.Key.LBracket:
                        return (shift) ? "{" : "[";
                    case Keyboard.Key.Multiply:
                        return "*";
                    case Keyboard.Key.Period:
                        return (shift) ? ">" : ".";
                    case Keyboard.Key.Quote:
                        return (shift) ? "\"" : "\'";
                    case Keyboard.Key.RBracket:
                        return (shift) ? "}" : "]";
                    case Keyboard.Key.SemiColon:
                        return (shift) ? ":" : ";";
                    case Keyboard.Key.Subtract:
                        return "-";
                    case Keyboard.Key.Tilde:
                        return (shift) ? "~" : "`";
                }
            }
            return "";
        }
    }
}

