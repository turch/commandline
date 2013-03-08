using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace CommandLine.Kernel
{
    /// <summary>
    /// Encapsulates a property that defines an option specification.
    /// </summary>
    internal class OptionProperty
    {
        public OptionProperty(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
        }
    }
}
