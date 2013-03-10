using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class OptionGroupToken : Token
    {
        public OptionGroupToken(string text)
            : base(text)
        {
        }
    }
}
