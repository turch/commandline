using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Kernel
{
    internal sealed class ValueListProperty : IProperty
    {
        private readonly PropertyInfo _property;
        private readonly ValueListAttribute _attribute;

        public ValueListProperty(PropertyInfo property, ValueListAttribute attribute)
        {
            _property = property;
            _attribute = attribute;
        }

        public IList<string> GetList<T>(T target)
        {
            var concreteType = _attribute.ConcreteType;

            _property.SetValue(target, Activator.CreateInstance(concreteType), null);

            return (IList<string>)_property.GetValue(target, null);
        }

        public int MaximumElements
        {
            get { return _attribute.MaximumElements; }
        }
    }
}
