using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class OptionGroupToken : OptionToken
    {
        public OptionGroupToken(string text)
            : base(text)
        {
        }
    }
}
