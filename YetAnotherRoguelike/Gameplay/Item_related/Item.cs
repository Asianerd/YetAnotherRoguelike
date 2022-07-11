using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Gameplay
{
    class Item
    {
        // Item class has own Random to ensure every drop is more randomized
        public static Random dropRNG = new Random();
        public static Texture2D shadowSprite;
        public static Dictionary<Type, Texture2D> itemSprites = new Dictionary<Type, Texture2D>();
        public static Dictionary<Tile.Type, Dictionary<Type, int>> lootTable = new Dictionary<Tile.Type, Dictionary<Type, int>>();
        public static List<Item> collection = new List<Item>();
        public static Vector2 spriteOrigin;

        public static void Initialize()
        {
            spriteOrigin = new Vector2(8);
            shadowSprite = Game.Instance.Content.Load<Texture2D>("Entities/shadow");

            foreach(Type t in Enum.GetValues(typeof(Type)))
            {
                itemSprites.Add(t, Game.Instance.Content.Load<Texture2D>($"Item_sprites/{t}"));
            }
            lootTable.Add(Tile.Type.Coal_ore, new Dictionary<Type, int>() {
                { Type.Coal, 3 },
                { Type.Stone, 1 }
            });

            lootTable.Add(Tile.Type.Stone, new Dictionary<Type, int>() {
                { Type.Stone, 5 }
            });
        }

        public static Dictionary<Type, int> FetchDropChance(Tile.Type type, bool addOffset = true)
        {
            Dictionary<Type, int> result = lootTable.Keys.Contains(type) ? lootTable[type] : new Dictionary<Type, int>() { };
            if (addOffset)
            {
                List<Type> k = result.Keys.ToList();
                foreach(Type t in k)
                {
                    result[t] += dropRNG.Next(-2, 2);
                    if (result[t] <= 0)
                    {
                        result[t] = 1;
                    }
                }
            }
            return result;
        }

        public static void UpdateAll()
        {
            foreach(Item x in collection)
            {
                x.Update();
            }

            collection = collection.Where(n => !n.dead).ToList();
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (Item x in collection)
            {
                x.DrawShadow(spriteBatch);
            }
            foreach (Item x in collection)
            {
                x.Draw(spriteBatch);
            }
        }

        public static float followDistance = 100f;
        public static float pickupDistance = 30f;
        public static float deathDistance = 4000f;
        public static float mergeDistance = 30f;

        public Type type;
        public Vector2 position;
        public GameValue animationAge;
        public Color color;
        public int amount = 1;

        public bool follow = false;

        public bool dead = false;

        Item(Type t, Vector2 pos, int _amount)
        {
            type = t;
            amount = _amount;
            position = pos;
            animationAge = new GameValue(0, 120, 1, Game.random.Next(0, 20));
        }

        public static void Spawn(Type t, Vector2 pos, int _amount)
        {
            foreach(Item i in collection)
            {
                if (t != i.type)
                {
                    continue;
                }
                if (Vector2.Distance(pos, i.position) <= mergeDistance)
                {
                    i.amount += _amount;
                    return;
                }
            }
            collection.Add(new Item(t, pos, _amount));
        }

        public void Update()
        {
            if (dead)
            {
                return;
            }

            float d = Vector2.Distance(position, Player.Instance.position);

            if (d <= followDistance)
            {
                follow = true;
            }

            if (d > deathDistance)
            {
                dead = true;
                return;
            }

            if (d < pickupDistance)
            {
                dead = true;
            }

            if (follow)
            {
                position = Vector2.Lerp(position, Player.Instance.position, 0.3f);
            }


            animationAge.Regenerate();
            if (animationAge.Percent() >= 1f)
            {
                animationAge.I = 0;
            }

            float highest = 0;
            foreach (LightSource light in LightSource.sources)
            {
                float distance = Vector2.Distance(light.position, Chunk.WorldToTile(position));
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
            color = Color.White * (highest / 40f);
            color.A = 255;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(!Game.playArea.Contains(position))
            {
                return;
            }
            spriteBatch.Draw(itemSprites[type], new Vector2(position.X, position.Y + (MathF.Sin(animationAge.Percent() * 2f * MathF.PI)) * 10f), null, color, 0f, spriteOrigin, 3f, SpriteEffects.None, 0f);
        }

        public void DrawShadow(SpriteBatch spriteBatch)
        {
            if (!Game.playArea.Contains(position))
            {
                return;
            }
            spriteBatch.Draw(shadowSprite, new Vector2(position.X - 16, position.Y + 20), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }


        public enum Type
        {
            Stone,
            Coal
        }
    }
}
