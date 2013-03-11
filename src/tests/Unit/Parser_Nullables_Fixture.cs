using System;
using System.Collections.Generic;
using System.IO;
using CommandLine.Tests.Conventions;
using CommandLine.Tests.Extensions;
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Fakes;
using Xunit.Extensions;

namespace CommandLine.Tests.Unit
{
    public class Parser_Nullables_Fixture
    {
        [Theory, ParserTestConventions]
        public void Parse_nullable_integer_option(Parser sut, int intergerValue)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "-i", intergerValue.ToInvariantString() }, () => { result = false; });

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(intergerValue);
        }

        [Theory, ParserTestConventions]
        public void Parse_nullable_integer_option__without_value(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new string[] { }, () => { result = false; });

            result.Should().BeTrue();
            options.IntegerValue.Should().NotHaveValue();
        }

        [Theory, ParserTestConventions]
        public void Passing_a_bad_value_to_a_nullable_integer_options_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "-i", "string-value" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Theory, ParserTestConventions]
        public void Not_passing_a_value_to_a_nullable_integer_options_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "-int" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Theory, ParserTestConventions]
        public void Parse_nullable_enumeration_option(Parser sut, FileAttributes enumValue)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { string.Concat("--enum2=", enumValue.ToInvariantString()) }, () => { result = false; });

            result.Should().BeTrue();
            options.EnumValueWithMoreValues.Should().Be(enumValue);
        }

        [Theory, ParserTestConventions]
        public void Parse_nullable_enumeration_option__without_value(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new string[] { }, () => { result = false; });

            result.Should().BeTrue();
            options.EnumValue.Should().BeNull();
        }

        [Theory, ParserTestConventions]
        public void Passing_a_bad_value_to_a_nullable_Enumeration_options_fails(Parser sut, FileAttributes enumValue)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "-f", "I_am_Not_a_Correct_Value" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Theory, ParserTestConventions]
        public void Not_Passing_a_value_to_a_nullable_enumeration_options_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "--enum" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Theory, ParserTestConventions]
        public void Parse_nullable_double_option(Parser sut, double doubleValue)
        {
            var result = true;
            var expected = doubleValue.AsFractional();
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { string.Concat("-d", expected.ToInvariantString()) }, () => { result = false; });

            result.Should().BeTrue();
            options.DoubleValue.Value.AsRounded().Should().Be(expected);
        }

        [Theory, ParserTestConventions]
        public void Parse_nullable_double_option__without_value(Parser sut)
        {
            var result2 = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new string[] { }, () => { result2 = false; });

            result2.Should().BeTrue();
            options.DoubleValue.Should().NotHaveValue();
        }

        [Theory, ParserTestConventions]
        public void Passing_a_bad_value_to_a_nullable_double_options_fails(Parser sut, double doubleValue)
        {
            var result = true;
            var expected = doubleValue.AsFractional();
            // passing a bad formatted value for conventions culture
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "--double", expected.ToItalianCultureString() }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Theory, ParserTestConventions]
        public void Not_passing_a_value_to_a_nullable_double_options_fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "-d" }, () => { result = false; });

            result.Should().BeFalse();
        }
    }
}