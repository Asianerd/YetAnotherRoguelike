using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike
{
    class MainGame : Scene
    {
        public MainGame() : base(Scenes.MainGame)
        {
            Tile.Initialize();
            Map.Initialize();
            var x = new Player();
        }

        public override void Update(GameTime gameTime)
        {
            Camera.Instance.Update();
            Map.Update();
            Player.Instance.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Map.Draw(spriteBatch);
            Player.Instance.Draw(spriteBatch);
            base.Draw(gameTime);
        }

        public override void OnSceneLoad()
        {
            Camera.Instance.position = Camera.Instance.target;
        }
    }
}
