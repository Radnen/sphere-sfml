using System;
using Engine.Objects;
using Jurassic.Library;
using SFML.Graphics;
using SFML.System;

namespace Engine
{
    public static class GlobalPrimitives
    {
        private static RectangleShape _rect = new RectangleShape();
        private static RectangleShape _orect = new RectangleShape();
        private static CircleShape _circle = new CircleShape();
        private static CircleShape _ocircle = new CircleShape();
        private static Vertex[] _verts = new Vertex[4];

        public static RenderTarget Target;

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
            return new Vector2f(Convert.ToSingle(obj["x"]), Convert.ToSingle(obj["y"]));
        }

        public static void Rectangle(double x, double y, double width, double height, ColorInstance color)
        {
            Color sfml_color = color.Color;
            _verts[0] = new Vertex(new Vector2f((float)x, (float)y), sfml_color);
            _verts[1] = new Vertex(new Vector2f((float)(x + width), (float)y), sfml_color);
            _verts[2] = new Vertex(new Vector2f((float)(x + width), (float)(y + height)), sfml_color);
            _verts[3] = new Vertex(new Vector2f((float)x, (float)(y + height)), sfml_color);
            Program.Batch.AddVerts(_verts, 4);
        }

        public static void ApplyColorMask(ColorInstance color)
        {
            Rectangle(0, 0, GlobalProps.Width, GlobalProps.Height, color);
        }

        public static void GradientRectangle(double x, double y, double width, double height, ColorInstance c1, ColorInstance c2, ColorInstance c3, ColorInstance c4)
        {
            _verts[0] = new Vertex(new Vector2f((float)x, (float)y), c1.Color);
            _verts[1] = new Vertex(new Vector2f((float)(x + width), (float)y), c2.Color);
            _verts[2] = new Vertex(new Vector2f((float)(x + width), (float)(y + height)), c3.Color);
            _verts[3] = new Vertex(new Vector2f((float)x, (float)(y + height)), c4.Color);
            Program.Batch.AddVerts(_verts, 4);
        }

        public static void OutlinedRectangle(double x, double y, double width, double height, ColorInstance color, [DefaultParameterValue(1.0)] double thickness = 1.0)
        {
            Program.Batch.Flush();
            _orect.Position = new Vector2f((float)x + 1, (float)y + 1);
            _orect.OutlineColor = color.Color;
            _orect.Size = new Vector2f((float)width - 2, (float)height - 2);
            _orect.OutlineThickness = (float)thickness;
            Target.Draw(_orect);
        }

        // is this even being used?
        public static void OutlinedRectangles(ArrayInstance items, ColorInstance color)
        {
            uint size = items.Length * 4;
            Color c = color.GetColor();
            Vertex[] verts = new Vertex[size];
            for (var i = 0; i < size; i += 4)
            {
                ObjectInstance rect = items[i / 4] as ObjectInstance;
                int x1 = (int)rect["x"];
                int y1 = (int)rect["y"];
                int x2 = x1 + (int)rect["w"];
                int y2 = y1 + (int)rect["h"];
                verts[i + 0] = new Vertex(new Vector2f(x1, y1), c);
                verts[i + 1] = new Vertex(new Vector2f(x2, y1), c);
                verts[i + 2] = new Vertex(new Vector2f(x2, y2), c);
                verts[i + 3] = new Vertex(new Vector2f(x1, y2), c);
            }
            Program.Batch.AddVerts(verts, verts.Length, PrimitiveType.LinesStrip);
        }

        public static void Triangle(double x1, double y1, double x2, double y2, double x3, double y3, ColorInstance color)
        {
            Color c = color.Color;
            _verts[0] = new Vertex(new Vector2f((float)x1, (float)y1), c);
            _verts[1] = new Vertex(new Vector2f((float)x2, (float)y2), c);
            _verts[2] = new Vertex(new Vector2f((float)x3, (float)y3), c);
            Program.Batch.AddVerts(_verts, 3, PrimitiveType.Triangles);
        }

