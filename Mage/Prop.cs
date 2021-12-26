using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mage
{
    public abstract class Prop : Entity
    {
        public Prop(Layer layer, float depth, Hitbox hitbox, Atlas atlas) : base(layer, depth, hitbox, atlas)
        {
        }
    }
}