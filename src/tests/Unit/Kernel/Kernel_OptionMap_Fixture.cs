using System.Collections.Generic;
using CommandLine.Kernel;
using CommandLine.Parsing;
using CommandLine.Tests.Fakes;

using Xunit;
using FluentAssertions;
using CommandLine;
using CommandLine.Infrastructure;

namespace CommandLine.Tests.Unit.Kernel
{
    public class Kernel_OptionMap_Fixture
    {
        [Fact]
        public void Retrieve_option_with_both_name()
        {
            var options = new Fake_Simple_Options();
            var props = new OptionPropertyQuery().SelectProperties(options.GetType());
            var sut = OptionMap.Create(new ParserSettings(), options, new NullOptionPropertyGuard(), props);

            var opt1 = sut["s"];
            var opt2 = sut["string"];

            opt1.Should().Be(opt2);
            opt1.ShortName.Should().Be('s');
            opt1.LongName.Should().Be("string");
        }

        [Fact]
        public void Retrieve_option_with_short_name()
        {
            var options = new Fake_Simple_Options();
            var props = new OptionPropertyQuery().SelectProperties(options.GetType());
            var sut = OptionMap.Create(new ParserSettings(), options, new NullOptionPropertyGuard(), props);

            var opt = sut["i"];

            opt.ShortName.Should().Be('i');
            opt.LongName.Should().BeNull();
        }

        [Fact]
        public void Retrieve_option_with_long_name()
        {
            var options = new Fake_Simple_Options();
            var props = new OptionPropertyQuery().SelectProperties(options.GetType());
            var sut = OptionMap.Create(new ParserSettings(), options, new NullOptionPropertyGuard(), props);

            var opt = sut["switch"];

            opt.ShortName.Should().BeNull();
            opt.LongName.Should().Be("switch");
        }

        [Fact]
        public void Retrieve_non_existent_option()
        {
            var options = new Fake_Simple_Options();
            var props = new OptionPropertyQuery().SelectProperties(options.GetType());
            var sut = OptionMap.Create(new ParserSettings(), options, new NullOptionPropertyGuard(), props);

            var opt = sut["maybe-avalue"];

            opt.Should().BeNull();
        }
    }
}

