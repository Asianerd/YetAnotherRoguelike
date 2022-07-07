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

            /*int resolution = 2;
            for (int y = -resolution; y <= resolution; y++)
            {
                for (int x = -resolution; x <= resolution; x++)
                {
                    chunks.Add(new Chunk(new Vector2(x, y)));
                }
            }*/

            //chunks.Add(new Chunk(new Vector2(-1, 0)));
            chunks.Add(new Chunk(new Vector2(-3, 0)));
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
            // Returns the tile at the real world coordinate
            // null if no tile is found / coordinates isnt in an existing chunk

            // 1. locate chunk
            // 2. calculate coordinates in chunk
            // 3. return tile

            Chunk chunkResult = ChunkAt(x, y, true); // problem maybe?
            if (chunkResult == null)
            {
                return null;
            }

            int fixedX = x >= 0 ? (x % Chunk.size) : (((x + 1) % Chunk.size - 1) + Chunk.size); // problem maybe?
            int fixedY = y >= 0 ? (y % Chunk.size) : (((y + 1) % Chunk.size - 1) + Chunk.size); //

/*            Debug.WriteLine($"{x} : {y}");
            Debug.WriteLine($"{fixedX} : {fixedY}");
            Debug.WriteLine("\n");*/


            return chunkResult.collection[fixedY][fixedX];
        }

        public static Tile.Type TypeAt(Vector2 position)
        {
            // Returns the tile type at the real world coordinate
            // null if no tile is found / coordinates isnt in an existing chunk

            Tile result = Fetch((int)position.X, (int)position.Y);
            if (result == null)
            {
                return Tile.Type.Air;
            }

            return result.type;
        }

        public static Chunk ChunkAt(int x, int y, bool print = false)
        {
            // Takes in real world coordinates and returns a chunk that contains that coordinate
            // If no chunk is found, null is returned
            if (print)
            {
                Debug.WriteLine($"ChunkAt Input : ({x}, {y})");
            }
            Vector2 chunkPosition = Chunk.ChunkPosition(x, y);
            if (print)
            {
                Debug.WriteLine(chunkPosition);
                Debug.WriteLine("\n\n");
            }
            foreach (Chunk chunk in chunks)
            {
                if (chunk.position == chunkPosition)
                {
                    if (print)
                    {
                        Debug.WriteLine($"Chunk pos : {chunk.position}");
                    }
                    return chunk;
                }
            }
            /*Point target = new Point(x, y);
            foreach (Chunk chunk in chunks)
            {
                if (chunk.rect.Contains(target))
                {
                    return chunk;
                }
            }*/
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