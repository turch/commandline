using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Kernel;

namespace CommandLineUnitTest.Fakes
{
    class FakeShortOptionSpecification : IOptionSpecification
    {
        private readonly string[] _shortNames;

        public FakeShortOptionSpecification()
        {
            _shortNames = new[] { "a", "b", "c", "d" };
        }

        public bool IsSatisfiedBy(string name)
        {
            if (name.Length != 1)
            {
                throw new ArgumentNullException("name");
            }
            return _shortNames.Contains(name, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
