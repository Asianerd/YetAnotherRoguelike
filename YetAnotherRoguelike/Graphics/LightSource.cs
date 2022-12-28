using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Graphics
{
    class LightSource
    {
        public static int lightSourcesCount = 0;
        public static List<LightSource> sources = new List<LightSource>();
        // only use Append and Remove when adding sources

        public Vector2 position;
        public Color color;
        public float strength, range;
        public float oneOverRange; // performance reasons

        public LightSource(Vector2 p, Color c, float s, float r)
        {
            position = p;
            color = c;
            range = r;
            strength = s;

            oneOverRange = 1f / range;
        }

        public static void Append(LightSource light)
        {
            if (sources.Contains(light))
            {
                return;
            }
            sources.Add(light);
            lightSourcesCount = sources.Count;
        }

        public static void Remove(LightSource light)
        {
            if (sources.Contains(light))
            {
                sources.Remove(light);
                lightSourcesCount = sources.Count;
            }
        }
    }
}
