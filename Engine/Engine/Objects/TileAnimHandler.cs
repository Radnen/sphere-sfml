using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Engine.Objects
{
    /// <summary>
    /// Provides an easy method to animate tiles on a map.
    /// </summary>
    public class TileAnimHandler
    {
        List<IntRect> _sources;
        List<int[]> _tiles;
        FastTextureAtlas _atlas;
        int _frame = 0;
        int _current = 0;

        public TileAnimHandler(FastTextureAtlas atlas)
        {
            _atlas = atlas;
            _sources = new List<IntRect>();
            _tiles = new List<int[]>();
        }

        public void AddTile(int index, int frames)
        {
            _tiles.Add(new int[2] { index, frames });
            _sources.Add(_atlas.TextureAtlas.Sources[index]);
        }

        public void Animate()
        {
            _frame++;
            if (_frame == _tiles[_current][1])
            {
                _current = (_current + 1) % _tiles.Count;
                _frame = 0;
                _atlas.SetImageAt((uint)_tiles[0][0], (uint)_tiles[_current][0]);
            }
        }

        public void Reset()
        {
            _atlas.SetImageAt((uint)_tiles[0][0], (uint)_tiles[0][0]);
        }
    }
}

