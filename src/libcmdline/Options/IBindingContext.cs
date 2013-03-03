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

        bool SetValue(string value);

        bool SetValue(IList<string> values);

        bool SetValue(bool value);

        void SetDefault();
    }
}