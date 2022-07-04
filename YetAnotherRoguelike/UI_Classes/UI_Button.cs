using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class UI_Button : UI_Element
    {
        public static List<Texture2D> defaultSprite;

        public static void Initialize()
        {
            defaultSprite = new List<Texture2D>();
            for (int i = 1; i <= 9; i++)
            {
                defaultSprite.Add(Game.Instance.Content.Load<Texture2D>($"UI/Buttons/default{i}"));
            }
        }

        string title;
        Rectangle rect;
        Rectangle targetRect;
        Action action;

        GameValue hoverAge = new GameValue(0, 10, 1);

        bool hovered = false;
        public bool isPressed, wasPressed, currentPressed;

        public UI_Button(Rectangle _rect, Action _action, string _title = "") : base()
        {
            title = _title;
            rect = new Rectangle(
                (int)(_rect.X - (_rect.Width / 2f)),
                (int)(_rect.Y - (_rect.Height / 2f)),
                _rect.Width,
                _rect.Height
                );
            action = _action;

            targetRect = new Rectangle(new Point(rect.X, rect.Y - 10), rect.Size);
        }

        public override void Update()
        {
            hovered = rect.Contains(Game.mouseState.Position);
            if (hovered)
            {
                Cursor.state = Cursor.CursorStates.Select;
            }

            wasPressed = isPressed;
            isPressed = hovered && (Game.mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);
            currentPressed = isPressed && !wasPressed;

            if (currentPressed)
            {
                action();
            }

            hoverAge.Regenerate(hovered ? 1 : -1);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int pixel = 32;

            Rectangle renderedRect = new Rectangle(Vector2.Lerp(rect.Location.ToVector2(), targetRect.Location.ToVector2(), (float)Math.Sin(hoverAge.Percent() * Math.PI / 2)).ToPoint(), rect.Size);
            spriteBatch.Draw(defaultSprite[0], new Rectangle(renderedRect.X, renderedRect.Y, pixel, pixel), Color.White);
            spriteBatch.Draw(defaultSprite[1], new Rectangle(renderedRect.X + pixel, renderedRect.Y, renderedRect.Width - (pixel * 2), pixel), Color.White);
            spriteBatch.Draw(defaultSprite[2], new Rectangle(renderedRect.Right - pixel, renderedRect.Y, pixel, pixel), Color.White);

            spriteBatch.Draw(defaultSprite[3], new Rectangle(renderedRect.X, renderedRect.Y + pixel, pixel, renderedRect.Height - (pixel * 2)), Color.White);
            spriteBatch.Draw(defaultSprite[4], new Rectangle(renderedRect.X + pixel, renderedRect.Y + pixel, renderedRect.Width - pixel, renderedRect.Height - (pixel * 2)), Color.White);
            spriteBatch.Draw(defaultSprite[5], new Rectangle(renderedRect.Right - pixel, renderedRect.Y + pixel, pixel, renderedRect.Height - (pixel * 2)), Color.White);

            spriteBatch.Draw(defaultSprite[6], new Rectangle(renderedRect.X, renderedRect.Bottom - pixel, pixel, pixel), Color.White);
            spriteBatch.Draw(defaultSprite[7], new Rectangle(renderedRect.X + pixel, renderedRect.Bottom - pixel, renderedRect.Width - (pixel * 2), pixel), Color.White);
            spriteBatch.Draw(defaultSprite[8], new Rectangle(renderedRect.Right - pixel, renderedRect.Bottom - pixel, pixel, pixel), Color.White);

            spriteBatch.DrawString(UI.defaultFont, title, renderedRect.Center.ToVector2(), Color.White, 0f, UI.defaultFont.MeasureString(title) / 2f, 1.5f, SpriteEffects.None, 0f);
        }
    }
}