﻿#region License
// <copyright file="Parser.cs" company="Giacomo Stelluti Scala">
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine.Extensions;
using CommandLine.Infrastructure;
using CommandLine.Parsing;
using CommandLine.Text;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Provides methods to parse command line arguments.
    /// </summary>
    public sealed class Parser : IDisposable
    {
        private static readonly Parser DefaultParser = new Parser(true);
        private readonly ParserSettings _settings;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Parser"/> class.
        /// </summary>
        public Parser()
        {
            _settings = new ParserSettings { Consumed = true };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class,
        /// configurable with a <see cref="ParserSettings"/> object.
        /// </summary>
        /// <param name="settings">The <see cref="ParserSettings"/> object is used to configure
        /// aspects and behaviors of the parser.</param>
        [Obsolete("Use constructor that accepts Action<ParserSettings>.")]
        public Parser(ParserSettings settings)
        {
            Assumes.NotNull(settings, "settings", SR.ArgumentNullException_ParserSettingsInstanceCannotBeNull);
            
            if (settings.Consumed)
            {
                throw new InvalidOperationException(SR.InvalidOperationException_ParserSettingsInstanceCanBeUsedOnce);
            }

            _settings = settings;
            _settings.Consumed = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class,
        /// configurable with <see cref="ParserSettings"/> using a delegate.
        /// </summary>
        /// <param name="configuration">The <see cref="Action&lt;ParserSettings&gt;"/> delegate used to configure
        /// aspects and behaviors of the parser.</param>
        public Parser(Action<ParserSettings> configuration)
        {
            Assumes.NotNull(configuration, "configuration", SR.ArgumentNullException_ParserSettingsDelegateCannotBeNull);

            _settings = new ParserSettings();
            configuration.Invoke(_settings);
            _settings.Consumed = true;
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "singleton", Justification = "The constructor that accepts a boolean is designed to support default singleton, the parameter is ignored")]
        private Parser(bool singleton)
            : this(with =>
                {
                    with.CaseSensitive = false;
                    with.MutuallyExclusive = false;
                    with.HelpWriter = Console.Error;
                    with.ParsingCulture = CultureInfo.InvariantCulture;
                })
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CommandLine.Parser"/> class.
        /// </summary>
        ~Parser()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the singleton instance created with basic defaults.
        /// </summary>
        public static Parser Default
        {
            get { return DefaultParser; }
        }

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments returning values as an instance
        /// of type <typeparamref name="T"/>. Type public fields should be decorated with appropriate attributes.
        /// If parsing fails, the method invokes the <paramref name="onFail"/> delegate.
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments.</param>
        /// <param name="onFail">The <see cref="Action"/> delegate executed when parsing fails.</param>
        /// <typeparam name="T">An object's type used to receive values.
        /// Parsing rules are defined using <see cref="CommandLine.BaseOptionAttribute"/> derived types.</typeparam>
        /// <returns>An instance of type <typeparamref name="T"/> with parsed values.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="onFail"/> is null.</exception>
        public T ParseArguments<T>(string[] args, Action onFail)
            where T : new()
        {
            Assumes.NotNull(args, "args", SR.ArgumentNullException_ArgsStringArrayCannotBeNull);
            Assumes.NotNull(onFail, "onFail", SR.ArgumentNullException_OnFailDelegateCannotBeNull);

            var resultAndOptions = this.ParseArguments<T>(args);
            var result = resultAndOptions.Item1;
            var options = resultAndOptions.Item2;

            if (!result)
            {
                HandleDynamicAutoBuild(options);

                onFail();
            }

            return options;
        }

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments returning values as an instance
        /// of type <typeparamref name="T"/>. Type public fields should be decorated with appropriate attributes.
        /// If parsing fails, the method invokes the <paramref name="onFail"/> delegate.
        /// This overload supports verb commands.
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments.</param>
        /// <param name="onVerbCommand">Delegate executed to capture verb command name and instance.</param>
        /// <param name="onFail">The <see cref="Action"/> delegate executed when parsing fails.</param>
        /// <typeparam name="T">An object's type used to receive values.
        /// Parsing rules are defined using <see cref="CommandLine.BaseOptionAttribute"/> derived types.</typeparam>
        /// <returns>An instance of type  <typeparamref name="T"/> with parsed values.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="onVerbCommand"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="onFail"/> is null.</exception>
        public T ParseArguments<T>(string[] args, Action<string, object> onVerbCommand, Action onFail)
            where T : new()
        {
            Assumes.NotNull(args, "args", SR.ArgumentNullException_ArgsStringArrayCannotBeNull);
            Assumes.NotNull(onVerbCommand, "onVerbCommand", SR.ArgumentNullException_OnVerbDelegateCannotBeNull);
            Assumes.NotNull(onFail, "onFail", SR.ArgumentNullException_OnFailDelegateCannotBeNull);

            var resultAndOptionsAndVerbInstance = this.ParseArgumentsVerbs<T>(args);

            var result = resultAndOptionsAndVerbInstance.Item1;
            var options = resultAndOptionsAndVerbInstance.Item2;
            var verbInstance = resultAndOptionsAndVerbInstance.Item3;

            // TODO: mutually activate delegates?
            onVerbCommand(args.FirstOrDefault() ?? string.Empty, result ? verbInstance : null);

            if (!result)
            {
                HandleDynamicAutoBuild(options);

                onFail();
            }

            return options;
        }

        /// <summary>
        /// Frees resources owned by the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "By design")]
        internal static object InternalGetVerbOptionsInstanceByName(string verb, object target, out bool found)
        {
            found = false;
            if (string.IsNullOrEmpty(verb))
            {
                return target;
            }

            var pair = ReflectionHelper.RetrieveOptionProperty<VerbOptionAttribute>(target, verb);
            found = pair != null;
            return found ? pair.Left().GetValue(target, null) : target;
        }

        private static T SetParserStateIfNeeded<T>(T options, IEnumerable<ParsingError> errors)
        {
            if (!options.CanReceiveParserState())
            {
                return options;
            }

            var property = ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(options)[0].Left();

            var parserState = property.GetValue(options, null);
            if (parserState != null)
            {
                if (!(parserState is IParserState))
                {
                    throw new InvalidOperationException(SR.InvalidOperationException_ParserStateInstanceBadApplied);
                }

                if (!(parserState is ParserState))
                {
                    throw new InvalidOperationException(SR.InvalidOperationException_ParserStateInstanceCannotBeNotNull);
                }
            }
            else
            {
                try
                {
                    property.SetValue(options, new ParserState(), null);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(SR.InvalidOperationException_ParserStateInstanceBadApplied, ex);
                }
            }

            var state = (IParserState)property.GetValue(options, null);

            foreach (var error in errors)
            {
                state.Errors.Add(error);
            }

            return options;
        }

        private static void DisplayHelpText<T>(T options, Tuple<MethodInfo, HelpOptionAttribute> pair, TextWriter helpWriter)
        {
            string helpText;
            HelpOptionAttribute.InvokeMethod(options, pair, out helpText); // TODO: refactor this
            helpWriter.Write(helpText);
        }

        private static StringComparison GetStringComparison(ParserSettings settings)
        {
            return settings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        private Tuple<bool, T> ParseArguments<T>(string[] args)
            where T : new()
        {
            var options = new T();
            var pair = ReflectionHelper.RetrieveMethod<HelpOptionAttribute>(options);
            var helpWriter = _settings.HelpWriter;

            // TODO: refactoring following query in TargetCapabilitiesExtensions?
            if (pair != null && helpWriter != null)
            {
                if (this.TryParseHelp(args, pair.Right()))
                {
                    DisplayHelpText(options, pair, helpWriter);
                    return new Tuple<bool, T>(false, options);
                }

                var optionsAndResult = this.ParseArgumentsCore(args, options);
                var result = optionsAndResult.Item1;
                options = optionsAndResult.Item2;

                if (!result)
                {
                    DisplayHelpText(options, pair, helpWriter);
                    return new Tuple<bool, T>(false, options);
                }
            }

            return this.ParseArgumentsCore(args, options);
        }

        private Tuple<bool, T> ParseArgumentsCore<T>(string[] args, T options)
        {
            var hadError = false;
            var optionMap = OptionMap.Create(options, _settings);
            optionMap.SetDefaults();
            var valueMapper = new ValueMapper(options, _settings.ParsingCulture);

            var arguments = new StringArrayEnumerator(args);
            while (arguments.MoveNext())
            {
                var argument = arguments.Current;
                if (string.IsNullOrEmpty(argument))
                {
                    continue;
                }

                var parser = ArgumentParser.Create(argument, _settings.IgnoreUnknownArguments);
                if (parser != null)
                {
                    var result = parser.Parse(arguments, optionMap, options);
                    if ((result & PresentParserState.Failure) == PresentParserState.Failure)
                    {
                        options = SetParserStateIfNeeded(options, parser.PostParsingState);
                        hadError = true;
                        continue;
                    }

                    if ((result & PresentParserState.MoveOnNextElement) == PresentParserState.MoveOnNextElement)
                    {
                        arguments.MoveNext();
                    }
                }
                else if (valueMapper.CanReceiveValues)
                {
                    if (!valueMapper.MapValueItem(argument))
                    {
                        hadError = true;
                    }
                }
            }

            hadError |= !optionMap.EnforceRules();

            return new Tuple<bool, T>(!hadError, options);
        }

        private Tuple<bool, T, object> ParseArgumentsVerbs<T>(string[] args)
            where T : new()
        {
            var options = new T();

            var verbs = ReflectionHelper.RetrievePropertyList<VerbOptionAttribute>(options);
            var helpInfo = ReflectionHelper.RetrieveMethod<HelpVerbOptionAttribute>(options);

            if (args.Length == 0)
            {
                if (helpInfo != null || _settings.HelpWriter != null)
                {
                    DisplayHelpVerbText(options, helpInfo, null);
                }

                return new Tuple<bool, T, object>(false, options, null);
            }

            var optionMap = OptionMap.Create(options, verbs, _settings);

            if (TryParseHelpVerb(args, options, helpInfo, optionMap))
            {
                return new Tuple<bool, T, object>(false, options, null);
            }

            var verbOption = optionMap[args.First()];

            // User invoked a bad verb name
            if (verbOption == null)
            {
                if (helpInfo != null)
                {
                    DisplayHelpVerbText(options, helpInfo, null);
                }

                return new Tuple<bool, T, object>(false, options, null);
            }

            var verbInstance = verbOption.GetValue(options);
            if (verbInstance == null)
            {
                // Developer has not provided a default value and did not assign an instance
                verbInstance = verbOption.CreateInstance(options);
            }

            var resultAndVerbInstance = this.ParseArgumentsCore(args.Skip(1).ToArray(), verbInstance);
            var result = resultAndVerbInstance.Item1;
            verbInstance = resultAndVerbInstance.Item2;

            if (!result && helpInfo != null)
            {
                // Particular verb parsing failed, we try to print its help
                DisplayHelpVerbText(options, helpInfo, args.First());
            }

            return new Tuple<bool, T, object>(result, options, verbInstance);
        }

        private bool TryParseHelp(string[] args, HelpOptionAttribute helpOption)
        {
            var caseSensitive = _settings.CaseSensitive;
            foreach (var arg in args)
            {
                if (helpOption.ShortName != null)
                {
                    if (ArgumentParser.CompareShort(arg, helpOption.ShortName, caseSensitive))
                    {
                        return true;
                    }
                }

                if (string.IsNullOrEmpty(helpOption.LongName))
                {
                    continue;
                }

                if (ArgumentParser.CompareLong(arg, helpOption.LongName, caseSensitive))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryParseHelpVerb<T>(string[] args, T options, Tuple<MethodInfo, HelpVerbOptionAttribute> helpInfo, OptionMap optionMap)
        {
            var helpWriter = _settings.HelpWriter;
            if (helpInfo != null && helpWriter != null)
            {
                if (string.Compare(args[0], helpInfo.Right().LongName, GetStringComparison(_settings)) == 0)
                {
                    // User explicitly requested help
                    var verb = args.FirstOrDefault();
                    if (verb != null)
                    {
                        var verbOption = optionMap[verb];
                        if (verbOption != null)
                        {
                            if (verbOption.GetValue(options) == null)
                            {
                                // We need to create an instance also to render help
                                verbOption.CreateInstance(options);
                            }
                        }
                    }

                    DisplayHelpVerbText(options, helpInfo, verb);
                    return true;
                }
            }

            return false;
        }

        private void DisplayHelpVerbText<T>(T options, Tuple<MethodInfo, HelpVerbOptionAttribute> helpInfo, string verb)
        {
            string helpText;
            if (verb == null)
            {
                HelpVerbOptionAttribute.InvokeMethod(options, helpInfo, null, out helpText);
            }
            else
            {
                HelpVerbOptionAttribute.InvokeMethod(options, helpInfo, verb, out helpText);
            }

            if (_settings.HelpWriter != null)
            {
                _settings.HelpWriter.Write(helpText);
            }
        }

        private void InvokeAutoBuildIfNeeded<T>(T options)
        {
            if (_settings.HelpWriter == null ||
                options.HasHelpMethod() ||
                options.HasHelpVerbCommandMethod())
            {
                return;
            }

            // We print help text for the user
            _settings.HelpWriter.Write(
                HelpText.AutoBuild(
                    options,
                    current => HelpText.DefaultParsingErrorsHandler(options, current),
                    options.AnyVerbCommands()));
        }

        private void HandleDynamicAutoBuild<T>(T options)
        {
            if (_settings.DynamicAutoBuild)
            {
                InvokeAutoBuildIfNeeded(options);
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_settings != null)
                {
                    _settings.Dispose();
                }

                _disposed = true;
            }
        }
    }
}