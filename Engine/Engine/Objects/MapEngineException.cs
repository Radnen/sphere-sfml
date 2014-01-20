using Jurassic;

namespace Engine.Objects
{
    class MapEngineException : JavaScriptException
    {
        public MapEngineException(string type, string msg) : base(Program._engine, type, msg) { }
    }
}
