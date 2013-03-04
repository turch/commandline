using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Options
{
    interface IUnderlyingBindingContext
    {
        object GetValue();

        object SetValueWithBuiltInstance();

        bool SetValue(string value);

        bool SetValue(IList<string> values);

        bool SetValue(bool value);

        void SetDefault();
    }
}