using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.Graphics
{
    class Lightmap
    {
        /*public static RenderTarget2D renderTarget;
        public static Texture2D lightmap;
        public static Texture2D lightTexture; // the finished texture, same size as the screen
        public static Texture2D radialSprite;
        public static Color[] lightmapArray;

        public static Point size; // size of array
        public static double resolution = 0.5d;

        public static Rectangle drawnRect;*/

        public static RenderTarget2D renderTarget;
        public static Texture2D lightTexture; // the finished texture, same size as the screen
        public static Texture2D radialSprite;
        public static Vector2 spriteOrigin;

        public static Rectangle drawnRect;
        public static float scaleCoefficient;

        public static void Initialize()
        {
            radialSprite = Game.Instance.Content.Load<Texture2D>("Light/radial");
            spriteOrigin = radialSprite.Bounds.Size.ToVector2() / 2f;
            scaleCoefficient = Tile.tileSize / spriteOrigin.X;

            OnScreenSizeChange();
        }

        public static void OnScreenSizeChange()
        {
            /*renderTarget = new RenderTarget2D(
                Game.graphics.GraphicsDevice,
                (int)Game.screenSize.X,
                (int)Game.screenSize.Y
                );

            size = new Point(
                (int)Math.Ceiling(Game.screenSize.X / (Tile.tileSize * resolution)),
                (int)Math.Ceiling(Game.screenSize.Y / (Tile.tileSize * resolution))
                );
            lightmapArray = new Color[size.X * size.Y];

            lightmap = new Texture2D(Game.graphics.GraphicsDevice, size.X, size.Y);
            lightTexture = new Texture2D(Game.graphics.GraphicsDevice, renderTarget.Bounds.Size.X, renderTarget.Bounds.Size.Y);

            drawnRect = new Rectangle(new Point(0), Game.screenSize.ToPoint());*/

            renderTarget = new RenderTarget2D(
                Game.graphics.GraphicsDevice,
                (int)Game.screenSize.X,
                (int)Game.screenSize.Y
                );
            drawnRect = new Rectangle(new Point(0), Game.screenSize.ToPoint());
        }

        public static void GenerateMap()
        {
            Game.graphics.GraphicsDevice.SetRenderTarget(renderTarget);

            Game.spriteBatch.Begin(blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Immediate);
            foreach (LightSource s in LightSource.sources)
            {
                Game.spriteBatch.Draw(radialSprite, (s.position * Tile.tileSize) + Camera.renderOffset, null, s.color, 0f, spriteOrigin, s.range * scaleCoefficient, SpriteEffects.None, 0f);
            }
            Game.spriteBatch.End();

            Game.graphics.GraphicsDevice.SetRenderTarget(null);

            lightTexture = (Texture2D)renderTarget;
        }

        /*public static void __GenerateMap() // Depreciated
        {
            float posCoefficientX = Tile.tileSize * size.X;
            float posCoefficientY = Tile.tileSize * size.Y;
            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    Vector2 position = new Vector2(
                        x * posCoefficientX,
                        y * posCoefficientY
                        );

                    float highest = 0;
                    float totalIntensity = 0;
                    int amount = 0;
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
                        *//*final.R = (byte)((final.R + (color.R * compensation)) / 2f);
                        final.G = (byte)((final.G + (color.G * compensation)) / 2f);
                        final.B = (byte)((final.B + (color.B * compensation)) / 2f);*/
                        /*final.R = (byte)MathF.Min(final.R + (color.R * compensation), 255);
                        final.G = (byte)MathF.Min(final.G + (color.G * compensation), 255);
                        final.B = (byte)MathF.Min(final.B + (color.B * compensation), 255);*//*
                        final.R += (byte)(color.R * compensation);
                        final.G += (byte)(color.G * compensation);
                        final.B += (byte)(color.B * compensation);
                    }
                    final *= (highest / 60f); // dampening so that its not too bright
                    lightmapArray[x + (y * size.X)] = final;
                }
            }

            lightmap.SetData(lightmapArray);
        }*/

        public static void Draw(SpriteBatch spritebatch)
        {
            //spritebatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
            spritebatch.Draw(lightTexture, drawnRect, Color.White);
            //spritebatch.End();
        }
    }
}
