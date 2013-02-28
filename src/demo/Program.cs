#region License
// <copyright file="Program.cs" company="Giacomo Stelluti Scala">
//   Copyright 2015-2013 Giacomo Stelluti Scala
// </copyright>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
#region Using Directives
using System;
using System.Text;
using CommandLine.Text;
#endregion

namespace CommandLine.Demo
{
    sealed partial class Program
    {
        private static readonly HeadingInfo HeadingInfo = new HeadingInfo("sampleapp", "1.8");

        /// <summary>
        /// Application's Entry Point.
        /// </summary>
        /// <param name="args">Command line arguments splitted by the system.</param>
        private static void Main(string[] args)
        {
            var options = CommandLine.Parser.Default.ParseArguments<Options>(args, () => Environment.Exit(-2));

            Run(options);
        }

        private static void Run(Options options)
        {
            if (options.VerboseLevel == null)
            {
                Console.WriteLine("verbose [off]");
            }
            else
            {
                Console.WriteLine(
                    "verbose [on]: {0}",
                    options.VerboseLevel < 0 || options.VerboseLevel > 2 ? "#invalid value#" : options.VerboseLevel.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("input file: {0} ...", options.InputFile);

            foreach (string defFile in options.DefinitionFiles)
            {
                Console.WriteLine("  using definition file: {0}", defFile);
            }

            Console.WriteLine("  start offset: {0}", options.StartOffset);
            Console.WriteLine("  tabular data computation: {0}", options.Calculate.ToString().ToLowerInvariant());
            Console.WriteLine("  on errors: {0}", options.IgnoreErrors ? "continue" : "stop processing");
            Console.WriteLine("  optimize for: {0}", options.Optimization.ToString().ToLowerInvariant());

            if (options.AllowedOperators != null)
            {
                var builder = new StringBuilder();
                builder.Append("  allowed operators: ");

                foreach (string op in options.AllowedOperators)
                {
                    builder.Append(op);
                    builder.Append(", ");
                }

                Console.WriteLine(builder.Remove(builder.Length - 2, 2).ToString());
            }

            Console.WriteLine();

            if (!string.IsNullOrEmpty(options.OutputFile))
            {
                HeadingInfo.WriteMessage(string.Format("writing elaborated data: {0} ...", options.OutputFile));
            }
            else
            {
                HeadingInfo.WriteMessage("elaborated data:");
                Console.WriteLine("[...]");
            }
        }
    }
}