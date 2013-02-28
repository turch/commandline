﻿#region License
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
#endregion

namespace CommandLine.Parsing
{
    internal static class PropertyWriter
    {
        public static bool WriteScalar<T>(string value, T target, PropertyInfo property, CultureInfo parsingCulture)
        {
            try
            {
                object propertyValue = null;
                if (property.PropertyType.IsEnum)
                {
                    propertyValue = Enum.Parse(property.PropertyType, value, true);
                }
                else
                {
                    propertyValue = Convert.ChangeType(value, property.PropertyType, parsingCulture);
                }

                property.SetValue(target, propertyValue, null);
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }

            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "FormatException (thrown by ConvertFromString) is thrown as Exception.InnerException, so we've to catch directly System.Exception")]
        public static bool WriteNullable<T>(string value, T target, PropertyInfo property, CultureInfo parsingCulture)
        {
            var nc = new NullableConverter(property.PropertyType);

            // FormatException (thrown by ConvertFromString) is thrown as Exception.InnerException, so we've to catch directly System.Exception
            try
            {
                property.SetValue(target, nc.ConvertFromString(null, parsingCulture, value), null);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
