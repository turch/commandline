using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Parsing.Transitions
{
    static class TransitionExtensions
    {
        public static FailureTransition ToFailureTransition(this Transition transition, IEnumerable<ParsingError> parsingErrors)
        {
            return new FailureTransition(parsingErrors);
        }

        public static SuccessfulTransition ToSuccessfulTransition(this Transition transition)
        {
            return new SuccessfulTransition();
        }

        public static ContinuationTransition ToContinuationTransition(this Transition transition)
        {
            return new ContinuationTransition();
        }
    }
}