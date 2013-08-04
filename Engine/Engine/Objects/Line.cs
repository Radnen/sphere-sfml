using System;
using SFML.Window;

namespace Engine.Objects
{
    public class Line
    {
        public Line(short x1, short y1, short x2, short y2)
        {
            Start = new Vector2f(x1, y1);
            End = new Vector2f(x2, y2);
        }

        public Line(Vector2f start, Vector2f end)
        {
            Start = start;
            End = end;
        }

        public Vector2f Start { get; set; }
        public Vector2f End { get; set; }

        public static bool Intersects(Line A, Line B)
        {
            float q = (A.Start.Y - B.Start.Y) * (B.End.X - B.Start.X) -
                (A.Start.X - B.Start.X) * (B.End.Y - B.Start.Y);
            float d = (A.End.X - A.Start.X) * (B.End.Y - B.Start.Y) -
                (A.End.Y - A.Start.Y) * (B.End.X - B.Start.X);

            if (d == 0)
                return false;

            float r = q / d;
            q = (A.Start.Y - B.Start.Y) * (A.End.X - A.Start.X) -
                (A.Start.X - B.Start.X) * (A.End.Y - A.Start.Y);
            float s = q / d;

            return !(r < 0 || r > 1 || s < 0 || s > 1);
        }

        public void Draw(ColorInstance color)
        {
            GlobalPrimitives.Line(Start.X, Start.Y, End.X, End.Y, color);
        }
    }
}

