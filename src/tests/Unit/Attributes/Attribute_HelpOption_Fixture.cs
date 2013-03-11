using System;
using System.IO;
using CommandLine.Tests.Conventions;
using Xunit;
using FluentAssertions;
using Xunit.Extensions;

namespace CommandLine.Tests.Unit.Attributes
{
    public class Attribute_HelpOption_Fixture
    {
        [Theory, ParserWithHelpTestConventionsAttribute]
        public void Correct_input_not_activates_help(Parser sut, string argument1, string argument2)
        {
            var result = true;
            var arguments = new[] { string.Concat("-i", argument1), string.Concat("-o", argument2) };
            var options = sut.ParseArguments<Fake_With_Help>(arguments, () => result = false);

            result.Should().BeTrue();
            sut.Settings.HelpWriter.ToString().Length.Should().Be(0);
        }

        [Theory, ParserWithHelpTestConventionsAttribute]
        public void Bad_input_activates_help(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_With_Help>(
                    new[] { "math.xml", "-oresult.xml" }, () => result = false);

            result.Should().BeFalse();
            var helpText = sut.Settings.HelpWriter.ToString();
            (helpText.Length > 0).Should().BeTrue();
        }

        [Theory, ParserWithHelpTestConventionsAttribute]
        public void Explicit_help_activation(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_With_Help>(
                    new[] { "--help" }, () => result = false);

            result.Should().BeFalse();
            var helpText = sut.Settings.HelpWriter.ToString();
            (helpText.Length > 0).Should().BeTrue();
        }
    }
}
