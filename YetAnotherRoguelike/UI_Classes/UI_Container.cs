using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class UI_Container
    {
        public List<UI_Element> elements = new List<UI_Element>();

        public UI_Container(List<UI_Element> _elements)
        {
            elements = _elements;
        }

        public void UpdateAll()
        {
            foreach(UI_Element x in elements)
            {
                if (!x.active)
                {
                    continue;
                }
                x.Update();
            }
        }

        public void DrawAll(SpriteBatch spriteBatch)
        {
            foreach(UI_Element x in elements)
            {
                if (!x.active)
                {
                    continue;
                }
                x.Draw(spriteBatch);
            }
        }
    }
}
