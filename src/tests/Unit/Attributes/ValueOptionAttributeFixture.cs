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
            var args = "foo bar".Split();
            var result = true;

            var options = CommandLine.Parser.Default.ParseArguments<OptionsWithValueOptionImplicitIndex>(
                args, () => { result = false; });

            result.Should().BeTrue();
            options.Should().NotBeNull();
            options.A.ShouldBeEquivalentTo("foo");
            options.B.ShouldBeEquivalentTo("bar");
            options.C.Should().BeNull();
        }

        [Fact]
        public void Index_explicitly_set_on_value_option()
        {
            var args = "foo bar".Split();
            var result = true;

            var options = CommandLine.Parser.Default.ParseArguments<OptionsWithValueOptionExplicitIndex>
                (args, () => { result = false; });

            result.Should().BeTrue();
            options.Should().NotBeNull();
            options.A.Should().BeNull();
            options.B.ShouldBeEquivalentTo("bar");
            options.C.ShouldBeEquivalentTo("foo");
        }
    }
}
