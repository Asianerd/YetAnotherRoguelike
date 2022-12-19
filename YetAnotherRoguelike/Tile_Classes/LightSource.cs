using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class LightSource
    {
        public static List<LightSource> sources = new List<LightSource>();
        public delegate void LightEvent();
        public static event LightEvent OnLightUpdate;
        // wanted to update lights only when a new light source is added, but that wont work with dynamic lights
        // TODO : maybe update lights in a seperate thread?

        public Vector2 position; // Light position, in tile-coordinates
        public float strength, range;
        public Color color;

        public LightSource(Vector2 _position, float _strength, float _range, Color _color)
        {
            position = _position;
            strength = _strength;
            range = _range;
            color = _color;
        }

        public static void Append(LightSource light)
        {
            if (sources.Contains(light))
            {
                return;
            }
            sources.Add(light);
        }

        public static void Remove(LightSource light)
        {
            if (sources.Contains(light))
            {
                sources.Remove(light);
            }
        }
    }
}
