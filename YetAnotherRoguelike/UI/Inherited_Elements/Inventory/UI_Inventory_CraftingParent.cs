using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Data;
using YetAnotherRoguelike.PhysicsObject;

namespace YetAnotherRoguelike.UI
{
    class UI_Inventory_CraftingParent : UI_Element
    {
        public static UI_Inventory_CraftingParent Instance = null;
        public static new List<Texture2D> backgroundSprite;
        public static List<Texture2D> backgroundBorderSprite;
        public static List<Texture2D> topPanel, bottomPanel;

        public static new void Initialize()
        {
            backgroundSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingParentBackground"), 4, 4);
            backgroundBorderSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingParentBackgroundBorder"), 4, 4);
            topPanel = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingParentTopPanel"), 4, 4);
            bottomPanel = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingParentBottomPanel"), 4, 4);

            UI_Inventory_CraftingChild.Initialize();
        }

        public List<UI_Inventory_CraftingChild> collection;
        public float scroll = 0, scrollTarget = 0;
        float maxScroll;
        public Rectangle topPanelRect, bottomPanelRect;

        public UI_Inventory_CraftingParent(UI_Container p, Rectangle r) : base(p, r)
        {
            if (Instance == null)
            {
                Instance = this;
            }

            collection = new List<UI_Inventory_CraftingChild>();
            topPanelRect = new Rectangle(0, 0, 0, 0);
            bottomPanelRect = new Rectangle(0, 0, 0, 0);
        }

        public override void Update()
        {
            base.Update();
            if (hovered)
            {
                scrollTarget += MouseInput.scrollVel;
            }

            scroll = GeneralDependencies.Lerp(scroll, scrollTarget, 0.05f * Game.compensation, 1f);

            if (collection.Count <= 5)
            {
                scrollTarget = 0;
            }
            else
            {
                if (scroll >= 0)
                {
                    scrollTarget = 0;
                }

                if (scroll <= maxScroll)
                {
                    scrollTarget = maxScroll;
                }
            }

            foreach (UI_Inventory_CraftingChild x in collection)
            {
                x.FetchHovered();
            }
            foreach (UI_Inventory_CraftingChild x in collection)
            {
                if (hoveredElement == x)
                {
                    x.hovered = true;
                }
                x.Update();
            }
        }

        public void OnScreenResize(Rectangle c) // c is the rectangle of the container
        {
            topPanelRect = new Rectangle(
                c.Left,
                c.Top,
                c.Width,
                UI_ItemSlot.size
                );

            bottomPanelRect = new Rectangle(
                c.Left,
                c.Bottom - UI_ItemSlot.size,
                c.Width,
                UI_ItemSlot.size
                );
        }

        public void UpdateList()
        {
            if (UI_Inventory_Container.Instance.page != UI_Inventory_Container.InventoryPage.Crafting)
            {
                return;
            }

            collection = new List<UI_Inventory_CraftingChild>();

            List<JSON_CraftingData> craftable = new List<JSON_CraftingData>(), uncraftable = new List<JSON_CraftingData>();
            foreach (JSON_CraftingData x in JSON_CraftingData.craftingData)
            {
                bool c = true;

                foreach (Item i in x.ingredients)
                {
                    if (!Player.Instance.inventoryItemCount.ContainsKey(i.type))
                    {
                        c = false;
                        continue;
                    }
                    if (Player.Instance.inventoryItemCount[i.type] < i.amount)
                    {
                        c = false;
                    }
                }

                if (c)
                {
                    craftable.Add(x);
                }
                else
                {
                    uncraftable.Add(x);
                }
            }

            foreach (JSON_CraftingData x in craftable)
            {
                collection.Add(new UI_Inventory_CraftingChild(this, parentContainer, new Rectangle(
                    rect.Left,
                    rect.Top + (int)(collection.Count * UI_ItemSlot.size),
                    rect.Width,
                    UI_ItemSlot.size
                    ), x.ingredients, x.product, true));
            }

            foreach (JSON_CraftingData x in uncraftable)
            {
                collection.Add(new UI_Inventory_CraftingChild(this, parentContainer, new Rectangle(
                    rect.Left,
                    rect.Top + (int)(collection.Count * UI_ItemSlot.size),
                    rect.Width,
                    UI_ItemSlot.size
                    ), x.ingredients, x.product, false));
            }

            maxScroll = rect.Height - (collection.Count * UI_ItemSlot.size);
        }

