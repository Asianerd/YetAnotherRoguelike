using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace YetAnotherRoguelike
{
    public static class GeneralDependencies
    {
        public static Dictionary<Keys, Vector2> axialVectors;

        public static void Initialize()
        {
            axialVectors = new Dictionary<Keys, Vector2>()
            {
                { Keys.W, new Vector2(0, -1) },
                { Keys.S, new Vector2(0, 1) },
                { Keys.A, new Vector2(-1, 0) },
                { Keys.D, new Vector2(1, 0) },

                { Keys.Up, new Vector2(0, -1) },
                { Keys.Down, new Vector2(0, 1) },
                { Keys.Left, new Vector2(-1, 0) },
                { Keys.Right, new Vector2(1, 0) },
            };
        }

        public static Color Blend(Color a, Color b)
        {
            return new Color(
                ((float)a.R / 2f) + ((float)b.R / 2f),
                ((float)a.G / 2f) + ((float)b.G / 2f),
                ((float)a.B / 2f) + ((float)b.B / 2f)
                );
        }

        public static Color Blend(Color a, Color b, float compensation)
        {
            a.R += (byte)(b.R * compensation);
            a.G += (byte)(b.G * compensation);
            a.B += (byte)(b.B * compensation);
            return a;
        }

        public static int CantorPairing(int a, int b)
        {
            return (int)(0.5 * (a + b) * (a + b + 1) + b);
        }
    }
}
