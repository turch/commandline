namespace CommandLine.Tests.Fakes
{
    class Fake_Simple_With_ParserState_Options
    {
        [Option('s', "string")]
        public string StringValue { get; set; }

        [Option('i', null)]
        public int IntegerValue { get; set; }

        [Option("switch")]
        public bool BooleanValue { get; set; }

        [ParserState]
        public IParserState ParserState { get; set; }
    } 
}