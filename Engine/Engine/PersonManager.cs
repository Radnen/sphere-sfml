using System;
using System.Collections.Generic;
using Engine.Objects;
using Jurassic;
using Jurassic.Library;
using SFML.System;

namespace Engine
{
    public static class PersonManager
    {
        public static Dictionary<string, Person> PeopleTable { get; private set; }
        private static List<string> _personlist;
        public static List<Person> People { get; private set; }
        public static string CurrentPerson = "";
        public static string ObstPerson = "";

        private static int _talk_dist = 8;

        static PersonManager()
        {
            People = new List<Person>();
            _personlist = new List<string>();
            PeopleTable = new Dictionary<string, Person>();
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
            engine.SetGlobalFunction("GetPersonMask", new Func<string, object>(GetPersonMask));
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
            engine.SetGlobalFunction("GetPersonBase", new Func<string, object>(GetPersonBase));
            engine.SetGlobalFunction("GetCurrentPerson", new Func<string>(GetCurrentPerson));
            engine.SetGlobalFunction("GetObstructingPerson", new Func<string>(GetObstructingPerson));
            engine.SetGlobalFunction("ClearPersonCommands", new Action<string>(ClearPersonCommands));
            engine.SetGlobalFunction("GetPersonList", new Func<ArrayInstance>(GetPersonList));
            engine.SetGlobalFunction("GetPersonFrameRevert", new Func<string, int>(GetPersonFrameRevert));
            engine.SetGlobalFunction("SetPersonFrameRevert", new Action<string, int>(SetPersonFrameRevert));
            engine.SetGlobalFunction("GetPersonSpriteset", new Func<string, object>(GetPersonSpriteset));
            engine.SetGlobalFunction("SetPersonSpriteset", new Action<string, SpritesetInstance>(SetPersonSpriteset));
            engine.SetGlobalFunction("GetPersonData", new Func<string, object>(GetPersonData));
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
            engine.SetGlobalFunction("SetTalkDistance", new Action<int>(SetTalkDistance));
            engine.SetGlobalFunction("GetTalkDistance", new Func<int>(GetTalkDistance));
        }

        private static string GetObstructingPerson()
        {
            return ObstPerson;
        }

        private static void SetTalkDistance(int dist)
        {
            _talk_dist = dist;
        }

        private static int GetTalkDistance()
        {
            return _talk_dist;
        }

        public static void CreatePerson(string name, string ss, [DefaultParameterValue(true)] bool destroy = true)
        {
            if (PeopleTable.ContainsKey(name))
                return;

            SpritesetInstance sprite = AssetManager.GetSpriteset(ss);
            Person p = new Person(name, sprite, destroy);
            PeopleTable.Add(name, p);
            _personlist.Add(name);
			People.Add(p);

			Map start = MapEngineHandler.Map;
			if (start != null)
			{
				p.Layer = start.StartLayer;
				SetPersonXY(p.Name, start.StartX, start.StartY);
			}
        }

        public static void CreatePerson(Entity person) {
            if (PeopleTable.ContainsKey(person.Name))
                return;

            SpritesetInstance sprite = AssetManager.GetSpriteset(person.Spriteset);
            Person p = new Person(person.Name, sprite, true);
            p.Layer = person.Layer;
            int w = (int)p.Base["x2"] - (int)p.Base["x1"];
            int h = (int)p.Base["y2"] - (int)p.Base["y1"];
            p.Position = new Vector2f(person.X - w/2, person.Y - h/2);
            for (var i = 0; i < person.Scripts.Count; ++i)
                p.SetScript((PersonScripts)i, person.Scripts[i]);

            PeopleTable.Add(person.Name, p);
            _personlist.Add(person.Name);
			People.Add(p);
        }

