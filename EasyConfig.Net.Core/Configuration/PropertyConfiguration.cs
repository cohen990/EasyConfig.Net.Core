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
            bool isRequired,
            bool shouldHideInLog,
            ConfigurationAttribute configurationAttribute,
            ConfigurationSources? overrideSource,
            string overrideKey,
            PropertyInfo propertyInfo)
            : base(
                defaultValue,
                isRequired,
                shouldHideInLog,
                configurationAttribute,
                overrideSource,
                overrideKey,
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
