using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike.UI
{
    class UI_ItemSlot : UI_Element
    {
        public static Texture2D slotSprite;
        public static Texture2D selectionSprite;
        public static int size = 64;

        public Item item;

        public UI_ItemSlot(UI_Container p, Rectangle r, Item i) : base(p, r, ElementType.ItemSlot)
        {
            rect = r;
            item = i;
        }

        public override void Update()
        {
            base.Update();

            if (!hovered)
            {
                return;
            }
            if (item.amount != 0)
            {
                Cursor.state = Cursor.CursorStates.Grab;
            }
            if (MouseInput.left.active)
            {
                if ((Cursor.item.type == item.type) && (Cursor.item.data == item.data))
                {
                    int toAdd, toRemove, canAdd = item.stackSize - item.amount;
                    if (Cursor.item.amount > canAdd)
                    {
                        toAdd = canAdd;
                        toRemove = toAdd;
                    }
                    else
                    {
                        toAdd = Cursor.item.amount;
                        toRemove = Cursor.item.amount;
                    }
                    item.amount += toAdd;
                    Cursor.item.amount -= toRemove;
                    if (Cursor.item.amount <= 0)
                    {
                        Cursor.item = new Item(Item.Type.None, 0);
                    }
                }
                else
                {
                    Item temp = new Item(Cursor.item.type, Cursor.item.amount, Cursor.item.data);

                    /*if (Cursor.item.type == Item.Type.Crucible)
                        Debug.WriteLine(Chemical.collection[Cursor.item.data[Item.DataType.Chemical]].Total());*/

                    Cursor.item.type = item.type;
                    Cursor.item.amount = item.amount;
                    Cursor.item.data = item.data;

                    item.type = temp.type;
                    item.amount = temp.amount;
                    item.data = temp.data;
                }
            }
            else if (MouseInput.right.active)
            {
                if (Cursor.item.type == Item.Type.None)
                {
                    int removed = item.amount / 2;

                    int added = item.amount - removed;
                    Cursor.item = new Item(item.type, added, item.data);
                    item.amount -= added;
                    if (item.amount <= 0)
                    {
                        item.type = Item.Type.None;
                        item.amount = 0;
                        item.data = null;
                    }
                }
                else
                {
                    Item temp = new Item(Cursor.item.type, Cursor.item.amount, Cursor.item.data);

                    Cursor.item.type = item.type;
                    Cursor.item.amount = item.amount;
                    Cursor.item.data = item.data;

                    item.type = temp.type;
                    item.amount = temp.amount;
                    item.data = temp.data;
                }
            }
        }

        public override void Draw(SpriteBatch spritebatch, int offsetX = 0, int offsetY = 0)
        {
            Rectangle drawnRect = new Rectangle(
                rect.X + offsetX,
                rect.Y + offsetY,
                rect.Width,
                rect.Height
                );

            spritebatch.Draw(slotSprite, drawnRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            if (item.type == Item.Type.None)
            {
                return;
            }
            spritebatch.Draw(item.FetchSprite(), new Rectangle(
                drawnRect.X + offsetX,
                drawnRect.Y + offsetY,
                drawnRect.Width,
                drawnRect.Height
                ), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.205f);
            if (item.amount != 1)
            {
                spritebatch.DrawString(mainFont, item.amount.ToString(), drawnRect.Location.ToVector2(), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.21f);
            }
        }
    }
}
