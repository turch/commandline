using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Extensions;

namespace CommandLine.Parsing
{
    internal class ArgumentComparer
    {
        public static bool CompareToShort(string argument, char? option, bool caseSensitive)
        {
            return string.Compare(
                argument,
                ToOption(option),
                caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool CompareToLong(string argument, string option, bool caseSensitive)
        {
            return string.Compare(
                argument,
                ToOption(option),
                caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool IsInputValue(string argument)
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

        private static bool IsLongOption(string value)
        {
            return value[0] == '-' && value[1] == '-';
        }

        private static string ToOption(string value)
        {
            return string.Concat("--", value);
        }

        private static string ToOption(char? value)
        {
            return string.Concat("-", value);
        }

        private static bool IsDash(string value)
        {
            return string.CompareOrdinal(value, "-") == 0;
        }
    }
}
