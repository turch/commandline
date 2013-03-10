using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;


namespace CommandLine.Kernel
{
    /// <summary>
    /// Encapsulates a property that defines an option specification.
    /// </summary>
    internal class OptionProperty : IProperty,  IOptionSpecifiaction
    {
        private readonly string _shortName;
        private readonly string _longName;
        private readonly bool _required;
        private readonly string _setName;

        public OptionProperty(PropertyInfo property, IOptionAttribute attribute)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            _shortName = attribute.ShortName.HasValue
                             ? attribute.ShortName.Value.ToOptionName()
                             : string.Empty;
            
            _longName = _longName ?? property.Name;
            _required = attribute.Required;
            _setName = attribute.MutuallyExclusiveSet;
        }

        public string ShortName
        {
            get { return _shortName; }
        }

        public string LongName
        {
            get { return _longName; }
        }

        public bool Required
        {
            get { return _required; }
        }

        public string SetName
        {
            get { return _setName; }
        }
    }
}