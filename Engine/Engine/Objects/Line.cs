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

        public void Draw(ColorInstance color)
        {
            GlobalPrimitives.Line(Start.X, Start.Y, End.X, End.Y, color);
        }
    }
}

