using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Graphics;
using YetAnotherRoguelike.PhysicsObject;

namespace YetAnotherRoguelike
{
    class GroundItem
    {
        #region Statics
        public static Texture2D shadowSprite;
        public static Vector2 shadowSpriteOrigin, shadowSpriteOffset;
        public static List<GroundItem> collection;

        public static void Initialize()
        {
            shadowSprite = Game.Instance.Content.Load<Texture2D>("Entities/shadow");
            shadowSpriteOrigin = shadowSprite.Bounds.Size.ToVector2() / 2f;
            shadowSpriteOffset = new Vector2(0, 0.44f);
            collection = new List<GroundItem>();
        }

        public static void UpdateAll()
        {
            /*List<GroundItem> collectionCopy = new List<GroundItem>(collection);
            foreach (GroundItem x in collectionCopy)
            {
                x.Update();
            }*/

            int index = 0;
            while (index < collection.Count())
            {
                collection[index].Update();
                index++;
            }

            collection = collection.Where(n => !n.dead).ToList();
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (GroundItem x in collection)
            {
                x.DrawShadow(spriteBatch);
            }
            foreach (GroundItem x in collection)
            {
                x.Draw(spriteBatch);
            }
        }
        #endregion


        public static float followDistance = 1.5f;
        public static float pickupDistance = 0.4f;
        public static float deathDistance = 300f;
        public static float mergeDistance = 0.5f;

        public static float fallDistance = 0.5f;

        public bool dead = false;
        public Item item;
        public Vector2 position;

        public GameValue pickupCooldown;
        public bool follow = false;

        public Color color = Color.White;
        public GameValue animationAge;
        float increment, start, fall;
        Vector2 finalPositionOffset; // used for shadow sprite
        float finalY;

        float drawnLayer;
        
        public GroundItem(Item i, Vector2 p, Vector2 origin, bool c = false)
        {
            item = i;
            position = p;
            animationAge = new GameValue(0, 120, 1, Game.random.Next(0, 20));

            fall = Game.random.Next(0, 3);

            start = position.Y + (Game.random.Next(-100, 100) / 1000f);
            increment = ((position - origin) * 0.005f).X;

            finalPositionOffset = Vector2.Zero;
            finalY = ((-16 * MathF.Pow((fallDistance) - 0.25f, 2)) + 1) * 0.5f;

            pickupCooldown = new GameValue(0, 60, 1);
            pickupCooldown.AffectValue(c ? 0f : 1f);
        }

        public void Update()
        {
            if (dead)
            {
                return;
            }

            pickupCooldown.Regenerate(Game.compensation);

            if (pickupCooldown.Percent() == 1f)
            {
                float d = Vector2.Distance(position, Player.Instance.position);

                if (d <= followDistance)
                {
                    follow = Player.Instance.InventoryCanFit(item);
                    // follow = (whether player has space for this item)
                }

                if (d >= deathDistance)
                {
                    dead = true;
                    return;
                }

                if (d <= pickupDistance)
                {
                    if (follow)
                    {
                        if (Player.Instance.InventoryCanFit(item))
                        {
                            Player.Instance.InventoryAppend(item);
                        }
                        dead = true;
                        return;
                    }
                }
            }


            if (!follow)
            {
                fall += Game.compensation * 0.75f;
                float p = fall * 0.025f;
                if (p < fallDistance)
                {
                    position.Y = start - ((-16 * MathF.Pow((p < fallDistance ? p : fallDistance) - 0.25f, 2)) + 1) * 0.5f;
                    position.X += increment;

                    finalPositionOffset.Y = finalY - ((-16 * MathF.Pow((p < fallDistance ? p : fallDistance) - 0.25f, 2)) + 1) * 0.5f;
                }

                // grounditems merging with other ground items
                foreach (GroundItem i in collection)
                {
                    if (i.dead)
                    {
                        continue;
                    }
                    if (i.item.type != item.type)
                    {
                        continue;
                    }
                    if (i.item.Full())
                    {
                        continue;
                    }
                    if (i == this)
                    {
                        continue;
                    }

                    if (Vector2.Distance(i.position, position) < mergeDistance)
                    {
                        i.item.amount += item.amount;
                        //item.amount = 0; // just in case
                        dead = true;
                        break;
                    }
                }
            }
            else
            {
                if (pickupCooldown.Percent() == 1f)
                {
                    position = Vector2.Lerp(position, Player.Instance.position, 0.3f * Game.compensation);
                }
            }

            animationAge.Regenerate(Game.compensation);
            if (animationAge.Percent() >= 1f)
            {
                animationAge.I = 0;
            }

            //UpdateColor();
        }

        public void UpdateColor()
        {
            // no color blending, just light intensity
            float highest = 0;
            foreach (LightSource light in LightSource.sources)
            {
                float distance = Vector2.Distance(light.position, position);
                if (distance > light.range)
                {
                    continue;
                }

                float intensity = (light.strength * (1f - (distance * light.oneOverRange)));

                if (intensity > highest)
                {
                    highest = intensity;
                }
            }
            color = Color.White * (highest * 0.05f);
            color.A = 255;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (!Game.playArea.Contains(position))
            {
                return;
            }

            if (dead)
            {
                return;
            }

            Vector2 renderedPosition = position * Tile_Classes.Tile.tileSize;
            renderedPosition.Y += MathF.Sin(animationAge.Percent() * MathF.PI * 2f) * Tile_Classes.Tile.tileSize * 0.1f;
            drawnLayer = Camera.GetDrawnLayer(renderedPosition.Y);
            spritebatch.Draw(Item.itemSprites[item.type], renderedPosition, null, color, 0f, Item.spriteOrigin, Tile_Classes.Tile.renderScale * 3f, SpriteEffects.None, drawnLayer);
        }

        public void DrawShadow(SpriteBatch spritebatch)
        {
            if (!Game.playArea.Contains(position))
            {
                return;
            }

            float _scale = ((1f + MathF.Sin(animationAge.Percent() * MathF.PI * 2f)) * 0.2f) + 0.5f;
            spritebatch.Draw(shadowSprite, (position - finalPositionOffset + shadowSpriteOffset) * Tile_Classes.Tile.tileSize, null, Color.White, 0f, shadowSpriteOrigin, Tile_Classes.Tile.renderScale * _scale, SpriteEffects.None, drawnLayer);
        }
    }
}
