using System;
using SFML.Graphics;
using SFML.Window;

namespace Engine
{
    public static class GlobalPrimitives
    {
        private static RectangleShape _rect = new RectangleShape();
        private static CircleShape _circle = new CircleShape();

        public static RenderWindow window;

        public static void Rectangle(float x, float y, float width, float height, Color color)
        {
            _rect.Position = new Vector2f(x, y);
            _rect.FillColor = color;
            _rect.Size = new Vector2f(width, height);
            window.Draw(_rect);
        }

        public static void GradientRectangle(float x, float y, float width, float height, Color color1, Color color2, Color color3, Color color4)
        {
            Vertex[] v = { new Vertex(new Vector2f(x, y), color1), new Vertex(new Vector2f(x+width, y), color2),
                new Vertex(new Vector2f(x+width, y+height), color3), new Vertex(new Vector2f(x, y+height), color4) };
            window.Draw(v, PrimitiveType.Quads);
        }

        public static void OutlinedRectangle(float x, float y, float width, float height, Color color, float thickness)
        {
            _rect.Position = new Vector2f(x, y);
            _rect.Size = new Vector2f(width, height);
            _rect.FillColor = new Color(0, 0, 0, 0);
            _rect.OutlineColor = color;
            _rect.OutlineThickness = thickness;

            window.Draw(_rect);
        }

        public static void Triangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            Vertex[] v = { new Vertex(new Vector2f(x1, y1), color), new Vertex(new Vector2f(x2, y2), color), new Vertex(new Vector2f(x3, y3), color) };
            window.Draw(v, PrimitiveType.Triangles);
        }

        public static void Line(float x1, float y1, float x2, float y2, Color color)
        {
            Vertex[] v = { new Vertex(new Vector2f(x1, y1), color), new Vertex(new Vector2f(x2, y2), color) };
            window.Draw(v, PrimitiveType.Lines);
        }

        public static void GradientLine(float x1, float y1, float x2, float y2, Color color1, Color color2)
        {
            Vertex[] v = { new Vertex(new Vector2f(x1, y1), color1), new Vertex(new Vector2f(x2, y2), color2) };
            window.Draw(v, PrimitiveType.Lines);
        }

        public static void FilledCircle(float x, float y, float radius, Color color)
        {
            _circle.Radius = radius;
            _circle.FillColor = color;
            _circle.Position = new Vector2f((float)x, (float)y);
            _circle.Origin = new Vector2f((float)radius, (float)radius);
            window.Draw(_circle);
        }
    }
}

