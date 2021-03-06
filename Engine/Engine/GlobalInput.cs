using System;
using System.Collections.Generic;
using Engine.Objects;
using SFML.System;
using SFML.Window;
using Jurassic;
using Jurassic.Core;

namespace Engine
{
    public static class GlobalInput
    {
        private static Queue<int> _keyQueue = new Queue<int>(10);
        private static Queue<int> _wheelQueue = new Queue<int>(10);
        private static bool _fullscreen, _anyKey;
        private static Dictionary<int, Tuple<CompiledMethod, CompiledMethod>> _boundKeys = new Dictionary<int, Tuple<CompiledMethod, CompiledMethod>>();

        public static int TalkKey { get; private set; }
        public static int TalkButton { get; private set; }

        static GlobalInput()
        {
            TalkKey = (int)Keyboard.Key.Space;
        }

        public static void window_KeyPressed(object sender, KeyEventArgs e) {
            _anyKey = true;

            int code = (int)e.Code;

            _keyQueue.Enqueue(code);

            if (_boundKeys.ContainsKey(code) && _boundKeys[code].Item1 != null)
                    _boundKeys[code].Item1.Execute();

            if (e.Code == Keyboard.Key.F10)
                Program.SetFullScreen(!Program.FullScreen);
            if (e.Code == Keyboard.Key.F1)
                Engine.Objects.MapEngineHandler.ToggleFPSThrottle();
            if (e.Code == Keyboard.Key.F2)
                Program.SetScaled(!Program.Scaled);
        }

        public static void window_KeyReleased(object sender, KeyEventArgs e) {
            _anyKey = false;
            int code = (int)e.Code;
            if (_boundKeys.ContainsKey(code) && _boundKeys[code].Item2 != null)
                _boundKeys[code].Item2.Execute();
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

        public static bool IsKeyPressed(int code)
        {
            if (code == (int)Keyboard.Key.RControl)
                return Keyboard.IsKeyPressed(Keyboard.Key.RControl) ||
                    Keyboard.IsKeyPressed(Keyboard.Key.LControl);
            if (code == (int)Keyboard.Key.RShift)
                return Keyboard.IsKeyPressed(Keyboard.Key.RShift) ||
                    Keyboard.IsKeyPressed(Keyboard.Key.LShift);
            if (code == (int)Keyboard.Key.RAlt)
                return Keyboard.IsKeyPressed(Keyboard.Key.RAlt) ||
                    Keyboard.IsKeyPressed(Keyboard.Key.LAlt);
            return Keyboard.IsKeyPressed((Keyboard.Key)code);
        }

        public static bool IsAnyKeyPressed()
        {
            return _anyKey;
        }

        public static bool IsMouseButtonPressed(int code)
        {
            return Mouse.IsButtonPressed((Mouse.Button)code);
        }

        public static int GetMouseWheelEvent()
        {
            while (_wheelQueue.Count == 0)
            {
                Program._window.DispatchEvents();
            }
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
            return Mouse.GetPosition(Program._window).Y;
        }

        public static int GetMouseY()
        {
            return Mouse.GetPosition(Program._window).Y;
        }

        public static void SetMouseX(int new_x)
        {
            int y = GetMouseY();
            Mouse.SetPosition(new Vector2i(new_x, y), Program._window);
        }

        public static void SetMouseY(int new_y)
        {
            int x = GetMouseX();
            Mouse.SetPosition(new Vector2i(x, new_y), Program._window);
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
            Mouse.SetPosition(new Vector2i(x, y), Program._window);
        }

        public static void BindKey(int code, string js_down, string js_up)
        {
            CompiledMethod down = new CompiledMethod(Program._engine, js_down);
            CompiledMethod up = new CompiledMethod(Program._engine, js_up);
            _boundKeys[code] = new Tuple<CompiledMethod, CompiledMethod>(down, up);
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

