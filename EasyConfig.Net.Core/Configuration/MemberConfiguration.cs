using System;
using EasyConfig.Attributes;

namespace EasyConfig.Configuration
{
    public abstract class MemberConfiguration
    {
        protected MemberConfiguration(
            object defaultValue,
            bool hasDefault,
            bool isRequired,
            bool shouldHideInLog,
            ConfigurationAttribute configurationAttribute,
            Type type)
        {
            DefaultValue = defaultValue;
            HasDefault = hasDefault;
            IsRequired = isRequired;
            ShouldHideInLog = shouldHideInLog;
            Key = configurationAttribute.Key;
            Alias = configurationAttribute.Alias;
            ConfigurationSources = configurationAttribute.ConfigurationSources;
            MemberType = type;
        }

        public Type MemberType { get; set; }

        public bool HasDefault { get; }
        public bool IsRequired { get; }
        public bool ShouldHideInLog { get; }
        public object DefaultValue { get; }
        public string Key { get; set; }
        public string Alias { get; set; }
        public ConfigurationSources ConfigurationSources { get; set; }

        public abstract void SetValue(object instance, object value);
    }
}
