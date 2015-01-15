using System;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    public class ByteArrayInstance : ObjectInstance
    {
        #region Private Methods
        private class GetSizeFunc : FunctionInstance
        {
            public GetSizeFunc(ScriptEngine parent) : base(parent)
            {
            }

            public override object CallLateBound(object thisObject, params object[] argumentValues)
            {
                return ((ByteArrayInstance)thisObject)._bytes.Length;
            }
        }

        private class ConcatFunc : FunctionInstance
        {
            public ConcatFunc(ScriptEngine parent)
                : base(parent)
            {
            }

            public override object CallLateBound(object thisObject, params object[] argumentValues)
            {
                return ((ByteArrayInstance)thisObject).Concat(argumentValues[0] as ByteArrayInstance);
            }
        }

        private class SliceFunc : FunctionInstance
        {
            public SliceFunc(ScriptEngine parent)
                : base(parent)
            {
            }

            public override object CallLateBound(object thisObject, params object[] argumentValues)
            {
                return ((ByteArrayInstance)thisObject).Slice((int)argumentValues[0], (int)argumentValues[1]);
            }
        }
        #endregion

        private static PropertyDescriptor[] _descriptors;

        byte[] _bytes;

        public ByteArrayInstance(ScriptEngine parent, string source)
            : base(parent)
        {
            _bytes = new byte[source.Length];
            for (int i = 0; i < _bytes.Length; ++i) { _bytes[i] = (byte)source[i]; }
            PopulateProperties();
        }

        public ByteArrayInstance(ScriptEngine parent, byte[] source)
            : base(parent)
        {
            _bytes = source;
        }

        private void PopulateProperties()
        {
            PopulateDescriptors(Engine);
            DefineProperty("getSize", _descriptors[0], false);
            DefineProperty("concat", _descriptors[1], false);
            DefineProperty("slice", _descriptors[2], false);
            DefineProperty("length", _descriptors[3], false);
            DefineProperty("toString", _descriptors[4], false);
        }

        private static void PopulateDescriptors(ScriptEngine parent)
        {
            if (_descriptors != null) return;
            _descriptors = new PropertyDescriptor[5];
            _descriptors[0] = new PropertyDescriptor(new GetSizeFunc(parent), PropertyAttributes.Sealed);
            _descriptors[1] = new PropertyDescriptor(new ConcatFunc(parent), PropertyAttributes.Sealed);
            _descriptors[2] = new PropertyDescriptor(new SliceFunc(parent), PropertyAttributes.Sealed);
            _descriptors[3] = new PropertyDescriptor(new GetSizeFunc(parent), null, PropertyAttributes.Sealed);
            _descriptors[4] = new PropertyDescriptor(new ToStringFunc(parent, "bytearray"), PropertyAttributes.Sealed);
        }

        public byte[] GetBytes()
        {
            return _bytes;
        }

        public int GetSize()
        {
            return _bytes.Length;
        }

        public ByteArrayInstance Concat(ByteArrayInstance array)
        {
            int start = _bytes.Length;
            byte[] bytes = new byte[array._bytes.Length + start];
            Buffer.BlockCopy(_bytes, 0, bytes, 0, start);
            Buffer.BlockCopy(array._bytes, 0, bytes, start, array._bytes.Length);
            return new ByteArrayInstance(Engine, bytes);
        }

        public ByteArrayInstance Slice(int from, int to)
        {
            if (from < 0) from = 0;
            if (to == -1 || to - from < 0)
                to = _bytes.Length;

            byte[] bytes = new byte[to - from];
            Buffer.BlockCopy(_bytes, from, bytes, 0, to - from);

            return new ByteArrayInstance(Engine, bytes);
        }

        public override string ToString()
        {
            return "[object bytearray]";
        }
    }
}

