using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Kernel;

namespace CommandLineUnitTest.Fakes
{
    class FakeNullOptionSpecification : IOptionSpecification
    {
        public bool IsSatisfiedBy(string name)
        {
            throw new NotImplementedException();
        }
    }
}
