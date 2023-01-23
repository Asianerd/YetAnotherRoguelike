using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Graphics;

namespace YetAnotherRoguelike.Tile_Classes
{
    class Tile_BlastFurnace : Tile
    {
        public static int productCount = 6;

        public Item input, fuel;
        public List<Item> products;
        public GameValue smeltingProgress;

        public Tile_BlastFurnace(Point tCoords, Point cCoords, BlockType t, GameValue d = null, LightSource l = null) :base(tCoords, cCoords, t, d:d, l:l)
        {
            input = Item.Empty();
            fuel = Item.Empty();
            products = new List<Item>();
            for (int i = 0; i < productCount; i++)
            {
                products.Add(Item.Empty());
            }

            smeltingProgress = new GameValue(0, 120, 1);
        }

        public override void Update()
        {
            base.Update();


        }

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);
        }
    }
}
