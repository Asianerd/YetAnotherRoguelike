using System;

namespace YetAnotherRoguelike
{
    class GameValue
    {
        public double I;
        public double Min;
        public double Max;
        public double Regeneration;
        public float rate;

        public bool repeat;
        //  |---------I----------|    |---I----------------|    |----------------I----|
        // Min                  Max  Min                  Max  Min                  Max


        public GameValue(double _min, double _max, double _regeneration, double _iPercent = 100.0, bool _repeat = false)
        {
            Min = _min;
            Max = _max;
            Regeneration = _regeneration;
            I = (_max - _min) * (_iPercent / 100);
            repeat = _repeat;

            rate = (float)(Max / Regeneration) / 60f;
        }

        public void Regenerate(double _multiplier = 1, bool clamp = true)
        {
            I += Regeneration * _multiplier;
            if (clamp)
            {
                if (repeat)
                {
                    if (I > Max)
                    {
                        I = Min;
                        return;
                    }
                }
                I = Math.Clamp(I, Min, Max);
            }
        }

        public void AffectValue(double _amount, bool _limit = true)
        {
            I += _amount;
            if (_limit)
            {
                I = Math.Clamp(I, Min, Max);
            }
        }

        public void AffectValue(float _percent, bool _limit = true)
        {
            I = (Max - Min) * _percent; // Percent = 0f to 1f
        }

        public float Percent()
        {
            return (float)(I / (Max - Min));
        }

        public double PercentToValue(float _percent)
        {
            return (Max - Min) * _percent;
        }


        public void AffectMax(double _amount)
        {
            Max += _amount;
        }

        public void AffectMax(float _percent)
        {
            Max *= _percent; // Percent = 0f to 1f
        }


        public void AffectMin(double _amount)
        {
            Min += _amount;
        }

        public void AffectMin(float _percent)
        {
            Min *= _percent; // Percent = 0f to 1f
        }

        #region Utility functions
        public static float Lerp(float start, float end, float amount)
        {
            return start + ((end - start) * amount);
        }
        #endregion
    }
}
