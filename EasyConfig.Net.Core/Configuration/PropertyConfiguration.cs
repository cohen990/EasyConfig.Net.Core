using System;
using System.Reflection;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;

namespace EasyConfig.Configuration
{
    public class PropertyConfiguration : MemberConfiguration
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyConfiguration(
            object defaultValue,
            bool hasDefault,
            bool isRequired,
            bool shouldHideInLog,
            ConfigurationAttribute configurationAttribute,
            PropertyInfo propertyInfo)
            : base(
                defaultValue,
                hasDefault,
                isRequired,
                shouldHideInLog,
                configurationAttribute,
                propertyInfo.PropertyType)
        {
            _propertyInfo = propertyInfo;
        }

        public override void SetValue(object instance, object value)
        {
            try
            {
                _propertyInfo.SetValue(instance, value);
            }
            catch (ArgumentException e)
            {
                throw new ConfigurationPropertyException(_propertyInfo.Name, e);
            }
        }
    }
}
