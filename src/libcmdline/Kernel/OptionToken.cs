using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal abstract class OptionToken : Token
    {
        protected OptionToken(string text)
            : base(text)
        {
        }
    }
}