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
        public NeonBlock(Vector2 pos, Chunk _parent, Type t) : base(t, pos, _parent)
        {
            light = new LightSource(position, 50, 10, t switch {
                Type.Neon_Blue => new Color(82, 241, 242),
                Type.Neon_Pink => new Color(255, 0, 244),
                Type.Neon_Yellow => Color.YellowGreen,
                _ => Color.Aquamarine
            });
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
