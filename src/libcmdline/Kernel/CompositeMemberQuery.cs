using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal class CompositeMemberQuery : IMemberQuery
    {
        private readonly IEnumerable<IMemberQuery> _queries;

        public CompositeMemberQuery(IEnumerable<IMemberQuery> queries)
            : this(queries.ToArray())
        {
        }

        public CompositeMemberQuery(params IMemberQuery[] queries)
        {
            if (queries == null)
            {
                throw new ArgumentNullException("queries");
            }

            _queries = queries;
        }

        public IEnumerable<IMember> SelectMembers(Type type)
        {
            return (from query in _queries
                    from result in query.SelectMembers(type)
                    select result);
        }
    }
}
