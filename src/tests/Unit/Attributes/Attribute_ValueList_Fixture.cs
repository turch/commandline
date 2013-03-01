#region License
//
// Command Line Library: ValueListAttributeFixture.cs
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
using System.ComponentModel;

using CommandLine.Tests.Fakes;

using Xunit;
using FluentAssertions;
#endregion

namespace CommandLine.Tests.Unit.Attributes
{
    public class Attribute_ValueList_Fixture : BaseFixture
    {
        #region Mock Objects
        private class MockSpecializedList : List<string>
        {
        }

        private class MockOptions
        {
            [ValueList(typeof(List<string>))]
            public IList<string> Values { get; set; }
        }
        #endregion

        [Fact]
        public void Will_throw_exception_if_concrete_type_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ValueListAttribute(null));
        }

        [Fact]
        public void Will_throw_exception_if_concrete_type_is_incompatible()
        {
             Assert.Throws<ParserException>(
                () => new ValueListAttribute(new List<object>().GetType()));
        }

        [Fact]
        public void Concrete_type_is_generic_list_of_string()
        {
            new ValueListAttribute(new List<string>().GetType());
        }

        [Fact]
        public void Concrete_type_is_generic_list_of_string_sub_type()
        {
            new ValueListAttribute(new MockSpecializedList().GetType());
        }

        [Fact]
        public void Get_generic_list_of_string_interface_reference()
        {
            var options = new MockOptions();

            IList<string> values = ValueListAttribute.GetReference(options);
            values.Should().NotBeNull();
            values.GetType().Should().Be(typeof(List<string>));
        }

        [Fact]
        public void Use_generic_list_of_string_interface_reference()
        {
            var options = new MockOptions();

            var values = ValueListAttribute.GetReference(options);
            values.Add("value0");
            values.Add("value1");
            values.Add("value2");

            base.ElementsShouldBeEqual(new string[] { "value0", "value1", "value2" }, options.Values);
        }

        [Fact]
        public void Value_list_attribute_isolates_non_option_values()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<SimpleOptionsWithValueList>(
                new[] { "--switch", "file1.ext", "file2.ext", "file3.ext", "-s", "out.ext" }, () => { result = false; });

            result.Should().BeTrue();

            options.Items[0].Should().Be("file1.ext");
            options.Items[1].Should().Be("file2.ext");
            options.Items[2].Should().Be("file3.ext");
            options.StringValue.Should().Be("out.ext");
            options.BooleanValue.Should().BeTrue();
            Console.WriteLine(options);
        }

        [Fact]
        public void Value_list_with_max_elem_inside_bounds()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<OptionsWithValueListMaximumThree>(
                new[] { "file.a", "file.b", "file.c" }, () => { result = false; });

            result.Should().BeTrue();

            options.InputFilenames[0].Should().Be("file.a");
            options.InputFilenames[1].Should().Be("file.b");
            options.InputFilenames[2].Should().Be("file.c");
            options.OutputFile.Should().BeNull();
            options.Overwrite.Should().BeFalse();
            Console.WriteLine(options);
        }

        [Fact]
        public void Value_list_with_max_elem_outside_bounds()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<OptionsWithValueListMaximumThree>(
                    new[] { "file.a", "file.b", "file.c", "file.d" }, () => { result = false; });

            result.Should().BeFalse();
        }

        [Fact]
        public void Value_list_with_max_elem_set_to_zero_succeeds()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<OptionsWithValueListMaximumZero>(
                new string[] { }, () => { result = false; });

            result.Should().BeTrue();

            options.Junk.Should().HaveCount(n => n == 0);
            Console.WriteLine(options);
        }

        [Fact]
        public void Value_list_with_max_elem_set_to_zero_failes()
        {
            var parser = new CommandLine.Parser();
            var result = true;
            var options = parser.ParseArguments<OptionsWithValueListMaximumZero>(
                new[] { "some", "value" }, () => { result = false; });

            result.Should().BeFalse();
        }
    }
}
