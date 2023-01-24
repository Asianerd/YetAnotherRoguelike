using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI
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
                Cursor.state = Cursor.CursorStates.Select;
                if (MouseInput.left.active)
                {
                    UI_Inventory_Container.Instance.ChangePage(page);
                }
            }
        }

        public override void Draw(SpriteBatch spritebatch, int offsetX = 0, int offsetY = 0)
        {
            base.Draw(spritebatch, offsetX, offsetY);

            Rectangle renderedRect = new Rectangle(
                rect.X + offsetX,
                rect.Y + offsetY,
                rect.Width,
                rect.Height
                );

            float layer = UI_Inventory_Container.Instance.page == page ? 0.95f : 0.05f;
            Color color = Color.White * (UI_Inventory_Container.Instance.page == page ? 1 : 0.5f);
            color.A = 255;

            spritebatch.Draw(tabSprite[0], new Rectangle(renderedRect.X, renderedRect.Y, pixel, pixel), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spritebatch.Draw(tabSprite[1], new Rectangle(renderedRect.X + pixel, renderedRect.Y, renderedRect.Width - (pixel * 2), pixel), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spritebatch.Draw(tabSprite[2], new Rectangle(renderedRect.Right - pixel, renderedRect.Y, pixel, pixel), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);

            spritebatch.Draw(tabSprite[3], new Rectangle(renderedRect.X, renderedRect.Y + pixel, pixel, renderedRect.Height - (pixel * 2)), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spritebatch.Draw(tabSprite[4], new Rectangle(renderedRect.X + pixel, renderedRect.Y + pixel, renderedRect.Width - (pixel * 2), renderedRect.Height - (pixel * 2)), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spritebatch.Draw(tabSprite[5], new Rectangle(renderedRect.Right - pixel, renderedRect.Y + pixel, pixel, renderedRect.Height - (pixel * 2)), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);

            spritebatch.Draw(tabSprite[6], new Rectangle(renderedRect.X, renderedRect.Bottom - pixel, pixel, pixel), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spritebatch.Draw(tabSprite[7], new Rectangle(renderedRect.X + pixel, renderedRect.Bottom - pixel, renderedRect.Width - (pixel * 2), pixel), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spritebatch.Draw(tabSprite[8], new Rectangle(renderedRect.Right - pixel, renderedRect.Bottom - pixel, pixel, pixel), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }
    }
}
