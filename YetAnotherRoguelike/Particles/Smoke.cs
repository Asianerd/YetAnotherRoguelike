using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.Particles
{
    class Smoke : Particle
    {
        public Smoke(Vector2 pos) : base(Type.Smoke, pos, new GameValue(0, 120, 1, 0))
        {

        }

        public override void Update()
        {
            base.Update();

            position.Y -= 1f;
            position.X += Game.random.Next(-100, 100) / 100f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(blank, position, null, Color.White * (1f - age.Percent()), 0f, Vector2.Zero, 5f, SpriteEffects.None, 0f);
        }
    }
}
