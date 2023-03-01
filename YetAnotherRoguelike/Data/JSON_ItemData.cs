using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.Data
{
    class JSON_ItemData
    {
        public static Dictionary<Item.Type, JSON_ItemData> itemData;

        public static void Initialize()
        {
            itemData = new Dictionary<Item.Type, JSON_ItemData>();
            foreach (KeyValuePair<string, JSON_ItemData> item in JsonSerializer.Deserialize<Dictionary<string, JSON_ItemData>>(File.ReadAllText("Data/item_data.json")))
            {
                var x = item.Value;
                //x.itemType = Enum.GetValues(typeof(Item.Type)).Cast<Item.Type>().Where(n => n.ToString() == item.Key).First();
                x.itemType = GeneralDependencies.ParseEnum<Item.Type>(item.Key);
                itemData.Add(x.itemType, x);
            }
            foreach (Item.Type x in Enum.GetValues(typeof(Item.Type)).Cast<Item.Type>())
            {
                if (itemData.ContainsKey(x))
                {
                    continue;
                }
                var i = new JSON_ItemData();
                i.itemType = x;
                itemData.Add(x, i);
            }
            foreach (JSON_ItemData x in itemData.Values)
            {
                x.SetData();
            }
        }

        public static JSON_ItemData FetchData(Item.Type t)
        {
            if (itemData.ContainsKey(t))
            {
                return itemData[t];
            }
            return null;
        }

        public string name { get; set; }
        public string tile_placed { get; set; }
        public string stack_type { get; set; }
        public string selection_type { get; set; }

        public Item.Type itemType;
        public Tile.BlockType tilePlaced = Tile.BlockType.Air;
        public Item.StackType stackType = Item.StackType.Regular;
        public Item.Species selectionType = Item.Species.Unset;

        public JSON_ItemData() { }

        public void SetData()
        {
            if (name == null)
            {
                name = itemType.ToString();
            }
            if (tile_placed != null)
            {
                tilePlaced = Enum.GetValues(typeof(Tile.BlockType)).Cast<Tile.BlockType>().Where(n => n.ToString() == tile_placed).First();
            }
            if (stack_type != null)
            {
                stackType = Enum.GetValues(typeof(Item.StackType)).Cast<Item.StackType>().Where(n => n.ToString() == stack_type).First();
            }
            if (selection_type != null)
            {
                selectionType = Enum.GetValues(typeof(Item.Species)).Cast<Item.Species>().Where(n => n.ToString() == selection_type).First();
            }
        }
    }
}
