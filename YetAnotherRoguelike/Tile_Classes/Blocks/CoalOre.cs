using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Tile_Classes.Blocks
{
    class CoalOre : Tile
    {
        LightSource light;

        public CoalOre(Vector2 pos, Chunk _parent) : base(Type.Coal_ore, pos, _parent)
        {
            light = new LightSource(position, 5, 3, Color.Gray);
            LightSource.sources.Add(light);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (LightSource.sources.Contains(light))
            {
                LightSource.sources.Remove(light);
            }
        }

        public override void HardUnload()
        {
            base.HardUnload();

            if (LightSource.sources.Contains(light))
            {
                LightSource.sources.Remove(light);
            }
        }
    }
}
