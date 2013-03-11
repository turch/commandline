using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Infrastructure;

namespace CommandLine.Kernel
{
    internal sealed class ParserStateProperty : IProperty
    {
        private readonly PropertyInfo _property;

        public ParserStateProperty(PropertyInfo property)
        {
            _property = property;
        }

        public IParserState GetValue<T>(T options)
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
