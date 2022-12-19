using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using YetAnotherRoguelike.Gameplay;

namespace YetAnotherRoguelike.Tile_Classes
{
    class LightTile : Tile
    {
        public static Dictionary<Type, LightSource> lightTileSources;

        public LightSource light;

        public LightTile(Type t, Vector2 pos, Chunk p) : base(t, pos, p)
        {
            light = new LightSource(
                pos + (Vector2.One / 2f),
                lightTileSources[t].strength,
                lightTileSources[t].range,
                lightTileSources[t].color
                );
            LightSource.Append(light);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LightSource.Remove(light);
        }

        public override void SoftUnload()
        {
            base.SoftUnload();
            LightSource.Remove(light);
        }

        public override void HardUnload()
        {
            base.HardUnload();
            LightSource.Remove(light);
        }

        public override void Reload()
        {
            base.Reload();
            LightSource.Append(light);
        }
    }
}
