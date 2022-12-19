using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike.UI_Classes.Block_UI
{
    class InteractablesUI_Monolith
    {
        public static List<UI_Container> containers;

        public static void Initialize()
        {
            containers = new List<UI_Container>();
            containers.Add(new BlastFurnace_UI());
        }

        public static void DisableAll()
        {
            foreach (UI_Container x in containers)
            {
                // maybe add a check if the ui is important/should be kept on screen
                x.active = false;
            }
        }

        public static void UpdateAll()
        {
            if (Player.Instance.entitySpeed >= 1f)
            {
                DisableAll();
            }

            foreach(UI_Container x in containers)
            {
                x.UpdateAll();
            }
        }

        public static void DrawAll(SpriteBatch spriteBatch, Point offset)
        {
            foreach(UI_Container x in containers)
            {
                x.DrawAll(spriteBatch, offset);
            }
        }
    }
}
