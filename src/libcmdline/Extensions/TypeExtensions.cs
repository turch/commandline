using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Extensions
{
    static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
