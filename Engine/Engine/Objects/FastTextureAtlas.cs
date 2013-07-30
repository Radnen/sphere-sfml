using System;
using SFML.Graphics;

namespace Engine
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
            RenderTexture = new RenderTexture(atlas._size, atlas._size);

            foreach (IntRect r in atlas.Sources)
            {
                _atlas.TextureRect = r;
                _atlas.Position = new SFML.Window.Vector2f(r.Left, r.Top);
                RenderTexture.Draw(_atlas);
            }

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

        public void Update()
        {
            if (_modified)
            {
                RenderTexture.Display();
                _modified = false;
            }
        }
    }
}

