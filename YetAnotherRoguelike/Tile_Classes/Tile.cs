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
        public static int tileSize = 64;

        public static void Initialize()
        {
            foreach (Type x in Enum.GetValues(typeof(Type)).Cast<Type>())
            {
                List<Texture2D> final = new List<Texture2D>();
                for (int i = 1; i <= 16; i++)
                {
                    final.Add(Game.Instance.Content.Load<Texture2D>($"Tiles/{x}/{i}"));
                }
                blockSprites.Add(x, final);
            }
            tileColors = new Dictionary<Type, Color>()
            {
                { Type.Concrete, Color.White },
                { Type.Stone, Color.Gray },
                { Type.Neon, Color.Aquamarine },
                { Type.Coal_ore, new Color(31, 34, 54) },
            };
        }

        public static Tile CreateTile(Type type, Vector2 pos, Chunk parent)
        {
            return type switch
            {
                Type.Neon => new Tile_Classes.Blocks.NeonBlock(pos, parent),
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
                if (Perlin_Noise.Fetch(-Chunk.CorrectedTileToWorld(pos), 128f) >= 0.90f)
                {
                    type = Type.Coal_ore;
                }
            }

            return CreateTile(type, pos, parent);
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

        public Tile(Type _type, Vector2 pos, Chunk _parent)
        {
            parent = _parent;
            type = _type;
            position = pos;
            tPosition = Chunk.FixTilePos(position);
            rect = new Rectangle((pos * tileSize).ToPoint(), new Point(tileSize, tileSize));
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
            float highest = 0;
            List<Color> colors = new List<Color>();
            List<float> intensities = new List<float>();
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
                intensities.Add(percent);

                if (intensity >= highest)
                {
                    highest = intensity;
                }
            }
            lightLevel = highest;
            float compensation = 1f / intensities.Sum();
            Color final = Color.Black;
            foreach (Color color in colors)
            {
                final.R += (byte)(color.R * compensation);
                final.G += (byte)(color.G * compensation);
                final.B += (byte)(color.B * compensation);
            }
            final.A = 255;
            lightColour = final;

            if (type == Type.Air)
            {
                return;
            }
            if (!Game.playArea.Intersects(rect))
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

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Game.playArea.Intersects(rect))
            {
                spriteBatch.Draw(UI.blank, rect, lightColour * (lightLevel / 40f));
                //spriteBatch.Draw(blockSprites[type][spriteIndex], rect, Color.White * (lightLevel / 10f));
                if (type == Type.Air)
                {
                    return;
                }
                spriteBatch.Draw(blockSprites[type][spriteIndex], rect, lightColour * (lightLevel / 20f));
            }
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
                    Item.Spawn(item.Key, selfPosition + new Vector2(
                                tileSize * (Game.random.Next(0, 1000) / 1000f),
                                tileSize * (Game.random.Next(0, 1000) / 1000f)
                                ), item.Value);
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

        public enum Type
        {
            Air,
            Stone,
            Concrete,
            Neon,

            Coal_ore
        }
    }
}
