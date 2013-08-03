using System;

namespace Engine.Objects
{
    public struct PersonCommand
    {
        public readonly int command;
        public readonly bool immediate;
        public readonly FunctionScript script;

        public PersonCommand(int cmd, bool imm, object code = null)
        {
            command = cmd;
            immediate = imm;
            if (code != null)
                script = new FunctionScript(code);
            else
                script = null;
        }
    }
}

