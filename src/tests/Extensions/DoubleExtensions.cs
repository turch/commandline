using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Extensions
{
    static class DoubleExtensions
    {
        public static double WithFractionalDigits(this double value, int digits = 4)
        {
            return Math.Round(value - 1 / value, digits);
        }
    }
}
