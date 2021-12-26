using System;
using Microsoft.Xna.Framework;

namespace Mage
{
    public abstract class Hitbox
    {
        public abstract int X { get; set; }
        public abstract int Y { get; set; }
        public abstract int Width { get; set; }
        public abstract int Height { get; set; }

        public int Left => X;
        public int Right => X + Width;
        public int Top => Y;
        public int Bottom => Y + Height;

        public Point Center => new Point(X + (Width / 2), Y + (Height / 2));

        public bool Collides(Hitbox other)
        {
            if (other is AABB aabb)
                return Collides(aabb);

            throw new NotImplementedException("Collisions against the type given aren't specified");
        }

        public abstract bool Collides(AABB other);
    }
}