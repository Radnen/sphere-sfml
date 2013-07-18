using System;
using System.Collections.Generic;
using Engine.Objects;
using Jurassic;
using Jurassic.Library;
using SFML.Window;

namespace Engine
{
    public static class PersonManager
    {
        private static SortedDictionary<string, Person> _people = new SortedDictionary<string, Person>();

        public static void BindToEngine(ScriptEngine engine)
        {
            engine.SetGlobalFunction("CreatePerson", new Action<string, string, bool>(CreatePerson));
            engine.SetGlobalFunction("DestroyPerson", new Action<string>(DestroyPerson));
            engine.SetGlobalFunction("SetPersonX", new Action<string, int>(SetPersonX));
            engine.SetGlobalFunction("SetPersonY", new Action<string, int>(SetPersonY));
            engine.SetGlobalFunction("GetPersonX", new Func<string, int>(GetPersonX));
            engine.SetGlobalFunction("GetPersonY", new Func<string, int>(GetPersonY));
            engine.SetGlobalFunction("SetPersonXFloat", new Action<string, double>(SetPersonXFloat));
            engine.SetGlobalFunction("SetPersonYFloat", new Action<string, double>(SetPersonYFloat));
            engine.SetGlobalFunction("GetPersonXFloat", new Func<string, double>(GetPersonXFloat));
            engine.SetGlobalFunction("GetPersonYFloat", new Func<string, double>(GetPersonXFloat));
            engine.SetGlobalFunction("SetPersonXY", new Action<string, int, int>(SetPersonXY));
            engine.SetGlobalFunction("SetPersonXYFloat", new Action<string, double, double>(SetPersonXYFloat));
            engine.SetGlobalFunction("SetPersonVisible", new Action<string, bool>(SetPersonVisible));
            engine.SetGlobalFunction("GetPersonVisible", new Func<string, bool>(IsPersonVisible));
        }

        public static void CreatePerson(string name, string ss, bool destroy = true)
        {
            ss = GlobalProps.BasePath + "/spritesets/" + ss;
            SpritesetInstance sprite = new SpritesetInstance(Program._engine, ss);
            _people.Add(name, new Person(name, sprite, destroy));
        }

        public static void DestroyPerson(string name)
        {
            _people.Remove(name);
        }

        public static void SetPersonX(string name, int x)
        {
            _people[name].Position = new Vector2f(x, _people[name].Position.Y);
        }

        public static int GetPersonX(string name)
        {
            return (int)_people[name].Position.X;
        }

        public static void SetPersonY(string name, int y)
        {
            _people[name].Position = new Vector2f(_people[name].Position.X, y);
        }

        public static int GetPersonY(string name)
        {
            return (int)_people[name].Position.Y;
        }

        public static void SetPersonXFloat(string name, double x)
        {
            _people[name].Position = new Vector2f((float)x, _people[name].Position.Y);
        }

        public static double GetPersonXFloat(string name)
        {
            return _people[name].Position.X;
        }

        public static void SetPersonYFloat(string name, double y)
        {
            _people[name].Position = new Vector2f(_people[name].Position.Y, (float)y);
        }

        public static double GetPersonYFloat(string name)
        {
            return _people[name].Position.Y;
        }

        public static void SetPersonXY(string name, int x, int y)
        {
            _people[name].Position = new Vector2f(x, y);
        }

        public static void SetPersonXYFloat(string name, double x, double y)
        {
            _people[name].Position = new Vector2f((float)x, (float)y);
        }

        public static void SetPersonVisible(string name, bool visible)
        {
            _people[name].Visible = visible;
        }

        public static bool IsPersonVisible(string name)
        {
            return _people[name].Visible;
        }
    }
}

