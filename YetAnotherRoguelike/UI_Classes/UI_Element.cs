using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class UI_Element
    {
        public Rectangle rect;
        public bool active = true;
        public bool hovered = false;

        public virtual void Update()
        {
            hovered = rect.Contains(Game.mouseState.Position);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Point offset)
        {

        }
    }
}
