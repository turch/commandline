using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Kernel;

namespace CommandLineUnitTest.Fakes
{
    class FakeShortOptionNameRule : IOptionNameRule
    {
        private readonly string[] _shortNames;

        public FakeShortOptionNameRule()
        {
            _shortNames = new[] { "a", "b", "c", "d" };
        }

        public bool ContainsName(string name)
        {
            if (name.Length != 1)
            {
                throw new ArgumentNullException("name");
            }
            return _shortNames.Contains(name, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
