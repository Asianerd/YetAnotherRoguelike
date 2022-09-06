using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike.UI_Classes
{
    class Image : UI_Element
    {
        public Texture2D sprite;

        public Image(Texture2D _sprite) : base()
        {
            sprite = _sprite;
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            spriteBatch.Draw(sprite, rect, Color.White);
        }
    }
}
