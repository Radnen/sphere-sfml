using System;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;

namespace Engine.Objects
{
    public class ColorInstance : ObjectInstance
    {
        private Color _color;

        public ColorInstance(ScriptEngine parent)
            : this(parent, 0, 0, 0, 0)
        {
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
            : this (parent, color.GetColor())
        {
        }

        /// <summary>
        /// Gets the underlying SFML color.
        /// </summary>
        public Color GetColor()
        {
            _color.R = (byte)(Convert.ToInt32(this["red"]) % 256);
            _color.G = (byte)(Convert.ToInt32(this["green"]) % 256);
            _color.B = (byte)(Convert.ToInt32(this["blue"]) % 256);
            _color.A = (byte)(Convert.ToInt32(this["alpha"]) % 256);

            return _color;
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object color]";
        }
    }
}

