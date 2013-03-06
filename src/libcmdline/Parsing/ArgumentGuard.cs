using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Options;

namespace CommandLine.Parsing
{
    internal static class ArgumentGuard
    {
        public static void EnsureOptionAttributeIsArrayCompatible(OptionInfo option)
        {
            if (!option.IsAttributeArrayCompatible)
            {
                throw new ParserException();
            }
        }

        public static void EnsureOptionArrayAttributeIsNotBoundToScalar(OptionInfo option)
        {
            if (!option.IsArray && option.IsAttributeArrayCompatible)
            {
                throw new ParserException();
            }
        }
    }
}
