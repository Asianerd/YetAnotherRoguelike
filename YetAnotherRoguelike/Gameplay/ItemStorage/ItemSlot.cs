using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Gameplay.ItemStorage
{
    class ItemSlot : UI_Element
    {
        public static Texture2D slotSprite;
        public static int size = 64;

        public static void Initialize()
        {
            slotSprite = Scene.Content.Load<Texture2D>("UI/Storage_Items/item_slot");
        }

        public Item item;
        public Rectangle rect;

        public ItemSlot(Item _item, Vector2 position)
        {
            item = _item;
            rect = new Rectangle(position.ToPoint(), new Point(size));
        }

        public override void Update()
        {

        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            Rectangle drawnRect = new Rectangle(rect.X + offset.X, rect.Y + offset.Y, rect.Width, rect.Height);

            spriteBatch.Draw(slotSprite, drawnRect, Color.White);
            if (item.type != Item.Type.None)
            {
                spriteBatch.Draw(Item.itemSprites[item.type], drawnRect, Color.White);
                spriteBatch.DrawString(UI.defaultFont, item.amount.ToString(), drawnRect.Location.ToVector2(), Color.White);
            }
        }
    }
}
