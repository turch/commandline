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
            var list = new OptionPropertyQuery().SelectProperties(options.GetType());

            var map = new OptionMap(list.Count(), _settings);

            foreach (OptionProperty prop in list)
            {
                map[prop.UniqueName] = prop;
            }

            map.RawOptions = options;
            return map;
        }

        public OptionMap CreateVerbOptionMap(T options)
            //,IEnumerable<Tuple<PropertyInfo, VerbOptionAttribute>> verbs)
        {
            var verbs = new VerbOptionPropertyQuery().SelectProperties(options.GetType());

            var map = new OptionMap(verbs.Count(), _settings);

            foreach (OptionProperty verb in verbs)
            {
                //var optionInfo = new OptionProperty(verb.Left(), verb.Right());

                if (!verb.HasParameterLessCtor && verb.UnderlyingProperty.GetValue(options, null) == null)
                {
                    throw new ParserException("Type {0} must have a parameterless constructor or" +
                        " be already initialized to be used as a verb command.".FormatInvariant(verb.UnderlyingProperty.PropertyType));
                }

                map[verb.UniqueName] = verb;
            }

            map.RawOptions = options;
            return map;
        }
    }
}
