using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CommandLine.Extensions
{
    static class ObjectExtensions
    {
        public static string ToInvariantString(this object value)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
