using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.PhysicsObject;

namespace YetAnotherRoguelike.UI
{
    class UI_Inventory_CraftingChild : UI_Element
    {
        public static new List<Texture2D> backgroundSprite;
        public static List<Texture2D> outlineSprite;
        public static Texture2D arrowSprite, arrowProgressSprite;
        public static Vector2 arrowOffset;
        public static Vector2 itemOffset, outputOffset;
        public static Point arrowProgressSize;
        public static Vector2 arrowProgressOffset;
        public static Color disabledColor;

        public static new void Initialize()
        {
            backgroundSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingChildBackground"), 4, 4);
            outlineSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingChildOutline"), 4, 4);
            arrowSprite = Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingChildArrow");
            arrowProgressSprite = Game.Instance.Content.Load<Texture2D>("UI/Inventory/CraftingChildArrowProgress");
            arrowProgressSize = arrowProgressSprite.Bounds.Size;
            arrowProgressOffset = new Vector2(4 * 4, 0);
            arrowOffset = new Vector2(180, 0);
            itemOffset = new Vector2(0, UI_ItemSlot.size * 0.5f);
            outputOffset = new Vector2(-UI_ItemSlot.size, 0);

            disabledColor = new Color(Color.Black, 0.6f);
        }

        bool craftable;

        UI_Inventory_CraftingParent craftingParent;
        float originalY;
        Vector2 itemIncrement;

        GameValue progress;
        List<Item> inputs;
        Item output;

        public UI_Inventory_CraftingChild(UI_Inventory_CraftingParent _craftParent, UI_Container p, Rectangle r, List<Item> _in, Item _out, bool _craftable) : base(p, r)
        {
            craftingParent = _craftParent;
            originalY = r.Y;

            craftable = _craftable;

            progress = new GameValue(0, 20, 1, 0);

            inputs = _in;
            output = _out;
            itemIncrement = new Vector2(180 / inputs.Count, 0);

            outputOffset = new Vector2(rect.Width - (UI_ItemSlot.size * 0.5f), 0);
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
                if (craftable)
                {
                    Cursor.state = Cursor.CursorStates.Select;
                    progress.Regenerate(Game.compensation * (MouseInput.left.isPressed ? 4f : 0f));
                }

                Point normalized = Game.mouseState.Position - rect.Location;

                type = normalized.X >= rect.Width - UI_ItemSlot.size ? ElementType.CraftingChildOutput : ElementType.CraftingChildInput;
            }

            if (!craftable)
            {
                return;
            }

            if (progress.Percent() >= 1f)
            {
                progress.AffectValue(0f);

                foreach (Item x in inputs)
                {
                    Player.Instance.InventoryRemove(x.type, x.amount);
                }
                GroundItem.collection.Add(new GroundItem(new Item(output.type, output.amount, ItemData.DeepCopy(output.data)), Player.Instance.position, Player.Instance.position, c: false));
            }

            progress.Regenerate(Game.compensation * -3f);
        }

