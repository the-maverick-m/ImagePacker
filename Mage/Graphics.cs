using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
// using FontStashSharp;
using System.IO;

namespace Mage
{
    public static class Graphics
    {
        public static Texture2D BlankTex { get; private set; }
        // public static FontSystem FontSystem { get; private set; }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            BlankTex = new Texture2D(graphicsDevice, 1, 1);
            BlankTex.SetData(new Color[] { Color.White });

            // FontSystem = new FontSystem();
            // FontSystem.AddFont(File.ReadAllBytes(Resources.GetPath("Default.ttf")));
        }

        public static void DrawRect(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(BlankTex, rectangle, null, color);
        }

        public static void DrawHollowRect(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            spriteBatch.DrawLine(new Vector2(rectangle.Left, rectangle.Top),
             new Vector2(rectangle.Right, rectangle.Top), color);
            spriteBatch.DrawLine(new Vector2(rectangle.Right, rectangle.Top),
             new Vector2(rectangle.Right, rectangle.Bottom), color);
            spriteBatch.DrawLine(new Vector2(rectangle.Right, rectangle.Bottom),
             new Vector2(rectangle.Left, rectangle.Bottom), color);
            spriteBatch.DrawLine(new Vector2(rectangle.Left, rectangle.Bottom),
             new Vector2(rectangle.Left, rectangle.Top), color);
        }

        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            spriteBatch.Draw(BlankTex, new Rectangle((int)position.X, (int)position.Y, 1, 1), null, color);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            Vector2 difference = end - start;
            float angle = (float)Math.Atan2(difference.Y, difference.X);
            spriteBatch.Draw(BlankTex, start, null, color, angle, Vector2.Zero,
             new Vector2(difference.Length(), 1), SpriteEffects.None, 0);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int width)
        {
            Vector2 difference = end - start;
            float angle = (float)Math.Atan2(difference.Y, difference.X);
            spriteBatch.Draw(BlankTex, start, null, color, angle, Vector2.Zero,
             new Vector2(difference.Length(), width), SpriteEffects.None, 0);
        }
    }
}