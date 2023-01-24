using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI
{
    class UI_Inventory_CraftingChild : UI_Element
    {
        public static new List<Texture2D> backgroundSprite;
        public static List<Texture2D> outlineSprite;
        public static Texture2D arrowSprite;
        public static Vector2 arrowOffset;
        public static Vector2 itemOffset;

        public static new void Initialize()
        {
            backgroundSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingChildBackground"), 4, 4);
            outlineSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingChildOutline"), 4, 4);
            arrowSprite = Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingChildArrow");
            arrowOffset = new Vector2(180, 0);
            itemOffset = new Vector2(0, UI_ItemSlot.size * 0.5f);
            // (currently tryna make the displayed input items line up properly)
        }

        UI_Inventory_CraftingParent craftingParent;
        float originalY;
        Vector2 itemIncrement;

        List<Item> inputs;
        Item output;

        public UI_Inventory_CraftingChild(UI_Inventory_CraftingParent _craftParent, UI_Container p, Rectangle r, List<Item> _in, Item _out) : base(p, r)
        {
            craftingParent = _craftParent;
            originalY = r.Y;

            inputs = _in;
            output = _out;
            itemIncrement = new Vector2(180 / inputs.Count, 0);

        }

        public override void FetchHovered()
        {
            rect.Y = (int)(originalY + craftingParent.scroll);
            base.FetchHovered();
        }

        public override void Update()
        {
            base.Update();
            if (hovered)
            {
                Cursor.state = Cursor.CursorStates.Select;
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
            if (!craftingParent.rect.Intersects(renderedRect))
            {
                return;
            }

            spritebatch.Draw(backgroundSprite[0], new Rectangle(renderedRect.X, renderedRect.Y, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[1], new Rectangle(renderedRect.X + pixel, renderedRect.Y, renderedRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[2], new Rectangle(renderedRect.Right - pixel, renderedRect.Y, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

            spritebatch.Draw(backgroundSprite[3], new Rectangle(renderedRect.X, renderedRect.Y + pixel, pixel, renderedRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[4], new Rectangle(renderedRect.X + pixel, renderedRect.Y + pixel, renderedRect.Width - (pixel * 2), renderedRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[5], new Rectangle(renderedRect.Right - pixel, renderedRect.Y + pixel, pixel, renderedRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

            spritebatch.Draw(backgroundSprite[6], new Rectangle(renderedRect.X, renderedRect.Bottom - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[7], new Rectangle(renderedRect.X + pixel, renderedRect.Bottom - pixel, renderedRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[8], new Rectangle(renderedRect.Right - pixel, renderedRect.Bottom - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

            spritebatch.Draw(arrowSprite, renderedRect.Location.ToVector2() + arrowOffset, null, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.31f);

            foreach (var x in inputs.Select((value, index) => new { value, index }))
            {
                spritebatch.Draw(Item.itemSprites[x.value.type], renderedRect.Location.ToVector2() + (itemIncrement * (x.index + 0.5f)) + itemOffset, null, Color.White, 0f, Item.spriteOrigin, 3f, SpriteEffects.None, 0.32f);
            }

            if (!hovered)
            {
                return;
            }

            spritebatch.Draw(outlineSprite[0], new Rectangle(renderedRect.X, renderedRect.Y, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[1], new Rectangle(renderedRect.X + pixel, renderedRect.Y, renderedRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[2], new Rectangle(renderedRect.Right - pixel, renderedRect.Y, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);

            spritebatch.Draw(outlineSprite[3], new Rectangle(renderedRect.X, renderedRect.Y + pixel, pixel, renderedRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[4], new Rectangle(renderedRect.X + pixel, renderedRect.Y + pixel, renderedRect.Width - (pixel * 2), renderedRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[5], new Rectangle(renderedRect.Right - pixel, renderedRect.Y + pixel, pixel, renderedRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);

            spritebatch.Draw(outlineSprite[6], new Rectangle(renderedRect.X, renderedRect.Bottom - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[7], new Rectangle(renderedRect.X + pixel, renderedRect.Bottom - pixel, renderedRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[8], new Rectangle(renderedRect.Right - pixel, renderedRect.Bottom - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
        }
    }
}
