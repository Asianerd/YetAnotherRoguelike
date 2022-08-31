using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike.Gameplay.ItemStorage
{
    class Inventory : UI_Container
    {
        public static Inventory Instance;
        public static int inventorySize = 30;
        static int rowLength = 5;

        public List<ItemSlot> slots;

        public GameValue age = new GameValue(0, 30, 1, 0);
        public float i = 0;

        public Inventory(List<UI_Element> _elements) : base(_elements)
        {
            Instance = this;

            ItemSlot.Initialize();

            slots = new List<ItemSlot>();
            for (int x = 0; x < inventorySize; x++)
            {
                slots.Add(new ItemSlot(Player.Instance.inventory[x], new Vector2(
                    ((x * ItemSlot.size) + (ItemSlot.size / 2)) - ((MathF.Floor(x / (float)rowLength) * (ItemSlot.size * rowLength))),
                    (Game.screenSize.Y / 2) + (ItemSlot.size * (MathF.Floor(x / (float)rowLength))) - (((inventorySize / rowLength) * ItemSlot.size) / 2f)
                    )));
            }

            foreach(UI_Element x in slots)
            {
                elements.Add(x);
            }
        }

        public void Toggle()
        {
            active = !active;
        }

        public override void UpdateAll()
        {
            age.Regenerate(active ? Game.compensation : -Game.compensation);

            if (age.Percent() <= 0f)
            {
                return;
            }
            base.UpdateAll();
        }

        public override void DrawAll(SpriteBatch spriteBatch, Point offset)
        {
            if(age.Percent() <= 0f)
            {
                //return;
            }
            base.DrawAll(spriteBatch, new Point((int)(MathF.Sin(age.Percent() * MathF.PI / 2f) * 400) - 400, 0));
        }
    }
}
