using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class OptionPropertyQuery : IOptionPropertyQuery
    {
        public IEnumerable<IOptionProperty> SelectProperties(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return from pi in type.GetProperties()
                   let attributes = pi.GetCustomAttributes(typeof(IOptionAttribute), true)
                   where attributes.Length > 0
                   select new OptionProperty(pi) as IOptionProperty;
        }
    }
}