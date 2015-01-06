using System;
using Jurassic.Core;
using Jurassic.Library;

namespace Engine
{
    /// <summary>
    /// Is either a function or some source code to be compiled that can be executed efficiently.
    /// </summary>
    public class FunctionScript
    {
        #region Executable
        private interface IExecutable { void Execute(); }

        private class FuncExe : IExecutable
        {
            FunctionInstance _instance;
            public FuncExe(FunctionInstance instance) { _instance = instance; }
            public void Execute() { _instance.Call(Program._engine.Global); }
        }

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

