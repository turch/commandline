#region License
//
// Command Line Library: NullableTypesParsingFixture.cs
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
using System.Collections.Generic;
using System.IO;
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Fakes;
#endregion

namespace CommandLine.Tests.Unit.Parser
{
    public class Parser_Nullables_Fixture : ParserBaseFixture
    {
        [Fact]
        public void Parse_nullable_integer_option()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NullableTypesOptions>(
                new[] { "-i", "99" }, () => { result = false; });

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(99);

            parser = new CommandLine.Parser();
            var result2 = true;
            options = parser.ParseArguments<NullableTypesOptions>(
                new string[] { }, () => { result2 = false; });

            result2.Should().BeTrue();
            options.IntegerValue.Should().NotHaveValue();
        }

        [Fact]
        public void Passing_bad_value_to_a_nullable_integer_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NullableTypesOptions>(
                new[] { "-i", "string-value" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_nullable_integer_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NullableTypesOptions>(
                new[] { "-int" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Parse_nullable_enumeration_option()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NullableTypesOptions>(
                new[] { "--enum=ReadWrite" }, () => { result = false; });

            result.Should().BeTrue();
            options.EnumValue.Should().Be(FileAccess.ReadWrite);

            parser = new CommandLine.Parser();
            var result2 = true;
            options = parser.ParseArguments<NullableTypesOptions>(
                new string[] { }, () => { result2 = false; });

            result2.Should().BeTrue();
            options.EnumValue.Should().BeNull();
        }

        [Fact]
        public void Passing_bad_value_to_a_nullable_enumeration_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NullableTypesOptions>(
                new[] { "-e", "Overwrite" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_nullable_enumeration_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NullableTypesOptions>(
                new[] { "--enum" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Parse_nullable_double_option()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NullableTypesOptions>(
                new[] { "-d9.999" }, () => { result = false; });

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(9.999);

            parser = new CommandLine.Parser();
            var result2 = true;
            options = parser.ParseArguments<NullableTypesOptions>(
                new string[] { }, () => { result2 = false; });

            result2.Should().BeTrue();
            options.DoubleValue.Should().NotHaveValue();
        }

        [Fact]
        public void Passing_bad_value_to_a_nullable_double_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NullableTypesOptions>(
                new[] { "--double", "9,999" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_nullable_double_option_fails()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NullableTypesOptions>(
                new[] { "-d" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Parse_string_option_and_nullable_value_types()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<NullableTypesOptions>(new[] { "--string", "alone" }, () => { result = false; });

            result.Should().BeTrue();
            options.StringValue.Should().Be("alone");

            options = new NullableTypesOptions();
            parser = new CommandLine.Parser();
            var result2 = true;
            options = parser.ParseArguments<NullableTypesOptions>(
                new[] { "-d1.789", "--int", "10099", "-stogether", "--enum", "Read" }, () => { result2 = false; });

            result2.Should().BeTrue();
            options.DoubleValue.Should().Be(1.789D);
            options.IntegerValue.Should().Be(10099);
            options.StringValue.Should().Be("together");
            options.EnumValue.Should().Be(FileAccess.Read);
        }
    }
}