        public override void Draw(SpriteBatch spritebatch, int offsetX = 0, int offsetY = 0)
        {
            base.Draw(spritebatch, offsetX, offsetY);

            // to future me or whoever finds this, im sorry :)

            /*spritebatch.Draw(backgroundSprite[0], new Rectangle(rect.X + offsetX, rect.Y + offsetY, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            spritebatch.Draw(backgroundSprite[1], new Rectangle(rect.X + offsetX + pixel, rect.Y + offsetY, rect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            spritebatch.Draw(backgroundSprite[2], new Rectangle(rect.Right + offsetX - pixel, rect.Y + offsetY, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

            spritebatch.Draw(backgroundSprite[3], new Rectangle(rect.X + offsetX, rect.Y + offsetY + pixel, pixel, rect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            spritebatch.Draw(backgroundSprite[4], new Rectangle(rect.X + offsetX + pixel, rect.Y + offsetY + pixel, rect.Width - (pixel * 2), rect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            spritebatch.Draw(backgroundSprite[5], new Rectangle(rect.Right + offsetX - pixel, rect.Y + offsetY + pixel, pixel, rect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

            spritebatch.Draw(backgroundSprite[6], new Rectangle(rect.X + offsetX, rect.Bottom + offsetY - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            spritebatch.Draw(backgroundSprite[7], new Rectangle(rect.X + offsetX + pixel, rect.Bottom + offsetY - pixel, rect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            spritebatch.Draw(backgroundSprite[8], new Rectangle(rect.Right + offsetX - pixel, rect.Bottom + offsetY - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);*/

            GeneralDependencies.NineSliceDraw(spritebatch, backgroundSprite, rect, pixel, Color.White, 0.2f);

            foreach (UI_Inventory_CraftingChild x in collection)
            {
                x.Draw(spritebatch, offsetX:offsetX, offsetY:offsetY);
            }

            /*spritebatch.Draw(topPanel[0], new Rectangle(topPanelRect.X + offsetX, topPanelRect.Y + offsetY, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(topPanel[1], new Rectangle(topPanelRect.X + offsetX + pixel, topPanelRect.Y + offsetY, topPanelRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(topPanel[2], new Rectangle(topPanelRect.Right + offsetX - pixel, topPanelRect.Y + offsetY, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);

            spritebatch.Draw(topPanel[3], new Rectangle(topPanelRect.X + offsetX, topPanelRect.Y + offsetY + pixel, pixel, topPanelRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(topPanel[4], new Rectangle(topPanelRect.X + offsetX + pixel, topPanelRect.Y + offsetY + pixel, topPanelRect.Width - (pixel * 2), topPanelRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(topPanel[5], new Rectangle(topPanelRect.Right + offsetX - pixel, topPanelRect.Y + offsetY + pixel, pixel, topPanelRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);

            spritebatch.Draw(topPanel[6], new Rectangle(topPanelRect.X + offsetX, topPanelRect.Bottom + offsetY - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(topPanel[7], new Rectangle(topPanelRect.X + offsetX + pixel, topPanelRect.Bottom + offsetY - pixel, topPanelRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(topPanel[8], new Rectangle(topPanelRect.Right + offsetX - pixel, topPanelRect.Bottom + offsetY - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);*/

            GeneralDependencies.NineSliceDraw(spritebatch, topPanel, topPanelRect, pixel, Color.White, 0.4f);

            /*spritebatch.Draw(bottomPanel[0], new Rectangle(bottomPanelRect.X + offsetX, bottomPanelRect.Y + offsetY, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(bottomPanel[1], new Rectangle(bottomPanelRect.X + offsetX + pixel, bottomPanelRect.Y + offsetY, bottomPanelRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(bottomPanel[2], new Rectangle(bottomPanelRect.Right + offsetX - pixel, bottomPanelRect.Y + offsetY, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);

            spritebatch.Draw(bottomPanel[3], new Rectangle(bottomPanelRect.X + offsetX, bottomPanelRect.Y + offsetY + pixel, pixel, bottomPanelRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(bottomPanel[4], new Rectangle(bottomPanelRect.X + offsetX + pixel, bottomPanelRect.Y + offsetY + pixel, bottomPanelRect.Width - (pixel * 2), bottomPanelRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(bottomPanel[5], new Rectangle(bottomPanelRect.Right + offsetX - pixel, bottomPanelRect.Y + offsetY + pixel, pixel, bottomPanelRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);

            spritebatch.Draw(bottomPanel[6], new Rectangle(bottomPanelRect.X + offsetX, bottomPanelRect.Bottom + offsetY - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(bottomPanel[7], new Rectangle(bottomPanelRect.X + offsetX + pixel, bottomPanelRect.Bottom + offsetY - pixel, bottomPanelRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
            spritebatch.Draw(bottomPanel[8], new Rectangle(bottomPanelRect.Right + offsetX - pixel, bottomPanelRect.Bottom + offsetY - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.4f);*/

            GeneralDependencies.NineSliceDraw(spritebatch, bottomPanel, bottomPanelRect, pixel, Color.White, 0.4f);



            /*spritebatch.Draw(backgroundBorderSprite[0], new Rectangle(rect.X + offsetX, rect.Y + offsetY, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);
            spritebatch.Draw(backgroundBorderSprite[1], new Rectangle(rect.X + offsetX + pixel, rect.Y + offsetY, rect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);
            spritebatch.Draw(backgroundBorderSprite[2], new Rectangle(rect.Right + offsetX - pixel, rect.Y + offsetY, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);

            spritebatch.Draw(backgroundBorderSprite[3], new Rectangle(rect.X + offsetX, rect.Y + offsetY + pixel, pixel, rect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);
            spritebatch.Draw(backgroundBorderSprite[4], new Rectangle(rect.X + offsetX + pixel, rect.Y + offsetY + pixel, rect.Width - (pixel * 2), rect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);
            spritebatch.Draw(backgroundBorderSprite[5], new Rectangle(rect.Right + offsetX - pixel, rect.Y + offsetY + pixel, pixel, rect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);

            spritebatch.Draw(backgroundBorderSprite[6], new Rectangle(rect.X + offsetX, rect.Bottom + offsetY - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);
            spritebatch.Draw(backgroundBorderSprite[7], new Rectangle(rect.X + offsetX + pixel, rect.Bottom + offsetY - pixel, rect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);
            spritebatch.Draw(backgroundBorderSprite[8], new Rectangle(rect.Right + offsetX - pixel, rect.Bottom + offsetY - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);*/

            GeneralDependencies.NineSliceDraw(spritebatch, backgroundBorderSprite, rect, pixel, Color.White, 0.35f);
        }
    }
}
