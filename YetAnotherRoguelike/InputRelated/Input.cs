using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace YetAnotherRoguelike
{
    class Input
    {
        public static Dictionary<Keys, Input> collection = new Dictionary<Keys, Input>();

        public static void Initialize(List<Keys> keys)
        {
            foreach(Keys x in keys)
            {
                collection.Add(x, new Input(x));
            }
        }

        public static void UpdateAll(KeyboardState keyboardState)
        {
            foreach(Input x in collection.Values)
            {
                x.Update(keyboardState);
            }
        }

        Keys key;
        public bool isPressed, wasPressed, active;

        public Input(Keys _key)
        {
            key = _key;
        }

        public void Update(KeyboardState keyboardState)
        {
            wasPressed = isPressed;
            isPressed = keyboardState.IsKeyDown(key);

            active = isPressed && !wasPressed;
        }
    }
}
