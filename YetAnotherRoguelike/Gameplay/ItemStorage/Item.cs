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
        public static int regularStackSize = 256;

        public static Dictionary<Type, Texture2D> itemSprites = new Dictionary<Type, Texture2D>();
        public static Dictionary<Tile.Type, Dictionary<Type, int>> lootTable = new Dictionary<Tile.Type, Dictionary<Type, int>>();
        public static Dictionary<Type, StackType> itemStackTypes = new Dictionary<Type, StackType>();
        public static Dictionary<StackType, int> stackSizes = new Dictionary<StackType, int>();

        #region Statics
        public static void Initialize()
        {
            stackSizes = new Dictionary<StackType, int>()
            {
                { StackType.Regular, regularStackSize },
                { StackType.Single, 1 }
            };
            itemStackTypes = new Dictionary<Type, StackType>()
            {

            };

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

        public static Dictionary<Type, int> FetchDropChance(Tile.Type type, bool addOffset = true)
        {
            Dictionary<Type, int> result = lootTable.Keys.Contains(type) ? lootTable[type] : new Dictionary<Item.Type, int>() { };
            if (addOffset)
            {
                List<Type> k = result.Keys.ToList();
                foreach (Type t in k)
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


        Type _type;
        public Type type
        {
            get { return _type; }
            set
            {
                _type = value;
                UpdateStackSize();
            }
        }
        int _amount;
        public int amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                UpdateStackSize();
            }
        }
        public int stackSize;
        public UsageType usageType;

        public static Item Empty()
        {
            return new Item(Type.None, 0);
        }

        public Item(Type __type, int __amount)
        {
            type = __type;
            amount = __amount;
            usageType = UsageType.Block;

            stackSize = itemStackTypes.ContainsKey(type) ? stackSizes[itemStackTypes[type]] : regularStackSize;
        }

        public void UpdateStackSize()
        {
            stackSize = itemStackTypes.ContainsKey(type) ? stackSizes[itemStackTypes[type]] : regularStackSize;
        }

        public bool Full()
        {
            return amount >= stackSize;
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

        public enum StackType
        {
            Regular,
            Single
        }

        public enum UsageType
        {
            Block,
            Weapon
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
                        Enum.GetValues(typeof(Type)).Cast<Type>().ToList().Where(n => n.ToString() == x).First(),
                        loot[x]
                        );
                }
            }

            public Dictionary<string, int> loot { get; set; }

            public Tile.Type blockType;
            public Dictionary<Type, int> lootTable = new Dictionary<Type, int>();
        }
    }
}
