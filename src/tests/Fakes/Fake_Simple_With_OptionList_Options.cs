using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class Fake_Simple_With_OptionList_Options : Fake_Simple_Options
    {
        [OptionList('k', "keywords", ':')]
        public IList<string> SearchKeywords { get; set; }
    }
}