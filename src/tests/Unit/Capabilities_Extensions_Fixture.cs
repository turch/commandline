using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Tests.Fakes;

using FluentAssertions;

using Xunit;

namespace CommandLine.Tests.Unit
{
    class FakeWithHelp
    {
        [Option]
        public bool IsFake { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return string.Empty;
        }
    }

    public class Capabilities_Extensions_Fixture
    {
        [Fact]
        public void Should_detect_if_target_has_verbs()
        {
            new OptionsWithVerbs().AnyVerbs().Should().BeTrue();
        }

        [Fact]
        public void Should_detect_if_target_has_verb_help()
        {
            // Given when
            var options = new OptionsWithVerbs();

            // Than
            options.HasVerbHelp().Should().BeTrue();
            options.HasHelp().Should().BeFalse();
        }

        [Fact]
        public void Should_detect_if_target_has_help()
        {
            // Given when
            var options = new FakeWithHelp();

            // Than
            options.HasHelp().Should().BeTrue();
            options.HasVerbHelp().Should().BeFalse();
        }

        [Fact]
        public void Should_detect_if_target_has_parser_state()
        {
            new OptionsWithVerbsHelp().CanReceiveParserState().Should().BeTrue();
        }
    }
}
