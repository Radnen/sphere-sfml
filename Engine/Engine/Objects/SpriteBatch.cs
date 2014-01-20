using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Engine.Objects
{
    class SpriteBatch
    {
        Dictionary<Texture, VertexArray> _batch = new Dictionary<Texture, VertexArray>();

        public void Render()
        {
            foreach (KeyValuePair<Texture, VertexArray> pair in _batch)
            {
                RenderStates states = new RenderStates(pair.Key);
                Program._window.Draw(pair.Value, states);
                pair.Value.Clear();
            }
        }

        public void Add(Vector2f location, Texture tex, Color color, double rotation)
        {
            if (!_batch.ContainsKey(tex)) _batch[tex] = new VertexArray(PrimitiveType.Quads);
            VertexArray array = _batch[tex];

            array.Append(new Vertex(location, color, new Vector2f()));
            array.Append(new Vertex(new Vector2f(location.X + tex.Size.X, location.Y), color, new Vector2f(tex.Size.X, 0)));
            array.Append(new Vertex(new Vector2f(location.X + tex.Size.X, location.Y + tex.Size.Y), color, new Vector2f(tex.Size.X, tex.Size.Y)));
            array.Append(new Vertex(new Vector2f(location.X, location.Y + tex.Size.Y), color, new Vector2f(0, tex.Size.Y)));
        }


    }
}
