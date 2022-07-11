using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using YetAnotherRoguelike.Gameplay;

namespace YetAnotherRoguelike
{
    class MainGame : Scene
    {
        public static float worldBorder;

        public MainGame() : base(Scenes.MainGame)
        {
            Tile.Initialize();
            Map.Initialize();
            Particle.Initialize();
            Item.Initialize();

            worldBorder = (Perlin_Noise.size * Chunk.realSize) * 0.4f;
            var x = new Player();

            //backgroundColor = new Color(44, 173, 24);
            //backgroundColor = Color.Green * 1.5f;
            //backgroundColor = new Color(58, 58, 66);
            backgroundColor = Color.Black;
        }

        public override void Update(GameTime gameTime)
        {
            Camera.Instance.Update();
            Map.Update();
            Player.Instance.Update();
            Player.Instance.position = new Vector2(
                Math.Clamp(Player.Instance.position.X, -worldBorder, worldBorder),
                Math.Clamp(Player.Instance.position.Y, -worldBorder, worldBorder)
                );
            Item.UpdateAll();
            Particle.UpdateAll();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Map.Draw(spriteBatch);
            Item.DrawAll(spriteBatch);
            Particle.DrawAll(spriteBatch);
            Player.Instance.Draw(spriteBatch);
            base.Draw(gameTime);
        }

        public override void OnSceneLoad()
        {
            Camera.Instance.position = Camera.Instance.target;
        }
    }
}
