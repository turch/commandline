using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Extensions;
using CommandLine.Options;

namespace CommandLine.Parsing
{
    internal sealed class ArgumentParserFactory<T>
    {
        private readonly T _options;
        private readonly OptionMap _map;
        private readonly ParserSettings _settings;

        public ArgumentParserFactory(T options, OptionMap map, ParserSettings settings)
        {
            _options = options;
            _map = map;
            _settings = settings;
        }

        public ArgumentParser<T> Create(string argument)
        {
            if (argument.IsNumeric())
            {
                return new NullArgumentParser<T>(_options, _map, _settings);
            }

            if (IsDash(argument))
            {
                return new NullArgumentParser<T>(_options, _map, _settings);
            }

            if (IsLongOption(argument))
            {
                return new LongOptionParser<T>(_options, _map, _settings);
            }

            if (IsShortOption(argument))
            {
                return new OptionGroupParser<T>(_options, _map, _settings);
            }

            //return null;
            return new NullArgumentParser<T>(_options, _map, _settings);
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