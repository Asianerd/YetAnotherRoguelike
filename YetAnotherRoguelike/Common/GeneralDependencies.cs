﻿using System;
using System.Globalization;
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

        public static float Lerp(float start, float end, float progress, float snapWeight = 0)
        {
            if (snapWeight == 0)
            {
                return start + progress * (end - start);
            }
            if (MathF.Abs(start - end) <= snapWeight)
            {
                return end;
            }
            return start + progress * (end - start);
        }

        public static Color HexToColor(string hex)
        {
            // dont include the # before the color

            //int r, g, b;

            //r = ((int)(hex[0]) * 16) + 
            /*Color final = new Color(uint.Parse(hex, System.Globalization.NumberStyles.HexNumber));
            // this returns BGR

            byte temp = final.R;
            final.R = final.B;
            final.B = temp;
            final.A = (byte)255;
            // this turns BGR to RGB*/

            return new Color(
                (int.Parse(hex[0].ToString(), NumberStyles.HexNumber) * 16) + (int.Parse(hex[1].ToString(), NumberStyles.HexNumber)),
                (int.Parse(hex[2].ToString(), NumberStyles.HexNumber) * 16) + (int.Parse(hex[3].ToString(), NumberStyles.HexNumber)),
                (int.Parse(hex[4].ToString(), NumberStyles.HexNumber) * 16) + (int.Parse(hex[5].ToString(), NumberStyles.HexNumber)),
                hex.Length == 8 ?
                (int.Parse(hex[6].ToString(), NumberStyles.HexNumber) * 16) + (int.Parse(hex[7].ToString(), NumberStyles.HexNumber))
                : 255
                );
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

        public static void NineSliceDraw(SpriteBatch sb, List<Texture2D> s, Rectangle r, int p, Color c, float l)
        {
            sb.Draw(s[0], new Rectangle(r.X         , r.Y, p                , p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[1], new Rectangle(r.X + p     , r.Y, r.Width - p - p  , p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[2], new Rectangle(r.Right - p , r.Y, p                , p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);

            sb.Draw(s[3], new Rectangle(r.X         , r.Y + p, p                , r.Height - p - p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[4], new Rectangle(r.X + p     , r.Y + p, r.Width - p - p  , r.Height - p - p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[5], new Rectangle(r.Right - p , r.Y + p, p                , r.Height - p - p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);

            sb.Draw(s[6], new Rectangle(r.X         , r.Bottom - p, p               , p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[7], new Rectangle(r.X + p     , r.Bottom - p, r.Width - p - p , p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[8], new Rectangle(r.Right - p , r.Bottom - p, p               , p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
        }

        public static void NineSliceDraw(SpriteBatch sb, List<Texture2D> s, Rectangle r, int p, Color c, float l, Point o)
        {
            sb.Draw(s[0], new Rectangle(r.X + o.X, r.Y + o.Y, p, p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[1], new Rectangle(r.X + o.X + p, r.Y + o.Y, r.Width - p - p, p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[2], new Rectangle(r.Right + o.X - p, r.Y + o.Y, p, p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);

            sb.Draw(s[3], new Rectangle(r.X + o.X, r.Y + o.Y + p, p, r.Height - p - p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[4], new Rectangle(r.X + o.X + p, r.Y + o.Y + p, r.Width - p - p, r.Height - p - p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[5], new Rectangle(r.Right + o.X - p, r.Y + o.Y + p, p, r.Height - p - p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);

            sb.Draw(s[6], new Rectangle(r.X + o.X, r.Bottom + o.Y - p, p, p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[7], new Rectangle(r.X + o.X + p, r.Bottom + o.Y - p, r.Width - p - p, p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
            sb.Draw(s[8], new Rectangle(r.Right + o.X - p, r.Bottom + o.Y - p, p, p), null, c, 0f, Vector2.Zero, SpriteEffects.None, l);
        }

        public static void DrawLine(SpriteBatch sb, Point from, Point to, float w, Color c, float l)
        {
            float deg = MathF.Atan2(to.Y - from.Y, to.X - from.X);
            float dist = MathF.Sqrt(MathF.Pow(from.X - to.X, 2) + MathF.Pow(from.Y - to.Y, 2));

            sb.Draw(UI.UI_Element.twoXBlank, from.ToVector2(), null, c, deg, new Vector2(0, 1), new Vector2(dist / 2, w), SpriteEffects.None, l);
        }
    }
}
