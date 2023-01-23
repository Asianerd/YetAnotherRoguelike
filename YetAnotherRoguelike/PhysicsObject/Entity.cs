using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Graphics;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.PhysicsObject
{
    class Entity
    {
        public static float friction = 0.5f;
        public static Rectangle collisionRect = new Rectangle(0, 0, 0, 0); // rect used for collision, just change the location and size accordingly

        public Vector2 position; // is in tile-coordinates
        public Vector2 hitbox; // in tile-coordinates
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
                else if (value.X < 0)
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
                else if (value.Y < 0)
                {
                    if (value.Y < _velocity.Y)
                    {
                        _velocity.Y = value.Y;
                    }
                }
            }
        }

        public float drawnLayer;

        public Entity(Vector2 p, Vector2 h)
        {
            position = p;
            hitbox = h;
        }

        public virtual void Update()
        {
            Vector2 compensatedVelocity = velocity * Game.compensation;
            Vector2 target = position + compensatedVelocity;

            Vector2 collisionPosition;
            collisionPosition = new Vector2(position.X + compensatedVelocity.X, position.Y) + (hitbox * 0.5f);
            if (!Chunk.CollideRect(collisionPosition, hitbox))
            {
                position.X = target.X;
            }

            collisionPosition = new Vector2(position.X, position.Y + compensatedVelocity.Y) + (hitbox * 0.5f);
            if (!Chunk.CollideRect(collisionPosition, hitbox))
            {
                position.Y = target.Y;
            }

            _velocity *= friction * Game.compensation;

            drawnLayer = Camera.GetDrawnLayer(position.Y * Tile.tileSize);
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {

        }
    }
}
