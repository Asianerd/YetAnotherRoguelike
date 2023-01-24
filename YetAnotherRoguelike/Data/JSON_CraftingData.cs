using System;
using System.Linq ;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;

namespace YetAnotherRoguelike.Data
{
    class JSON_CraftingData
    {
        public static List<JSON_CraftingData> craftingData;

        public static void Initialize()
        {
            craftingData = new List<JSON_CraftingData>();
            foreach (JSON_CraftingData x in JsonSerializer.Deserialize<List<JSON_CraftingData>>(File.ReadAllText("Data/crafting_data.json")))
            {
                x.SetData();
                craftingData.Add(x);
            }

            Debug.WriteLine($"Crafting data count : {craftingData.Count}");
        }

        public JSON_CraftingData() { }

        public Dictionary<string, int> input { get; set; }
        public Dictionary<string, int> output { get; set; }

        public List<Item> ingredients;
        public Item product;

        public void SetData()
        {
            ingredients = new List<Item>();
            foreach (KeyValuePair<string, int> i in input)
            {
                ingredients.Add(new Item(
                    Enum.GetValues(typeof(Item.Type)).Cast<Item.Type>().Where(n => n.ToString() == i.Key).First(),
                    i.Value
                    ));
            }

            product = new Item(
                Enum.GetValues(typeof(Item.Type)).Cast<Item.Type>().Where(n => n.ToString() == output.ToList()[0].Key).First(),
                output.ToList()[0].Value
                );
        }
    }
}
