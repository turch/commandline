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
    public class Kernel_OptionMap_MutuallyExclusive_Fixture
    {
        [Fact]
        public void Options_from_different_set_cannot_be_specified()
        {
            var options = new Fake_WithMultipleSet_Options();
            var props = new OptionMemberQuery().SelectMembers(options.GetType());
            var sut = OptionMap.Create(
                new ParserSettings { MutuallyExclusive = true }, options, new NullOptionPropertyGuard(), props);

            sut["red"].IsDefined = true;
            sut["red"].ReceivedValue = true;
            sut["green"].IsDefined = true;
            sut["green"].ReceivedValue = true;

            var result = sut.EnforceRules();

            result.Should().BeFalse();
        }

        [Fact]
        public void Options_from_different_set_can_be_specified()
        {
            var options = new Fake_WithMultipleSet_Options();
            var props = new OptionMemberQuery().SelectMembers(options.GetType());
            var sut = OptionMap.Create(
                new ParserSettings { MutuallyExclusive = true }, options, new NullOptionPropertyGuard(), props);

            sut["red"].IsDefined = true;
            sut["red"].ReceivedValue = true;
            sut["hue"].IsDefined = true;
            sut["hue"].ReceivedValue = true;

            var result = sut.EnforceRules();

            result.Should().BeTrue();
        }

    }
}
