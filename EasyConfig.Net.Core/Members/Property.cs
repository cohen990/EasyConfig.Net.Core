using System;
using System.Reflection;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;

namespace EasyConfig.Members
{
    public class Property : Member
    {
        private readonly PropertyInfo _propertyInfo;

        public Property(
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
