using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Objects
{
    /// <summary>
    /// A Sphere map layer.
    /// </summary>
    public class Layer
    {
        #region attributes
        private TwoArray<short> _tiles;

        /// <summary>
        /// Gets the width of this Layer in tiles.
        /// </summary>
        public short Width { get; private set; }

        /// <summary>
        /// Gets the height of this Layer in tiles.
        /// </summary>
        public short Height { get; private set; }

        /// <summary>
        /// Gets or sets the parallax x value of this Layer.
        /// </summary>
        public float ParallaxX { get; set; }

        /// <summary>
        /// Gets or sets the parallax y value of this Layer.
        /// </summary>
        public float ParallaxY { get; set; }

        /// <summary>
        /// Gets the scroll x value of this Layer. 
        /// </summary>
        public float ScrollX { get; set; }

        /// <summary>
        /// Gets the scroll y value of this Layer.
        /// </summary>
        public float ScrollY { get; set; }

        /// <summary>
        /// Gets r sets the reflectivity of this Layer.
        /// </summary>
        public bool Reflective { get; set; }

        /// <summary>
        /// Gets or sets the visibility of this layer.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets if it is parallaxed.
        /// </summary>
        public bool Parallax { get; set; }
        
        /// <summary>
        /// Gets the flags used by this Layer.
        /// </summary>
        public short Flags { get; set; }

        /// <summary>
        /// Gets the name of this Layer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the list of obstruction segments used by this layer.
        /// </summary>
        public List<Segment> Segments { get; private set; }
        #endregion

        /// <summary>
        /// Constructs a new Sphere layer.
        /// </summary>
        public Layer()
        {
            Segments = new List<Segment>();
            Visible = true;
        }

        /// <summary>
        /// Writes data to the binary writer.
        /// </summary>
        /// <param name="writer">BinaryWriter to use.</param>
        public void Save(BinaryWriter writer)
        {
            // save header:
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Flags);
            writer.Write(ParallaxX);
            writer.Write(ParallaxY);
            writer.Write(ScrollX);
            writer.Write(ScrollY);
            writer.Write(Segments.Count);
            writer.Write(Reflective);
            writer.Write(new byte[3]);
            writer.Write((short)Name.Length);
            writer.Write(Name.ToCharArray());

            // save tiles:
            for (int i = 0, size = Width * Height; i < size; ++i)
                writer.Write(_tiles.Array[i]);

            // save segments:
            foreach (Segment segment in Segments)
                segment.Save(writer);
        }

        /// <summary>
        /// Creates a proper Sphere from a data stream.
        /// </summary>
        /// <param name="reader">BinaryReader to use.</param>
        /// <returns>Sphere Layer object.</returns>
        public static Layer FromBinary(BinaryReader reader)
        {
            Layer layer = new Layer
                {
                    Width = reader.ReadInt16(),
                    Height = reader.ReadInt16(),
                    Flags = reader.ReadInt16()
                };

            layer.Visible = (~layer.Flags & 1) == 1;
            layer.Parallax = (layer.Flags & 2) == 2;
            layer.ParallaxX = reader.ReadSingle();
            layer.ParallaxY = reader.ReadSingle();
            layer.ScrollX = reader.ReadSingle();
            layer.ScrollY = reader.ReadSingle();
            int segs = reader.ReadInt32();
            layer.Reflective = reader.ReadBoolean();
            reader.ReadBytes(3); // reserved

            short length = reader.ReadInt16();
            layer.Name = new string(reader.ReadChars(length));

            layer._tiles = new TwoArray<short>(layer.Width, layer.Height);
            for (int i = 0, size = layer.Width*layer.Height; i < size; ++i)
                layer._tiles.Array[i] = reader.ReadInt16();

            for (int i = 0; i < segs; ++i) layer.Segments.Add(new Segment(reader));
            return layer;
        }

        /// <summary>
        /// Creates a blank layer.
        /// </summary>
        /// <param name="width">Width in tiles to use.</param>
        /// <param name="height">Height in tiles to use.</param>
        public void CreateNew(short width, short height)
        {
            Width = width;
            Height = height;
            Name = "Untitled";

            _tiles = new TwoArray<short>(width, height);
        }

        /// <summary>
        /// Sets a tile, if applicable.
        /// </summary>
        /// <param name="x">X-coord to use.</param>
        /// <param name="y">Y-coord to use.</param>
        /// <param name="index">Index to set to.</param>
        /// <returns>True if the tile had been set.</returns>
        public bool SetTile(int x, int y, short index)
        {
            return _tiles.TrySet(x, y, index);
        }

        /// <summary>
        /// Checks to see if the layer has correct tile indicies.
        /// Will set afflicted tiles to 0, if check fails.
        /// </summary>
        /// <param name="max">The maximum tile index expected.</param>
        /// <returns>False if there are invalid tiles.</returns>
        public bool Validate(int max)
        {
            bool retVal = true;
            for (int i = 0, size = Width*Height; i < size; ++i)
            {
                if (_tiles.Array[i] < max) continue;
                _tiles.Array[i] = 0;
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Pump in a 2D array to replace current tiles with.
        /// </summary>
        /// <param name="tiles">New array of tile indicies to use.</param>
        public void SetTiles(short[] tiles)
        {
            _tiles.Set(tiles);
        }

        /// <summary>
        /// Creates a hard copy of the data.
        /// </summary>
        /// <returns>A clone of the layers tiles.</returns>
        public short[] CloneTiles()
        {
            short[] array = new short[Width * Height];
            Array.Copy(_tiles.Array, array, array.Length);
            return array;
        }

        /// <summary>
        /// Adjusts the tile indicies for when tiles were removed or added.
        /// </summary>
        /// <param name="startindex">Index to start at.</param>
        /// <param name="delta">How much of a shift to make.</param>
        public void AdjustTiles(short startindex, short delta)
        {
            for (int i = 0, size = Width*Height; i < size; ++i)
            {
                short v = _tiles.Array[i];
                if (v > startindex)
                {
                    v += delta;
                    _tiles.Array[i] = Math.Max((short)0, v);
                }
            }
        }

        /// <summary>
        /// Gets tile index at the (x, y) position.
        /// </summary>
        /// <param name="x">X-coord of tile.</param>
        /// <param name="y">Y-coord of tile.</param>
        /// <returns>The zero based tile index.</returns>
        public short GetTile(int x, int y)
        {
            short v = -1;
            _tiles.Get(x, y, ref v);
            return v;
        }

        /// <summary>
        /// Resizes the field to the new size.
        /// </summary>
        /// <param name="width">New width of the field.</param>
        /// <param name="height">New height of the field.</param>
        public void Resize(short width, short height)
        {
            _tiles.Resize(width, height);
        }
    }
}
