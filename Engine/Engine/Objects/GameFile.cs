using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Objects
{
    public class GameFile
    {
        private SortedDictionary<string, string> _data;

        public string FileName { get; private set; }

        public GameFile()
        {
            _data = new SortedDictionary<string, string>();
        }

        public bool ReadFile(string filename)
        {
            if (!File.Exists(filename))
                return false;

            FileName = filename;
            string[] lines = File.ReadAllLines(filename);
            for (var i = 0; i < lines.Length; ++i)
            {
                string[] data = lines[i].Split('=');
                if (data.Length == 2) {
                    if (data[1].Length == 0)
                        _data[data[0]] = "";
                    else
                        _data[data[0]] = data[1];
                }
            }
            return true;
        }

        public void WriteFile(string filename)
        {
            FileName = filename;
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (KeyValuePair<string, string> pair in _data)
                {
                    writer.WriteLine(pair.Key + "=" + _data[pair.Key]);
                }
            }
        }

        public void SetData(string key, object data)
        {
            _data[key] = data.ToString();
        }

        public bool TryGetData(string key, out bool result)
        {
            if (_data.ContainsKey(key))
                return bool.TryParse(_data[key], out result);
            else
                result = false;
            return false;
        }

        public bool TryGetData(string key, out double result)
        {
            if (_data.ContainsKey(key))
                return double.TryParse(_data[key], out result);
            else
                result = 0.0;
            return false;
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

        public int GetNumKeys()
        {
            return _data.Keys.Count;
        }

        public string GetKeyAt(int index)
        {
            int i = 0;
            foreach (string s in _data.Keys)
            {
                if (i == index)
                    return s;
                i++;
            }
            return "";
        }
    }
}

