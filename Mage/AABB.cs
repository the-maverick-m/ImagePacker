using System;
using Microsoft.Xna.Framework;

namespace Mage
{
    public class AABB : Hitbox
    {
        public static AABB Empty { get; } = new AABB(0, 0, 0, 0);

        public static AABB One { get; } = new AABB(0, 0, 1, 1);

        public override int X { get; set; }
        public override int Y { get; set; }
        public override int Width { get; set; }
        public override int Height { get; set; }

        public AABB()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public AABB(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        public override bool Collides(AABB other)
        {
            return other.Left < Right &&
                   Left < other.Right &&
                   other.Top < Bottom &&
                   Top < other.Bottom;
        }
    }
}