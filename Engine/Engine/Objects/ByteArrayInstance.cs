using System;
using Jurassic;
using Jurassic.Library;

namespace Engine
{
    public class ByteArrayInstance : ObjectInstance
    {
        byte[] _bytes;

        public ByteArrayInstance(ScriptEngine parent, string source)
            : base(parent)
        {
            PopulateFunctions();
            _bytes = new byte[source.Length];
            for (var i = 0; i < source.Length; ++i)
                _bytes[i] = (byte)source[i];
        }

        public ByteArrayInstance(ScriptEngine parent, byte[] source)
            : base(parent)
        {
            PopulateFunctions();
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
        public void Concat(ByteArrayInstance array)
        {
            byte[] other = array.GetBytes();
            byte[] bytes = new byte[_bytes.Length + other.Length];
            int index = 0;
            for (var i = 0; i < _bytes.Length; ++i)
            {
                bytes[index] = _bytes[i];
                index++;
            }
            for (var i = 0; i < other.Length; ++i)
            {
                bytes[index] = other[i];
                index++;
            }
            _bytes = bytes;
        }

        [JSFunction(Name = "splice")]
        public ByteArrayInstance Splice(int from, int to)
        {
            if (to == -1 || to - from < 0)
                to = _bytes.Length;

            byte[] bytes = new byte[to - from];
            for (var i = to; i < from; ++i)
                bytes[i] = _bytes[i];

            return new ByteArrayInstance(Engine, bytes);
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object bytearray]";
        }
    }
}

