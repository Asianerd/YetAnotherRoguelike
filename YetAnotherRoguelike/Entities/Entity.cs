using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Entity
    {
        public static List<Entity> collection = new List<Entity>();

        public Vector2 position;
        public Rectangle rect;
        public Point size;
        public Species species;
        public PhysicsBody physics;
        protected Vector2 spriteOrigin;

        public static float renderScale = 5f;

        public Entity(Species _species, PhysicsBody _physics, Point _size)
        {
            collection.Add(this);
            species = _species;
            physics = _physics;
            size = _size;
        }

        public virtual void ApplyPhysics()
        {
            physics.Update();

            Vector2 totalVelocity = TotalVelocity();
            Vector2 targetPosition = position + totalVelocity;

            Vector2 xVelocity = new Vector2(totalVelocity.X, 0);
            Rectangle xRect = new Rectangle((position - (spriteOrigin * renderScale) + xVelocity + new Vector2(0, size.Y / 2f)).ToPoint(), new Point(size.X, (size.Y / 2)));
            if (!Map.CollideTiles(xRect))
            {
                position.X = targetPosition.X;
            }

            Vector2 yVelocity = new Vector2(0, totalVelocity.Y);
            Rectangle yRect = new Rectangle((position - (spriteOrigin * renderScale) + yVelocity + new Vector2(0, size.Y / 2f)).ToPoint(), new Point(size.X, (size.Y / 2)));
            if (!Map.CollideTiles(yRect))
            {
                position.Y = targetPosition.Y;
            }
        }

        public virtual Vector2 TotalVelocity()
        {
            return physics.velocity;
        }

        public virtual void Update()
        {
            rect = new Rectangle((position - (spriteOrigin * renderScale)).ToPoint(), size);
            ApplyPhysics();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public virtual void OnDeath()
        {
            if (collection.Contains(this))
            {
                collection.Remove(this);
            }
        }

        public enum Species
        {
            Player,
            Cyborg
        }
    }
}
