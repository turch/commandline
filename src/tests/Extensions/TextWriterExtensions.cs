using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Extensions
{
    static class TextWriterExtensions
    {
        public static string[] AsLines(this TextWriter writer)
        {
            return writer == null ?
                new string[] {} :
                writer.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
