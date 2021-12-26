using System;
using Microsoft.Xna.Framework;

namespace Mage
{
    public abstract class Actor : Entity
    {
        private bool _collideCheckedThisFrame;
        private float _xRemainder;
        private float _yRemainder;

        protected Actor(Layer layer, float depth, Hitbox hitbox, Atlas atlas)
            : base(layer, depth, hitbox, atlas)
        {
        }

        public void Move(Vector2 amount, bool stopOnSolid = true)
        {
            MoveX(amount.X, stopOnSolid);
            MoveY(amount.Y, stopOnSolid);
        }

        public void MoveX(float amount, bool stopOnSolid = true)
        {
            _xRemainder += amount;

            int moveAmount = (int) Math.Round(_xRemainder);
            int facing = Math.Sign(moveAmount);
            _xRemainder -= moveAmount;

            if (moveAmount != 0) _collideCheckedThisFrame = true;
            while (moveAmount != 0)
            {
                AABB testBox = new AABB(X + facing, Y, Width, Height);

                bool collided = false;
                Entity other = null;
                foreach (Entity entity in Layer.Tracker.Entities[Depth])
                {
                    if (entity == this) continue;
                    if (testBox.Collides(entity.Hitbox))
                    {
                        collided = true;
                        other = entity;
                        break;
                    }
                }

                if (collided)
                {
                    if (stopOnSolid)
                    {
                        if (other.IsSolid)
                            moveAmount = 0;
                    }
                    else
                    {
                        X += facing;
                        moveAmount -= facing;
                    }

                    Vector2 direction = new Vector2(facing, 0);
                    OnCollide(new CollisionData(other, direction));
                }
                else
                {
                    X += facing;
                    moveAmount -= facing;
                }
            }
        }

        public void MoveY(float amount, bool stopOnSolid = true)
        {
            _yRemainder += amount;

            int moveAmount = (int) Math.Round(_yRemainder);
            int facing = Math.Sign(moveAmount);
            _yRemainder -= moveAmount;

            if (moveAmount != 0) _collideCheckedThisFrame = true;
            while (moveAmount != 0)
            {
                AABB testBox = new AABB(X, Y + facing, Width, Height);

                bool collided = false;
                Entity other = null;
                foreach (Entity entity in Layer.Tracker.Entities[Depth])
                {
                    if (entity == this) continue;
                    if (testBox.Collides(entity.Hitbox))
                    {
                        collided = true;
                        other = entity;
                        break;
                    }
                }

                if (collided)
                {
                    if (stopOnSolid)
                    {
                        if (other.IsSolid)
                            moveAmount = 0;
                    }
                    else
                    {
                        Y += facing;
                        moveAmount -= facing;
                    }

                    Vector2 direction = new Vector2(0, facing);
                    OnCollide(new CollisionData(other, direction));
                }
                else
                {
                    Y += facing;
                    moveAmount -= facing;
                }
            }
        }

        public virtual void OnCollide(CollisionData data)
        {
        }

        public override void Update(float delta)
        {
            if (!_collideCheckedThisFrame)
                foreach (Entity entity in Layer.Tracker.Entities[Depth])
                {
                    if (entity == this) continue;
                    if (Hitbox.Collides(entity.Hitbox))
                    {
                        OnCollide(new CollisionData(entity, Vector2.Zero));
                        break;
                    }
                }

            _collideCheckedThisFrame = false;
            base.Update(delta);
        }

        public bool CollideCheck<T>() where T : Entity
        {
            foreach (Entity entity in Layer.Tracker.Entities[Depth])
                if (entity is T)
                {
                    if (entity == this) continue;
                    if (Hitbox.Collides(entity.Hitbox))
                        return true;
                }

            return false;
        }
    }
}