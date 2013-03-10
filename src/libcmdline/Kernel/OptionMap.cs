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
using System.Linq;
using System.Reflection;
using CommandLine.Extensions;
using CommandLine.Infrastructure;
#endregion

namespace CommandLine.Kernel
{
    internal sealed class OptionMap
    {
        private readonly ParserSettings _settings;
        private readonly Dictionary<string, string> _names;
        private readonly Dictionary<string, OptionProperty> _map;
        private readonly Dictionary<string, MutuallyExclusiveInfo> _mutuallyExclusiveSetMap;

        public static OptionMap Create<T>(
            ParserSettings settings,
            T options,
            IOptionPropertyGuard guard,
            IEnumerable<IProperty> properties)
        {
            var map = new OptionMap(settings);

            foreach (OptionProperty prop in properties)
            {
                guard.Execute(prop);

                map[prop.UniqueName] = prop;
            }

            map.RawOptions = options;
            return map;
        }

        internal OptionMap(ParserSettings settings) 
        {
            _settings = settings;
            const int capacity = 16; // TODO: to replace

            IEqualityComparer<string> comparer =
                _settings.CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            _names = new Dictionary<string, string>(capacity, comparer);
            _map = new Dictionary<string, OptionProperty>(capacity * 2, comparer);

            if (_settings.MutuallyExclusive)
            {
                _mutuallyExclusiveSetMap = new Dictionary<string, MutuallyExclusiveInfo>(capacity, StringComparer.OrdinalIgnoreCase);
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
                OptionProperty option = null;

                if (_map.ContainsKey(key))
                {
                    option = _map[key];
                }
                else
                {
                    if (_names.ContainsKey(key))
                    {
                        var optionKey = _names[key];
                        option = _map[optionKey];
                    }
                }

                return option;
            }

            set
            {
                _map[key] = value;

                if (value.HasBothNames)
                {
                    _names[value.LongName] = new string(value.ShortName.Value, 1);
                }
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

            foreach (var option in _map.Values)
            {
                if (option.IsDefined && option.MutuallyExclusiveSet != null)
                {
                    BuildMutuallyExclusiveMap(option);
                }
            }

            foreach (var info in _mutuallyExclusiveSetMap.Values)
            {
                if (info.Occurrence > 1)
                {
                    SetParserStateIfNeeded(RawOptions, info.BadOption, null, true);
                    return false;
                }
            }

            return true;
        }

        private void BuildMutuallyExclusiveMap(OptionProperty option)
        {
            var setName = option.MutuallyExclusiveSet;
            if (!_mutuallyExclusiveSetMap.ContainsKey(setName))
            {
                _mutuallyExclusiveSetMap.Add(setName, new MutuallyExclusiveInfo(option));
            }

            _mutuallyExclusiveSetMap[setName].IncrementOccurrence();
        }
  
        private sealed class MutuallyExclusiveInfo
        {
            private int _count;

            public MutuallyExclusiveInfo(OptionProperty option)
            {
                BadOption = option;
            }

            public OptionProperty BadOption { get; private set; }

            public int Occurrence
            {
                get { return this._count; }
            }

            public void IncrementOccurrence()
            {
                ++this._count;
            }
        }
    }
}