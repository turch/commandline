#region License
// <copyright file="UnboundValues.cs" company="Giacomo Stelluti Scala">
//   Copyright 2015-2013 Giacomo Stelluti Scala
// </copyright>
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
#endregion
#region Using Directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using CommandLine.Extensions;
using CommandLine.Infrastructure;
using CommandLine.Kernel;
using CommandLine.Parsing;

#endregion

namespace CommandLine
{
    // TODO: refactor this
    internal sealed class UnboundValues<T>
    {
        private readonly T _options;
        private readonly CultureInfo _parsingCulture;
        private readonly IEnumerable<ValueOptionProperty> _valueOptionAttributeList;
        //private readonly ValueListAttribute _valueListAttribute;
        private readonly ValueListProperty _valueListProperty;
        private readonly bool _hasValueList;
        private readonly IList<string> _valueListReference;
        private int _valueOptionIndex;

        public UnboundValues(T options, CultureInfo parsingCulture)
        {
            _options = options;
            _parsingCulture = parsingCulture;

            _valueOptionAttributeList = SetValueOptionList(_options);

            //_valueListAttribute = ValueListAttribute.GetAttribute(_options).Right();
            //if (_valueListAttribute != null)
            //{
            //    _valueListReference = ValueListAttribute.GetReference(_options);
            //}

            var properties = new ValueListMemberQuery().SelectMembers(options.GetType());
            _hasValueList = properties.OfType<ValueListProperty>().Any();
            if (_hasValueList)
            {
                _valueListProperty = ((ValueListProperty)properties.First());
                _valueListReference = _valueListProperty.GetList(options);
            }
        }

        public bool CanWrite
        {
            get { return HasValueList || AnyValueOption; }
        }

        private bool HasValueList
        {
            //get { return _valueListAttribute != null; }
            get { return _hasValueList; }
        }

        private bool AnyValueOption
        {
            get { return _valueOptionAttributeList.Any(); }
        }

        public bool WriteValue(string value)
        {
            if (AnyValueOption &&
                _valueOptionIndex < _valueOptionAttributeList.Count())
            {
                var valueOption = _valueOptionAttributeList.ElementAt(_valueOptionIndex++);

                return valueOption.MutateValue(_options, value, _parsingCulture);
            }

            return HasValueList && this.WriteItemToValueList(value);
        }

        private bool WriteItemToValueList(string value)
        {
            if (_valueListProperty.MaximumElements == 0 ||
                _valueListProperty.MaximumElements == _valueListReference.Count)
            {
                return false;
            }

            _valueListReference.Add(value);
            return true;
        }

        private static IEnumerable<ValueOptionProperty> SetValueOptionList(T options)
        {
            return from prop in new ValueOptionMemberQuery().SelectMembers(options.GetType()).Cast<ValueOptionProperty>()
                   orderby prop.Index
                   select prop;
        }
    }
}