using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Graphics;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.Particles
{
    class BreakBlock : Particle
    {
        public static float fallDistance = 0.6f;

        Color color;
        Vector2 origin;
        Vector2 increment;
        float start, i = 1;
        float rotation, rotIncrement;
        float scale;
        // Vector2 scale;
        // i  :  1 ~> 0

        bool isEmissive;
        LightSource lightSource;
        float lightStrength, lightRange;

        public BreakBlock(Vector2 pos, Color _color, Vector2 _origin, LightSource l = null, bool defaultColor = true) : base(pos, new GameValue(0, 50, 1, 0))
        {
            // defaultColor is whether the color given is not in block_data.json
            origin = _origin;

            age.I += Game.random.Next(5, 15);

            //scale = new Vector2(8f * (Game.random.Next(90, 140) / 100f), 8f * (Game.random.Next(90, 140) / 100f));
            scale = 8f * (Game.random.Next(90, 140) / 100f);

            start = pos.Y + (Game.random.Next(-100, 100) / 100f);
            increment = (pos - origin) * 0.05f;
            increment.Y = 0;

            rotation = (Game.random.Next(0, 1000) / 1000f) * MathF.PI * 2f;
            rotIncrement = (Game.random.Next(0, 1000) / 10000f) * MathF.PI * 2f;
            if (origin.X > pos.X)
            {
                rotIncrement *= -1; // flip direction
            }

            float highest = 0;
            List<Color> colors = new List<Color>();
            List<float> intensities = new List<float>();
            if (defaultColor) // color is not in block_data.json
            {
                colors.Add(_color);
                intensities.Add(1f);
            }
            foreach (LightSource light in LightSource.sources)
            {
                float distance = Vector2.Distance(light.position, position / Tile.tileSize);
                if (distance > light.range)
                {
                    continue;
                }

                float percent = (1f - (distance / light.range));
                float intensity = (light.strength * percent);
                colors.Add(light.color * percent);
                intensities.Add(percent);

                if (intensity >= highest)
                {
                    highest = intensity;
                }
            }

            float compensation = 1f / intensities.Sum();
            Color final = Color.Black;
            foreach (Color c in colors)
            {
                final.R += (byte)(c.R * compensation);
                final.G += (byte)(c.G * compensation);
                final.B += (byte)(c.B * compensation);
            }
            if (!defaultColor) // color is in block_data.json
            {
                final.R = (byte)((final.R * 0.3f) + (_color.R * 0.7f));
                final.G = (byte)((final.G * 0.3f) + (_color.G * 0.7f));
                final.B = (byte)((final.B * 0.3f) + (_color.B * 0.7f));
            }
            color = final * (highest / 20f);
            color.A = 255;

            if (l != null)
            {
                isEmissive = true;

                lightStrength = l.strength;
                lightRange = l.range * 0.1f;

                lightSource = new LightSource(pos / Tile.tileSize, l.color, lightStrength, lightRange);
                LightSource.Append(lightSource);
            }
        }

        public override void Update()
        {
            base.Update();

            //i = (1f - age.Percent());
            i = age.Percent();

            //position.Y = start - ((Math.Abs(MathF.Sin(i * 2f * MathF.PI)) * 20f) / (i >= 0.5 ? 1f : 1f));
            position.Y = (start - (((-16 * MathF.Pow((i < fallDistance ? i : fallDistance) - 0.25f, 2)) + 1) * 20f));

            if (i <= fallDistance)
            {
                position += increment * (1f - i) * Game.compensation;
                rotation += rotIncrement * Game.compensation * i;
            }

            if (!isEmissive)
            {
                return;
            }

            lightSource.position = position / Tile.tileSize;
            lightSource.strength = lightStrength * (1f - i);
            lightSource.range = lightRange * (1f - i);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //spriteBatch.Draw(blank, position, null, color * Math.Clamp((1f - i) * 5f, 0f, 1f), 0f, Vector2.Zero, 8f, SpriteEffects.None, 0f);
            spriteBatch.Draw(blank, position, null, color * Math.Clamp((1f - i) * 5f, 0f, 1f), rotation, blankOrigin, scale, SpriteEffects.None, 0f);
        }

        public override void OnDeath()
        {
            if (isEmissive)
            {
                LightSource.Remove(lightSource);
            }
        }
    }
}
