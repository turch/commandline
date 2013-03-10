#region License
// <copyright file="LongOptionParser.cs" company="Giacomo Stelluti Scala">
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

using System.Linq;

using CommandLine.Options;

namespace CommandLine.Parsing
{
    internal sealed class LongOptionParser<T> : ArgumentParser<T>
    {
        public LongOptionParser(T options, OptionMap map, ParserSettings settings)
            : base(options, map, settings)
        {
        }

        public override Transition Parse(IArgumentEnumerator argumentEnumerator)
        {
            var parts = argumentEnumerator.Current.Substring(2).Split(new[] { '=' }, 2);
            var option = Map[parts[0]];

            if (option == null)
            {
                if (!Settings.IgnoreUnknownArguments)
                {
                    return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesSpecification(null, parts[0]) });
                }
                return new MoveNextTransition();
            }

            option.IsDefined = true;

            ArgumentGuard.EnsureOptionArrayAttributeIsNotBoundToScalar(option);

            bool valueSetting;

            var context = base.CreateBindingContext(option);

            if (!option.IsBoolean)
            {
                if (parts.Length == 1 && (argumentEnumerator.IsLast || !ArgumentComparer.IsValue(argumentEnumerator.Next)))
                {
                    return new FailureTransition(Enumerable.Empty<ParsingError>());
                }

                if (parts.Length == 2)
                {
                    if (!option.IsArray)
                    {
                        valueSetting = context.SetValue(parts[1]);
                        if (!valueSetting)
                        {
                            return new FailureTransition(new [] { ParsingError.DefineOptionThatViolatesFormat(option) });
                        }
                        return new SuccessfulTransition();
                    }

                    ArgumentGuard.EnsureOptionAttributeIsArrayCompatible(option);

                    var items = argumentEnumerator.ConsumeNextValues();
                    items.Insert(0, parts[1]);

                    valueSetting = context.SetValue(items);
                    if (!valueSetting)
                    {
                        return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
                    }
                    return new SuccessfulTransition();
                }
                else
                {
                    if (!option.IsArray)
                    {
                        valueSetting = context.SetValue(argumentEnumerator.Next);
                        if (!valueSetting)
                        {
                            return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
                        }
                        return new MoveNextTransition();
                    }

                    ArgumentGuard.EnsureOptionAttributeIsArrayCompatible(option);

                    var items = argumentEnumerator.ConsumeNextValues();

                    valueSetting = context.SetValue(items);
                    if (!valueSetting)
                    {
                        return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
                    }

                    return new SuccessfulTransition();
                }
            }

            if (parts.Length == 2)
            {
                return new FailureTransition(Enumerable.Empty<ParsingError>());
            }

            valueSetting = context.SetValue(true);
            if (!valueSetting)
            {
                return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
            }
            return new SuccessfulTransition();
        }
    }
}