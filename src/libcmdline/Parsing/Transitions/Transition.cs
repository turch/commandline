using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Parsing
{
    internal abstract class Transition
    {
        private readonly IEnumerable<ParsingError> _parsingErrors;

        protected Transition(IEnumerable<ParsingError> parsingErrors)
        {
            _parsingErrors = Enumerable.Empty<ParsingError>().Concat(parsingErrors);
        }

        public IEnumerable<ParsingError> ParsingErrors
        {
            get
            {
                return _parsingErrors;
            }
        }
    }
}