﻿using System;
using System.Collections.Generic;
using System.IO;
using SFML.System;
using SFML.Graphics;

namespace Engine.Objects
{
    /// <summary>
    /// A Sphere map object.
    /// </summary>
    public class Map : IDisposable
    {
        #region Attributes
        private short _version = 1;

        /// <summary>
        /// Gets or sets the starting x position.
        /// </summary>
        public short StartX { get; set; }

        /// <summary>
        /// Gets or sets the starting y position.
        /// </summary>
        public short StartY { get; set; }

        /// <summary>
        /// Gets or sets the start layer.
        /// </summary>
        public byte StartLayer { get; set; }

        /// <summary>
        /// Gets a list of string used by this Map.
        /// </summary>
        public List<string> Scripts { get; private set; }

        /// <summary>
        /// Gets or sets a list of layers used by this Map.
        /// </summary>
        public List<Layer> Layers { get; set; }

        /// <summary>
        /// Gets a list of entities used by this Map.
        /// </summary>
        public List<Entity> Entities { get; private set; }

        /// <summary>
        /// Gets a list of zones used by this map.
        /// </summary>
        public List<Zone> Zones { get; private set; }

        /// <summary>
        /// Gets the width of the zero'th layer in the map.
        /// </summary>
        public int Width
        {
            get { return Layers[0].Width; }
        }

        /// <summary>
        /// Gets the height of the zero'th layer in the Map.
        /// </summary>
        public int Height
        {
            get { return Layers[0].Height; }
        }

        /// <summary>
        /// Gets or sets the tileset associated with this map.
        /// </summary>
        public Tileset Tileset { get; set; }

        /// <summary>
        /// Gets if the map had an error while loading.
        /// </summary>
        public bool ErrorOnLoad { get; private set; }
        #endregion

        /// <summary>
        /// Creates a new, empty map.
        /// </summary>
        public Map()
        {
            Scripts = new List<string>();
            Layers = new List<Layer>();
            Entities = new List<Entity>();
            Zones = new List<Zone>();
        }

        /// <summary>
        /// Creates a new map from file.
        /// </summary>
        public Map(string filename)
            : this()
        {
            Load(filename);
        }

        /// <summary>
        /// Creates a new map with values.
        /// </summary>
        /// <param name="width">The width in tiles.</param>
        /// <param name="height">The height in tiles.</param>
        /// <param name="tileWidth">The tilewidth in pixels.</param>
        /// <param name="tileHeight">The tileheight in pixels.</param>
        /// <param name="tilesetPath">The path to the tileset.</param>
        public void CreateNew(short width, short height, short tileWidth, short tileHeight, string tilesetPath)
        {
            for (int i = 0; i < 9; ++i) Scripts.Add("");

            // create a base layer:
            Layer layer = new Layer();
            layer.CreateNew(width, height);
            Layers.Add(layer);

            // create a starting tile:
            Tileset = new Tileset();

            if (string.IsNullOrEmpty(tilesetPath))
                Tileset.CreateNew(tileWidth, tileHeight);
            else
            {
                Tileset = Tileset.FromFile(tilesetPath);
                Scripts[0] = Path.GetFileName(tilesetPath);
            }
        }

        /// <summary>
        /// Loads a map from the given filename.
        /// </summary>
        /// <param name="filename">The filename of the map.</param>
        /// <returns>True if the load was a success.</returns>
        public bool Load(string filename)
        {
            if (!File.Exists(filename))
                Program.Abort(string.Format("File {0} not found.", filename));

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                // read header:
                reader.ReadChars(4);
                _version = reader.ReadInt16();
                reader.ReadByte();
                int numLayers = reader.ReadByte();
                reader.ReadByte();
                int numEntities = reader.ReadInt16();
                StartX = reader.ReadInt16();
                StartY = reader.ReadInt16();
                StartLayer = reader.ReadByte();
                reader.ReadByte();
                int numStrings = reader.ReadInt16();
                int numZones = reader.ReadInt16();
                reader.ReadBytes(235);

                // read scripts:
                while (numStrings-- > 0)
                {
                    short length = reader.ReadInt16();
                    Scripts.Add(new string(reader.ReadChars(length)));
                }

                // read layers:
                while (numLayers-- > 0)
                {
                    Layers.Add(Layer.FromBinary(reader));
                    _vertex_layers.Add(new Vertex[]{ });
                }

                // read entities:
                while (numEntities-- > 0)
                    Entities.Add(new Entity(reader));

                // read zones:
                while (numZones-- > 0)
                    Zones.Add(Zone.FromBinary(reader));

                // read tileset:
                if (Scripts[0].Length == 0)
                    Tileset = Tileset.FromBinary(reader);
                else
                {
                    string path = Path.GetDirectoryName(filename) + "\\" + Scripts[0];
                    Tileset = Tileset.FromFile(path);
                }

                // init all layers:
                bool validated = true;
                foreach (Layer layer in Layers)
                {
                    validated = layer.Validate(Tileset.Tiles.Count);
                }
                ErrorOnLoad = !validated;
            }

