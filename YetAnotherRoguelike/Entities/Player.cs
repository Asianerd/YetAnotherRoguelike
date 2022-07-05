using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Player : Entity
    {
        public static Player Instance;

        public static Texture2D sprite;
        public static new float renderScale = 5f;
        public static float speed = 8f;

        public PhysicsBody walkingPhysics;

        public Player() : base(Species.Player, new PhysicsBody(Vector2.Zero), new Point(60, 60))
        {
            Instance = this;

            sprite = Game.Instance.Content.Load<Texture2D>("Entities/Player/body");
            spriteOrigin = sprite.Bounds.Size.ToVector2() / 2f;

            walkingPhysics = new PhysicsBody(Vector2.Zero, maxVel:10f);
        }

        public override void Update()
        {
            Movement();

            if (Input.collection[Keys.Space].active)
            {
                position = Vector2.Zero;
            }

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, 0f, spriteOrigin, renderScale, SpriteEffects.None, 0f);
        }

        public override void ApplyPhysics()
        {
            walkingPhysics.Update();
            physics.Update();

            Vector2 totalVelocity = TotalVelocity();
            Vector2 targetPosition = position + totalVelocity;

            Vector2 xVelocity = new Vector2(totalVelocity.X, 0);
            Rectangle xRect = new Rectangle((position - (spriteOrigin * renderScale) + xVelocity + new Vector2(0, size.Y/2f)).ToPoint(), new Point(size.X, (size.Y / 2)));
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

        public override Vector2 TotalVelocity()
        {
            return physics.velocity + walkingPhysics.velocity;
        }

        public void Movement()
        {
            Vector2 final = Vector2.Zero;
            foreach (Keys x in new Keys[] { Keys.W, Keys.A, Keys.S, Keys.D })
            {
                if (Input.collection[x].isPressed)
                {
                    final += GeneralDependencies.axialVectors[x];
                }
            }
            if ((final.X != 0) || (final.Y != 0))
            {
                final.Normalize();
            }
            final *= speed * (Input.collection[Keys.LeftShift].isPressed ? 2 : 1);
            walkingPhysics.velocity += final;
        }
    }
}
