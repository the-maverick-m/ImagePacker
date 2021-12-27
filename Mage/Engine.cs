using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Mage
{
    public abstract class Engine : Game
    {
        public GraphicsDeviceManager GraphicsDeviceManager => _graphics;

        public RenderTarget2D RenderTarget { get; private set; }

        // Queue the scene so that it starts to be used on the next frame
        // This is to prevent a scene being drawn before being updated
        private Scene _scene;
        private Scene _queuedScene;

        public Scene Scene
        {
            get => _scene;
            set => _queuedScene = value;
        }

        private GraphicsDeviceManager _graphics;
        private Renderer _renderer;

        private const float TargetAspectRatio = 16f / 9f;
        public Rectangle TargetBounds { get; private set; }
        public Point Border { get; private set; }


        // Used for clearing the game render target
        private readonly Color[] _renderTargetClear;

        public Engine()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;

            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            IsFixedTimeStep = true;
#if !DEBUG
            IsMouseVisible = false;
            _graphics.IsFullScreen = true;
#endif

            _renderTargetClear = new Color[1920 * 1080];
            for (int i = 0; i < 1920 * 1080; i++)
            {
                _renderTargetClear[i] = Color.Transparent;
            }
        }

        protected void CalculateTargetBounds()
        {
            if (GraphicsDevice.Viewport.AspectRatio == TargetAspectRatio)
            {
                TargetBounds = GraphicsDevice.Viewport.Bounds;
                Border = Point.Zero;
            }
            // Window is too narrow
            else if (GraphicsDevice.Viewport.AspectRatio < TargetAspectRatio)
            {
                int finalHeight = (int) (GraphicsDevice.Viewport.Width / TargetAspectRatio);
                Border = new Point(0, (int) ((GraphicsDevice.Viewport.Height / 2) - (finalHeight / 2)));
                TargetBounds = new Rectangle(0, (int) Border.Y,
                    GraphicsDevice.Viewport.Width, finalHeight);
            }
            // Window is too wide
            else
            {
                int finalWidth = (int) (GraphicsDevice.Viewport.Height * TargetAspectRatio);
                Border = new Point((int) ((GraphicsDevice.Viewport.Width / 2) - (finalWidth / 2)), 0);
                TargetBounds = new Rectangle((int) Border.X, 0,
                    finalWidth, GraphicsDevice.Viewport.Height);
            }
        }

        protected override void Initialize()
        {
            SaveData.Initialize(GetType().Name.Replace("Game", ""));

            Window.Title = GetType().Name.Replace("Game", "");
            Window.ClientSizeChanged += (sender, e) =>
            {
                if (Window.ClientBounds.Width < 100) _graphics.PreferredBackBufferWidth = 100;
                if (Window.ClientBounds.Height < 100) _graphics.PreferredBackBufferHeight = 100;
                _graphics.ApplyChanges();

                CalculateTargetBounds();
            };
            Input.Initialize(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GraphicsDevice.DiscardColor = Color.Transparent;
            CalculateTargetBounds();

            Graphics.Initialize(GraphicsDevice);

            // extra parameters are same as default except for preserving contents
            RenderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080, false,
                SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            _renderer = new Renderer(this);
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_scene != _queuedScene)
            {
                _scene?.Unload();
                _scene?.Dispose();
                _scene = _queuedScene;
                GC.Collect();
                _queuedScene.Load();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            float delta = (float) gameTime.ElapsedGameTime.TotalSeconds;
            Input.Poll();
            Scene?.Update(delta);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear game target
            RenderTarget.SetData(_renderTargetClear);
            GraphicsDevice.SetRenderTarget(RenderTarget);

            Scene?.Draw(_renderer);
            GraphicsDevice.SetRenderTarget(null);

            _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            _renderer.SpriteBatch.Draw(RenderTarget, TargetBounds, null, Color.White);
            _renderer.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}