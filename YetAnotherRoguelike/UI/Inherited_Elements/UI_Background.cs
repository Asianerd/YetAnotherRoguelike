using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI
{
    class UI_Background : UI_Element
    {
        public UI_Background(UI_Container p, Rectangle r) : base(p, r)
        {

        }

        public override void Draw(SpriteBatch spritebatch, int offsetX = 0, int offsetY = 0)
        {
            base.Draw(spritebatch);

            spritebatch.Draw(backgroundSprite[0], new Rectangle(offsetX + rect.X, offsetY + rect.Y, pixel, pixel), Color.White);
            spritebatch.Draw(backgroundSprite[1], new Rectangle(offsetX + rect.X + pixel, offsetY + rect.Y, rect.Width - (pixel * 2), pixel), Color.White);
            spritebatch.Draw(backgroundSprite[2], new Rectangle(offsetX + rect.Right - pixel, offsetY + rect.Y, pixel, pixel), Color.White);

            spritebatch.Draw(backgroundSprite[3], new Rectangle(offsetX + rect.X, offsetY + rect.Y + pixel, pixel, rect.Height - (pixel * 2)), Color.White);
            spritebatch.Draw(backgroundSprite[4], new Rectangle(offsetX + rect.X + pixel, offsetY + rect.Y + pixel, rect.Width - pixel, rect.Height - (pixel * 2)), Color.White);
            spritebatch.Draw(backgroundSprite[5], new Rectangle(offsetX + rect.Right - pixel, offsetY + rect.Y + pixel, pixel, rect.Height - (pixel * 2)), Color.White);

            spritebatch.Draw(backgroundSprite[6], new Rectangle(offsetX + rect.X, offsetY + rect.Bottom - pixel, pixel, pixel), Color.White);
            spritebatch.Draw(backgroundSprite[7], new Rectangle(offsetX + rect.X + pixel, offsetY + rect.Bottom - pixel, rect.Width - (pixel * 2), pixel), Color.White);
            spritebatch.Draw(backgroundSprite[8], new Rectangle(offsetX + rect.Right - pixel, offsetY + rect.Bottom - pixel, pixel, pixel), Color.White);
        }
    }
}
