using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    public class ToStringFunc : FunctionInstance
    {
        string _type = "object";

        public ToStringFunc(ScriptEngine engine, string type)
            : base(engine)
        {
            _type = string.Format("[object {0}]", type);
        }

        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            return _type;
        }
    }
}
