using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Engine
{
    class BloomTest
    {
        Shader _bloom;
        RenderTexture _texture;
        RenderStates _states;
        Vertex[] _verts;
        RenderTarget _target;

        public BloomTest(RenderTexture target_tex, RenderTarget target)
        {
            _target = target;

            _bloom = new Shader(null, "bloom.glsl");
            _texture = new RenderTexture((uint)GlobalProps.Width, (uint)GlobalProps.Height);
            _states.BlendMode = BlendMode.Alpha;
            _states.Shader = _bloom;
            _states.Transform = Transform.Identity;
            _states.Texture = target_tex.Texture;
            _bloom.SetParameter("referenceTex", Shader.CurrentTexture);
            _bloom.SetParameter("pixelWidth", 4);
            _bloom.SetParameter("pixelHeight", 4);

            int w = GlobalProps.Width, h = GlobalProps.Height;
            Vector2f v0 = new Vector2f(0, 0);
            Vector2f v1 = new Vector2f(w, 0);
            Vector2f v2 = new Vector2f(w, h);
            Vector2f v3 = new Vector2f(0, h);

            _verts = new Vertex[4];
            _verts[0] = new Vertex(v0, Color.White, v0);
            _verts[1] = new Vertex(v1, Color.White, v1);
            _verts[2] = new Vertex(v2, Color.White, v2);
            _verts[3] = new Vertex(v3, Color.White, v3);
        }

        public void Draw()
        {
            _target.Draw(_verts, PrimitiveType.Quads, _states);
        }
    }
}
