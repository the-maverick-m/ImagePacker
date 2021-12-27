using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mage
{
    public class Layer : IDisposable
    {
        public Tracker Tracker { get; set; } = new Tracker();

        public Scene Scene { get; }

        public Camera Camera { get; set; }

        public RenderTarget2D RenderTarget { get; }

        private bool _disposed;

        public Layer(Scene scene, int width, int height)
        {
            Scene = scene;
            scene.Layers.Add(this);
            Camera = new Camera(this, width, height);
            Camera.Position = new Vector2(width / 2, height / 2);

            RenderTarget = new RenderTarget2D(Scene.Engine.GraphicsDevice, width, height);
        }

        public void Add(Entity entity)
        {
            Tracker.Add(entity);
        }

        public void Remove(Entity entity)
        {
            Tracker.Remove(entity);
        }

        public void Update(float delta)
        {
            foreach (var key in Tracker.Entities.Keys)
                foreach (var entity in Tracker.Entities[key])
                    entity.Update(delta);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                RenderTarget.Dispose();
                _disposed = true;
            }
        }

        ~Layer()
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