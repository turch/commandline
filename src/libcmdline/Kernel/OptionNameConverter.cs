using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    /// <summary>
    /// Contains extension methods to convert option names.
    /// </summary>
    static class OptionNameConverter
    {
        public static string ToOptionName(this char name)
        {
            return new string(name, 1);
        }
    }
}
