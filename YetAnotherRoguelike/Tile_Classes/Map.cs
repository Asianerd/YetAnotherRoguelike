using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Map
    {
        public static List<Chunk> chunks = new List<Chunk>();
        //public static List<LightSource> lightSources = new List<LightSource>();

        public static void Initialize()
        {
            Chunk.Initialize();

            /*LightSource.Append(new LightSource(Vector2.Zero, 80, 20, Color.Red));
            LightSource.Append(new LightSource(new Vector2(10, 0), 80, 20, Color.Blue));
            LightSource.Append(new LightSource(new Vector2(5, 10), 80, 20, Color.Green));*/
        }

        public static void Update()
        {
            int resolution = 2;
            Vector2 playerChunk = Chunk.ChunkPosition(Chunk.CorrectedWorldToTile(Player.Instance.position));
            for (int y = -resolution; y <= resolution; y++)
            {
                for (int x = -resolution; x <= resolution; x++)
                {
                    Vector2 target = playerChunk + new Vector2(x, y);
                    Chunk result = FetchChunk(target);
                    if (result == null)
                    {
                        chunks.Add(new Chunk(target));
                    }
                }
            }

            foreach (Chunk x in chunks)
            {
                x.Update();
            }

            chunks = chunks.Where(n => n.active || n.custom).ToList();
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
            return FetchChunk(Chunk.ChunkPosition(Chunk.CorrectedWorldToTile(position)));
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


        #region Tile Manipulation (destroy/place)
        public static void Break(Vector2 position)
        {
            Break((int)position.X, (int)position.Y);
        }

        public static void Break(Vector2 position, bool noPosCorrection)
        {
            Break((int)position.X, (int)position.Y, true);
        }

        public static void Break(int x, int y, bool noPosCorrection)
        {
            // Takes in world-coordinates and breaks the block there
            Chunk chunk = ChunkAt(x, y);
            if (chunk == null)
            {
                return;
            }
            if (Tile.Destroy(Fetch(Chunk.WorldToTile(new Vector2(x, y)))))
            {
                chunk.custom = true;
            }
        }

        public static void Break(int x, int y)
        {
            // Takes in world-coordinates and breaks the block there
            Chunk chunk = ChunkAt(x, y);
            if (chunk == null)
            {
                return;
            }
            if (Tile.Destroy(Fetch(Chunk.CorrectedWorldToTile(new Vector2(x, y)))))
            {
                chunk.custom = true;
            }
        }


        public static void Place(Tile.Type block, Vector2 position)
        {
            Place(block, (int)position.X, (int)position.Y);
        }

        public static void Place(Tile.Type block, int x, int y)
        {
            // Takes in world-coordinates and places the block there
            Chunk chunk = ChunkAt(x, y);
            if (chunk == null)
            {
                return;
            }
            if (Fetch(Chunk.CorrectedWorldToTile(new Vector2(x, y))).type != Tile.Type.Air)
            {
                return;
            }
            Point pos = Chunk.FixTilePos(Chunk.CorrectedWorldToTile(new Vector2(x, y))).ToPoint();
            chunk.collection[pos.Y][pos.X] = Tile.CreateTile(block, Chunk.CorrectedWorldToTile(new Vector2(x, y)).ToPoint().ToVector2(), chunk);
            chunk.custom = true;
        }
        #endregion
    }
}