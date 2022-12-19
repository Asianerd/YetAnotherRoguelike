using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Gameplay
{
    class GroundItem
    {
        public static Texture2D shadowSprite;
        public static List<GroundItem> collection = new List<GroundItem>();
        public static Vector2 spriteOrigin;

        public static void Initialize()
        {
            spriteOrigin = new Vector2(8);
            shadowSprite = Game.Instance.Content.Load<Texture2D>("Entities/shadow");
        }

        public static void UpdateAll()
        {
            var t = new List<GroundItem>(collection);
            foreach(GroundItem x in t)
            {
                x.Update();
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

        #region Instance stuff
        public static float followDistance = 100f;
        public static float pickupDistance = 30f;
        public static float deathDistance = 4000f;
        public static float mergeDistance = 50f;

        public static float fallDistance = 0.5f;

        public Item item;
        public Vector2 position;
        public GameValue animationAge;
        public Color color;

        public GameValue pickupCooldown;
        public bool follow = false;
        public bool dead = false;

        float increment, start, fall;

        GroundItem(Item.Type t, int _amount, Vector2 pos, Vector2 origin, bool _cooldown = false)
        {
            item = new Item(t, _amount);
            position = pos;
            animationAge = new GameValue(0, 120, 1, Game.random.Next(0, 20));

            fall = Game.random.Next(0, 10);

            start = pos.Y + (Game.random.Next(-100, 100) / 100f);
            increment = ((pos - origin) * 0.05f).X;

            pickupCooldown = new GameValue(0, 60, 1, _cooldown ? 0 : 100);
        }

        public static void Spawn(Item item, Vector2 pos, bool _cooldown = false, bool _check = false)
        {
            if (_check)
            {
                if (item.type == Item.Type.None)
                {
                    return;
                }
            }

            float d = (Game.random.Next(0, 100) / 100f) * MathF.PI * 2f;
            collection.Add(new GroundItem(item.type, item.amount, pos + new Vector2(
                MathF.Cos(d),
                MathF.Sin(d)
                ), pos, _cooldown));
        }

        public static void Spawn(Item.Type t, Vector2 pos, int _amount, Vector2 origin, bool _cooldown = false)
        {
            /*foreach(Item i in collection)
            {
                if (t != i.type)
                {
                    continue;
                }
                if (Vector2.Distance(pos, i.position) < mergeDistance)
                {
                    i.amount += _amount;
                    return;
                }
            }*/
            collection.Add(new GroundItem(t, _amount, pos, origin, _cooldown));
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
                    follow = Player.Instance.Pickable(item.type);
                }

                if (d > deathDistance)
                {
                    dead = true;
                    return;
                }

                if (d < pickupDistance)
                {
                    if (follow)
                    {
                        Player.Instance.Pickup(item.type, item.amount);
                        dead = true;
                        return;
                    }
                }
            }

            if (follow)
            {
                position = Vector2.Lerp(position, Player.Instance.position, 0.3f * Game.compensation);
            }
            else
            {
                fall += Game.compensation;
                float p = fall / 40f;
                if (p < fallDistance)
                {
                    position.Y = start - (((-16 * MathF.Pow((p < fallDistance ? p : fallDistance) - 0.25f, 2)) + 1) * 20f);
                    position.X += increment;
                }

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
                    if (i.item.amount >= i.item.stackSize)
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
                        item.amount = 0; // just in case
                        dead = true;
                        break;
                    }
                }
            }

            animationAge.Regenerate(Game.compensation);
            if (animationAge.Percent() >= 1f)
            {
                animationAge.I = 0;
            }

            float highest = 0;
            foreach (LightSource light in LightSource.sources)
            {
                float distance = Vector2.Distance(light.position, Chunk.CorrectedWorldToTile(position));
                if (distance > light.range)
                {
                    continue;
                }

                float intensity = (light.strength * (1f - (distance / light.range)));

                if (intensity > highest)
                {
                    highest = intensity;
                }
            }
            color = Color.White * (highest / 20f);
            color.A = 255;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Game.playArea.Contains(position))
            {
                return;
            }
            Vector2 renderPosition = new Vector2(position.X, position.Y + (MathF.Sin(animationAge.Percent() * 2f * MathF.PI)) * 10f);
            spriteBatch.Draw(Item.itemSprites[item.type], renderPosition, null, color, 0f, spriteOrigin, 3f, SpriteEffects.None, 0f);
            if (item.amount >= 2)
            {
                spriteBatch.Draw(Item.itemSprites[item.type], renderPosition + new Vector2(10, 10), null, color, 0f, spriteOrigin, 3f, SpriteEffects.None, 0f);
                if (item.amount > 2)
                {
                    spriteBatch.Draw(Item.itemSprites[item.type], renderPosition + new Vector2(5f, -5f), null, color, 0f, spriteOrigin, 3f, SpriteEffects.None, 0f);
                }
            }
/*            if (Vector2.Distance(Cursor.worldPosition, position) <= 30f)
            {
                spriteBatch.DrawString(UI.defaultFont, $"{item.type} x{item.amount}", position, Color.White);
            }*/
        }

        public void DrawShadow(SpriteBatch spriteBatch)
        {
            if (!Game.playArea.Contains(position))
            {
                return;
            }
            spriteBatch.Draw(shadowSprite, new Vector2(position.X - 16, position.Y + 20), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        #endregion

        /*
            Coal_ore,
            Bauxite,
            Hematite,
            Sphalerite, // Tin
            Calamine, // Zinc
            Galena,
            Cinnabar, // Mercury
            Argentite, // Silver
            Bismuth,
            */
    }
}
