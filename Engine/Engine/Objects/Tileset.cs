using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    /// <summary>
    /// A sphree Tileset object.
    /// </summary>
    public class Tileset : IDisposable
    {
        /// <summary>
        /// Gets a list of Tile objects this Tileset uses.
        /// </summary>
        public List<Tile> Tiles { get; private set; }

        /// <summary>
        /// Gets or sets the width of a tile in pixels.
        /// </summary>
        public short TileWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of a tile in pixels.
        /// </summary>
        public short TileHeight { get; set; }

        private short _version = 1;
        private byte _hasObstruct, _compression;

        /// <summary>
        /// Gets if this has disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Creates a fresh, empty tileset.
        /// </summary>
        public Tileset()
        {
            IsDisposed = false;
            Tiles = new List<Tile>();
        }

        /// <summary>
        /// Adds a blank tile to the list of tiles.
        /// </summary>
        /// <param name="tileWidth">The tile width in pixels.</param>
        /// <param name="tileHeight">The tile height in pixels.</param>
        public void CreateNew(short tileWidth, short tileHeight)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Tiles.Add(new Tile(TileWidth, TileHeight));
        }

        /// <summary>
        /// Loads a tileset independantly from a file.
        /// </summary>
        /// <param name="filename">The filename to load from.</param>
        /// <returns>A tileset object from the file or null if it doesn't exist.</returns>
        public static Tileset FromFile(string filename)
        {
            if (!File.Exists(filename)) return null;
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                return FromBinary(reader);
            }
        }

        /// <summary>
        /// Loads a tileset from a filestream.
        /// </summary>
        /// <param name="reader">The System.IO.BinrayReader to use.</param>
        /// <returns>A tileset object.</returns>
        public static Tileset FromBinary(BinaryReader reader)
        {
            Tileset ts = new Tileset();
            reader.ReadChars(4); // sign
            ts._version = reader.ReadInt16();  // version
            short numTiles = reader.ReadInt16();
            ts.TileWidth = reader.ReadInt16();
            ts.TileHeight = reader.ReadInt16();
            reader.ReadInt16(); // tile_bpp
            ts._compression = reader.ReadByte();
            ts._hasObstruct = reader.ReadByte();
            reader.ReadBytes(240);

            int bitSize = ts.TileWidth * ts.TileHeight * 4;
            while (numTiles-- > 0)
            {
                Tile newTile = new Tile(ts.TileWidth, ts.TileHeight);
                newTile.Graphic = new Image((uint)ts.TileWidth, (uint)ts.TileHeight, reader.ReadBytes(bitSize));
                ts.Tiles.Add(newTile);
            }

            foreach (Tile t in ts.Tiles)
            {
                reader.ReadByte();
                t.Animated = reader.ReadBoolean();
                t.NextAnim = reader.ReadInt16();
                t.Delay = reader.ReadInt16();
                reader.ReadByte();
                t.Blocked = reader.ReadByte();
                int segs = reader.ReadInt16();
                int amt = reader.ReadInt16();
                reader.ReadBytes(20);
                t.Name = new string(reader.ReadChars(amt));
                while (segs-- > 0)
                {
                    Line l = new Line(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
                    t.Obstructions.Add(l);
                }
            }

            return ts;
        }

        /// <summary>
        /// Savbes the tileset to a separate file.
        /// </summary>
        /// <param name="filename">The filename to save to.</param>
        public void Save(string filename)
        {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filename)))
            {
                // save header data:
                writer.Write(".rts".ToCharArray());
                writer.Write(_version);
                writer.Write((short)Tiles.Count);
                writer.Write(TileWidth);
                writer.Write(TileHeight);
                writer.Write((short)32);
                writer.Write(_compression);

                foreach (Tile t in Tiles)
                    if (t.Obstructions.Count > 0) _hasObstruct = 1;

                writer.Write(_hasObstruct);
                writer.Write(new byte[240]);

                // save tile pixels:
                foreach (Tile tile in Tiles)
                    writer.Write(tile.Graphic.Pixels);

                // save tile info:
                foreach (Tile t in Tiles)
                {
                    writer.Write(new byte());
                    writer.Write(t.Animated);
                    writer.Write(t.NextAnim);
                    writer.Write(t.Delay);
                    writer.Write(new byte());
                    writer.Write((byte)2);
                    writer.Write((short)t.Obstructions.Count);
                    writer.Write((short)t.Name.Length);
                    writer.Write(new byte[20]);
                    writer.Write(t.Name.ToCharArray());
                    foreach (Line l in t.Obstructions)
                    {
                        writer.Write((short)l.Start.X); writer.Write((short)l.Start.Y);
                        writer.Write((short)l.End.X); writer.Write((short)l.End.Y);
                    }
                }

                writer.Flush();
            }
        }

        /// <summary>
        /// Disposes and clears this Tileset.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    foreach (Tile tile in Tiles)
                    {
                        if (tile.Graphic != null) tile.Graphic.Dispose();
                        tile.Graphic = null;
                    }
                }
                Tiles = null;
            }
            IsDisposed = true;
        }
    }
}
