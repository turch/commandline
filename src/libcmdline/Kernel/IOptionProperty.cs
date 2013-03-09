﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal interface IOptionProperty : IProperty
    {
        string ShortName { get;  }

        string LongName { get; }
    }
}