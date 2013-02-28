using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using CommandLine.Tests.Fakes;
using FluentAssertions;

namespace CommandLine.Tests.Unit.Attributes
{
    public class OptionArrayAttributeFixture
    {
        /// <summary>
        /// https://github.com/gsscoder/commandline/issues/6
        /// </summary>
        [Fact]
        public void Should_correctly_parse_two_consecutive_arrays()
        {
            // Given
            var parser = new CommandLine.Parser();
            var result = true;
            var argumets = new[] { "--source", @"d:/document.docx", "--output", @"d:/document.xlsx",
                    "--headers", "1", "2", "3", "4",              // first array
                    "--content", "5", "6", "7", "8", "--verbose"  // second array
                };

            // When
            var options = parser.ParseArguments<OptionsWithTwoArrays>(argumets, () => { result = false; });

            // Than
            result.Should().BeTrue();
            options.Should().NotBeNull();
            options.Headers.Should().HaveCount(c => c == 4);
            options.Headers.Should().ContainInOrder(new uint[] { 1, 2, 3, 4 });
            options.Content.Should().HaveCount(c => c == 4);
            options.Content.Should().ContainInOrder(new uint[] { 5, 6, 7, 8 });
        }

        [Fact]
        public void Should_use_property_name_as_long_name_if_omitted()
        {
            // Given
            var parser = new CommandLine.Parser();
            var result = true;
            var arguments = new[] {
                "--offsets", "-2", "-1", "0", "1" , "2"
            };

            // When
            var options = parser.ParseArguments<OptionsWithImplicitLongName>(arguments, () => { result = false; });

            // Than
            result.Should().BeTrue();
            options.Should().NotBeNull();
            options.Offsets.Should().HaveCount(c => c == 5);
            options.Offsets.Should().ContainInOrder(new[] { -2, -1, 0, 1, 2 });
        }
    }
}
