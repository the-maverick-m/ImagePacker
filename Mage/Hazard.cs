using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mage
{
    public abstract class Hazard : Entity
    {
        public Hazard(Layer layer, float depth, Hitbox hitbox, Atlas atlas)
         : base(layer, depth, hitbox, atlas)
        {
        }
    }
}