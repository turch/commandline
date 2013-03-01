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

    public class CapabilitiesFixture
    {
        [Fact]
        public void Should_detect_if_target_has_verbs()
        {
            // Given when
            var capabilities = new Capabilities<OptionsWithVerbs>(new OptionsWithVerbs());

            // Than
            capabilities.AnyVerbs.Should().BeTrue();
        }

        [Fact]
        public void Should_detect_if_target_has_verb_help()
        {
            // Given when
            var capabilities = new Capabilities<OptionsWithVerbs>(new OptionsWithVerbs());

            // Than
            capabilities.HasVerbHelp.Should().BeTrue();
            capabilities.HasHelp.Should().BeFalse();
        }

        [Fact]
        public void Should_detect_if_target_has_help()
        {
            // Given when
            var capabilities = new Capabilities<FakeWithHelp>(new FakeWithHelp());

            // Than
            capabilities.HasHelp.Should().BeTrue();
            capabilities.HasVerbHelp.Should().BeFalse();
        }

        [Fact]
        public void Should_detect_if_target_has_parser_state()
        {
            // Given when
            var capabilities = new Capabilities<OptionsWithVerbsHelp>(new OptionsWithVerbsHelp());

            // Than
            capabilities.HasParserState.Should().BeTrue();
        }
    }
}
