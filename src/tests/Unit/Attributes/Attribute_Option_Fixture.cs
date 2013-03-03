using CommandLine.Tests.Conventions;
using CommandLine.Tests.Extensions;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit.Extensions;

namespace CommandLine.Tests.Unit.Attributes
{
    public class Attribute_Option_Fixture
    {
        [Theory, ParserTestConventions]
        public void Use_Property_Name_if_is_Long_Name_if_Omitted(Parser sut)
        {
            // Given
            var result = true;
            var arguments = new[] {
                "--download", "something",
                "--up-load", "this",
                "-b", "1024"
            };

            // When
            var options = sut.ParseArguments<Fake_With_Implicit_LongName_Options>(
                arguments, () => { result = false; });

            // Than
            result.Should().BeTrue();
            options.Should().NotBeNull();
            options.Download.Should().Be("something");
            options.Upload.Should().Be("this");
            options.Bytes.Should().Be(1024);
        }

        [Theory, ParserWithHelpTestDynamicAutoBuildConventionsAttribute]
        public void Use_Property_Name_if_is_Long_Name_if_Omitted_when_printing_Help(Parser sut)
        {
            var arguments = new[] { "-b", "not_a_number" };

            sut.ParseArguments<Fake_With_Implicit_LongName_Options>(arguments, () => { });

            var lines = sut.Settings.HelpWriter.AsLines().Select(a => a.Trim()).ToArray();

            lines[2].Should().Be("--download");
            lines[3].Should().Be("--up-load");
            lines[4].Should().Be("-b");
            lines[5].Should().Be("--offsets");
            lines[6].Should().Be("--segments");
        }
    }
}