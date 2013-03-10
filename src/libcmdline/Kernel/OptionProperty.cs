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

namespace CommandLine.Kernel
{
    internal sealed class OptionProperty : IProperty
    {
        public OptionProperty(PropertyInfo property, BaseOptionAttribute attribute)
        {
            ShortName = attribute.ShortName;
            if (attribute.AutoLongName)
            {
                LongName = property.Name.ToLowerInvariant();
                UniqueName = LongName;
            }
            else
            {
                LongName = attribute.LongName;
                UniqueName = attribute.UniqueName;
            }
            Required = attribute.Required;
            MutuallyExclusiveSet = attribute.MutuallyExclusiveSet;
            DefaultValue = attribute.DefaultValue;
            HasDefaultValue = attribute.HasDefaultValue;
            IsBoolean = property.PropertyType == typeof(bool);
            IsArray = property.PropertyType.IsArray;
            IsAttributeArrayCompatible = attribute is OptionArrayAttribute;
            HasBothNames = attribute.ShortName.HasValue && attribute.LongName != null;
            HasParameterLessCtor = property.PropertyType.GetConstructor(Type.EmptyTypes) != null;

            // TODO: refactor this and bindingcontext
            UnderlyingProperty = property;
            UnderlyingAttribute = attribute;
        }

        // TEST ctor
        internal OptionProperty(char? shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
        }

        public char? ShortName { get; private set; }

        public string LongName { get; private set; }

        public string UniqueName { get; private set; }

        public string MutuallyExclusiveSet { get; private set; }

        public bool Required { get; private set; }

        public bool IsBoolean { get; private set; }

        public bool IsArray { get; private set; }

        public bool IsAttributeArrayCompatible { get; protected set; }

        // TODO: refactor this property
        public bool IsDefined { get; internal set; }

        // TODO: refactor this property
        public bool ReceivedValue { get; internal set; }

        public bool HasBothNames { get; private set; }

        public bool HasParameterLessCtor { get; private set; }

        public bool HasDefaultValue { get; private set; }

        public object DefaultValue { get; private set; }

        public PropertyInfo UnderlyingProperty { get; private set; }

        public BaseOptionAttribute UnderlyingAttribute { get; private set; }
    }
}