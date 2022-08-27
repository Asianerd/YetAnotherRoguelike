using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Tile_Classes.Blocks
{
    class NeonBlock : Tile
    {
        LightSource light;
        public NeonBlock(Vector2 pos, Chunk _parent) : base(Type.Neon, pos, _parent)
        {
            light = new LightSource(position, 50, 20, Color.Aquamarine);
            LightSource.Append(light);
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
