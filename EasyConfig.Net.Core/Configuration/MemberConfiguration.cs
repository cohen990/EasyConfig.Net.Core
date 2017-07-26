using System;
using EasyConfig.Attributes;

namespace EasyConfig.Configuration
{
    public abstract class MemberConfiguration
    {
        protected MemberConfiguration(
            object defaultValue,
            bool isRequired,
            bool shouldHideInLog,
            ConfigurationAttribute configurationAttribute,
            ConfigurationSources? overrideSource,
            string overrideKey,
            Type type)
        {
            DefaultValue = defaultValue;
            IsRequired = isRequired;
            ShouldHideInLog = shouldHideInLog;
            Key = configurationAttribute.Key;
            Alias = configurationAttribute.Alias;
            ConfigurationSources = configurationAttribute.ConfigurationSources;
            OverrideSource = overrideSource;
            OverrideKey = overrideKey;
            MemberType = type;
        }

        public ConfigurationSources? OverrideSource { get; set; }
        public string OverrideKey { get; }

        public Type MemberType { get; set; }

        public bool HasDefault => DefaultValue != null;
        public bool IsRequired { get; }
        public bool ShouldHideInLog { get; }
        public object DefaultValue { get; }
        public string Key { get; set; }
        public string Alias { get; set; }
        public ConfigurationSources ConfigurationSources { get; set; }

        public abstract void SetValue(object instance, object value);
    }
}
