using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Parsing
{
    internal sealed class ChangeState
    {
        private readonly IList<ParsingError> _parsingErrors; 

        public ChangeState()
        {
            _parsingErrors = new List<ParsingError>();
        }


    }
}
