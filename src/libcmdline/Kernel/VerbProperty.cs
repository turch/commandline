using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Infrastructure;

namespace CommandLine.Kernel
{
    internal sealed class VerbProperty : IMember
    {
        private readonly PropertyInfo _property;
        private readonly VerbAttribute _attribute;
        private readonly string _name;

        public VerbProperty(PropertyInfo property, VerbAttribute attribute)
        {
            _property = property;
            _attribute = attribute;
            _name = attribute.Name;
        }

        public string Name
        {
            get { return _name; }
        }

        // TODO: move code like this in a sort of target initilizer
        public object SetValueWithBuiltInstance(object target)
        {
            object instance;

            try
            {
                instance = Activator.CreateInstance(_property.PropertyType);

                _property.SetValue(target, instance, null);
            }
            catch (Exception e)
            {
                throw new ParserException(SR.CommandLineParserException_CannotCreateInstanceForVerbCommand, e);
            }

            return instance;
        }

        public object GetValue(object target)
        {
            return _property.GetValue(target, null);
        }
    }
}
