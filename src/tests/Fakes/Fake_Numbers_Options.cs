namespace CommandLine.Tests.Fakes
{
    class Fake_Numbers_Options
    {
        [Option('b', "byte")]
        public byte ByteValue { get; set; }

        [Option('s', "short")]
        public short ShortValue { get; set; }

        [Option('i', "int")]
        public int IntegerValue { get; set; }

        [Option('l', "long")]
        public long LongValue { get; set; }

        [Option('f', "float")]
        public float FloatValue { get; set; }

        [Option('d', "double")]
        public double DoubleValue { get; set; }

        [Option("n-double")]
        public double? NullableDoubleValue { get; set; }
    }
}