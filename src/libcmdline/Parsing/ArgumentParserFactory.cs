using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Extensions;

namespace CommandLine.Parsing
{
    internal static class ArgumentParserFactory
    {
        public static IArgumentParser Create(string argument, bool ignoreUnknownArguments = false)
        {
            if (argument.IsNumeric())
            {
                //return null;
                return new NullArgumentParser();
            }

            if (IsDash(argument))
            {
                //return null;
                return new NullArgumentParser();
            }

            if (IsLongOption(argument))
            {
                return new LongOptionParser(ignoreUnknownArguments);
            }

            if (IsShortOption(argument))
            {
                return new OptionGroupParser(ignoreUnknownArguments);
            }

            //return null;
            return new NullArgumentParser();
        }

        private static bool IsDash(string value)
        {
            return string.CompareOrdinal(value, "-") == 0;
        }

        private static bool IsShortOption(string value)
        {
            return value[0] == '-';
        }

        private static bool IsLongOption(string value)
        {
            return value[0] == '-' && value[1] == '-';
        }
    }
}