using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Options;

namespace CommandLine.Parsing
{
    internal sealed class NullArgumentParser : IArgumentParser
    {
        public Transition Parse<T>(IArgumentEnumerator argumentEnumerator, OptionMap map, T options)
        {
            return new SuccessfulTransition();
        }
    }
}
