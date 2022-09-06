using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using YetAnotherRoguelike.UI_Classes.Player_UI;
using YetAnotherRoguelike.UI_Classes;
using Microsoft.Xna.Framework.Input;

namespace YetAnotherRoguelike.Gameplay.ItemStorage
{
    class Inventory : UI_Container
    {
        public static Inventory Instance;
        public static int inventorySize = 30;
        static int rowLength = 5;

        public List<ItemSlot> slots;

        public GameValue age = new GameValue(0, 15, 1, 0);
        public float i = 0;

        public Vector2 selectionBoxPosition = Vector2.Zero;
        public GameValue selectionAge = new GameValue(0, 60, 1, 0);
        public GameValue selectionBob = new GameValue(0, 60, 1, _repeat: true);

        public Inventory(List<UI_Element> _elements) : base(_elements)
        {
            Instance = this;

            ItemSlot.Initialize();
            var _ = new Hotbar(new List<UI_Element>());

            elements.Add(new Background(new Rectangle(
                -50,
                (int)(Game.screenSize.Y/2) - 250,
                550,
                500
                )));

            slots = new List<ItemSlot>();
            for (int x = 0; x < inventorySize; x++)
            {
                slots.Add(new ItemSlot(Player.Instance.inventory[x], new Vector2(
                    ((x * ItemSlot.size) + (ItemSlot.size / 2)) - ((MathF.Floor(x / (float)rowLength) * (ItemSlot.size * rowLength))),
                    (Game.screenSize.Y / 2) + (ItemSlot.size * (MathF.Floor(x / (float)rowLength))) - (((inventorySize / rowLength) * ItemSlot.size) / 2f)
                    )));
            }

            foreach (HotbarSection x in Hotbar.Instance.sections)
            {
                slots.Add(new ItemSlot(x.item, new Vector2(400, (Game.screenSize.Y / 2f) + (((x.index - 2) * (ItemSlot.size + 16)) + 8))));
            }

            foreach (UI_Element x in slots)
            {
                elements.Add(x);
            }
        }

        public override void UpdateAll()
        {
            foreach (var k in (new Keys[] { Keys.D1, Keys.D2, Keys.D3, Keys.D4 }).Select((value, index) => new { value, index }))
            {
                if (Input.collection[k.value].active)
                {
                    Hotbar.selectedSection = k.index;
                }
            }

            age.Regenerate(active ? Game.compensation : -Game.compensation);

            if (age.Percent() <= 0f)
            {
                return;
            }

            selectionBob.Regenerate(Game.compensation);
            

            base.UpdateAll();
            var x = slots.Where(n => n.hovered);
            ItemSlot.hoveredSlot = x.Count() > 0 ? x.Last() : null;
            if (ItemSlot.hoveredSlot != null)
            {
                Cursor.state = MouseInput.left.isPressed ? Cursor.CursorStates.Select_pressed : Cursor.CursorStates.Select;
                selectionBoxPosition = Vector2.Lerp(selectionBoxPosition, ItemSlot.hoveredSlot.rect.Center.ToVector2(), 0.5f);
                //selectionAge.Regenerate(4 * Game.compensation);
            }
            /*else
            {
                selectionAge.Regenerate(-4 * Game.compensation);
            }*/
            selectionAge.Regenerate((4 * Game.compensation) * (Cursor.item.type == Item.Type.None ? -1 : 1));
        }

        public override void DrawAll(SpriteBatch spriteBatch, Point offset)
        {
            if (age.Percent() <= 0f)
            {
                return;
            }
            base.DrawAll(spriteBatch, new Point((int)(MathF.Sin(age.Percent() * MathF.PI / 2f) * 500) - 500, 0));
            spriteBatch.Draw(ItemSlot.selectionSprite, selectionBoxPosition, null, selectionAge.Percent() * Color.White, 0f, ItemSlot.selectionSpriteOrigin, 4f + (MathF.Sin(selectionBob.Percent() * 2 * MathF.PI) * 0.2f), SpriteEffects.None, 0f);
        }
    }
}
