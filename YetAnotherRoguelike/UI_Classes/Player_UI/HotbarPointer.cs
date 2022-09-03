using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike.UI_Classes.Player_UI
{
    class HotbarPointer : UI_Element
    {
        public static Texture2D sprite;
        public static Vector2 spriteOrigin;
        public static HotbarPointer Instance;

        public HotbarPointer() : base()
        {
            Instance = this;
            sprite = Game.Instance.Content.Load<Texture2D>("UI/Hotbar/Pointer");
            spriteOrigin = sprite.Bounds.Size.ToVector2() / 2f;
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            Vector2 target = (new Vector2(
                MathF.Cos(Hotbar.Instance.degrees),
                MathF.Sin(Hotbar.Instance.degrees)
                ) * 60 * Hotbar.Instance.p) + Hotbar.Instance.position;
            spriteBatch.Draw(sprite, target, null, Color.White * Hotbar.Instance.p, Hotbar.Instance.degrees, spriteOrigin, 0.3f * Hotbar.Instance.p, SpriteEffects.None, 0f);
        }
    }
}
