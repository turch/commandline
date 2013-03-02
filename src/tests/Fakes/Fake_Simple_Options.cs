namespace CommandLine.Tests.Fakes
{
    class Fake_Simple_Options
    {
        public Fake_Simple_Options()
        {
            IntegerValue = 0;
        }

        [Option('s', "string")]
        public string StringValue { get; set; }

        [Option('i', null)]
        public int IntegerValue { get; set; }

        [Option("switch")]
        public bool BooleanValue { get; set; }
    } 
}