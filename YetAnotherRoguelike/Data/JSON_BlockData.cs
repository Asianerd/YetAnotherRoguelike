using System;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using YetAnotherRoguelike.Tile_Classes;
using Microsoft.Xna.Framework;

namespace YetAnotherRoguelike.Data
{
    class JSON_BlockData
    {
        public static Dictionary<Tile.BlockType, JSON_BlockData> blockData;

        public static void Initialize()
        {
            blockData = new Dictionary<Tile.BlockType, JSON_BlockData>();

            foreach (KeyValuePair<string, JSON_BlockData> item in JsonSerializer.Deserialize<Dictionary<string, JSON_BlockData>>(File.ReadAllText("Data/block_data.json")))
            {
                var x = item.Value;
                x.blockType = Enum.GetValues(typeof(Tile.BlockType)).Cast<Tile.BlockType>().ToList().Where(n => n.ToString() == item.Key).First();
                x.SetData();
                blockData.Add(x.blockType, x);
            }
        }

        public Dictionary<string, int> loot { get; set; }
        public float[] light { get; set; }
        public int[] break_color { get; set; }


        public Tile.BlockType blockType;
        public Dictionary<Item.Type, int> lootData;
        public Color breakParticleColor;
        public Color lightColor;
        public float lightStrength, lightRange;

        public JSON_BlockData() { }

        public void SetData()
        {
            if (break_color != null)
            {
                breakParticleColor = new Color(break_color[0], break_color[1], break_color[2], break_color[3]);
            }
            if (light != null)
            {
                lightColor = new Color((int)light[0], (int)light[1], (int)light[2]);
                lightStrength = light[3];
                lightRange = light[4];
            }
            if (loot != null)
            {
                lootData = new Dictionary<Item.Type, int>();
                foreach (KeyValuePair<string, int> x in loot)
                {
                    lootData.Add(
                        Enum.GetValues(typeof(Item.Type)).Cast<Item.Type>().Where(n => n.ToString() == x.Key).First(),
                        x.Value
                        );
                }
            }
        }
    }
}
