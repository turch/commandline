using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Kernel;

namespace CommandLineUnitTest.Fakes
{
    class FakeEmptyOptionNameRule : IOptionNameRule
    {
        public bool ContainsName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
