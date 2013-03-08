using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal class StringEnumerableTokenizer
    {
        private readonly IStringTokenizer _stringTokenizer;

        public StringEnumerableTokenizer(IStringTokenizer stringTokenizer)
        {
            _stringTokenizer = stringTokenizer;
        }

        public IEnumerable<IToken> ToTokenEnumerable(IEnumerable<string> arguments)
        {
            return arguments.SelectMany(arg => _stringTokenizer.ToTokenEnumerable(arg));
        }
    }
}
