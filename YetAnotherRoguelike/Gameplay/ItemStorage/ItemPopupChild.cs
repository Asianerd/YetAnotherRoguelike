using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike.Gameplay.ItemStorage
{
    class ItemPopupChild
    {
        public Item.Type type;
        int amount;
        public string title;

        public GameValue age;

        public Vector2 textOffset;

        public ItemPopupChild(Item.Type _type, int _amount)
        {
            type = _type;
            amount = _amount;

            age = new GameValue(0, 100, -1);

            UpdateRect();
        }

        public void AddAmount(int _amount)
        {
            amount += _amount;
            UpdateRect();
        }

        public void UpdateRect()
        {
            title = $"{type} : x{amount}";
            textOffset = UI.defaultFont.MeasureString(title) / 2f;
        }

        public void Update()
        {
            age.Regenerate(Game.compensation);
        }

        public void Draw(SpriteBatch spriteBatch, int index)
        {
            /*spriteBatch.DrawString(UI.defaultFont, title, new Vector2(
                (Game.screenSize.X / 2f) - textOffset.X,
                (Game.screenSize.Y - (50 * index)) - textOffset.Y - 50
                ), ((-4f * (1f - (float)age.Percent())) + 4f) * Color.White);*/
            spriteBatch.DrawString(UI.defaultFont, title, Camera.WorldToScreen(Player.Instance.position) + new Vector2(
                -textOffset.X,
                -textOffset.Y - (50 * index) - 50
                ), ((-4f * (1f - (float)age.Percent())) + 4f) * Color.White);
        }
    }
}
