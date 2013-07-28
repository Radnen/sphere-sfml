using System;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    public class FileInstance : ObjectInstance
    {
        GameFile _file = new GameFile();

        public FileInstance(ScriptEngine parent, string filename)
            : base(parent)
        {
            PopulateFunctions();
            _file.ReadFile(filename);
        }

        [JSFunction(Name = "close")]
        public void Close()
        {
            _file.WriteFile(_file.FileName);
        }

        [JSFunction(Name = "getNumKeys")]
        public int GetNumKeys()
        {
            return _file.GetNumKeys();
        }

        [JSFunction(Name = "getKey")]
        public string GetKeyAt(int index)
        {
            return _file.GetKeyAt(index);
        }

        [JSFunction(Name = "flush")]
        public void Flush()
        {
            return;
        }

        [JSFunction(Name = "read")]
        public object Read(string key, object o)
        {
            int intdata;
            double doubledata;
            bool booldata;
            string strdata;
            Type t = o.GetType();

            if (t == typeof(string))
            {
                if (_file.TryGetData(key, out strdata))
                    return strdata;
            }
            else if (t == typeof(int))
            {
                if (_file.TryGetData(key, out intdata))
                    return intdata;
                if (_file.TryGetData(key, out doubledata))
                    return (int)doubledata;
            }
            else if (t == typeof(double))
            {
                if (_file.TryGetData(key, out doubledata))
                    return doubledata;
            }
            else if (t == typeof(bool))
            {
                if (_file.TryGetData(key, out booldata))
                    return booldata;
            }

            return o;
        }

        [JSFunction(Name = "write")]
        public void Write(string key, object value)
        {
            _file.SetData(key, value);
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object file]";
        }
    }
}

