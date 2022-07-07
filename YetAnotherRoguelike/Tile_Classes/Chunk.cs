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
            // Takes in world coordinates and returns which chunk coordinate its in
            return new Vector2((int)((x / realSize) - (x < 0 ? 1 : 0)), (int)((y / realSize) - (y < 0 ? 1 : 0)));
            //return new Vector2((int)((x / size) - (x < 0 ? 1 : 0)), (int)((y / size) - (y < 0 ? 1 : 0)));
            //return new Vector2(x / (size), y / (size));
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

            int count = 0;
            for (int y = 0; y < size; y++)
            {
                collection.Add(new List<Tile>());
                for (int x = 0; x < size; x++)
                {
                    Vector2 chunkPos = new Vector2(x + (position.X * size), y + (position.Y * size));
                    //collection[y].Add(new Tile(Game.random.Next(0, 100) >= 30 ? Tile.Type.Air : Tile.Type.Stone, chunkPos));
                    collection[y].Add(new Tile(y == 0 ? Tile.Type.Stone : Tile.Type.Air, chunkPos));
                    count++;
                }
            }
        }

        public void Update(bool hard = false)
        {
            active = Vector2.Distance(Player.Instance.position, worldPosition) <= 2000;

            if ((!active) || (!hard))
            {
                return;
            }

            foreach (List<Tile> column in collection)
            {
                foreach (Tile item in column)
                {
                    item.Update();
                    item.UpdateSprite();
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

            spriteBatch.DrawString(UI.defaultFont, $"{Map.ChunkAt(rect.Center.X, rect.Center.Y).position}\n{position.ToString()}", rect.Location.ToVector2(), Color.White);
        }
    }
}