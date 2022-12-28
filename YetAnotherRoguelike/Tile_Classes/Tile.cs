using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Graphics;

namespace YetAnotherRoguelike.Tile_Classes
{
    class Tile
    {
        public static Dictionary<BlockType, List<Texture2D>> tileSprites;
        public static Dictionary<BlockType, Color> blockParticleBreakColors;
        public static List<BlockType> blockLightData; // list of types with light data, refer to JSON_BlockData.collection for the data
        public static Vector2 spriteOrigin;
        public static float spriteRenderScale = 4f; // coefficient to enlarge 16x16 to 64x64
        public static float tileSize = 64f; // size in pixels; sprites are 16x16
        static float _renderScale = 1f;
        public static float renderScale
        {
            get { return _renderScale; }
            set
            {
                _renderScale = value;
                tileSize = _renderScale * 64f;
                spriteRenderScale = tileSize / 16f;
                spriteOrigin = (new Vector2(tileSize) / 2f) / spriteRenderScale;
            }
        }

        public static void Initialize()
        {
            spriteOrigin = (new Vector2(tileSize) / 2f) / spriteRenderScale;

            tileSprites = new Dictionary<BlockType, List<Texture2D>>();
            foreach (BlockType t in Enum.GetValues(typeof(BlockType)).Cast<BlockType>())
            {
                Texture2D sprite = Game.Instance.Content.Load<Texture2D>($"Tiles/{t.ToString().ToLower()}");
                List<Texture2D> final = GeneralDependencies.Split(sprite, 16, 16);
                tileSprites.Add(t, final);
            }

            blockParticleBreakColors = new Dictionary<BlockType, Color>();
            foreach (BlockType t in Enum.GetValues(typeof(BlockType)).Cast<BlockType>())
            {
                if (!Data.JSON_BlockData.blockData.ContainsKey(t))
                {
                    blockParticleBreakColors.Add(t, Color.White);
                    continue;
                }
                blockParticleBreakColors.Add(t, Data.JSON_BlockData.blockData[t].break_color == null ? Color.Transparent : Data.JSON_BlockData.blockData[t].breakParticleColor);
            }

            blockLightData = Data.JSON_BlockData.blockData.Where(n => n.Value.light != null).Select(n => n.Key).ToList();
        }

        public static BlockType GenerateTileType(float p, Point pos, Chunk parentChunk)
        {
            //return BlockType.Air;
            BlockType type = BlockType.Air;
            if (p > 0.5f)
            {
                type = BlockType.Stone;
                float chance = PerlinNoise.OreInstance.GetNoise(pos.X, pos.Y);
                if (chance >= 0.55f)
                {
                    // make the ore selection system better
                    // implement rarity system
                    //return BlockType.Stone;
                    type = new BlockType[] {
                        BlockType.Coal_ore,
                        BlockType.Bauxite,
                        BlockType.Hematite,
                        BlockType.Sphalerite, // Tin
                        BlockType.Calamine, // Zinc
                        BlockType.Galena,
                        BlockType.Cinnabar, // Mercury
                        BlockType.Argentite, // Silver
                        BlockType.Bismuth,
                        BlockType.Neon_Blue,
                        BlockType.Neon_Purple
                        /*Type.Neon_Yellow*/
                    }[parentChunk.tileRandom.Next(0, 11)];
                    //}[new Random(GeneralDependencies.CantorPairing(pos.X, pos.Y)).Next(0, 11)]; <- creating new random is damn expensive
                }
            }
            return type;
        }

        public static Tile CreateTile(Point tCoords, Point cCoords, BlockType t)
        {
            // certain tiles have different ctors
            // find a better way in the future?
            return t switch
            {
                /*BlockType.Neon_Blue => new Tile(tCoords, cCoords, t, new LightSource(tCoords.ToVector2(), new Color(82, 241, 242), 25, 10)),
                BlockType.Neon_Purple => new Tile(tCoords, cCoords, t, new LightSource(tCoords.ToVector2(), new Color(255, 0, 244), 25, 10)),*/
                _ => new Tile(tCoords, cCoords, t,
                l : blockLightData.Contains(t) ? new LightSource(tCoords.ToVector2(),
                Data.JSON_BlockData.blockData[t].lightColor,
                Data.JSON_BlockData.blockData[t].lightStrength,
                Data.JSON_BlockData.blockData[t].lightRange
                ) : null)
            };
        }



        /* 2 position types
         * tile coordinates
         * in-chunk coordinates
         */
        public Point tileCoordinates;
        public Vector2 tileCoordinatesV; // vector2 version of tile coordinate for performance
        Vector2 renderedPosition; // "world position" basically tile position * tilesize
        public Point chunkCoordinates;
        public BlockType type;
        bool isAir;

        public GameValue durability, durabilityCooldown = new GameValue(0, 30, 1);

        int spriteIndex = 0;
        bool isEmissive = false;
        public LightSource lightsource;
        Color color;

