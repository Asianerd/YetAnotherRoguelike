using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Gameplay.ItemStorage
{
    class ItemSlot : UI_Element
    {
        public static ItemSlot hoveredSlot;
        public static Texture2D slotSprite;
        public static Texture2D selectionSprite;
        public static Vector2 selectionSpriteOrigin;
        public static int size = 64;

        public static void Initialize()
        {
            slotSprite = Scene.Content.Load<Texture2D>("UI/Storage_Items/item_slot");
            selectionSprite = Scene.Content.Load<Texture2D>("UI/Storage_Items/selection_sprite");
            selectionSpriteOrigin = selectionSprite.Bounds.Size.ToVector2() / 2f;
        }

        public Item item;

        public ItemSlot(Item _item, Vector2 position)
        {
            item = _item;
            rect = new Rectangle(position.ToPoint(), new Point(size));
        }

        public override void Update()
        {
            base.Update();

            if (hovered)
            {
                if (MouseInput.left.active)
                {
                    if (Cursor.item.type == item.type)
                    {
                        int toAdd, toRemove, canAdd = Item.stackSize - item.amount;
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
                        Item t = new Item(item.type, item.amount);
                        item.type = Cursor.item.type;
                        item.amount = Cursor.item.amount;
                        Cursor.item.type = t.type;
                        Cursor.item.amount = t.amount;
                    }
                }
                else if (MouseInput.right.isPressed)
                {
                    if (MouseInput.right.active)
                    {
                        if (Cursor.item.type == Item.Type.None)
                        {
                            int removed = item.amount / 2;

                            int added = item.amount - removed;
                            Debug.WriteLine($"{item.amount} : {item.amount / 2} : {added}");
                            Cursor.item = new Item(item.type, added);
                            item.amount -= added;
                            if (item.amount <= 0)
                            {
                                item.type = Item.Type.None;
                                item.amount = 0;
                            }
                        }
                    }
/*                    else
                    {
                        if (Cursor.item.type != Item.Type.None)
                        {
                            if (item.type == Cursor.item.type)
                            {
                                item.amount++;
                                Cursor.item.amount--;
                                if (Cursor.item.amount <= 0)
                                {
                                    Cursor.item = new Item(Item.Type.None, 0);
                                }
                            }
                            else if (item.type == Item.Type.None)
                            {
                                int toAdd = 0, toRemove = 0, canAdd = Item.stackSize - item.amount;
                                if (1 <= canAdd)
                                {
                                    toAdd = 1;
                                    toRemove = 1;
                                }
                                item.type = Cursor.item.type;
                                item.amount += toAdd;
                                Cursor.item.amount -= toRemove;
                                if (Cursor.item.amount <= 0)
                                {
                                    Cursor.item = new Item(Item.Type.None, 0);
                                }
                            }
                        }
                    }*/
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            Rectangle drawnRect = new Rectangle(rect.X + offset.X, rect.Y + offset.Y, rect.Width, rect.Height);

            spriteBatch.Draw(slotSprite, drawnRect, Color.White);
            if (item.type != Item.Type.None)
            {
                spriteBatch.Draw(Item.itemSprites[item.type], new Rectangle(
                    drawnRect.Location.X - (hovered ? 5 : 0),
                    drawnRect.Location.Y - (hovered ? 5 : 0),
                    drawnRect.Size.X + (hovered ? 10 : 0),
                    drawnRect.Size.Y + (hovered ? 10 : 0)
                    ), Color.White);
                spriteBatch.DrawString(UI.defaultFont, item.amount.ToString(), drawnRect.Location.ToVector2(), Color.White);
            }
        }
    }
}
