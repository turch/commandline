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
    internal sealed class LongOptionParser : ArgumentParser
    {
        private readonly bool _ignoreUnkwnownArguments;

        public LongOptionParser(bool ignoreUnkwnownArguments)
        {
            _ignoreUnkwnownArguments = ignoreUnkwnownArguments;
        }

        //public override ChangeStateTransition Parse<T>(IArgumentEnumerator argumentEnumerator, OptionMap map, T options)
        public override Transition Parse<T>(IArgumentEnumerator argumentEnumerator, OptionMap map, T options)
        {
            var parts = argumentEnumerator.Current.Substring(2).Split(new[] { '=' }, 2);
            var option = map[parts[0]];

            if (option == null)
            {
                if (!_ignoreUnkwnownArguments)
                {
                    //DefineOptionThatViolatesSpecification(null, parts[0]);
                    //return ChangeStateTransition.Failure;
                    return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesSpecification(null, parts[0]) });
                }
                //return ChangeStateTransition.MoveOnNextElement;
                return new MoveNextTransition();
            }

            option.IsDefined = true;

            ArgumentParser.EnsureOptionArrayAttributeIsNotBoundToScalar(option);

            bool valueSetting;

            if (!option.IsBoolean)
            {
                if (parts.Length == 1 && (argumentEnumerator.IsLast || !ArgumentComparer.IsAnInvalidOptionName(argumentEnumerator.Next)))
                {
                    //return ChangeStateTransition.Failure;
                    return new FailureTransition(Enumerable.Empty<ParsingError>());
                }

                if (parts.Length == 2)
                {
                    if (!option.IsArray)
                    {
                        valueSetting = option.BindingContext.SetValue(parts[1]);
                        if (!valueSetting)
                        {
                            //DefineOptionThatViolatesFormat(option);
                            return new FailureTransition(new [] { ParsingError.DefineOptionThatViolatesFormat(option) });
                        }
                        //return ArgumentParser.BooleanToParserState(valueSetting);
                        return new SuccessfulTransition(); //return new MoveNextTransition();
                    }

                    ArgumentParser.EnsureOptionAttributeIsArrayCompatible(option);

                    var items = ArgumentParser.GetNextInputValues(argumentEnumerator);
                    items.Insert(0, parts[1]);

                    valueSetting = option.BindingContext.SetValue(items);
                    if (!valueSetting)
                    {
                        //DefineOptionThatViolatesFormat(option);
                        return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
                    }
                    //return ArgumentParser.BooleanToParserState(valueSetting);
                    return new SuccessfulTransition(); //return new MoveNextTransition();
                }
                else
                {
                    if (!option.IsArray)
                    {
                        valueSetting = option.BindingContext.SetValue(argumentEnumerator.Next);
                        if (!valueSetting)
                        {
                            //DefineOptionThatViolatesFormat(option);
                            return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
                        }
                        //return ArgumentParser.BooleanToParserState(valueSetting, true);
                        return new MoveNextTransition();
                    }

                    ArgumentParser.EnsureOptionAttributeIsArrayCompatible(option);

                    var items = ArgumentParser.GetNextInputValues(argumentEnumerator);

                    valueSetting = option.BindingContext.SetValue(items);
                    if (!valueSetting)
                    {
                        //DefineOptionThatViolatesFormat(option);
                        return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
                    }

                    //return ArgumentParser.BooleanToParserState(valueSetting);
                    return new SuccessfulTransition();
                }
            }

            if (parts.Length == 2)
            {
                //return ChangeStateTransition.Failure;
                return new FailureTransition(Enumerable.Empty<ParsingError>());
            }

            valueSetting = option.BindingContext.SetValue(true);
            if (!valueSetting)
            {
                //DefineOptionThatViolatesFormat(option);
                return new FailureTransition(new[] { ParsingError.DefineOptionThatViolatesFormat(option) });
            }
            //return ArgumentParser.BooleanToParserState(valueSetting);
            return new SuccessfulTransition();
        }
    }
}