using System;

namespace Mage
{
    public class HitboxCollection : Hitbox
    {
        public Hitbox[] Hitboxes { get; set; }

        public override int X
        {
            get
            {
                int minX = Hitboxes[0].X;
                foreach (var hitbox in Hitboxes)
                    minX = Math.Min(minX, hitbox.X);
                return minX;
            }
            set
            {
                int difference = value - X;
                foreach (var hitbox in Hitboxes)
                    hitbox.X += difference;
            }
        }

        public override int Y
        {
            get
            {
                int minY = Hitboxes[0].Y;
                foreach (var hitbox in Hitboxes)
                    minY = Math.Min(minY, hitbox.Y);
                return minY;
            }
            set
            {
                int difference = value - Y;
                foreach (var hitbox in Hitboxes)
                    hitbox.Y += difference;
            }
        }

        public override int Width
        {
            get
            {
                int maxX = Hitboxes[0].X;
                foreach (var hitbox in Hitboxes)
                    maxX = Math.Max(maxX, hitbox.X);
                return maxX - X;
            }
            set => throw new NotImplementedException();
        }

        public override int Height
        {
            get
            {
                int maxY = Hitboxes[0].Y;
                foreach (var hitbox in Hitboxes)
                    maxY = Math.Max(maxY, hitbox.Y);
                return maxY - Y;
            }
            set => throw new NotImplementedException();
        }

        public HitboxCollection(params Hitbox[] hitboxes)
        {
            if (hitboxes.Length < 1)
                throw new ArgumentException("Attempted to create hitboxlist with no hitboxes");
            Hitboxes = hitboxes;
        }

        public override bool Collides(AABB other)
        {
            foreach (var hitbox in Hitboxes)
            {
                if (hitbox.Collides(other))
                    return true;
            }
            return false;
        }
    }
}