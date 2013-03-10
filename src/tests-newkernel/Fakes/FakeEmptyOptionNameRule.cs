using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Kernel;

namespace CommandLineUnitTest.Fakes
{
    class FakeEmptyOptionNameRule : INameSpecificationRule
    {
        public bool ContainsName(string optionName)
        {
            throw new NotImplementedException();
        }
    }
}
