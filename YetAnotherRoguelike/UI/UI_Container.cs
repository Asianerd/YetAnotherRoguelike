using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI
{
    class UI_Container
    {
        public static List<UI_Container> containers;
        public static UI_Container hoveredContainer;

        public static void Initialize()
        {
            UI_Element.Initialize();
            containers = new List<UI_Container>() {
                new UI_Inventory_Container(),
                new UI_Gameplay_Container(),

                new UI_Furnace()
            };
        }

        public static void UpdateAll()
        {
            hoveredContainer = null;
            UI_Element.hoveredElement = null;

            foreach (UI_Container x in containers)
            {
                x.FetchHovered();
            }

            if (UI_Element.hoveredElement != null)
            {
                UI_Element.hoveredElement.hovered = true;
                hoveredContainer = UI_Element.hoveredElement.parentContainer;
            }

            foreach (UI_Container x in containers)
            {
                x.Update();
            }
        }

        public bool active = false;
        public List<UI_Element> elements;
        public Point drawnOffset;

        public UI_Container(List<UI_Element> e, bool isActive = false)
        {
            active = isActive;

            elements = e;

            drawnOffset = Point.Zero;

            Game.OnScreenResize += OnScreenResize;
        }

        public virtual void FetchHovered()
        {
            if (!active)
            {
                return;
            }
            foreach (UI_Element x in elements)
            {
                x.FetchHovered();
            }
        }

        public virtual void Update()
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

        public virtual void Toggle()
        {
            active = !active;
        }

        public virtual void Draw()
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
