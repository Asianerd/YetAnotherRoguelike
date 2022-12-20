using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.PhysicsObject
{
    class Player:Entity
    {
        public static Player Instance = null;
        public static Texture2D sprite = null;
        public static Vector2 spriteOrigin;

        public Point currentChunkPos;
        public Chunk[,] surroundingChunks = new Chunk[3, 3];

        public Player(Vector2 pos, Texture2D _sprite):base(pos)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            if (sprite == null)
            {
                sprite = _sprite;
                spriteOrigin = sprite.Bounds.Size.ToVector2() / 2f;
            }
        }

        public override void Update()
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
            final *= Input.collection[Keys.LeftShift].isPressed ? 0.25f : 0.12f;
            velocity = final;

            base.Update();

            currentChunkPos = Chunk.TileToChunkCoordinates((int)position.X, (int)position.Y);
            for (int x = -Chunk.chunkGenerationRange; x <= Chunk.chunkGenerationRange; x++)
            {
                for (int y = -Chunk.chunkGenerationRange; y <= Chunk.chunkGenerationRange; y++)
                {
                    Chunk.GenerateChunk(currentChunkPos.X + x, currentChunkPos.Y + y);
                }
            }

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    surroundingChunks[x + 1, y + 1] = (Chunk.FetchChunkAt(currentChunkPos.X + x, currentChunkPos.Y + y));
                }
            }
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(sprite, position * Tile.tileSize, null, Color.White, 0f, spriteOrigin, Tile.spriteRenderScale, SpriteEffects.None, 0f);

            base.Draw(spritebatch);
        }
    }
}
