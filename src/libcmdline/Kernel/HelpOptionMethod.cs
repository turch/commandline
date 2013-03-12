using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Kernel
{
    internal class HelpOptionMethod : IMember
    {
        private readonly MethodInfo _method;
        private readonly HelpOptionAttribute _attribute;
        private readonly char? _shortName;
        private readonly string _longName;

        public HelpOptionMethod(MethodInfo method, HelpOptionAttribute attribute)
        {
            _method = method;
            _attribute = attribute;
            _shortName = attribute.ShortName;
            _longName = attribute.LongName;
        }

        public char? ShortName
        {
            get { return _shortName; }
        }

        public string LongName
        {
            get { return _longName; }
        }

        public void Invoke<T>(T options, out string text)
        {
            text = null;

            if (!CheckMethodSignature(_method))
            {
                throw new MemberAccessException();
            }

            text = (string)_method.Invoke(options, null);
        }

        private static bool CheckMethodSignature(MethodInfo value)
        {
            return value.ReturnType == typeof(string) && value.GetParameters().Length == 0;
        }
    }
}
