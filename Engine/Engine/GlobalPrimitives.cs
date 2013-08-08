using System;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;
using Engine.Objects;

namespace Engine
{
    public static class GlobalPrimitives
    {
        private static RectangleShape _rect = new RectangleShape();
        private static RectangleShape _orect = new RectangleShape();
        private static CircleShape _circle = new CircleShape();
        private static CircleShape _ocircle = new CircleShape();
        private static Vertex[] _verts = new Vertex[4];

        public static RenderWindow window;

        static GlobalPrimitives()
        {
            _orect.FillColor = new Color(0, 0, 0, 0);
            _ocircle.FillColor = new Color(0, 0, 0, 0);
        }

        /// <summary>
        /// Attempts to translate a JS object: {x: 0, y: 0} into a Vector2f point.
        /// </summary>
        public static Vector2f GetVector(ObjectInstance obj)
        {
            Vector2f vect = new Vector2f(0, 0);
            vect.X = Convert.ToSingle(obj["x"]);
            vect.Y = Convert.ToSingle(obj["y"]);
            return vect;
        }

        public static void Rectangle(double x, double y, double width, double height, ColorInstance color)
        {
            _rect.Position = new Vector2f((float)x, (float)y);
            _rect.FillColor = color.GetColor();
            _rect.Texture = null;
            _rect.Size = new Vector2f((float)width, (float)height);
            window.Draw(_rect);
        }

        public static void ApplyColorMask(ColorInstance color)
        {
            Rectangle(0, 0, GlobalProps.Width, GlobalProps.Height, color);
        }

        public static void GradientRectangle(double x, double y, double width, double height, ColorInstance color1, ColorInstance color2, ColorInstance color3, ColorInstance color4)
        {
            _verts[0].Position = new Vector2f((float)x, (float)y);
            _verts[0].Color = color1.GetColor();
            _verts[1].Position = new Vector2f((float)(x + width), (float)y);
            _verts[1].Color = color2.GetColor();
            _verts[2].Position = new Vector2f((float)(x + width), (float)(y + height));
            _verts[2].Color = color3.GetColor();
            _verts[3].Position = new Vector2f((float)x, (float)(y + height));
            _verts[3].Color = color4.GetColor();
            window.Draw(_verts, PrimitiveType.Quads);
        }

        public static void OutlinedRectangle(double x, double y, double width, double height, ColorInstance color, [DefaultParameterValue(1.0)] double thickness = 1.0)
        {
            Color c = color.GetColor();
            _orect.Position = new Vector2f((float)x+1, (float)y+1);
            _orect.OutlineColor = c;
            _orect.Size = new Vector2f((float)width-2, (float)height-2);
            _orect.OutlineThickness = (float)thickness;
            window.Draw(_orect);
        }

        public static void OutlinedRectangles(ArrayInstance items, ColorInstance color)
        {
            Color c = color.GetColor();
            Vertex[] verts = new Vertex[items.Length * 4];
            for (var i = 0; i < items.Length; ++i)
            {
                ObjectInstance rect = items[i] as ObjectInstance;
                int x1 = (int)rect["x"];
                int y1 = (int)rect["y"];
                int x2 = x1 + (int)rect["w"];
                int y2 = y1 + (int)rect["h"];
                int index = i * 4;
                verts[i] = new Vertex(new Vector2f(x1, y1), c);
                verts[i + 1] = new Vertex(new Vector2f(x2, y1), c);
                verts[i + 2] = new Vertex(new Vector2f(x2, y2), c);
                verts[i + 3] = new Vertex(new Vector2f(x1, y2), c);
            }
        }

        public static void Triangle(double x1, double y1, double x2, double y2, double x3, double y3, ColorInstance color)
        {
            Color sfml_color = color.GetColor();
            Vertex[] v = { new Vertex(new Vector2f((float)x1, (float)y1), sfml_color),
                new Vertex(new Vector2f((float)x2, (float)y2), sfml_color),
                new Vertex(new Vector2f((float)x3, (float)y3), sfml_color) };
            window.Draw(v, PrimitiveType.Triangles);
        }

        public static void GradientTriangle(ObjectInstance A, ObjectInstance B, ObjectInstance C, ColorInstance color1, ColorInstance color2, ColorInstance color3)
        {
            Vertex[] v = { new Vertex(GetVector(A), color1.GetColor()),
                           new Vertex(GetVector(B), color2.GetColor()),
                           new Vertex(GetVector(C), color3.GetColor()) };
            window.Draw(v, PrimitiveType.Triangles);
        }

        public static void Polygon(ArrayInstance points, ColorInstance color, [DefaultParameterValue(false)] bool inverse = false)
        {
            if (points == null || color == null)
                throw new NullReferenceException();

            Color sfml_color = color.GetColor();

            Vertex[] v = new Vertex[points.Length];
            for (var i = 0; i < points.Length; ++i)
            {
                ObjectInstance obj = points[i] as ObjectInstance;
                if (obj == null)
                    throw new NullReferenceException();
                v[i] = new Vertex(GetVector(obj), sfml_color);
            }

            window.Draw(v, PrimitiveType.TrianglesFan);
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
            if (points == null || color == null)
                throw new NullReferenceException();

            Color sfml_col = color.GetColor();

            Vertex[] v = new Vertex[points.Length];
            for (var i = 0; i < points.Length; ++i)
            {
                ObjectInstance obj = points[i] as ObjectInstance;
                if (obj != null)
                    v[i] = new Vertex(GetVector(obj), sfml_col);
                else
                    throw new Jurassic.JavaScriptException(Program._engine, "Invalid object", "Not a JS object in array.");
            }

            window.Draw(v, PrimitiveType.Lines);
        }

        public static void PointSeries(ArrayInstance points, ColorInstance color)
        {
            if (points == null || color == null)
                throw new NullReferenceException();

            Color sfml_col = color.GetColor();

            Vertex[] v = new Vertex[points.Length];
            for (var i = 0; i < points.Length; ++i)
            {
                ObjectInstance obj = points[i] as ObjectInstance;
                if (obj != null)
                    v[i] = new Vertex(GetVector(obj), sfml_col);
                else
                    throw new NullReferenceException();
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

        public static void OutlinedCircle(double x, double y, double radius, ColorInstance color)
        {
            _circle.Radius = (float)radius;
            _circle.OutlineColor = color.GetColor();
            _circle.Position = new Vector2f((float)x, (float)y);
            _circle.Origin = new Vector2f((float)radius, (float)radius);
            window.Draw(_circle);
        }
    }
}

