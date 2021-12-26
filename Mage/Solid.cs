using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mage
{
    public abstract class Solid : Entity
    {
        public Solid(Layer layer, float depth, Hitbox hitbox, Atlas atlas)
         : base(layer, depth, hitbox, atlas)
        {
            IsSolid = true;
        }
    }
}