using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    public interface IArgumentEquatable
    {
        bool EqualsTo(string rawArgument);
    }
}