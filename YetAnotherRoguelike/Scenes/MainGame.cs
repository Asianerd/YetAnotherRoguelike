using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using YetAnotherRoguelike.Gameplay;
using YetAnotherRoguelike.Gameplay.ItemStorage;
using YetAnotherRoguelike.UI_Classes.Player_UI;

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
            GroundItem.Initialize();

            worldBorder = (Perlin_Noise.size * Chunk.realSize) * 0.4f;
            var _ = new Player();
            Inventory __ = new Inventory(new List<UI_Element>());
            General_Container ___ = new General_Container(new List<UI_Element>());


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
            Inventory.Instance.UpdateAll();
            General_Container.Instance.UpdateAll();
            GroundItem.UpdateAll();
            Particle.UpdateAll();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Map.Draw(spriteBatch);
            Particle.DrawAll(spriteBatch);
            Player.Instance.Draw(spriteBatch);
            GroundItem.DrawAll(spriteBatch);
            base.Draw(gameTime);
        }

        public override void DrawUI(GameTime gameTime)
        {
            General_Container.Instance.DrawAll(spriteBatch, Point.Zero);
            Inventory.Instance.DrawAll(spriteBatch, Point.Zero);
            base.DrawUI(gameTime);
        }

        public override void OnSceneLoad()
        {
            Camera.Instance.position = Camera.Instance.target;
        }
    }
}
