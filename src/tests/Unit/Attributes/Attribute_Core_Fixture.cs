using System;
using Xunit;

namespace CommandLine.Tests.Unit.Attributes
{
    public class Attribute_Core_Fixture
    {
        class CustomOptionAttribute : BaseOptionAttribute
        {
            public CustomOptionAttribute(string longName)
                : base(null, longName)
            {
            }

            public CustomOptionAttribute(char shortName, string longName)
                : base(shortName, longName)
            {
            }
        }

        #region API change 01
        [Fact]
        public void Short_name_with_line_terminator_throws_exception()
        {
            Assert.Throws<ArgumentException>(() =>
                new OptionAttribute('\n'));
        }

        [Fact]
        public void Short_name_with_line_terminator_throws_exception_2()
        {
            Assert.Throws<ArgumentException>(() =>
                new OptionAttribute('\r'));
        }

        [Fact]
        public void Short_name_with_white_space_throws_exception()
        {
            Assert.Throws<ArgumentException>(() =>
                new OptionAttribute(' '));
        }

        [Fact]
        public void Short_name_with_white_space_throws_exception_2()
        {
            Assert.Throws<ArgumentException>(() =>
                new OptionAttribute('\t'));
        }
        #endregion

        [Fact]
        public void All_options_allow_one_character_in_short_name()
        {
            new OptionAttribute('o', null);
            new OptionListAttribute('l', null);
            new HelpOptionAttribute('?', null);
            new CustomOptionAttribute('c', null);
        }

        [Fact]
        public void All_options_allow_null_value_in_short_name()
        {
            new OptionAttribute("option-attr");
            new OptionListAttribute("option-list-attr");
            new HelpOptionAttribute("help-attr");
            new CustomOptionAttribute("custom-attr");
        }
    }
}
