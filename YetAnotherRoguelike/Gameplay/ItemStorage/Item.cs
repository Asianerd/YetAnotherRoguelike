using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike.Gameplay
{
    class Item
    {
        public static int stackSize = 64;

        public Type type;
        public int amount;

        public Item(Type _type, int _amount)
        {
            type = _type;
            amount = _amount;
        }


        public enum Type
        {
            Stone,
            Coal,

            Bauxite,
            Hematite,
            Sphalerite,
            Calamine,
            Galena,
            Cinnabar,
            Argentite,
            Bismuth
        }
    }
}
