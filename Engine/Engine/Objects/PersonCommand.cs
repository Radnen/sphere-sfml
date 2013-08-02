using System;

namespace Engine.Objects
{
    public struct PersonCommand
    {
        public readonly int command;
        public readonly bool immediate;

        public PersonCommand(int cmd, bool imm)
        {
            command = cmd;
            immediate = imm;
        }
    }
}

