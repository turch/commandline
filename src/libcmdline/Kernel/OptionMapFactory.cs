using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Extensions;

namespace CommandLine.Kernel
{
    internal class OptionMapFactory<T>
    {
        private readonly ParserSettings _settings;

        public OptionMapFactory(
            ParserSettings settings)
        {
            _settings = settings;
        }

        public OptionMap CreateOptionMap(T options)
        {
            var list = MetadataQuery.Get<PropertyInfo, BaseOptionAttribute, T>(
                options,
                a => a.Item1 is PropertyInfo && a.Item2 is BaseOptionAttribute);
            //var list = new OptionPropertyQuery().SelectProperties(typeof(T));

            var map = new OptionMap(list.Count(), _settings);

            foreach (var pair in list)
            {
                var prop = new OptionProperty(pair.Left(), pair.Right());
                map[prop.UniqueName] = prop;
            }

            map.RawOptions = options;
            return map;
        }

        public OptionMap CreateVerbOptionMap(T options,
            IEnumerable<Tuple<PropertyInfo, VerbOptionAttribute>> verbs)
        {
            var map = new OptionMap(verbs.Count(), _settings);

            foreach (var verb in verbs)
            {
                var optionInfo = new OptionProperty(verb.Left(), verb.Right());

                if (!optionInfo.HasParameterLessCtor && verb.Left().GetValue(options, null) == null)
                {
                    throw new ParserException("Type {0} must have a parameterless constructor or" +
                        " be already initialized to be used as a verb command.".FormatInvariant(verb.Left().PropertyType));
                }

                map[verb.Right().UniqueName] = optionInfo;
            }

            map.RawOptions = options;
            return map;
        }
    }
}
