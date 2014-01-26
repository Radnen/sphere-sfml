using System;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;

namespace Engine.Objects
{
    public class ColorConstructor : ClrFunction
    {
        public ColorConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Color", new ColorInstance(engine.Object.InstancePrototype))
        {
        }

        [JSConstructorFunction]
        public ColorInstance Construct(int r, int g, int b, [DefaultParameterValue(255)] int a = 255)
        {
            return new ColorInstance(InstancePrototype, r, g, b, a);
        }
    }

    public class ColorInstance : ObjectInstance
    {
        private Color _color;

        public ColorInstance(ObjectInstance prototype, int r, int g, int b, int a)
            : base(prototype)
        {
            PopulateFunctions();
            this["red"] = r;
            this["green"] = g;
            this["blue"] = b;
            this["alpha"] = a;
            _color = new Color((byte)r, (byte)g, (byte)b, (byte)a);
        }

        public ColorInstance(ObjectInstance prototype)
            : base (prototype)
        {
            PopulateFunctions();
        }

        public ColorInstance(ScriptEngine parent, int r, int g, int b, int a)
            : base(parent)
        {
            PopulateFunctions();
            this["red"] = r;
            this["green"] = g;
            this["blue"] = b;
            this["alpha"] = a;
            _color = new Color((byte)r, (byte)g, (byte)b, (byte)a);
        }

        public ColorInstance(ScriptEngine parent, Color color)
            : this(parent, color.R, color.G, color.B, color.A)
        {
        }

        public ColorInstance(ScriptEngine parent, ColorInstance color)
            : this(parent, color._color)
        {
        }

        /// <summary>
        /// Gets the underlying SFML color.
        /// </summary>
        public Color GetColor()
        {
            _color.R = Convert.ToByte(this["red"]);
            _color.G = Convert.ToByte(this["green"]);
            _color.B = Convert.ToByte(this["blue"]);
            _color.A = Convert.ToByte(this["alpha"]);

            return _color;
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object color]";
        }
    }
}

