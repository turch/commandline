using System;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public class Parser_Core_Fixture
    {
        [Fact]
        public void Will_throw_exception_if_arguments_array_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CommandLine.Parser().ParseArguments<Fake_Simple_Options>(null, () => {}));
        }

        [Theory, ParserTestConventions]
        public void Parse_String_Option(string stringValue, Parser sut)
        {
            var arguments = new[] { "-s", stringValue };

            var options = sut.ParseArguments<Fake_Simple_Options>(arguments, () => {});

            Assert.Equal(stringValue, options.StringValue);
        }

        [Theory, ParserTestConventions]
        public void Parse_string_integer_and_bool_options(Parser sut, string stringValue, int integerValue)
        {
            var arguments = new[] { "-s", stringValue, string.Concat("-i", integerValue), "--switch" };

            var options = sut.ParseArguments<Fake_Simple_Options>(arguments, () => {});

            Assert.Equal(stringValue, options.StringValue);
            Assert.Equal(integerValue, options.IntegerValue);
            Assert.True(options.BooleanValue);
        }

        [Theory, ParserTestConventions]
        public void Parse_short_adjacent_options_with_double(Parser sut, double doubleValue)
        {
            var expected = doubleValue.AsFractional();
            var arguments = new[] { "-ca", string.Concat("-d", expected.ToInvariantString()) };

            var options = sut.ParseArguments<Fake_Booleans_Options>(arguments, () => {});

            Assert.Equal(expected, options.DoubleValue);
            Assert.True(options.BooleanA);
            Assert.True(options.BooleanC);
        }

        [Theory, ParserTestConventions]
        public void Parse_short_and_long_options_with_double(Parser sut, double doubleValue)
        {
            var expected = doubleValue.AsFractional();
            var arguments = new[] { "-b", string.Concat("--double=", expected.ToInvariantString()) };
            var options = sut.ParseArguments<Fake_Booleans_Options>(arguments, () => {});

            Assert.Equal(expected, options.DoubleValue);
            Assert.True(options.BooleanB);
        }

        [Theory, ParserTestConventions]
        public void Parse_using_option_list(Parser sut, int count, Generator<string> generator)
        {
            var strings = generator.Take(count + 2).Select(a => a.Replace(":", ""));
            var items = string.Join(":", strings.ToArray());
            var arguments = new[] { "-k", items, "-s", "test-file.txt" };

            var options = sut.ParseArguments<Fake_Simple_With_OptionList_Options>(arguments, () => {});

            Assert.Equal(items.Split(':').ToList(), options.SearchKeywords);
        }

        #region #BUG0000
        [Theory, ParserTestConventions]
        public void Short_option_refuses_equal_token(Parser sut, int integerValue)
        {
            var result = true;
            var arguments = new[] { string.Concat("-i=", integerValue) };

            var options = sut.ParseArguments<Fake_Simple_Options>(arguments, () => { result = false; });

            Assert.False(result);
        }
        #endregion

        [Theory, ParserTestConventions]
        public void Parse_options_with_enumeration(Parser sut, FileAttributes enumValue)
        {
            var arguments = new[] { "-s", "data.bin", "-x", enumValue.ToString() };
            var options = sut.ParseArguments<Fake_Simple_With_Enum_Options>(arguments, () => { });

            Assert.Equal(enumValue, options.FileAttributesValue);
        }

        [Theory, ParserWithItalianCultureTestConventions]
        public void Parse_culture_specific_number(Parser sut, double doubleValue)
        {
            var expected = doubleValue.AsFractional();
            var arguments = new[] { "-d", expected.ToItalianCultureString() };

            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, options.DoubleValue);
        }

        [Theory, ParserWithItalianCultureTestConventions]
        public void Parse_culture_specific_nullable_number(Parser sut, double doubleValue)
        {
            var expected = doubleValue.AsFractional();
            var arguments = new[] { "--n-double", expected.ToItalianCultureString() };

            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, options.NullableDoubleValue);
        }

        [Theory, ParserTestConventions]
        public void Parse_options_with_defaults(Parser sut)
        {
            var arguments = new string[] {};
            var options = sut.ParseArguments<Fake_With_Defaults_Options>(arguments, () => { });

            Assert.Equal("str", options.StringValue);
            Assert.Equal(9, options.IntegerValue);
            Assert.True(options.BooleanValue);
        }

        [Theory, ParserTestConventions]
        public void Parse_options_with_default_array(Parser sut)
        {
            var arguments = new string[] {};

            var options = sut.ParseArguments<Fake_With_Defaults_Array_Options>(arguments, () => { });

            Assert.Equal(new[] { "a", "b", "c" }, options.StringArrayValue);
            Assert.Equal(new[] { 1, 2, 3 }, options.IntegerArrayValue);
            Assert.Equal(new[] { 1.1d, 2.2d, 3.3d }, options.DoubleArrayValue);
        }

        [Fact]
        public void Will_throw_exception_with_options_having_bad_defaults()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments<Fake_With_Bad_Defaults_Options>(
                    new string[] { }, () => {}));
        }

        #region #BUG0002
        [Theory, ParserTestConventions]
        public void Parsing_non_existent_short_option_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Simple_Options>(
                new[] { "-x" }, () => { result = false; });
            
            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Parsing_non_existent_long_option_dails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Simple_Options>(
                new[] { "--extend" }, () => { result = false; });

            Assert.False(result);
        }
        #endregion

        #region #REQ0000
        [Theory, AutoData]
        public void By_default_parser_is_case_sensitive(Parser sut)
        {
            var options = sut.ParseArguments<Fake_Mixed_Case_Options>(
                new[] { "-a", "alfa", "--beta-OPTION", "beta" }, () => { });

            Assert.Equal("alfa", options.AlfaValue);
            Assert.Equal("beta", options.BetaValue);
        }

        [Theory, AutoData]
        public void By_default_parser_is_case_sensitive__reverse_proof(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Mixed_Case_Options>(
                new[] { "-A", "alfa", "--Beta-Option", "beta" }, () => { result = false; });

            Assert.False(result);
        }

        [Fact]
        public void Case_sensitiveness_can_be_disabled()
        {
            var parser = new CommandLine.Parser(new ParserSettings(false)); //Ref.: #DGN0001

            var options = parser.ParseArguments<Fake_Mixed_Case_Options>(
                new[] { "-A", "alfa", "--Beta-Option", "beta" }, () => {});

            Assert.Equal("alfa", options.AlfaValue);
            Assert.Equal("beta", options.BetaValue);
        }
        #endregion

        #region #BUG0003
        [Theory, ParserTestConventions]
        public void Not_passing_a_value_to_a_string_type_long_option_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Simple_Options>(
                new[] { "--string" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Not_passing_a_value_to_a_byte_type_long_option_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--byte" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Not_passing_a_value_to_a_short_type_long_option_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--short" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Not_passing_a_value_to_an_integer_type_long_option_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--int" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Not_passing_a_value_a_Long_type_long_option_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--long" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Not_passing_a_value_a_float_type_long_option_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--float" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Not_passing_a_value_a_double_type_long_option_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--double" }, () => { result = false; });

            Assert.False(result);
        }
        #endregion

        #region #REQ0001
        [Theory, ParserTestConventions]
        public void Allow_single_dash_as_option_input_value(Parser sut)
        {
            var options = sut.ParseArguments<Fake_Simple_Options>(new[] { "--string", "-" }, () => { });

            Assert.Equal("-", options.StringValue);
        }

        [Theory, ParserTestConventions]
        public void Allow_single_dash_as_non_option_value(Parser sut)
        {
            var options = sut.ParseArguments<SimpleOptionsWithValueList>(
                new[] { "-sparser.xml", "-", "--switch" }, () => {});

            Assert.Equal("parser.xml", options.StringValue);
            Assert.True(options.BooleanValue);
            Assert.Equal(1, options.Items.Count);
            Assert.Contains("-", options.Items);
        }
        #endregion

        #region #BUG0004
        [Theory, ParserTestConventions]
        public void Parse_negative_integer_value(Parser sut, int integerValue)
        {
            var expected = (0 - integerValue);
            var arguments = new[] { "-i", expected.ToInvariantString() };
            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, expected);
        }

        [Theory, ParserTestConventions]
        public void Parse_negative_integer_value__adjacent_style(Parser sut, int integerValue)
        {
            var expected = (0 - integerValue);
            var arguments = new[] { string.Concat("-i", expected.ToInvariantString()) };
            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, options.IntegerValue);
        }

        [Theory, ParserTestConventions]
        public void Parse_negative_integer_value__long_option(Parser sut, int integerValue)
        {
            var expected = (0 - integerValue);
            var arguments = new[] { "--int", expected.ToInvariantString() };
            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, options.IntegerValue);
        }

        [Theory, ParserTestConventions]
        public void Parse_negative_integer_value__long_option__equal(Parser sut, int integerValue)
        {
            var expected = (0 - integerValue);
            var arguments = new[] { string.Concat("--int=", expected.ToInvariantString()) };
            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, options.IntegerValue);
        }


        [Theory, ParserTestConventions]
        public void Parse_negative_floating_point_value(Parser sut, float floatValue)
        {
            var expected = 0f - floatValue.AsFractional();
            var arguments = new[] { "-f", expected.ToInvariantString() };
            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => {});

            Assert.Equal(expected, options.FloatValue.AsRounded());
        }

        [Theory, ParserTestConventions]
        public void Parse_negative_floating_point_value__adjacent(Parser sut, float floatValue)
        {
            var expected = 0 - floatValue.AsFractional();
            var arguments = new[] { string.Concat("-f", expected.ToInvariantString()) };
            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, options.FloatValue.AsRounded());
        }

        [Theory, ParserTestConventions]
        public void Parse_negative_floating_point_value__long_option(Parser sut, float floatValue)
        {
            var expected = 0 - floatValue.AsFractional();
            var arguments = new[] { "--float", expected.ToInvariantString() };
            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, options.FloatValue.AsRounded());
        }

        [Theory, ParserTestConventions]
        public void Parse_negative_floating_point_value__long_option__equal(Parser sut, float floatValue)
        {
            var expected = 0 - floatValue.AsFractional();
            var arguments = new[] { string.Concat("--float=", expected.ToInvariantString()) };
            var options = sut.ParseArguments<Fake_Numbers_Options>(arguments, () => { });

            Assert.Equal(expected, options.FloatValue.AsRounded());
        }
        #endregion

        #region #BUG0005
        [Theory, ParserTestConventions]
        public void Passing_short_value_to_byte_option_must_fail_gracefully(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(
                new[] { "-b", short.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Passing_integer_value_to_short_option_must_fail_gracefully(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(
                new[] { "-s", int.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Passing_long_value_to_integer_option_must_fail_gracefully(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(
                new[] { "-i", long.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Passing_float_value_to_long_option_must_fail_gracefully(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(
                new[] { "-l", float.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, ParserTestConventions]
        public void Passing_double_value_to_float_option_must_fail_gracefully(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(
                new[] { "-f", double.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            Assert.False(result);
        }
        #endregion

        #region ISSUE#15
        /// <summary>
        /// https://github.com/gsscoder/commandline/issues/15
        /// </summary>
        [Theory, ParserTestConventions]
        public void Parser_should_report_missing_value(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Complex_Options>(new[] { "-i", "-o" }, () => { result = false; });

            Assert.False(result);
            Assert.True(options.LastParserState.Errors.Count > 0);
        }
        #endregion

        #region ISSUE#64
        /// <summary>
        /// https://github.com/gsscoder/commandline/issues/64
        /// </summary>
        [Theory, ParserTestConventions]
        public void By_default_correct_but_undefined_long_options_should_be_reported_as_errors(Parser sut)
        {
            var result = true;
            var arguments = new[]
                {
                    "-s", "i_am_defined",
                    "--instead_i_am_undefined"
                };
            var options = sut.ParseArguments<Fake_Simple_With_ParserState_Options>(arguments, () => { result = false; });

            result.Should().BeFalse();
            options.ParserState.Errors.Should().HaveCount(c => c == 1);
            options.ParserState.Errors.ElementAt(0).BadOption.LongName.Should().Be("instead_i_am_undefined");
            options.ParserState.Errors.ElementAt(0).ViolatesSpecification.Should().BeTrue();
        }

        [Theory, ParserTestConventions]
        public void By_default_correct_but_undefined_short_options_should_be_reported_as_errors(Parser sut)
        {
            var result = true;
            var arguments = new[]
                {
                    "-s", "i_am_defined",
                    "-w"
                };
            var options = sut.ParseArguments<Fake_Simple_With_ParserState_Options>(arguments, () => { result = false; });

            result.Should().BeFalse();
            options.ParserState.Errors.Should().HaveCount(c => c == 1);
            options.ParserState.Errors.ElementAt(0).BadOption.ShortName.Should().Be('w');
            options.ParserState.Errors.ElementAt(0).ViolatesSpecification.Should().BeTrue();
        }
        #endregion
    }
}