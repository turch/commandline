#region License
//
// Command Line Library: OptionArrayAttributeParsingFixture.cs
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
using System.Threading;
using System.Globalization;
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Fakes;
#endregion

namespace CommandLine.Tests.Unit.Attributes
{
    // TODO: this is one of the oldest test class and need to be heavily refactored
    public class Attribute_OptionArray_Fixture : ParserBaseFixture
    {
        [Fact]
        public void Parse_string_array_option_using_short_name()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-z", "alfa", "beta", "gamma" }, () => result = false);

            result.Should().BeTrue();
            //base.ElementsShouldBeEqual(new[] { "alfa", "beta", "gamma" }, options.StringArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "alfa", "beta", "gamma" });
        }

        [Fact]
        public void Parse_string_array_option_using_long_name()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "--strarr", "alfa", "beta", "gamma" }, () => result = false);

            result.Should().BeTrue();
            //base.ElementsShouldBeEqual(new[] { "alfa", "beta", "gamma" }, options.StringArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "alfa", "beta", "gamma" });
        }

        [Fact]
        public void Parse_string_array_option_using_short_name_with_value_adjacent()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-zapple", "kiwi" }, () => result = false);

            result.Should().BeTrue();
            //base.ElementsShouldBeEqual(new[] { "apple", "kiwi" }, options.StringArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "apple", "kiwi" });
        }

        [Fact]
        public void Parse_string_array_option_using_long_name_with_equal_sign()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "--strarr=apple", "kiwi" }, () => result = false);

            result.Should().BeTrue();
            //base.ElementsShouldBeEqual(new[] { "apple", "kiwi" }, options.StringArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "apple", "kiwi" });
        }

        [Fact]
        public void Parse_string_array_option_using_short_name_and_string_option_after()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-z", "one", "two", "three", "-s", "after" }, () => result = false);

            result.Should().BeTrue();
            //base.ElementsShouldBeEqual(new[] { "one", "two", "three" }, options.StringArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "one", "two", "three" });
            options.StringValue.Should().Be("after");
        }

        [Fact]
        public void Parse_string_array_option_using_short_name_and_string_option_before()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-s", "before", "-z", "one", "two", "three" }, () => result = false);

            result.Should().BeTrue();
            options.StringValue.Should().Be("before");
            //base.ElementsShouldBeEqual(new[] { "one", "two", "three" }, options.StringArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "one", "two", "three" });
        }

        [Fact]
        public void Parse_string_array_option_using_short_name_with_options_before_and_after()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] {
                "-i", "191919", "-z", "one", "two", "three", "--switch", "--string=near" }, () => result = false);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(191919);
            //base.ElementsShouldBeEqual(new[] { "one", "two", "three" }, options.StringArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "one", "two", "three" });
            options.BooleanValue.Should().BeTrue();
            options.StringValue.Should().Be("near");
        }

        [Fact]
        public void Parse_string_array_option_using_long_name_with_value_list()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArrayAndValueList>(new[] {
                "-shere", "-i999", "--strarr=0", "1", "2", "3", "4", "5", "6", "7", "8", "9" , "--switch", "f1.xml", "f2.xml"}, () => result = false);

            result.Should().BeTrue();
            options.StringValue.Should().Be("here");
            options.IntegerValue.Should().Be(999);
            //base.ElementsShouldBeEqual(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, options.StringArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            options.BooleanValue.Should().BeTrue();
            //base.ElementsShouldBeEqual(new[] { "f1.xml", "f2.xml" }, options.Items);
            options.Items.Should().ContainInOrder(new[] { "f1.xml", "f2.xml" });
        }

        [Fact]
        public void Passing_no_value_to_a_string_array_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-z" }, () => result = false);

            result.Should().BeFalse();

            var result2 = true;
            options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "--strarr" }, () => result2 = false);

            result2.Should().BeFalse();
        }

        /****************************************************************************************************/

        [Fact]
        public void Parse_integer_array_option_using_short_name()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-y", "1", "2", "3" }, () => result = false);

            result.Should().BeTrue();
            //base.ElementsShouldBeEqual(new int[] { 1, 2, 3 }, options.IntegerArrayValue);
            options.IntegerArrayValue.Should().ContainInOrder(new[] { 1, 2, 3 });
        }

        [Fact]
        public void Passing_bad_value_to_an_integer_array_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-y", "one", "2", "3" }, () => result = false);

            result.Should().BeFalse();

            parser = new CommandLine.Parser();
            var result2 = true;
            options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-yone", "2", "3" }, () => result2 = false);

            result2.Should().BeFalse();

            parser = new CommandLine.Parser();
            var result3 = true;
            options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "--intarr", "1", "two", "3" }, () => result3 = false);

            result3.Should().BeFalse();

            parser = new CommandLine.Parser();
            var result4 = true;
            options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "--intarr=1", "2", "three" }, () => result4 = false);

            result4.Should().BeFalse();
        }


        [Fact]
        public void Passing_no_value_to_an_integer_array_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-y" }, () => result = false);

            result.Should().BeFalse();

            var result2 = true;
            options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "--intarr" }, () => result2 = false);

            result2.Should().BeFalse();
        }

        /****************************************************************************************************/

        [Fact]
        public void Parse_double_array_option_using_short_name()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-q", "0.1", "2.3", "0.9" }, () => result = false);

            result.Should().BeTrue();
            //base.ElementsShouldBeEqual(new double[] { .1, 2.3, .9 }, options.DoubleArrayValue);
            options.DoubleArrayValue.Should().ContainInOrder(new[] { .1d, 2.3d, .9d });
        }

        /****************************************************************************************************/

        [Fact]
        public void Parse_different_arrays_together__combination_one()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] {
                "-z", "one", "two", "three", "four",
                "-y", "1", "2", "3", "4",
                "-q", "0.1", "0.2", "0.3", "0.4"
            }, () => result = false);

            result.Should().BeTrue();
            //base.ElementsShouldBeEqual(new[] { "one", "two", "three", "four" }, options.StringArrayValue);
            //base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            //base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "one", "two", "three", "four" });
            options.IntegerArrayValue.Should().ContainInOrder(new[] { 1, 2, 3, 4 });
            options.DoubleArrayValue.Should().ContainInOrder(new[] { .1d, .2d, .3d, .4d });

            parser = new CommandLine.Parser();
            var result2 = true;
            options = parser.ParseArguments<SimpleOptionsWithArray>(new[] {
                "-y", "1", "2", "3", "4",
                "-z", "one", "two", "three", "four",
                "-q", "0.1", "0.2", "0.3", "0.4"
            }, () => result2 = false);

            result2.Should().BeTrue();
            //base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            //base.ElementsShouldBeEqual(new[] { "one", "two", "three", "four" }, options.StringArrayValue);
            //base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "one", "two", "three", "four" });
            options.IntegerArrayValue.Should().ContainInOrder(new[] { 1, 2, 3, 4 });
            options.DoubleArrayValue.Should().ContainInOrder(new[] { .1d, .2d, .3d, .4d });

            parser = new CommandLine.Parser();
            var result3 = true;
            options = parser.ParseArguments<SimpleOptionsWithArray>(new[] {
                "-q", "0.1", "0.2", "0.3", "0.4",
                "-y", "1", "2", "3", "4",
                "-z", "one", "two", "three", "four"
            }, () => result3 = false);

            result3.Should().BeTrue();
            //base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            //base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            //base.ElementsShouldBeEqual(new[] { "one", "two", "three", "four" }, options.StringArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "one", "two", "three", "four" });
            options.IntegerArrayValue.Should().ContainInOrder(new[] { 1, 2, 3, 4 });
            options.DoubleArrayValue.Should().ContainInOrder(new[] { .1d, .2d, .3d, .4d });
        }

        [Fact]
        public void Parse_different_arrays_together__combination_two()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] {
                "-z", "one", "two", "three", "four",
                "-y", "1", "2", "3", "4",
                "-q", "0.1", "0.2", "0.3", "0.4",
                "--string=after"
            }, () => result = false);

            result.Should().BeTrue();
            //base.ElementsShouldBeEqual(new[] { "one", "two", "three", "four" }, options.StringArrayValue);
            //base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            //base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "one", "two", "three", "four" });
            options.IntegerArrayValue.Should().ContainInOrder(new[] { 1, 2, 3, 4 });
            options.DoubleArrayValue.Should().ContainInOrder(new[] { .1d, .2d, .3d, .4d });
            options.StringValue.Should().Be("after");

            parser = new CommandLine.Parser();
            var result2 = true;
            options = parser.ParseArguments<SimpleOptionsWithArray>(new[] {
                "--string", "before",
                "-y", "1", "2", "3", "4",
                "-z", "one", "two", "three", "four",
                "-q", "0.1", "0.2", "0.3", "0.4"
            }, () => result2 = false);

            result2.Should().BeTrue();
            options.StringValue.Should().Be("before");
            //base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            //base.ElementsShouldBeEqual(new[] { "one", "two", "three", "four" }, options.StringArrayValue);
            //base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "one", "two", "three", "four" });
            options.IntegerArrayValue.Should().ContainInOrder(new[] { 1, 2, 3, 4 });
            options.DoubleArrayValue.Should().ContainInOrder(new[] { .1d, .2d, .3d, .4d });

            parser = new CommandLine.Parser();
            var result3 = true;
            options = parser.ParseArguments<SimpleOptionsWithArray>(new[] {
                "-q", "0.1", "0.2", "0.3", "0.4",
                "-y", "1", "2", "3", "4",
                "-s", "near-the-center",
                "-z", "one", "two", "three", "four"
            }, () => result3 = false);

            result3.Should().BeTrue();
            //base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            //base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            options.StringValue.Should().Be("near-the-center");
            //base.ElementsShouldBeEqual(new[] { "one", "two", "three", "four" }, options.StringArrayValue);
            options.StringArrayValue.Should().ContainInOrder(new[] { "one", "two", "three", "four" });
            options.IntegerArrayValue.Should().ContainInOrder(new[] { 1, 2, 3, 4 });
            options.DoubleArrayValue.Should().ContainInOrder(new[] { .1d, .2d, .3d, .4d });

            parser = new CommandLine.Parser();
            var result4 = true;
            options = parser.ParseArguments<SimpleOptionsWithArray>(new[] {
                "--switch",
                "-z", "one", "two", "three", "four",
                "-y", "1", "2", "3", "4",
                "-i", "1234",
                "-q", "0.1", "0.2", "0.3", "0.4",
                "--string", "I'm really playing with the parser!"
            }, () => result4 = false);

            result4.Should().BeTrue();
            options.BooleanValue.Should().BeTrue();
            //base.ElementsShouldBeEqual(new[] { "one", "two", "three", "four" }, options.StringArrayValue);
            //base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            options.IntegerValue.Should().Be(1234);
            //base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            options.StringValue.Should().Be("I'm really playing with the parser!");

            options.StringArrayValue.Should().ContainInOrder(new[] { "one", "two", "three", "four" });
            options.IntegerArrayValue.Should().ContainInOrder(new[] { 1, 2, 3, 4 });
            options.DoubleArrayValue.Should().ContainInOrder(new[] { .1d, .2d, .3d, .4d });
        }

        /****************************************************************************************************/

        [Fact]
        public void Will_throw_exception_if_option_array_attribute_bound_to_string_with_short_name()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments<SimpleOptionsWithBadOptionArray>(new[] { "-v", "a", "b", "c" }, () => {}));
        }

        [Fact]
        public void Will_throw_exception_if_option_array_attribute_bound_to_string_with_long_name()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments<SimpleOptionsWithBadOptionArray>(new[] { "--bstrarr", "a", "b", "c" }, () => { }));
        }

        [Fact]
        public void Will_throw_exception_if_option_array_attribute_bound_to_integer_with_short_name()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments<SimpleOptionsWithBadOptionArray>(new[] { "-w", "1", "2", "3" }, () => { }));
        }

        [Fact]
        public void Will_throw_exception_if_option_array_attribute_bound_to_integer_with_long_name()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments<SimpleOptionsWithBadOptionArray>(new[] { "--bintarr", "1", "2", "3" }, () => { }));
        }

        [Fact]
        public void Parse_culture_specific_number()
        {
            var parser = new CommandLine.Parser(new ParserSettings { ParsingCulture = new CultureInfo("it-IT") });
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithArray>(new[] { "-q", "1,2", "1,23", "1,234" }, () => result = false);

            result.Should().BeTrue();
            //base.ElementsShouldBeEqual(new double[] { 1.2, 1.23, 1.234 }, options.DoubleArrayValue);
            options.DoubleArrayValue.Should().ContainInOrder(new[] { 1.2d, 1.23d, 1.234d });
        }

        /****************************************************************************************************/

        [Fact]
        public void Parse_two_uint_consecutive_array()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = CommandLine.Parser.Default.ParseArguments<OptionsWithUIntArray>(new[] {
                "--somestr", "just a string",
                "--arrayone", "10", "20", "30", "40",
                "--arraytwo", "11", "22", "33", "44",
                "--somebool"
            }, () => result = false);

            result.Should().BeTrue();
            options.SomeStringValue.Should().Be("just a string");
            //base.ElementsShouldBeEqual(new uint[] {10, 20, 30, 40}, options.ArrayOne);
            //base.ElementsShouldBeEqual(new uint[] {11, 22, 33, 44}, options.ArrayTwo);
            options.ArrayOne.Should().ContainInOrder(new uint[] { 10, 20, 30, 40 });
            options.ArrayTwo.Should().ContainInOrder(new uint[] { 11, 22, 33, 44 });
            options.SomeBooleanValue.Should().BeTrue();
        }

        [Fact]
        public void Parse_two_uint_consecutive_array_using_short_names()
        {
            var result = true;
            var options = CommandLine.Parser.Default.ParseArguments<OptionsWithUIntArray>(new[] {
                "-s", "just a string",
                "-o", "10", "20", "30", "40",
                "-t", "11", "22", "33", "44",
                "-b"
            }, () => result = false);

            result.Should().BeTrue();
            options.SomeStringValue.Should().Be("just a string");
            //base.ElementsShouldBeEqual(new uint[] {10, 20, 30, 40}, options.ArrayOne);
            //base.ElementsShouldBeEqual(new uint[] {11, 22, 33, 44}, options.ArrayTwo);
            options.ArrayOne.Should().ContainInOrder(new uint[] { 10, 20, 30, 40 });
            options.ArrayTwo.Should().ContainInOrder(new uint[] { 11, 22, 33, 44 });
            options.SomeBooleanValue.Should().BeTrue();
        }

        /// <summary>
        /// https://github.com/gsscoder/commandline/issues/6
        /// </summary>
        [Fact]
        public void Should_correctly_parse_two_consecutive_arrays()
        {
            // Given
            var parser = new CommandLine.Parser();
            var result = true;
            var argumets = new[] { "--source", @"d:/document.docx", "--output", @"d:/document.xlsx",
                    "--headers", "1", "2", "3", "4",              // first array
                    "--content", "5", "6", "7", "8", "--verbose"  // second array
                };

            // When
            var options = parser.ParseArguments<OptionsWithTwoArrays>(argumets, () => { result = false; });

            // Than
            result.Should().BeTrue();
            options.Should().NotBeNull();
            options.Headers.Should().HaveCount(c => c == 4);
            options.Headers.Should().ContainInOrder(new uint[] { 1, 2, 3, 4 });
            options.Content.Should().HaveCount(c => c == 4);
            options.Content.Should().ContainInOrder(new uint[] { 5, 6, 7, 8 });
        }

        [Fact]
        public void Should_use_property_name_as_long_name_if_omitted()
        {
            // Given
            var parser = new CommandLine.Parser();
            var result = true;
            var arguments = new[] {
                "--offsets", "-2", "-1", "0", "1" , "2"
            };

            // When
            var options = parser.ParseArguments<OptionsWithImplicitLongName>(arguments, () => { result = false; });

            // Than
            result.Should().BeTrue();
            options.Should().NotBeNull();
            options.Offsets.Should().HaveCount(c => c == 5);
            options.Offsets.Should().ContainInOrder(new[] { -2, -1, 0, 1, 2 });
        }
    }
}

