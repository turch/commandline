using System;
using System.ComponentModel;

namespace CommandLine.Tests.Fakes
{
    class Fake_Booleans_Options //: OptionsBase
    {
        public Fake_Booleans_Options()
        {
            this.DoubleValue = 0;
        }

        [Option('a', "option-a")]
        public bool BooleanA { get; set; }

        [Option('b', "option-b")]
        public bool BooleanB { get; set; }

        [Option('c', "option-c")]
        public bool BooleanC { get; set; }

        [Option('d', "double")]
        public double DoubleValue { get; set; }
    }
}