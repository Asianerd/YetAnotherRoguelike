using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike.Tile_Classes
{
    class InteractableTile : Tile
    {
        bool hovered = false;

        public InteractableTile(Type t, Vector2 pos, Chunk _parent) : base(t, pos, _parent)
        {

        }

        public override void Update()
        {
            base.Update();

            Rectangle displayedRect = new Rectangle(
                (position + Camera.Instance.renderOffset).ToPoint(),
                new Point(tileSize)
                );
            hovered = displayedRect.Contains(Game.mouseState.Position);
        }
    }
}
