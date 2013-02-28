#region License
//
// Command Line Library: ParserFixture.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2013 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using CommandLine.Tests.Fakes;
using Xunit;
using FluentAssertions;
#endregion

namespace CommandLine.Tests.Unit.Parser
{
    public class ParserFixture : ParserBaseFixture
    {
        [Fact]
        public void Will_throw_exception_if_arguments_array_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CommandLine.Parser().ParseArguments<SimpleOptions>(null, () => {}));
        }

        //[Fact]
        //public void Will_throw_exception_if_options_instance_is_null()
        //{
        //    Assert.Throws<ArgumentNullException>(
        //        () => new CommandLine.Parser().ParseArguments(new[] {}, null));
        //}

        [Fact]
        public void Parse_string_option()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptions>(new[] { "-s", "something" }, () => { result = false; });

            options.Should().NotBeNull();
            options.StringValue.Should().Be("something");
            Console.WriteLine(options);
        }

        [Fact]
        public void Parse_string_integer_bool_options()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptions>(
                    new[] { "-s", "another string", "-i100", "--switch" }, () => { result = false; });

            options.Should().NotBeNull();
            options.StringValue.Should().Be("another string");
            options.IntegerValue.Should().Be(100);
            options.BooleanValue.Should().BeTrue();
            Console.WriteLine(options);
        }

        [Fact]
        public void Parse_short_adjacent_options()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<BooleanSetOptions>(new[] { "-ca", "-d65" }, () => { result = false; });

            options.Should().NotBeNull();
            options.BooleanThree.Should().BeTrue();
            options.BooleanOne.Should().BeTrue();
            options.BooleanTwo.Should().BeFalse();
            options.NonBooleanValue.Should().Be(65D);
            Console.WriteLine(options);
        }

        [Fact]
        public void Parse_short_long_options()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<BooleanSetOptions>(new[] { "-b", "--double=9" }, () => { result = false; });

            options.Should().NotBeNull();
            options.BooleanTwo.Should().BeTrue();
            options.BooleanOne.Should().BeFalse();
            options.BooleanThree.Should().BeFalse();
            options.NonBooleanValue.Should().Be(9D);
            Console.WriteLine(options);
        }
 
        [Fact]
        public void Parse_option_list()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithOptionList>(new[] {
                                "-k", "string1:stringTwo:stringIII", "-s", "test-file.txt" }, () => { result = false; });

            options.Should().NotBeNull();
            options.SearchKeywords[0].Should().Be("string1");
            Console.WriteLine(options.SearchKeywords[0]);
            options.SearchKeywords[1].Should().Be("stringTwo");
            Console.WriteLine(options.SearchKeywords[1]);
            options.SearchKeywords[2].Should().Be("stringIII");
            Console.WriteLine(options.SearchKeywords[2]);
            options.StringValue.Should().Be("test-file.txt");
            Console.WriteLine(options.StringValue);
        }

        #region #BUG0000
        [Fact]
        public void Short_option_refuses_equal_token()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptions>(new[] { "-i=10" }, () => { result = false; });
            result.Should().BeFalse();
            Console.WriteLine(options);
        }
        #endregion

        [Fact]
        public void Parse_enum_options()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithEnum>(new[] { "-s", "data.bin", "-a", "ReadWrite" }, () => { result = false; });

            options.Should().NotBeNull();
            options.StringValue.Should().Be("data.bin");
            options.FileAccess.Should().Be(FileAccess.ReadWrite);
            Console.WriteLine(options);
        }

        [Fact]
        public void Parse_culture_specific_number()
        {
            var parser = new CommandLine.Parser(with => with.ParsingCulture = new CultureInfo("it-IT"));
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "-d", "10,986" }, () => { result = false; });

            options.Should().NotBeNull();
            options.DoubleValue.Should().Be(10.986D);
        }

        [Fact]
        public void Parse_culture_specific_nullable_number()
        {
            var result = true;
            var actualCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var parser = new CommandLine.Parser();
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--n-double", "12,32982" }, () => { result = false; });

            options.Should().NotBeNull();
            options.NullableDoubleValue.Should().Be(12.32982D);

            Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        [Fact]
        public void Parse_options_with_defaults()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithDefaults>(new string[] { }, () => { result = false; });

            options.Should().NotBeNull();
            options.StringValue.Should().Be("str");
            options.IntegerValue.Should().Be(9);
            options.BooleanValue.Should().BeTrue();
        }

        [Fact]
        public void Parse_options_with_default_array()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithDefaultArray>(new[] { "-y", "4", "5", "6" }, () => { result = false; });

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
            var options = parser.ParseArguments<SimpleOptions>(new[] { "-x" }, () => { result = false; });
            
            result.Should().BeFalse();
        }

        [Fact]
        public void Parsing_non_existent_long_option_fails_without_throwing_an_exception()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptions>(
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
            var options = parser.ParseArguments<SimpleOptions>(new[] { "--string" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_byte_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--byte" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_short_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--short" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_an_integer_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--int" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_long_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--long" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_float_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--float" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_double_type_long_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--double" }, () => { result = false; });

            result.Should().BeFalse();
        }
        #endregion

        #region #REQ0001
        [Fact]
        public void Allow_single_dash_as_option_input_value()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptions>(new[] { "--string", "-" }, () => { result = false; });

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
            var options = parser.ParseArguments<SimpleOptions>(new[] { "-i", "-4096" }, () => { result = false; });

            options.Should().NotBeNull();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle2()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "-i-4096" }, () => { result = false; });

            options.Should().NotBeNull();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle3()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--int", "-4096" }, () => { result = false; });

            options.Should().NotBeNull();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle4()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--int=-4096" }, () => { result = false; });

            options.Should().NotBeNull();
            options.IntegerValue.Should().Be(-4096);
        }


        [Fact]
        public void Parse_negative_floating_point_value()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "-d", "-4096.1024" }, () => { result = false; });

            options.Should().NotBeNull();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void Parse_negative_floating_point_value_input_style2()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "-d-4096.1024" }, () => { result = false; });

            options.Should().NotBeNull();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void Parse_negative_floating_point_value_input_style3()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--double", "-4096.1024" }, () => { result = false; });

            options.Should().NotBeNull();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void Parse_negative_floating_point_value_input_style4()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(new[] { "--double=-4096.1024" }, () => { result = false; });

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
            var options = parser.ParseArguments<NumberSetOptions>(
                new[] { "-b", short.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_integer_value_to_short_option_must_fail_gracefully()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(
                new[] { "-s", int.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_long_value_to_integer_option_must_fail_gracefully()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(
                new[] { "-i", long.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_float_value_to_long_option_must_fail_gracefully()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(
                new[] { "-l", float.MaxValue.ToString(CultureInfo.InvariantCulture) }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_double_value_to_float_option_must_fail_gracefully()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NumberSetOptions>(
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

