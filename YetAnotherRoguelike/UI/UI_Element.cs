using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike.UI
{
    class UI_Element
    {
        public static Texture2D blank;
        public static List<Texture2D> backgroundSprite;
        public static int pixel = 16; // used when drawing backgroundSprite

        public static void Initialize()
        {
            blank = Game.Instance.Content.Load<Texture2D>("blank");
            backgroundSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/background2"), 4, 4);
        }


        //public PositionType positionType = PositionType.Screen;
        /*public Rectangle baseRect; // if positionType is screen, (0, 0) -> top left, (1, 1) -> bottom right
        public Rectangle displayRect;
        // rect used to draw to screen
        // math is done to convert (0.5, 0.5) -> (960, 540)     (only for screen position type)*/
        public Rectangle rect; // css type ui is too complicated, using this for now

        public UI_Element(Rectangle r)
        {
            rect = r;
        }

        public virtual void Update()
        {

        }

        public virtual void Draw(SpriteBatch spritebatch)
        {

        }

        public virtual void OnScreenResize()
        {

        }/*

        public enum PositionType
        {
            Screen,
            Relative,
            Absolute
        }*/
    }
}
