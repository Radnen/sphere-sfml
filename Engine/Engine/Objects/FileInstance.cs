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
            if (o.GetType() == typeof(int))
            {
                if (_file.TryGetData(key, out intdata))
                    return intdata;
                if (_file.TryGetData(key, out doubledata))
                    return (int)doubledata;
            }
            else if (o.GetType() == typeof(double))
            {
                if (_file.TryGetData(key, out doubledata))
                    return doubledata;
            }
            else if (o.GetType() == typeof(bool))
            {
                if (_file.TryGetData(key, out booldata))
                    return booldata;
            }
            else if (o.GetType() == typeof(string))
            {
                if (_file.TryGetData(key, out strdata))
                    return strdata;
            }
            return o;
        }

        [JSFunction(Name = "write")]
        public void Write(string key, object value)
        {
            _file.SetData(key, value);
        }
    }
}

