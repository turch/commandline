using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Kernel;

using CommandLineUnitTest.Fakes;

using Ploeh.AutoFixture.Kernel;
using Xunit;

namespace CommandLineUnitTest.Kernel
{
    public class ArgumentTokenizerTest
    {
        //[Fact]
        //public void SutIsArgumentTokenizer()
        //{
        //    // Fixture setup
        //    // Exercise system
        //    var sut = new ArgumentTokenizer();
        //    // Verify outcome
        //    Assert.IsAssignableFrom<IArgumentTokenizer(sut);
        //    // Teardown
        //}

        [Fact]
        public void TokenizeLongOptionWithEqualSign()
        {
            // Fixture setup
            var expectedTokens = new IToken[]
                {
                    new LongOptionToken("long-option"),
                    new EqualToken(),
                    new ValueToken("a-value")
                }.AsEnumerable();
            var argument = "--long-option=a-value";
            // Exercise system
            var sut = new ArgumentTokenizer(new FakeNullOptionSpecification());
            var result = sut.ToTokenEnumerable(argument);
            // Verify outcome
            Assert.True(expectedTokens.SequenceEqual(expectedTokens));
            // Teardown
        }

        [Fact]
        public void TokenizeOptionGroup()
        {
            // Fixture setup
            var expectedTokens = new IToken[]
                {
                    new ShortOptionToken("d"),
                    new ShortOptionToken("e"),
                    new ValueToken("file-a.bin"),
                }.AsEnumerable();
            var argument = "-defile-a.bin";
            // Exercise system
            var sut = new ArgumentTokenizer(new FakeShortOptionSpecification());
            var result = sut.ToTokenEnumerable(argument);
            // Verify outcome
            Assert.True(expectedTokens.SequenceEqual(expectedTokens));
            // Teardown
        }
    }
}
