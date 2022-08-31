using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class UI_Element
    {
        public Vector2 position;
        public bool active = true;

        public virtual void Update()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch, Point offset)
        {

        }
    }
}
