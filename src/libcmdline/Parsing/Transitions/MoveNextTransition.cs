using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Parsing
{
    internal class MoveNextTransition : Transition
    {
        public MoveNextTransition()
            : base(Enumerable.Empty<ParsingError>())
        {
        }
    }
}