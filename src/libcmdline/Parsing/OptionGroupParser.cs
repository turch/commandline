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

using CommandLine.Options;

namespace CommandLine.Parsing
{
    internal sealed class OptionGroupParser : ArgumentParser
    {
        private readonly bool _ignoreUnkwnownArguments;

        public OptionGroupParser(bool ignoreUnkwnownArguments)
        {
            _ignoreUnkwnownArguments = ignoreUnkwnownArguments;
        }

        public override PresentParserState Parse<T>(IArgumentEnumerator argumentEnumerator, OptionMap map, T options)
        {
            var optionGroup = new OneCharStringEnumerator(argumentEnumerator.Current.Substring(1));

            while (optionGroup.MoveNext())
            {
                var option = map[optionGroup.Current];
                if (option == null)
                {
                    if (!_ignoreUnkwnownArguments)
                    {
                        DefineOptionThatViolatesSpecification(char.Parse(optionGroup.Current), null);
                        return PresentParserState.Failure;
                    }
                    return PresentParserState.MoveOnNextElement;
                }

                option.IsDefined = true;

                ArgumentParser.EnsureOptionArrayAttributeIsNotBoundToScalar(option);

                if (!option.IsBoolean)
                {
                    if (argumentEnumerator.IsLast && optionGroup.IsLast)
                    {
                        return PresentParserState.Failure;
                    }

                    bool valueSetting;
                    if (!optionGroup.IsLast)
                    {
                        if (!option.IsArray)
                        {
                            valueSetting = option.BindingContext.SetValue(optionGroup.GetRemainingFromNext());
                            if (!valueSetting)
                            {
                                DefineOptionThatViolatesFormat(option);
                            }

                            return ArgumentParser.BooleanToParserState(valueSetting);
                        }

                        ArgumentParser.EnsureOptionAttributeIsArrayCompatible(option);

                        var items = ArgumentParser.GetNextInputValues(argumentEnumerator);
                        items.Insert(0, optionGroup.GetRemainingFromNext());

                        valueSetting = option.BindingContext.SetValue(items);
                        if (!valueSetting)
                        {
                            DefineOptionThatViolatesFormat(option);
                        }

                        return ArgumentParser.BooleanToParserState(valueSetting, true);
                    }

                    if (!argumentEnumerator.IsLast && !ArgumentComparer.IsAnInvalidOptionName(argumentEnumerator.Next))
                    {
                        return PresentParserState.Failure;
                    }

                    if (!option.IsArray)
                    {
                        valueSetting = option.BindingContext.SetValue(argumentEnumerator.Next);
                        if (!valueSetting)
                        {
                            DefineOptionThatViolatesFormat(option);
                        }

                        return ArgumentParser.BooleanToParserState(valueSetting, true);
                    }

                    ArgumentParser.EnsureOptionAttributeIsArrayCompatible(option);

                    var moreItems = ArgumentParser.GetNextInputValues(argumentEnumerator);

                    valueSetting = option.BindingContext.SetValue(moreItems);
                    if (!valueSetting)
                    {
                        DefineOptionThatViolatesFormat(option);
                    }

                    return ArgumentParser.BooleanToParserState(valueSetting);
                }

                if (!optionGroup.IsLast && map[optionGroup.Next] == null)
                {
                    return PresentParserState.Failure;
                }

                if (!option.BindingContext.SetValue(true))
                {
                    return PresentParserState.Failure;
                }
            }

            return PresentParserState.Success;
        }
    }
}