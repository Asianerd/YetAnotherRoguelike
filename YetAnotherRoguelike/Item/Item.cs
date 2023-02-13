using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Data;

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
            foreach (Type x in Enum.GetValues(typeof(Type)).Cast<Type>())
            {
                itemStackTypes.Add(x, StackType.Regular);
            }

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

        public static Item DeepCopy(Item x)
        {
            return new Item(x.type, x.amount);
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
        public Species selectionType = Species.Unset;

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

        public void UpdateSelectionType()
        {
            selectionType = Species.Unset;
            JSON_ItemData result = JSON_ItemData.FetchData(type);
            if (result != null)
            {
                if (result.selectionType != Species.Unset)
                {
                    selectionType = result.selectionType;
                }
                else if (result.tilePlaced != Tile_Classes.Tile.BlockType.Air)
                {
                    selectionType = Species.Placeable;
                }
            }
        }

        public void UpdateSelf()
        {
            UpdateStackSize();
            if (_amount <= 0)
            {
                type = Type.None;
                _amount = 0;
            }
            UpdateSelectionType();
        }

        public bool Full(bool over = false)
        {
            UpdateStackSize(); // just in case
            if (over)
            {
                return amount > stackSize;
            }
            else
            {
                return amount >= stackSize;
            }
        }

        public void AssignData(Item x)
        {
            type = x.type;
            amount = x.amount;

            UpdateSelf();
        }

        public bool IsSame(Item x)
        {
            return (type == x.type) && (amount == x.amount);
        }


        public enum Species
        {
            Unset,
            Placeable,
            Tool, // used to mine blocks
            Weapon, // attacking
        }

        public enum Type
        {
            None,

            Stone,

            Clay,
                Clay_Cast,

            Coal,
            Bauxite,
            Hematite,
            Sphalerite,
            Calamine,
            Galena,
            Cinnabar,
            Argentite,
            Bismuth,

            Rudimentary_Furnace,
        }

        public enum StackType
        {
            Regular,
            Single
        }
    }
}
