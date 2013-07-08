using System;
using SFML.Graphics;
using SFML.Window;
using Jurassic.Library;
using Engine.Objects;

namespace Engine
{
    public static class GlobalPrimitives
    {
        private static RectangleShape _rect = new RectangleShape();
        private static RectangleShape _orect = new RectangleShape();
        private static CircleShape _circle = new CircleShape();

        public static RenderWindow window;

        public static void Rectangle(double x, double y, double width, double height, ColorInstance color)
        {
            _rect.Position = new Vector2f((float)x, (float)y);
            _rect.FillColor = color.GetColor();
            _rect.Texture = null;
            _rect.Size = new Vector2f((float)width, (float)height);
            window.Draw(_rect);
        }

        public static void GradientRectangle(double x, double y, double width, double height, ColorInstance color1, ColorInstance color2, ColorInstance color3, ColorInstance color4)
        {
            Vertex[] v = { new Vertex(new Vector2f((float)x, (float)y), color1.GetColor()),
                new Vertex(new Vector2f((float)(x + width), (float)y), color2.GetColor()),
                new Vertex(new Vector2f((float)(x + width), (float)(y + height)), color3.GetColor()),
                new Vertex(new Vector2f((float)x, (float)(y + height)), color4.GetColor()) };
            window.Draw(v, PrimitiveType.Quads);
        }

        public static void OutlinedRectangle(double x, double y, double width, double height, ColorInstance color, double thickness = 1.0f)
        {
            _orect.Position = new Vector2f((float)x, (float)y);
            _orect.Size = new Vector2f((float)width, (float)height);
            _orect.FillColor = new Color(0, 0, 0, 0);
            _orect.OutlineColor = color.GetColor();
            _orect.OutlineThickness = (float)thickness;

            window.Draw(_orect);
        }

        public static void Triangle(double x1, double y1, double x2, double y2, double x3, double y3, ColorInstance color)
        {
            Color sfml_color = color.GetColor();
            Vertex[] v = { new Vertex(new Vector2f((float)x1, (float)y1), sfml_color),
                new Vertex(new Vector2f((float)x2, (float)y2), sfml_color),
                new Vertex(new Vector2f((float)x3, (float)y3), sfml_color) };
            window.Draw(v, PrimitiveType.Triangles);
        }

        public static void Point(double x, double y, ColorInstance color)
        {
            Vertex[] v = { new Vertex(new Vector2f((float)x, (float)y), color.GetColor()) };
            window.Draw(v, PrimitiveType.Points);
        }

        public static void Line(double x1, double y1, double x2, double y2, ColorInstance color)
        {
            Color sfml_color = color.GetColor();
            Vertex[] v = { new Vertex(new Vector2f((float)x1, (float)y1), sfml_color),
                new Vertex(new Vector2f((float)x2, (float)y2), sfml_color) };
            window.Draw(v, PrimitiveType.Lines);
        }

        public static void LineSeries(ArrayInstance points, ColorInstance color)
        {
            Color sfml_col = color.GetColor();

            Vertex[] v = new Vertex[points.Length];
            for (var i = 0; i < points.Length; ++i)
            {
                ObjectInstance obj = points[i.ToString()] as ObjectInstance;
                if (obj != null)
                {
                    Vector2f vect = new Vector2f();
                    vect.X = (int)obj.GetPropertyValue("x");
                    vect.Y = (int)obj.GetPropertyValue("y");
                    v[i] = new Vertex(vect, sfml_col);
                }
            }

            window.Draw(v, PrimitiveType.Lines);
        }

        public static void PointSeries(ArrayInstance points, ColorInstance color)
        {
            Color sfml_col = color.GetColor();

            Vertex[] v = new Vertex[points.Length];
            for (var i = 0; i < points.Length; ++i)
            {
                ObjectInstance obj = points[i.ToString()] as ObjectInstance;
                if (obj != null)
                {
                    Vector2f vect = new Vector2f();
                    vect.X = ((int)obj.GetPropertyValue("x")-1);
                    vect.Y = ((int)obj.GetPropertyValue("y")+1);
                    v[i] = new Vertex(vect, sfml_col);
                }
            }

            window.Draw(v, PrimitiveType.Points);
        }

        public static void GradientLine(double x1, double y1, double x2, double y2, ColorInstance color1, ColorInstance color2)
        {
            Vertex[] v = { new Vertex(new Vector2f((float)x1, (float)y1), color1.GetColor()),
                new Vertex(new Vector2f((float)x2, (float)y2), color2.GetColor()) };
            window.Draw(v, PrimitiveType.Lines);
        }

        public static void FilledCircle(double x, double y, double radius, ColorInstance color)
        {
            _circle.Radius = (float)radius;
            _circle.FillColor = color.GetColor();
            _circle.Position = new Vector2f((float)x, (float)y);
            _circle.Origin = new Vector2f((float)radius, (float)radius);
            window.Draw(_circle);
        }
    }
}

