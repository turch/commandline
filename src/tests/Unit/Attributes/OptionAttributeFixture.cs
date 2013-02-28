using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Unit.Attributes
{
    public class OptionAttributeFixture
    {
        [Fact]
        public void Should_use_property_name_as_long_name_if_omitted()
        {
            // Given
            var parser = new CommandLine.Parser();
            var result = true;
            var arguments = new[] {
                "--download", "something",
                "--up-load", "this",
                "-b", "1024"
            };

            // When
            var options = parser.ParseArguments<OptionsWithImplicitLongName>(
                arguments, () => { result = false; });

            // Than
            result.Should().BeTrue();
            options.Should().NotBeNull();
            options.Download.Should().Be("something");
            options.Upload.Should().Be("this");
            options.Bytes.Should().Be(1024);
        }
    }
}
