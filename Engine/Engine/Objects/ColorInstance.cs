using System;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;

namespace Engine.Objects
{
    public class ColorInstance : ObjectInstance
    {
        #region internal methods
        public class ColorGetter : FunctionInstance
        {
            int _type;
            public ColorGetter(ScriptEngine engine, int type)
                : base(engine)
            {
                _type = type;
            }

            public override object CallLateBound(object thisObject, params object[] argumentValues)
            {
                var color = (ColorInstance)thisObject;
                switch (_type)
                {
                    case 0: return color.R;
                    case 1: return color.G;
                    case 2: return color.B;
                    case 3: return color.A;
                }
                return Undefined.Value;
            }
        }

        public class ColorSetter : FunctionInstance
        {
            int _type;
            public ColorSetter(ScriptEngine engine, int type)
                : base(engine)
            {
                _type = type;
            }

            public override object CallLateBound(object thisObject, params object[] argumentValues)
            {
                var color = (ColorInstance)thisObject;
                int v = TypeConverter.ToInt32(argumentValues[0]);
                switch (_type)
                {
                    case 0: return color.R = v;
                    case 1: return color.G = v;
                    case 2: return color.B = v;
                    case 3: return color.A = v;
                }

                return v;
            }
        }
        #endregion

        private Color _color;
        private static PropertyDescriptor[] _descriptors;

        public int R { get { return _color.R; } set { _color.R = (byte)value; } }
        public int G { get { return _color.G; } set { _color.G = (byte)value; } }
        public int B { get { return _color.B; } set { _color.B = (byte)value; } }
        public int A { get { return _color.A; } set { _color.A = (byte)value; } }

        public Color Color { get { return _color; } }

        public ColorInstance(ObjectInstance prototype)
            : base (prototype)
        {
        }

        public ColorInstance(ScriptEngine parent, int r, int g, int b, int a)
            : base(parent)
        {
            _color = new Color((byte)r, (byte)g, (byte)b, (byte)a);

            PopulateDescriptors(parent);

            DefineProperty("red", _descriptors[0], false);
            DefineProperty("green", _descriptors[1], false);
            DefineProperty("blue", _descriptors[2], false);
            DefineProperty("alpha", _descriptors[3], false);
            DefineProperty("toString", _descriptors[4], false);
        }

        /// <summary>
        /// The idea here is to cache descriptors so that we have lightning fast performance.
        /// </summary>
        /// <param name="parent">The ScriptEngine that owns this ColorInstance.</param>
        private static void PopulateDescriptors(ScriptEngine parent)
        {
            if (_descriptors != null) return;

            _descriptors = new PropertyDescriptor[5];
            for (int i = 0; i < 4; ++i)
            {
                var getter = new ColorGetter(parent, i);
                var setter = new ColorSetter(parent, i);
                _descriptors[i] = new PropertyDescriptor(getter, setter, PropertyAttributes.Sealed);
            }
            _descriptors[4] = new PropertyDescriptor(new ToStringFunc(parent, "color"), PropertyAttributes.Sealed);
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
            return _color;
        }

        /// <summary>
        /// Gets the color as a packed integer.
        /// </summary>
        public int GetInt()
        {
            return (A << 24) | (B << 16) | (G << 8) | R;
        }

        /// <summary>
        /// Standard Sphere ToString()
        /// </summary>
        public override string ToString()
        {
            return "[object color]";
        }
    }
}