        public override void Draw(SpriteBatch spritebatch, int offsetX = 0, int offsetY = 0)
        {
            base.Draw(spritebatch, offsetX, offsetY);
            multiRect.X = rect.X + offsetX;
            multiRect.Y = rect.Y + offsetY;
            multiRect.Size = rect.Size;
            if (!craftingParent.rect.Intersects(multiRect))
            {
                return;
            }

            if (!craftable)
            {
                spritebatch.Draw(blank, multiRect, null, disabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.375f);
            }

            /*spritebatch.Draw(backgroundSprite[0], new Rectangle(multiRect.X, multiRect.Y, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[1], new Rectangle(multiRect.X + pixel, multiRect.Y, multiRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[2], new Rectangle(multiRect.Right - pixel, multiRect.Y, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

            spritebatch.Draw(backgroundSprite[3], new Rectangle(multiRect.X, multiRect.Y + pixel, pixel, multiRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[4], new Rectangle(multiRect.X + pixel, multiRect.Y + pixel, multiRect.Width - (pixel * 2), multiRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[5], new Rectangle(multiRect.Right - pixel, multiRect.Y + pixel, pixel, multiRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

            spritebatch.Draw(backgroundSprite[6], new Rectangle(multiRect.X, multiRect.Bottom - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[7], new Rectangle(multiRect.X + pixel, multiRect.Bottom - pixel, multiRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
            spritebatch.Draw(backgroundSprite[8], new Rectangle(multiRect.Right - pixel, multiRect.Bottom - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);*/

            GeneralDependencies.NineSliceDraw(spritebatch, backgroundSprite, multiRect, pixel, Color.White, 0.3f);

            Rectangle progressRect = new Rectangle(0, 0, (int)(arrowProgressSize.X * progress.Percent()), arrowProgressSize.Y);
            spritebatch.Draw(arrowSprite, multiRect.Location.ToVector2() + arrowOffset, null, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.31f);
            spritebatch.Draw(arrowProgressSprite, multiRect.Location.ToVector2() + arrowOffset + arrowProgressOffset, progressRect, Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0.315f);

            foreach (var x in inputs.Select((value, index) => new { value, index }))
            {
                spritebatch.Draw(x.value.FetchSprite(), multiRect.Location.ToVector2() + (itemIncrement * (x.index + 0.5f)) + itemOffset, null, Color.White, 0f, Item.spriteOrigin, 3f, SpriteEffects.None, 0.32f);

                int _inInventory = Player.Instance.inventoryItemCount.ContainsKey(x.value.type) ? Player.Instance.inventoryItemCount[x.value.type] : 0;
                int _needed = x.value.amount;

                string text = $"{_inInventory}/{_needed}";
                Vector2 stringSize = mainFont.MeasureString(text);

                spritebatch.DrawString(mainFont, text,
                    multiRect.Location.ToVector2()
                    + (itemIncrement * (x.index + 0.5f))
                    + itemOffset
                    + new Vector2(
                        0,
                        (stringSize.Y * 0.5f)
                        ),
                    _inInventory < _needed ? Color.Red : Color.White, 0f, stringSize * 0.5f, 0.9f, SpriteEffects.None, 0.325f);
            }

            Vector2 _stringSize = mainFont.MeasureString(output.amount.ToString());
            spritebatch.Draw(output.FetchSprite(), multiRect.Location.ToVector2() + itemOffset + outputOffset, null, Color.White, 0f, Item.spriteOrigin, 3f, SpriteEffects.None, 0.32f);
            spritebatch.DrawString(mainFont, output.amount.ToString(), multiRect.Location.ToVector2() + itemOffset + outputOffset + new Vector2(UI_ItemSlot.size * 0.2f, (_stringSize.Y * 0.5f)), Color.White, 0f, _stringSize * 0.5f, 0.9f, SpriteEffects.None, 0.325f);

            if (!hovered)
            {
                return;
            }

            /*spritebatch.Draw(outlineSprite[0], new Rectangle(multiRect.X, multiRect.Y, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[1], new Rectangle(multiRect.X + pixel, multiRect.Y, multiRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[2], new Rectangle(multiRect.Right - pixel, multiRect.Y, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);

            spritebatch.Draw(outlineSprite[3], new Rectangle(multiRect.X, multiRect.Y + pixel, pixel, multiRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[4], new Rectangle(multiRect.X + pixel, multiRect.Y + pixel, multiRect.Width - (pixel * 2), multiRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[5], new Rectangle(multiRect.Right - pixel, multiRect.Y + pixel, pixel, multiRect.Height - (pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);

            spritebatch.Draw(outlineSprite[6], new Rectangle(multiRect.X, multiRect.Bottom - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[7], new Rectangle(multiRect.X + pixel, multiRect.Bottom - pixel, multiRect.Width - (pixel * 2), pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);
            spritebatch.Draw(outlineSprite[8], new Rectangle(multiRect.Right - pixel, multiRect.Bottom - pixel, pixel, pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.37f);*/

            GeneralDependencies.NineSliceDraw(spritebatch, outlineSprite, multiRect, pixel, Color.White, 0.37f);
        }

        public string FetchInput()
        {
            string final = "";
            foreach (Item x in inputs)
            {
                final += $"({(Player.Instance.inventoryItemCount.ContainsKey(x.type) ? Player.Instance.inventoryItemCount[x.type] : 0)}/{x.amount}) {x.FetchName()}\n";
            }
            return final;
        }

        public string FetchOutput()
        {
            return $"x{output.amount} {output.FetchName()}";
        }
    }
}
