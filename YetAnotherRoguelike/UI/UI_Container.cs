using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI
{
    class UI_Container
    {
        public static void Initialize()
        {
            UI_Element.Initialize();
            var _ = new UI_Inventory_Container();
        }


        public bool active = false;
        public List<UI_Element> elements;

        public UI_Container(List<UI_Element> e, bool isActive = false)
        {
            active = isActive;

            elements = e;

            Game.OnScreenResize += OnScreenResize;
        }

        public void Update()
        {
            if (!active)
            {
                return;
            }
            foreach (UI_Element x in elements)
            {
                x.Update();
            }
        }

        public void Toggle()
        {
            active = !active;
        }

        public void Draw()
        {
            if (!active)
            {
                return;
            }
            foreach (UI_Element x in elements)
            {
                x.Draw(Game.spriteBatch);
            }
        }

        public virtual void OnScreenResize()
        {

        }
    }
}
