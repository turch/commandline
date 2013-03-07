using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class LongOptionToken : OptionToken
    {
        public LongOptionToken(string text)
            : base(text)
        {
        }
    }
}
