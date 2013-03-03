using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Parsing
{
    internal static class OptionInfoFactory
    {
        public static OptionInfo FromMetadata(BaseOptionAttribute attribute, PropertyInfo property, CultureInfo parsingCulture)
        {
            var longName = attribute.LongName;

            return new OptionInfo
                {
                    ShortName = attribute.ShortName,
                    LongName = longName,
                    Required = attribute.Required,
                    MutuallyExclusiveSet = attribute.MutuallyExclusiveSet,
                    DefaultValue = attribute.DefaultValue,
                    HasDefaultValue = attribute.HasDefaultValue,
                    IsBoolean = property.PropertyType == typeof(bool),
                    IsArray = property.PropertyType.IsArray,
                    IsAttributeArrayCompatible = attribute is OptionArrayAttribute,
                    HasBothNames = attribute.ShortName.HasValue && longName != null,
                    HasParameterLessCtor = property.PropertyType.GetConstructor(Type.EmptyTypes) != null,
                    ParsingCulture = parsingCulture,

                    InnerProperty = property,
                    InnerAttribute = attribute,
                };
        }
    }
}