using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Kernel
{
    internal class HelpOptionMethod : IMethod
    {
        private readonly MethodInfo _method;

        public HelpOptionMethod(MethodInfo method)
        {
            _method = method;
        }
    }
}
