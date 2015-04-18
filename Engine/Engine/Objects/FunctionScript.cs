using System;
using Jurassic.Core;
using Jurassic.Library;

namespace Engine
{
    /// <summary>
    /// Is either a JS function or a source code string to be compiled and executed efficiently.
    /// </summary>
    public class FunctionScript
    {
        #region Executable
        private interface IExecutable { void Execute(); }

        // handler for a JS function instance.
        private class FuncExe : IExecutable
        {
            FunctionInstance _instance;
            public FuncExe(FunctionInstance instance) { _instance = instance; }
            public void Execute() { _instance.Call(Program._engine.Global); }
        }

        // handler for a source code string instance.
        private class CompExe : IExecutable
        {
            CompiledMethod _method;
            public CompExe(CompiledMethod method) { _method = method; }
            public void Execute() { _method.Execute(); }
        }
        #endregion

        private IExecutable _executable;

        public FunctionScript(object item)
        {
            if (item is FunctionInstance)
                _executable = new FuncExe(item as FunctionInstance);
            else {
                _executable = new CompExe(new CompiledMethod(Program._engine, item.ToString()));
#if(DEBUG)
                Console.WriteLine("Compiled Script: \"{0}\"", item.ToString());
#endif
            }
        }

        public void Execute()
        {
            _executable.Execute();
        }
    }
}

