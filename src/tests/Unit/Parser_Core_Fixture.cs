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

        [Theory, AutoData]
        public void Parse_Options_with_Default_Array(Parser sut)
        {
            var arguments = new string[] {};

            var options = sut.ParseArguments<Fake_With_Defaults_Array_Options>(arguments, () => { });

            Assert.Equal(new[] { "a", "b", "c" }, options.StringArrayValue);
            Assert.Equal(new[] { 1, 2, 3 }, options.IntegerArrayValue);
            Assert.Equal(new[] { 1.1d, 2.2d, 3.3d }, options.DoubleArrayValue);
        }

        [Fact]
        public void Will_throw_Exception_with_Options_having_Bad_Defaults()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments<Fake_With_Bad_Defaults_Options>(
                    new string[] { }, () => {}));
        }

        #region #BUG0002
        [Theory, AutoData]
        public void Parsing_Non_Existent_Short_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Simple_Options>(
                new[] { "-x" }, () => { result = false; });
            
            Assert.False(result);
        }

        [Theory, AutoData]
        public void Parsing_Non_Existent_Long_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Simple_Options>(
                new[] { "--extend" }, () => { result = false; });

            Assert.False(result);
        }
        #endregion

        #region #REQ0000
        [Theory, AutoData]
        public void By_Default_Parser_Is_Case_Sensitive(Parser sut)
        {
            var options = sut.ParseArguments<Fake_Mixed_Case_Options>(
                new[] { "-a", "alfa", "--beta-OPTION", "beta" }, () => { });

            Assert.Equal("alfa", options.AlfaValue);
            Assert.Equal("beta", options.BetaValue);
        }

        [Theory, AutoData]
        public void By_Default_Parser_Is_Case_Sensitive__reverse_proof(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Mixed_Case_Options>(
                new[] { "-A", "alfa", "--Beta-Option", "beta" }, () => { result = false; });

            Assert.False(result);
        }

        [Fact]
        public void Case_Sensitiveness_can_be_Disabled()
        {
            var parser = new CommandLine.Parser(new ParserSettings(false)); //Ref.: #DGN0001

            var options = parser.ParseArguments<Fake_Mixed_Case_Options>(
                new[] { "-A", "alfa", "--Beta-Option", "beta" }, () => {});

            Assert.Equal("alfa", options.AlfaValue);
            Assert.Equal("beta", options.BetaValue);
        }
        #endregion

        #region #BUG0003
        [Theory, AutoData]
        public void Not_Passing_a_Value_to_a_String_type_Long_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Simple_Options>(
                new[] { "--string" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, AutoData]
        public void Not_Passing_a_Value_to_a_Byte_type_Long_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--byte" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, AutoData]
        public void Not_Passing_a_Value_to_a_Short_type_Long_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--short" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, AutoData]
        public void Not_Passing_a_Value_to_an_Integer_type_Long_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--int" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, AutoData]
        public void Not_Passing_a_Value_a_Long_type_Long_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--long" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, AutoData]
        public void Not_Passing_a_Value_a_Float_type_Long_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--float" }, () => { result = false; });

            Assert.False(result);
        }

        [Theory, AutoData]
        public void Not_Passing_a_Value_a_Double_type_Long_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Numbers_Options>(new[] { "--double" }, () => { result = false; });

            Assert.False(result);
        }
        #endregion

        #region #REQ0001
        [Theory, AutoData]
        public void Allow_Single_Dash_as_Option_Input_Value(Parser sut)
        {
            var options = sut.ParseArguments<Fake_Simple_Options>(new[] { "--string", "-" }, () => { });

            Assert.Equal("-", options.StringValue);
        }

        [Theory, AutoData]
        public void Allow_Single_Dash_as_Non_Option_Value(Parser sut)
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

