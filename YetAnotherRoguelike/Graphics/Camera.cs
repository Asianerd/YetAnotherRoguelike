using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.PhysicsObject;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.Graphics
{
    public static class Camera
    {
        public static Vector2 position = Vector2.Zero;
        public static Vector2 target = Vector2.Zero;
        public static Vector2 renderOffset = Vector2.Zero;

        static float inverseScreenHeight;

        public static void Update()
        {
            if (Player.Instance != null)
            {
                target = Player.Instance.position * Tile.tileSize;
            }

            inverseScreenHeight = 1 / Game.screenSize.Y;

            position = Vector2.Lerp(position, target, 0.1f * Game.compensation);

            renderOffset = (Game.screenSize * 0.5f) - position;
        }

        public static float GetDrawnLayer(float y, float offset = 0f)
        {
            return Math.Clamp(((y + renderOffset.Y) * inverseScreenHeight) + offset, 0f, 1f);
        }
    }
}
