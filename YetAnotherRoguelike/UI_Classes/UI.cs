﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class UI
    {
        public static SpriteFont defaultFont;
        public static Texture2D blank;

        public static void Initialize()
        {
            defaultFont = Game.Instance.Content.Load<SpriteFont>("Fonts/defaultFont");
            blank = Game.Instance.Content.Load<Texture2D>("blank");

            Cursor.Initialize();
            UI_Button.Initialize();
        }
    }
}
