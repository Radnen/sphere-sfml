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
        private static Dictionary<string, Person> _people;
        private static List<string> _personlist;
        public static List<Person> People { get; private set; }

        public static string _current = "";

        static PersonManager()
        {
            People = new List<Person>();
            _personlist = new List<string>();
            _people = new Dictionary<string, Person>();
        }

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
            engine.SetGlobalFunction("IsPersonVisible", new Func<string, bool>(IsPersonVisible));
            engine.SetGlobalFunction("QueuePersonCommand", new Action<string, int, bool>(QueuePersonCommand));
            engine.SetGlobalFunction("QueuePersonScript", new Action<string, object, bool>(QueuePersonScript));
            engine.SetGlobalFunction("IsCommandQueueEmpty", new Func<string, bool>(IsCommandQueueEmpty));
            engine.SetGlobalFunction("GetPersonMask", new Func<string, ColorInstance>(GetPersonMask));
            engine.SetGlobalFunction("SetPersonMask", new Action<string, ColorInstance>(SetPersonMask));
            engine.SetGlobalFunction("SetPersonSpeedXY", new Action<string, double, double>(SetPersonSpeedXY));
            engine.SetGlobalFunction("SetPersonSpeed", new Action<string, double>(SetPersonSpeed));
            engine.SetGlobalFunction("GetPersonSpeedX", new Func<string, double>(GetPersonSpeedX));
            engine.SetGlobalFunction("GetPersonSpeedY", new Func<string, double>(GetPersonSpeedY));
            engine.SetGlobalFunction("SetPersonFrame", new Action<string, int>(SetPersonFrame));
            engine.SetGlobalFunction("GetPersonFrame", new Func<string, int>(GetPersonFrame));
            engine.SetGlobalFunction("SetPersonDirection", new Action<string, string>(SetPersonDirection));
            engine.SetGlobalFunction("GetPersonDirection", new Func<string, string>(GetPersonDirection));
            engine.SetGlobalFunction("SetPersonOffsetX", new Action<string, double>(SetPersonOffsetX));
            engine.SetGlobalFunction("SetPersonOffsetY", new Action<string, double>(SetPersonOffsetY));
            engine.SetGlobalFunction("GetPersonOffsetX", new Func<string, double>(GetPersonOffsetX));
            engine.SetGlobalFunction("GetPersonOffsetY", new Func<string, double>(GetPersonOffsetY));
            engine.SetGlobalFunction("SetPersonLayer", new Action<string, int>(SetPersonLayer));
            engine.SetGlobalFunction("GetPersonLayer", new Func<string, int>(GetPersonLayer));
            engine.SetGlobalFunction("GetPersonBase", new Func<string, ObjectInstance>(GetPersonBase));
            engine.SetGlobalFunction("GetCurrentPerson", new Func<string>(GetCurrentPerson));
            engine.SetGlobalFunction("ClearPersonCommands", new Action<string>(ClearPersonCommands));
            engine.SetGlobalFunction("GetPersonList", new Func<ArrayInstance>(GetPersonList));
            engine.SetGlobalFunction("GetPersonFrameRevert", new Func<string, int>(GetPersonFrameRevert));
            engine.SetGlobalFunction("SetPersonFrameRevert", new Action<string, int>(SetPersonFrameRevert));
            engine.SetGlobalFunction("GetPersonData", new Func<string, ObjectInstance>(GetPersonData));
            engine.SetGlobalFunction("SetPersonData", new Action<string, ObjectInstance>(SetPersonData));
            engine.SetGlobalFunction("SetPersonValue", new Action<string, string, object>(SetPersonValue));
            engine.SetGlobalFunction("GetPersonValue", new Func<string, string, object>(GetPersonValue));
            engine.SetGlobalFunction("SetPersonScript", new Action<string, int, object>(SetPersonScript));
            engine.SetGlobalFunction("CallPersonScript", new Action<string, int>(CallPersonScript));
            engine.SetGlobalFunction("DoesPersonExist", new Func<string, bool>(DoesPersonExist));
            engine.SetGlobalFunction("IsIgnoringPersonObstructions", new Func<string, bool>(IsIgnoringPersonObstructions));
            engine.SetGlobalFunction("IsIgnoringTileObstructions", new Func<string, bool>(IsIgnoringTileObstructions));
            engine.SetGlobalFunction("IgnorePersonObstructions", new Action<string, bool>(IgnorePersonObstructions));
            engine.SetGlobalFunction("IgnoreTileObstructions", new Action<string, bool>(IgnoreTileObstructions));
            engine.SetGlobalFunction("IsPersonObstructed", new Func<string, double, double, bool>(IsPersonObstructed));
        }

        public static void CreatePerson(string name, string ss, [DefaultParameterValue(true)] bool destroy = true)
        {
            if (_people.ContainsKey(name))
                return;
            SpritesetInstance sprite = AssetManager.GetSpriteset(ss);
            Person p = new Person(name, sprite, destroy);
            _people.Add(name, p);
            _personlist.Add(name);
            People.Add(p);
        }

        public static void CreatePerson(Entity person) {
            if (_people.ContainsKey(person.Name))
                return;

            //string ss = GlobalProps.BasePath + "/spritesets/" + person.Spriteset;
            //SpritesetInstance sprite = new SpritesetInstance(Program._engine, ss);
            SpritesetInstance sprite = AssetManager.GetSpriteset(person.Spriteset);
            Person p = new Person(person.Name, sprite, true);
            p.Layer = person.Layer;
            int w = (int)p.Base["x2"] - (int)p.Base["x1"];
            int h = (int)p.Base["y2"] - (int)p.Base["y1"];
            p.Position = new Vector2f(person.X - w/2, person.Y - h/2);
            for (var i = 0; i < person.Scripts.Count; ++i)
                p.SetScript((PersonScripts)i, person.Scripts[i]);

            _people.Add(person.Name, p);
            _personlist.Add(person.Name);
            People.Add(p);
        }

        public static void DestroyPerson(string name)
        {
            if (_people.ContainsKey(name))
            {
                People.Remove(_people[name]);
                _people.Remove(name);
                _personlist.Remove(name);
            }
        }

        /// <summary>
        /// Orders the people by the y axis, like in vanilla Sphere.
        /// </summary>
        public static void OrderPeople()
        {
            People.Sort((A, B) => (int)(A.Position.Y - B.Position.Y));
        }

        /// <summary>
        /// Removes the non-essential person objects on a map.
        /// </summary>
        public static void RemoveNonEssential()
        {
            for (var i = 0; i < _personlist.Count; ++i)
            {
                if (_people[_personlist[i]].DestroyOnMap)
                {
                    DestroyPerson(_personlist[i]);
                    i--;
                }
            }
        }

        public static bool CheckPersonObstructions(ref Vector2f position, Person person)
        {
            foreach (Person b in People)
            {
                if (b.Name == person.Name)
                    continue;
                if (person.CheckObstructions(ref position, b))
                    return true;
            }
            return false;
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

        public static void QueuePersonCommand(string name, int command, bool immediate)
        {
            _people[name].QueueCommand(command, immediate);
        }

        public static void QueuePersonScript(string name, object script, bool immediate)
        {
            _people[name].QueueScript(script, immediate);
        }

        public static bool IsCommandQueueEmpty(string name)
        {
            return _people[name].IsQueueEmpty();
        }

        public static ArrayInstance GetPersonList()
        {
            return Program._engine.Array.New(_personlist.ToArray());
        }

        public static bool DoesPersonExist(string name)
        {
            return _people.ContainsKey(name);
        }

        public static void SetPersonMask(string name, ColorInstance color)
        {
            _people[name].Mask = color.GetColor();
        }

        public static ColorInstance GetPersonMask(string name)
        {
            return new ColorInstance(Program._engine, _people[name].Mask);
        }

        public static void SetPersonSpeed(string name, double s)
        {
            _people[name].Speed = new Vector2f((float)s, (float)s);
        }

        public static void SetPersonSpeedXY(string name, double x, double y)
        {
            _people[name].Speed = new Vector2f((float)x, (float)y);
        }

        public static double GetPersonSpeedX(string name)
        {
            return _people[name].Speed.X;
        }

        public static double GetPersonSpeedY(string name)
        {
            return _people[name].Speed.Y;
        }

        public static void SetPersonFrame(string name, int v)
        {
            _people[name].Frame = v;
        }

        public static int GetPersonFrame(string name)
        {
            return _people[name].Frame;
        }

        public static string GetPersonDirection(string name)
        {
            return _people[name].Direction;
        }

        public static void SetPersonDirection(string name, string d)
        {
            _people[name].Direction = d;
        }

        public static void SetPersonLayer(string name, int layer)
        {
            _people[name].Layer = layer;
        }

        public static int GetPersonLayer(string name)
        {
            return _people[name].Layer;
        }

        public static ObjectInstance GetPersonBase(string name)
        {
            return _people[name].Base;
        }

        public static void SetPersonOffsetX(string name, double x)
        {
            _people[name].Offset = new Vector2f((float)x, _people[name].Offset.Y);
        }

        public static void SetPersonOffsetY(string name, double y)
        {
            _people[name].Offset = new Vector2f(_people[name].Offset.X, (float)y);
        }

        public static double GetPersonOffsetX(string name)
        {
            return _people[name].Offset.X;
        }

        public static double GetPersonOffsetY(string name)
        {
            return _people[name].Offset.Y;
        }

        public static string GetCurrentPerson()
        {
            return _current;
        }

        public static void ClearPersonCommands(string name)
        {
            _people[name].ClearComands();
        }

        public static int GetPersonFrameRevert(string name)
        {
            return _people[name].FrameRevert;
        }

        public static void SetPersonFrameRevert(string name, int r)
        {
            _people[name].FrameRevert = r;
        }

        public static ObjectInstance GetPersonData(string name)
        {
            // TODO: implement & update data props:
            // eithre here, or when you set the SS.
            return _people[name].Data;
        }

        public static void SetPersonData(string name, ObjectInstance o)
        {
            _people[name].Data = o;
        }

        public static void SetPersonValue(string name, string key, object o)
        {
            _people[name].Data[key] = o;
        }

        public static object GetPersonValue(string name, string key)
        {
            return _people[name].Data[key];
        }

        public static void CallPersonScript(string name, int type)
        {
            _current = name;
            _people[name].CallScript((PersonScripts)type);
        }

        public static void SetPersonScript(string name, int type, object script)
        {
            _people[name].SetScript((PersonScripts)type, script);
        }

        public static void IgnorePersonObstructions(string name, bool value)
        {
            _people[name].IgnorePersons = value;
        }

        public static bool IsIgnoringPersonObstructions(string name)
        {
            return _people[name].IgnorePersons;
        }

        public static void IgnoreTileObstructions(string name, bool value)
        {
            _people[name].IgnoreTiles = value;
        }

        public static bool IsIgnoringTileObstructions(string name)
        {
            return _people[name].IgnoreTiles;
        }

        public static bool IsPersonObstructed(string name, double x, double y)
        {
            Vector2f pos = new Vector2f((float)x, (float)y);
            return _people[name].IsObstructedAt(pos);
        }
    }
}

