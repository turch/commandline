using CommandLine.Text;

namespace CommandLine.Tests.Unit.Attributes
{
    public class Fake_With_Help
    {
        [Option('i', "input", Required = true, HelpText = "Input file with equations, xml format (see manual).")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output file with results, otherwise standard output.")]
        public string OutputFile { get; set; }

        [Option("paralell", Required = false, HelpText = "Paralellize processing in multiple threads.")]
        public bool ParalellizeProcessing { get; set; }

        [Option('v', null, Required = false, HelpText = "Show detailed processing messages.")]
        public bool Verbose { get; set; }

        [HelpOption(HelpText = "Display this screen.")]
        public string GetUsage()
        {
            var help = new HelpText(new HeadingInfo("MyProgram", "1.0"));
            help.Copyright = new CopyrightInfo("Authors, Inc.", 2007);
            help.AddPreOptionsLine("This software is under the terms of the XYZ License");
            help.AddPreOptionsLine("(http://license-text.org/show.cgi?xyz).");
            help.AddPreOptionsLine("Usage: myprog --input equations-file.xml -o result-file.xml");
            help.AddPreOptionsLine("       myprog -i equations-file.xml --paralell");
            help.AddPreOptionsLine("       myprog -i equations-file.xml -vo result-file.xml");
            help.AddOptions(this);
            return help;
        }
    }
}