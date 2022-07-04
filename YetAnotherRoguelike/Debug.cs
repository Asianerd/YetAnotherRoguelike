using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace YetAnotherRoguelike
{
    public static class Debug
    {
        public static void WriteLine(object obj)
        {
            System.Diagnostics.Debug.WriteLine(obj);
        }
    }
}
