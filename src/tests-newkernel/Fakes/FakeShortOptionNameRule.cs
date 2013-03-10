using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Kernel;

namespace CommandLineUnitTest.Fakes
{
    class FakeShortOptionNameRule : INameSpecificationRule
    {
        private readonly string[] _shortNames;

        public FakeShortOptionNameRule()
        {
            _shortNames = new[] { "a", "b", "c", "d" };
        }

        public bool ContainsName(string optionName)
        {
            if (optionName.Length != 1)
            {
                throw new ArgumentNullException("optionName");
            }
            return _shortNames.Contains(optionName, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
