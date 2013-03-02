namespace CommandLine.Tests.Fakes
{
    class Fake_With_Defaults_Options
    {
        [Option('s', "string", DefaultValue = "str")]
        public string StringValue { get; set; }

        [Option('i', null, DefaultValue = 9)]
        public int IntegerValue { get; set; }

        [Option("switch", DefaultValue=  true)]
        public bool BooleanValue { get; set; }
    } 
}