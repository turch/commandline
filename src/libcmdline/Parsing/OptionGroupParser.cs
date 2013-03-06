#region License
// <copyright file="OptionGroupParser.cs" company="Giacomo Stelluti Scala">
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
    internal sealed class OptionGroupParser : IArgumentParser
    {
        private readonly bool _ignoreUnkwnownArguments;

        public OptionGroupParser(bool ignoreUnkwnownArguments)
        {
            _ignoreUnkwnownArguments = ignoreUnkwnownArguments;
        }

        public Transition Parse<T>(IArgumentEnumerator argumentEnumerator, OptionMap map, T options)
        {
            var optionGroup = new OneCharStringEnumerator(argumentEnumerator.Current.Substring(1));

            while (optionGroup.MoveNext())
            {
                var option = map[optionGroup.Current];
                if (option == null)
                {
                    if (!_ignoreUnkwnownArguments)
                    {
                        return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesSpecification(char.Parse(optionGroup.Current), null )});
                    }
                    return new MoveNextTransition();
                }

                option.IsDefined = true;

                ArgumentGuard.EnsureOptionArrayAttributeIsNotBoundToScalar(option);

                if (!option.IsBoolean)
                {
                    if (argumentEnumerator.IsLast && optionGroup.IsLast)
                    {
                        return new FailureTransition(Enumerable.Empty<ParsingError>());
                    }

                    bool valueSetting;
                    if (!optionGroup.IsLast)
                    {
                        if (!option.IsArray)
                        {
                            valueSetting = option.BindingContext.SetValue(optionGroup.GetRemainingFromNext());
                            if (!valueSetting)
                            {
                                return new FailureTransition(new [] { ParsingError.DefineOptionThatViolatesFormat(option) });
                            }

                            return new SuccessfulTransition();
                        }

                        ArgumentGuard.EnsureOptionAttributeIsArrayCompatible(option);

                        var items = argumentEnumerator.ConsumeNextValues();
                        items.Insert(0, optionGroup.GetRemainingFromNext());

                        valueSetting = option.BindingContext.SetValue(items);
                        if (!valueSetting)
                        {
                            return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
                        }

                        return new MoveNextTransition();
                    }

                    if (!argumentEnumerator.IsLast && !ArgumentComparer.IsValue(argumentEnumerator.Next))
                    {
                        return new FailureTransition(Enumerable.Empty<ParsingError>());
                    }

                    if (!option.IsArray)
                    {
                        valueSetting = option.BindingContext.SetValue(argumentEnumerator.Next);
                        if (!valueSetting)
                        {
                            return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
                        }

                        return new MoveNextTransition();
                    }

                    ArgumentGuard.EnsureOptionAttributeIsArrayCompatible(option);

                    var moreItems = argumentEnumerator.ConsumeNextValues();

                    valueSetting = option.BindingContext.SetValue(moreItems);
                    if (!valueSetting)
                    {
                         return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
                    }

                    return new SuccessfulTransition();
                }

                if (!optionGroup.IsLast && map[optionGroup.Next] == null)
                {
                    return new FailureTransition(Enumerable.Empty<ParsingError>());
                }

                if (!option.BindingContext.SetValue(true))
                {
                    return new FailureTransition(Enumerable.Empty<ParsingError>());
                }
            }

            return new SuccessfulTransition();
        }
    }
}