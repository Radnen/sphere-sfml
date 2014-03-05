using System;
using System.Collections.Generic;
using SFML.Graphics;
using Engine.Objects;

namespace Engine
{
    public static class AssetManager
    {
        private static Dictionary<string, SpritesetInstance> _spritesets;
        private static Dictionary<string, Texture> _textures;

        static AssetManager()
        {
            _spritesets = new Dictionary<string, SpritesetInstance>();
            _textures = new Dictionary<string, Texture>();
        }

        public static SpritesetInstance GetSpriteset(string filename, bool clone = false)
        {
            if (!_spritesets.ContainsKey(filename))
            {
                string path = Program.ParseSpherePath(filename, "spritesets");
                _spritesets[filename] = new SpritesetInstance(Program._engine, path);
            }

            if (clone)
                return _spritesets[filename].Clone();
            else
                return _spritesets[filename];
        }

        public static Texture GetTexture(string filename)
        {
            if (!_textures.ContainsKey(filename))
            {
                Texture baseTex = new Texture(filename);
                baseTex.Smooth = GlobalProps.SmoothTextures;
                _textures[filename] = baseTex;
            }

            return _textures[filename];
        }
    }
}

