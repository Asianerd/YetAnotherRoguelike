using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Tile_Classes
{
    class Chunk
    {
        public static List<Chunk> chunks = new List<Chunk>();
        public static int chunkSize = 16;
        public static int chunkGenerationRange = 2; // generates chunks in a 5x5 box around the player
        /* . . . . . . .
         * . X X X X X .
         * . X X X X X .
         * . X X P X X .
         * . X X X X X .
         * . X X X X X .
         * . . . . . . .
         */

        public static void GenerateChunk(int x, int y) // If chunk with that coordinate doesnt exist, generate the chunk
        {
            foreach (Chunk c in chunks)
            {
                if (c.position.X == x)
                {
                    if (c.position.Y == y)
                    {
                        return;
                    }
                }
            }

            chunks.Add(new Chunk(new Point(x, y)));
        }

        public static Point TileToChunkCoordinates(int x, int y)
        {
            return new Point((int)Math.Floor(x / (float)chunkSize), (int)Math.Floor(y / (float)chunkSize));
        }

        public static Chunk FetchChunkAt(int x, int y) // Fetches the chunk that contains the tile-coordinate
        {
            int tx = (int)Math.Floor(x / (float)chunkSize);
            int ty = (int)Math.Floor(y / (float)chunkSize);
            
            foreach (Chunk c in chunks)
            {
                if (c.position.X == tx)
                {
                    if (c.position.Y == ty)
                    {
                        return c;
                    }
                }
            }

            return null;
        }

        public static Tile FetchTileAt(int x, int y) // Fetches the tile at the tile-coordinate
        {
            Chunk _c = FetchChunkAt(x, y);

            if (_c == null)
            {
                return null;
            }

            return _c.contents[FixCoords(x), FixCoords(y)];
        }

        public static Tile.BlockType FetchTypeAt(int x, int y)
        {
            Tile result = FetchTileAt(x, y);
            if (result == null)
            {
                return Tile.BlockType.Air;
            }
            return result.type;
        }

        #region Chunk-specific functions
        // instead of iterating all chunks in memory, only iterate through a few specific ones
        public static Chunk FetchChunkAt(int x, int y, List<Chunk> collection) // Fetches the chunk that contains the tile-coordinate
        {
            int tx = (int)Math.Floor(x / (float)chunkSize);
            int ty = (int)Math.Floor(y / (float)chunkSize);

            foreach (Chunk c in collection)
            {
                if (c.position.X == tx)
                {
                    if (c.position.Y == ty)
                    {
                        return c;
                    }
                }
            }

            return null;
        }

        public static Tile FetchTileAt(int x, int y, List<Chunk> collection) // Fetches the tile at the tile-coordinate
        {
            Chunk _c = FetchChunkAt(x, y, collection);

            if (_c == null)
            {
                return null;
            }

            return _c.contents[FixCoords(x), FixCoords(y)];
        }

        public static Tile.BlockType FetchTypeAt(int x, int y, List<Chunk> collection)
        {
            Tile result = FetchTileAt(x, y, collection);
            if (result == null)
            {
                return Tile.BlockType.Air;
            }
            return result.type;
        }
        #endregion

        // eg : [10, 1] -> [3, 1]
        public static int FixCoords(int i) // converts tile-coordinate to in-chunk coordinate
        {
            bool negative = Math.Sign(i) == -1;

            if (!negative)
            {
                return i % chunkSize;
            }

             return (chunkSize + ((i + 1) % chunkSize)) - 1;
        }

        public Tile[,] contents = new Tile[chunkSize, chunkSize]; // 8x8 chunk size
        public Random tileRandom;
        public Point position;
        public Rectangle rect;

        public bool loaded = false; // if chunk is loaded

        public bool modified = false;
        public bool dead = false; // if dead, remove after updating all chunks

        public Chunk(Point p)
        {
            position = p;
            rect = new Rectangle((position.ToVector2() * chunkSize).ToPoint(), new Point(chunkSize, chunkSize));
            tileRandom = new Random(GeneralDependencies.CantorPairing(position.X, position.Y));
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    Point blockPos = new Point(x + (chunkSize * position.X), y + (chunkSize * position.Y));
                    contents[x, y] = Tile.CreateTile(blockPos, new Point(x, y), Tile.GenerateTileType(PerlinNoise.Instance.GetNoise(blockPos.X, blockPos.Y), blockPos, this));
                }
            }
/*
            if (position == new Point(0))
            {
                contents[0, 0] = Tile.CreateTile(new Point(0), new Point(0), Tile.BlockType.Neon_Blue);
            }*/
        }

        public void Update()
        {
            if (dead)
            {
                return;
            }

            bool previous = loaded;
            loaded = Game.playArea.Intersects(rect);
            // theres definitely a faster way of doing this
            /* if loaded
             *      - load chunk
             * if not loaded
             *      - if modified
             *          - unload chunk
             *      - if not modified
             *          - delete chunk
             */
            if (previous)
            {
                if (!loaded)
                {
                    UnloadChunk();
                }
            }
            else
            {
                if (loaded)
                {
                    LoadChunk();
                }
            }

            if (!loaded)
            {
                if (!modified)
                {
                    dead = true;

                    DeleteChunk();
                }
            }

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    contents[x, y].Update();
                }
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (!loaded)
            {
                return;
            }

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    contents[x, y].Draw(spritebatch);
                }
            }
        }

        public Tile LocalFetchTile(int x, int y) // parameters in tile-coordinates
        {
            if (x < 0)
            {
                return null;
            }
            if (x >= chunkSize)
            {
                return null;
            }

            if (y < 0)
            {
                return null;
            }
            if (x >= chunkSize)
            {
                return null;
            }

            return contents[x, y];
        }

        public Tile.BlockType LocalFetch(int x, int y)
        {
            if (x < 0)
            {
                return Tile.BlockType.Air;
            }
            if (x >= chunkSize)
            {
                return Tile.BlockType.Air;
            }

            if (y < 0)
            {
                return Tile.BlockType.Air;
            }
            if (x >= chunkSize)
            {
                return Tile.BlockType.Air;
            }

            return contents[x, y].type;
        }

        public void LoadChunk()
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    contents[x, y].OnReload();
                }
            }
        }

        public void UnloadChunk()
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    contents[x, y].OnUnload();
                }
            }
        }

        public void DeleteChunk()
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    contents[x, y].OnChunkDelete();
                }
            }
        }
    }
}
