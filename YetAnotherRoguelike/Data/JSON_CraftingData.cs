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
        }

        public JSON_CraftingData() { }

        public Dictionary<string, int> input { get; set; }
        public Dictionary<string, int> output { get; set; }
        public Dictionary<string, Dictionary<string, string>> d { get; set; }

        public List<Item> ingredients;
        public Item product;
        public Dictionary<Item.DataType, ItemData> data;

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

            if (d != null)
            {
                data = new Dictionary<Item.DataType, ItemData>();
                foreach (KeyValuePair<string, Dictionary<string, string>> x in d)
                {
                    Item.DataType t = GeneralDependencies.ParseEnum<Item.DataType>(x.Key);
                    switch (t)
                    {
                        case Item.DataType.Chemical:
                            /* Chemical : {
                             *      "Fe" : "0.1",
                             *      "crucible_size" : "Large"
                             * }
                             */
                            Dictionary<Chemical.Element, double> c = new Dictionary<Chemical.Element, double>();

                            foreach (KeyValuePair<string, string> i in x.Value)
                            {
                                if (i.Key == "crucible_size")
                                {
                                    continue;
                                }
                                c.Add(
                                    GeneralDependencies.ParseEnum<Chemical.Element>(i.Key),
                                    double.Parse(i.Value)
                                    );
                            }
                            ChemicalContainer.CrucibleType _crucibleType = ChemicalContainer.CrucibleType.Medium;
                            if (x.Value.ContainsKey("crucible_size"))
                            {
                                _crucibleType = GeneralDependencies.ParseEnum<ChemicalContainer.CrucibleType>(x.Value["crucible_size"]);
                            }

                            data.Add(t, new Chemical(
                                _composition: c,
                                t: _crucibleType
                                ));
                            break;
                        default:
                            break;
                    }
                }
            }

            product = new Item(
                Enum.GetValues(typeof(Item.Type)).Cast<Item.Type>().Where(n => n.ToString() == output.ToList()[0].Key).First(),
                output.ToList()[0].Value,
                d:data
                );
        }
    }
}
