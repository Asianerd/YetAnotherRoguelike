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
        public int amount;
        public string name;

        public Vector2 position;
        public GameValue age;

        public ItemPopupChild(Item.Type _type, int _amount)
        {
            type = _type;
            amount = _amount;

            age = new GameValue(0, 100, -1);
        }

        public void Update()
        {
            age.Regenerate(Game.compensation);
        }

        public void Draw(SpriteBatch spriteBatch, int index)
        {
            spriteBatch.DrawString(UI.defaultFont, $"{type} : {amount}", new Vector2(500, 500 + (50 * index)), ((-4f * (1f - (float)age.Percent())) + 4f) * Color.White);
        }
    }
}
