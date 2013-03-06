using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Extensions;

namespace CommandLine.Options
{
    internal static class OptionMapFactory
    {
        public static OptionMap Create<T>(T target, ParserSettings settings)
        {
            var list = Metadata.Get<PropertyInfo, BaseOptionAttribute, T>(
                target,
                a => a.Item1 is PropertyInfo && a.Item2 is BaseOptionAttribute);
            if (list == null)
            {
                return null;
            }

            var map = new OptionMap(list.Count(), settings);

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
                        target,
                        settings.ParsingCulture);
                }
            }

            map.RawOptions = target;
            return map;
        }

        public static OptionMap Create<T>(
            T target,
            IEnumerable<Tuple<PropertyInfo, VerbOptionAttribute>> verbs,
            ParserSettings settings)
        {
            var map = new OptionMap(verbs.Count(), settings);

            foreach (var verb in verbs)
            {
                var optionInfo = OptionInfoFactory.CreateFromMetadata(verb.Right(), verb.Left(), target, settings.ParsingCulture);

                if (!optionInfo.HasParameterLessCtor && verb.Left().GetValue(target, null) == null)
                {
                    throw new ParserException("Type {0} must have a parameterless constructor or" +
                        " be already initialized to be used as a verb command.".FormatInvariant(verb.Left().PropertyType));
                }

                map[verb.Right().UniqueName] = optionInfo;
            }

            map.RawOptions = target;
            return map;
        }
    }
}
