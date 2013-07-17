using System;
using System.IO;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class SpritesetInstance : ObjectInstance
    {
        short _version;
        short _width, _height;
        short _bx1, _by1, _bx2, _by2;

        public SpritesetInstance(ScriptEngine parent)
            : base(parent)
        {
            PopulateFunctions();
        }

        public SpritesetInstance(ScriptEngine parent, string filename)
            : base (parent)
        {
            ReadFromFile(filename);
            PopulateFunctions();
        }

        public SpritesetInstance(ScriptEngine parent, int width, int height, int images, int dirs, int frames)
            : base (parent)
        {
            PopulateFunctions();
            this["images"] = Program._engine.Array.New(new object[images]);

            object[] directions = new object[dirs];
            for (var i = 0; i < dirs; ++i)
            {
                ObjectInstance direction = Program.CreateObject();
                direction["name"] = "";
                object[] frames_array = new object[frames];
                for (var f = 0; f < frames; ++f)
                {
                    ObjectInstance frame = Program.CreateObject();
                    frame["index"] = (int)0;
                    frame["delay"] = (int)8;
                    frames_array[i] = frame;
                }

                direction["frames"] = Program._engine.Array.New(frames_array);
                directions[i] = direction;
            }

            this["directions"] = Program._engine.Array.New(directions);
        }

        private void ReadFromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException();

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                if (!new String(reader.ReadChars(4)).Equals(".rss"))
                    throw new SpritesetException(filename, "Invalid header.");
                _version = reader.ReadInt16();
                short num_images = reader.ReadInt16();
                _width = reader.ReadInt16();
                _height = reader.ReadInt16();
                short num_dir = reader.ReadInt16();
                _bx1 = reader.ReadInt16(); _by1 = reader.ReadInt16();
                _bx2 = reader.ReadInt16(); _by2 = reader.ReadInt16();
                reader.ReadBytes(106);

                if (_version != 3)
                    throw new SpritesetException(filename, "Invalid version");
                else
                {
                    object[] images = new object[num_images];
                    for (var i = 0; i < num_images; ++i)
                    {
                        int size = _width * _height * 4;
                        Image frame = new Image((uint)_width, (uint)_height, reader.ReadBytes(size));
                        images[i] = new ImageInstance(Program._engine, new Texture(frame), false);
                    }
                    this["images"] = Program._engine.Array.New(images);

                    object[] dirs = new object[num_dir];
                    for (var d = 0; d < num_dir; ++d)
                    {
                        short num_frames = reader.ReadInt16();
                        reader.ReadBytes(6);
                        short len = reader.ReadInt16();
                        string name = new string(reader.ReadChars(len));
                        object[] frames = new object[num_frames];

                        for (var f = 0; f < num_frames; ++f)
                        {
                            short index = reader.ReadInt16();
                            short delay = reader.ReadInt16();
                            reader.ReadBytes(4);

                            frames[f] = Program.CreateObject();
                            ((ObjectInstance)frames[f])["index"] = (int)index;
                            ((ObjectInstance)frames[f])["delay"] = (int)delay;
                        }

                        dirs[d] = Program.CreateObject();
                        ((ObjectInstance)dirs[d])["name"] = name;
                        ((ObjectInstance)dirs[d])["frames"] = Program._engine.Array.New(frames);
                    }
                    this["directions"] = Program._engine.Array.New(dirs);
                }
            }
        }

        private void SaveToFile(string filename)
        {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filename)))
            {
                writer.Write(".rss".ToCharArray());
                writer.Write(_version);
                writer.Write((short)((ArrayInstance)this["images"]).Length);
                writer.Write(_width);
                writer.Write(_height);
                writer.Write((short)((ArrayInstance)this["directions"]).Length);
                writer.Write(_bx1);
                writer.Write(_by1);
                writer.Write(_bx2);
                writer.Write(_by2);
                writer.Write(new Byte[106]);

                ArrayInstance images = this["images"] as ArrayInstance;
                for (var i = 0; i < images.Length; ++i)
                {
                    Image img = ((ImageInstance)images[i]).GetImage();
                    writer.Write(img.Pixels);
                }

                ArrayInstance dirs = this["directions"] as ArrayInstance;
                for (var d = 0; d < dirs.Length; ++d)
                {
                    ObjectInstance direction = dirs[d] as ObjectInstance;
                    ArrayInstance frames = direction["frames"] as ArrayInstance;

                    writer.Write((short)frames.Length);
                    writer.Write(new Byte[6]);
                    writer.Write((short)((string)direction["name"]).Length);
                    writer.Write(((string)direction["name"]).ToCharArray());

                    for (var f = 0; f < frames.Length; ++f)
                    {
                        ObjectInstance frame = frames[f] as ObjectInstance; 
                        writer.Write((short)((int)frame["index"]));
                        writer.Write((short)((int)frame["delay"]));
                        writer.Write(new Byte[4]);
                    }
                }
            }
        }

        [JSFunction(Name = "save")]
        public void Save(string filename)
        {
            SaveToFile(filename);
        }

        [JSFunction(Name = "clone")]
        public SpritesetInstance Clone()
        {
            SpritesetInstance instance = new SpritesetInstance(Engine);
            instance._version = _version;
            instance._bx1 = _bx1; instance._bx2 = _bx2;
            instance._by1 = _by1; instance._by2 = _by2;
            instance._width = _width;
            instance._height = _height;

            ArrayInstance my_images = this["images"] as ArrayInstance;
            object[] images = new object[my_images.Length];
            for (var i = 0; i < my_images.Length; ++i)
            {
                images[i] = ((ImageInstance)my_images[i]).Clone();
            }
            instance["images"] = Program._engine.Array.New(images);

            ArrayInstance directions = this["directions"] as ArrayInstance;
            object[] dirs = new object[directions.Length];
            for (var i = 0; i < directions.Length; ++i)
            {
                ObjectInstance direction = Program.CreateObject();
                direction["name"] = ((ObjectInstance)directions[i])["name"];
                ArrayInstance frames_array = ((ObjectInstance)directions[i])["frames"] as ArrayInstance;
                object[] frames = new object[frames_array.Length];
                for (var f = 0; f < frames.Length; ++f)
                {
                    ObjectInstance frame = Program.CreateObject();
                    frame["index"] = ((ObjectInstance)frames_array[f])["index"];
                    frame["delay"] = ((ObjectInstance)frames_array[f])["delay"];
                    frames[f] = frame;
                }
                direction["frames"] = Program._engine.Array.New(frames);
                directions[i] = direction;
            }
            instance["directions"] = Program._engine.Array.New(dirs);

            return instance;
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object spriteset]";
        }
    }

    public class SpritesetException : Exception
    {
        public SpritesetException(string filename, string msg)
            : base(string.Format("Invalid WindowStyle at {0}: {1}", filename, msg))
        {
        }
    }
}
