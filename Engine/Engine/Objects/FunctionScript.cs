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
        private CompiledMethod _compilede;
        private FunctionInstance _func;

        public FunctionScript(object item)
        {
            _isFunc = item is FunctionInstance;
            if (item is string)
            {
                _compilede = new CompiledMethod(Program._engine, (string)item);
            }
            else if (item is FunctionInstance)
            {
                _func = (FunctionInstance)item;
            }
            else
                throw new InvalidCastException("Parameter not of type string or function.");
        }

        public void Execute()
        {
            if (_isFunc)
                _func.Call(Program._engine.Global);
            else
                _compilede.Execute();
        }
    }
}

