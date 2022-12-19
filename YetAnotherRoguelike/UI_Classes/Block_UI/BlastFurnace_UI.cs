using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using YetAnotherRoguelike.Gameplay;
using YetAnotherRoguelike.Gameplay.ItemStorage;
using YetAnotherRoguelike.Tile_Classes.Blocks;

namespace YetAnotherRoguelike.UI_Classes.Block_UI
{
    class BlastFurnace_UI : UI_Container
    {
        public static BlastFurnace_UI Instance;

        Background background;
        ItemSlot inputSlot, fuelSlot, outputSlot;

        BlastFurnace block;

        public BlastFurnace_UI() : base(new List<UI_Element>())
        {
            Instance = this;

            background = new Background(new Rectangle(0, 0, 400, 200));
            inputSlot = new ItemSlot(Item.Empty(), Vector2.Zero);
            fuelSlot = new ItemSlot(Item.Empty(), Vector2.Zero);
            outputSlot = new ItemSlot(Item.Empty(), Vector2.Zero);

            elements = new List<UI_Element>()
            {
                background,
                inputSlot,
                fuelSlot,
                outputSlot
            };
        }

        public void AssignData(BlastFurnace _block)
        {
            block = _block;

            inputSlot.item = block.input;
            fuelSlot.item = block.fuel;
            outputSlot.item = block.output;

            Vector2 renderedPosition = block.rect.Center.ToVector2() + Camera.Instance.renderOffset;
            background.rect.X = (int)(renderedPosition.X - (background.rect.Width / 2f));
            background.rect.Y = (int)(renderedPosition.Y - (background.rect.Height / 2f));
        }

        public override void UpdateAll()
        {
            if (!active)
            {
                return;
            }
            base.UpdateAll();
        }

        public override void DrawAll(SpriteBatch spriteBatch, Point offset)
        {
            if (!active)
            {
                return;
            }

            base.DrawAll(spriteBatch, offset);
        }
    }
}
