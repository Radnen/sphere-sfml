using System;

namespace Engine.Objects
{
    public struct PersonCommand
    {
        public int command;
        public bool immediate;

        public PersonCommand(int cmd, bool imm)
        {
            command = cmd;
            immediate = imm;
        }
    }
}

