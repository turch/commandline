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
    public class TokenizerTest
    {
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
            var sut = new Tokenizer(new FakeEmptyOptionNameRule());
            var result = sut.CreateTokens(argument);
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
            var sut = new Tokenizer(new FakeShortOptionNameRule());
            var result = sut.CreateTokens(argument);
            // Verify outcome
            Assert.True(expectedTokens.SequenceEqual(expectedTokens));
            // Teardown
        }
    }
}
