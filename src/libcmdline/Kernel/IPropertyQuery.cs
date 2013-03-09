using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Kernel
{
    internal interface IPropertyQuery
    {
        IEnumerable<IProperty> SelectProperties(Type type);
    }
}