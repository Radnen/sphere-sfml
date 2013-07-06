using System;
using SFML.Graphics;
using Jurassic;
using Jurassic.Library;

namespace Engine
{
    public class ColorConstructor : ClrFunction
    {
        public ColorConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Color", new ColorInstance(engine.Object.InstancePrototype))
        {
        }

        [JSConstructorFunction]
        public ColorInstance Construct(int r, int g, int b, int a)
        {
            return new ColorInstance(InstancePrototype, r, g, b, a);
        }
    }

    public class ColorInstance : ObjectInstance
    {
        private Color _color;

        public ColorInstance(ObjectInstance proto)
            : base(proto)
        {
            PopulateFunctions();
            this["red"] = 0;
            this["green"] = 0;
            this["blue"] = 0;
            this["alpha"] = 0;
        }

        public ColorInstance(ObjectInstance proto, int r, int g, int b, int a)
            : base(proto)
        {
            PopulateFunctions();
            this["red"] = r;
            this["green"] = g;
            this["blue"] = b;
            this["alpha"] = a;
        }

        public Color GetColor()
        {
            _color.R = (byte)((int)this["red"]);
            _color.G = (byte)((int)this["green"]);
            _color.B = (byte)((int)this["blue"]);
            _color.A = (byte)((int)this["alpha"]);
            return _color;
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object color]";
        }
    }
}

