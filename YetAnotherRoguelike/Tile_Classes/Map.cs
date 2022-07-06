using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Map
    {
        public static List<Chunk> chunks = new List<Chunk>();

        public static void Initialize()
        {
            Chunk.Initialize();

            int resolution = 2;
            for (int y = -resolution; y <= resolution; y++)
            {
                for (int x = -resolution; x <= resolution; x++)
                {
                    chunks.Add(new Chunk(new Vector2(x, y)));
                }
            }

            //chunks.Add(new Chunk(new Vector2(-1, 0)));
            //chunks.Add(new Chunk(new Vector2(1, 0)));
        }

        public static void Update()
        {
            int resolution = 0;
            for (int y = -resolution; y <= resolution; y++)
            {
                for (int x = -resolution; x <= resolution; x++)
                {
                    if (ChunkAt((int)Player.Instance.position.X + (x * Chunk.realSize), (int)Player.Instance.position.Y + (y * Chunk.realSize)) == null)
                    {
                        //chunks.Add(new Chunk(Chunk.ChunkPosition(Player.Instance.position) + new Vector2(x, y)));
                    }
                }
            }

            foreach (Chunk x in chunks)
            {
                x.Update();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Chunk x in chunks)
            {
                x.Draw(spriteBatch);
            }
        }

        #region Tile fetching
        public static Tile Fetch(int x, int y) // NOT FETCHING THE CORRECT TILES
        {
            Chunk chunkResult = ChunkAt(x, y);
            if (chunkResult == null)
            {
                return null;
            }

            // PROBLEM HERE
            // negative numbers not properly converted
            //int fixedX = x % Chunk.size + (x >= 0 ? 0 : Chunk.size), fixedY = y % Chunk.size + (y >= 0 ? 0 : Chunk.size);
            int fixedX = x >= 0 ? (x % Chunk.size) : (((x + 1) % Chunk.size) - 1) + Chunk.size;
            int fixedY = y >= 0 ? (y % Chunk.size) : (((y + 1) % Chunk.size) - 1) + Chunk.size;
            //((i + 1) % chunkSize - 1) + chunkSize
            if ((fixedX >= Chunk.size) || (fixedX < 0))
            {
                return null;
            }
            if ((fixedY >= Chunk.size) || (fixedY < 0))
            {
                return null;
            }

            return chunkResult.collection[fixedY][fixedX];
        }

        public static Tile Fetch(Vector2 position)
        {
            return Fetch((int)position.X, (int)position.Y);
        }

        public static Tile Fetch(Point position)
        {
            return Fetch(position.X, position.Y);
        }

        public static Tile.Type TypeAt(Vector2 position)
        {
            Tile result = Fetch(position);
            if (result == null)
            {
                return Tile.Type.Air;
            }
            else
            {
                return result.type;
            }
        }

        public static Chunk ChunkAt(int x, int y)
        {
            // takes in real world coordinates and returns the chunk at that position
            Vector2 chunkPosition = Chunk.ChunkPosition(x, y);
            foreach (Chunk chunk in chunks)
            {
                if (chunk.position == chunkPosition)
                {
                    return chunk;
                }
            }
            return null;
        }
        #endregion

        #region Collision
        public static bool CollideTiles(Rectangle rect)
        {
            foreach (Chunk chunk in chunks)
            {
                if (!rect.Intersects(chunk.rect))
                {
                    continue;
                }

                foreach (List<Tile> column in chunk.collection)
                {
                    foreach (Tile tile in column)
                    {
                        if (tile.type == Tile.Type.Air)
                        {
                            continue;
                        }

                        if (rect.Intersects(tile.rect))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;

            /*foreach (List<Tile> column in collection)
            {
                foreach (Tile item in column)
                {
                    if (item.type == Tile.Type.Air)
                    {
                        continue;
                    }
                    if (rect.Intersects(item.rect))
                    {
                        return true;
                    }
                }
            }
            return false;*/
        }
        #endregion
    }
}