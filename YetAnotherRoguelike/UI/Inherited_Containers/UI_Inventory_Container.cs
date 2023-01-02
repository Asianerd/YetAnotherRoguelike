using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI
{
    class UI_Inventory_Container : UI_Container
    {
        public static UI_Inventory_Container Instance = null;

        public UI_Background background;

        public UI_Inventory_Container() : base(new List<UI_Element>() { })
        {
            if (Instance == null)
            {
                Instance = this;
            }

            background = new UI_Background(new Rectangle(0, 0, 500, 500));

            elements.Add(background);

            OnScreenResize();
        }

        public override void OnScreenResize()
        {
            base.OnScreenResize();

            background.rect.Location = new Point(background.rect.X, (int)((Game.screenSize.Y / 2f) - (background.rect.Height / 2f)));
        }
    }
}
