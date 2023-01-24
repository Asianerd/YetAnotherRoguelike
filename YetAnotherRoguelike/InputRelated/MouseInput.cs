using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherRoguelike
{
    class MouseInput
    {
        public static MouseInput right, left;
        public static float scrollVel; // the current speed of scrolling
        static float previousScroll;

        public static void Initialize()
        {
            right = new MouseInput();
            left = new MouseInput();
        }

        public static void UpdateAll()
        {
            right.Update(Game.mouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);
            left.Update(Game.mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);

            scrollVel = Game.mouseState.ScrollWheelValue - previousScroll;
            previousScroll = Game.mouseState.ScrollWheelValue;
        }

        public bool isPressed, wasPressed, active;

        void Update(bool current)
        {
            wasPressed = isPressed;
            isPressed = current;
            active = isPressed && !wasPressed;
        }
    }
}
