using System;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public unsafe class SurfaceInstance : ObjectInstance
    {
        private Texture _tex;
        private bool _changed;
        private BlendModes _mode;
        private byte[] _pixels;
        private int _width, _height;

        public SurfaceInstance(ScriptEngine parent, int width, int height, Color bg_color)
            : base(parent.Object.InstancePrototype)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Width must be > 0.");
            _width = width;

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Height must be > 0.");
            _height = height;

            _pixels = new byte[width * height << 2];
            for (int i = 0; i < _pixels.Length; i += 4)
            {
                _pixels[i + 0] = bg_color.R;
                _pixels[i + 1] = bg_color.G;
                _pixels[i + 2] = bg_color.B;
                _pixels[i + 3] = bg_color.A;
            }

            Init();
        }

        public SurfaceInstance(ScriptEngine parent, string filename)
            : base(parent.Object.InstancePrototype)
        {
            using (Image img = new Image(filename))
            {
                _pixels = img.Pixels;
                _width = (int)img.Size.X;
                _height = (int)img.Size.Y;
            }
            Init();
        }

        public SurfaceInstance(ScriptEngine parent, byte[] pixels, int width, int height)
            : base(parent.Object.InstancePrototype)
        {
            _width = width;
            _height = height;
            _pixels = pixels;
            Init();
        }

        private void Update()
        {
            _tex.Update(_pixels);
            _changed = false;
        }

        private void Init() {
            PopulateFunctions();
            _tex = new Texture((uint)_width, (uint)_height);
            _tex.Smooth = GlobalProps.SmoothTextures;

            Update();
            SetBlendMode((int)BlendModes.Blend);

            DefineProperty("width", new PropertyDescriptor((int)_width, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor((int)_height, PropertyAttributes.Sealed), true);
        }

        public Byte[] GetBytes()
        {
            return _pixels;
        }

        [JSFunction(Name = "blit")]
        public void Blit(double x, double y)
        {
            if (_changed) Update();
            Program.Batch.Add(_tex, (float)x, (float)y);
        }

        [JSFunction(Name = "blitSurface")]
        public void BlitSurface(SurfaceInstance surf, int x, int y)
        {
            fixed (byte* buf = _pixels)
            {
                DrawImage(buf, x, y, surf.GetBytes(), (int)surf._width, (int)surf._height);
            }
            _changed = true;
        }

        [JSFunction(Name = "blitMaskSurface")]
        public void BlitMaskSurface(SurfaceInstance surf, int ox, int oy, ColorInstance mask)
        {
            Color mask_c = mask.GetColor(), final = new Color();
            fixed (byte* buf = _pixels)
            {
                for (int y = 0; y < surf._height; ++y)
                {
                    for (int x = 0; x < surf._width; ++x)
                    {
                        Color dest = surf.GetColorAt(x, y);
                        ColorBlend(ref dest, ref mask_c, ref final); // todo
                        SetPixel(buf, ox + x, oy + y, ref final);
                    }
                }
            }

            _changed = true;
        }

        [JSFunction(Name = "blitImage")]
        public void BlitImage(int x, int y, ImageInstance instance)
        {
            using (Image img = instance.GetImage())
            {
                DrawImage(x, y, img.Pixels, (int)img.Size.X, (int)img.Size.Y);
            }
            _changed = true;
        }

        private void DrawImage(byte* buf, int ox, int oy, byte[] pixels, int width, int height)
        {
            int left = Math.Max(0, ox);
            int top = Math.Max(0, oy);
            int right = Math.Min(_width, ox + width);
            int bottom = Math.Min(_height, oy + height);
            for (int y = top; y < bottom; ++y)
            {
                for (int x = left; x < right; ++x)
                {
                    int s = (x - ox + (y - oy) * width) << 2;
                    int c = (pixels[s + 3] << 24) + (pixels[s + 2] << 16) + (pixels[s + 1] << 8) + pixels[s]; // abgr
                    SetPixelFast(buf, x, y, c);
                }
            }
        }

        private void DrawImageMask(byte* buf, int ox, int oy, byte[] pixels, int width, int height, int mask)
        {
            int left = Math.Max(0, ox);
            int top = Math.Max(0, oy);
            int right = Math.Min(_width, ox + width);
            int bottom = Math.Min(_height, oy + height);
            for (int y = top; y < bottom; ++y)
            {
                for (int x = left; x < right; ++x)
                {
                    int s = (x - ox + (y - oy) * width) << 2;
                    int c = (pixels[s + 3] << 24) + (pixels[s + 2] << 16) + (pixels[s + 1] << 8) + pixels[s]; // abgr
                    int d = FastBlend2(c, mask);
                    SetPixelFast(buf, x, y, d);
                }
            }
        }
        
        private void DrawImage(int ox, int oy, byte[] pixels, int width, int height)
        {
            fixed (byte* pix = _pixels)
            {
                DrawImage(pix, ox, oy, pixels, width, height);
            }
        }

        [JSFunction(Name = "setPixel")]
        public void SetPixel(int x, int y, ColorInstance color)
        {
            fixed (byte* buf = _pixels)
            {
                SetPixelFast(buf, x, y, color.GetInt());
            }
            _changed = true;
        }

        public static void Blend(Color* c1, ref Color c2)
        {
            float w = (float)c2.A / 255;
            c1->R = (byte)(w * (c2.R - c1->R) + c1->R);
            c1->G = (byte)(w * (c2.G - c1->G) + c1->G);
            c1->B = (byte)(w * (c2.B - c1->B) + c1->B);
            c1->A = Math.Max(c1->A, c2.A);
        }

        public static int FastBlend(int c1, int c2, float weight)
        {
            int a1 = (c1 >> 24 & 0xff);
            int b1 = ((c1 & 0xff0000) >> 16);
            int g1 = ((c1 & 0xff00) >> 8);
            int r1 = (c1 & 0xff);

            int a2 = (c2 >> 24 & 0xff);
            int b2 = ((c2 & 0xff0000) >> 16);
            int g2 = ((c2 & 0xff00) >> 8);
            int r2 = (c2 & 0xff);

            int a = Math.Max(a1, a2);
            int b = (int)(weight * (b2 - b1) + b1);
            int g = (int)(weight * (g2 - g1) + g1);
            int r = (int)(weight * (r2 - r1) + r1);

            return a << 24 | b << 16 | g << 8 | r;
        }

        public static int FastBlend2(int c1, int c2)
        {
            int a1 = (c1 >> 24 & 0xff);
            int b1 = ((c1 & 0xff0000) >> 16);
            int g1 = ((c1 & 0xff00) >> 8);
            int r1 = (c1 & 0xff);

            int a2 = (c2 >> 24 & 0xff);
            int b2 = ((c2 & 0xff0000) >> 16);
            int g2 = ((c2 & 0xff00) >> 8);
            int r2 = (c2 & 0xff);

            //int b = (int)(weight * (b2 - b1) + b1);
            //int g = (int)(weight * (g2 - g1) + g1);
            //int r = (int)(weight * (r2 - r1) + r1);
            int r = (int)(r2 * (float)r1 / 255);
            int g = (int)(g2 * (float)g1 / 255);
            int b = (int)(b2 * (float)b1 / 255);
            int a = (int)(a2 * (float)a1 / 255);

            return a1 << 24 | b << 16 | g << 8 | r;
        }

        public void SetPixelFast(byte* buffer, int x, int y, int c)
        {
            int* p = (int*)buffer + (x + y * (int)_width);

            /*switch (_mode)
            {
                case BlendModes.Blend:
                    *p = FastBlend(*p, c, (float)(c >> 24 & 0xff) / 255);
                    break;
                case BlendModes.Replace:
                    *p = c;
                    break;
            }*/
            if (_mode == BlendModes.Blend)
            {
                *p = FastBlend(*p, c, (float)(c >> 24 & 0xff) / 255);
            }
            else
            {
                *p = c;
            }
        }

        public void SetPixel(byte* buffer, int x, int y, ref Color c)
        {
            int idx = y * (int)_width + x;
            if (idx < 0 || idx >= _pixels.Length) { Console.WriteLine(idx); return; }
            //if (x < 0 || y < 0 || x > _width || y > _height) return;
            Color* color = (Color*)(buffer + (idx << 2));
            switch (_mode)
            {
                case BlendModes.Blend:
                    Blend(color, ref c);
                    break;
                case BlendModes.Replace:
                    *color = c;
                    break;
            }
        }

        public Color GetColorAt(int x, int y) {
            int s = (x + y * _width) << 2;
            return new Color(_pixels[s + 0], _pixels[s + 1], _pixels[s + 2], _pixels[s + 3]);
        }

        private static void ColorBlend(ref Color source, ref Color dest, ref Color out_c)
        {
            out_c.R = (byte)(source.R * (float)dest.R / 255);
            out_c.G = (byte)(source.G * (float)dest.G / 255);
            out_c.B = (byte)(source.B * (float)dest.B / 255);
            out_c.A = (byte)(source.A * (float)dest.A / 255);
        }

        private static void WeightedBlend(ref Color source, ref Color dest, double w, out Color out_c)
        {
            double w1 = 1.0 - w;
            out_c.R = (byte)(source.R * w + dest.R * w1);
            out_c.G = (byte)(source.G * w + dest.G * w1);
            out_c.B = (byte)(source.B * w + dest.B * w1);
            out_c.A = (byte)(source.A * w + dest.A * w1);
        }

        public void SetReplace(int x, int y, Color dest) {
            int scan0 = (x + y * (int)_width) << 2;
            _pixels[scan0 + 0] = dest.R;
            _pixels[scan0 + 1] = dest.G;
            _pixels[scan0 + 2] = dest.B;
            _pixels[scan0 + 3] = dest.A;
        }

        private void SetAlphaBlend(int x, int y, Color dest)
        {
            Color source = GetColorAt(x, y);
            int scan0 = (x + y * (int)_width) << 2;
            float w0 = (float)dest.A / 255, w1 = (1 - w0);
            _pixels[scan0 + 0] = (byte)(source.R * w1 + dest.R * w0);
            _pixels[scan0 + 1] = (byte)(source.G * w1 + dest.G * w0);
            _pixels[scan0 + 2] = (byte)(source.B * w1 + dest.B * w0);
            _pixels[scan0 + 3] = (byte)(source.A * w1 + dest.A * w0);
        }

        private void SetAddBlend(int x, int y, Color dest)
        {
            Color source = GetColorAt(x, y);
            int scan0 = (x + y * (int)_width) << 2;
            _pixels[scan0 + 0] = (byte)Math.Min(source.R + dest.R, 255);
            _pixels[scan0 + 1] = (byte)Math.Min(source.G + dest.G, 255);
            _pixels[scan0 + 2] = (byte)Math.Min(source.B + dest.B, 255);
            //_pixels[scan0 + 3] = 255;
        }

        private void SetSubtractBlend(int x, int y, Color dest)
        {
            Color source = GetColorAt(x, y);
            int scan0 = (x + y * (int)_width) << 2;
            _pixels[scan0 + 0] = (byte)Math.Max(source.R - dest.R, 0);
            _pixels[scan0 + 1] = (byte)Math.Max(source.G - dest.G, 0);
            _pixels[scan0 + 2] = (byte)Math.Max(source.B - dest.B, 0);
            //_pixels[scan0 + 3] = 255;
        }

        [JSFunction(Name = "replaceColor")]
        public void ReplaceColor(ColorInstance colorA, ColorInstance colorB)
        {
            Color a = colorA.GetColor();
            Color b = colorB.GetColor();
            for (var i = 0; i < _pixels.Length; i += 4)
            {
                if (_pixels[i] == a.R && _pixels[i + 1] == a.G && _pixels[i + 2] == a.B && _pixels[i + 3] == a.A)
                {
                    _pixels[i + 0] = b.R;
                    _pixels[i + 1] = b.G;
                    _pixels[i + 2] = b.B;
                    _pixels[i + 3] = b.A;
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
            byte[] src0 = _pixels;
            byte[] copy = new byte[_pixels.Length];

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

            for (int i = 0; i < copy.Length; ++i) { _pixels[i] = copy[i]; }
            _changed = true;
        }

        [JSFunction(Name = "resize")]
        public SurfaceInstance Resize(int width, int height)
        {
            Byte[] newbytes = new byte[width * height * 4];
            Array.Copy(_pixels, newbytes, Math.Min(newbytes.Length, _pixels.Length));
            return new SurfaceInstance(Engine, newbytes, width, height);
        }

        [JSFunction(Name = "rescale")]
        public SurfaceInstance Rescale(int width, int height)
        {
            byte[] src0 = _pixels;
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

            return new SurfaceInstance(Engine, copy, width, height);
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
                Array.Reverse(_pixels, i, scan);
            _changed = true;
        }

        [JSFunction(Name = "flipVertically")]
        public void FlipVertically()
        {
            Array.Reverse(_pixels, 0, _pixels.Length);
            FlipHorizontally();
            _changed = true;
        }

        [JSFunction(Name = "createImage")]
        public ImageInstance CreateImage()
        {
            if (_changed) Update();

            return new ImageInstance(Program._engine, _tex);
        }

        [JSFunction(Name = "save")]
        public void Save(string filename)
        {
            if (_changed) Update();

            _tex.CopyToImage().SaveToFile(Program.ParseSpherePath(filename, "images"));
        }

        [JSFunction(Name = "clone")]
        public SurfaceInstance Clone()
        {
            byte[] bytes = new byte[_width * _height * 4];
            Array.Copy(_pixels, bytes, _pixels.Length);
            return new SurfaceInstance(Engine, bytes, _width, _height);
        }

        [JSFunction(Name = "setBlendMode")]
        public void SetBlendMode(int mode)
        {
            _mode = (BlendModes)mode;
        }

        [JSFunction(Name = "pointSeries")]
        public void PointSeries(ArrayInstance array, ColorInstance color)
        {
            int c = color.GetInt();
            fixed (byte* buf = _pixels)
            {
                for (var i = 0; i < array.Length; ++i)
                {
                    Vector2f vect = GlobalPrimitives.GetVector(array[i] as ObjectInstance);
                    SetPixelFast(buf, (int)vect.X, (int)vect.Y, c);
                }
            }
            _changed = true;
        }

        /// <summary>
        /// Bresenham's Line Algorithm.
        /// </summary>
        [JSFunction(Name = "line")]
        public void Line(int x1, int y1, int x2, int y2, ColorInstance color)
        {
            int col = color.GetInt();
            fixed (byte* buf = _pixels)
            {
                Line(buf, x1, y1, x2, y2, col);
            }
            _changed = true;
        }

        public void Line(byte* buf, int x1, int y1, int x2, int y2, int c)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy, e2;
            SetPixelFast(buf, x1, y1, c);

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
                SetPixelFast(buf, x1, y1, c);
            }
        }

        [JSFunction(Name = "lineSeries")]
        public void LineSeries(ArrayInstance points, ColorInstance color)
        {
            int c = color.GetInt();
            fixed (byte* buf = _pixels)
            {
                for (var i = 1; i < points.Length; i += 2)
                {
                    Vector2f start = GlobalPrimitives.GetVector(points[i - 1] as ObjectInstance);
                    Vector2f end = GlobalPrimitives.GetVector(points[i] as ObjectInstance);
                    Line(buf, (int)start.X, (int)start.Y, (int)end.X, (int)end.Y, c);
                }
            }
            _changed = true;
        }

        [JSFunction(Name = "gradientLine")]
        public void GradientLine(int x1, int y1, int x2, int y2, ColorInstance col1, ColorInstance col2)
        {
            fixed (byte* buf = _pixels)
            {
                GradientLine(buf, x1, y1, x2, y2, col1.GetInt(), col2.GetInt());
            }
            _changed = true;
        }

        public void GradientLine(byte* buf, int x1, int y1, int x2, int y2, int c1, int c2)
        {
            x1 = x1 < 0 ? 0 : x1;
            x2 = x2 > _width ? _width : x2;
            y1 = y1 < 0 ? 0 : y1;
            y2 = y2 > _height ? _height : y2;

            int dx = Math.Abs(x2 - x1), nx;
            int dy = Math.Abs(y2 - y1), ny;
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy, e2;
            float max_d = (dx * dx + dy * dy);

            SetPixelFast(buf, x1, y1, c2);

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
                SetPixelFast(buf, x1, y1, FastBlend(c1, c2, (nx * nx + ny * ny) / max_d));
            }
        }

        [JSFunction(Name = "rectangle")]
        public void Rectangle(int ox, int oy, int w, int h, ColorInstance color)
        {
            h = Math.Max(0, Math.Min(h + oy, (int)_height));
            w = Math.Max(0, Math.Min(w + ox, (int)_width));
            int c = color.GetInt();
            fixed (byte* buf = _pixels)
            {
                for (int y = oy; y < h; ++y)
                    for (int x = ox; x < w; ++x)
                        SetPixelFast(buf, x, y, c);
            }
            _changed = true;
        }

        [JSFunction(Name = "gradientRectangle")]
        public void GradientRectangle(int ox, int oy, int w, int h, ColorInstance col1, ColorInstance col2, ColorInstance col3, ColorInstance col4)
        {
            h = Math.Max(0, Math.Min(h + oy, (int)_height));
            w = Math.Max(0, Math.Min(w + ox, (int)_width));
            int c1 = col1.GetInt(), c2 = col2.GetInt();
            int c3 = col3.GetInt(), c4 = col4.GetInt();

            fixed (byte* buf = _pixels)
            {
                for (int y = 0; y < h; ++y)
                {
                    float weight = (float)y / h;
                    int w1 = FastBlend(c1, c4, weight);
                    int w2 = FastBlend(c2, c3, weight);
                    for (int x = 0; x < w; ++x)
                    {
                        int c = FastBlend(w1, w2, (float)x / w);
                        SetPixelFast(buf, ox + x, oy + y, c);
                    }
                }
            }
            _changed = true;
        }

        [JSFunction(Name = "outlinedRectangle")]
        public void OutlinedRectangle(int x, int y, int w, int h, ColorInstance color)
        {
            int c = color.GetInt();
            fixed (byte* buf = _pixels)
            {
                Line(buf, x, y, x + w, y, c);
                Line(buf, x + w, y, x + w, y + h, c);
                Line(buf, x + w, y + h, x, y + h, c);
                Line(buf, x, y, x, y + h, c);
            }
            _changed = true;
        }

        [JSFunction(Name = "filledCircle")]
        public void FilledCircle(int ox, int oy, int r, ColorInstance color, [DefaultParameterValue(false)] bool antialias = false)
        {
            if (ox < 0 || oy < 0 || r < 0 || color == null)
                throw new ArgumentException("Invalid parameters.");
            if (r == 0)
                return;

            fixed (byte* buf = _pixels)
            {

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
                                SetPixel(buf, ox + x + r, oy + y + r, ref c);
                                SetPixel(buf, ox + r - x, oy + y + r, ref c);
                                SetPixel(buf, ox + r - x, oy + r - y, ref c);
                                SetPixel(buf, ox + x + r, oy + r - y, ref c);
                            }
                        }
                    }
                }
                else
                {
                    int ci = color.GetInt();
                    for (var y = 0; y < r; ++y)
                    {
                        int lw = (int)(r * Math.Cos(Math.Asin(1 - (float)y / r)));
                        Line(buf, ox + r - lw, oy + y, ox + r + lw, oy + y, ci);
                        Line(buf, ox + r - lw, oy + h - y - 1, ox + r + lw, oy + h - y - 1, ci);
                    }
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

            fixed (byte* buf = _pixels)
            {
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
                                SetPixel(buf, ox + x + r, oy + y + r, ref c);
                                SetPixel(buf, ox + r - x, oy + y + r, ref c);
                                SetPixel(buf, ox + r - x, oy + r - y, ref c);
                                SetPixel(buf, ox + x + r, oy + r - y, ref c);
                            }
                        }
                    }
                }
                else
                {
                    var pi2 = Math.PI * 2;
                    var step = pi2 / (Math.Min(360, pi2 * r));
                    int ci = color.GetInt();
                    for (double pt = 0.0, pt2 = step; pt < pi2; pt += step, pt2 += step)
                    {
                        int x1 = (int)(r + r * Math.Sin(pt));
                        int y1 = (int)(r + r * Math.Cos(pt));
                        int x2 = (int)(r + r * Math.Sin(pt2));
                        int y2 = (int)(r + r * Math.Cos(pt2));
                        Line(buf, ox + x1, oy + y1, ox + x2, oy + y2, ci);
                    }
                }
            }
            _changed = true;
        }

        [JSFunction(Name = "drawText")]
        public void DrawText(FontInstance font, int x, int y, string text)
        {
            int color = font.GetColorMask().GetInt();
            fixed (byte* buf = _pixels)
            {
                for (var i = 0; i < text.Length; ++i)
                {
                    Image img = font.GetGlyph(text[i]);
                    DrawImageMask(buf, x, y, img.Pixels, (int)img.Size.X, (int)img.Size.Y, color);
                    x += (int)img.Size.X;
                }
            }
            _changed = true;
        }

        [JSFunction(Name = "setAlpha")]
        public void SetAlpha(int v)
        {
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
            return new SurfaceInstance(Engine, bytes, w, h);
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object surface]";
        }
    }
}

