using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Objects
{
    public class GameFile
    {
        private Dictionary<string, string> _data;

        public string FileName { get; private set; }

        public GameFile()
        {
            _data = new Dictionary<string, string>();
        }

        public bool ReadFile(string filename)
        {
            if (!File.Exists(filename))
                return false;

            FileName = filename;
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
            if (_data.ContainsKey(key))
                return bool.TryParse(_data[key], out result);
            else {
                result = false;
                return false;
            }
        }

        public bool TryGetData(string key, out int result)
        {
            if (_data.ContainsKey(key))
                return int.TryParse(_data[key], out result);
            else
                result = 0;
                return false;
        }

        public bool TryGetData(string key, out string result)
        {
            return _data.TryGetValue(key, out result);
        }
    }
}

