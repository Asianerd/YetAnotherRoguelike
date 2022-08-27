using System;
using System.Linq;
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

        public static Vector2 renderOffset;

        public static Tile targetedTile;

        public PhysicsBody walkingPhysics;
        public LightSource bodyLight;

        public Player() : base(Species.Player, new PhysicsBody(Vector2.Zero), new Point(60, 60))
        {
            Instance = this;

            sprite = Game.Instance.Content.Load<Texture2D>("Entities/Player/body");
            spriteOrigin = sprite.Bounds.Size.ToVector2() / 2f;
            renderOffset = spriteOrigin * 2f * renderScale;

            walkingPhysics = new PhysicsBody(Vector2.Zero, maxVel:10f);
            bodyLight = new LightSource(Vector2.Zero, 10, 5, Color.White);
            //LightSource.Append(bodyLight);
        }

        public override void Update()
        {
            targetedTile = Map.Fetch(Chunk.CorrectedWorldToTile(Cursor.worldPosition));

            Movement();

            bodyLight.position = Chunk.CorrectedWorldToTile(position);

            if (Input.collection[Keys.Space].active)
            {
                position = Vector2.Zero;
            }

            /*if (Input.collection[Keys.F].active)
            {
                Chunk result = Map.ChunkAt(position);
                if (result != null)
                {
                    result.custom = true;
                }
            }*/

            if (Input.collection[Keys.E].active)
            {
                LightSource.Append(new LightSource(Chunk.CorrectedWorldToTile(Cursor.WorldPosition()), 20, 10, Color.White));
            }

            if (Input.collection[Keys.F].active)
            {
                //Particle.particles.Add(new Particles.Smoke(Cursor.WorldPosition()));
                foreach(Gameplay.GroundItem x in Gameplay.GroundItem.collection)
                {
                    x.follow = true;
                }
            }

            if (MouseInput.left.isPressed)
            {
                //Map.Break(Cursor.WorldPosition());
                targetedTile.DegenerateDurability(-1f);
            }

            if (MouseInput.right.active)
            {
                Map.Place(Tile.Type.Neon, Cursor.WorldPosition());
            }

            if (Input.collection[Keys.R].active)
            {
                if (targetedTile.type == Tile.Type.Air)
                {
                    position = Cursor.WorldPosition();
                }
            }

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float highest = 0;
            List<Color> colors = new List<Color>();
            List<float> intensities = new List<float>();
            foreach (LightSource light in LightSource.sources)
            {
                if (light == bodyLight)
                {
                    continue;
                }

                float distance = Vector2.Distance(light.position, Chunk.CorrectedWorldToTile(position));
                if (distance > light.range)
                {
                    continue;
                }

                float percent = (1f - (distance / light.range));
                float intensity = (light.strength * percent);
                colors.Add(light.color * percent);
                intensities.Add(percent);

                if (intensity >= highest)
                {
                    highest = intensity;
                }
            }
            float lightLevel = highest;
            float compensation = 1f / intensities.Sum();
            Color final = Color.Black;
            foreach (Color color in colors)
            {
                final.R += (byte)(color.R * compensation);
                final.G += (byte)(color.G * compensation);
                final.B += (byte)(color.B * compensation);
            }
            Color lightColour = final;
            spriteBatch.Draw(sprite, position, null, Color.White, 0f, spriteOrigin, renderScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(sprite, position, null, lightColour * (lightLevel / 80f), 0f, spriteOrigin, renderScale, SpriteEffects.None, 0f);
        }

        public override void ApplyPhysics()
        {
            walkingPhysics.Update();

            /*physics.Update();

            position += TotalVelocity();*/

            base.ApplyPhysics();
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
