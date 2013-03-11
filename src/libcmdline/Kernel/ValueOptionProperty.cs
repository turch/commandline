using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine.Extensions;

namespace CommandLine.Kernel
{
    internal sealed class ValueOptionProperty : IProperty
    {
        private readonly PropertyInfo _property;
        private readonly ValueOptionAttribute _attribute;
        private readonly int _index;

        public ValueOptionProperty(PropertyInfo propertyInfo, ValueOptionAttribute attribute)
        {
            _property = propertyInfo;
            _attribute = attribute;
            _index = attribute.Index;
        }

        public int Index
        {
            get { return _index; }
        }

        public bool MutateValue<T>(T target, string value, CultureInfo parsingCulture)
        {
            if (_property.PropertyType.IsNullable())
            {
                return PropertyWriter.WriteNullable(value, target, _property, parsingCulture);
            }
            return PropertyWriter.WriteScalar(value, target, _property, parsingCulture);
        }
    }
}
