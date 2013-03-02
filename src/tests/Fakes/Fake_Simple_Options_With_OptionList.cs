using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class Fake_Simple_Options_With_OptionList : Fake_Simple_Options
    {
        [OptionList('k', "keywords", ':')]
        public IList<string> SearchKeywords { get; set; }
    }
}