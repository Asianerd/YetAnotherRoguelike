using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YetAnotherRoguelike
{
    class Camera
    {
        public static Camera Instance;

        public Vector2 position = Game.screenSize / 2f, target, renderOffset;

        public Camera()
        {
            Instance = this;
        }

        public void Update()
        {
            /*Vector2 final = Vector2.Zero;
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
            final *= 10f;
            target += final;*/
            if (Player.Instance != null)
            {
                target = Player.Instance.position;
            }

            position = Vector2.Lerp(position, target, 0.1f);

            renderOffset = (Game.screenSize / 2f) - position;
        }
    }
}
