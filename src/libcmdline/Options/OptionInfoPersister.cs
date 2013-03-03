#region License
// <copyright file="PropertyWriter.cs" company="Giacomo Stelluti Scala">
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using CommandLine.Extensions;
using CommandLine.Infrastructure;

#endregion

namespace CommandLine.Options
{
    internal class OptionInfoPersister
    {
        public static bool SetValue<T>(string value, T options, OptionInfo optionInfo)
        {
            if (optionInfo.InnerAttribute is OptionListAttribute)
            {
                return SetValueList(value, options, optionInfo);
            }

            if (optionInfo.InnerProperty.PropertyType.IsNullable())
            {
                return optionInfo.ReceivedValue = PropertyWriter.WriteNullable(value, options, optionInfo.InnerProperty, optionInfo.ParsingCulture);
            }

            return optionInfo.ReceivedValue = PropertyWriter.WriteScalar(value, options, optionInfo.InnerProperty, optionInfo.ParsingCulture);
        }

        public static bool SetValue<T>(IList<string> values, T options, OptionInfo optionInfo)
        {
            var elementType = optionInfo.InnerProperty.PropertyType.GetElementType();
            var array = Array.CreateInstance(elementType, values.Count);

            for (var i = 0; i < array.Length; i++)
            {
                try
                {
                    array.SetValue(Convert.ChangeType(values[i], elementType, optionInfo.ParsingCulture), i);
                    optionInfo.InnerProperty.SetValue(options, array, null);
                }
                catch (FormatException)
                {
                    return false;
                }
            }

            return optionInfo.ReceivedValue = true;
        }

        public static bool SetValue<T>(bool value, T options, OptionInfo optionInfo)
        {
            optionInfo.InnerProperty.SetValue(options, value, null);
            return optionInfo.ReceivedValue = true;
        }

        public static void SetDefault<T>(T options, OptionInfo optionInfo)
        {
            if (optionInfo.HasDefaultValue)
            {
                try
                {
                    optionInfo.InnerProperty.SetValue(options, optionInfo.DefaultValue, null);
                }
                catch (Exception e)
                {
                    throw new ParserException("Bad default value.", e);
                }
            }
        }

        private static bool SetValueList<T>(string value, T options, OptionInfo optionInfo)
        {
            optionInfo.InnerProperty.SetValue(options, new List<string>(), null);
            var fieldRef = (IList<string>)optionInfo.InnerProperty.GetValue(options, null);
            var values = value.Split(((OptionListAttribute)optionInfo.InnerAttribute).Separator);
            foreach (var item in values)
            {
                fieldRef.Add(item);
            }

            return optionInfo.ReceivedValue = true;
        }
    }
}