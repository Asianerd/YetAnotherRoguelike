using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Chemical : ItemData
    {
        public enum Element
        {
            Slag,
            Al,
            Fe,
            Zn,
            Cu,
            Pb,
            Hg,
            Ag,
            Bi,
        }

        #region Statics
        public static Dictionary<Element, Color> elementColors;

        public static void Initialize()
        {
            elementColors = new Dictionary<Element, Color>()
            {
                { Element.Slag, GeneralDependencies.HexToColor("3e4256") },
                { Element.Al, GeneralDependencies.HexToColor("b3b4b7") },
                { Element.Fe, GeneralDependencies.HexToColor("d12c2c") },
                { Element.Zn, GeneralDependencies.HexToColor("92898a") },
                { Element.Cu, GeneralDependencies.HexToColor("e68049") },
                { Element.Pb, GeneralDependencies.HexToColor("5c6274") },
                { Element.Hg, GeneralDependencies.HexToColor("e8e8e8") },
                { Element.Ag, GeneralDependencies.HexToColor("d8d8d8") },
                { Element.Bi, GeneralDependencies.HexToColor("75e9e5") }
            };
        }

        public static Chemical Fit(Chemical subject, Chemical add)
        {
            //double difference = Math.Abs((add.Total() + subject.Total()) - subject.Total());
            double overflow = (subject.Total() + add.Total()) - subject.container.Size();
            double rate = (subject.container.Size() - subject.Total()) / add.Total();
            if (overflow <= 0)
            {
                return subject;
            }

            foreach (KeyValuePair<Element, double> x in add.composition)
            {
                subject.AddElement(x.Key, x.Value * rate);
            }

            return subject;
        }
        #endregion

        #region Instance
        public Dictionary<Element, double> composition; // sum of all values must be less than container.Size()
        public ChemicalContainer container;

        public Chemical(Dictionary<Element, double> _composition):base()
        {
            composition = _composition;
            container = new ChemicalContainer();
        }

        public bool IsFull()
        {
            return Total() >= container.Size();
        }

        public double Total()
        {
            double total = 0;
            foreach (double x in composition.Values)
            {
                total += x;
            }
            return total;
        }

        public bool Empty()
        {
            return Total() <= 0;
        }

        public void AddElement(Element e, double a)
        {
            if (!composition.ContainsKey(e))
            {
                composition.Add(e, 0);
            }
            composition[e] += a;
        }

        public void Add(Chemical c)
        {
            if ((Total() + c.Total()) <= container.Size())
            {
                foreach(KeyValuePair<Element, double> x in c.composition)
                {
                    AddElement(x.Key, x.Value);
                }
            }
            else
            {
                composition = Fit(this, c).composition;
            }
        }

        public override string ToString()
        {
            //string final = $"{Total()}%\n";
            string final = $"";
            foreach (KeyValuePair<Element, double> x in composition)
            {
                final += $"   {x.Key} {(x.Value > 1 ? Math.Round(x.Value, 3) : (int)(x.Value * 1000f))}{(x.Value > 1 ? "ℓ" : "mℓ")}\n";
            }
            return final;
        }

        public Texture2D FetchSprite()
        {
            return ChemicalContainer.crucibleSprites[container.type][Total() > 0 ? 1 : 0];
        }
        #endregion
    }
}
