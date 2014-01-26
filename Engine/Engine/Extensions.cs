using System;
using SFML.Graphics;
using Jurassic.Library;

namespace Engine
{
    public static class Conversions
    {
        public static int ToColorInt(ObjectInstance instance)
        {
            int r = (int)instance["red"];
            int g = (int)instance["green"];
            int b = (int)instance["blue"];
            int a = (int)instance["alpha"];

            return (a << 24) | (b << 16) | (g << 8) | r;
        }

        public static Color ToColor(ObjectInstance instance)
        {
            byte r = Convert.ToByte(instance["red"]);
            byte g = Convert.ToByte(instance["green"]);
            byte b = Convert.ToByte(instance["blue"]);
            byte a = Convert.ToByte(instance["alpha"]);

            return new Color(r, g, b, a);
        }

        public static ObjectInstance ToColorObject(Color color)
        {
            ObjectInstance obj = Program.CreateObject();
            obj["red"] = (int)color.R;
            obj["green"] = (int)color.G;
            obj["blue"] = (int)color.B;
            obj["alpha"] = (int)color.A;

            return obj;
        }
    }

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

