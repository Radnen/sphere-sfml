using System;
using Jurassic.Core;
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
            if (_isFunc)
                _func = item as FunctionInstance;
            else {
                _compiled = new CompiledMethod(Program._engine, item.ToString());
#if(DEBUG)
                Console.WriteLine("Compiled Script: \"{0}\"", source);
#endif
            }
        }

        public void Execute()
        {
            try
            {
                if (_isFunc)
                    _func.Call(Program._engine.Global);
                else
                    _compiled.Execute();
            }
            catch (Exception e) {
                //Console.WriteLine(e.Message + "\n" + _compiled == null + ", " + _isFunc);
            }
        }
    }
}

