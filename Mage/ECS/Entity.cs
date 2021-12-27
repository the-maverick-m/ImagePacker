using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mage
{
    public abstract class Entity
    {
        public Layer Layer { get; }

        public Atlas Atlas { get; }

        public float Depth { get; }

        public Hitbox Hitbox { get; set; }
        public bool IsSolid { get; set; }

        public Vector2 Position
        {
            get => new Vector2(Hitbox.X, Hitbox.Y);
            set
            {
                Hitbox.X = (int)value.X;
                Hitbox.Y = (int)value.Y;
            }
        }

        public int X
        {
            get => Hitbox.X;
            set => Hitbox.X = value;
        }

        public int Y
        {
            get => Hitbox.Y;
            set => Hitbox.Y = value;
        }

        public int Width
        {
            get => Hitbox.Width;
            set => Hitbox.Width = value;
        }

        public int Height
        {
            get => Hitbox.Height;
            set => Hitbox.Height = value;
        }

        public Entity(Layer layer, float depth, Hitbox hitbox, Atlas atlas)
        {
            Layer = layer;
            Atlas = atlas;
            Hitbox = hitbox;
            Depth = depth;

            layer.Add(this);
        }

        public virtual void OnAdd() { }
        public virtual void OnRemove() { }

        public virtual void Update(float delta) { }
        public abstract void Draw(Renderer renderer);
    }
}