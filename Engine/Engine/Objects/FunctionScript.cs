using System;
using Jurassic;
using Jurassic.Library;

namespace Engine
{
    /// <summary>
    /// Is either a function or compiled source that can be executed.
    /// </summary>
    public class FunctionScript
    {
        private bool _isFunc = false;
        private CompiledMethod _compiled;
        private FunctionInstance _func;

        public FunctionScript(object item)
        {
            _isFunc = item is FunctionInstance;
            string source = null;
            if (item is string || item is ConcatenatedString) // No idea why concatenated strings are treated any different...
                source = item.ToString();
            else if (_isFunc)
                _func = item as FunctionInstance;
            else
                throw new InvalidCastException("Parameter not of type string or function: " + item.GetType());

            if (!_isFunc)
            {
                _compiled = new CompiledMethod(Program._engine, source);
#if(DEBUG)
                Console.WriteLine("Compiled Script: \"{0}\"", source);
#endif
            }
        }

        public void Execute()
        {
            if (_isFunc)
                _func.Call(Program._engine.Global);
            else
                _compiled.Execute();
        }
    }
}

