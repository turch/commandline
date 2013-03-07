using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Extensions;

namespace CommandLine.Options
{
    internal class OptionMapFactory<T>
    {
        private readonly T _options;
        private readonly ParserSettings _settings;

        public OptionMapFactory(
            T options,
            ParserSettings settings)
        {
            _options = options;
            _settings = settings;
        }

        public OptionMap CreateOptionMap()
        {
            var list = MetadataQuery.Get<PropertyInfo, BaseOptionAttribute, T>(
                _options,
                a => a.Item1 is PropertyInfo && a.Item2 is BaseOptionAttribute);
            if (list == null)
            {
                return null;
            }

            var map = new OptionMap(list.Count(), _settings);

            foreach (var pair in list)
            {
                if (pair.Left() != null && pair.Right() != null)
                {
                    string uniqueName;

                    if (pair.Right().AutoLongName)
                    {
                        uniqueName = pair.Left().Name.ToLowerInvariant();
                        pair.Right().LongName = uniqueName;
                    }
                    else
                    {
                        uniqueName = pair.Right().UniqueName;
                    }

                    map[uniqueName] = OptionInfoFactory.CreateFromMetadata(
                        pair.Right(),
                        pair.Left(),
                        _options,
                        _settings.ParsingCulture);
                }
            }

            map.RawOptions = _options;
            return map;
        }

        public OptionMap CreateVerbOptionMap(
            IEnumerable<Tuple<PropertyInfo, VerbOptionAttribute>> verbs)
        {
            var map = new OptionMap(verbs.Count(), _settings);

            foreach (var verb in verbs)
            {
                var optionInfo = OptionInfoFactory.CreateFromMetadata(verb.Right(), verb.Left(), _options, _settings.ParsingCulture);

                if (!optionInfo.HasParameterLessCtor && verb.Left().GetValue(_options, null) == null)
                {
                    throw new ParserException("Type {0} must have a parameterless constructor or" +
                        " be already initialized to be used as a verb command.".FormatInvariant(verb.Left().PropertyType));
                }

                map[verb.Right().UniqueName] = optionInfo;
            }

            map.RawOptions = _options;
            return map;
        }
    }
}
