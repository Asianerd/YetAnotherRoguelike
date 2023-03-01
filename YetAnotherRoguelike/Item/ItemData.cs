using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike
{
    class ItemData
    {
        // mostly used by being inherited by Chemical class or etc
        public ItemData() { }

        public static Dictionary<Item.DataType, ItemData> DeepCopy(Dictionary<Item.DataType, ItemData> target)
        {
            Dictionary<Item.DataType, ItemData> final = new Dictionary<Item.DataType, ItemData>();

            foreach (KeyValuePair<Item.DataType, ItemData> x in target)
            {
                final.Add(
                    x.Key,
                    x.Key switch
                    {
                        Item.DataType.Chemical => new Chemical(
                            new Dictionary<Chemical.Element, double>(((Chemical)x.Value).composition),
                            ((Chemical)x.Value).container.type
                            ),
                        _ => x.Value
                    }
                    );
            }

            return final;
        }
    }
}
