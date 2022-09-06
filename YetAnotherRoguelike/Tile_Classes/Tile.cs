using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Gameplay;

namespace YetAnotherRoguelike
{
    class Tile
    {
        public static Dictionary<Type, List<Texture2D>> blockSprites = new Dictionary<Type, List<Texture2D>>();
        public static Dictionary<Type, Color> tileColors = new Dictionary<Type, Color>();
        public static Dictionary<int, Type> oreChances = new Dictionary<int, Type>(); // from 0 - 1000
        public static int tileSize = 64;

        public static void Initialize()
        {
            foreach (Type t in Enum.GetValues(typeof(Type)).Cast<Type>())
            {
                List<Texture2D> final = new List<Texture2D>();
                Texture2D sprite = Game.Instance.Content.Load<Texture2D>($"Tiles/{t.ToString().ToLower()}");
                /*for (int i = 1; i <= 16; i++)
                {
                    final.Add(Game.Instance.Content.Load<Texture2D>($"Tiles/{x}/{i}"));
                }*/
                final = GeneralDependencies.Split(sprite, 16, 16);
                blockSprites.Add(t, final);

                // generating particle colors for when tile is broken (messy, i know)
                Texture2D texture = final[15];
                Color[] _r = new Color[256];
                texture.GetData(_r);
                List<Color> result = _r.ToList();
                result = result.Where(n => n != Color.Transparent).ToList();
                result = result.Where(n => n != new Color(112, 112, 112)).ToList();
                result = result.Where(n => n != new Color(85, 85, 85)).ToList();
                result = result.Where(n => n != new Color(145, 145, 145)).ToList();
                result = result.Where(n => n != new Color(168, 168, 168)).ToList();
                Dictionary<Color, int> amount = new Dictionary<Color, int>();
                foreach (Color c in result)
                {
                    if (amount.ContainsKey(c))
                    {
                        amount[c]++;
                    }
                    else
                    {
                        amount.Add(c, 1);
                    }
                }
                Color finalColor = Color.Black;
                int highest = 0;
                foreach(Color item in amount.Keys)
                {
                    if (amount[item] > highest)
                    {
                        finalColor = item;
                        highest = amount[item];
                    }
                }
                tileColors.Add(t, finalColor);
            }
            /*tileColors = new Dictionary<Type, Color>()
            {
                { Type.Concrete, Color.White },
                { Type.Stone, Color.Gray },
                { Type.Neon, Color.Aquamarine },
                { Type.Coal_ore, new Color(31, 34, 54) },
                { Type.Bauxite, rgb(184, 74, 28) },
                { Type.Hematite, rgb(158, 166, 168) },
                { Type.Sphalerite, rgb(56, 56, 56) },
                { Type.Calamine, rgb(214, 175, 133) },
                { Type.Galena, rgb(214, 175, 133) },
                { Type.Cinnabar, rgb(196, 75, 59) },
                { Type.Argentite, rgb(202, 202, 207) },
                { Type.Bismuth, rgb(135, 214, 182) },
            };*/
            oreChances = new Dictionary<int, Type>()
            {
                { 500, Type.Coal_ore },
                { 750, Type.Bauxite },
            };
        }

        public static Tile CreateTile(Type type, Vector2 pos, Chunk parent)
        {
            return type switch
            {
                Type.Neon_Blue => new Tile_Classes.Blocks.NeonBlock(pos, parent, Type.Neon_Blue),
                Type.Neon_Pink => new Tile_Classes.Blocks.NeonBlock(pos, parent, Type.Neon_Pink),
                Type.Neon_Yellow => new Tile_Classes.Blocks.NeonBlock(pos, parent, Type.Neon_Yellow),
                Type.Coal_ore => new Tile_Classes.Blocks.CoalOre(pos, parent),
                _ => new Tile(type, pos, parent)
            };
        }

        public static Tile GenerateTile(float p, Vector2 pos, Chunk parent)
        {
            Type type = Type.Air;
            if (p > 0.5f)
            {
                type = Type.Stone;
                float chance = Perlin_Noise.Fetch(-Chunk.CorrectedTileToWorld(pos), 128f);
                if (chance >= 0.80f)
                {
                    type = new Type[] {
                        Type.Coal_ore,
                        Type.Bauxite,
                        Type.Hematite,
                        Type.Sphalerite, // Tin
                        Type.Calamine, // Zinc
                        Type.Galena,
                        Type.Cinnabar, // Mercury
                        Type.Argentite, // Silver
                        Type.Bismuth,
                        Type.Neon_Blue,
                        Type.Neon_Pink
                        /*Type.Neon_Yellow*/
                    }[new Random(GeneralDependencies.CantorPairing((int)pos.X, (int)pos.Y)).Next(0, 11)];
                }
            }

            return CreateTile(type, pos, parent);
        }

        public static Type OreByChance(int p)
        {
            foreach(int chance in oreChances.Keys)
            {
                if (p >= chance)
                {
                    return oreChances[chance];
                }
            }
            return Type.Stone;
        }

        public Chunk parent;
        public Type type;
        public Vector2 position; // in tile-coordinates
        public Vector2 tPosition;   // Position in own chunk
                                    // Top left = (0, 0)
        public Rectangle rect;
        int spriteIndex = 0;
        float lightLevel = 0;
        Color lightColour = Color.White;
        public GameValue durability, durabilityCooldown = new GameValue(0, 30, 1);

        bool isAir;

        public Tile(Type _type, Vector2 pos, Chunk _parent)
        {
            parent = _parent;
            type = _type;
            position = pos;
            tPosition = Chunk.FixTilePos(position);
            rect = new Rectangle((pos * tileSize).ToPoint(), new Point(tileSize, tileSize));

            durability = new GameValue(0, type switch
            {
                Type.Stone => 80,
                Type.Coal_ore => 180,
                _ => 100,
            }, 1, 100);

            isAir = type == Type.Air;
        }

