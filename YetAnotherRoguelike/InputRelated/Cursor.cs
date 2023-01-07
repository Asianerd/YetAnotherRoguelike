using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Graphics;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike
{
    class Cursor
    {
        static Dictionary<CursorStates, Texture2D> cursorSprites = new Dictionary<CursorStates, Texture2D>();

        public static CursorStates state = CursorStates.Default;

        public static Point position;
        public static Point tPosition;
        public static Vector2 tPositionV;
        public static Vector2 tSubPosition;
        // the position in a tile
        // if cursor is in upper half of a tile, then y > 0.5
        // if cursor is in right half of a tile, then x > 0.5
        /*      #(0.5,0)#
         *      #   #   #
         * (0, 0.5) # (1, 0.5)
         *      #   #   #
         *      #(0.5,1)#
         */

        public static LightSource light;

        public static void Initialize()
        {
            foreach (CursorStates x in Enum.GetValues(typeof(CursorStates)).Cast<CursorStates>())
            {
                cursorSprites.Add(x, Game.Instance.Content.Load<Texture2D>($"UI/Cursors/{x.ToString().ToLower()}"));
            }

            light = new LightSource(Vector2.Zero, Color.White, 10, 15);
            //LightSource.Append(light);
        }

        public static void Update()
        {
            state = CursorStates.Default;

            position = Game.mouseState.Position;
            Vector2 tPos = ((position.ToVector2() - Camera.renderOffset) / Tile.tileSize);
            tPositionV = tPos;
            tPos = new Vector2(MathF.Round(tPos.X), MathF.Round(tPos.Y));
            tPosition = tPos.ToPoint();

            light.position = tPositionV;

            tSubPosition.X = ((tPositionV.X + 0.5f) % 1f);
            tSubPosition.Y = ((tPositionV.Y + 0.5f) % 1f);
            if (MathF.Sign(tSubPosition.X) == -1)
            {
                tSubPosition.X += 1;
            }
            if (MathF.Sign(tSubPosition.Y) == -1)
            {
                tSubPosition.Y += 1;
            }
            tSubPosition.X %= 1f;
            tSubPosition.Y %= 1f;
        }

        public static void Draw()
        {
            Game.spriteBatch.Draw(cursorSprites[state], Game.mouseState.Position.ToVector2(), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
        }

        public enum CursorStates
        {
            Default,
            Select,
            Select_pressed,
            Orb,
            Hidden
        }
    }
}
