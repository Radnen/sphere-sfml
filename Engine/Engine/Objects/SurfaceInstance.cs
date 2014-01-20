using System;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class SurfaceInstance : ObjectInstance
    {
        private Texture _tex;
        private bool _changed;
        private Sprite _sprite;
        private BlendModes _mode;
        private byte[] _bytes;
        private uint _width, _height;
        private Action<int, int, Color> _draw;

        public SurfaceInstance(ScriptEngine parent, int width, int height, Color bg_color)
            : base(parent.Object.InstancePrototype)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Width must be > 0.");
            _width = (uint)width;

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Height must be > 0.");
            _height = (uint)height;

            _bytes = new byte[width * height * 4];
            for (int i = 0; i < _bytes.Length; i += 4)
            {
                _bytes[i + 0] = bg_color.R;
                _bytes[i + 1] = bg_color.G;
                _bytes[i + 2] = bg_color.B;
                _bytes[i + 3] = bg_color.A;
            }

            Init();
        }

        public SurfaceInstance(ScriptEngine parent, string filename)
            : base(parent.Object.InstancePrototype)
        {
            using (Image img = new Image(filename))
            {
                _bytes = img.Pixels;
                _width = img.Size.X;
                _height = img.Size.Y;
            }
            Init();
        }

        public SurfaceInstance(ScriptEngine parent, byte[] contents, uint width, uint height)
            : base(parent.Object.InstancePrototype)
        {
            _bytes = contents;
            _width = width;
            _height = height;
            Init();
        }

        private void Init() {
            PopulateFunctions();
            _tex = new Texture(_width, _height);
            _tex.Update(_bytes);
            _sprite = new Sprite(_tex);

            SetBlendMode((int)BlendModes.Blend);

            DefineProperty("width", new PropertyDescriptor((int)_width, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor((int)_height, PropertyAttributes.Sealed), true);
        }

        public Byte[] GetBytes()
        {
            return _bytes;
        }

        [JSFunction(Name = "blit")]
        public void Blit(double x, double y)
        {
            if (_changed)
                _tex.Update(_bytes);

            _sprite.Position = new Vector2f((float)x, (float)y);
            Program._window.Draw(_sprite);
        }

        [JSFunction(Name = "blitSurface")]
        public void BlitSurface(SurfaceInstance surf, int x, int y)
        {
            DrawImage(x, y, surf.GetBytes(), surf._width, surf._height);
            _changed = true;
        }

        [JSFunction(Name = "blitMaskSurface")]
        public void BlitMaskSurface(SurfaceInstance surf, int ox, int oy, ColorInstance mask)
        {
            Color mask_c = mask.GetColor();

            for (int y = 0; y < surf._height; ++y)
            {
                for (int x = 0; x < surf._width; ++x)
                {
                    Color dest = surf.GetColorAt(x, y);
                    Color final = ColorBlend(ref dest, ref mask_c);
                    SetPixel(ox + x, oy + y, ref final);
                }
            }

            _changed = true;
        }

        [JSFunction(Name = "blitImage")]
        public void BlitImage(int x, int y, ImageInstance instance)
        {
            using (Image img = instance.GetImage())
            {
                DrawImage(x, y, img.Pixels, img.Size.X, img.Size.Y);
            }
            _changed = true;
        }

        private void DrawImage(int ox, int oy, Byte[] pixels, uint width, uint height)
        {
            Color c;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int scan0 = (x + y * (int)width) << 2;

                    c.R = pixels[scan0 + 0];
                    c.G = pixels[scan0 + 1];
                    c.B = pixels[scan0 + 2];
                    c.A = pixels[scan0 + 3];

                    SetPixel(ox + x, oy + y, ref c);
                }
            }
        }

        [JSFunction(Name = "setPixel")]
        public void SetPixel(int x, int y, ColorInstance color)
        {
            Color col = color.GetColor();
            SetPixel(x, y, ref col);
            _changed = true;
        }

        public void SetPixel(int x, int y, ref Color dest)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
                return;
            _draw(x, y, dest);
        }

        public Color GetColorAt(int x, int y) {
            int scan0 = (x + y * (int)_width) << 2;
            byte R = _bytes[scan0 + 0];
            byte G = _bytes[scan0 + 1];
            byte B = _bytes[scan0 + 2];
            byte A = _bytes[scan0 + 3];
            return new Color(R, G, B, A);
        }

        private static Color ColorBlend(ref Color source, ref Color dest)
        {
            byte r = (byte)(source.R * (float)dest.R / 255);
            byte g = (byte)(source.G * (float)dest.G / 255);
            byte b = (byte)(source.B * (float)dest.B / 255);
            byte a = (byte)(source.A * (float)dest.A / 255);
            return new Color(r, g, b, a);
        }

        private static Color WeightedBlend(ref Color source, ref Color dest, double w)
        {
            double w1 = 1.0 - w;
            byte r = (byte)(source.R * w + dest.R * w1);
            byte g = (byte)(source.G * w + dest.G * w1);
            byte b = (byte)(source.B * w + dest.B * w1);
            byte a = (byte)(source.A * w + dest.A * w1);
            return new Color(r, g, b, a);
        }

        public void SetReplace(int x, int y, Color c) {
            int scan0 = (x + y * (int)_width) << 2;
            _bytes[scan0 + 0] = c.R;
            _bytes[scan0 + 1] = c.G;
            _bytes[scan0 + 2] = c.B;
            _bytes[scan0 + 3] = c.A;
        }

        private void SetAlphaBlend(int x, int y, Color dest)
        {
            Color source = GetColorAt(x, y);
            int scan0 = (x + y * (int)_width) << 2;
            float w0 = (float)dest.A / 255;
            _bytes[scan0 + 0] = (byte)(source.R * (1 - w0) + dest.R * w0);
            _bytes[scan0 + 1] = (byte)(source.G * (1 - w0) + dest.G * w0);
            _bytes[scan0 + 2] = (byte)(source.B * (1 - w0) + dest.B * w0);
            _bytes[scan0 + 3] = (byte)(source.A * (1 - w0) + dest.A * w0);
        }

        private void SetAddBlend(int x, int y, Color dest)
        {
            Color source = GetColorAt(x, y);
            int scan0 = (x + y * (int)_width) << 2;
            _bytes[scan0 + 0] = (byte)Math.Min(source.R + dest.R, 255);
            _bytes[scan0 + 1] = (byte)Math.Min(source.G + dest.G, 255);
            _bytes[scan0 + 2] = (byte)Math.Min(source.B + dest.B, 255);
            _bytes[scan0 + 3] = 255;
        }

        private void SetSubtractBlend(int x, int y, Color dest)
        {
            Color source = GetColorAt(x, y);
            int scan0 = (x + y * (int)_width << 2);
            _bytes[scan0 + 0] = (byte)Math.Max(source.R - dest.R, 0);
            _bytes[scan0 + 1] = (byte)Math.Max(source.G - dest.G, 0);
            _bytes[scan0 + 2] = (byte)Math.Max(source.B - dest.B, 0);
            _bytes[scan0 + 3] = 255; 
        }

        [JSFunction(Name = "replaceColor")]
        public void ReplaceColor(ColorInstance colorA, ColorInstance colorB)
        {
            Color a = colorA.GetColor();
            Color b = colorB.GetColor();
            for (var i = 0; i < _bytes.Length; i += 4)
            {
                if (_bytes[i] == a.R && _bytes[i + 1] == a.G && _bytes[i + 2] == a.B && _bytes[i + 3] == a.A)
                {
                    _bytes[i + 0] = b.R;
                    _bytes[i + 1] = b.G;
                    _bytes[i + 2] = b.B;
                    _bytes[i + 3] = b.A;
                }
            }
            _changed = true;
        }

        private static int RotateX(int x, int y, double rad, int m_x, int m_y)
        {
            return (int)(Math.Round((x - m_x) * Math.Cos(rad) -
                     (y - m_y) * Math.Sin(rad)) + m_x);
        }

        private static int RotateY(int x, int y, double rad, int m_x, int m_y)
        {
            return (int)(Math.Round((x - m_x) * Math.Sin(rad) +
                     (y - m_y) * Math.Cos(rad)) + m_y);
        }

        [JSFunction(Name = "rotate")]
        public void Rotate(double radians, bool resize)
        {
            byte[] src0 = _bytes;
            byte[] copy = new byte[_bytes.Length];

            int w2 = (int)(_width / 2);
            int h2 = (int)(_height / 2);

            for (int x = 0; x < _width; ++x)
            {
                for (int y = 0; y < _height; ++y)
                {
                    int nx = RotateX(x, y, radians, w2, h2);
                    int ny = RotateY(x, y, radians, w2, h2);
                    if (nx < 0 || ny < 0 || nx >= _width || ny >= _height) continue;

                    int scan0 = (x + y * (int)_width) << 2;
                    int scan1 = (nx + ny * (int)_width) << 2;

                    copy[scan0 + 0] = src0[scan1 + 0];
                    copy[scan0 + 1] = src0[scan1 + 1];
                    copy[scan0 + 2] = src0[scan1 + 2];
                    copy[scan0 + 3] = src0[scan1 + 3];
                }
            }

            for (int i = 0; i < copy.Length; ++i) { _bytes[i] = copy[i]; }
            _changed = true;
        }

        [JSFunction(Name = "resize")]
        public SurfaceInstance Resize(int width, int height)
        {
            Byte[] newbytes = new byte[width * height * 4];
            Array.Copy(_bytes, newbytes, Math.Min(newbytes.Length, _bytes.Length));
            return new SurfaceInstance(Engine, newbytes, (uint)width, (uint)height);
        }

        [JSFunction(Name = "rescale")]
        public SurfaceInstance Rescale(int width, int height)
        {
            byte[] src0 = _bytes;
            byte[] copy = new byte[width * height * 4];
            float w = _width / (float)width;
            float h = _height / (float)height;

            for (int y = 0; y < height; ++y) 
            {
                for (int x = 0; x < width; ++x)
                {
                    int scan0 = (x + y * width) << 2;
                    int scan1 = ((int)(x * w) + (int)(y * h) * (int)_width) << 2;
                    copy[scan0 + 0] = src0[scan1 + 0];
                    copy[scan0 + 1] = src0[scan1 + 1];
                    copy[scan0 + 2] = src0[scan1 + 2];
                    copy[scan0 + 3] = src0[scan1 + 3];
                }
            }

            return new SurfaceInstance(Engine, copy, (uint)width, (uint)height);
        }

        [JSFunction(Name = "getPixel")]
        public ColorInstance GetPixel(int x, int y)
        {
            return new ColorInstance(Program._engine, GetColorAt(x, y));
        }

        [JSFunction(Name = "flipHorizontally")]
        public void FlipHorizontally()
        {
            int scan = (int)_width * 4;
            int size = scan * (int)_height;
            for (int i = 0; i < size; i += scan)
                Array.Reverse(_bytes, i, scan);
            _changed = true;
        }

        [JSFunction(Name = "flipVertically")]
        public void FlipVertically()
        {
            //_image.FlipVertically();
            _changed = true;
        }

        [JSFunction(Name = "createImage")]
        public ImageInstance CreateImage()
        {
            if (_changed)
                _tex.Update(_bytes);

            return new ImageInstance(Program._engine, _tex);
        }

        [JSFunction(Name = "save")]
        public void Save(string filename)
        {
            if (_changed)
                _tex.Update(_bytes);

            _tex.CopyToImage().SaveToFile(Program.ParseSpherePath(filename, "images"));
        }

        [JSFunction(Name = "clone")]
        public SurfaceInstance Clone()
        {
            byte[] bytes = new byte[_width * _height * 4];
            Array.Copy(_bytes, bytes, _bytes.Length);
            return new SurfaceInstance(Engine, bytes, _width, _height);
        }

        [JSFunction(Name = "setBlendMode")]
        public void SetBlendMode(int mode)
        {
            _mode = (BlendModes)mode;
            switch (_mode)
            {
                case BlendModes.Replace:
                    _draw = SetReplace;
                    break;
                case BlendModes.Blend:
                    _draw = SetAlphaBlend;
                    break;
                case BlendModes.Add:
                    _draw = SetAddBlend;
                    break;
                case BlendModes.Subtract:
                    _draw = SetSubtractBlend;
                    break;
            }
        }

        [JSFunction(Name = "pointSeries")]
        public void PointSeries(ArrayInstance array, ColorInstance color)
        {
            Color c = color.GetColor();
            for (var i = 0; i < array.Length; ++i)
            {
                Vector2f vect = GlobalPrimitives.GetVector(array[i] as ObjectInstance);
                SetPixel((int)vect.X, (int)vect.Y, ref c);
            }
            _changed = true;
        }

        /// <summary>
        /// Bresenham's Line Algorithm.
        /// </summary>
        [JSFunction(Name = "line")]
        public void Line(int x1, int y1, int x2, int y2, ColorInstance color)
        {
            Color col = color.GetColor();
            Line(x1, y1, x2, y2, ref col);
            _changed = true;
        }

        public void Line(int x1, int y1, int x2, int y2, ref Color color)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy, e2;
            SetPixel(x1, y1, ref color);

            while (x1 != x2 || y1 != y2)
            {
                e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
                SetPixel(x1, y1, ref color);
            }
        }

        [JSFunction(Name = "lineSeries")]
        public void LineSeries(ArrayInstance points, ColorInstance color)
        {
            Color c = color.GetColor();
            for (var i = 1; i < points.Length; i += 2)
            {
                Vector2f start = GlobalPrimitives.GetVector(points[i - 1] as ObjectInstance);
                Vector2f end = GlobalPrimitives.GetVector(points[i] as ObjectInstance);
                Line((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, ref c);
            }
            _changed = true;
        }

        [JSFunction(Name = "gradientLine")]
        public void GradientLine(int x1, int y1, int x2, int y2, ColorInstance col1, ColorInstance col2)
        {
            GradientLine(x1, y1, x2, y2, col1.GetColor(), col2.GetColor());
            _changed = true;
        }

        public void GradientLine(int x1, int y1, int x2, int y2, Color col1, Color col2)
        {
            int dx = Math.Abs(x2 - x1), nx;
            int dy = Math.Abs(y2 - y1), ny;
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy, e2;
            double max_d = (dx * dx + dy * dy);

            SetPixel(x1, y1, ref col2);

            while (x1 != x2 || y1 != y2)
            {
                e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
                nx = x2 - x1;
                ny = y2 - y1;
                Color final = WeightedBlend(ref col2, ref col1, (nx * nx + ny * ny) / max_d);
                SetPixel(x1, y1, ref final);
            }
        }

        [JSFunction(Name = "rectangle")]
        public void Rectangle(int ox, int oy, int w, int h, ColorInstance color)
        {
            h += oy;
            w += ox;
            Color c = color.GetColor();
            for (int y = oy; y < h; ++y)
                for (int x = ox; x < w; ++x)
                    SetPixel(x, y, ref c);
            _changed = true;
        }

        [JSFunction(Name = "gradientRectangle")]
        public void GradientRectangle(int x, int y, int w, int h, ColorInstance color1, ColorInstance color2, ColorInstance color3, ColorInstance color4)
        {
            Color c1 = color1.GetColor(), c2 = color2.GetColor();
            Color c3 = color3.GetColor(), c4 = color4.GetColor();
            h += y;
            w += x;
            for (var i = y; i < h; ++i)
            {
                Color A = WeightedBlend(ref c4, ref c1, (double)i / h);
                Color B = WeightedBlend(ref c3, ref c2, (double)i / h);
                GradientLine(x, i, w, i, A, B);
            }
            _changed = true;
        }

        [JSFunction(Name = "outlinedRectangle")]
        public void OutlinedRectangle(int x, int y, int w, int h, ColorInstance color)
        {
            Color c = color.GetColor();
            Line(x, y, x + w, y, ref c);
            Line(x + w, y, x + w, y + h, ref c);
            Line(x + w, y + h, x, y + h, ref c);
            Line(x, y, x, y + h, ref c);
            _changed = true;
        }

        [JSFunction(Name = "filledCircle")]
        public void FilledCircle(int ox, int oy, int r, ColorInstance color, [DefaultParameterValue(false)] bool antialias = false)
        {
            if (ox < 0 || oy < 0 || r < 0 || color == null)
                throw new ArgumentException("Invalid parameters.");
            if (r == 0)
                return;

            var pi2 = Math.PI / 2;
            var w = r * 2;
            var h = w;
            Color c = color.GetColor();
            ox -= r;
            oy -= r;

            if (antialias)
            {
                for (var y = 0; y < r; ++y)
                {
                    for (var x = 0; x < r; ++x)
                    {
                        var dist = Math.Sqrt(x * x + y * y);
                        if (dist < r)
                        {
                            if (dist > r - 1)
                                c.A = (byte)(c.A * Math.Sin(Math.Sin((r - dist) * pi2) * pi2));
                            SetPixel(ox + x + r, oy + y + r, ref c);
                            SetPixel(ox + r - x, oy + y + r, ref c);
                            SetPixel(ox + r - x, oy + r - y, ref c);
                            SetPixel(ox + x + r, oy + r - y, ref c);
                        }
                    }
                }
            }
            else
            {
                for (var y = 0; y < r; ++y)
                {
                    int lw = (int)(r * Math.Cos(Math.Asin(1 - (float)y / r)));
                    Line(ox + r - lw, oy + y, ox + r + lw, oy + y, ref c);
                    Line(ox + r - lw, oy + h - y - 1, ox + r + lw, oy + h - y - 1, ref c);
                }
            }
            _changed = true;
        }

        [JSFunction(Name = "outlinedCircle")]
        public void OutlinedCircle(int ox, int oy, int r, ColorInstance color, [DefaultParameterValue(false)] bool antialias = false)
        {
            if (r < 0 || color == null || ox < 0 || oy < 0)
                throw new ArgumentException("Invalid parameters.");
            else if (r == 0)
                return;

            var w = r * 2;
            var h = w;
            var c = color.GetColor();

            if (antialias)
            {
                var pio2 = Math.PI / 2;
                for (var y = 0; y < r; ++y)
                {
                    for (var x = 0; x < r; ++x)
                    {
                        var dist = Math.Sqrt(x * x + y * y);
                        if (dist > r - 2 && dist < r)
                        {
                            c.A = (byte)(c.A * Math.Sin(Math.Sin((1 - Math.Abs(dist - r + 1)) * pio2) * pio2));
                            SetPixel(ox + x + r, oy + y + r, ref c);
                            SetPixel(ox + r - x, oy + y + r, ref c);
                            SetPixel(ox + r - x, oy + r - y, ref c);
                            SetPixel(ox + x + r, oy + r - y, ref c);
                        }
                    }
                }
            }
            else
            {
                var pi2 = Math.PI * 2;
                var step = pi2 / (Math.Min(360, pi2 * r));
                for (double pt = 0.0, pt2 = step; pt < pi2; pt += step, pt2 += step)
                {
                    int x1 = (int)(r + r * Math.Sin(pt));
                    int y1 = (int)(r + r * Math.Cos(pt));
                    int x2 = (int)(r + r * Math.Sin(pt2));
                    int y2 = (int)(r + r * Math.Cos(pt2));
                    Line(ox + x1, oy + y1, ox + x2, oy + y2, ref c);
                }
            }
            _changed = true;
        }

        [JSFunction(Name = "drawText")]
        public void DrawText(FontInstance font, int x, int y, string text)
        {
            for (var i = 0; i < text.Length; ++i) {
                Image img = font.GetGlyph(text[i]);
                DrawImage(x, y, img.Pixels, img.Size.X, img.Size.Y);
                x += (int)img.Size.X;
            }
            _changed = true;
        }

        [JSFunction(Name = "setAlpha")]
        public void SetAlpha(int v)
        {
            //_image.CreateMaskFromColor(color.GetColor());
            _changed = true;
        }

        [JSFunction(Name = "cloneSection")]
        public SurfaceInstance CloneSection(int ox, int oy, int w, int h)
        {
            byte[] bytes = new byte[w * h * 4];
            for (var y = 0; y < h; ++y)
            {
                for (var x = 0; x < w; ++x)
                {
                    Color c = GetColorAt(ox + x, oy + y);
                    int scan0 = x + y * w;
                    bytes[scan0 + 0] = c.R;
                    bytes[scan0 + 1] = c.G;
                    bytes[scan0 + 2] = c.B;
                    bytes[scan0 + 3] = c.A;
                }
            }
            return new SurfaceInstance(Engine, bytes, (uint)w, (uint)h);
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object surface]";
        }
    }
}