        public static void GradientTriangle(ObjectInstance A, ObjectInstance B, ObjectInstance C, ColorInstance color1, ColorInstance color2, ColorInstance color3)
        {
            _verts[0] = new Vertex(GetVector(A), color1.GetColor());
            _verts[1] = new Vertex(GetVector(B), color2.GetColor());
            _verts[2] = new Vertex(GetVector(C), color3.GetColor());
            Program.Batch.AddVerts(_verts, 3, PrimitiveType.Triangles);
        }

        public static void Polygon(ArrayInstance points, ColorInstance color, [DefaultParameterValue(false)] bool inverse = false)
        {
            if (points == null || color == null)
                throw new NullReferenceException();

            Color c = color.GetColor();

            Vertex[] v = new Vertex[points.Length];
            for (var i = 0; i < points.Length; ++i)
            {
                ObjectInstance obj = points[i] as ObjectInstance;
                v[i] = new Vertex(GetVector(obj), c);
            }

            Program.Batch.AddVerts(v, v.Length, PrimitiveType.TrianglesFan);
        }

        public static void Point(double x, double y, ColorInstance color)
        {
            _verts[0] = new Vertex(new Vector2f((float)x, (float)y), color.GetColor());
            Program.Batch.AddVerts(_verts, 1, PrimitiveType.Points);
        }

        public static void Line(double x1, double y1, double x2, double y2, ColorInstance color)
        {
            Color c = color.Color;
            _verts[0] = new Vertex(new Vector2f((float)x1, (float)y1), c);
            _verts[1] = new Vertex(new Vector2f((float)x2, (float)y2), c);
            Program.Batch.AddVerts(_verts, 2, PrimitiveType.Lines);
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

            Program.Batch.AddVerts(v, v.Length, PrimitiveType.Lines);
        }

        public static void PointSeries(ArrayInstance points, ColorInstance color)
        {
            Program.Batch.Flush();
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

            Program.Batch.AddVerts(v, v.Length, PrimitiveType.Points);
        }

        public static void GradientLine(double x1, double y1, double x2, double y2, ColorInstance color1, ColorInstance color2)
        {
            _verts[0] = new Vertex(new Vector2f((float)x1, (float)y1), color1.GetColor());
            _verts[1] = new Vertex(new Vector2f((float)x2, (float)y2), color2.GetColor());
            Program.Batch.AddVerts(_verts, 2, PrimitiveType.Lines);
        }

        public static void FilledCircle(double x, double y, double radius, ColorInstance color)
        {
            Program.Batch.Flush();
            _circle.Radius = (float)radius;
            _circle.FillColor = color.Color;
            _circle.Position = new Vector2f((float)x, (float)y);
            _circle.Origin = new Vector2f((float)radius, (float)radius);
            Target.Draw(_circle);
        }

        public static void GradientCircle(double x, double y, double radius, ColorInstance inner, ColorInstance outer, [DefaultParameterValue(false)] bool antialias = false)
        {
            Program.Batch.Flush();
            _circle.Radius = (float)radius;
            _circle.FillColor = inner.Color;
            _circle.Position = new Vector2f((float)x, (float)y);
            _circle.Origin = new Vector2f((float)radius, (float)radius);
            Target.Draw(_circle);
        }

        public static void OutlinedCircle(double x, double y, double radius, ColorInstance color, [DefaultParameterValue(1.0)] double size = 1.0)
        {
            Program.Batch.Flush();
            _ocircle.Radius = (float)radius;
            _ocircle.OutlineColor = color.Color;
            _ocircle.OutlineThickness = (float)size;
            _ocircle.Position = new Vector2f((float)x, (float)y);
            _ocircle.Origin = new Vector2f((float)radius, (float)radius);
            Target.Draw(_ocircle);
        }
    }
}
