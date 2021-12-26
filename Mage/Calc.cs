using System;
using Microsoft.Xna.Framework;

namespace Mage
{
    public static class Calc
    {
        public static float Approach(float value, float target, float change)
        {
            return value < target ? Math.Min(value + change, target) : Math.Max(value - change, target);
        }
    }
}