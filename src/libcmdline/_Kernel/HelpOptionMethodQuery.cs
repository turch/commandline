using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class HelpOptionMethodQuery : IMethodQuery
    {
        public IEnumerable<IMethod> SelectMethods(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return from mi in type.GetMethods()
                   let attributes = mi.GetCustomAttributes(typeof(HelpOptionAttribute), true)
                   where attributes.Length > 0
                   select new HelpOptionMethod(mi) as IMethod;
        }
    }
}
