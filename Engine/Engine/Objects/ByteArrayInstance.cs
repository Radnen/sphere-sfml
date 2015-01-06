using System;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    public class ByteArrayInstance : ObjectInstance
    {
        byte[] _bytes;

        public ByteArrayInstance(ScriptEngine parent, string source)
            : base(parent)
        {
            PopulateFunctions();
            this["length"] = source.Length;
            _bytes = new byte[source.Length];
            for (var i = 0; i < source.Length; ++i)
                _bytes[i] = (byte)source[i];
        }

        public ByteArrayInstance(ScriptEngine parent, byte[] source)
            : base(parent)
        {
            PopulateFunctions();
            this["length"] = source.Length;
            _bytes = source;
        }

        public byte[] GetBytes()
        {
            return _bytes;
        }

        [JSFunction(Name = "getSize")]
        public int GetSize()
        {
            return _bytes.Length;
        }

        [JSFunction(Name = "concat")]
        public ByteArrayInstance Concat(ByteArrayInstance array)
        {
            int old = _bytes.Length, start = GetSize();
            byte[] other = array.GetBytes();
            byte[] bytes = new byte[other.Length + start];
            Array.Copy(GetBytes(), 0, bytes, 0, start);
            Array.Copy(other, 0, bytes, start, array.GetSize());
            return new ByteArrayInstance(Engine, bytes);
        }

        [JSFunction(Name = "slice")]
        public ByteArrayInstance Slice(int from, int to)
        {
            if (from < 0) from = 0;
            if (to == -1 || to - from < 0)
                to = _bytes.Length;

            byte[] bytes = new byte[to - from];
            Array.Copy(_bytes, from, bytes, 0, to - from);

            return new ByteArrayInstance(Engine, bytes);
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object bytearray]";
        }
    }
}

