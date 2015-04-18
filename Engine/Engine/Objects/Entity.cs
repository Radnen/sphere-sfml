using System;
using System.Collections.Generic;
using System.IO;
using Jurassic.Core;
using SFML.Graphics;
using SFML.System;

namespace Engine.Objects
{
    /// <summary>
    /// A representation of an entity.
    /// </summary>
    public class Entity : IDisposable
    {
        #region attributes
        /// <summary>
        /// Gets or sets the x location of this Entity in pixels.
        /// </summary>
        public short X { get; set; }

        /// <summary>
        /// Gets or sets the y location of this Entity in pixels.
        /// </summary>
        public short Y { get; set; }

        /// <summary>
        /// Gets or sets the layer index of this Entity.
        /// </summary>
        public short Layer { get; set; }

        /// <summary>
        /// Gets or sets the name of this entity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the spriteset filename of this entity.
        /// </summary>
        public string Spriteset { get;  set; }

        /// <summary>
        /// Gets or sets the JS function of this Entity.
        /// </summary>
        public string Function { get; set; }

        public CompiledMethod Trigger { get; set; }

        /// <summary>
        /// Gets the list of string associated with this Entity.
        /// </summary>
        public List<string> Scripts { get; private set; }

        /// <summary>
        /// Gets or sets the visibility of this Entity.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the Entity.EntityType of this Entity.
        /// </summary>
        public EntityType Type { get; set; }

        private Image _graphic;
        private bool _disposed;
        #endregion

        /// <summary>
        /// A Sphere entity type.
        /// </summary>
        public enum EntityType
        {
            /// <summary>
            /// A Sphere Person type.
            /// </summary>
            Person = 1,

            /// <summary>
            /// A Sphere Trigger type.
            /// </summary>
            Trigger = 2
        };

        /// <summary>
        /// Creates a new Entity with the supplied type.
        /// </summary>
        /// <param name="type">The Entity.EntityType to use.</param>
        public Entity(EntityType type)
        {
            Type = type;
            Scripts = new List<string>();
            for (int i = 0; i < 5; ++i) Scripts.Add("");
        }

        /// <summary>
        /// Creates a new entity that was embedded next in the stream
        /// </summary>
        /// <param name="stream">The System.IO.BinaryReader to use.</param>
        public Entity(BinaryReader stream)
        {
            X = stream.ReadInt16();
            Y = stream.ReadInt16();
            Layer = stream.ReadInt16();
            Scripts = new List<string>();
            Type = (EntityType)stream.ReadInt16();
            stream.ReadBytes(8);

            short len;
            if (Type == EntityType.Person)
            {
                len = stream.ReadInt16();
                Name = new string(stream.ReadChars(len));
                len = stream.ReadInt16();
                Spriteset = new string(stream.ReadChars(len));
                int scripts = stream.ReadInt16();

                // read the person script data
                for (int i = 0; i < scripts; ++i)
                {
                    len = stream.ReadInt16();
                    Scripts.Add(new string(stream.ReadChars(len)));
                }

                stream.ReadBytes(16); // reserved
            }
            else
            {
                len = stream.ReadInt16();
                Function = new string(stream.ReadChars(len));
                Trigger = new CompiledMethod(Program._engine, Function);
            }
        }

        /// <summary>
        /// Saves the entity, embedding it into a file stream.
        /// </summary>
        /// <param name="binwrite">The System.IO.BinaryWriter to use.</param>
        public void Save(BinaryWriter binwrite)
        {
            // Header Data //
            binwrite.Write(X);
            binwrite.Write(Y);
            binwrite.Write(Layer);
            binwrite.Write((short)Type);
            binwrite.Write(new byte[8]);

            // Type Data //
            if (Type == EntityType.Person)
            {
                binwrite.Write((short)Name.Length);
                binwrite.Write(Name.ToCharArray());
                binwrite.Write((short)Spriteset.Length);
                binwrite.Write(Spriteset.ToCharArray());

                binwrite.Write((short)Scripts.Count);
                foreach (string s in Scripts)
                {
                    binwrite.Write((short)s.Length);
                    binwrite.Write(s.ToCharArray());
                }
                binwrite.Write(new byte[16]);
            }
            else
            {
                binwrite.Write((short)Function.Length);
                binwrite.Write(Function.ToCharArray());
            }
        }

        /// <summary>
        /// Executes the trigger.
        /// </summary>
        public void ExecuteTrigger()
        {
            if (Type == EntityType.Person) return;
            Trigger.Execute();
        }

        /// <summary>
        /// Creates a perfect copy of this entity.
        /// </summary>
        /// <returns>A copy of the Entity.</returns>
        public Entity Copy()
        {
            Entity ent = new Entity(Type) {X = X, Y = Y, Layer = Layer, Visible = Visible};
            if (Type == EntityType.Person)
            {
                string[] strings = new string[5];
                Scripts.CopyTo(strings);
                ent.Name = Name;
                ent.Scripts.Clear();
                ent.Scripts.AddRange(strings);
                ent.Spriteset = Spriteset;
            }
            else if (Type == EntityType.Trigger)
            {
                ent.Function = Function;
            }
            return ent;
        }

        /// <summary>
        /// Dispose and close this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_graphic != null) _graphic.Dispose();
                }

                _graphic = null;
            }
            _disposed = true;
        }
    }
}
