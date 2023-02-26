using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YetAnotherRoguelike
{
    class Chemical
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
        /* long story short, items cant be inherited, current itemslot system will break
         * so items will have a data field, just like minecraft NBT tags
         * for crucible items, their tag will be an int pointing to the key in the collection of chemicals
         * so to access the chemical of a crucible, itll go something like
         *      int key = crucible.nbt["chemical_key"];
         *      Chemical chem = Chemical.collection[key];
         */
        public static Dictionary<int, Chemical> collection = new Dictionary<int, Chemical>();
        // potential memory leak :flushed:

        public static int RegisterNewChemical(Dictionary<Element, double> c)
        {
            return (new Chemical(c).address);
        }
        #endregion

        #region Instance

        public int address;
        public Dictionary<Element, double> composition;
        // allows a sum of 100.0 values

        public Chemical(Dictionary<Element, double> _composition)
        {
            composition = _composition;

            int current = -1;
            bool found = false;
            foreach (int x in collection.Keys)
            {
                current++;

                if (current != x)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                current++;
            }
            collection.Add(current, this);
            address = current;
        }

        public bool IsFull()
        {
            return Total() >= 100;
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
            if ((Total() + c.Total()) <= 100)
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

        public static Chemical Fit(Chemical subject, Chemical add)
        {
            //double difference = Math.Abs((add.Total() + subject.Total()) - subject.Total());
            double overflow = (subject.Total() + add.Total()) - 100;
            double rate = (100 - subject.Total()) / add.Total();
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

        public override string ToString()
        {
            string final = $"Capacity : {Total()}%\n";
            foreach (KeyValuePair<Element, double> x in composition)
            {
                final += $" {x.Key} : {Math.Round(x.Value, 2)}%\n";
            }
            return final;
        }
        #endregion
    }
}
