using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Extensions;
using CommandLine.Infrastructure;

namespace CommandLine.Options
{
    // TODO: refactor this class
    sealed class BindingContext<T> //: IUnderlyingBindingContext
    {
        private readonly ParserSettings _settings;
        private readonly OptionProperty _optionInfo;
        private readonly PropertyInfo _property;
        private readonly BaseOptionAttribute _attribute;
        private readonly T _target;

        public BindingContext(ParserSettings settings, OptionProperty optionInfo, T target)
        {
            _settings = settings;
            _optionInfo = optionInfo;
            _property = optionInfo.UnderlyingProperty;
            _attribute = optionInfo.UnderlyingAttribute;
            _target = target;
        }

        public object GetValue()
        {
            return _property.GetValue(_target, null);
        }

        public object SetValueWithBuiltInstance()
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

        public bool SetValue(string value)
        {
            if (_attribute is OptionListAttribute)
            {
                return SetValueList(value);
            }

            if (_property.PropertyType.IsNullable())
            {
                return _optionInfo.ReceivedValue = PropertyWriter.WriteNullable(value, _target, _property, _settings.ParsingCulture);
            }

            return _optionInfo.ReceivedValue = PropertyWriter.WriteScalar(value, _target, _property, _settings.ParsingCulture);
        }

        public bool SetValue(IList<string> values)
        {
            var elementType = _property.PropertyType.GetElementType();
            var array = Array.CreateInstance(elementType, values.Count);

            for (var i = 0; i < array.Length; i++)
            {
                try
                {
                    array.SetValue(Convert.ChangeType(values[i], elementType, _settings.ParsingCulture), i);

                    _property.SetValue(_target, array, null);
                }
                catch (FormatException)
                {
                    return false;
                }
            }

            return _optionInfo.ReceivedValue = true;
        }

        public bool SetValue(bool value)
        {
            _property.SetValue(_target, value, null);

            return _optionInfo.ReceivedValue = true;
        }

        public void SetDefault()
        {
            if (_optionInfo.HasDefaultValue)
            {
                try
                {
                    _property.SetValue(_target, _optionInfo.DefaultValue, null);
                }
                catch (Exception e)
                {
                    throw new ParserException("Bad default value.", e);
                }
            }
        }

        private bool SetValueList(string value)
        {
            _property.SetValue(_target, new List<string>(), null);

            var fieldRef = (IList<string>)_property.GetValue(_target, null);

            var values = value.Split(((OptionListAttribute)_attribute).Separator);

            foreach (var item in values)
            {
                fieldRef.Add(item);
            }

            return _optionInfo.ReceivedValue = true;
        }
    }
}