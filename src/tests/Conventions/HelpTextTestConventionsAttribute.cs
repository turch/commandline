using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;

namespace CommandLine.Tests.Conventions
{
    public class HelpTextTestConventionsAttribute : AutoDataAttribute
    {
        public HelpTextTestConventionsAttribute()
            : base(new Fixture().Customize(new AutoMoqCustomization()))
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
    }
}