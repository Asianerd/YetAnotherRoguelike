using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Gameplay.ItemStorage;
using YetAnotherRoguelike.Gameplay;

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

        public List<Item> inventory;

        public Player() : base(Species.Player, new PhysicsBody(Vector2.Zero), new Point(60, 60))
        {
            Instance = this;

            sprite = Game.Instance.Content.Load<Texture2D>("Entities/Player/body");
            spriteOrigin = sprite.Bounds.Size.ToVector2() / 2f;
            renderOffset = spriteOrigin * 2f * renderScale;

            walkingPhysics = new PhysicsBody(Vector2.Zero, maxVel:10f);
            bodyLight = new LightSource(Vector2.Zero, 10, 5, Color.White);
            inventory = new List<Item>();
            for (int i = 0; i < Inventory.inventorySize; i++)
            {
                inventory.Add(new Item(Item.Type.None, 0));
            }
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

            if (Input.collection[Keys.E].active)
            {
                //LightSource.Append(new LightSource(Chunk.CorrectedWorldToTile(Cursor.WorldPosition()), 20, 10, Color.White));
                Inventory.Instance.Toggle();
            }

            if (Input.collection[Keys.F].active)
            {
                //Particle.particles.Add(new Particles.Smoke(Cursor.WorldPosition()));
                foreach(GroundItem x in GroundItem.collection)
                {
                    x.follow = true;
                }
            }

            if (!UI_Container.hoverContainer && (Cursor.item.type == Item.Type.None))
            {
                if (MouseInput.left.isPressed)
                {
                    //Map.Break(Cursor.WorldPosition());
                    if (targetedTile != null)
                    {
                        targetedTile.DegenerateDurability(-100f * Game.compensation);
                    }
                }

                if (MouseInput.right.active)
                {
                    Map.Place(Tile.Type.Neon, Cursor.WorldPosition());
                }
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

        #region Physics
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
            final *= speed * (Input.collection[Keys.LeftShift].isPressed ? 2 : 1) * Game.compensation;
            walkingPhysics.velocity += final;
        }
        #endregion

        #region Inventory
        public bool Pickable(Item.Type type)
        {
            // possible memoization?
            foreach(Item x in inventory)
            {
                if (x.type == Item.Type.None)
                {
                    return true;
                }
                if (x.type == type)
                {
                    if (x.amount < Item.stackSize)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Pickup(Item.Type type, int amount)
        {
            int balance;
            bool success = false;
            foreach(Item x in inventory)
            {
                if (x.type == Item.Type.None)
                {
                    x.type = type;
                    x.amount = amount;
                    success = true;
                    break;
                }
                else if (x.type == type)
                {
                    if (x.amount < Item.stackSize)
                    {
                        x.amount += amount;
                        success = true;
                        break;
                    }
                }
                if (success)
                {
                    balance = x.amount - Item.stackSize;
                    if (balance > 0)
                    {
                        x.amount = Item.stackSize;
                        Drop(new Item(x.type, balance));
                    }

                    //ItemPopupParent.Instance.Add(type, amount);
                    // maybe not?
                    return;
                }
            }
        }

        public void Drop(Item item)
        {
            float d = (Game.random.Next(0, 100) / 100f) * MathF.PI * 2f;
            GroundItem.Spawn(item.type, position + new Vector2(
                MathF.Cos(d),
                MathF.Sin(d)
                ), item.amount, position);
        }
        #endregion
    }
}
