using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike
{
    class Cursor
    {
        public static CursorStates state = CursorStates.Default;
        public static Dictionary<CursorStates, Texture2D> cursorSprites = new Dictionary<CursorStates, Texture2D>();

        public static void Initialize()
        {
            foreach (CursorStates x in Enum.GetValues(typeof(CursorStates)).Cast<CursorStates>())
            {
                cursorSprites.Add(x, Game.Instance.Content.Load<Texture2D>($"UI/Cursors/{x.ToString().ToLower()}"));
            }
        }

        public static void Update()
        {
            state = CursorStates.Default;
        }

        public static void Draw()
        {
            Game.spriteBatch.Draw(cursorSprites[(state == CursorStates.Select) && (Game.mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) ? CursorStates.Select_pressed : state], Game.mouseState.Position.ToVector2(), null, Color.White, 0f, Vector2.Zero, 5f, SpriteEffects.None, 0f);
        }

        public enum CursorStates
        {
            Default,
            Select,
            Select_pressed
        }
    }
}
