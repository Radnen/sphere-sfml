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
            if (item is Jurassic.ConcatenatedString) // I think this is a jurassic parse error.
                source = item.ToString();
            else if (item is string)
                source = item.ToString();
            else if (item is FunctionInstance)
                _func = item as FunctionInstance;
            else
                throw new InvalidCastException("Parameter not of type string or function: " + item.GetType());

            if (!_isFunc)
            {
                _compiled = new CompiledMethod(Program._engine, source);
                Console.WriteLine("Compiled Script: \"{0}\"", source);
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

