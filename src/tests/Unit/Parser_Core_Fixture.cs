using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

using CommandLine.Tests.Conventions;
using CommandLine.Tests.Extensions;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace CommandLine.Tests.Unit
{
    public class Parser_Core_Fixture : ParserBaseFixture
    {
        [Fact]
        public void Will_throw_Exception_if_Arguments_Array_is_Null()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CommandLine.Parser().ParseArguments<Fake_Simple_Options>(null, () => {}));
        }

        [Theory, AutoData]
        public void Parse_String_Option(string stringValue, Parser sut)
        {
            var arguments = new[] { "-s", stringValue };

            var options = sut.ParseArguments<Fake_Simple_Options>(arguments, () => {});

            Assert.Equal(stringValue, options.StringValue);
        }

        [Theory, AutoData]
        public void Parse_String_Integer_and_Bool_Options(Parser sut, string stringValue, int integerValue)
        {
            var arguments = new[] { "-s", stringValue, string.Concat("-i", integerValue), "--switch" };

            var options = sut.ParseArguments<Fake_Simple_Options>(arguments, () => {});

            Assert.Equal(stringValue, options.StringValue);
            Assert.Equal(integerValue, options.IntegerValue);
            Assert.True(options.BooleanValue);
        }

        [Theory, AutoData]
        public void Parse_Short_Adjacent_Options_with_Double(Parser sut, double doubleValue)
        {
            var expected = doubleValue.WithFractionalDigits();
            var arguments = new[] { "-ca", string.Concat("-d", expected.ToInvariantCulture()) };

            var options = sut.ParseArguments<Fake_Booleans_Options>(arguments, () => {});

            Assert.Equal(expected, options.DoubleValue);
            Assert.True(options.BooleanA);
            Assert.True(options.BooleanC);
        }

        [Theory, AutoData]
        public void Parse_Short_and_Long_Options_with_Double(Parser sut, double doubleValue)
        {
            var expected = doubleValue.WithFractionalDigits();
            var arguments = new[] { "-b", string.Concat("--double=", expected.ToInvariantCulture()) };
            var options = sut.ParseArguments<Fake_Booleans_Options>(arguments, () => {});

            Assert.Equal(expected, options.DoubleValue);
            Assert.True(options.BooleanB);
        }

        [Theory, AutoData]
        public void Parse_using_OptionList(Parser sut, int count, Generator<string> generator)
        {
            var strings = generator.Take(count + 2).Select(a => a.Replace(":", ""));
            var items = string.Join(":", strings.ToArray());
            var arguments = new[] { "-k", items, "-s", "test-file.txt" };

            var options = sut.ParseArguments<Fake_Simple_With_OptionList_Options>(arguments, () => {});

            Assert.Equal(items.Split(':').ToList(), options.SearchKeywords);
        }

        #region #BUG0000
        [Theory, AutoData]
        public void Short_Option_refuses_Equal_Token(Parser sut, int integerValue)
        {
            var result = true;
            var arguments = new[] { string.Concat("-i=", integerValue) };

            var options = sut.ParseArguments<Fake_Simple_Options>(arguments, () => { result = false; });

            Assert.False(result);
        }
        #endregion

        [Theory, AutoData]
        public void Parse_Options_with_Enumeration(Parser sut, FileAttributes enumValue)
        {
            var arguments = new[] { "-s", "data.bin", "-x", enumValue.ToString() };
            var options = sut.ParseArguments<Fake_Simple_With_Enum_Options>(arguments, () => { });

            Assert.Equal(enumValue, options.FileAttributesValue);
        }

        [Theory, ParserWithItalianCultureTestConventions]
        public void Parse_Culture_specific_Number(Parser sut, double doubleValue)
        {
            var expected = doubleValue.WithFractionalDigits();
            var arguments = new[] { "-d", expected.ToItalianCulture() };

            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, options.DoubleValue);
        }

        [Theory, ParserWithItalianCultureTestConventions]
        public void Parse_Culture_specific_Nullable_Number(Parser sut, double doubleValue)
        {
            var expected = doubleValue.WithFractionalDigits();
            var arguments = new[] { "--n-double", expected.ToItalianCulture() };

            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, options.NullableDoubleValue);
        }

        [Theory, AutoData]
        public void Parse_options_with_defaults(Parser sut)
        {
            var arguments = new string[] {};
            var options = sut.ParseArguments<Fake_With_Defaults_Options>(arguments, () => { });

            Assert.Equal("str", options.StringValue);
            Assert.Equal(9, options.IntegerValue);
            Assert.True(options.BooleanValue);
        }

        [Fact]
        public void Parse_options_with_default_array()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_With_Defaults_Array_Options>(new[] { "-y", "4", "5", "6" }, () => { result = false; });

            options.Should().NotBeNull();
            options.StringArrayValue.Should().Equal(new[] { "a", "b", "c" });
            options.IntegerArrayValue.Should().Equal(new[] { 4, 5, 6 });
            options.DoubleArrayValue.Should().Equal(new[] { 1.1, 2.2, 3.3 });
        }

        [Fact]
        public void Parse_options_with_bad_defaults()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments<SimpleOptionsWithBadDefaults>(new string[] { }, () => {}));
        }

        #region #BUG0002
        [Fact]
        public void Parsing_non_existent_short_option_fails_without_throwing_an_exception()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Simple_Options>(new[] { "-x" }, () => { result = false; });
            
            result.Should().BeFalse();
        }

        [Fact]
        public void Parsing_non_existent_long_option_fails_without_throwing_an_exception()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Simple_Options>(
                new[] { "--extend" }, () => { result = false; });

            result.Should().BeFalse();
        }
        #endregion

        #region #REQ0000
        [Fact]
        public void Default_parsing_is_case_sensitive()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<MixedCaseOptions>(
                new[] { "-a", "alfa", "--beta-OPTION", "beta" }, () => { result = false; });

            options.Should().NotBeNull();
            options.AlfaValue.Should().Be("alfa");
            options.BetaValue.Should().Be("beta");
        }

        [Fact]
        public void Using_wrong_case_with_default_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<MixedCaseOptions>(
                new[] { "-A", "alfa", "--Beta-Option", "beta" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Disabling_case_sensitive()
        {
            var parser = new CommandLine.Parser(new ParserSettings(false)); //Ref.: #DGN0001
            var result = true;
            var options = parser.ParseArguments<MixedCaseOptions>(
                new[] { "-A", "alfa", "--Beta-Option", "beta" }, () => { result = false; });

            options.Should().NotBeNull();
            options.AlfaValue.Should().Be("alfa");
            options.BetaValue.Should().Be("beta");
        }
        #endregion

        #region #BUG0003
        [Fact]
        public void Passing_no_value_to_a_string_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Simple_Options>(new[] { "--string" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_byte_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "--byte" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_short_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "--short" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_an_integer_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "--int" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_long_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "--long" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_float_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "--float" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_double_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "--double" }, () => { result = false; });

            result.Should().BeFalse();
        }
        #endregion

        #region #REQ0001
        [Fact]
        public void Allow_single_dash_as_option_input_value()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Simple_Options>(new[] { "--string", "-" }, () => { result = false; });

            options.Should().NotBeNull();
            options.StringValue.Should().Be("-");
        }

        [Fact]
        public void Allow_single_dash_as_non_option_value()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithValueList>(
                new[] { "-sparser.xml", "-", "--switch" }, () => { result = false; });

            options.Should().NotBeNull();
            options.StringValue.Should().Be("parser.xml");
            options.BooleanValue.Should().BeTrue();
            options.Items.Count.Should().Be(1);
            options.Items[0].Should().Be("-");
        }
        #endregion

        #region #BUG0004
        [Fact]
        public void Parse_negative_integer_value()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Simple_Options>(new[] { "-i", "-4096" }, () => { result = false; });

            options.Should().NotBeNull();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle2()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "-i-4096" }, () => { result = false; });

            options.Should().NotBeNull();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle3()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "--int", "-4096" }, () => { result = false; });

            options.Should().NotBeNull();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle4()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "--int=-4096" }, () => { result = false; });

            options.Should().NotBeNull();
            options.IntegerValue.Should().Be(-4096);
        }


        [Fact]
        public void Parse_negative_floating_point_value()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "-d", "-4096.1024" }, () => { result = false; });

            options.Should().NotBeNull();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void Parse_negative_floating_point_value_input_style2()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "-d-4096.1024" }, () => { result = false; });

            options.Should().NotBeNull();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void Parse_negative_floating_point_value_input_style3()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "--double", "-4096.1024" }, () => { result = false; });

            options.Should().NotBeNull();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void Parse_negative_floating_point_value_input_style4()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(new[] { "--double=-4096.1024" }, () => { result = false; });

            options.Should().NotBeNull();
            options.DoubleValue.Should().Be(-4096.1024D);
        }
        #endregion

        #region #BUG0005
        [Fact]
        public void Passing_short_value_to_byte_option_must_fail_gracefully()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(
                new[] { "-b", short.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_integer_value_to_short_option_must_fail_gracefully()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(
                new[] { "-s", int.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_long_value_to_integer_option_must_fail_gracefully()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(
                new[] { "-i", long.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_float_value_to_long_option_must_fail_gracefully()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(
                new[] { "-l", float.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_double_value_to_float_option_must_fail_gracefully()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<Fake_Numbers_Options>(
                new[] { "-f", double.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            result.Should().BeFalse();
        }
        #endregion

        #region ISSUE#15
        /// <summary>
        /// https://github.com/gsscoder/commandline/issues/15
        /// </summary>
        [Fact]
        public void Parser_should_report_missing_value()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<ComplexOptions>(new[] { "-i", "-o" }, () => { result = false; });

            result.Should().BeFalse();
            
            options.LastParserState.Errors.Count.Should().BeGreaterThan(0);
        }
        #endregion
    }
}

