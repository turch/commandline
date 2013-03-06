using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Parsing
{
    internal class SuccessfulTransition : Transition
    {
        public SuccessfulTransition()
            : base(Enumerable.Empty<ParsingError>())
        {
        }
    }
}