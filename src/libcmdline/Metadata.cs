using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine
{
    static class Metadata
    {
        private static readonly Dictionary<Type, IEnumerable<Tuple<MemberInfo, object>>> Cache = new Dictionary<Type, IEnumerable<Tuple<MemberInfo, object>>>();

        public static IEnumerable<Tuple<MemberInfo, object>> GetAttributes<T>(T options)
        {
            IEnumerable<Tuple<MemberInfo, object>> metadata;

            if (!Cache.ContainsKey(typeof(T)))
            {
                metadata = from member in options
                               .GetType()
                               .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                            let attributes = member.GetCustomAttributes(true)
                            from attribute in attributes
                            where attribute is ITargetDescriptor
                            select new Tuple<MemberInfo, object>(member, attribute);
                Cache.Add(typeof(T), metadata);
            }
            else
            {
                metadata = Cache[typeof(T)];
            }

            return metadata;
        }
    }
}