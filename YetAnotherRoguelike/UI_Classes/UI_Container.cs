using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class UI_Container
    {
        public static List<UI_Container> containers = new List<UI_Container>();
        public static bool hoverContainer; // if any container is hovered

        public static void Update()
        {
            hoverContainer = containers.Where(n => n.active && n.hovered).Count() > 0;
        }

        public static void Unload()
        {
            containers = new List<UI_Container>();
        }

        public bool active, hovered = false;
        public List<UI_Element> elements = new List<UI_Element>();

        public UI_Container(List<UI_Element> _elements)
        {
            elements = _elements;

            containers.Add(this);
        }

        public virtual void Toggle()
        {
            active = !active;
        }

        public virtual void UpdateAll()
        {
            hovered = false;
            foreach(UI_Element x in elements)
            {
                if (!x.active)
                {
                    continue;
                }
                x.Update();
                if (x.hovered)
                {
                    hovered = true;
                }
            }
        }

        public virtual void DrawAll(SpriteBatch spriteBatch, Point offset)
        {
            foreach(UI_Element x in elements)
            {
                if (!x.active)
                {
                    continue;
                }
                x.Draw(spriteBatch, offset);
            }
        }
    }
}
