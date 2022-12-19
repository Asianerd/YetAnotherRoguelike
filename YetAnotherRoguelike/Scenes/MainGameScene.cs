using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.Scenes
{
    class MainGameScene:Scene
    {
        public MainGameScene():base(SceneTypes.MainGame, Color.Black)
        {
            for (int y = -5; y < 5; y++)
            {
                for (int x = -5; x < 5; x++)
                {
                    Chunk.chunks.Add(new Chunk(new Point(x, y)));
                }
            }
        }

        public override void Update()
        {
            base.Update();

            foreach (Chunk x in Chunk.chunks)
            {
                x.Update();
            }
        }

        public override void Draw()
        {
            base.Draw();

            foreach (Chunk x in Chunk.chunks)
            {
                x.Draw(Game.spriteBatch);
            }
        }
    }
}
