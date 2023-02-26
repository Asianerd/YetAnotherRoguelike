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

        public static Vector2 lowerSpriteRenderScale = Vector2.One; // used to scale the lower part of the blocks
        public static Vector2 lowerSpriteOffset;
        public static Color lowerSpriteTint;

        public static Tile targetedTile;

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

                lowerSpriteRenderScale = new Vector2(spriteRenderScale, spriteRenderScale / 2f);
                lowerSpriteOffset = new Vector2(0, tileSize * 0.75f);
            }
        }

        public static void Initialize()
        {
            spriteOrigin = (new Vector2(tileSize) / 2f) / spriteRenderScale;
            lowerSpriteTint = new Color(Color.White * 0.6f, 1f);

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

            renderScale = 1f; // just to call the setter function
        }

        public static BlockType GenerateTileType(float p, Point pos, Chunk parentChunk)
        {
            //return BlockType.Air;
            BlockType type = BlockType.Air;

            if (p > 0.5f)
            {
                type = BlockType.Stone;

                if (PerlinNoise.OrganicTilesInstance.GetNoise(pos.X, pos.Y) <= 0.02f)
                {
                    type = BlockType.Clay;
                    return type;
                }

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
                BlockType.Rudimentary_Furnace => new Tile_BaseFurnace(tCoords, cCoords, BlockType.Rudimentary_Furnace, new LightSource(tCoords.ToVector2(), new Color(255, 189, 0), 1, 0)),
                BlockType.Blast_Furnace => new Tile_BaseFurnace(tCoords, cCoords, BlockType.Blast_Furnace, new LightSource(tCoords.ToVector2(), new Color(255, 189, 0), 1, 0)),
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
        public Vector2 renderedPosition; // "world position" basically tile position * tilesize
        public Point chunkTileCoordinates; // position in the chunk; (0, 0) to (chunkSize, chunkSize)
        public Point chunkCoordinates; // position of parent chunk
        //public Rectangle rect; // rectangle in tile coordinates
        public BlockType type;
        public bool interactable = false;
        public bool targeted;
        bool isAir;
        public bool updateSpriteNextFrame = true;

        public GameValue durability, durabilityCooldown = new GameValue(0, 30, 1);

        int spriteIndex = 0;
        bool isEmissive = false;
        public LightSource lightsource;

        float drawnLayer; // order in drawing

        public BlockType[,] neighbours = new BlockType[3, 3];

        public Tile(Point tCoords, Point cCoords, BlockType t, GameValue d = null, LightSource l = null)
        {
            type = t;

            isAir = type == BlockType.Air;

            tileCoordinates = tCoords;
            tileCoordinatesV = tileCoordinates.ToVector2();
            chunkTileCoordinates = cCoords;
            chunkCoordinates = Chunk.TileToChunkCoordinates(tileCoordinates.X, tileCoordinates.Y);

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
            if (updateSpriteNextFrame)
            {
                updateSpriteNextFrame = false;
                UpdateSprite();
            }

            durabilityCooldown.Regenerate(Game.compensation);
            if (durabilityCooldown.Percent() >= 1f)
            {
                durability.AffectValue(1f);
            }

            drawnLayer = Camera.GetDrawnLayer(renderedPosition.Y);
        }
        #endregion

        public virtual void UpdateSprite()
        {
            // expensive
            neighbours[1, 0] = Chunk.FetchTypeAt(tileCoordinates.X, tileCoordinates.Y - 1); // up
            neighbours[2, 1] = Chunk.FetchTypeAt(tileCoordinates.X + 1, tileCoordinates.Y); // right
            neighbours[1, 2] = Chunk.FetchTypeAt(tileCoordinates.X, tileCoordinates.Y + 1); // down
            neighbours[0, 1] = Chunk.FetchTypeAt(tileCoordinates.X - 1, tileCoordinates.Y); // left

            neighbours[0, 2] = Chunk.FetchTypeAt(tileCoordinates.X - 1, tileCoordinates.Y + 1);
            neighbours[2, 2] = Chunk.FetchTypeAt(tileCoordinates.X + 1, tileCoordinates.Y + 1);

            // fix spriteindexing

            spriteIndex = (neighbours[1, 0] != BlockType.Air ? 0 : 1) + (neighbours[2, 1] != BlockType.Air ? 0 : 2) + (neighbours[1, 2] != BlockType.Air ? 0 : 4) + (neighbours[0, 1] != BlockType.Air ? 0 : 8);
        }

        public virtual void UpdateColor()
        {
            // more expensive lmao
            /*float highest = 0;
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
            color.A = (byte)255f;*/
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

            if (neighbours[1, 2] == BlockType.Air)
            {
                spritebatch.Draw(tileSprites[type][
                    16 + (neighbours[0, 1] != BlockType.Air ? 2 : 0) + (neighbours[2, 1] != BlockType.Air ? 1 : 0)
                    ], renderedPosition + lowerSpriteOffset, null, lowerSpriteTint, 0f, spriteOrigin, lowerSpriteRenderScale, SpriteEffects.None, Camera.GetDrawnLayer(renderedPosition.Y + lowerSpriteOffset.Y, -0.1f));
            }
            spritebatch.Draw(tileSprites[type][spriteIndex], renderedPosition, null, Color.White, 0f, spriteOrigin, spriteRenderScale, SpriteEffects.None, drawnLayer);

            if (durability.Percent() != 1f)
            {
                float _drawnLayer = Math.Clamp(drawnLayer + 0.05f, 0f, 1f);
                spritebatch.Draw(Game.emptySprite, new Rectangle(new Vector2(renderedPosition.X + 4 - (tileSize / 2f), renderedPosition.Y + 20 - (tileSize / 2f)).ToPoint(), new Point(56, 24)), null, Color.Black * 0.5f, 0f, Vector2.Zero, SpriteEffects.None, _drawnLayer - 0.01f);
                spritebatch.Draw(Game.emptySprite, new Rectangle(new Vector2(renderedPosition.X + 8 - (tileSize / 2f), renderedPosition.Y + 24 - (tileSize / 2f)).ToPoint(), new Point((int)(48f * (1f - (float)durability.Percent())), 16)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, _drawnLayer);
            }
        }

        public virtual void OnDestroy() // when block is destroyed
        {
            if (type != BlockType.Air)
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
            }

            if (isEmissive)
            {
                LightSource.Remove(lightsource);
            }

            Chunk.FetchChunkWithCoords(chunkCoordinates.X, chunkCoordinates.Y).OnBlockModify(true);
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

            Clay,

            Neon_Blue,
            Neon_Purple,
            Neon_Yellow,
            Neon_White,

            Neon_R,
            Neon_G,
            Neon_B,

            Coal_ore,
            Bauxite,
            Hematite,
            Sphalerite, // Tin
            Calamine, // Zinc
            Galena,
            Cinnabar, // Mercury
            Argentite, // Silver
            Bismuth,

            Rudimentary_Furnace,
            Blast_Furnace
        }
    }
}
