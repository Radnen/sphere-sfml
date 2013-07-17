using System;
using SFML.Graphics;
using Jurassic;
using Jurassic.Library;

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
        }

        public ColorInstance(ScriptEngine parent, Color color)
            : this(parent, (int)color.R, (int)color.G, (int)color.B, (int)color.A)
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
            _color.R = (byte)Double.Parse(this["red"].ToString());
            _color.G = (byte)Double.Parse(this["green"].ToString());
            _color.B = (byte)Double.Parse(this["blue"].ToString());
            _color.A = (byte)Double.Parse(this["alpha"].ToString());
            return _color;
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object color]";
        }
    }
}

