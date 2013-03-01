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
        private static readonly Dictionary<object, IEnumerable<Tuple<MemberInfo, object>>> Cache = new Dictionary<object, IEnumerable<Tuple<MemberInfo, object>>>();

        public static IEnumerable<Tuple<MemberInfo, object>> GetAll<T>(T options)
        {
            IEnumerable<Tuple<MemberInfo, object>> metadata;

            if (!Cache.ContainsKey(options))
            {
                metadata = from member in options
                               .GetType()
                               .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                            let attributes = member.GetCustomAttributes(true)
                            from attribute in attributes
                            where attribute is ITargetDescriptor
                            select new Tuple<MemberInfo, object>(member, attribute);
                Cache.Add(options, metadata);
            }
            else
            {
                metadata = Cache[options];
            }

            return metadata;
        }

        public static IEnumerable<Tuple<TMember, TAttribute>> Get<TMember, TAttribute, T>(T options, Func<Tuple<MemberInfo, object>, bool> predicate)
            where TMember : MemberInfo
            where TAttribute : ITargetDescriptor
        {
            var attributes = Metadata.GetAll(options).Where(predicate);
            return attributes.Select(a => new Tuple<TMember, TAttribute>((TMember)a.Item1, (TAttribute)a.Item2));
        }

        public static Tuple<TMember, TAttribute> GetSingle<TMember, TAttribute, T>(T options, Func<Tuple<MemberInfo, object>, bool> predicate)
            where TMember : MemberInfo
            where TAttribute : ITargetDescriptor
        {
            var attribute = Metadata.GetAll(options).SingleOrDefault(predicate);
            return attribute != null ?
                new Tuple<TMember, TAttribute>(
                    (TMember)attribute.Item1,
                    (TAttribute)attribute.Item2) :
                null;
        }
    }
}