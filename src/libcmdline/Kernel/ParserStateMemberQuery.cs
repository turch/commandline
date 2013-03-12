using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class ParserStateMemberQuery : IMemberQuery
    {
        public IEnumerable<IMember> SelectMembers(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            yield return (from pi in type.GetProperties()
                   let attributes = pi.GetCustomAttributes(typeof(ParserStateAttribute), true)
                   where attributes.Length == 1
                   select new ParserStateProperty(pi) as IMember).SingleOrDefault();
        }
    }
}