        public Tile(Point tCoords, Point cCoords, BlockType t, GameValue d = null, LightSource l = null)
        {
            type = t;

            isAir = type == BlockType.Air;

            tileCoordinates = tCoords;
            tileCoordinatesV = tileCoordinates.ToVector2();
            chunkCoordinates = cCoords;

            renderedPosition = tileCoordinates.ToVector2() * tileSize;

            durability = d != null ? d : new GameValue(0, 100, 1, 100);

            if (l == null)
            {
                return;
            }
            isEmissive = true;
            lightsource = l;

            LightSource.Append(lightsource);
        }

        #region Game logic
        public void DecreaseDurability(float i)
        {
            durability.Regenerate(i);
            durabilityCooldown.AffectValue(0f);
        }

        public virtual void Update()
        {
            durabilityCooldown.Regenerate(Game.compensation);
            if (durabilityCooldown.Percent() >= 1f)
            {
                durability.AffectValue(1f);
            }
        }
        #endregion

        public virtual void UpdateSprite()
        {
            // expensive
            bool u = Chunk.FetchTypeAt(tileCoordinates.X, tileCoordinates.Y - 1) != BlockType.Air;
            bool r = Chunk.FetchTypeAt(tileCoordinates.X + 1, tileCoordinates.Y) != BlockType.Air;
            bool d = Chunk.FetchTypeAt(tileCoordinates.X, tileCoordinates.Y + 1) != BlockType.Air;
            bool l = Chunk.FetchTypeAt(tileCoordinates.X - 1, tileCoordinates.Y) != BlockType.Air;

            spriteIndex = (u ? 0 : 1) + (r ? 0 : 2) + (d ? 0 : 4) + (l ? 0 : 8);
        }

        public virtual void UpdateColor()
        {
            // more expensive lmao
            float highest = 0;
            float totalIntensity = 0;
            int amount = 0;
            Color[] colors = new Color[LightSource.lightSourcesCount];
            foreach (LightSource light in LightSource.sources)
            {
                float distance = Vector2.Distance(light.position, tileCoordinatesV);
                if (distance > light.range)
                {
                    continue;
                }

                float percent = (1f - (distance * light.oneOverRange));
                float intensity = (light.strength * percent);
                totalIntensity += percent;
                colors[amount] = light.color * percent;

                if (intensity > highest)
                {
                    highest = intensity;
                }

                amount++;
            }
            //final /= amount;
            float compensation = 1f / totalIntensity;
            color = Color.Black;
            for (int i = 0; i < amount; i++)
            {
                color.R += (byte)(colors[i].R * compensation);
                color.G += (byte)(colors[i].G * compensation);
                color.B += (byte)(colors[i].B * compensation);
            }
            color *= (highest * 0.08f);
            color.A = (byte)255f;
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            if (isAir)
            {
                return;
            }

            if (!Game.playArea.Contains(tileCoordinates))
            {
                return;
            }

            UpdateSprite();
            UpdateColor();

            spritebatch.Draw(tileSprites[type][spriteIndex], renderedPosition, null, color, 0f, spriteOrigin, spriteRenderScale, SpriteEffects.None, 0f);

            if (durability.Percent() != 1f)
            {
                spritebatch.Draw(Game.emptySprite, new Rectangle(new Vector2(renderedPosition.X + 4 - (tileSize / 2f), renderedPosition.Y + 20 - (tileSize / 2f)).ToPoint(), new Point(56, 24)), Color.Black * 0.5f);
                spritebatch.Draw(Game.emptySprite, new Rectangle(new Vector2(renderedPosition.X + 8 - (tileSize / 2f), renderedPosition.Y + 24 - (tileSize / 2f)).ToPoint(), new Point((int)(48f * (1f - (float)durability.Percent())), 16)), Color.White);
            }
        }

        public virtual void OnDestroy() // when block is destroyed
        {
            for (int i = 0; i <= 20; i++)
            {
                Particle.collection.Add(new Particles.BreakBlock(
                        renderedPosition + new Vector2(
                            tileSize * (Game.random.Next(-1000, 1000) / 2000f),
                            tileSize * (Game.random.Next(-1000, 1000) / 2000f)
                            ),
                        blockParticleBreakColors[type],
                        renderedPosition,
                        l: isEmissive ? lightsource : null
                        ));
            }

            foreach (KeyValuePair<Item.Type, int> x in Item.blockDrops[type])
            {
                GroundItem.collection.Add(new GroundItem(
                    new Item(x.Key, x.Value),
                    tileCoordinatesV + new Vector2(
                        Game.random.Next(-100, 100) / 200f,
                        Game.random.Next(-100, 100) / 200f
                        ),
                    tileCoordinatesV
                    ));
            }

            if (isEmissive)
            {
                LightSource.Remove(lightsource);
            }
        }


        public virtual void OnReload() // when unloaded chunk is reloaded
        {
            if (isEmissive)
            {
                LightSource.Append(lightsource);
            }
        }

        public virtual void OnUnload() // when loaded chunk is unloaded
        {
            if (isEmissive)
            {
                LightSource.Remove(lightsource);
            }
        }

        public virtual void OnChunkDelete() // when chunk is deleted
        {
            if (isEmissive)
            {
                LightSource.Remove(lightsource);
            }
        }

        public enum BlockType
        {
            Air,
            Stone,
            Concrete,

            Neon_Blue,
            Neon_Purple,
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
