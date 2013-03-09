using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    /// <summary>
    /// Tokenize a single argument without options specification knowledge.
    /// </summary>
    internal class StringTokenizer : IStringTokenizer
    {
        public IEnumerable<IToken> ToTokenEnumerable(string argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            if (argument.Equals("=", StringComparison.Ordinal))
            {
                return new IToken[] { new EqualToken() };
            }

            if (argument.Length > 2 && argument.Substring(0, 2) == "--")
            {
                if (argument.Contains("="))
                {
                    var parts = argument.Split('=');
                    var lhs = parts[0].Substring(2);
                    var rhs = parts[1];
                    return new IToken[] { new LongOptionToken(lhs), new EqualToken(), new ValueToken(rhs) };
                }
            }

            if (argument.Length > 2 && argument[0] == '-')
            {
                var group = argument.Substring(1);
                return new IToken[] { new OptionGroupToken(argument)  };
            }

            return new IToken[] { new ValueToken(argument) };
        }

    }
}
