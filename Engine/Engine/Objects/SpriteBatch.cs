using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    public class SpriteBatch
    {
        private RenderTarget _target;

        private uint MAX = 200 * 4, _idx = 0;
        private RenderStates _states;
        private Vertex[] _array;
        private Vector2f ul, ur, lr, ll;
        private Texture _tex;
        private PrimitiveType _last = PrimitiveType.Quads;
        private float cos, sin;

        public SpriteBatch(RenderTarget target)
        {
            _states = new RenderStates(BlendMode.Alpha, Transform.Identity, null, null);
            _array = new Vertex[MAX];
            ul = new Vector2f();
            ur = new Vector2f();
            lr = new Vector2f();
            ll = new Vector2f();
            _target = target;
        }

        /// <summary>
        /// Sets the batcher to use the specified blend mode.
        /// </summary>
        public void SetBlendMode(BlendMode mode)
        {
            _states.BlendMode = mode;
        }

        /// <summary>
        /// Adds the texture to the batcher, drawn at the (x, y) location.
        /// </summary>
        public void Add(Texture tex, float x, float y)
        {
            Add(tex, x, y, Color.White);
        }

        /// <summary>
        /// Add a pre-transformed texture to the batcher.
        /// </summary>
        public void Add(Texture tex, Vertex[] transform)
        {
            if (_tex != tex) Flush(tex);

            _array[_idx + 0] = transform[0];
            _array[_idx + 1] = transform[1];
            _array[_idx + 2] = transform[2];
            _array[_idx + 3] = transform[3];
            _idx += 4;

            if (_idx == _array.Length) Flush();
        }

        /// <summary>
        /// Adds the texture to the batcher, drawn at the (x, y) location with the specified color.
        /// </summary>
        public void Add(Texture tex, float x, float y, Color color)
        {
            if (_tex != tex)
            {
                Flush(tex);
                ur.X = tex.Size.X;
                lr.X = tex.Size.X;
                lr.Y = tex.Size.Y;
                ll.Y = tex.Size.Y;
            }

            float lx = x + tex.Size.X;
            float ly = y + tex.Size.Y;

            _array[_idx + 0] = new Vertex(new Vector2f(x, y), color, ul);
            _array[_idx + 1] = new Vertex(new Vector2f(lx, y), color, ur);
            _array[_idx + 2] = new Vertex(new Vector2f(lx, ly), color, lr);
            _array[_idx + 3] = new Vertex(new Vector2f(x, ly), color, ll);
            _idx += 4;

            if (_idx == _array.Length) Flush();
        }

        /// <summary>
        /// Adds a clipped or stretched version of the texture to the batcher, with the specified color.
        /// </summary>
        public void Add(Texture tex, IntRect source, FloatRect dest, Color color)
        {
            if (tex != _tex) Flush(tex);

            float x = dest.Left + dest.Width;
            float y = dest.Top + dest.Height;
            int x2 = source.Left + source.Width;
            int y2 = source.Top + source.Height;

            _array[_idx + 0] = new Vertex(new Vector2f(dest.Left, dest.Top), color, new Vector2f(source.Left, source.Top));
            _array[_idx + 1] = new Vertex(new Vector2f(x, dest.Top), color, new Vector2f(x2, source.Top));
            _array[_idx + 2] = new Vertex(new Vector2f(x, y), color, new Vector2f(x2, y2));
            _array[_idx + 3] = new Vertex(new Vector2f(dest.Left, y), color, new Vector2f(source.Left, y2));
            _idx += 4;

            if (_idx == _array.Length) Flush();
        }

        /// <summary>
        /// Adds verts to the batcher, must be a quad (for now).
        /// </summary>
        public void AddVerts(Vertex[] verts, int count, PrimitiveType type = PrimitiveType.Quads)
        {
            if (_tex != null) { Flush(); _tex = null; _states.Texture = null; }
            if (_idx + count > _array.Length) { Flush(type); }
            if (type != _last) { Flush(_last); _last = type; }

            for (int i = 0; i < count; ++i) _array[_idx + i] = verts[i];
            _idx += (uint)count;
        }

        /// <summary>
        /// Adds a rotated image to the batcher.
        /// </summary>
        public void Add(Texture tex, float x, float y, Color color, double r)
        {
            if (_tex != tex)
            {
                Flush(tex);
                ur.X = tex.Size.X;
                lr.X = tex.Size.X;
                lr.Y = tex.Size.Y;
                ll.Y = tex.Size.Y;
            }

            cos = (float)Math.Cos(r);
            sin = (float)Math.Sin(r);

            float w = tex.Size.X / 2;
            float h = tex.Size.Y / 2;
            float wx = x + w;
            float wy = y + h;

            _array[_idx + 0] = new Vertex(new Vector2f(wx + RotateX(-w, -h), wy + RotateY(-w, -h)), color, ul);
            _array[_idx + 1] = new Vertex(new Vector2f(wx + RotateX(w, -h), wy + RotateY(w, -h)), color, ur);
            _array[_idx + 2] = new Vertex(new Vector2f(wx + RotateX(w, h), wy + RotateY(w, h)), color, lr);
            _array[_idx + 3] = new Vertex(new Vector2f(wx + RotateX(-w, h), wy + RotateY(-w, h)), color, ll);
            _idx += 4;

            if (_idx == _array.Length) Flush();
        }

        private float RotateX(float x, float y)
        {
            return (x * cos) - (y * sin);
        }

        private float RotateY(float x, float y)
        {
            return (x * sin) + (y * cos);
        }

        /// <summary>
        /// Renders it all to screen, clearing out it's current buffer.
        /// </summary>
        public void Flush()
        {
            _target.Draw(_array, 0, _idx, _last, _states);
            _idx = 0;
        }

        public void Flush(PrimitiveType type)
        {
            _target.Draw(_array, 0, _idx, type, _states);
            _idx = 0;
        }

        private void Flush(Texture replace)
        {
            _target.Draw(_array, 0, _idx, _last, _states);
            _idx = 0;
            _tex = replace;
            _states.Texture = replace;
            _last = PrimitiveType.Quads;
        }
    }
}