            return true;
        }

        /// <summary>
        /// Attempts to save the map to the given filename.
        /// </summary>
        /// <param name="filename">The path to save the Map to.</param>
        /// <returns>True if the save was successful.</returns>
        public bool Save(string filename)
        {
            if (Scripts.Count == 0 || Scripts[0].Length == 0) return false;
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filename)))
            {
                // write header:
                writer.Write(".rmp".ToCharArray());
                writer.Write(_version);
                writer.Write(byte.MinValue);
                writer.Write((byte)Layers.Count);
                writer.Write(byte.MinValue);
                writer.Write((short)Entities.Count);
                writer.Write(StartX);
                writer.Write(StartY);
                writer.Write(StartLayer);
                writer.Write(byte.MinValue);
                writer.Write((short)Scripts.Count);
                writer.Write((short)Zones.Count);
                writer.Write(new byte[235]);

                // write scripts:
                foreach (string s in Scripts)
                {
                    writer.Write((short)s.Length);
                    writer.Write(s.ToCharArray());
                }

                // save layers:
                foreach (Layer l in Layers) l.Save(writer);

                // save entities:
                foreach (Entity e in Entities) e.Save(writer);

                // save zones:
                foreach (Zone z in Zones) z.Save(writer);

                writer.Flush();
            }

            string path = filename.Substring(0, filename.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            Tileset.Save(path + Scripts[0]);
            
            return true;
        }

        private List<Vertex[]> _vertex_layers = new List<Vertex[]>();
        private RenderStates _states = new RenderStates(BlendMode.Alpha, Transform.Identity, null, null);

        /// <summary>
        /// Compiles the map into a texture-coordinated vertex array & render states.
        /// </summary>
        /// <returns>The tile map.</returns>
        /// <param name="tileatlas">The tileatlas to get tex-coords from use.</param>
        public Tuple<List<Vertex[]>, RenderStates> GetTileMap(TextureAtlas atlas)
        {
            _states.Texture = atlas.Texture;

            for (int i = 0; i < Layers.Count; ++i)
                UpdateVertexLayer(i, atlas);

            return new Tuple<List<Vertex[]>, RenderStates>(_vertex_layers, _states);
        }

        // TODO: create method to rewrite a single tile rather than the whole layer.

        public void UpdateVertexLayer(int layer, TextureAtlas atlas)
        {
            Layer l = Layers[layer];
            int tw = Tileset.TileWidth;
            int th = Tileset.TileHeight;

            int size = l.Width * l.Height * 4;
            Vertex[] lverts = new Vertex[size];
            Vector2f loc1 = new Vector2f(0, 0);
            Vector2f loc2 = new Vector2f(tw, 0);
            Vector2f loc3 = new Vector2f(tw, th);
            Vector2f loc4 = new Vector2f(0, th);
            for (var i = 0; i < lverts.Length; i += 4)
            {
                int tile = l.GetTile((int)loc1.X / tw, (int)loc1.Y / th);
                if (tile >= 0)
                {
                    IntRect source = atlas.Sources[tile];
                    int w = source.Left + source.Width;
                    int h = source.Top + source.Height;
                    lverts[i + 0] = new Vertex(loc1, new Vector2f(source.Left, source.Top));
                    lverts[i + 1] = new Vertex(loc2, new Vector2f(w, source.Top));
                    lverts[i + 2] = new Vertex(loc3, new Vector2f(w, h));
                    lverts[i + 3] = new Vertex(loc4, new Vector2f(source.Left, h));
                }

                loc1.X += tw;
                loc2.X += tw;
                loc3.X += tw;
                loc4.X += tw;
                if (loc1.X == l.Width * tw)
                {
                    loc1.Y += th;
                    loc2.Y += th;
                    loc3.Y += th;
                    loc4.Y += th;
                    loc1.X = loc4.X = 0;
                    loc2.X = loc3.X = tw;
                }
            }

            _vertex_layers[layer] = lverts;
        }

        /// <summary>
        /// Disposes and clears this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (Entity e in Entities) e.Dispose();
                    if (Tileset != null) Tileset.Dispose();
                    Layers.Clear();
                }
                Layers = null;
                Tileset = null;
                Entities = null;
            }
            _disposed = true;
        }

        /// <summary>
        /// Resizes the map's layers to the new size.
        /// </summary>
        /// <param name="width">The new width in tiles.</param>
        /// <param name="height">The new height in tiles.</param>
        public void ResizeAllLayers(short width, short height)
        {
            foreach (Layer lay in Layers) lay.Resize(width, height);
        }
    }
}
