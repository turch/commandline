using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine.Infrastructure;

namespace CommandLine.Kernel
{
    internal enum ParsedValueErrorKind
    {
        NotFound = 1,

    }

    internal class ParsedValue
    {
        //public ParsedValue(string name, string value, OptionSpec spec)
        //{
        //    Assumes.NotNull(name, "name");
        //    Assumes.NotNull(value, "value");
        //    Assumes.NotNull(spec, "spec");

        //    Name = name;
        //    Value = value;
        //    Specification = spec;
        //}

        //public ParsedValue(string value)
        //{
        //    Assumes.NotNull(value, "value");

        //    Value = value;
        //}

        public string Name { get; set; }

        public string Value { get; set; }

        public OptionSpec Specification { get; set; }

        public ParsedValueErrorKind? Error { get; set; }
    }
}