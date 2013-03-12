#region License
// <copyright file="VerbOptionAttribute.cs" company="Giacomo Stelluti Scala">
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

using CommandLine.Infrastructure;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Models a verb command specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class VerbAttribute : Attribute
    {
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerbAttribute"/> class.
        /// </summary>
        /// <param name="name">The long name of the verb command.</param>
        public VerbAttribute(string name)
        {
            Assumes.NotNullOrEmpty(name, "name");

            _name = name;
        }

        /// <summary>
        /// Verb commands do not support short name by design.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        public string HelpText
        {
            get; set;
        }
    }
}