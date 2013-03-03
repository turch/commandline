using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Infrastructure;

namespace CommandLine.Options
{
    public class BindingContext<T> : IBindingContext
    {
        private readonly BaseOptionAttribute _attribute;
        private readonly PropertyInfo _property;
        private readonly object _target;

        public BindingContext(BaseOptionAttribute attribute, PropertyInfo property, T target)
        {
            _attribute = attribute;
            _property = property;
            _target = target;
        }

        public object UnderlyingValue
        {
            get
            {
                return _property.GetValue(_target, null);
            }
        }

        public object BuildInstanceIntoUnderlyingValue()
        {
            object instance;

            try
            {
                instance = Activator.CreateInstance(_property.PropertyType);

                _property.SetValue(_target, instance, null);
            }
            catch (Exception e)
            {
                throw new ParserException(SR.CommandLineParserException_CannotCreateInstanceForVerbCommand, e);
            }

            return instance;
        }
    }
}