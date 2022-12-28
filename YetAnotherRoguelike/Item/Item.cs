using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Item
    {
        public static Dictionary<Tile_Classes.Tile.BlockType, Dictionary<Type, int>> blockDrops;

        public static Dictionary<Type, Texture2D> itemSprites;
        public static Dictionary<Type, StackType> itemStackTypes;
        public static Dictionary<StackType, int> stackSizes;
        public static Vector2 spriteOrigin;
        public static int maxStackSize = 256;

        public static void Initialize()
        {
            spriteOrigin = new Vector2(8);

            itemSprites = new Dictionary<Type, Texture2D>();
            foreach (Type t in Enum.GetValues(typeof(Type)).Cast<Type>())
            {
                if (t == Type.None)
                {
                    continue;
                }
                itemSprites.Add(t, Game.Instance.Content.Load<Texture2D>($"Item_sprites/{t}"));
            }

            itemStackTypes = new Dictionary<Type, StackType>();

            stackSizes = new Dictionary<StackType, int>()
            {
                { StackType.Regular, maxStackSize },
                { StackType.Single, 1 }
            };

            blockDrops = new Dictionary<Tile_Classes.Tile.BlockType, Dictionary<Type, int>>();
            foreach (Tile_Classes.Tile.BlockType t in Enum.GetValues(typeof(Tile_Classes.Tile.BlockType)).Cast<Tile_Classes.Tile.BlockType>())
            {
                blockDrops.Add(
                    t,
                    Data.JSON_BlockData.blockData.ContainsKey(t) && (Data.JSON_BlockData.blockData[t].loot != null) ? (
                        Data.JSON_BlockData.blockData[t].lootData
                    ) : new Dictionary<Type, int>()
                    );
            }

            
        }

        public static Item Empty()
        {
            return new Item(Type.None, 0);
        }



        public Type type;
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

        public Item(Type t, int a)
        {
            type = t;
            amount = a;

            stackSize = itemStackTypes.ContainsKey(type) ? stackSizes[itemStackTypes[type]] : maxStackSize;
        }

        public void UpdateStackSize()
        {
            stackSize = itemStackTypes.ContainsKey(type) ? stackSizes[itemStackTypes[type]] : maxStackSize;
        }

        public void UpdateSelf()
        {
            UpdateStackSize();
            if (_amount <= 0)
            {
                type = Type.None;
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
    }
}