        public static void DestroyPerson(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                People.Remove(value);
                PeopleTable.Remove(name);
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
        /// Gets the closest person within talking range.
        /// </summary>
        /// <returns>The closest person.</returns>
        /// <param name="name">Compare all others to this one.</param>
        public static string GetClosest(string name)
        {
            int talk_x = GetPersonX(name), talk_y = GetPersonY(name);
            string direction = GetPersonDirection(name);

            if (direction.Contains("north"))
                talk_y -= _talk_dist;
            if (direction.Contains("south"))
                talk_y += _talk_dist;
            if (direction.Contains("east"))
                talk_x += _talk_dist;
            if (direction.Contains("west"))
                talk_x -= _talk_dist;

            foreach (Person p in People)
            {
                if (name == p.Name)
                    continue;

                float dx = p.Position.X - talk_x;
                float dy = p.Position.Y - talk_y;

                if ((int)Math.Sqrt(dx * dx + dy * dy) <= _talk_dist)
                    return p.Name;
            }
            return null;
        }

        /// <summary>
        /// Removes the non-essential person objects on a map.
        /// </summary>
        public static void RemoveNonEssential()
        {
            for (var i = 0; i < _personlist.Count; ++i)
            {
                if (PeopleTable[_personlist[i]].DestroyOnMap)
                {
                    DestroyPerson(_personlist[i]);
                    i--;
                }
            }
        }

        public static bool CheckPersonObstructions(ref Vector2f position, Person person)
        {
			foreach (Person p in People)
            {
                if (p.Layer == person.Layer && p.Name != person.Name &&
                    person.CheckObstructions(ref position, p))
                {
                    ObstPerson = p.Name;
                    return true;
                }
            }
            return false;
        }

        public static void SetPersonX(string name, int x)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                x -= value.BaseWidth / 2;
                value.Position = new Vector2f(x, value.Position.Y);
            }
        }

