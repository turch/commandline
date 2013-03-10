using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal class ParsedValue
    {
        private readonly IProperty _property;

        public ParsedValue(IProperty property)
        {
            _property = property;
        }
    }
}
