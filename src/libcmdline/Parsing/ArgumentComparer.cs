using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Extensions;

namespace CommandLine.Parsing
{
    internal class ArgumentComparer
    {
        public static bool CompareAsShortNameOption(string argument, char? option, bool caseSensitive)
        {
            return string.Compare(
                argument,
                AsShortOption(option),
                caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool CompareAsLongNameOption(string argument, string option, bool caseSensitive)
        {
            return string.Compare(
                argument,
                AsLongOption(option),
                caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool IsValue(string argument)
        {
            if (argument.IsNumeric())
            {
                return true;
            }

            if (argument.Length > 0)
            {
                return IsDash(argument) || !IsShortOption(argument);
            }

            return true;
        }

        private static bool IsShortOption(string value)
        {
            return value[0] == '-';
        }

        private static string AsLongOption(string value)
        {
            return string.Concat("--", value);
        }

        private static string AsShortOption(char? value)
        {
            return string.Concat("-", value);
        }

        private static bool IsDash(string value)
        {
            return string.CompareOrdinal(value, "-") == 0;
        }
    }
}
