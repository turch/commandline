using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Kernel
{
    internal interface IMemberQuery
    {
        IEnumerable<IMember> SelectMembers(Type type);
    }
}