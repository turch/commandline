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
        public void Parse_Nullable_Integer_Option(Parser sut, int intergerValue)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "-i", intergerValue.ToInvariantString() }, () => { result = false; });

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(intergerValue);
        }

        [Theory, ParserTestConventions]
        public void Parse_Nullable_Integer_Option__without_value(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new string[] { }, () => { result = false; });

            result.Should().BeTrue();
            options.IntegerValue.Should().NotHaveValue();
        }

        [Theory, ParserTestConventions]
        public void Passing_a_Bad_Value_to_a_Nullable_Integer_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "-i", "string-value" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Theory, ParserTestConventions]
        public void Not_Passing_a_Value_to_a_Nullable_Integer_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "-int" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Theory, ParserTestConventions]
        public void Parse_Nullable_Enumeration_Option(Parser sut, FileAttributes enumValue)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { string.Concat("--enum2=", enumValue.ToInvariantString()) }, () => { result = false; });

            result.Should().BeTrue();
            options.EnumValue.Should().Be(enumValue);
        }

        [Theory, ParserTestConventions]
        public void Parse_Nullable_Enumeration_Option__without_value(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new string[] { }, () => { result = false; });

            result.Should().BeTrue();
            options.EnumValue.Should().BeNull();
        }

        [Theory, ParserTestConventions]
        public void Passing_a_Bad_Value_to_a_Nullable_Enumeration_Option_Fails(Parser sut, FileAttributes enumValue)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "-f", enumValue.ToInvariantString() }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Theory, ParserTestConventions]
        public void Not_Passing_a_value_to_a_Nullable_Enumeration_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "--enum" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Theory, ParserTestConventions]
        public void Parse_Nullable_Double_Option(Parser sut, double doubleValue)
        {
            var result = true;
            var expected = doubleValue.AsFractional();
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { string.Concat("-d", expected) }, () => { result = false; });

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(expected);
        }

        [Theory, ParserTestConventions]
        public void Parse_Nullable_Double_Option__without_value(Parser sut)
        {
            var result2 = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new string[] { }, () => { result2 = false; });

            result2.Should().BeTrue();
            options.DoubleValue.Should().NotHaveValue();
        }

        [Theory, ParserTestConventions]
        public void Passing_a_Bad_Value_to_a_Nullable_Double_Option_Fails(Parser sut, double doubleValue)
        {
            var result = true;
            var expected = doubleValue.AsFractional();
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "--double", expected.ToInvariantString() }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Theory, ParserTestConventions]
        public void Not_Passing_a_Value_to_a_Nullable_Double_Option_Fails(Parser sut)
        {
            var result = true;
            var options = sut.ParseArguments<Fake_Nullables_Options>(
                new[] { "-d" }, () => { result = false; });

            result.Should().BeFalse();
        }
    }
}