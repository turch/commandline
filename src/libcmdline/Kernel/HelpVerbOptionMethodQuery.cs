using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class HelpVerbOptionMethodQuery : IMemberQuery
    {
        public IEnumerable<IMember> SelectMembers(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            yield return (from mi in type.GetMethods()
                          let attributes = mi.GetCustomAttributes(typeof(HelpVerbOptionAttribute), true)
                          where attributes.Length > 0
                          select new HelpVerbOptionMethod(mi, (HelpVerbOptionAttribute)attributes.ElementAt(0)) as IMember).SingleOrDefault();
        }
    }
}