        public static int GetPersonX(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return (int)(value.Position.X + value.BaseWidth / 2);
            }
            else return 0;
        }

        public static void SetPersonY(string name, int y)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Position = new Vector2f(value.Position.X, y - value.BaseHeight / 2);
            }
        }

        public static int GetPersonY(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return (int)(value.Position.Y + value.BaseHeight / 2);
            }
            else return 0;
        }

        public static void SetPersonXFloat(string name, double x)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                x -= value.BaseWidth / 2;
                value.Position = new Vector2f((float)x, value.Position.Y);
            }
        }

        public static double GetPersonXFloat(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Position.X + value.BaseWidth / 2;
            }
            else return 0;
        }

        public static void SetPersonYFloat(string name, double y)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                y -= value.BaseHeight / 2;
                value.Position = new Vector2f(value.Position.Y, (float)y);
            }
        }

        public static double GetPersonYFloat(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Position.Y + value.BaseHeight / 2;
            }
            else return 0;
        }

        public static void SetPersonXY(string name, int x, int y)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                x -= value.BaseWidth / 2;
                y -= value.BaseHeight / 2;
                value.Position = new Vector2f(x, y);
            }
        }

        public static void SetPersonXYFloat(string name, double x, double y)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                x -= value.BaseWidth / 2;
                y -= value.BaseHeight / 2;
                value.Position = new Vector2f((float)x, (float)y);
            }
        }

        public static void SetPersonVisible(string name, bool visible)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Visible = visible;
            }
        }

        public static bool IsPersonVisible(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Visible;
            }
            else return false;
        }

        public static void QueuePersonCommand(string name, int command, bool immediate)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.QueueCommand(command, immediate);
            }
        }

        public static void QueuePersonScript(string name, object script, bool immediate)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.QueueScript(script, immediate);
            }
        }

        public static bool IsCommandQueueEmpty(string name)
        {
            return PeopleTable[name].IsQueueEmpty();
        }

        public static ArrayInstance GetPersonList()
        {
            return Program._engine.Array.New(_personlist.ToArray());
        }

        public static bool DoesPersonExist(string name)
        {
            return PeopleTable.ContainsKey(name);
        }

        public static void SetPersonMask(string name, ColorInstance color)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Mask = color.GetColor();
            }
        }

        public static object GetPersonMask(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return new ColorInstance(Program._engine, value.Mask);
            }
            else return Undefined.Value;
        }

        public static void SetPersonSpeed(string name, double s)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Speed = new Vector2f((float)s, (float)s);
            }
        }

        public static void SetPersonSpeedXY(string name, double x, double y)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Speed = new Vector2f((float)x, (float)y);
            }
        }

        public static void SetPersonSpriteset(string name, SpritesetInstance instance)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Spriteset = instance;
            }
        }

        public static object GetPersonSpriteset(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Spriteset;
            }
            return Undefined.Value;
        }

        public static double GetPersonSpeedX(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Speed.X;
            }
            else return 0;
        }

        public static double GetPersonSpeedY(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Speed.Y;
            }
            else return 0;
        }

        public static void SetPersonFrame(string name, int v)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Frame = v;
            }
        }

        public static int GetPersonFrame(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Frame;
            }
            else return 0;
        }

        public static string GetPersonDirection(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Direction;
            }
            else return "";
        }

        public static void SetPersonDirection(string name, string d)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Direction = d;
            }
        }

        public static void SetPersonLayer(string name, int layer)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Layer = layer;
            }
        }

        public static int GetPersonLayer(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Layer;
            }
            else return 0;
        }

        public static object GetPersonBase(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Base;
            }
            else return Undefined.Value;
        }

        public static void SetPersonOffsetX(string name, double x)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Offset = new Vector2f((float)x, value.Offset.Y);
            }
        }

        public static void SetPersonOffsetY(string name, double y)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Offset = new Vector2f(value.Offset.X, (float)y);
            }
        }

        public static double GetPersonOffsetX(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Offset.X;
            }
            else return 0;
        }

        public static double GetPersonOffsetY(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Offset.Y;
            }
            else return 0;
        }

        public static string GetCurrentPerson()
        {
            return CurrentPerson;
        }

        public static void ClearPersonCommands(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.ClearComands();
            }
        }

        public static int GetPersonFrameRevert(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.FrameRevert;
            }
            else return 0;
        }

        public static void SetPersonFrameRevert(string name, int r)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.FrameRevert = r;
            }
        }

        public static object GetPersonData(string name)
        {
            // TODO: implement & update data props:
            // eithre here, or when you set the SS.
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.Data;
            }
            else return Undefined.Value;
        }

        public static void SetPersonData(string name, ObjectInstance o)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Data = o;
            }
        }

        public static void SetPersonValue(string name, string key, object o)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.Data[key] = o;
            }
        }

        public static object GetPersonValue(string name, string key)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return PeopleTable[name].Data[key];
            }
            else return Undefined.Value;
        }

        public static void CallPersonScript(string name, int type)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                CurrentPerson = name;
                value.CallScript((PersonScripts)type);
            }
        }

        public static void SetPersonScript(string name, int type, object script)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.SetScript((PersonScripts)type, script);
            }
        }

        public static void IgnorePersonObstructions(string name, bool ignore)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                value.IgnorePersons = ignore;
            }
        }

        public static bool IsIgnoringPersonObstructions(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return value.IgnorePersons;
            }
            else return false;
        }

        public static void IgnoreTileObstructions(string name, bool ignore)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                PeopleTable[name].IgnoreTiles = ignore;
            }
        }

        public static bool IsIgnoringTileObstructions(string name)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                return PeopleTable[name].IgnoreTiles;
            }
            else return false;
        }

        public static bool IsPersonObstructed(string name, double x, double y)
        {
            Person value;
            if (PeopleTable.TryGetValue(name, out value))
            {
                x -= value.BaseWidth / 2;
                y -= value.BaseHeight / 2;
                return value.IsObstructedAt(new Vector2f((float)x, (float)y));
            }
            else return false;
        }
    }
}

