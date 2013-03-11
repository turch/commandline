using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

        // TODO: exposing these members will be replaced with logic inserted in this class

        public PropertyInfo InnerProperty { get { return _property; } } 

        //public ValueOptionAttribute InnerAttribute { get { return _attribute; } }
    }
}
