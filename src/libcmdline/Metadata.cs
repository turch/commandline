using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine
{
    static class Metadata
    {
        public static IEnumerable<Tuple<MemberInfo, object>> GetAttributes<T>(T options)
        {
            return from member in options.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                   let attributes = member.GetCustomAttributes(true)
                   from attribute in attributes
                   where attribute is ITargetDescriptor
                   select new Tuple<MemberInfo, object>(member, attribute);
        }
    }
}