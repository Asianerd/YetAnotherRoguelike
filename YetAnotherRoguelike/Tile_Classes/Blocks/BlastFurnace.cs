using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using YetAnotherRoguelike.Gameplay;
using YetAnotherRoguelike.UI_Classes.Block_UI;

namespace YetAnotherRoguelike.Tile_Classes.Blocks
{
    class BlastFurnace : InteractableTile
    {
        LightSource light;
        public Item input, output, fuel;

        public BlastFurnace(Vector2 pos, Chunk _parent) : base(Type.Blast_Furnace, pos, _parent)
        {
            light = new LightSource(
                pos + (Vector2.One / 2f),
                LightTile.lightTileSources[type].strength,
                LightTile.lightTileSources[type].range,
                LightTile.lightTileSources[type].color
                );
            LightSource.Append(light);

            input = Item.Empty();
            output = Item.Empty();
            fuel = Item.Empty();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LightSource.Remove(light);

            GroundItem.Spawn(input, position, _check:true);
            GroundItem.Spawn(output, position, _check: true);
            GroundItem.Spawn(fuel, position, _check: true);

            input = Item.Empty();
            output = Item.Empty();
            fuel = Item.Empty();
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

        public override void Update()
        {
            base.Update();

            if (MouseInput.right.active)
            {
                if (rect.Contains(Game.mouseState.Position.ToVector2() - Camera.Instance.renderOffset))
                {
                    BlastFurnace_UI.Instance.AssignData(this);
                    BlastFurnace_UI.Instance.Toggle();
                }
            }
        }
    }
}
