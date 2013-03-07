#region License
// <copyright file="ArgumentParser.cs" company="Giacomo Stelluti Scala">
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
using CommandLine.Extensions;
using CommandLine.Options;

#endregion

namespace CommandLine.Parsing
{
    internal abstract class ArgumentParser<T>
    {
        private readonly T _options;
        private readonly OptionMap _map;
        private readonly ParserSettings _settings;

        protected ArgumentParser(T options, OptionMap map, ParserSettings settings)
        {
            _options = options;
            _map = map;
            _settings = settings;
        }

        protected T Options 
        {
            get { return _options; }
        }

        protected OptionMap Map
        {
            get { return _map; }
        }

        protected  ParserSettings Settings
        {
            get { return _settings; }
        }

        public abstract Transition Parse(IArgumentEnumerator argumentEnumerator);
    }
}