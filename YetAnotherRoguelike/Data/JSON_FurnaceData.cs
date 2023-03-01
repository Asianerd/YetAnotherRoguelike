﻿using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace YetAnotherRoguelike.Data
{
    class JSON_FurnaceData
    {
        public static List<Recipe> recipes;
        public static Dictionary<Item.Type, List<int>> fuelData;

        static List<JSON_FurnaceData> furnaceData;

        public static void Initialize()
        {
            recipes = new List<Recipe>();
            fuelData = new Dictionary<Item.Type, List<int>>();

            furnaceData = new List<JSON_FurnaceData>();
            ParentFurnaceJSON parent = JsonSerializer.Deserialize<ParentFurnaceJSON>(File.ReadAllText("Data/furnace_data.json"));
            parent.AssignData();

            // data system done, now need to implement it into the actual furnace
        }

        public class Recipe
        {
            public List<Item> inputs;
            public Chemical output;
            public int temperature;

            public Recipe(List<Item> _i, Chemical _o, int _t)
            {
                inputs = _i;
                output = _o;
                temperature = _t;
            }
        }

        class ParentFurnaceJSON
        {
            public ParentFurnaceJSON() { }

            public Dictionary<string, List<int>> fuel { get; set; }
            public List<ChildFurnaceJSON> loot_table { get; set; }


            public void AssignData()
            {
                fuelData = new Dictionary<Item.Type, List<int>>();

                foreach (KeyValuePair<string, List<int>> x in fuel)
                {
                    fuelData.Add(
                        Enum.GetValues(typeof(Item.Type)).Cast<Item.Type>().Where(n => n.ToString() == x.Key).First(),
                        x.Value
                        );
                }

                foreach (ChildFurnaceJSON x in loot_table)
                {
                    x.AssignData();
                }
            }
        }

        class ChildFurnaceJSON
        {
            public ChildFurnaceJSON() { }

            public void AssignData()
            {
                Chemical outputs = new Chemical(new Dictionary<Chemical.Element, double>(), ChemicalContainer.CrucibleType.Infinite);

                foreach (KeyValuePair<string, double> x in output)
                {
                    outputs.composition.Add(
                        Enum.GetValues(typeof(Chemical.Element)).Cast<Chemical.Element>().Where(n => n.ToString() == x.Key).First(),
                        x.Value
                        );
                }

                recipes.Add(new Recipe(
                    inputs.Select(n => new Item(
                        Enum.GetValues(typeof(Item.Type)).Cast<Item.Type>().Where(i => i.ToString() == n.Key).First(),
                        n.Value
                        )).ToList(),
                    outputs,
                    temperature
                    ));
            }

            public Dictionary<string, int> inputs { get; set; }
            public Dictionary<string, double> output { get; set; }
            public int temperature { get; set; }
        }
    }
}
