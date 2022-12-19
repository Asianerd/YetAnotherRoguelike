using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Tile_Classes
{
    class Tile
    {
        public static Dictionary<BlockType, List<Texture2D>> tileSprites;
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
        }

        public static BlockType GenerateTileType(float p, Point pos)
        {
            BlockType type = BlockType.Air;
            if (p > 0.5f)
            {
                type = BlockType.Stone;
                float chance = PerlinNoise.OreInstance.GetNoise(pos.X, pos.Y);
                if (chance >= 0.54f)
                {
                    // make the ore selection system better
                    // implement rarity system
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
                    }[new Random(GeneralDependencies.CantorPairing(pos.X, pos.Y)).Next(0, 11)];
                }
            }
            return type;
        }

        /* 2 position types
         * tile coordinates
         * in-chunk coordinates
         */

        public Point tileCoordinates;
        public Point chunkCoordinates;
        public BlockType type;
        int spriteIndex = 0;

        bool isAir;

        public Tile(Point tCoords, Point cCoords, BlockType t)
        {
            type = t;

            isAir = type == BlockType.Air;

            tileCoordinates = tCoords;
            chunkCoordinates = cCoords;
        }

        public virtual void UpdateSprite()
        {
            bool u = Chunk.FetchTypeAt(tileCoordinates.X, tileCoordinates.Y - 1) != BlockType.Air;
            bool r = Chunk.FetchTypeAt(tileCoordinates.X + 1, tileCoordinates.Y) != BlockType.Air;
            bool d = Chunk.FetchTypeAt(tileCoordinates.X, tileCoordinates.Y + 1) != BlockType.Air;
            bool l = Chunk.FetchTypeAt(tileCoordinates.X - 1, tileCoordinates.Y) != BlockType.Air;

            spriteIndex = (u ? 0 : 1) + (r ? 0 : 2) + (d ? 0 : 4) + (l ? 0 : 8);
        }

        public virtual void Update()
        {

        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            if (isAir)
            {
                return;
            }

            UpdateSprite();

            Vector2 renderedPosition = tileCoordinates.ToVector2() * tileSize;
            spritebatch.Draw(tileSprites[type][spriteIndex], renderedPosition, null, Color.White, 0f, spriteOrigin, spriteRenderScale, SpriteEffects.None, 0f);
        }

        public virtual void OnDestroy() { } // when block is destroyed
        public virtual void OnReload() { }      // when unloaded chunk is reloaded
        public virtual void OnSoftUnload() { }  // when loaded chunk is unloaded
        public virtual void OnChunkDelete() { } // when chunk is deleted

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
