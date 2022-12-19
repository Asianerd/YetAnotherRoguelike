using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using YetAnotherRoguelike.Graphics;

namespace YetAnotherRoguelike.Graphics
{
    class Lightmap
    {
        //public static List<List<Color>> lightmap;
        public static RenderTarget2D renderTarget;
        // no idea why Gaussian blur needs 2 RT
        // why not clear the first one and reuse it???
        public static RenderTarget2D GaussianRT1;
        public static RenderTarget2D GaussianRT2;

        public static Texture2D final;
        public static Texture2D lightmap;
        public static Color[] lightmapArray;

        public static Rectangle rect;
        public static Point size;

        public static float resolution = 0.5f;
        // the lower the better

        public static void Initialize()
        {
            size = new Point(
                (int)Math.Ceiling(Game.screenSize.X / (Tile.tileSize * resolution)),
                (int)Math.Ceiling(Game.screenSize.Y / (Tile.tileSize * resolution))
                );
            lightmapArray = new Color[size.Y * size.X];
            // TODO : Update when screen is resized
            renderTarget = new RenderTarget2D(
                Game.graphics.GraphicsDevice,
                (int)Math.Ceiling(Game.screenSize.X),
                (int)Math.Ceiling(Game.screenSize.Y)
                );

            // GaussianBlur initialization
            GaussianRT1 = new RenderTarget2D(
                Game.graphics.GraphicsDevice,
                (int)Math.Ceiling(Game.screenSize.X),
                (int)Math.Ceiling(Game.screenSize.Y)
                );
            GaussianRT2 = new RenderTarget2D(
                Game.graphics.GraphicsDevice,
                (int)Math.Ceiling(Game.screenSize.X),
                (int)Math.Ceiling(Game.screenSize.Y)
                );
            GaussianBlur.Instance.ComputeKernel(7, 1f);
            GaussianBlur.Instance.ComputeOffsets(Game.screenSize.X, Game.screenSize.Y);
            //

            rect = new Rectangle(
                new Point(0),
                Game.screenSize.ToPoint()
                );
            lightmap = new Texture2D(Game.graphics.GraphicsDevice, size.X, size.Y);
            final = new Texture2D(Game.graphics.GraphicsDevice, renderTarget.Bounds.Size.X, renderTarget.Bounds.Size.Y);

            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    lightmapArray[x + (y * size.X)] = Color.White;
                }
            }
        }

        public static void Update()
        {
            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    Vector2 position = Chunk.WorldToTile(Chunk.CorrectedTileToWorld(new Vector2(x, y) * resolution) - Camera.Instance.renderOffset);

                    float highest = 0;
                    float totalIntensity = 0;
                    int amount = 0;
                    //Vector4 final = Vector4.Zero;
                    List<Color> colors = new List<Color>();
                    foreach (LightSource light in LightSource.sources)
                    {
                        float distance = Vector2.Distance(light.position, position);
                        if (distance > light.range)
                        {
                            continue;
                        }

                        amount++;
                        float percent = (1f - (distance / light.range));
                        float intensity = (light.strength * percent);
                        totalIntensity += percent;
                        //final += c.ToVector4();
                        colors.Add(light.color * percent);

                        if (intensity > highest)
                        {
                            highest = intensity;
                        }
                    }
                    //final /= amount;
                    float compensation = 1f / totalIntensity;
                    Color final = Color.Black;
                    foreach (Color color in colors)
                    {
                        /*final.R = (byte)((final.R + (color.R * compensation)) / 2f);
                        final.G = (byte)((final.G + (color.G * compensation)) / 2f);
                        final.B = (byte)((final.B + (color.B * compensation)) / 2f);*/
                        /*final.R = (byte)MathF.Min(final.R + (color.R * compensation), 255);
                        final.G = (byte)MathF.Min(final.G + (color.G * compensation), 255);
                        final.B = (byte)MathF.Min(final.B + (color.B * compensation), 255);*/
                        final.R += (byte)(color.R * compensation);
                        final.G += (byte)(color.G * compensation);
                        final.B += (byte)(color.B * compensation);
                    }
                    final *= (highest / 60f);
                    lightmapArray[x + (y * size.X)] = final;
                }
            }

            lightmap.SetData(lightmapArray); // 30x17 array of colors

            //renderTarget.SetData(lightmapArray);
            /*Color[] c = new Color[renderTarget.Bounds.Size.X * renderTarget.Bounds.Size.Y]; // 1920x1080 array
            renderTarget.GetData(c);
            final.SetData(c);*/
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            //Texture2D blurredTexture = GaussianBlur.Instance.PerformGaussianBlur(lightmap, GaussianRT1, GaussianRT2, spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
            // apply shader effects right here
            //spriteBatch.Draw(blurredTexture, rect, Color.White); // drawing to a 1920x1080 RT for upscaling
            spriteBatch.Draw(lightmap, rect, Color.White);
            spriteBatch.End();
            //GaussianBlur.Instance.RenderWithBlur(lightmap, GaussianRT1, GaussianRT2, spriteBatch);
        }
    }
}
