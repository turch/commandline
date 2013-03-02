using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Extensions
{
    static class NumbersExtensions
    {
        public static double AsFractional(this float value, int digits = 4)
        {
            return Math.Round(value - 1 / value, digits);
        }

        public static double AsFractional(this double value, int digits = 4)
        {
            return Math.Round(value - 1 / value, digits);
        }

        public static double AsRounded(this float value, int digits = 4)
        {
            return Math.Round(value, digits);
        }

        public static double AsRounded(this double value, int digits = 4)
        {
            return Math.Round(value, digits);
        }
    }
}
