using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Parsing
{
    internal static class IArgumentEnumeratorExtensions
    {
        public static IList<string> ConsumeNextValues(this IArgumentEnumerator enumerator)
        {
            var list = new List<string>();

            while (enumerator.MoveNext())
            {
                if (ArgumentComparer.IsValue(enumerator.Current))
                {
                    list.Add(enumerator.Current);
                }
                else
                {
                    break;
                }
            }

            if (!enumerator.MovePrevious())
            {
                throw new ParserException();
            }

            return list;
        }
    }
}
