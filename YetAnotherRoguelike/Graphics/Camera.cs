using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Graphics
{
    public static class Camera
    {
        public static Vector2 position = Vector2.Zero;
        public static Vector2 target = Vector2.Zero;
        public static Vector2 renderOffset = Vector2.Zero;

        public static void Update()
        {
            Vector2 final = Vector2.Zero;
            //foreach (Keys x in new Keys[] { Keys.Up, Keys.Left, Keys.Down, Keys.Right })
            foreach (Keys x in new Keys[] { Keys.W, Keys.A, Keys.S, Keys.D })
            {
                if (Input.collection[x].isPressed)
                {
                    final += GeneralDependencies.axialVectors[x];
                }
            }
            if ((final.X != 0) || (final.Y != 0))
            {
                final.Normalize();
            }
            final *= Input.collection[Keys.LeftShift].isPressed ? 50f : 20f;
            target += final;

            position = Vector2.Lerp(position, target, 0.1f * Game.compensation);

            renderOffset = (Game.screenSize / 2f) - position;
        }
    }
}
