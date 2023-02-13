using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Graphics;

namespace YetAnotherRoguelike.Particles
{
    class Fire : Particle
    {
        float scale, speed;
        LightSource light;

        public Fire(Vector2 p) : base(p, new GameValue(0, 40, Game.random.Next(600, 1400) / 1000f, 0))
        {
            scale = Game.random.Next(500, 1500) / 1000f;
            speed = 0.02f * (1.5f - scale);

            light = new LightSource(Vector2.Zero, Color.Orange, 1f, 1f);
            LightSource.Append(light);
        }

        public override void Update()
        {
            base.Update();

            position.Y -= Game.compensation * speed;
            light.position = position;

            light.range = (1f - age.Percent());
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);

            spritebatch.Draw(blank, position * Tile_Classes.Tile.tileSize, null, Color.Orange * (1f - age.Percent()), 0f, blankOrigin, 5f * scale, SpriteEffects.None, Camera.GetDrawnLayer(position.Y * Tile_Classes.Tile.tileSize));
        }

        public override void OnDeath()
        {
            base.OnDeath();

            LightSource.Remove(light);
        }
    }
}
