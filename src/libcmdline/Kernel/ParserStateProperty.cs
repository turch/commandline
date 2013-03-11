using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Kernel
{
    // TODO: at the end of this rectoring, outling a better hierarchy
    internal sealed class ParserStateProperty : IProperty
    {
        public ParserStateProperty(PropertyInfo property)
        {
            UnderlyingProperty = property;
        }

        public PropertyInfo UnderlyingProperty { get; private set; }
    }
}
