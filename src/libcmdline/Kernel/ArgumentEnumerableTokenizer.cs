using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal class ArgumentEnumerableTokenizer
    {
        private readonly ArgumentTokenizer _argumentTokenizer;

        public ArgumentEnumerableTokenizer(ArgumentTokenizer argumentTokenizer)
        {
            _argumentTokenizer = argumentTokenizer;
        }

        public IEnumerable<IToken> ToTokenEnumerable(IEnumerable<string> arguments)
        {
            return arguments.SelectMany(arg => _argumentTokenizer.ToTokenEnumerable(arg));
        }
    }
}
