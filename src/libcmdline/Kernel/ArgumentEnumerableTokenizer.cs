using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal class ArgumentEnumerableTokenizer
    {
        private readonly ArgumentTokenizer _stringTokenizer;

        public ArgumentEnumerableTokenizer()
        {
            //_stringTokenizer = new ArgumentTokenizer();
        }

        public IEnumerable<IToken> ToTokenEnumerable(IEnumerable<string> arguments)
        {
            return arguments.SelectMany(arg => _stringTokenizer.ToTokenEnumerable(arg));
        }
    }
}
