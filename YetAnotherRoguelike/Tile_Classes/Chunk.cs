using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Chunk
    {
        public static int size = 8;
        public static int realSize;

        public static void Initialize()
        {
            realSize = size * Tile.tileSize;
        }

        public static Vector2 ChunkPosition(Vector2 position)
        {
            return ChunkPosition((int)position.X, (int)position.Y);
        }

        public static Vector2 ChunkPosition(int x, int y) // THE PROBLEM
        {
            /// <summary>
            /// Converts tile-coordinates to chunk-coordinates
            /// </summary>
            Vector2 result = new Vector2((x + (x < 0 ? 1 : 0)) / size, (y + (y < 0 ? 1 : 0)) / size);
            if (y < 0) { result.Y -= 1; }
            if (x < 0) { result.X -= 1; }
            return result;
        }

        public static Vector2 FixTilePos(Vector2 position)
        {
            return FixTilePos((int)position.X, (int)position.Y);
        }

        public static int[] FixTilePos(int x, int y, bool trigger) // trigger does nothing, just so that overloading works
        {
            Vector2 result = FixTilePos(x, y);
            return new int[] { (int)result.X, (int)result.Y };
        }

        public static Vector2 FixTilePos(int x, int y)
        {
            // Takes in tile coordinates and returns tile coordinates
            /* Fixes tile coordinates
             *      chunk size : 8
             *      possible tile positions : 0-7
             *      
             *      input : 20
             *      output : 4
             *      
             *      input : 7
             *      output : 7
             *      
             *      input : 8
             *      output : 1
             */
            int X = (x % size) + (x >= 0 ? 0 : size);
            int Y = (y % size) + (y >= 0 ? 0 : size);
            return new Vector2(X == size ? 0 : X, Y == size ? 0 : Y);
        }

        public static Vector2 WorldToTile(Vector2 position)
        {
            return new Vector2(position.X / Tile.tileSize, position.Y / Tile.tileSize);
        }

        public static Vector2 CorrectedWorldToTile(Vector2 position)
        {
            return new Vector2((position.X / Tile.tileSize) - (position.X < 0 ? 1 : 0), (position.Y / Tile.tileSize) - (position.Y < 0 ? 1 : 0));
        }

        public static Vector2 CorrectedTileToWorld(Vector2 position)
        {
            return new Vector2((position.X * Tile.tileSize) - (position.X < 0 ? Tile.tileSize : 0), (position.Y * Tile.tileSize) - (position.Y < 0 ? Tile.tileSize : 0));
        }

        public static Vector2 TileToWorld(Vector2 position)
        {
            // used when the tile position is already corrected
            return new Vector2(position.X * Tile.tileSize, position.Y * Tile.tileSize);
        }


        public List<List<Tile>> collection = new List<List<Tile>>();
        public Vector2 position;
        public Vector2 worldPosition;
        public Rectangle rect;

        public bool active = true;
        public bool custom = false; // Whether the chunk has been customized by the player

        public Chunk(Vector2 pos, List<List<Tile>> tiles)
        {
            position = pos;
            worldPosition = (pos * size * Tile.tileSize);
            rect = new Rectangle(worldPosition.ToPoint(), new Point(realSize));
            collection = tiles;
        }

        public Chunk(Vector2 pos)
        {
            position = pos;
            worldPosition = (pos * size * Tile.tileSize);
            rect = new Rectangle(worldPosition.ToPoint(), new Point(realSize));

            for (int y = 0; y < size; y++)
            {
                collection.Add(new List<Tile>());
                for (int x = 0; x < size; x++)
                {
                    Vector2 chunkPos = new Vector2(x + (position.X * size), y + (position.Y * size));
                    //collection[y].Add(new Tile(Game.random.Next(0, 100) >= 30 ? Tile.Type.Air : Tile.Type.Stone, chunkPos));
                    //                                                              around 0.4f is good
                    //collection[y].Add(new Tile(Perlin_Noise.Fetch(CorrectedTileToWorld(chunkPos)), chunkPos, this));
                    collection[y].Add(Tile.GenerateTile(Perlin_Noise.Fetch(CorrectedTileToWorld(chunkPos), 300f), chunkPos, this));
                }
            }
        }

        public void Update(bool hard = false)
        {
            active = Vector2.Distance(Player.Instance.position, worldPosition) <= 2000;

            if ((!active) && (!hard))
            {
                if (!custom)
                {
                    UnloadPrepare();
                }
                return;
            }


            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Tile item = collection[y][x];
                    item.Update();
                    item.UpdateSprite(this);
                    if (item.durability.Percent() <= 0f)
                    {
                        Map.Break(TileToWorld(item.position), true);
                        custom = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(UI.blank, new Rectangle(rect.X + 10, rect.Y + 10, rect.Width - 20, rect.Height - 20), custom ? Color.Purple : Color.Transparent);

            foreach (List<Tile> column in collection)
            {
                foreach (Tile item in column)
                {
                    item.Draw(spriteBatch);
                }
            }
        }

        public void UnloadPrepare()
        {
            // called before unloading chunk
            foreach (List<Tile> column in collection)
            {
                foreach (Tile item in column)
                {
                    item.HardUnload();
                }
            }
        }
    }
}