using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Infrastructure;

namespace CommandLine.Kernel
{
    internal sealed class ParserStateProperty : IMember
    {
        private readonly PropertyInfo _property;

        public ParserStateProperty(PropertyInfo property)
        {
            _property = property;
        }

        public T MutateParsingErrorCollection<T>(T options, IEnumerable<ParsingError> errors)
        {
            var state = GetValue(options);
            foreach (var err in errors)
            {
                state.Errors.Add(err);
            }
            return options;
        }

        private IParserState GetValue<T>(T options)
        {
            var parserState = _property.GetValue(options, null);

            if (parserState != null)
            {
                GuardAgainstBadParserState(parserState);
            }
            else
            {
                CreateParserState(options);
            }

            return (IParserState)_property.GetValue(options, null);
        }

        private static void GuardAgainstBadParserState(object parserState)
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

        // TODO: move this code in a type that initialize the options class
        private void CreateParserState<T>(T options)
        {
            try
            {
                this._property.SetValue(options, new ParserState(), null);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(SR.InvalidOperationException_ParserStateInstanceBadApplied, ex);
            }
        }
    }
}
