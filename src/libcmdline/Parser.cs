#region License
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
using CommandLine.Kernel;
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

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "singleton", Justification = "The constructor that accepts a boolean is designed to support default singleton, the parameter is ignored.")]
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

        internal ParserSettings Settings
        {
            get { return _settings; }
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

            var resultAndOptionsAndVerbInstance = this.ParseArgumentsImplVerbs<T>(args);

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

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "By design.")]
        internal static object InternalGetVerbOptionsInstanceByName<T>(string verb, T target, out bool found)
        {
            found = false;
            if (string.IsNullOrEmpty(verb))
            {
                return target;
            }

            var verbs = new VerbOptionPropertyQuery().SelectMembers(target.GetType()).Cast<OptionProperty>();
            var verbOption = verbs.SingleOrDefault(a => string.CompareOrdinal(a.UniqueName, verb) == 0);
            found = verbOption != null;
            return found ? verbOption.InnerProperty.GetValue(target, null) : target;
        }

        private static T SetParserStateIfNeeded<T>(T options, IEnumerable<ParsingError> errors)
        {
            var properties = new ParserStatePropertyQuery().SelectMembers(options.GetType());
            if (properties.OfType<ParserStateProperty>().Any())
            {
                return ((ParserStateProperty)properties.First()).MutateParsingErrorCollection(options, errors);
            }
            return options;
        }

        private static void DisplayHelpText<T>(T options, HelpOptionMethod helpMethod, TextWriter helpWriter)
        {
            string helpText;
            helpMethod.Invoke(options, out helpText);
            helpWriter.Write(helpText);
        }

        private Tuple<bool, T> ParseArguments<T>(string[] args)
            where T : new()
        {
            var options = new T();
             
            var methods = new HelpOptionMethodQuery().SelectMembers(options.GetType());

            var helpWriter = _settings.HelpWriter;

            if (methods.OfType<HelpOptionMethod>().Any() && helpWriter != null)
            {
                var helpMethod = (HelpOptionMethod)methods.First();
                if (this.TryParseHelp(args, helpMethod))
                {
                    DisplayHelpText(options, helpMethod, helpWriter);
                    return new Tuple<bool, T>(false, options);
                }

                var optionsAndResult = this.ParseArgumentsImpl(args, options);
                var result = optionsAndResult.Item1;
                options = optionsAndResult.Item2;

                if (!result)
                {
                    DisplayHelpText(options, helpMethod, helpWriter);
                    return new Tuple<bool, T>(false, options);
                }
            }

            return this.ParseArgumentsImpl(args, options);
        }

        private Tuple<bool, T> ParseArgumentsImpl<T>(string[] args, T options)
        {
            var hadError = false;
            var props = new OptionPropertyQuery().SelectMembers(options.GetType());
            var optionMap = OptionMap.Create(_settings, options, new NullOptionPropertyGuard(), props);
            optionMap.SetDefaults(options);
            var unboundValues = new UnboundValues<T>(options, _settings.ParsingCulture);
            var parsingErrors = Enumerable.Empty<ParsingError>();
            var factory = new ArgumentParserFactory<T>(options, optionMap, _settings);

            var arguments = new StringArrayEnumerator(args);
            while (arguments.MoveNext())
            {
                var argument = arguments.Current;
                if (string.IsNullOrEmpty(argument))
                {
                    continue;
                }

                var parser = factory.Create(argument);

                // TODO: this need to be refactored
                if (parser is NullArgumentParser<T> &&
                    unboundValues.CanWrite)
                {
                    if (!unboundValues.WriteValue(argument))
                    {
                        hadError = true;
                    }
                    continue;
                }

                var result = parser.Parse(arguments);

                if (result is FailureTransition)
                {
                    parsingErrors = parsingErrors.Concat(result.ParsingErrors);
                    hadError = true;
                    continue;
                }

                if (result is MoveNextTransition)
                {
                    arguments.MoveNext();
                }
            }

            options = SetParserStateIfNeeded(options, parsingErrors);
            hadError |= !optionMap.EnforceRules();

            return new Tuple<bool, T>(!hadError, options);
        }

        private Tuple<bool, T, object> ParseArgumentsImplVerbs<T>(string[] args)
            where T : new()
        {
            var options = new T();

            var verbs = new VerbOptionPropertyQuery().SelectMembers(options.GetType());
            var methods = new HelpVerbOptionMethodQuery().SelectMembers(options.GetType());
            var hasHelpMethod = methods.OfType<HelpVerbOptionMethod>().Any();

            HelpVerbOptionMethod helpInfo = null;
            if (hasHelpMethod)
            {
                helpInfo = (HelpVerbOptionMethod)methods.First();
            }

            if (args.Length == 0)
            {
                if (hasHelpMethod || _settings.HelpWriter != null)
                {
                    DisplayHelpVerbText(options, helpInfo, null);
                }

                return new Tuple<bool, T, object>(false, options, null);
            }

            var optionMap = OptionMap.Create(
                _settings,
                options,
                new ThrowingVerbOptionParameterLessCtorGuard(),
                verbs);

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

            var bindingContext = new BindingContext<T>(_settings, verbOption, options);
            var verbInstance = bindingContext.GetValue();
            if (verbInstance == null)
            {
                // Developer has not provided a default value and did not assign an instance
                verbInstance = bindingContext.SetValueWithBuiltInstance();
            }

            var resultAndVerbInstance = this.ParseArgumentsImpl(args.Skip(1).ToArray(), verbInstance);
            var result = resultAndVerbInstance.Item1;
            verbInstance = resultAndVerbInstance.Item2;

            if (!result && helpInfo != null)
            {
                // Particular verb parsing failed, we try to print its help
                DisplayHelpVerbText(options, helpInfo, args.First());
            }

            return new Tuple<bool, T, object>(result, options, verbInstance);
        }

        private bool TryParseHelp(string[] args, HelpOptionMethod helpOption)
        {
            var caseSensitive = _settings.CaseSensitive;
            foreach (var arg in args)
            {
                if (helpOption.ShortName != null)
                {
                    if (ArgumentComparer.CompareAsShortNameOption(arg, helpOption.ShortName, caseSensitive))
                    {
                        return true;
                    }
                }

                if (string.IsNullOrEmpty(helpOption.LongName))
                {
                    continue;
                }

                if (ArgumentComparer.CompareAsLongNameOption(arg, helpOption.LongName, caseSensitive))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryParseHelpVerb<T>(string[] args, T options, HelpVerbOptionMethod helpInfo, OptionMap optionMap)
        {
            var helpWriter = _settings.HelpWriter;
            if (helpInfo != null && helpWriter != null)
            {
                if (string.Compare(args[0], helpInfo.LongName, _settings.StringComparison) == 0)
                {
                    // User explicitly requested help
                    var verb = args.FirstOrDefault();
                    if (verb != null)
                    {
                        var verbOption = optionMap[verb];
                        if (verbOption != null)
                        {
                            var bindingContext = new BindingContext<T>(_settings, verbOption, options);
                            if (bindingContext.GetValue() == null)
                            {
                                // We need to create an instance also to render help
                                bindingContext.SetValueWithBuiltInstance();
                            }
                        }
                    }

                    DisplayHelpVerbText(options, helpInfo, verb);
                    return true;
                }
            }

            return false;
        }

        private void DisplayHelpVerbText<T>(T options, HelpVerbOptionMethod method, string verb)
        {
            string helpText;

            method.Invoke(options, verb, out helpText);

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