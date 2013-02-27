using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Attributes
{
    public class ValueOptionAttributeFixture : BaseFixture
    {
        [Fact]
        public void Index_implicit_by_declaration_order()
        {
            string[] args = "foo bar".Split();

            var options = CommandLine.Parser.Default.ParseArguments<OptionsWithValueOptionImplicitIndex>(args);

            options.Should().NotBeNull();
            options.A.ShouldBeEquivalentTo("foo");
            options.B.ShouldBeEquivalentTo("bar");
            options.C.Should().BeNull();
        }

        [Fact]
        public void Index_explicitly_set_on_value_option()
        {
            string[] args = "foo bar".Split();

            var options = CommandLine.Parser.Default.ParseArguments<OptionsWithValueOptionExplicitIndex>(args);

            options.Should().NotBeNull();
            options.A.Should().BeNull();
            options.B.ShouldBeEquivalentTo("bar");
            options.C.ShouldBeEquivalentTo("foo");
        }
    }
}
