using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class ShortOptionToken : OptionToken
    {
        public ShortOptionToken(string text)
            : base(text)
        {
        }
    }
}
