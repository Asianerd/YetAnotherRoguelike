using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI
{
    class UI_Background : UI_Element
    {
        public UI_Background(Rectangle r) : base(r)
        {

        }

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);

            spritebatch.Draw(backgroundSprite[0], new Rectangle(rect.X, rect.Y, pixel, pixel), Color.White);
            spritebatch.Draw(backgroundSprite[1], new Rectangle(rect.X + pixel, rect.Y, rect.Width - (pixel * 2), pixel), Color.White);
            spritebatch.Draw(backgroundSprite[2], new Rectangle(rect.Right - pixel, rect.Y, pixel, pixel), Color.White);

            spritebatch.Draw(backgroundSprite[3], new Rectangle(rect.X, rect.Y + pixel, pixel, rect.Height - (pixel * 2)), Color.White);
            spritebatch.Draw(backgroundSprite[4], new Rectangle(rect.X + pixel, rect.Y + pixel, rect.Width - pixel, rect.Height - (pixel * 2)), Color.White);
            spritebatch.Draw(backgroundSprite[5], new Rectangle(rect.Right - pixel, rect.Y + pixel, pixel, rect.Height - (pixel * 2)), Color.White);

            spritebatch.Draw(backgroundSprite[6], new Rectangle(rect.X, rect.Bottom - pixel, pixel, pixel), Color.White);
            spritebatch.Draw(backgroundSprite[7], new Rectangle(rect.X + pixel, rect.Bottom - pixel, rect.Width - (pixel * 2), pixel), Color.White);
            spritebatch.Draw(backgroundSprite[8], new Rectangle(rect.Right - pixel, rect.Bottom - pixel, pixel, pixel), Color.White);
        }
    }
}
