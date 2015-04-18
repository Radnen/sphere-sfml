using System;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.System;

namespace Engine.Objects
{
    public class ImageInstance : ObjectInstance
    {
        private Texture _image;
        private IntRect _source = new IntRect(0, 0, 0, 0);

        public Texture Texture { get { return _image; } }

        public ImageInstance(ScriptEngine parent, string filename)
            : base(parent.Object.InstancePrototype)
        {
            _image = AssetManager.GetTexture(filename);
            Init();
        }

        public ImageInstance(ScriptEngine parent, Texture copy, bool clone = true)
            : base(parent.Object.InstancePrototype)
        {
            _image = (clone) ? new Texture(copy) : copy;
            Init();
        }

        private void Init() {
            PopulateFunctions();

            DefineProperty("width", new PropertyDescriptor(_image.Size.X, PropertyAttributes.Sealed), true);
            DefineProperty("height", new PropertyDescriptor(_image.Size.Y, PropertyAttributes.Sealed), true);
            _source.Width = (int)_image.Size.X;
            _source.Height = (int)_image.Size.Y;
        }

        private bool Visible(double x, double y)
        {
            return (x < GlobalProps.Width && y < GlobalProps.Height &&
                x + _source.Width > 0 && y + _source.Height > 0);
        }

        public Image GetImage()
        {
            return _image.CopyToImage();
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object image]";
        }

        [JSFunction(Name = "save")]
        public void Save(string filename)
        {
            _image.CopyToImage().SaveToFile(GlobalProps.BasePath + "\\images\\" + filename);
        }

        [JSFunction(Name = "blit")]
        public void Blit(double x, double y, [DefaultParameterValue(0)] int mode = 0)
        {
            if (!Visible(x, y)) return;
            Program.Batch.Add(_image, (float)x, (float)y);
        }

        [JSFunction(Name = "blitMask")]
        public void BlitMask(double x, double y, ColorInstance color)
        {
            if (!Visible(x, y)) return;
            Program.Batch.Add(_image, (float)x, (float)y, color.Color);
        }

        [JSFunction(Name = "zoomBlit")]
        public void ZoomBlit(double x, double y, double z) {
            if (!Visible(x, y)) return;

            float wz = (float)(_source.Width * z);
            float hz = (float)(_source.Height * z);

            Program.Batch.Add(_image, _source, new FloatRect((float)x, (float)y, wz, hz), Color.White);
        }

        [JSFunction(Name = "zoomBlitMask")]
        public void ZoomBlitMask(double x, double y, double z, ColorInstance color) {
            if (!Visible(x, y)) return;

            float wz = (float)(_source.Width * z);
            float hz = (float)(_source.Height * z);

            Program.Batch.Add(_image, _source, new FloatRect((float)x, (float)y, wz, hz), color.GetColor());
        }

        [JSFunction(Name = "rotateBlit")]
        public void RotateBlit(double x, double y, double r) {
            if (!Visible(x, y)) return;
            Program.Batch.Add(_image, (float)x, (float)y, Color.White, r);
        }

        [JSFunction(Name = "rotateBlitMask")]
        public void RotateBlitMask(double x, double y, double r, ColorInstance color) {
            if (!Visible(x, y)) return;
            Program.Batch.Add(_image, (float)x, (float)y, color.Color, r);
        }

        [JSFunction(Name = "transformBlit")]
        public void TransformBlit(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            float w = (float)_image.Size.X, h = (float)_image.Size.Y;
            Vertex[] array = new Vertex[4];

            array[0] = new Vertex(new Vector2f((float)x1, (float)y1), new Vector2f(0, 0));
            array[1] = new Vertex(new Vector2f((float)x2, (float)y2), new Vector2f(w, 0));
            array[2] = new Vertex(new Vector2f((float)x3, (float)y3), new Vector2f(w, h));
            array[3] = new Vertex(new Vector2f((float)x4, (float)y4), new Vector2f(0, h));

            Program.Batch.Add(_image, array);
        }

        [JSFunction(Name = "transformBlitMask")]
        public void TransformBlitMask(ObjectInstance ul, ObjectInstance ur, ObjectInstance lr, ObjectInstance ll, ColorInstance color)
        {
            float w = (float)_image.Size.X, h = (float)_image.Size.Y;
            Vertex[] array = new Vertex[4];
            Color c = color.GetColor();

            array[0] = new Vertex(GlobalPrimitives.GetVector(ul), c, new Vector2f(0, 0));
            array[1] = new Vertex(GlobalPrimitives.GetVector(ur), c, new Vector2f(w, 0));
            array[2] = new Vertex(GlobalPrimitives.GetVector(lr), c, new Vector2f(w, h));
            array[3] = new Vertex(GlobalPrimitives.GetVector(ll), c, new Vector2f(0, h));

            Program.Batch.Add(_image, array);
        }

        [JSFunction(Name = "wrapBlit")]
        public void WrapBlit(int x1, int y1, int x2, int y2, [DefaultParameterValue(-1)] int width = -1, [DefaultParameterValue(-1)] int height = -1)
        {
            float w = width < 0 ? (float)_image.Size.X : width;
            float h = height < 0 ? (float)_image.Size.Y : height;

            Vertex[] array = new Vertex[4];
            _image.Repeated = true;

            array[0] = new Vertex(new Vector2f(x1, y1), new Vector2f(x2, y2));
            array[1] = new Vertex(new Vector2f(x1 + w, y1), new Vector2f(x2 + w, y2));
            array[2] = new Vertex(new Vector2f(x1 + w, y1 + h), new Vector2f(x2 + w, y2 + h));
            array[3] = new Vertex(new Vector2f(x1, y1 + h), new Vector2f(x2, y2 + h));

            Program.Batch.Add(_image, array);
        }

        [JSFunction(Name = "wrapBlitMask")]
        public void WrapBlitMask(int x1, int y1, int x2, int y2, ColorInstance col, [DefaultParameterValue(-1)] int width = -1, [DefaultParameterValue(-1)] int height = -1)
        {
            Color c = col.Color;
            float w = width < 0 ? (float)_image.Size.X : width;
            float h = height < 0 ? (float)_image.Size.Y : height;

            Vertex[] array = new Vertex[4];
            _image.Repeated = true;

            array[0] = new Vertex(new Vector2f(x1, y1), c, new Vector2f(x2, y2));
            array[1] = new Vertex(new Vector2f(x1 + w, y1), c, new Vector2f(x2 + w, y2));
            array[2] = new Vertex(new Vector2f(x1 + w, y1 + h), c, new Vector2f(x2 + w, y2 + h));
            array[3] = new Vertex(new Vector2f(x1, y1 + h), c, new Vector2f(x2, y2 + h));

            Program.Batch.Add(_image, array);
        }

        [JSFunction(Name = "createSurface")]
        public SurfaceInstance CreateSurface()
        {
            using (Image img = _image.CopyToImage())
            {
                return new SurfaceInstance(Program._engine, img.Pixels, (int)img.Size.X, (int)img.Size.Y);
            }
        }

        [JSFunction(Name = "clone")]
        public ImageInstance Clone()
        {
            return new ImageInstance(Engine, _image);
        }
    }
}
