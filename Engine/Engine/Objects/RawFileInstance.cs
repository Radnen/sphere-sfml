using System;
using Jurassic;
using Jurassic.Library;
using System.IO;

namespace Engine.Objects
{
    public class RawFileInstance : ObjectInstance
    {
        Stream _data;

        public RawFileInstance(ScriptEngine parent, string filename, bool writeable)
            : base(parent)
        {
            PopulateFunctions();
            if (writeable)
                _data = File.OpenWrite(filename);
            else
                _data = File.OpenRead(filename);
        }

        [JSFunction(Name = "read")]
        public ByteArrayInstance Read(int count)
        {
            using (BinaryReader reader = new BinaryReader(_data))
            {
                ByteArrayInstance array = new ByteArrayInstance(Engine, reader.ReadBytes(count));
                return array;
            }
        }

        [JSFunction(Name = "write")]
        public void Write(ByteArrayInstance array)
        {
            using (BinaryWriter writer = new BinaryWriter(_data))
            {
                writer.Write(array.GetBytes());
            }
        }

        [JSFunction(Name = "getSize")]
        public int GetSize()
        {
            return (int)_data.Length;
        }

        [JSFunction(Name = "setPosition")]
        public void SetPosition(int pos)
        {
            _data.Position = pos;
        }

        [JSFunction(Name = "getPosition")]
        public int GetPosition()
        {
            return (int)_data.Position;
        }

        [JSFunction(Name = "close")]
        public void Close()
        {
            _data.Close();
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object rawfile]";
        }
    }
}

