using System.Collections.Generic;
using CommandLine.Kernel;
using CommandLine.Parsing;
using Xunit;
using FluentAssertions;
using CommandLine;
using CommandLine.Infrastructure;

namespace CommandLine.Tests.Unit.Kernel
{
    public class Kernel_OptionMap_Fixture
    {
        #region Helper Nested Class
        class OptionMapBuilder
        {
            private readonly OptionMap _optionMap;
            private readonly List<OptionProperty> _options;
            private readonly List<string> _names;

            public OptionMapBuilder(int capacity)
            {
                _optionMap = new OptionMap(new ParserSettings(true));
                _options = new List<OptionProperty>(capacity);
                _names = new List<string>(capacity);
            }

            public void AppendOption(string longName)
            {
                var oa = new OptionAttribute(longName);
                var oi = oa.CreateOptionInfo();
                _optionMap[oa.UniqueName] = oi;
                _options.Add(oi);
                _names.Add(oa.UniqueName);
            }

            public void AppendOption(char shortName, string longName)
            {
                var oa = new OptionAttribute(shortName, longName);
                var oi = oa.CreateOptionInfo();
                _optionMap[oa.UniqueName] = oi;
                _options.Add(oi);
                _names.Add(oa.UniqueName);
            }

            public List<OptionProperty> Options
            {
                get { return _options; }
            }

            public List<string> Names
            {
                get { return _names; }
            }

            public OptionMap OptionMap
            {
                get { return _optionMap; }
            }
        }
        #endregion

        [Fact]
        public void Manage_options()
        {
            var omBuilder = new OptionMapBuilder(3);
            omBuilder.AppendOption('p', "pretend");
            omBuilder.AppendOption("newuse");
            omBuilder.AppendOption('D', null);

            var optionMap = omBuilder.OptionMap;

            omBuilder.Options[0].Should().BeSameAs(optionMap[omBuilder.Names[0]]);
            omBuilder.Options[1].Should().BeSameAs(optionMap[omBuilder.Names[1]]);
            omBuilder.Options[2].Should().BeSameAs(optionMap[omBuilder.Names[2]]);
        }

        //[Fact]
        //public void RetrieveNotExistentShortOption()
        //{
        //    var shortOi = _optionMap["y"];
        //    shortOi.Should().BeNull();
        //}

        //[Fact]
        //public void RetrieveNotExistentLongOption()
        //{
        //    var longOi = _optionMap["nomorebugshere"];
        //    longOi.Should().BeNull();
        //}

        private static OptionMap CreateMap (ref OptionMap map, IDictionary<string, OptionProperty> optionCache)
        {
            if (map == null)
            {
                map = new OptionMap(new ParserSettings (true));
            }

            var attribute1 = new OptionAttribute('p', "pretend");
            var attribute2 = new OptionAttribute("newuse");
            var attribute3 = new OptionAttribute('D', null);

            var option1 = attribute1.CreateOptionInfo();
            var option2 = attribute2.CreateOptionInfo();
            var option3 = attribute3.CreateOptionInfo();

            map[attribute1.UniqueName] = option1;
            map[attribute2.UniqueName] = option2;
            map[attribute3.UniqueName] = option3;

            if (optionCache != null)
            {
                optionCache[attribute1.UniqueName] = option1;
                optionCache[attribute1.UniqueName] = option2;
                optionCache[attribute2.UniqueName]= option3;
            }

            return map;
        }
    }
}

