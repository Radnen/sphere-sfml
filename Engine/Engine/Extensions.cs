using System;
using SFML.Graphics;
using Jurassic.Library;

namespace Engine
{
    public static class Extensions
    {
        public static void Clear(this Image img, Color color)
        {
            uint w = img.Size.X;
            uint h = img.Size.Y;

            for (uint x = 0; x < w; ++x)
            {
                for (uint y = 0; y < h; ++y)
                {
                    img.SetPixel(x, y, color);
                }
            }
        }

        public static void ReplaceColor(this Image img, Color A, Color B)
        {
            uint w = img.Size.X;
            uint h = img.Size.Y;

            for (uint y = 0; y < h; ++y)
            {
                for (uint x = 0; x < w; ++x)
                {
                    if (img.GetPixel(x, y).Equals(A))
                        img.SetPixel(x, y, B);
                }
            }
        }

        public static int ToInt(this Color c)
        {
            return (c.A << 24) + (c.B << 16) + (c.G << 8) + c.R;
        }
    }
}

