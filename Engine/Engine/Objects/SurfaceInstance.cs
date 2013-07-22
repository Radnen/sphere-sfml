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
            Image other = surf.GetImageRef();
            uint w = other.Size.X;
            uint h = other.Size.Y;

            for (uint xx = 0; xx < w; ++xx)
                for (uint yy = 0; yy < h; ++yy)
                    SetPixel((int)(x + xx), (int)(y + yy), other.GetPixel(xx, yy));

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
                    SetPixel((int)(x + xx), (int)(y + yy), ColorBlend(dest, mask_c));
                }
            }

            _changed = true;
        }

        [JSFunction(Name = "blitImage")]
        public void BlitSurface(int x, int y, ImageInstance img)
        {
            Image other = img.GetImage();
            uint w = other.Size.X;
            uint h = other.Size.Y;

            for (uint xx = 0; xx < w; ++xx)
                for (uint yy = 0; yy < h; ++yy)
                    SetPixel((int)(x + xx), (int)(y + yy), other.GetPixel(xx, yy));

            other.Dispose();
            _changed = true;
        }

        [JSFunction(Name = "setPixel")]
        public void SetPixel(int x, int y, ColorInstance color)
        {
            SetPixel(x, y, color.GetColor());
            _changed = true;
        }

        public void SetPixel(int x, int y, Color dest)
        {
            Color source = _image.GetPixel((uint)x, (uint)y);
            switch (_mode)
            {
                case BlendModes.Replace:
                    _image.SetPixel((uint)x, (uint)y, dest);
                    break;
                case BlendModes.Blend:
                    _image.SetPixel((uint)x, (uint)y, AlphaBlend(source, dest));
                    break;
                case BlendModes.Add:
                    _image.SetPixel((uint)x, (uint)y, AddBlend(source, dest));
                    break;
                case BlendModes.Subtract:
                    _image.SetPixel((uint)x, (uint)y, SubtractBlend(source, dest));
                    break;
            }
        }

        private static Color ColorBlend(Color source, Color dest)
        {
            byte r = (byte)(source.R * (double)dest.R / 255);
            byte g = (byte)(source.G * (double)dest.G / 255);
            byte b = (byte)(source.B * (double)dest.B / 255);
            byte a = (byte)(source.A * (double)dest.A / 255);
            return new Color(r, g, b, a);
        }

        private static Color AlphaBlend(Color source, Color dest)
        {
            double w0 = (double)dest.A / 255;
            double w1 = 1.0 - w0;
            byte r = (byte)(source.R * w1 + dest.R * w0);
            byte g = (byte)(source.G * w1 + dest.G * w0);
            byte b = (byte)(source.B * w1 + dest.B * w0);
            return new Color(r, g, b, 255);
        }

        private static Color AddBlend(Color source, Color dest)
        {
            byte r = (byte)Math.Min(source.R + dest.R, 255);
            byte g = (byte)Math.Min(source.G + dest.G, 255);
            byte b = (byte)Math.Min(source.B + dest.B, 255);
            return new Color(r, g, b, 255);
        }

        private static Color SubtractBlend(Color source, Color dest)
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

        /// <summary>
        /// Bresenham's Line Algorithm.
        /// </summary>
        [JSFunction(Name = "line")]
        public void Line(int x1, int y1, int x2, int y2, ColorInstance color)
        {
            Line(x1, y1, x2, y2, color.GetColor());
        }

        public void Line(int x1, int y1, int x2, int y2, Color color)
        {
            x1 = Math.Max(0, x1);
            y1 = Math.Max(0, y1);
            x2 = Math.Min(x2, (int)(_image.Size.X) - 1);
            y2 = Math.Min(y2, (int)(_image.Size.Y) - 1);
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy, e2;
            SetPixel(x1, y1, color);

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
                SetPixel(x1, y1, color);
            }
            _changed = true;
        }

        [JSFunction(Name = "lineSeries")]
        public void LineSeries(ArrayInstance points, ColorInstance color)
        {
            for (var i = 1; i < points.Length; i += 2)
            {
                Vector2f start = GlobalPrimitives.GetVector(points[i - 1] as ObjectInstance);
                Vector2f end = GlobalPrimitives.GetVector(points[i] as ObjectInstance);
                Line((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color);
            }
        }

        [JSFunction(Name = "gradientLine")]
        public void GradientLine(int x1, int y1, int x2, int y2, ColorInstance col1, ColorInstance col2)
        {
            GradientLine(x1, y1, x2, y2, col1.GetColor(), col2.GetColor());
        }

        public void GradientLine(int x1, int y1, int x2, int y2, Color col1, Color col2)
        {
            x1 = Math.Max(0, x1);
            y1 = Math.Max(0, y1);
            x2 = Math.Min(x2, (int)(_image.Size.X) - 1);
            y2 = Math.Min(y2, (int)(_image.Size.Y) - 1);
            int dx = Math.Abs(x2 - x1), nx;
            int dy = Math.Abs(y2 - y1), ny;
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy, e2;
            double dist;

            SetPixel(x1, y1, col2);

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
                dist = (nx * nx + ny * ny) / (double)(dx * dx + dy * dy);
                SetPixel(x1, y1, Program.BlendColorsWeighted(col2, col1, dist));
            }
            _changed = true;
        }

        [JSFunction(Name = "rectangle")]
        public void Rectangle(int x, int y, int w, int h, ColorInstance color)
        {
            w += x;
            h += y;
            Color col = color.GetColor();
            for (int i = y; i < h; ++i)
                Line(x, i, w, i, col);
            _changed = true;
        }

        [JSFunction(Name = "gradientRectangle")]
        public void GradientRectangle(int x, int y, int w, int h, ColorInstance c1, ColorInstance c2, ColorInstance c3, ColorInstance c4)
        {
            h += y;
            w += x;
            for (var i = y; i < h; ++i)
            {
                ColorInstance A = Program.BlendColorsWeighted(c4, c1, (double)i / h);
                ColorInstance B = Program.BlendColorsWeighted(c3, c2, (double)i / h);
                GradientLine(x, i, w, i, A, B);
            }
        }

        [JSFunction(Name = "setAlpha")]
        public void SetAlpha(ColorInstance color)
        {
            _image.CreateMaskFromColor(color.GetColor());
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

