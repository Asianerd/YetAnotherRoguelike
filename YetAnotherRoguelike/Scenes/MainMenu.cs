using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike
{
    class MainMenu : Scene
    {
        string titleString = "Yet Another Roguelike";
        GameValue titleAge = new GameValue(0, 300, 1);

        UI_Container container;

        public MainMenu() : base(Scenes.MainMenu)
        {
            container = new UI_Container(new List<UI_Element> {
                new UI_Button(new Rectangle((int)(Game.screenSize.X / 2f), (int)(Game.screenSize.Y * 0.7f), 400, 200), () => { ChangeScene(Scenes.MainGame); }, "Play game")
            });
        }

        public override void Update(GameTime gameTime)
        {
            titleAge.Regenerate(Game.compensation);
            if (titleAge.Percent() >= 1)
            {
                titleAge.AffectValue(0f);
            }

            container.UpdateAll();

            base.Update(gameTime);
        }

        public override void DrawUI(GameTime gameTime)
        {
            spriteBatch.DrawString(defaultFont, titleString, Game.screenSize / 2, Color.White, 0.02f * (float)Math.Cos(titleAge.Percent() * 2 * Math.PI), defaultFont.MeasureString(titleString) / 2, 2.5f + (0.05f * (float)Math.Sin(titleAge.Percent() * 2 * Math.PI)), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);

            container.DrawAll(spriteBatch, Point.Zero);

            base.Draw(gameTime);
        }

        public override void OnSceneLoad()
        {
            Camera.Instance.renderOffset = Vector2.Zero;

            base.OnSceneLoad();
        }
    }
}
