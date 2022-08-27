using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace YetAnotherRoguelike
{
    class Perlin_Noise
    {
        public static List<List<float>> map = new List<List<float>>();
        public static Random random = new Random(Game.seed);
        // size means how many chunks are generated in each cardinal direction
        public static int size = 512;
        public static int scale = 256;

        public static void Initialize()
        {
            map = new List<List<float>>();
            int current = 0;
            for (int y = -size; y < size; y++)
            {
                map.Add(new List<float>());
                for (int x = -size; x < size; x++)
                {
                    map[current].Add(random.Next(0, 10000) / 10000f);
                }
                current++;
            }
        }

        public static float Fetch(Vector2 position)
        {
            return Fetch(position, scale);
        }

        public static float Fetch(Vector2 position, float _s)
        {
            position /= _s;

            float xPercent = position.X - (float)Math.Floor(position.X);
            float yPercent = position.Y - (float)Math.Floor(position.Y);

            int upX = (int)Math.Floor(position.X) + size;
            int upY = (int)Math.Floor(position.Y) + size;

            Point topLeft = new Point(upX, upY);
            Point topRight = new Point(upX + 1, upY);

            Point bottomLeft = new Point(upX, upY + 1);
            Point bottomRight = new Point(upX + 1, upY + 1);

            float top = GameValue.Lerp(map[topLeft.Y][topLeft.X], map[topRight.Y][topRight.X], xPercent);
            float bottom = GameValue.Lerp(map[bottomLeft.Y][bottomLeft.X], map[bottomRight.Y][bottomRight.X], xPercent);
            return GameValue.Lerp(top, bottom, yPercent);
        }
    }
}
