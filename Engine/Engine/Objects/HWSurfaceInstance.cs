using System;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class HWSurfaceInstance : ObjectInstance
    {
        private RenderTexture _tex;
        private bool _changed = false;
        private BlendModes _mode;
        private uint _width, _height;
        private SpriteBatch _myBatch;
        private RenderStates _states;
        private Texture _cache;

        public HWSurfaceInstance(ScriptEngine parent, int width, int height, Color bg_color)
            : base(parent.Object.InstancePrototype)
        {
            Console.WriteLine("Create surf: {0}x{1} {2}", width, height, bg_color);
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Width must be > 0.");
            _width = (uint)width;

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Height must be > 0.");
            _height = (uint)height;

            _tex = new RenderTexture(_width, _height);
            _tex.Clear(bg_color);

            Init();
        }

        public HWSurfaceInstance(ScriptEngine parent, string filename)
            : base(parent.Object.InstancePrototype)
        {
            using (Texture tex = new Texture(filename))
            {
                _width = tex.Size.X;
                _height = tex.Size.Y;
                _tex = new RenderTexture(_width, _height);
                _myBatch = new SpriteBatch(_tex);
                _myBatch.SetBlendMode(BlendMode.None);
                _myBatch.Add(tex, 0, 0);
                _myBatch.Flush();
                Update();
            }
            Init();
        }

        private void Init() {
            PopulateFunctions();
            if (_myBatch == null) _myBatch = new SpriteBatch(_tex);
            SetBlendMode((int)BlendModes.Blend);
            DefineProperty("width", new PropertyDescriptor((int)_width, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor((int)_height, PropertyAttributes.Sealed), true);
        }

        public void Update()
        {
            _myBatch.Flush();
            _tex.Display();
            _cache = _tex.Texture;
            _changed = false;
        }

        [JSFunction(Name = "blit")]
        public void Blit(double x, double y)
        {
            if (_changed) Update();
            Program.Batch.Add(_cache, (float)x, (float)y);
        }

        [JSFunction(Name = "blitSurface")]
        public void BlitSurface(HWSurfaceInstance surf, int x, int y)
        {
            if (surf._changed) { surf.Update(); }
            _myBatch.Add(surf._cache, (float)x, (float)y);
            _changed = true;
        }

        [JSFunction(Name = "blitMaskSurface")]
        public void BlitMaskSurface(HWSurfaceInstance surf, int x, int y, ColorInstance mask)
        {
            if (surf._changed) { surf.Update(); }
            _myBatch.Add(surf._cache, (float)x, (float)y);
            _changed = true;
        }

        [JSFunction(Name = "blitImage")]
        public void BlitImage(int x, int y, ImageInstance instance)
        {
            _myBatch.Add(instance.Texture, (float)x, (float)y);
            _changed = true;
        }

        
        [JSFunction(Name = "setPixel")]
        public void SetPixel(int x, int y, ColorInstance color)
        {
            Vertex[] verts = new Vertex[1] { new Vertex(new Vector2f(x, y), color.GetColor()) };
            _myBatch.AddVerts(verts, 1, PrimitiveType.Points);
            _changed = true;
        }

        [JSFunction(Name = "replaceColor")]
        public void ReplaceColor(ColorInstance colorA, ColorInstance colorB)
        {
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
            _changed = true;
        }

        [JSFunction(Name = "resize")]
        public HWSurfaceInstance Resize(int width, int height)
        {
            return new HWSurfaceInstance(Engine, width, height, Color.White); // todo
        }

        [JSFunction(Name = "rescale")]
        public HWSurfaceInstance Rescale(int width, int height)
        {
            return new HWSurfaceInstance(Engine, width, height, Color.White); // todo;
        }

        [JSFunction(Name = "getPixel")]
        public ColorInstance GetPixel(int x, int y)
        {
            return new ColorInstance(Program._engine, Color.White);// GetColorAt(x, y));
        }

        [JSFunction(Name = "flipHorizontally")]
        public void FlipHorizontally()
        {
            // todo:  use viewport to flip.
            _changed = true;
        }

        [JSFunction(Name = "flipVertically")]
        public void FlipVertically()
        {
            // todo:  use viewport to flip.
            _changed = true;
        }

        [JSFunction(Name = "createImage")]
        public ImageInstance CreateImage()
        {
            if (_changed) Update();
            return new ImageInstance(Program._engine, _tex.Texture);
        }

        [JSFunction(Name = "save")]
        public void Save(string filename)
        {
            using (Image img = _tex.Texture.CopyToImage())
            {
                img.SaveToFile(Program.ParseSpherePath(filename, "images"));
            }
        }

        [JSFunction(Name = "clone")]
        public HWSurfaceInstance Clone()
        {
            if (_changed) Update();
            /*using (Image copy = _tex.Texture.CopyToImage())
            {
                return new HWSurfaceInstance(Engine, );
            }*/

            return new HWSurfaceInstance(Engine, (int)_width, (int)_height, Color.White);
        }

        [JSFunction(Name = "setBlendMode")]
        public void SetBlendMode(int mode)
        {
            _mode = (BlendModes)mode;
            switch (_mode)
            {
                case BlendModes.Blend:
                    _states.BlendMode = BlendMode.Alpha;
                    break;
                case BlendModes.Replace:
                    _states.BlendMode = BlendMode.None;
                    break;
                case BlendModes.Add:
                    _states.BlendMode = BlendMode.Add;
                    break;
                case BlendModes.Multiply:
                    _states.BlendMode = BlendMode.Multiply;
                    break;
            }
            _myBatch.Flush();
            _myBatch.SetBlendMode(_states.BlendMode);
        }

        [JSFunction(Name = "pointSeries")]
        public void PointSeries(ArrayInstance array, ColorInstance color)
        {
            Color c = color.GetColor();
            Vertex[] verts = new Vertex[array.Length];
            for (var i = 0; i < array.Length; ++i)
            {
                Vector2f vect = GlobalPrimitives.GetVector(array[i] as ObjectInstance);
                verts[i] = new Vertex(vect, c);
            }
            _myBatch.AddVerts(verts, verts.Length, PrimitiveType.Points);
            _changed = true;
        }

        [JSFunction(Name = "line")]
        public void Line(int x1, int y1, int x2, int y2, ColorInstance color)
        {
            Color col = color.GetColor();
            Vertex[] _verts = new Vertex[2];
            _verts[0] = new Vertex(new Vector2f(x1, y1), col);
            _verts[1] = new Vertex(new Vector2f(x2, y2), col);
            _myBatch.AddVerts(_verts, 2, PrimitiveType.Lines);
            _changed = true;

        }

        [JSFunction(Name = "lineSeries")]
        public void LineSeries(ArrayInstance points, ColorInstance color)
        {
            _changed = true;
        }

        [JSFunction(Name = "gradientLine")]
        public void GradientLine(int x1, int y1, int x2, int y2, ColorInstance col1, ColorInstance col2)
        {
            Vertex[] _verts = new Vertex[2];
            _verts[0] = new Vertex(new Vector2f(x1, y1), col1.GetColor());
            _verts[1] = new Vertex(new Vector2f(x2, y2), col2.GetColor());
            _myBatch.AddVerts(_verts, 2, PrimitiveType.Lines);
            _changed = true;
        }

        private Vertex[] _verts = new Vertex[4];

        private void FillVert(int n, int x, int y, Color c)
        {
            _verts[n].Position = new Vector2f(x, y);
            _verts[n].Color = c;
        }

        private void FillVert(int n, int x, int y, ref Color c)
        {
            _verts[n].Position = new Vector2f(x, y);
            _verts[n].Color = c;
        }

        [JSFunction(Name = "rectangle")]
        public void Rectangle(int x, int y, int w, int h, ColorInstance color)
        {
            Color col = color.Color;
            FillVert(0, x, y, ref col);
            FillVert(1, x + w, y, ref col);
            FillVert(2, x + w, y + h, ref col);
            FillVert(3, x, y + h, ref col);
            _myBatch.AddVerts(_verts, 4);
            _changed = true;
        }

        [JSFunction(Name = "gradientRectangle")]
        public void GradientRectangle(int x, int y, int w, int h, ColorInstance c1, ColorInstance c2, ColorInstance c3, ColorInstance c4)
        {
            FillVert(0, x, y, c1.Color);
            FillVert(1, x + w, y, c2.Color);
            FillVert(2, x + w, y + h, c3.Color);
            FillVert(3, x, y + h, c4.Color);
            _myBatch.AddVerts(_verts, 4);
            _changed = true;
        }

        [JSFunction(Name = "outlinedRectangle")]
        public void OutlinedRectangle(int x, int y, int w, int h, ColorInstance color)
        {
            _changed = true;
        }

        [JSFunction(Name = "filledCircle")]
        public void FilledCircle(int ox, int oy, int r, ColorInstance color, [DefaultParameterValue(false)] bool antialias = false)
        {
            _changed = true;
        }

        [JSFunction(Name = "outlinedCircle")]
        public void OutlinedCircle(int ox, int oy, int r, ColorInstance color, [DefaultParameterValue(false)] bool antialias = false)
        {
            _changed = true;
        }

        [JSFunction(Name = "drawText")]
        public void DrawText(FontInstance font, int x, int y, string text)
        {
            font.DrawText(_myBatch, x, y, text);
            _changed = true;
        }

        [JSFunction(Name = "setAlpha")]
        public void SetAlpha(int v)
        {
            /* noop */
        }

        [JSFunction(Name = "cloneSection")]
        public HWSurfaceInstance CloneSection(int ox, int oy, int w, int h)
        {
            /*todo: */
            return new HWSurfaceInstance(Engine, w, h, Color.White);
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object surface]";
        }
    }
}

