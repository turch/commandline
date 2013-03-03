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

namespace CommandLine.Options
{
    [DebuggerDisplay("ShortName = {ShortName}, LongName = {LongName}")]
    internal sealed class OptionInfo
    {
        public char? ShortName { get; set; }

        public string LongName { get; set; }

        public string MutuallyExclusiveSet { get; set; }

        public bool Required { get; set; }

        public bool IsBoolean { get; set; }

        public bool IsArray { get; set; }

        public bool IsAttributeArrayCompatible { get; set; }

        public bool IsDefined { get; set; }

        public bool ReceivedValue { get; set; }

        public bool HasBothNames { get; set; }

        public bool HasParameterLessCtor { get; set; }

        public Attribute InnerAttribute { get; set; }

        public PropertyInfo InnerProperty { get; set; }

        public CultureInfo ParsingCulture { get; set; }

        public bool HasDefaultValue { get; set; }

        public object DefaultValue { get; set; }

        public object GetValue(object target)
        {
            return InnerProperty.GetValue(target, null);
        }

        public object CreateInstance(object target)
        {
            object instance = null;

            try
            {
                instance = Activator.CreateInstance(InnerProperty.PropertyType);

                InnerProperty.SetValue(target, instance, null);
            }
            catch (Exception e)
            {
                throw new ParserException(SR.CommandLineParserException_CannotCreateInstanceForVerbCommand, e);
            }

            return instance;
        }
    }
}