using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine.Extensions;
using CommandLine.Infrastructure;

namespace CommandLine.Kernel
{
    internal class HelpVerbOptionMethod : IMember
    {
        private readonly MethodInfo _method;
        private readonly HelpVerbOptionAttribute _attribute;
        private readonly string _longName;

        public HelpVerbOptionMethod(MethodInfo method, HelpVerbOptionAttribute attribute)
        {
            _method = method;
            _attribute = attribute;
            _longName = attribute.LongName;
        }

        public string LongName
        {
            get { return _longName; }
        }

        public void Invoke<T>(
            T target,
            string verb,
            out string text)
        {
            if (!CheckMethodSignature(_method))
            {
                throw new MemberAccessException(
                    SR.MemberAccessException_BadSignatureForHelpVerbOptionAttribute.FormatInvariant(_method.Name));
            }

            text = (string)_method.Invoke(target, new object[] { verb });
        }

        private static bool CheckMethodSignature(MethodInfo value)
        {
            if (value.ReturnType == typeof(string) && value.GetParameters().Length == 1)
            {
                return value.GetParameters()[0].ParameterType == typeof(string);
            }

            return false;
        }
    }
}
