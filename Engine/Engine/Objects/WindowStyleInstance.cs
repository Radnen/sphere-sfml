using System;
using SFML.Graphics;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    public class WindowStyleInstance : ObjectInstance
    {
        RenderWindow _window;

        public WindowStyleInstance(ObjectInstance proto, string filename, RenderWindow window)
            : base(proto)
        {
            _window = window;
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object windowstyle]";
        }
    }
}

