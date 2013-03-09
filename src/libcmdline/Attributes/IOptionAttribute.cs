using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine
{
    /// <summary>
    /// Named option interface.
    /// </summary>
    public interface IOptionAttribute
    {
        char? ShortName { get; }

        string LongName { get;  }
    }
}
