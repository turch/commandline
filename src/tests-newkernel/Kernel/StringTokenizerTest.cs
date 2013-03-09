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

        [Fact]
        public void TokenizeLongOptionWithEqualSign()
        {
            // Fixture setup
            var expectedTokens = new IToken[]
                {
                    new LongOptionToken("long-option"),
                    new EqualToken(),
                    new ValueToken("a-value"), 
                }.AsEnumerable();
            var argument = "--long-option=a-value";
            // Exercise system
            var sut = new StringTokenizer();
            var result = sut.ToTokenEnumerable(argument);
            // Verify outcome
            Assert.True(expectedTokens.SequenceEqual(expectedTokens));
            // Teardown
        }
    }
}
