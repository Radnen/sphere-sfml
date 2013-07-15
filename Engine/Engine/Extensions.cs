using System;
using SFML.Graphics;

namespace Engine
{
    public static class Extensions
    {
        public static void Clear(this Image img, Color color)
        {
            uint w = img.Size.X;
            uint h = img.Size.Y;

            for (uint x = 0; x < w; ++x) {
                for (uint y = 0; y < h; ++y) {
                    img.SetPixel(x, y, color);
                }
            }
        }
    }
}

