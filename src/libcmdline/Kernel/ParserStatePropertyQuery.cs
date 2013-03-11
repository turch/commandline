using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class ParserStatePropertyQuery : IPropertyQuery
    {
        public IEnumerable<IProperty> SelectProperties(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            yield return (from pi in type.GetProperties()
                   let attributes = pi.GetCustomAttributes(typeof(ParserStateAttribute), true)
                   where attributes.Length == 1
                   select new ParserStateProperty(pi) as IProperty).SingleOrDefault();
        }
    }
}