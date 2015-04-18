using System;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class HWSurfaceInstance : ObjectInstance
    {
        private bool _changed = true;
        private BlendModes _mode;
        private RenderStates _states;
        private FloatRect _source;

        private static Texture _atlastex;
        private static Image _atlasimg;
        private static RenderTexture _atlas;
        private static SpriteBatch _batch;
        private static float _ox = 0;
        private static float _oy = 0;
        private static uint _aw = 1024;
        private static uint _ah = 1024;

        static HWSurfaceInstance()
        {
            _atlas = new RenderTexture(_aw, _ah);
            _atlastex = _atlas.Texture;
            _batch = new SpriteBatch(_atlas);
            _batch.SetBlendMode(BlendMode.None);
        }

        public HWSurfaceInstance(ScriptEngine parent, Texture texture)
            : base(parent.Object.InstancePrototype)
        {
            _source = new FloatRect(_ox, _oy, texture.Size.X, texture.Size.Y);
            _ox += _source.Width + 1;
            if (_ox > _aw) { _ox = 0; _oy += _source.Height + 1; }
            Init();
        }

        public HWSurfaceInstance(ScriptEngine parent, int width, int height, Color bg_color)
            : this(parent, new Texture((uint)width, (uint)height))
        {
            _atlas.Clear(bg_color);
            Update();
        }

        public HWSurfaceInstance(ScriptEngine parent, string filename)
            : this(parent, new Texture(filename))
        {
        }

        public HWSurfaceInstance(ScriptEngine parent, Image image)
            : this(parent, new Texture(image))
        {
        }

        private void Init() {
            PopulateFunctions();
            SetBlendMode((int)BlendModes.Blend);
            DefineProperty("width", new PropertyDescriptor((int)_source.Width, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor((int)_source.Height, PropertyAttributes.Sealed), true);
        }

        public void Update()
        {
            _batch.Flush();
            _atlas.Display();
            _changed = false;
        }

        [JSFunction(Name = "blit")]
        public void Blit(double x, double y)
        {
            if (_changed) Update();
            FloatRect dest = new FloatRect((float)x, (float)y, _source.Width, _source.Height);
            Program.Batch.Add(_atlastex, _source, dest, Color.White);
        }

        [JSFunction(Name = "blitSurface")]
        public void BlitSurface(HWSurfaceInstance surf, int x, int y)
        {
            if (surf == null) return;
            if (surf._changed) { surf.Update(); }

            _batch.Add(_atlastex, surf._source, _source, Color.White);
            _changed = true;
        }

        [JSFunction(Name = "blitMaskSurface")]
        public void BlitMaskSurface(HWSurfaceInstance surf, int x, int y, ColorInstance mask)
        {
            if (surf == null) return;
            if (surf._changed) { surf.Update(); }

            FloatRect dest = new FloatRect(x, y, _source.Width, _source.Height);
            _batch.Add(_atlastex, surf._source, dest, mask.Color);
            
            _changed = true;
        }

        [JSFunction(Name = "blitImage")]
        public void BlitImage(int x, int y, ImageInstance instance)
        {
            FloatRect dest = new FloatRect(x, y, _source.Width, _source.Height);
            _batch.Add(instance.Texture, _source.Left + x, _source.Top + y, Color.White);
            _changed = true;
        }

        
        [JSFunction(Name = "setPixel")]
        public void SetPixel(int x, int y, ColorInstance color)
        {
            Vertex[] verts = new Vertex[1] { new Vertex(new Vector2f(x, y), color.GetColor()) };
            _batch.AddVerts(verts, 1, PrimitiveType.Points);
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
            Image img = _atlastex.CopyToImage();
            IntRect rect = new IntRect((int)_source.Left, (int)_source.Top, (int)_source.Width, (int)_source.Height);
            Texture tex = new Texture(img, rect);
            return new ImageInstance(Program._engine, tex, false);
        }

        [JSFunction(Name = "save")]
        public void Save(string filename)
        {
            /*if (_changed)
            {
                Update();
                _atlasimg = _atlastex.CopyToImage();
            }

            using (Image img = new Image(_atlasimg, ))
            {
                img.SaveToFile(Program.ParseSpherePath(filename, "images"));
            }*/
        }

        [JSFunction(Name = "clone")]
        public HWSurfaceInstance Clone()
        {
            if (_changed) Update();
            /*using (Image copy = _tex.Texture.CopyToImage())
            {
                return new HWSurfaceInstance(Engine, );
            }*/

            return new HWSurfaceInstance(Engine, (int)_source.Width, (int)_source.Height, Color.White);
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
            _batch.Flush();
            _batch.SetBlendMode(_states.BlendMode);
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
            _batch.AddVerts(verts, verts.Length, PrimitiveType.Points);
            _changed = true;
        }

        [JSFunction(Name = "line")]
        public void Line(int x1, int y1, int x2, int y2, ColorInstance color)
        {
            Color col = color.GetColor();
            Vertex[] _verts = new Vertex[2];
            _verts[0] = new Vertex(new Vector2f(x1, y1), col);
            _verts[1] = new Vertex(new Vector2f(x2, y2), col);
            _batch.AddVerts(_verts, 2, PrimitiveType.Lines);
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
            _batch.AddVerts(_verts, 2, PrimitiveType.Lines);
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
            _batch.AddVerts(_verts, 4);
            _changed = true;
        }

        [JSFunction(Name = "gradientRectangle")]
        public void GradientRectangle(int x, int y, int w, int h, ColorInstance c1, ColorInstance c2, ColorInstance c3, ColorInstance c4)
        {
            FillVert(0, x, y, c1.Color);
            FillVert(1, x + w, y, c2.Color);
            FillVert(2, x + w, y + h, c3.Color);
            FillVert(3, x, y + h, c4.Color);
            _batch.AddVerts(_verts, 4);
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
            font.DrawText(_batch, x, y, text);
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

