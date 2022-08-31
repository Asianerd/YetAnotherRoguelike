using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Gameplay.ItemStorage
{
    class ItemPopupParent : UI_Element
    {
        public static ItemPopupParent Instance;
        public static List<ItemPopupChild> collection;

        public static void Initialize()
        {
            collection = new List<ItemPopupChild>();
        }

        public ItemPopupParent()
        {
            Instance = this;
            collection = new List<ItemPopupChild>();
        }

        public void Add(Item.Type type, int amount)
        {
            if (collection.Where(n => n.type == type).Count() >= 1)
            {
                var item = collection.Where(n => n.type == type).First();
                item.amount += amount;
                item.age.AffectValue(1f);

                int i = collection.IndexOf(item);
                collection.RemoveAt(i);
                collection.Add(item);
                return;
            }
            collection.Add(new ItemPopupChild(type, amount));
        }

        public override void Update()
        {
            foreach(ItemPopupChild x in collection)
            {
                x.Update();
            }
            collection = collection.Where(n => n.age.Percent() > 0.1f).ToList();
            //TODO : fix bug
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            for(int i = 0; i < collection.Count; i++)
            {
                collection[i].Draw(spriteBatch, i);
            }
            base.Draw(spriteBatch, offset);
        }
    }
}
