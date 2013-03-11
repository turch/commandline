#region License
// <copyright file="OptionMap.cs" company="Giacomo Stelluti Scala">
//   Copyright 2015-2013 Giacomo Stelluti Scala
// </copyright>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CommandLine.Extensions;
using CommandLine.Infrastructure;
#endregion

namespace CommandLine.Kernel
{
    // TODO: here apply SRP until the class doesn't comply
    internal sealed class OptionMap
    {
        private readonly ParserSettings _settings;
        private readonly IEqualityComparer<string> _comparer;
        private readonly Dictionary<Tuple<
                string, // short name
                string  // long name
            >, OptionProperty> _map; 

        public static OptionMap Create<T>(
            ParserSettings settings,
            T options,
            IOptionPropertyGuard guard,
            IEnumerable<IProperty> properties)
        {
            var map = new OptionMap(settings, guard, properties);

            map.RawOptions = options;
            return map;
        }

        // TODO: after refactoring -> private
        internal OptionMap(ParserSettings settings, IOptionPropertyGuard guard, IEnumerable<IProperty> properties)
        {
            var capacity = properties.Count();
            _settings = settings;
            _comparer = _settings.CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            _map = new Dictionary<Tuple<string, string>, OptionProperty>(capacity);

            foreach (OptionProperty prop in properties)
            {
                guard.Execute(prop);

                _map.Add(
                    Tuple.Create(
                        prop.ShortName.HasValue ? new string(prop.ShortName.Value, 1) : "",
                        prop.LongName ?? ""),
                    prop);
            }
        }
            
        // TODO: refactoring out this!
        internal object RawOptions
        {
            private get; set;
        }

        public OptionProperty this[string key]
        {
            get
            {
                return _map.SingleOrDefault(
                    s => _comparer.Equals(key, s.Key.Item1) || _comparer.Equals(key, s.Key.Item2)).Value;
            }
        }

        public bool EnforceRules()
        {
            return EnforceMutuallyExclusiveMap() && EnforceRequiredRule();
        }

        // TODO: refactor this
        public void SetDefaults<T>(T options)
        {
            foreach (var option in _map.Values)
            {
                var context = new BindingContext<T>(_settings, option, options);
                context.SetDefault();
            }
        }

        private static void SetParserStateIfNeeded<T>(T options, OptionProperty option, bool? required, bool? mutualExclusiveness)
        {
            var list = MetadataQuery.GetSingle<PropertyInfo, ParserStateAttribute, T>(
                options,
                a => a.Item2 is ParserStateAttribute);

            if (list == null)
            {
                return;
            }

            var property = list.Left();

            // This method can be called when parser state is still not intialized
            if (property.GetValue(options, null) == null)
            {
                property.SetValue(options, new CommandLine.ParserState(), null);
            }

            var parserState = (IParserState)property.GetValue(options, null);
            if (parserState == null)
            {
                return;
            }

            var error = new ParsingError
                {
                    BadOption =
                    {
                        ShortName = option.ShortName,
                        LongName = option.LongName
                    }
                };

            if (required != null)
            {
                error.ViolatesRequired = required.Value;
            }

            if (mutualExclusiveness != null)
            {
                error.ViolatesMutualExclusiveness = mutualExclusiveness.Value;
            }

            parserState.Errors.Add(error);
        }

        private bool EnforceRequiredRule()
        {
            var requiredRulesAllMet = true;

            foreach (var option in _map.Values)
            {
                if (option.Required && !(option.IsDefined && option.ReceivedValue))
                {
                    SetParserStateIfNeeded(RawOptions, option, true, null);
                    requiredRulesAllMet = false;
                }
            }

            return requiredRulesAllMet;
        }

        private bool EnforceMutuallyExclusiveMap()
        {
            if (!_settings.MutuallyExclusive)
            {
                return true;
            }

            var query = from opt in _map.Values
                        where opt.IsDefined && opt.MutuallyExclusiveSet != null
                        group opt by opt.MutuallyExclusiveSet;

            return query.All(@group => @group.Count() <= 1);
        }
    }
}