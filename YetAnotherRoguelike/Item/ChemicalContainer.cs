using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class ChemicalContainer
    {
        public enum CrucibleType
        {
            Small,
            Medium,
            Large,

            Infinite
        }

        public static Dictionary<CrucibleType, double> crucibleSizes; // in liters, ℓ
        public static Dictionary<CrucibleType, Texture2D[]> crucibleSprites;
        public static Dictionary<CrucibleType, Point> crucibleBarSizes; // item tooltip sizes for crucibles

        public static void Initialize()
        {
            crucibleSizes = new Dictionary<CrucibleType, double>()
            {
                { CrucibleType.Small, 0.5d },
                { CrucibleType.Medium, 2d },
                { CrucibleType.Large,  5d }
            };
            crucibleSizes[CrucibleType.Infinite] = double.MaxValue;

            crucibleSprites = new Dictionary<CrucibleType, Texture2D[]>();
            foreach (CrucibleType x in Enum.GetValues(typeof(CrucibleType)).Cast<CrucibleType>())
            {
                crucibleSprites.Add(x, new Texture2D[2]);
                crucibleSprites[x][0] = Game.Instance.Content.Load<Texture2D>($"Item_sprites/Crucible/{x}");
                crucibleSprites[x][1] = Game.Instance.Content.Load<Texture2D>($"Item_sprites/Crucible/{x}_filled");
            }

            crucibleBarSizes = new Dictionary<CrucibleType, Point>()
            {
                { CrucibleType.Small, new Point(1, 3) },
                { CrucibleType.Medium, new Point(2, 4) },
                { CrucibleType.Large,  new Point(3, 5) },
                { CrucibleType.Infinite,  new Point(3, 5) }
            };
        }

        public CrucibleType type;
        public string spaces; // used for the chemical tostring

        public ChemicalContainer(CrucibleType t = CrucibleType.Medium) : base()
        {
            type = t;
            spaces = new string(' ', crucibleBarSizes[type].X + 2);
        }

        public double Size()
        {
            return crucibleSizes[type];
        }
    }
}
