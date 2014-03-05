using System;
using SFML.Graphics;

namespace Engine.Objects
{
    /// <summary>
    /// Wraps a texture atlas to perform fast hardware-accelerated
    /// updates on the source image.
    /// </summary>
    public class FastTextureAtlas
    {
        public RenderTexture RenderTexture { get; private set; }
        public TextureAtlas TextureAtlas { get; private set; }

        public IntRect[] Sources
        {
            get
            {
                return TextureAtlas.Sources;
            }
        }

        private Sprite _atlas;
        private bool _modified = false;
        private RenderStates _replace;

        public FastTextureAtlas(TextureAtlas atlas)
        {
            TextureAtlas = atlas;
            _atlas = new Sprite(TextureAtlas.Texture);
            RenderTexture = new RenderTexture(atlas.Size, atlas.Size);
            RenderTexture.Draw(_atlas);
            RenderTexture.Display();
            _replace = new RenderStates(BlendMode.None, Transform.Identity, null, null);
        }

        public void SetImageAt(uint dest, uint source)
        {
            IntRect sourceRect = Sources[source];
            IntRect destRect = Sources[dest];
            _atlas.Position = new SFML.Window.Vector2f(destRect.Left, destRect.Top);
            _atlas.TextureRect = sourceRect;
            RenderTexture.Draw(_atlas, _replace);
            _modified = true;
        }

        public void Line(int x1, int y1, int x2, int y2)
        {
            Vertex[] verts = new Vertex[2];
            verts[0] = new Vertex(new SFML.Window.Vector2f(x1, y1), Color.Red);
            verts[1] = new Vertex(new SFML.Window.Vector2f(x2, y2), Color.Red);
            RenderTexture.Draw(verts, PrimitiveType.Lines);
        }

        public void Line(uint dest, Line l)
        {
            IntRect source = Sources[dest];
            Line off = l.Offset(new SFML.Window.Vector2f(source.Left + 1, source.Top));
            Vertex[] verts = new Vertex[2];
            verts[0] = new Vertex(off.Start, Color.Magenta);
            verts[1] = new Vertex(off.End, Color.Magenta);
            RenderTexture.Draw(verts, PrimitiveType.Lines);
        }

        public void Refresh()
        {
            if (_modified)
            {
                RenderTexture.Display();
                _modified = false;
            }
        }
    }
}

