using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Xna.Framework;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.Gameplay
{
    class Item
    {
        // Item class has own Random to ensure every drop is more randomized
        public static Random dropRNG = new Random();
        public static int regularStackSize = 256;

        public static Vector2 spriteOrigin;
        public static Dictionary<Type, Texture2D> itemSprites = new Dictionary<Type, Texture2D>();
        public static Dictionary<Tile.Type, Dictionary<Type, int>> lootTable = new Dictionary<Tile.Type, Dictionary<Type, int>>();
        public static Dictionary<Type, StackType> itemStackTypes = new Dictionary<Type, StackType>();
        public static Dictionary<StackType, int> stackSizes = new Dictionary<StackType, int>();

        #region Statics
        public static void Initialize()
        {
            spriteOrigin = new Vector2(8);
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
            foreach (KeyValuePair<string, JSON_BlockData> item in JsonSerializer.Deserialize<Dictionary<string, JSON_BlockData>>(File.ReadAllText("Data/block_data.json")))
            {
                var x = item.Value;
                x.blockType = Enum.GetValues(typeof(Tile.Type)).Cast<Tile.Type>().ToList().Where(n => n.ToString() == item.Key).First();
                JSON_BlockData.collection.Add(x);
            }
            foreach (JSON_BlockData i in JSON_BlockData.collection)
            {
                i.AssignData();
                lootTable.Add(i.blockType, i.lootTable);
            }

            LightTile.lightTileSources = new Dictionary<Tile.Type, LightSource>();
            foreach (KeyValuePair<string, JSON_Light> item in JsonSerializer.Deserialize<Dictionary<string, JSON_Light>>(File.ReadAllText("Data/block_lightsource_data.json")))
            {
                var x = item.Value;
                x.blockType = Enum.GetValues(typeof(Tile.Type)).Cast<Tile.Type>().ToList().Where(n => n.ToString().ToLower() == item.Key).First();
                LightTile.lightTileSources.Add(x.blockType, new LightSource(Vector2.Zero, x.s, x.r, new Color(x.c[0], x.c[1], x.c[2])));
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
            }
        }
        int _amount;
        public int amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                UpdateSelf();
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

        public void UpdateSelf()
        {
            UpdateStackSize();
            if (_amount <= 0)
            {
                _type = Type.None;
                _amount = 0;
            }
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



        public class JSON_BlockData
        {
            public static List<JSON_BlockData> collection = new List<JSON_BlockData>();

            /*public static JSON_BlockData Find(Tile.Type t)
            {
                foreach (JSON_BlockData x in collection)
                {
                    if (x.blockType == t)
                    {
                        return x;
                    }
                }
                return null;
            }*/

            public JSON_BlockData() { }

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

        public class JSON_Light
        {
            public float s { get; set; }
            public float r { get; set; }
            public int[] c { get; set; }

            public Tile.Type blockType;
            public LightSource light;
        }
    }
}
