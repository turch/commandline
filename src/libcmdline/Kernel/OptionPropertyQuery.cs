using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal class OptionPropertyQuery : IPropertyQuery
    {
        public IEnumerable<IProperty> SelectProperties(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return from pi in type.GetProperties()
                   let attributes = pi.GetCustomAttributes(typeof(BaseOptionAttribute), true)
                   where attributes.Length > 0
                   select new OptionProperty(pi) as IProperty;
        }
    }
}
