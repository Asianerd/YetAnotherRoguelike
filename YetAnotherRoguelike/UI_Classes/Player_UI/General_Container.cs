using System;
using System.Collections.Generic;
using System.Text;
using YetAnotherRoguelike.Gameplay.ItemStorage;

namespace YetAnotherRoguelike.UI_Classes.Player_UI
{
    class General_Container : UI_Container
    {
        public static General_Container Instance;

        public General_Container(List<UI_Element> _elements) : base(_elements)
        {
            active = true;
            Instance = this;

            elements.Add(new ItemPopupParent());
        }
    }
}
