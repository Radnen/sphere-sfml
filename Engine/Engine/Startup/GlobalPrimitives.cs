using System;
using SFML.Graphics;
using SFML.Window;

namespace Engine
{
    public static class GlobalPrimitives
    {
        public static RectangleShape _rect = new RectangleShape();
        public static CircleShape _circle = new CircleShape();

        public static void Rectangle(RenderWindow window, float x, float y, float width, float height, Color color)
        {
            _rect.Position = new Vector2f(x, y);
            _rect.FillColor = color;
            _rect.Size = new Vector2f(width, height);
            window.Draw(_rect);
        }

        public static void OutlinedRectangle(RenderWindow window, float x, float y, float width, float height, Color color)
        {
            _rect.Position = new Vector2f(x, y);
            _rect.Size = new Vector2f(width, height);
            _rect.OutlineColor = color;
            window.Draw(_rect);
        }

        public static void Triangle(RenderWindow window, float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            // TODO: implement in GL
        }

        public static void FilledCircle(RenderWindow window, float x, float y, float radius, Color color)
        {
            _circle.Radius = radius;
            _circle.FillColor = color;
            _circle.Position = new Vector2f((float)x, (float)y);
            _circle.Origin = new Vector2f((float)radius, (float)radius);
            window.Draw(_circle);
        }
    }
}

