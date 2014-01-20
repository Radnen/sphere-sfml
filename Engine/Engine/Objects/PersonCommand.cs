using System;
using System.Collections.Generic;

namespace Engine.Objects
{
    public class PersonCommand
    {
        public readonly int command;
        public readonly bool immediate;
        public readonly FunctionScript script;

        private static Dictionary<string, FunctionScript> _cache = new Dictionary<string, FunctionScript>();

        public PersonCommand(int cmd, bool imm, object code = null)
        {
            command = cmd;
            immediate = imm;
            if (code != null && (code is string || code is Jurassic.ConcatenatedString))
            {
                string key = code.ToString();
                if (!_cache.ContainsKey(key))
                {
                    script = new FunctionScript(code);
                    _cache[key] = script;
                }
                else script = _cache[key];
            }
            else script = null;
        }
    }
}