        /*public Tile(float t, Vector2 pos, Chunk _parent)
        {
            if (t <= 0.5f)
            {
                type = Type.Air;
            }
            else if (t <= 0.7f)
            {
                type = Type.Stone;
            }
            else
            {
                type = Type.Concrete;
            }

            position = pos;
            parent = _parent;
            tPosition = Chunk.FixTilePos(position);
            rect = new Rectangle((pos * tileSize).ToPoint(), new Point(tileSize, tileSize));
        }*/

        public virtual void UpdateSprite(Chunk parent)
        {
            if (!Game.playArea.Intersects(rect))
            {
                return;
            }

            float highest = 0;
            List<Color> colors = new List<Color>();
            float totalIntensity = 0;
            foreach (LightSource light in LightSource.sources)
            {
                float distance = Vector2.Distance(light.position, position);
                if (distance > light.range)
                {
                    continue;
                }

                float percent = (1f - (distance / light.range));
                float intensity = (light.strength * percent);
                colors.Add(light.color * percent);
                totalIntensity += percent;

                if (intensity >= highest)
                {
                    highest = intensity;
                }
            }
            lightLevel = highest;
            float compensation = 1f / totalIntensity;
            Color final = Color.Black;
            foreach (Color color in colors)
            {
                final.R += (byte)(color.R * compensation);
                final.G += (byte)(color.G * compensation);
                final.B += (byte)(color.B * compensation);
            }
            final.A = 255;
            lightColour = final;

            if (isAir)
            {
                return;
            }

            int right, left, up, down;
            up = Map.TypeAt(tPosition - Vector2.UnitY + (parent.position * Chunk.size)) != Type.Air ? 0 : 1;
            right = Map.TypeAt(tPosition + Vector2.UnitX + (parent.position * Chunk.size)) != Type.Air ? 0 : 2;
            down = Map.TypeAt(tPosition + Vector2.UnitY + (parent.position * Chunk.size)) != Type.Air ? 0 : 4;
            left = Map.TypeAt(tPosition - Vector2.UnitX + (parent.position * Chunk.size)) != Type.Air ? 0 : 8;
            spriteIndex = right + left + up + down;
        }

        public virtual void Update()
        {
            durabilityCooldown.Regenerate(Game.compensation);
            if (durabilityCooldown.Percent() >= 1f)
            {
                durability.AffectValue(1f);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Game.playArea.Intersects(rect))
            {
                spriteBatch.Draw(UI.blank, rect, lightColour * (lightLevel / 40f));
                //spriteBatch.Draw(blockSprites[type][spriteIndex], rect, Color.White * (lightLevel / 10f));
                if (isAir)
                {
                    return;
                }
                spriteBatch.Draw(blockSprites[type][spriteIndex], rect, lightColour * (lightLevel / 20f));
                if (durability.Percent() != 1f)
                {
                    spriteBatch.Draw(UI.blank, new Rectangle(new Vector2(rect.Location.X + 4, rect.Location.Y + 20).ToPoint(), new Point(56, 24)), Color.Black * 0.5f);
                    spriteBatch.Draw(UI.blank, new Rectangle(new Vector2(rect.Location.X + 8, rect.Location.Y + 24).ToPoint(), new Point((int)(48f * (1f - (float)durability.Percent())), 16)), Color.White);
                }
            }
        }

        public virtual void DegenerateDurability(float i)
        {
            durability.Regenerate(i);
            durabilityCooldown.AffectValue(0f);
        }



        public static bool Destroy(Tile tile)
        {
            if (tile.type == Type.Air)
            {
                return false;
            }
            else
            {
                Color pColor = tileColors[tile.type];
                Vector2 selfPosition = Chunk.TileToWorld(tile.position);
                Vector2 selfOrigin = selfPosition + (new Vector2(tileSize) / 2f);
                for (int i = 0; i <= 20; i++)
                {
                    Particle.particles.Add(new Particles.BreakBlock(
                            selfPosition + new Vector2(
                                tileSize * (Game.random.Next(0, 1000) / 1000f),
                                tileSize * (Game.random.Next(0, 1000) / 1000f)
                                ),
                            pColor,
                            selfOrigin
                            ));
                }
                foreach (KeyValuePair<Item.Type, int> item in Item.FetchDropChance(tile.type))
                {
                    GroundItem.Spawn(item.Key, selfPosition + new Vector2(
                                tileSize * (Game.random.Next(0, 1000) / 1000f),
                                tileSize * (Game.random.Next(0, 1000) / 1000f)
                                ), item.Value, tile.rect.Center.ToVector2());
                }
                tile.type = Type.Air;
                tile.OnDestroy();

                tile.parent.collection[(int)tile.tPosition.Y][(int)tile.tPosition.X] = new Tile(Type.Air, tile.position, tile.parent);
                return true;
            }
        }

        public virtual void OnDestroy()
        {

        }

        public virtual void HardUnload()
        {
            // called before the chunk is deleted
        }

        static Color rgb(int r, int g, int b)
        {
            // just here for convenience
            return new Color(r, g, b);
        }

        public enum Type
        {
            Air,
            Stone,
            Concrete,

            Neon_Blue,
            Neon_Pink,
            Neon_Yellow,

            Coal_ore,
            Bauxite,
            Hematite,
            Sphalerite, // Tin
            Calamine, // Zinc
            Galena,
            Cinnabar, // Mercury
            Argentite, // Silver
            Bismuth,

            Blast_Furnace
        }
    }
}
