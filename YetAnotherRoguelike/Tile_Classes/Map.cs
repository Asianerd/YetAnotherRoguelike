using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Map
    {
        public static List<List<Tile>> collection = new List<List<Tile>>();
        public static int size = 256;
        // Chunks should be 42 * 42 size

        public static void Initialize()
        {
            for (int y = 0; y < size; y++)
            {
                collection.Add(new List<Tile>());
                for (int x = 0; x < size; x++)
                {
                    collection[y].Add(new Tile(Game.random.Next(0, 100) >= 50 ? Tile.Type.Stone : Tile.Type.Air, new Vector2(x, y)));
                }
            }

            foreach (List<Tile> y in collection)
            {
                foreach (Tile x in y)
                {
                    x.UpdateSprite();
                }
            }
        }

        public static void Update()
        {
            foreach(List<Tile> y in collection)
            {
                foreach (Tile x in y)
                {
                    x.Update();
                    x.UpdateSprite();
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (List<Tile> y in collection)
            {
                foreach (Tile x in y)
                {
                    x.Draw(spriteBatch);
                }
            }
        }

        #region Tile fetching
        public static Tile Fetch(int x, int y)
        {
            if ((x >= size) || (x < 0))
            {
                return null;
            }
            if ((y >= size) || (y < 0))
            {
                return null;
            }

            return collection[y][x];
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
        #endregion

        #region Collision
        public static bool CollideTiles(Rectangle rect)
        {
            foreach (List<Tile> column in collection)
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
            return false;
        }
        #endregion
    }
}
