using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Particles
{
    class BreakBlock : Particle
    {
        public static float fallDistance = 0.6f;

        Color color;
        Vector2 origin;
        Vector2 increment;
        float start, i = 1;
        float scale;
        // i  :  1 ~> 0

        public BreakBlock(Vector2 pos, Color _color, Vector2 _origin) : base(Type.Block_break, pos, new GameValue(0, 50, 1, 0))
        {
            origin = _origin;

            age.I += Game.random.Next(5, 15);

            scale = 8f * (Game.random.Next(90, 140) / 100f);

            start = pos.Y + (Game.random.Next(-100, 100) / 100f);
            increment = (pos - origin) * 0.05f;
            increment.Y = 0;


            float highest = 0;
            List<Color> colors = new List<Color>() { _color * 1f } ;
            List<float> intensities = new List<float>() { 1f };
            foreach (LightSource light in LightSource.sources)
            {
                float distance = Vector2.Distance(light.position, Chunk.CorrectedWorldToTile(position));
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
            color = final * (highest / 80f);
            color.A = 255;
        }

        public override void Update()
        {
            base.Update();

            //i = (1f - age.Percent());
            i = age.Percent();

            //position.Y = start - ((Math.Abs(MathF.Sin(i * 2f * MathF.PI)) * 20f) / (i >= 0.5 ? 1f : 1f));
            position.Y = start - (((-16 * MathF.Pow((i < fallDistance ? i : fallDistance) - 0.25f, 2)) + 1) * 20f);

            if (i <= fallDistance)
            {
                position += increment * (1f - i);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //spriteBatch.Draw(blank, position, null, color * Math.Clamp((1f - i) * 5f, 0f, 1f), 0f, Vector2.Zero, 8f, SpriteEffects.None, 0f);
            spriteBatch.Draw(blank, position, null, color * Math.Clamp((1f - i) * 5f, 0f, 1f), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
