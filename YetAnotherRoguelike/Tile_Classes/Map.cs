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

            //chunks.Add(new Chunk(new Vector2(0, 0)));
            //chunks.Add(new Chunk(new Vector2(-3, 0)));
            /*chunks.Add(new Chunk(new Vector2(-3, 1)));
            chunks.Add(new Chunk(new Vector2(-2, 0)));
            chunks.Add(new Chunk(new Vector2(-2, 1)));*/

            foreach (Chunk x in chunks)
            {
                Debug.WriteLine(x.position);
            }

            /*foreach (Chunk chunk in chunks)
            {
                foreach (List<Tile> column in chunk.collection)
                {
                    foreach (Tile item in column)
                    {
                        Debug.WriteLine(item.position);
                    }
                }
            }*/
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
        public static Tile Fetch(int x, int y)
        {
            /// <summary>
            /// Returns tile at the position; null if no tile
            /// Takes in block-coordinates
            /// </summary>

            // 1. find chunk
            // 2. return block

            Vector2 chunkPosition = Chunk.ChunkPosition(x, y);
            Debug.WriteLine($"Input : ({x}, {y})");
            Debug.WriteLine(chunkPosition);
            Debug.WriteLine("\n");
            Chunk chunk = FetchChunk(chunkPosition);
            if (chunk == null)
            {
                return null;
            }

            int[] tilePosition = Chunk.FixTilePos(x, y, true);
            return chunk.collection[tilePosition[1]][tilePosition[0]];
        }

        public static Tile Fetch(Vector2 position)
        {
            return Fetch((int)position.X, (int)position.Y);
        }

        public static Chunk FetchChunk(Vector2 position)
        {
            /// <summary>
            /// Returns chunk at the position
            /// Takes in chunk-coordinates
            /// </summary>

            foreach(Chunk chunk in chunks)
            {
                if (chunk.position == position)
                {
                    return chunk;
                }
            }
            return null;
        }

        public static Chunk ChunkAt(Vector2 position)
        {
            // Return chunk that contains the world position
            return FetchChunk(Chunk.ChunkPosition(Chunk.WorldToTile(position)));
        }

        public static Chunk ChunkAt(int x, int y)
        {
            // Return chunk that contains the world position
            return ChunkAt(new Vector2(x, y));
        }

        public static Tile.Type TypeAt(Vector2 position)
        {
            // Takes tile-coordinate and returns the tile type there
            Tile result = Fetch(position);
            if (result == null)
            {
                return Tile.Type.Air;
            }
            return result.type;
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