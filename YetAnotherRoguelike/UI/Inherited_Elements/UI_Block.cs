using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI
{
    class UI_Block : UI_Element
    {
        public UI_Block(UI_Container p, Rectangle r) : base(p, r)
        {

        }

        public override void Draw(SpriteBatch spritebatch, int offsetX = 0, int offsetY = 0)
        {
            base.Draw(spritebatch);

            spritebatch.Draw(blank, rect, Color.White);
        }
    }
}
