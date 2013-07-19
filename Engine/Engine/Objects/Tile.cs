using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Engine.Objects
{
    /// <summary>
    /// A Sphere Tile object.
    /// </summary>
    public class Tile
    {
        private Image _graphic;

        /// <summary>
        /// Creates a tile object, from an existing image.
        /// </summary>
        /// <param name="image">System.Drawing.Image object to use.</param>
        public Tile(Image image)
        {
            Obstructions = new List<Line>();
            Name = "";
            Width = (int)image.Size.X;
            Height = (int)image.Size.Y;
            _graphic = new Image(image);
        }

        /// <summary>
        /// Create's a new tile object.
        /// </summary>
        /// <param name="width">The width of the tile.</param>
        /// <param name="height">The height of the tile.</param>
        public Tile(int width, int height)
        {
            Obstructions = new List<Line>();
            Name = "";
            Width = width;
            Height = height;
            _graphic = new Image((uint)width, (uint)height, Color.Black);
        }

        /// <summary>
        /// Gets or sets the width of the tile in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the tile in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the tile delay.
        /// </summary>
        public short Delay { get; set; }

        /// <summary>
        /// Gets or sets whether or not it is animated.
        /// </summary>
        public bool Animated { get; set; }

        /// <summary>
        /// Gets or sets whether or not it is blocked.
        /// </summary>
        public byte Blocked { get; set; }

        /// <summary>
        /// Gets or sets the next animated tile index.
        /// </summary>
        public short NextAnim { get; set; }

        /// <summary>
        /// Gets the list of obstructions tied to this Tile.
        /// </summary>
        public List<Line> Obstructions { get; private set; }

        /// <summary>
        /// Gets or sets the graphic used by this Tile.
        /// </summary>
        public Image Graphic
        {
            get { return _graphic; }
            set { if (_graphic != null) { _graphic.Dispose(); } _graphic = value; }
        }

        /// <summary>
        /// Gets or sets the name of this Tile.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Creates a perfect clone of this Tile.
        /// </summary>
        /// <returns>A copy of the Tile object.</returns>
        public Tile Clone()
        {
            Tile newTile = new Tile(_graphic) { Name = Name };
            return newTile;
        }
    }
}
