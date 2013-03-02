using System.IO;

namespace CommandLine.Tests.Fakes
{
    class Fake_Simple_Options_With_Enum : Fake_Simple_Options
    {
        [Option('a', "access", Required = true)]
        public FileAccess FileAccessValue { get; set; }

        [Option('x', "attributes", Required = true)]
        public FileAttributes FileAttributesValue { get; set; }
    }
}