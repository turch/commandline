using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Extensions;

namespace CommandLine.Kernel
{
    internal sealed class NullOptionPropertyGuard : IOptionPropertyGuard
    {
        public void Execute(OptionProperty verbOption)
        {
        }
    }
}
