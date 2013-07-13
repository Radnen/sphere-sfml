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
            : base (proto)
        {
            Color c = color.GetColor();
            this["red"] = (int)c.R;
            this["green"] = (int)c.G;
            this["blue"] = (int)c.B;
            this["alpha"] = (int)c.A;
        }

        /// <summary>
        /// Gets the underlying SFML color.
        /// </summary>
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

