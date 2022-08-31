using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace YetAnotherRoguelike.Gameplay
{
    class Item
    {
        // Item class has own Random to ensure every drop is more randomized
        public static Random dropRNG = new Random();
        public static int stackSize = 64;

        public static Dictionary<Type, Texture2D> itemSprites = new Dictionary<Type, Texture2D>();
        public static Dictionary<Tile.Type, Dictionary<Type, int>> lootTable = new Dictionary<Tile.Type, Dictionary<Type, int>>();

        #region Statics
        public static void Initialize()
        {
            foreach (Type t in Enum.GetValues(typeof(Type)))
            {
                if (t == Type.None)
                {
                    continue;
                }
                itemSprites.Add(t, Game.Instance.Content.Load<Texture2D>($"Item_sprites/{t}"));
            }

            //var x = JsonSerializer.Deserialize<List<ItemData>>(File.ReadAllText("Data/tile_data.json"));
            foreach (KeyValuePair<string, ItemData> item in JsonSerializer.Deserialize<Dictionary<string, ItemData>>(File.ReadAllText("Data/tile_data.json")))
            {
                var x = item.Value;
                x.blockType = Enum.GetValues(typeof(Tile.Type)).Cast<Tile.Type>().ToList().Where(n => n.ToString() == item.Key).First();
                ItemData.collection.Add(x);
            }
            foreach (ItemData i in ItemData.collection)
            {
                i.AssignData();
                lootTable.Add(i.blockType, i.lootTable);
            }
        }

        public static Dictionary<Item.Type, int> FetchDropChance(Tile.Type type, bool addOffset = true)
        {
            Dictionary<Item.Type, int> result = lootTable.Keys.Contains(type) ? lootTable[type] : new Dictionary<Item.Type, int>() { };
            if (addOffset)
            {
                List<Item.Type> k = result.Keys.ToList();
                foreach (Item.Type t in k)
                {
                    result[t] += dropRNG.Next(-2, 2);
                    if (result[t] <= 0)
                    {
                        result[t] = 1;
                    }
                }
            }
            return result;
        }
        #endregion

        public Type type;
        public int amount;

        public Item(Type _type, int _amount)
        {
            type = _type;
            amount = _amount;
        }


        public enum Type
        {
            None,

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

        public class ItemData
        {
            public static List<ItemData> collection = new List<ItemData>();

            public ItemData() { }

            public void AssignData()
            {
                //Debug.WriteLine(_rawBlockType == null);
                //blockType = Enum.GetValues(typeof(Tile.Type)).Cast<Tile.Type>().ToList().Where(n => n.ToString() == _rawBlockType).First();
                foreach (string x in loot.Keys)
                {
                    lootTable.Add(
                        Enum.GetValues(typeof(Item.Type)).Cast<Item.Type>().ToList().Where(n => n.ToString() == x).First(),
                        loot[x]
                        );
                }
            }

            public Dictionary<string, int> loot { get; set; }

            public Tile.Type blockType;
            public Dictionary<Item.Type, int> lootTable = new Dictionary<Item.Type, int>();
        }
    }
}
