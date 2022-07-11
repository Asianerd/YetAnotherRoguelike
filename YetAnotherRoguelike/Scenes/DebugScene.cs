using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class DebugScene : Scene
    {
        public DebugScene() : base(Scenes.Debug)
        {
            backgroundColor = Color.Black;
            Perlin_Noise.Fetch(Vector2.Zero);
        }

        public override void Update(GameTime gameTime)
        {
            Camera.Instance.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
/*            int s = 32;
            int resolution = 512;
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    //spriteBatch.Draw(blank, new Rectangle((x * 32) + 300, (y * 32) + 300, 32, 32), Color.White * Perlin_Noise.map[y][x]);
                    Vector2 position = new Vector2(x, y);
                    Rectangle rect = new Rectangle((position * s).ToPoint(), new Point(s, s));
                    if (!rect.Intersects(Game.playArea))
                    {
                        continue;
                    }
                    spriteBatch.Draw(blank, rect, Color.White * Perlin_Noise.Fetch(position));
                }
            }*/
            //Perlin_Noise.Fetch(Vector2.Zero);

            base.Draw(gameTime);
        }

        public override void OnSceneLoad()
        {
            Camera.Instance.renderOffset = Vector2.Zero;

            base.OnSceneLoad();
        }
    }
}
