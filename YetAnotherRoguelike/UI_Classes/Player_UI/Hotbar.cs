using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YetAnotherRoguelike.UI_Classes.Player_UI
{
    class Hotbar : UI_Container
    {
        public static Hotbar Instance;
        public static int selectedSection;
        public static int selectedItem;

        public List<HotbarSection> sections;
        public GameValue age = new GameValue(0, 10, 1);
        public float p;
        public float alteredDegrees;
        public float degrees;

        public Vector2 position;

        public Hotbar(List<UI_Element> _elements) : base(_elements)
        {
            Instance = this;

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
            if (active)
            {
                if (Vector2.Distance(position, Game.mouseState.Position.ToVector2()) >= 300)
                {
                    Toggle();
                }
            }

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
        }
    }
}
