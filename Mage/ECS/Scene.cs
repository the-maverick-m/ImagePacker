using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mage
{
    public abstract class Scene : IDisposable
    {
        public Engine Engine { get; }

        public List<Layer> Layers { get; set; } = new List<Layer>();

        private bool _disposed;

        public Scene(Engine engine)
        {
            Engine = engine;
        }

        public virtual void Update(float delta)
        {
            foreach (var layer in Layers)
                layer.Update(delta);
        }

        public virtual void Load() { }

        public virtual void Unload() { }

        public abstract void Draw(Renderer renderer);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                foreach (var layer in Layers)
                    layer.Dispose();

                _disposed = true;
            }
        }

        ~Scene()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}