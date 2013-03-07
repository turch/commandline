using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal abstract class Token : IToken
    {
        private readonly string _text;

        protected Token(string text)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }
    }
}