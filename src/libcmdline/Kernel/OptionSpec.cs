using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Extensions;

namespace CommandLine.Kernel
{
    internal class OptionSpec
    {
        public OptionSpec(char? shortName, string longName, bool boundToBool)
        {
            if (shortName == null && longName == null)
            {
                throw new ArgumentException("Both names null.");
            }

            ShortName = shortName;
            LongName = longName;
            BoundToBoolean = boundToBool;

            UniqueName = string.Join("", shortName.HasValue ? shortName.ToInvariantString() : "", longName ?? "");
        }

        public char? ShortName { get; private set; }

        public string LongName { get; private set; }

        public bool BoundToBoolean { get; private set; }

        public string UniqueName { get; private set; }
    }
}