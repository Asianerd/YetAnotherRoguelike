using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.UI.Inherited_Elements;

namespace YetAnotherRoguelike.UI
{
    class UI_Element
    {
        public static Texture2D blank;
        public static SpriteFont mainFont;
        public static UI_Element hoveredElement;

        public static List<Texture2D> backgroundSprite;
        public static int pixel = 16; // used when drawing backgroundSprite

        public static void Initialize()
        {
            blank = Game.Instance.Content.Load<Texture2D>("blank");
            mainFont = Game.Instance.Content.Load<SpriteFont>("Fonts/defaultFont");
            backgroundSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/background2"), 4, 4);

            UI_ItemSlot.slotSprite = Game.Instance.Content.Load<Texture2D>("UI/Storage_Items/item_slot");
            UI_ItemSlot.selectionSprite = Game.Instance.Content.Load<Texture2D>("UI/Storage_Items/selection_sprite");
        }


        //public PositionType positionType = PositionType.Screen;
        /*public Rectangle baseRect; // if positionType is screen, (0, 0) -> top left, (1, 1) -> bottom right
        public Rectangle displayRect;
        // rect used to draw to screen
        // math is done to convert (0.5, 0.5) -> (960, 540)     (only for screen position type)*/
        public ElementType type;
        public UI_Container parentContainer;
        public Rectangle rect; // css type ui is too complicated, using this for now
        public bool hovered;

        public UI_Element(UI_Container p, Rectangle r, ElementType t = ElementType.Unset)
        {
            type = t;
            rect = r;
            parentContainer = p;
        }

        public virtual void Update()
        {
            Debug.WriteLine("Test");
        }

        public virtual void FetchHovered()
        {
            hovered = false;
            if (rect.Contains(Cursor.position))
            {
                hoveredElement = this;
            }
        }

        public virtual void Draw(SpriteBatch spritebatch, int offsetX = 0, int offsetY = 0)
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

        public enum ElementType
        {
            // not necessary to always assign
            Unset,
            ItemSlot
        }
    }
}
