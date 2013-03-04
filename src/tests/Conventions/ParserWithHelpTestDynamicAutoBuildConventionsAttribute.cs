using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;

namespace CommandLine.Tests.Conventions
{
    public class ParserWithHelpTestDynamicAutoBuildConventionsAttribute : AutoDataAttribute
    {
        public ParserWithHelpTestDynamicAutoBuildConventionsAttribute()
            : base(new Fixture().Customize(new AutoMoqCustomization()))
        {
            Fixture.Register<Parser>(
                () => new Parser(with =>
                    {
                        with.HelpWriter = new StringWriter();
                        with.DynamicAutoBuild = true;
                    }));
        }
    }
}