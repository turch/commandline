using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Kernel
{
    /// <summary>
    /// Tokenize a single argument without options specification knowledge.
    /// </summary>
    internal class Tokenizer
    {
        private readonly IOptionNameRule _nameRule;

        public Tokenizer(IOptionNameRule nameRule)
        {
            _nameRule = nameRule;
        }

        public IEnumerable<IToken> CreateTokens(string argument)
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

            if (argument.Length > 1 && argument[0] == '-')
            {
                return new IToken[] { new ShortOptionToken(argument.Substring(1)) };
            }

            if (argument.Length > 2 && argument[0] == '-')
            {
                var items = argument.Substring(1).Select(c => c.ToOptionName());

                return items.TakeWhile(n => _nameRule.ContainsName(n))
                    .Select(o => new ShortOptionToken(o))
                    .Concat(new IToken[] {
                        new ValueToken(items.SkipWhile(n => !this._nameRule.ContainsName(n)).Aggregate((acc, c) => acc += c))
                        });
            }

            return new IToken[] { new ValueToken(argument) };
        }

    }
}
