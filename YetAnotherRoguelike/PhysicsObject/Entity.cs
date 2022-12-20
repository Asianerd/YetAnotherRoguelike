using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.PhysicsObject
{
    class Entity
    {
        public static float friction = 0.5f;

        public Vector2 position; // is in tile-coordinates
        public Vector2 _velocity = Vector2.Zero; // making this public incase velocity needs to be hard-set
        public Vector2 velocity
        {
            get { return _velocity; }
            set
            {
                if (value.X > 0)
                {
                    if (value.X > _velocity.X)
                    {
                        _velocity.X = value.X;
                    }
                }
                else
                {
                    if (value.X < _velocity.X)
                    {
                        _velocity.X = value.X;
                    }
                }

                if (value.Y > 0)
                {
                    if (value.Y > _velocity.Y)
                    {
                        _velocity.Y = value.Y;
                    }
                }
                else
                {
                    if (value.Y < _velocity.Y)
                    {
                        _velocity.Y = value.Y;
                    }
                }
            }
        }

        public Entity(Vector2 p)
        {
            position = p;
        }

        public virtual void Update()
        {
            position += velocity * Game.compensation;
            _velocity *= friction;
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {

        }
    }
}
