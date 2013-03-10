using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Kernel;

namespace CommandLine.Parsing
{
    internal static class ArgumentGuard
    {
        public static void EnsureOptionAttributeIsArrayCompatible(OptionProperty option)
        {
            if (!option.IsAttributeArrayCompatible)
            {
                throw new ParserException();
            }
        }

        public static void EnsureOptionArrayAttributeIsNotBoundToScalar(OptionProperty option)
        {
            if (!option.IsArray && option.IsAttributeArrayCompatible)
            {
                throw new ParserException();
            }
        }
    }
}
