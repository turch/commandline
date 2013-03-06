using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Parsing
{
    internal sealed class FailureTransition : Transition
    {
        public FailureTransition(IEnumerable<ParsingError> parsingErrors)
            : base(parsingErrors)
        {
        }
    }
}