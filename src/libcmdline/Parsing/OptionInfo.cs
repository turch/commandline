#region License
// <copyright file="OptionInfo.cs" company="Giacomo Stelluti Scala">
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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

using CommandLine.Infrastructure;
#endregion

namespace CommandLine.Parsing
{
    [DebuggerDisplay("ShortName = {ShortName}, LongName = {LongName}")]
    internal sealed class OptionInfo
    {
        private readonly CultureInfo _parsingCulture;
        private readonly BaseOptionAttribute _attribute;
        private readonly PropertyInfo _property;
        private readonly bool _required;
        private readonly char? _shortName;
        private readonly string _longName;
        private readonly string _mutuallyExclusiveSet;
        private readonly object _defaultValue;
        private readonly bool _hasDefaultValue;

        public OptionInfo(BaseOptionAttribute attribute, PropertyInfo property, CultureInfo parsingCulture)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute", SR.ArgumentNullException_AttributeCannotBeNull);
            }

            if (property == null)
            {
                throw new ArgumentNullException("property", SR.ArgumentNullException_PropertyCannotBeNull);
            }

            _required = attribute.Required;
            _shortName = attribute.ShortName;
            _longName = attribute.LongName;
            _mutuallyExclusiveSet = attribute.MutuallyExclusiveSet;
            _defaultValue = attribute.DefaultValue;
            _hasDefaultValue = attribute.HasDefaultValue;
            _attribute = attribute;
            _property = property;
            _parsingCulture = parsingCulture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionInfo"/> class. Used for unit testing purpose.
        /// </summary>
        /// <param name="shortName">Option short name.</param>
        /// <param name="longName">Option long name.</param>
        internal OptionInfo(char? shortName, string longName)
        {
            _shortName = shortName;
            _longName = longName;
        }

        public char? ShortName
        {
            get { return _shortName; }
        }

        public string LongName
        {
            get { return _longName; }
        }

        public string MutuallyExclusiveSet
        {
            get { return _mutuallyExclusiveSet; }
        }

        public bool Required
        {
            get { return _required; }
        }

        public bool IsBoolean
        {
            get { return _property.PropertyType == typeof(bool); }
        }

        public bool IsArray
        {
            get { return _property.PropertyType.IsArray; }
        }

        public bool IsAttributeArrayCompatible
        {
            get { return _attribute is OptionArrayAttribute; }
        }

        public bool IsDefined
        {
            get; set;
        }

        public bool ReceivedValue
        {
            get; set;
        }

        public bool HasBothNames
        {
            get
            {
                return _shortName != null && _longName != null;
            }
        }

        public bool HasParameterLessCtor
        {
            get; set;
        }

        public Attribute InnerAttribute
        {
            get
            {
                return _attribute;
            }
        }

        public PropertyInfo InnerProperty
        {
            get
            {
                return _property;
            }
        }

        public CultureInfo ParsingCulture
        {
            get
            {
                return _parsingCulture;
            }
        }

        public bool HasDefaultValue
        {
            get
            {
                return _hasDefaultValue;
            }
        }

        public object DefaultValue
        {
            get
            {
                return _defaultValue;
            }
        }

        public object GetValue(object target)
        {
            return _property.GetValue(target, null);
        }

        public object CreateInstance(object target)
        {
            object instance = null;

            try
            {
                instance = Activator.CreateInstance(_property.PropertyType);

                _property.SetValue(target, instance, null);
            }
            catch (Exception e)
            {
                throw new ParserException(SR.CommandLineParserException_CannotCreateInstanceForVerbCommand, e);
            }

            return instance;
        }
    }
}