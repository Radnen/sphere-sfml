using System;
using System.Collections.Generic;
using System.IO;

namespace Engine
{
    public class GameFile
    {
        private Dictionary<string, string> _data;

        public GameFile()
        {
            _data = new Dictionary<string, string>();
        }

        public bool ReadFile()
        {
            string filename = GlobalProps.BasePath + "/game.sgm";

            if (!File.Exists(filename))
                return false;

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string key = line.Substring(0, line.IndexOf("="));
                    string value = line.Substring(line.IndexOf("=") + 1);
                    _data.Add(key, value);
                }
            }

            return true;
        }

        public bool TryGetData(string key, out bool result)
        {
            string data = "";
            bool failed = _data.TryGetValue(key, out data);
            if (!failed)
                return bool.TryParse(data, out result);
            else
            {
                result = false;
                return false;
            }
        }

        public bool TryGetData(string key, out int result)
        {
            string data = "";
            bool failed = _data.TryGetValue(key, out data);
            if (!failed)
                return int.TryParse(data, out result);
            else
            {
                result = 0;
                return false;
            }
        }

        public bool TryGetData(string key, out string result)
        {
            return _data.TryGetValue(key, out result);
        }
    }
}

