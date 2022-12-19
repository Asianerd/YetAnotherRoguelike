using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public static float FullRadian(float radian)
        {
            if (radian < 0)
            {
                return (float)((Math.PI - MathF.Abs(radian)) + Math.PI);
            }
            else
            {
                return radian;
            }
        }

        public static float FullDegrees(float degrees)
        {
            if (degrees < 0)
            {
                return degrees + 360;
            }
            return degrees;
        }

        public static bool inBetween(float start, float end, float target)
        {
            if (end > start)
            {
                return (target > start) && (target < end);
            }
            else
            {
                return (target < start) && (target > end);
            }
        }

        // from stackoverflow
        // thanks @michael-hoffmann
        public static List<Texture2D> Split(Texture2D original, int partWidth, int partHeight)
        {
            int yCount = original.Height / partHeight;
            int xCount = original.Height / partHeight;
            List<Texture2D> r = new List<Texture2D>();
            int dataPerPart = partWidth * partHeight;

            Color[] originalData = new Color[original.Width * original.Height];
            original.GetData<Color>(originalData);

            for (int y = 0; y < (yCount * partHeight); y += partHeight)
            {
                for (int x = 0; x < (xCount * partWidth); x += partWidth)
                {
                    Texture2D part = new Texture2D(original.GraphicsDevice, partWidth, partHeight);
                    Color[] partData = new Color[dataPerPart];

                    for (int py = 0; py < partHeight; py++)
                    {
                        for (int px = 0; px < partWidth; px++)
                        {
                            int partIndex = px + py * partWidth;
                            if (y + py >= original.Height || x + px >= original.Width)
                            {
                                partData[partIndex] = Color.Transparent;
                            }
                            else
                            {
                                partData[partIndex] = originalData[(x + px) + ((y + py) * original.Width)];
                            }
                        }
                    }

                    part.SetData<Color>(partData);
                    r.Add(part);
                }
            }
            return r;
        }
    }
}
