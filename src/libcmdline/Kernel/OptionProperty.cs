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
    internal class OptionProperty : IProperty,  IOptionName
    {
        private readonly string _shortName;
        private readonly string _longName;

        public OptionProperty(PropertyInfo property, IOptionAttribute attribute)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            _shortName = attribute.ShortName.HasValue
                             ? Convert.ToString(attribute.ShortName, CultureInfo.InvariantCulture)
                             : string.Empty;
            
            _longName = _longName ?? property.Name;
        }

        public string ShortName
        {
            get { return _shortName; }
        }

        public string LongName
        {
            get { return _longName; }
        }
    }
}