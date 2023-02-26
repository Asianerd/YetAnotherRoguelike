using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.PhysicsObject;

namespace YetAnotherRoguelike.UI
{
    class UI_Furnace: UI_Container
    {
        public static UI_Furnace Instance = null;

        public List<UI_ItemSlot> inputs;
        public UI_ItemSlot crucible, fuel;
        public UI_FurnaceProgress furnaceProgress;
        public UI_Background background;

        public UI_Furnace():base(new List<UI_Element>())
        {
            if (Instance == null)
            {
                Instance = this;
                UI_FurnaceProgress.Initialize();
            }

            inputs = new List<UI_ItemSlot>();
            for (int i = 0; i < 3; i++)
            {
                inputs.Add(new UI_ItemSlot(this, new Rectangle(0, 0, UI_ItemSlot.size, UI_ItemSlot.size), Item.Empty()));
            }
            crucible = new UI_ItemSlot(this, new Rectangle(0, 0, UI_ItemSlot.size, UI_ItemSlot.size), Item.Empty());
            fuel = new UI_ItemSlot(this, new Rectangle(0, 0, UI_ItemSlot.size, UI_ItemSlot.size), Item.Empty());

            furnaceProgress = new UI_FurnaceProgress(this, new Rectangle(0, 0, UI_ItemSlot.size, UI_ItemSlot.size));

            background = new UI_Background(this, new Rectangle(0, 0, UI_ItemSlot.size * 5, (int)(UI_ItemSlot.size * 6f)));


            elements.Add(background);
            foreach (UI_ItemSlot x in inputs)
            {
                elements.Add(x);
            }
            elements.Add(crucible);
            elements.Add(fuel);
            elements.Add(furnaceProgress);

            OnScreenResize();
        }

        public override void Update()
        {
            base.Update();

            if (active)
            {
                if (Player.Instance.speed > 0f)
                {
                    active = false;
                }
            }
        }

        public override void OnScreenResize()
        {
            base.OnScreenResize();

            background.rect.X = (int)((Game.screenSize.X * 0.5f) - (background.rect.Width * 0.5f));
            background.rect.Y = (int)((Game.screenSize.Y * 0.5f) - (background.rect.Height * 0.5f));

            for (int i = 0; i < 3; i++)
            {
                inputs[i].rect.X = (int)(background.rect.Center.X + ((i - 1) * UI_ItemSlot.size * 1.5f) - (inputs[i].rect.Size.X / 2));
                inputs[i].rect.Y = (int)(background.rect.Top + (UI_ItemSlot.size * 0.5f));
            }

            crucible.rect.X = background.rect.Center.X - (crucible.rect.Size.X / 2);
            crucible.rect.Y = (int)(background.rect.Top + (UI_ItemSlot.size * 2));
            furnaceProgress.rect.X = background.rect.Center.X - (furnaceProgress.rect.Size.X / 2);
            furnaceProgress.rect.Y = (int)(background.rect.Top + (UI_ItemSlot.size * 3.25f));
            fuel.rect.X = background.rect.Center.X - (fuel.rect.Size.X / 2);
            fuel.rect.Y = (int)(background.rect.Top + (UI_ItemSlot.size * 4.5f));
        }

        public void Activate(string _n, List<Item> _i, Item _c, Item _f, GameValue _p, bool forceOpen = false)
        {
            if (!forceOpen && active)
            {
                active = !active;
                return;
            }

            for (int i = 0; i < inputs.Count; i++)
            {
                inputs[i].item = _i[i];
            }

            crucible.item = _c;
            fuel.item = _f;

            furnaceProgress.progress = _p;

            active = true;

            OnScreenResize();
        }

        public override void Draw()
        {
            if (!active)
            {
                return;
            }
            background.Draw(Game.spriteBatch);
            foreach (UI_ItemSlot x in inputs)
            {
                x.Draw(Game.spriteBatch);
            }
            crucible.Draw(Game.spriteBatch);
            fuel.Draw(Game.spriteBatch);
            furnaceProgress.Draw(Game.spriteBatch);
        }
    }
}
