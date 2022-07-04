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

            Rectangle newRect = new Rectangle((position + physics.velocity).ToPoint(), size);
            if (!Map.CollideTiles(newRect))
            {
                position += physics.velocity;
                return;
            }
        }

        public virtual Vector2 TotalVelocity()
        {
            return physics.velocity;
        }

        public virtual void Update()
        {
            rect = new Rectangle(position.ToPoint(), size);
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
