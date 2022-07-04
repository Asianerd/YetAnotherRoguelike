using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
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
            };
        }
    }
}
