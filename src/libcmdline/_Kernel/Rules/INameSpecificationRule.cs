using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal interface INameSpecificationRule
    {
        bool ContainsName(string optionName);
    }
}
