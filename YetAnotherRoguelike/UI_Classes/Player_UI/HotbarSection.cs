using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using YetAnotherRoguelike.Gameplay;

namespace YetAnotherRoguelike.UI_Classes.Player_UI
{
    class HotbarSection : UI_Element
    {
        public static Texture2D sprite;
        public static Vector2 spriteOrigin;

        public static void Initialize()
        {
            sprite = Game.Instance.Content.Load<Texture2D>("UI/Hotbar/Hotbar_section");
            spriteOrigin = sprite.Bounds.Size.ToVector2() / 2f;
        }


        public int index;
        public Item item;

        public float start, end;

        public GameValue selectionAge = new GameValue(0, 15, 1);

        public HotbarSection(int _index) : base()
        {
            index = _index;

            float degreeIncrement = 360f / 4f;
            float degreeRotation = ((index < 2 ? index + 2 : index - 2)) * degreeIncrement;

            start = GeneralDependencies.FullDegrees(degreeRotation);
            end = GeneralDependencies.FullDegrees(degreeRotation + degreeIncrement);
            // i hate radians so much

            item = Item.Empty();
        }

        public override void Update()
        {
            selectionAge.Regenerate(
                GeneralDependencies.inBetween(start, end, GeneralDependencies.FullDegrees(MathHelper.ToDegrees(MathHelper.ToRadians(Hotbar.Instance.alteredDegrees - 45)))) ?
                Game.compensation : -Game.compensation
                );
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            spriteBatch.Draw(sprite, Hotbar.Instance.position, null, Color.White * MathF.Sin(Hotbar.Instance.p * MathF.PI * 0.5f), (index / 4f) * 2 * MathF.PI, spriteOrigin, (0.3f * MathF.Sin(Hotbar.Instance.p * MathF.PI * 0.5f)) + (MathF.Sin(selectionAge.Percent() * MathF.PI * 0.5f) * 0.02f), SpriteEffects.None, 0f);
        }
    }
}