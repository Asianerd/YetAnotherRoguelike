using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class ChemicalContainer
    {
        public enum CrucibleType
        {
            Small,
            Medium,
            Large
        }

        public static Dictionary<CrucibleType, double> crucibleSizes; // in liters, ℓ
        public static Dictionary<CrucibleType, Texture2D[]> crucibleSprites;

        public static void Initialize()
        {
            crucibleSizes = new Dictionary<CrucibleType, double>()
            {
                { CrucibleType.Small, 0.5d },
                { CrucibleType.Medium, 2d },
                { CrucibleType.Large,  5d }
            };

            crucibleSprites = new Dictionary<CrucibleType, Texture2D[]>();
            foreach (CrucibleType x in Enum.GetValues(typeof(CrucibleType)).Cast<CrucibleType>())
            {
                crucibleSprites.Add(x, new Texture2D[2]);
                crucibleSprites[x][0] = Game.Instance.Content.Load<Texture2D>($"Item_sprites/Crucible/{x}");
                crucibleSprites[x][1] = Game.Instance.Content.Load<Texture2D>($"Item_sprites/Crucible/{x}_filled");
            }
        }

        public CrucibleType type;

        public ChemicalContainer(CrucibleType t = CrucibleType.Medium) : base()
        {
            type = t;
        }

        public double Size()
        {
            return crucibleSizes[type];
        }
    }
}
