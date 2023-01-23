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
        public static Texture2D chunkBorderSprite;
        public static Rectangle collisionRect; // rect used to check collision, just move the rect to the tile's location to check collision

        public static List<Chunk> chunks = new List<Chunk>();
        public static int chunkSize = 16;
        public static int chunkGenerationRange = 1; // generates chunks in a 5x5 box around the player
        /* . . . . . . .
         * . X X X X X .
         * . X X X X X .
         * . X X P X X .
         * . X X X X X .
         * . X X X X X .
         * . . . . . . .
         */

        public static void Initialize()
        {
            chunkBorderSprite = Game.Instance.Content.Load<Texture2D>("Debug/chunk_border");
            collisionRect = new Rectangle(0, 0, 1, 1);
        }

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

        #region Collision
        public static bool CollideRect(Vector2 position, Vector2 size)
        {
            /*foreach (Chunk x in chunks)
            {
                if (x.CollideTiles(position, size))
                {
                    return true;
                }
            }
            return false;*/

            foreach (Chunk x in chunks)
            {
                if (x.CollideTiles(position, size))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CollideRect(Rectangle rect)
        {
            // implement distance checking for optimization
            foreach (Chunk x in chunks)
            {
                if (x.CollideTiles(rect))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Fetching functions
        public static Chunk FetchChunkWithCoords(int x, int y)
        {
            foreach (Chunk c in chunks)
            {
                if (c.position.X == x)
                {
                    if (c.position.Y == y)
                    {
                        return c;
                    }
                }
            }

            return null;
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
        #endregion

        #region Unit conversions
        public static Point TileToChunkCoordinates(int x, int y)
        {
            return new Point((int)Math.Floor(x / (float)chunkSize), (int)Math.Floor(y / (float)chunkSize));
        }

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
        #endregion

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

        public Tile[,] contents = new Tile[chunkSize, chunkSize]; // 8x8 chunk size
        public Random tileRandom;
        public Point position;
        public Rectangle rect;
        public Rectangle drawnRect; // normal rect * tile size

        public bool loaded = false; // if chunk is loaded

        public bool modified = false;
        public bool dead = false; // if dead, remove after updating all chunks

        public Chunk(Point p)
        {
            position = p;
            rect = new Rectangle((position.ToVector2() * chunkSize).ToPoint(), new Point(chunkSize, chunkSize));
            drawnRect = new Rectangle((position.ToVector2() * chunkSize * Tile.tileSize).ToPoint(), (new Vector2(chunkSize * Tile.tileSize)).ToPoint());
            tileRandom = new Random(GeneralDependencies.CantorPairing(position.X, position.Y));
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    Point blockPos = new Point(x + (chunkSize * position.X), y + (chunkSize * position.Y));
                    contents[x, y] = Tile.CreateTile(blockPos, new Point(x, y), Tile.GenerateTileType(PerlinNoise.Instance.GetNoise(blockPos.X, blockPos.Y), blockPos, this));
                }
            }

            OnBlockModify(true);
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
                    if (contents[x, y].durability.Percent() <= 0f)
                    {
                        contents[x, y].OnDestroy();
                        contents[x, y] = new Tile(new Point(contents[x, y].tileCoordinates.X, contents[x, y].tileCoordinates.Y), new Point(x, y), Tile.BlockType.Air); // error at char 61

                        modified = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (!loaded)
            {
                return;
            }

            //spritebatch.Draw(Game.emptySprite, new Rectangle((int)(rect.X * Tile.tileSize) + 10, (int)(rect.Y * Tile.tileSize) + 10, (int)(rect.Width * Tile.tileSize) - 20, (int)(rect.Height * Tile.tileSize) - 20), Color.Green);

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    contents[x, y].Draw(spritebatch);
                }
            }

            if (Game.showDebug)
            {
                spritebatch.Draw(chunkBorderSprite, drawnRect, Color.White);
            }
        }

        #region Tile manipulation
        public void ReplaceTile(Point target, Tile newTile)
        {
            contents[target.X, target.Y].OnDestroy();
            contents[target.X, target.Y] = newTile;
        }

        /*public bool CollideTiles(Vector2 position, Vector2 size)
        {
            Rectangle targetRect = new Rectangle(
                (int)(position.X * Game.physicsQuality),
                (int)(position.Y * Game.physicsQuality),
                (int)(size.X * Game.physicsQuality),
                (int)(size.Y * Game.physicsQuality)
                );

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    if (contents[x, y].type == Tile.BlockType.Air)
                    {
                        continue;
                    }

                    collisionRect.X = (int)((x + position.X) * Game.physicsQuality);
                    collisionRect.Y = (int)((y + position.Y) * Game.physicsQuality);
                    collisionRect.Width = Game.physicsQuality;
                    collisionRect.Height = Game.physicsQuality;

                    if (targetRect.Intersects(collisionRect))
                    {
                        return true;
                    }
                }
            }
            return false;
        }*/

        public bool CollideTiles(Vector2 p, Vector2 s)
        {
            // position and size are in tile-coordinates
            Rectangle targetRect = new Rectangle((p * Game.physicsQuality).ToPoint(), (s * Game.physicsQuality).ToPoint());
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    if (contents[x, y].type == Tile.BlockType.Air)
                    {
                        continue;
                    }

                    collisionRect.X = (int)((x + (position.X * chunkSize)) * Game.physicsQuality);
                    collisionRect.Y = (int)((y + (position.Y * chunkSize)) * Game.physicsQuality);
                    collisionRect.Height = Game.physicsQuality;
                    collisionRect.Width = Game.physicsQuality;

                    if (collisionRect.Intersects(targetRect))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CollideTiles(Rectangle rect)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    if (contents[x, y].type == Tile.BlockType.Air)
                    {
                        continue;
                    }

                    collisionRect.X = x * Game.physicsQuality;
                    collisionRect.Y = y * Game.physicsQuality;
                    collisionRect.Width = Game.physicsQuality;
                    collisionRect.Height = Game.physicsQuality;

                    if (rect.Intersects(collisionRect))
                    {
                        return true;
                    }
                }
            }
            return false;
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
        #endregion

        #region Events
        public void OnBlockModify(bool updateNeighbours) // if any of the blocks in the chunk are broken/replaced
        {
            if (updateNeighbours)
            {
                for (int cx = -1; cx <= 1; cx++)
                {
                    for (int cy = -1; cy <= 1; cy++)
                    {
                        Chunk result = FetchChunkWithCoords(position.X + cx, position.Y + cy);
                        if (result != null)
                        {
                            result.OnBlockModify(false);
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int x = 0; x < chunkSize; x++)
                    {
                        contents[x, y].updateSpriteNextFrame = true;
                    }
                }
            }
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
        #endregion
    }
}
