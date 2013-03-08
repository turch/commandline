using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Kernel;
using Ploeh.AutoFixture.Kernel;
using Xunit;

namespace CommandLineUnitTest.Kernel
{
    public class StringTokenizerTest
    {
        [Fact]
        public void SutIsStringTokenizer()
        {
            // Fixture setup
            // Exercise system
            var sut = new StringTokenizer();
            // Verify outcome
            Assert.IsAssignableFrom<IStringTokenizer>(sut);
            // Teardown
        }
    }
}
