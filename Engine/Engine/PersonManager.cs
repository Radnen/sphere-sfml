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
            engine.SetGlobalFunction("GetObstructingPerson", new Func<string>(GetObstructingPerson));
            engine.SetGlobalFunction("ClearPersonCommands", new Action<string>(ClearPersonCommands));
            engine.SetGlobalFunction("GetPersonList", new Func<ArrayInstance>(GetPersonList));
            engine.SetGlobalFunction("GetPersonFrameRevert", new Func<string, int>(GetPersonFrameRevert));
            engine.SetGlobalFunction("SetPersonFrameRevert", new Action<string, int>(SetPersonFrameRevert));
            engine.SetGlobalFunction("GetPersonSpriteset", new Func<string, SpritesetInstance>(GetPersonSpriteset));
            engine.SetGlobalFunction("SetPersonSpriteset", new Action<string, SpritesetInstance>(SetPersonSpriteset));
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
            if (PeopleTable.ContainsKey(name))
            {
                People.Remove(PeopleTable[name]);
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
			x -= PeopleTable[name].BaseWidth / 2;
			PeopleTable[name].Position = new Vector2f(x, PeopleTable[name].Position.Y);
        }

        public static int GetPersonX(string name)
        {
			return (int)PeopleTable[name].Position.X + PeopleTable[name].BaseWidth / 2;
        }

        public static void SetPersonY(string name, int y)
        {
			PeopleTable[name].Position = new Vector2f(PeopleTable[name].Position.X, y - PeopleTable[name].BaseHeight / 2);
        }

        public static int GetPersonY(string name)
        {
			return (int)PeopleTable[name].Position.Y + PeopleTable[name].BaseHeight / 2;
        }

        public static void SetPersonXFloat(string name, double x)
        {
			x -= PeopleTable[name].BaseWidth / 2;
            PeopleTable[name].Position = new Vector2f((float)x, PeopleTable[name].Position.Y);
        }

        public static double GetPersonXFloat(string name)
        {
			return PeopleTable[name].Position.X + PeopleTable[name].BaseWidth / 2;
        }

        public static void SetPersonYFloat(string name, double y)
        {
			y -= PeopleTable[name].BaseHeight / 2;
            PeopleTable[name].Position = new Vector2f(PeopleTable[name].Position.Y, (float)y);
        }

        public static double GetPersonYFloat(string name)
        {
			return PeopleTable[name].Position.Y + PeopleTable[name].BaseHeight / 2;
        }

        public static void SetPersonXY(string name, int x, int y)
        {
			x -= PeopleTable[name].BaseWidth / 2;
			y -= PeopleTable[name].BaseHeight / 2;
            PeopleTable[name].Position = new Vector2f(x, y);
        }

        public static void SetPersonXYFloat(string name, double x, double y)
        {
			x -= PeopleTable[name].BaseWidth / 2;
			y -= PeopleTable[name].BaseHeight / 2;
            PeopleTable[name].Position = new Vector2f((float)x, (float)y);
        }

        public static void SetPersonVisible(string name, bool visible)
        {
            PeopleTable[name].Visible = visible;
        }

        public static bool IsPersonVisible(string name)
        {
            return PeopleTable[name].Visible;
        }

        public static void QueuePersonCommand(string name, int command, bool immediate)
        {
            PeopleTable[name].QueueCommand(command, immediate);
        }

        public static void QueuePersonScript(string name, object script, bool immediate)
        {
            PeopleTable[name].QueueScript(script, immediate);
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
            PeopleTable[name].Mask = color.GetColor();
        }

        public static ColorInstance GetPersonMask(string name)
        {
            return new ColorInstance(Program._engine, PeopleTable[name].Mask);
        }

        public static void SetPersonSpeed(string name, double s)
        {
            PeopleTable[name].Speed = new Vector2f((float)s, (float)s);
        }

        public static void SetPersonSpeedXY(string name, double x, double y)
        {
            PeopleTable[name].Speed = new Vector2f((float)x, (float)y);
        }

        public static void SetPersonSpriteset(string name, SpritesetInstance instance)
        {
            PeopleTable[name].Spriteset = instance;
        }

        public static SpritesetInstance GetPersonSpriteset(string name)
        {
            return PeopleTable[name].Spriteset;
        }

        public static double GetPersonSpeedX(string name)
        {
            return PeopleTable[name].Speed.X;
        }

        public static double GetPersonSpeedY(string name)
        {
            return PeopleTable[name].Speed.Y;
        }

        public static void SetPersonFrame(string name, int v)
        {
            PeopleTable[name].Frame = v;
        }

        public static int GetPersonFrame(string name)
        {
            return PeopleTable[name].Frame;
        }

        public static string GetPersonDirection(string name)
        {
            return PeopleTable[name].Direction;
        }

        public static void SetPersonDirection(string name, string d)
        {
            PeopleTable[name].Direction = d;
        }

        public static void SetPersonLayer(string name, int layer)
        {
            PeopleTable[name].Layer = layer;
        }

        public static int GetPersonLayer(string name)
        {
            return PeopleTable[name].Layer;
        }

        public static ObjectInstance GetPersonBase(string name)
        {
            return PeopleTable[name].Base;
        }

        public static void SetPersonOffsetX(string name, double x)
        {
            PeopleTable[name].Offset = new Vector2f((float)x, PeopleTable[name].Offset.Y);
        }

        public static void SetPersonOffsetY(string name, double y)
        {
            PeopleTable[name].Offset = new Vector2f(PeopleTable[name].Offset.X, (float)y);
        }

        public static double GetPersonOffsetX(string name)
        {
            return PeopleTable[name].Offset.X;
        }

        public static double GetPersonOffsetY(string name)
        {
            return PeopleTable[name].Offset.Y;
        }

        public static string GetCurrentPerson()
        {
            return CurrentPerson;
        }

        public static void ClearPersonCommands(string name)
        {
            PeopleTable[name].ClearComands();
        }

        public static int GetPersonFrameRevert(string name)
        {
            return PeopleTable[name].FrameRevert;
        }

        public static void SetPersonFrameRevert(string name, int r)
        {
            PeopleTable[name].FrameRevert = r;
        }

        public static ObjectInstance GetPersonData(string name)
        {
            // TODO: implement & update data props:
            // eithre here, or when you set the SS.
            return PeopleTable[name].Data;
        }

        public static void SetPersonData(string name, ObjectInstance o)
        {
            PeopleTable[name].Data = o;
        }

        public static void SetPersonValue(string name, string key, object o)
        {
            PeopleTable[name].Data[key] = o;
        }

        public static object GetPersonValue(string name, string key)
        {
            return PeopleTable[name].Data[key];
        }

        public static void CallPersonScript(string name, int type)
        {
            CurrentPerson = name;
            PeopleTable[name].CallScript((PersonScripts)type);
        }

        public static void SetPersonScript(string name, int type, object script)
        {
            PeopleTable[name].SetScript((PersonScripts)type, script);
        }

        public static void IgnorePersonObstructions(string name, bool value)
        {
            PeopleTable[name].IgnorePersons = value;
        }

        public static bool IsIgnoringPersonObstructions(string name)
        {
            return PeopleTable[name].IgnorePersons;
        }

        public static void IgnoreTileObstructions(string name, bool value)
        {
            PeopleTable[name].IgnoreTiles = value;
        }

        public static bool IsIgnoringTileObstructions(string name)
        {
            return PeopleTable[name].IgnoreTiles;
        }

        public static bool IsPersonObstructed(string name, double x, double y)
        {
			x -= PeopleTable[name].BaseWidth / 2;
			y -= PeopleTable[name].BaseHeight / 2;
			return PeopleTable[name].IsObstructedAt(new Vector2f((float)x, (float)y));
        }
    }
}

