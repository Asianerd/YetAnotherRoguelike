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
            return position / Tile.tileSize;
        }


        public List<List<Tile>> collection = new List<List<Tile>>();
        public Vector2 position;
        public Vector2 worldPosition;
        public Rectangle rect;

        public bool active = true;

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
                    collection[y].Add(new Tile(Game.random.Next(0, 100) >= 30 ? Tile.Type.Air : Tile.Type.Stone, chunkPos));
                }
            }
        }

        public void Update(bool hard = false)
        {
            active = Vector2.Distance(Player.Instance.position, worldPosition) <= 2000;


            if ((!active) && (!hard)) // TODO : Remove the or gate
            {
                return;
            }

            foreach (List<Tile> column in collection)
            {
                foreach (Tile item in column)
                {
                    item.Update();
                    item.UpdateSprite(this);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(UI.blank, new Rectangle(rect.X + 10, rect.Y + 10, rect.Width - 20, rect.Height - 20), active ? Color.Green : Color.Red);

            foreach (List<Tile> column in collection)
            {
                foreach (Tile item in column)
                {
                    item.Draw(spriteBatch);
                }
            }
        }
    }
}