using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Kernel;

namespace CommandLine.Parsing
{
    internal sealed class NullArgumentParser<T> : ArgumentParser<T>
    {
        public NullArgumentParser(T options, OptionMap map, ParserSettings settings)
            : base(options, map, settings)
        {
        }

        public override Transition Parse(IArgumentEnumerator argumentEnumerator)
        {
            return new SuccessfulTransition();
        }
    }
}
