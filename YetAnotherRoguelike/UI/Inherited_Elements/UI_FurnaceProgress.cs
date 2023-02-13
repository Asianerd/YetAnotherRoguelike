using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI
{
    class UI_FurnaceProgress : UI_Element
    {
        public static Texture2D sprite, progressSprite;

        public GameValue progress;

        public static new void Initialize()
        {
            sprite = Game.Instance.Content.Load<Texture2D>("UI/Furnace/FurnaceProgress");
            progressSprite = Game.Instance.Content.Load<Texture2D>("UI/Furnace/FurnaceProgressBar");
        }

        public UI_FurnaceProgress(UI_Container p, Rectangle r):base(p, r)
        {

        }

        public override void Draw(SpriteBatch spritebatch, int offsetX = 0, int offsetY = 0)
        {
            float layer = 0.21f;
            spritebatch.Draw(sprite, rect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layer);
            if (progress != null)
            {
                spritebatch.Draw(progressSprite, new Rectangle(
                rect.X, rect.Y + (int)(rect.Height * (1f - progress.Percent())), rect.Width, (int)(rect.Height * progress.Percent())
                ), new Rectangle(
                0, (int)(progressSprite.Height * (1f - progress.Percent())), progressSprite.Width, (int)(progressSprite.Height * progress.Percent())),
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, layer + 0.01f);
            }
        }
    }
}
