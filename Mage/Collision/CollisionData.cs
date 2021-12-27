using Microsoft.Xna.Framework;

namespace Mage
{
    public struct CollisionData
    {
        public Entity Other { get; }
        public Vector2 Direction { get; }

        public CollisionData(Entity other, Vector2 direction)
        {
            Other = other;
            Direction = direction;
        }
    }
}