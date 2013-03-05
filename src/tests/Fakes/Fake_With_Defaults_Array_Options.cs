using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class Fake_With_Defaults_Array_Options : Fake_With_Defaults_Options
    {
        [OptionArray('z', "strarr", DefaultValue = new[] { "a", "b", "c" })]
        public string[] StringArrayValue { get; set; }

        [OptionArray('y', "intarr", DefaultValue = new[] { 1, 2, 3 })]
        public int[] IntegerArrayValue { get; set; }

        [OptionArray('q', "dblarr", DefaultValue = new[] { 1.1d, 2.2d, 3.3d })]
        public double[] DoubleArrayValue { get; set; }
    }
}