using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Kernel
{
    internal class Likeness<TOption, TToken> : IEquatable<TToken>
        where TOption : IProperty
        where TToken : IToken
    {
        private readonly TOption _option;
        private readonly StringComparer _comparer;

        public Likeness(TOption option, StringComparer comparer)
        {
            _option = option;
        }

        public bool Equals(TToken other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            
            if (other is LongOptionToken)
            {
                
            }

            return false;
        }
    }
}
