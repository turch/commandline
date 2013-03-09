using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    /// <summary>
    /// Pulled from https://github.com/AutoFixture/AutoFixture source code.
    /// </summary>
    internal static class LightweightMaybe
    {
        internal static IEnumerable<T> Maybe<T>(this T value)
        {
            return new[] { value };
        }
    }
}