using System;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class SurfaceInstance : ObjectInstance
    {
        private Image _image;
        private Texture _tex;
        private bool _changed;
        private Sprite _sprite;
        private BlendModes _mode;

        public SurfaceInstance(ScriptEngine parent, int width, int height, Color bg_color)
            : base(parent)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Width must be > 0.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Height must be > 0.");

            _image = new Image((uint)width, (uint)height, bg_color);
            Init();
        }

        public SurfaceInstance(ScriptEngine parent, Image copy, bool clone = true)
            : base(parent)
        {
            _image = (clone) ? new Image(copy) : copy;
            Init();
        }

        public SurfaceInstance(ScriptEngine parent, string filename)
            : base(parent)
        {
            _image = new Image(filename);
            Init();
        }

        private void Init() {
            PopulateFunctions();
            _tex = new Texture(_image);
            _sprite = new Sprite(_tex);
            _mode = BlendModes.Blend;

            DefineProperty("width", new PropertyDescriptor((int)_image.Size.X, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor((int)_image.Size.Y, PropertyAttributes.Sealed), true);
        }

        public Image GetImageRef()
        {
            return _image;
        }

        [JSFunction(Name = "blit")]
        public void Blit(double x, double y)
        {
            if (_changed)
                _tex.Update(_image);

            _sprite.Position = new Vector2f((float)x, (float)y);
            Program._window.Draw(_sprite);
        }

        [JSFunction(Name = "blitSurface")]
        public void BlitSurface(SurfaceInstance surf, int x, int y)
        {
            DrawImage(x, y, surf.GetImageRef());
            _changed = true;
        }

        [JSFunction(Name = "blitMaskSurface")]
        public void BlitMaskSurface(SurfaceInstance surf, int x, int y, ColorInstance mask)
        {
            Color mask_c = mask.GetColor();
            Image other = surf.GetImageRef();
            uint w = other.Size.X;
            uint h = other.Size.Y;

            for (uint xx = 0; xx < w; ++xx)
            {
                for (uint yy = 0; yy < h; ++yy)
                {
                    Color dest = other.GetPixel(xx, yy);
                    Color final = ColorBlend(ref dest, ref mask_c);
                    SetPixel((int)(x + xx), (int)(y + yy), ref final);
                }
            }

            _changed = true;
        }

        [JSFunction(Name = "blitImage")]
        public void BlitImage(int x, int y, ImageInstance img)
        {
            Image other = img.GetImage();
            DrawImage(x, y, other);
            other.Dispose();
            _changed = true;
        }

        private void DrawImage(int x, int y, Image img)
        {
            uint w = img.Size.X;
            uint h = img.Size.Y;

            for (int j = 0; j < h; ++j)
            {
                for (int i = 0; i < w; ++i)
                {
                    Color color = img.GetPixel((uint)i, (uint)j);
                    SetPixel(x + i, y + j, ref color);
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
            if (x >= _image.Size.X || y >= _image.Size.Y || x < 0 || y < 0)
                return;
            Color source = _image.GetPixel((uint)x, (uint)y);
            switch (_mode)
            {
                case BlendModes.Replace:
                    _image.SetPixel((uint)x, (uint)y, dest);
                    break;
                case BlendModes.Blend:
                    _image.SetPixel((uint)x, (uint)y, AlphaBlend(ref source, ref dest));
                    break;
                case BlendModes.Add:
                    _image.SetPixel((uint)x, (uint)y, AddBlend(ref source, ref dest));
                    break;
                case BlendModes.Subtract:
                    _image.SetPixel((uint)x, (uint)y, SubtractBlend(ref source, ref dest));
                    break;
            }
        }

        private static Color ColorBlend(ref Color source, ref Color dest)
        {
            byte r = (byte)(source.R * (float)dest.R / 255);
            byte g = (byte)(source.G * (float)dest.G / 255);
            byte b = (byte)(source.B * (float)dest.B / 255);
            byte a = (byte)(source.A * (float)dest.A / 255);
            return new Color(r, g, b, a);
        }

        private static Color AlphaBlend(ref Color source, ref Color dest)
        {
            float w0 = (float)dest.A / 255;
            byte r = (byte)(source.R * (1 - w0) + dest.R * w0);
            byte g = (byte)(source.G * (1 - w0) + dest.G * w0);
            byte b = (byte)(source.B * (1 - w0) + dest.B * w0);
            return new Color(r, g, b, 255);
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

        private static Color AddBlend(ref Color source, ref Color dest)
        {
            byte r = (byte)Math.Min(source.R + dest.R, 255);
            byte g = (byte)Math.Min(source.G + dest.G, 255);
            byte b = (byte)Math.Min(source.B + dest.B, 255);
            return new Color(r, g, b, 255);
        }

        private static Color SubtractBlend(ref Color source, ref Color dest)
        {
            byte r = (byte)Math.Max(source.R - dest.R, 0);
            byte g = (byte)Math.Max(source.G - dest.G, 0);
            byte b = (byte)Math.Max(source.B - dest.B, 0);
            return new Color(r, g, b, 255);
        }

        [JSFunction(Name = "replaceColor")]
        public void ReplaceColor(ColorInstance colorA, ColorInstance colorB)
        {
            _image.ReplaceColor(colorA.GetColor(), colorB.GetColor());
            _changed = true;
        }

        [JSFunction(Name = "resize")]
        public void Resize(int width, int height)
        {
            Image copy = new Image((uint)width, (uint)height, new Color(0, 0, 0, 0));
            copy.Copy(_image, 0, 0, new IntRect(0, 0, Math.Min(width, (int)_image.Size.X), Math.Min(height, (int)_image.Size.Y)));
            _image.Dispose();
            _image = copy;
            _changed = true;
        }

        [JSFunction(Name = "getPixel")]
        public ColorInstance GetPixel(int x, int y)
        {
            return new ColorInstance(Program._engine, _image.GetPixel((uint)x, (uint)y));
        }

        [JSFunction(Name = "flipHorizontally")]
        public void FlipHorizontally()
        {
            _image.FlipHorizontally();
            _changed = true;
        }

        [JSFunction(Name = "flipVertically")]
        public void FlipVertically()
        {
            _image.FlipVertically();
            _changed = true;
        }

        [JSFunction(Name = "createImage")]
        public ImageInstance CreateImage()
        {
            if (_changed)
                _tex.Update(_image);

            return new ImageInstance(Program._engine, _tex);
        }

        [JSFunction(Name = "clone")]
        public SurfaceInstance Clone()
        {
            return new SurfaceInstance(Engine, _image);
        }

        [JSFunction(Name = "setBlendMode")]
        public void SetBlendMode(int mode)
        {
            _mode = (BlendModes)mode;
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
        public void Rectangle(int x, int y, int w, int h, ColorInstance color)
        {
            w += x;
            h += y;
            Color c = color.GetColor();
            for (int i = y; i < h; ++i)
                Line(x, i, w, i, ref c);
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
        public void FilledCircle(int ox, int oy, int r, ColorInstance color, bool antialias = false)
        {
            if (ox < 0 || oy < 0 || r < 0 || color == null)
                throw new ArgumentException("Invalid parameters.");
            if (r == 0)
                return;

            var pi2 = Math.PI / 2;
            var w = r * 2;
            var h = w;
            Color c = color.GetColor();

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
                    int lw = (int)(r * Math.Cos(Math.Asin(1 - y / r)));
                    Line(ox + r - lw, oy + y, ox + r + lw, oy + y, ref c);
                    Line(ox + r - lw, oy + h - y - 1, ox + r + lw, oy + h - y - 1, ref c);
                }
            }
            _changed = true;
        }

        [JSFunction(Name = "outlinedCircle")]
        public void OutlinedCircle(int ox, int oy, int r, ColorInstance color, bool antialias = false)
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
                DrawImage(x, y, img);
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
        public SurfaceInstance CloneSection(int x, int y, int w, int h)
        {
            using (Image image = new Image((uint)w, (uint)h))
            {
                image.Copy(_image, 0, 0, new IntRect(x, y, w, h));
                return new SurfaceInstance(Engine, image);
            }
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object surface]";
        }
    }
}

