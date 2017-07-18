using System;
using System.Reflection;
using EasyConfig.Attributes;

namespace EasyConfig.Configuration
{
    public class FieldConfiguration : MemberConfiguration
    {
        private readonly FieldInfo _fieldInfo;

        public FieldConfiguration(
            object defaultValue,
            bool hasDefault,
            bool isRequired,
            bool shouldHideInLog,
            ConfigurationAttribute configurationAttribute,
            FieldInfo fieldInfo) 
            : base(
                  defaultValue,
                  hasDefault,
                  isRequired,
                  shouldHideInLog,
                  configurationAttribute,
                  fieldInfo.FieldType)
        {
            _fieldInfo = fieldInfo;
        }

        public override void SetValue(object instance, object value)
        {
            _fieldInfo.SetValue(instance, value);
        }
    }
}
