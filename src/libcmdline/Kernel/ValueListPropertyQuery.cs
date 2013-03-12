using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class ValueListPropertyQuery : IPropertyQuery
    {
        public IEnumerable<IProperty> SelectProperties(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            yield return (from pi in type.GetProperties()
                   let attributes = pi.GetCustomAttributes(typeof(ValueListAttribute), true)
                   where attributes.Length == 1
                   select new ValueListProperty(pi, (ValueListAttribute)attributes.ElementAt(0)) as IProperty).SingleOrDefault();
        }
    }
}