namespace CommandLine.Tests.Fakes
{
    class Fake_With_Bad_Defaults_Options
    {
        [Option('s', "string", DefaultValue = "str")]
        public string StringValue { get; set; }

        [Option('i', null, DefaultValue = "bad")]
        public int IntegerValue { get; set; }

        [Option("switch", DefaultValue=true)]
        public bool BooleanValue { get; set; }
    } 
}