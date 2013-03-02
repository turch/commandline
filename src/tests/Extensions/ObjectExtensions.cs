﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Extensions
{
    static class ObjectExtensions
    {
        public static string ToItalianCulture(this object value)
        {
            return Convert.ToString(value, new CultureInfo("it-IT"));
        }

        public static string ToInvariantCulture(this object value)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
