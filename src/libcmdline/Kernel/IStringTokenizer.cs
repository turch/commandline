using System.Collections.Generic;

namespace CommandLine.Kernel
{
    internal interface IStringTokenizer
    {
        IEnumerable<IToken> ToTokenEnumerable(string argument);
    }
}