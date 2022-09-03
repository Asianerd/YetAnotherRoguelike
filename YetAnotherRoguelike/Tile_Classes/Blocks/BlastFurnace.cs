using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace YetAnotherRoguelike.Tile_Classes.Blocks
{
    class BlastFurnace : Tile
    {
        public BlastFurnace(Vector2 pos, Chunk _parent) : base(Type.Blast_Furnace, pos, _parent)
        {

        }
    }
}
