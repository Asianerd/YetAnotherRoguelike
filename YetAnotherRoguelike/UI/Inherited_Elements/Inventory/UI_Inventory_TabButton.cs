using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI.Inherited_Elements
{
    class UI_Inventory_TabButton : UI_Element
    {
        public static List<Texture2D> tabSprite;

        public static new void Initialize()
        {
            tabSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Inventory/TabButton"), 4, 4);
        }

        UI_Inventory_Container.InventoryPage page;

        public UI_Inventory_TabButton(UI_Container p, Rectangle r, UI_Inventory_Container.InventoryPage _p):base(p, r)
        {
            page = _p;
        }

        public override void Update()
        {
            base.Update();
            if (hovered)
            {
                if (MouseInput.left.active)
                {

                }
            }
        }

        public override void Draw(SpriteBatch spritebatch, int offsetX = 0, int offsetY = 0)
        {
            base.Draw(spritebatch, offsetX, offsetY);

            spritebatch.Draw(backgroundSprite[0], new Rectangle(offsetX + rect.X, offsetY + rect.Y, pixel, pixel), Color.White);
            spritebatch.Draw(backgroundSprite[1], new Rectangle(offsetX + rect.X + pixel, offsetY + rect.Y, rect.Width - (pixel * 2), pixel), Color.White);
            spritebatch.Draw(backgroundSprite[2], new Rectangle(offsetX + rect.Right - pixel, offsetY + rect.Y, pixel, pixel), Color.White);

            spritebatch.Draw(backgroundSprite[3], new Rectangle(offsetX + rect.X, offsetY + rect.Y + pixel, pixel, rect.Height - (pixel * 2)), Color.White);
            spritebatch.Draw(backgroundSprite[4], new Rectangle(offsetX + rect.X + pixel, offsetY + rect.Y + pixel, rect.Width - pixel, rect.Height - (pixel * 2)), Color.White);
            spritebatch.Draw(backgroundSprite[5], new Rectangle(offsetX + rect.Right - pixel, offsetY + rect.Y + pixel, pixel, rect.Height - (pixel * 2)), Color.White);

            spritebatch.Draw(backgroundSprite[6], new Rectangle(offsetX + rect.X, offsetY + rect.Bottom - pixel, pixel, pixel), Color.White);
            spritebatch.Draw(backgroundSprite[7], new Rectangle(offsetX + rect.X + pixel, offsetY + rect.Bottom - pixel, rect.Width - (pixel * 2), pixel), Color.White);
            spritebatch.Draw(backgroundSprite[8], new Rectangle(offsetX + rect.Right - pixel, offsetY + rect.Bottom - pixel, pixel, pixel), Color.White);
        }
    }
}
