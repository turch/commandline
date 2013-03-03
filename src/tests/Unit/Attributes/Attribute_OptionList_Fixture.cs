using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Attributes
{
    public class Attribute_OptionList_Fixture
    {

        [Fact]
        public void Should_use_property_name_as_long_name_if_omitted()
        {
            // Given
            var parser = new CommandLine.Parser();
            var result = true;
            var arguments = new[] {
                "--segments", "header.txt:body.txt:footer.txt"
            };

            // When
            var options = parser.ParseArguments<Fake_With_Implicit_LongName_Options>(
                arguments, () => { result = false; });

            // Than
            result.Should().BeTrue();
            options.Should().NotBeNull();
            options.Segments.Should().HaveCount(c => c == 3);
            options.Segments.Should().ContainInOrder(new[] { "header.txt", "body.txt", "footer.txt" });
        }
    }
}
