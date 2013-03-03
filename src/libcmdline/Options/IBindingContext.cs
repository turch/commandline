using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Options
{
    interface IBindingContext
    {
        object UnderlyingValue { get; }

        object BuildInstanceIntoUnderlyingValue();
    }
}