using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal static class ParserImpl
    {
        public static IEnumerable<ParsedValue> Parse(
            IEnumerable<string> arguments,
            IEnumerable<OptionSpec> optionSpecs)
        {
            var parsedValues = new List<ParsedValue>();
            var index = 0;

            while (index < arguments.Count())
            {
                var arg = arguments.ElementAt(index);

                if (arg.Length == 0 || arg.Length == 1)
                {
                    parsedValues.Add(new ParsedValue { Value = arg });
                    continue;
                }

                // option or option group
                if (arg.Length > 2 && arg[0] == '-' && arg[1] != '-')
                {
                    var segm = arg.Substring(1);
                    if (segm.Length == 1)
                    {
                        var segmAsChar = segm[0];
                        var foundSpecs = optionSpecs.Where(spec => spec.ShortName == segmAsChar);
                        var count = foundSpecs.Count();
                        if (count == 0)
                        {
                            parsedValues.Add(new ParsedValue { Error = ParsedValueErrorKind.NotFound });
                        }
                        if (count > 1)
                        {
                            throw new InvalidOperationException();
                        }
                        var foundSpec = foundSpecs.ElementAt(0);
                        if (foundSpec.BoundToBoolean)
                        {
                            parsedValues.Add(new ParsedValue { Name = segm, Specification = foundSpec });
                        }
                    }
                }
            }

            return parsedValues;
        }
    }
}
