using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal class ParsedValueBuilder
    {
        private readonly IEnumerable<IOptionSpecifiaction> _specifiactions;

        public ParsedValueBuilder(IEnumerable<IOptionSpecifiaction> specifiactions)
        {
            _specifiactions = specifiactions;
        }

        public IEnumerable<ParsedValue> CreateParseValues(IToken[] tokens)
        {
            return null;
        }
    }
}
