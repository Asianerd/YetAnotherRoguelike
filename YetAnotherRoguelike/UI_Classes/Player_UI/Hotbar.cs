using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YetAnotherRoguelike.Gameplay;

namespace YetAnotherRoguelike.UI_Classes.Player_UI
{
    class Hotbar : UI_Container
    {
        public static Hotbar Instance;
        public static int selectedSection = 0;
        public static Item selectedItem;

        public static Texture2D selectionSprite;
        public static Vector2 selectionSpriteOrigin;

        public List<HotbarSection> sections;
        public GameValue age = new GameValue(0, 10, 1);
        public float p;
        public float alteredDegrees;
        public float degrees;

        public Vector2 position;

        float i = 0f;

        public Hotbar(List<UI_Element> _elements) : base(_elements)
        {
            Instance = this;
            selectionSprite = Game.Instance.Content.Load<Texture2D>("UI/Hotbar/selection_indicator");
            selectionSpriteOrigin = selectionSprite.Bounds.Size.ToVector2() / 2f;

            HotbarSection.Initialize();

            sections = new List<HotbarSection>();
            for (int i = 0; i < 4; i++)
            {
                sections.Add(new HotbarSection(i));
                elements.Add(sections.Last());
            }

            elements.Add(new HotbarPointer());
        }

        public override void Toggle()
        {
            base.Toggle();
            if (active)
            {
                position = Game.mouseState.Position.ToVector2();
            }
        }

        public override void UpdateAll()
        {
            selectedItem = sections[selectedSection].item;

            if (active)
            {
                Cursor.state = Cursor.CursorStates.Hidden;
                if (Vector2.Distance(position, Game.mouseState.Position.ToVector2()) >= 300)
                {
                    Toggle();
                }
            }

            float target = ((selectedSection / 4f) * MathF.PI * 2f) - (MathF.PI / 2f);

            i = GameValue.Lerp(i, target, 0.2f);
            age.Regenerate(active ? Game.compensation : -Game.compensation);
            p = age.Percent();
            if (p < 0.1f)
            {
                return;
            }

            alteredDegrees = GeneralDependencies.FullDegrees(MathHelper.ToDegrees(MathF.Atan2(
                position.Y - Game.mouseState.Position.Y,
                position.X - Game.mouseState.Position.X
                )) - 180);
            degrees = MathF.Atan2(
                Game.mouseState.Position.Y - position.Y,
                Game.mouseState.Position.X - position.X
                );

            base.UpdateAll();
        }

        public override void DrawAll(SpriteBatch spriteBatch, Point offset)
        {
            if (age.Percent() < 0.1f)
            {
                return;
            }

            base.DrawAll(spriteBatch, offset);

            float p = MathF.Sin(Instance.p * MathF.PI * 0.5f);
            //float s = MathF.Sin(selectionAge.Percent() * MathF.PI * 0.5f);
            Color color = p * Color.White;

            spriteBatch.Draw(selectionSprite, position, null, color, i, selectionSpriteOrigin, 0.26f * p, SpriteEffects.None, 0f);
        }
    }
}
