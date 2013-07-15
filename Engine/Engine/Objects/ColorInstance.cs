using System;
using SFML.Graphics;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    public class ColorInstance : ObjectInstance
    {
        private Color _color;

        public ColorInstance(ObjectInstance proto)
            : this(proto, 0, 0, 0, 0)
        {
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

        public ColorInstance(ObjectInstance proto, Color color)
            : this(proto, (int)color.R, (int)color.G, (int)color.B, (int)color.A)
        {
        }

        public ColorInstance(ObjectInstance proto, ColorInstance color)
            : this (proto, color.GetColor())
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

