using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Extensions;

namespace CommandLine.Kernel
{
    internal sealed class ThrowingVerbOptionParameterLessCtorGuard : IOptionPropertyGuard
    {
        public void Execute(OptionProperty verbOption)
        {
            if (!verbOption.HasParameterLessCtor) // && verb.UnderlyingProperty.GetValue(options, null) == null)
            {
                //throw new ParserException("Type {0} must have a parameterless constructor or" +
                //    " be already initialized to be used as a verb command.".FormatInvariant(verb.UnderlyingProperty.PropertyType));
                throw new ParserException("Type {0} must have a parameterless constructor to be used as a verb command."
                    .FormatInvariant(verbOption.InnerProperty.PropertyType));
            }
        }
    }
}
