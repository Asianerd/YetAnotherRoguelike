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

        public GameValue selectionAge = new GameValue(0, 10, 1);
        public bool selected;

        public Vector2 itemPosition;
        public float rotation;

        public HotbarSection(int _index) : base()
        {
            index = _index;

            rotation = ((index / 4f) * 2 * MathF.PI) - (MathF.PI / 2f);

            float degreeIncrement = 360f / 4f;
            float degreeRotation = ((index < 2 ? index + 2 : index - 2)) * degreeIncrement;

            start = GeneralDependencies.FullDegrees(degreeRotation);
            end = GeneralDependencies.FullDegrees(degreeRotation + degreeIncrement);
            // i hate radians so much

            item = Item.Empty();
        }

        public override void Update()
        {
            selected = GeneralDependencies.inBetween(start, end, GeneralDependencies.FullDegrees(MathHelper.ToDegrees(MathHelper.ToRadians(Hotbar.Instance.alteredDegrees - 45))));
            hovered = selected;
            selectionAge.Regenerate(selected ? Game.compensation : -Game.compensation);

            itemPosition = new Vector2(
                MathF.Cos(rotation),
                MathF.Sin(rotation)
                ) * 110;

            if (selected)
            {
                if (MouseInput.left.active || MouseInput.right.active)
                {
                    Hotbar.selectedSection = index;
                    //Hotbar.Instance.Toggle();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            float p = MathF.Sin(Hotbar.Instance.p * MathF.PI * 0.5f);
            float s = MathF.Sin(selectionAge.Percent() * MathF.PI * 0.5f);
            Color color = p * Color.White;
            spriteBatch.Draw(sprite, Hotbar.Instance.position, null, color, rotation, spriteOrigin, (0.3f * p) + (0.02f * s), SpriteEffects.None, 0f);
            if (item.type != Item.Type.None)
            {
                Vector2 drawnPosition = Hotbar.Instance.position + (itemPosition * Hotbar.Instance.p * ((s * 0.02f) + 1f));
                spriteBatch.Draw(Item.itemSprites[item.type], new Rectangle(
                    drawnPosition.ToPoint(),
                    new Point((int)((64f * p)))
                    ), null, color, 0f, Item.spriteOrigin, SpriteEffects.None, 0f);
                spriteBatch.DrawString(UI.defaultFont, item.amount == 1 ? "" : item.amount.ToString(), drawnPosition, color);
            }
        }
    }
}