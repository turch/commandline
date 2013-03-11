using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Kernel;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Kernel
{
    public class Kernel_OptionMap_Required_Fixture
    {
        [Fact]
        public void If_one_of_two_required_options_is_specified_fails()
        {
            var options = new Fake_Complex_Options();
            var props = new OptionPropertyQuery().SelectProperties(options.GetType());
            var sut = OptionMap.Create(
                new ParserSettings { MutuallyExclusive = true }, options, new NullOptionPropertyGuard(), props);

            sut["input"].IsDefined = true;
            sut["input"].ReceivedValue = true;

            var result = sut.EnforceRules();

            result.Should().BeFalse();
        }

        [Fact]
        public void If_all_required_options_are_specified_succeeds()
        {
            var options = new Fake_Complex_Options();
            var props = new OptionPropertyQuery().SelectProperties(options.GetType());
            var sut = OptionMap.Create(
                new ParserSettings { MutuallyExclusive = true }, options, new NullOptionPropertyGuard(), props);

            sut["input"].IsDefined = true;
            sut["input"].ReceivedValue = true;
            sut["output"].IsDefined = true;
            sut["output"].ReceivedValue = true;

            var result = sut.EnforceRules();

            result.Should().BeTrue();
        }


    }
}
