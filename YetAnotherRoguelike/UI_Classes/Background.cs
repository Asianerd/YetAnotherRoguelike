using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike.UI_Classes
{
    class Background : UI_Element
    {
        public static List<Texture2D> defaultSprite;

        public static void Initialize()
        {
            /*defaultSprite = new List<Texture2D>();
            for (int i = 1; i <= 9; i++)
            {
                defaultSprite.Add(Game.Instance.Content.Load<Texture2D>($"UI/Buttons/default{i}"));
            }*/

            defaultSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/background2"), 4, 4);
        }

        public Background(Rectangle _rect) : base()
        {
            rect = _rect;
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            int pixel = 16;

            Rectangle renderedRect = new Rectangle(rect.X + offset.X, rect.Y + offset.Y, rect.Width, rect.Height);
            spriteBatch.Draw(defaultSprite[0], new Rectangle(renderedRect.X, renderedRect.Y, pixel, pixel), Color.White);
            spriteBatch.Draw(defaultSprite[1], new Rectangle(renderedRect.X + pixel, renderedRect.Y, renderedRect.Width - (pixel * 2), pixel), Color.White);
            spriteBatch.Draw(defaultSprite[2], new Rectangle(renderedRect.Right - pixel, renderedRect.Y, pixel, pixel), Color.White);

            spriteBatch.Draw(defaultSprite[3], new Rectangle(renderedRect.X, renderedRect.Y + pixel, pixel, renderedRect.Height - (pixel * 2)), Color.White);
            spriteBatch.Draw(defaultSprite[4], new Rectangle(renderedRect.X + pixel, renderedRect.Y + pixel, renderedRect.Width - pixel, renderedRect.Height - (pixel * 2)), Color.White);
            spriteBatch.Draw(defaultSprite[5], new Rectangle(renderedRect.Right - pixel, renderedRect.Y + pixel, pixel, renderedRect.Height - (pixel * 2)), Color.White);

            spriteBatch.Draw(defaultSprite[6], new Rectangle(renderedRect.X, renderedRect.Bottom - pixel, pixel, pixel), Color.White);
            spriteBatch.Draw(defaultSprite[7], new Rectangle(renderedRect.X + pixel, renderedRect.Bottom - pixel, renderedRect.Width - (pixel * 2), pixel), Color.White);
            spriteBatch.Draw(defaultSprite[8], new Rectangle(renderedRect.Right - pixel, renderedRect.Bottom - pixel, pixel, pixel), Color.White);
        }
    }
}
