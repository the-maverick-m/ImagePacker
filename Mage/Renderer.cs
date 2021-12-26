using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mage
{
    public class Renderer
    {
        private readonly SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch => _spriteBatch;
        public Engine Engine { get; }

        private RenderTarget2D _renderTarget;

        public Renderer(Engine engine)
        {
            Engine = engine;
            _spriteBatch = new SpriteBatch(engine.GraphicsDevice);
        }

        public void SetRenderTarget(RenderTarget2D renderTarget)
        {
            _renderTarget = renderTarget;
        }

        public RenderTarget2D GetRenderTarget()
        {
            return _renderTarget;
        }

        // Much of this code is copied from other parts of this class
        // If changes are made elsewhere, change them here as well
        public void DrawLayer(Layer layer, SamplerState samplerState,
        Color color, Effect effect, bool scaleResolution)
        {
            if (layer.Tracker.Entities.Count == 0) return;

            Engine.GraphicsDevice.SetRenderTarget(layer.RenderTarget);

            foreach (var depth in layer.Tracker.Entities.Keys)
            {
                Matrix matrix = scaleResolution ?
                layer.Camera.GetTranslation(depth) * layer.Camera.GetOffset() :
                layer.Camera.GetFullTransform(depth);

                effect?.Parameters["MatrixTransform"].SetValue(matrix *
                Matrix.CreateOrthographicOffCenter(0, layer.RenderTarget.Width,
                 layer.RenderTarget.Height, 0, 0, -1));

                _spriteBatch.Begin(SpriteSortMode.Deferred, null, samplerState, null,
                null, effect, matrix);

                foreach (var entity in layer.Tracker.Entities[depth])
                {
                    entity.Draw(this);
                }

                _spriteBatch.End();
            }

            Engine.GraphicsDevice.SetRenderTarget(_renderTarget ?? Engine.RenderTarget);
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, samplerState);

            if (scaleResolution)
            {
                Vector2 zoomOffset = new Vector2(
                    layer.RenderTarget.Width / (2 * layer.Camera.Zoom),
                 layer.RenderTarget.Height / (2 * layer.Camera.Zoom));

                Rectangle srcRect = new Rectangle(
                    (int)(layer.RenderTarget.Width / 2 - zoomOffset.X),
                    (int)(layer.RenderTarget.Height / 2 - zoomOffset.Y),
                    (int)zoomOffset.X * 2,
                    (int)zoomOffset.Y * 2
                );

                _spriteBatch.Draw(layer.RenderTarget, Engine.RenderTarget.Bounds, srcRect, color);
            }
            else
            {
                _spriteBatch.Draw(layer.RenderTarget, Engine.RenderTarget.Bounds, null, color);
            }

            _spriteBatch.End();

            // Did we use a custom render target?
            if (_renderTarget != null)
                Engine.GraphicsDevice.SetRenderTarget(Engine.RenderTarget);
        }

        public void Draw(Atlas atlas, string name, AABB aabb, Color color)
        {
            if (atlas[name] == null)
                _spriteBatch.DrawRect(aabb.ToRectangle(), color);
            else
                _spriteBatch.Draw(atlas.Texture, aabb.ToRectangle(), atlas[name].Bounds, color);
        }

        public void Draw(Atlas atlas, string name, Rectangle rectangle, Color color)
        {
            if (atlas[name] == null)
                _spriteBatch.DrawHollowRect(rectangle, color);
            else
                _spriteBatch.Draw(atlas.Texture, rectangle, atlas[name].Bounds, color);
        }

        public void DrawRect(Rectangle rectangle, Color color)
        {
            _spriteBatch.DrawRect(rectangle, color);
        }

        public void DrawHollowRect(Rectangle rectangle, Color color)
        {
            _spriteBatch.DrawHollowRect(rectangle, color);
        }

        public void DrawPoint(Vector2 position, Color color)
        {
            _spriteBatch.DrawPoint(position, color);
        }

        public void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            _spriteBatch.DrawLine(start, end, color);
        }
    }
}