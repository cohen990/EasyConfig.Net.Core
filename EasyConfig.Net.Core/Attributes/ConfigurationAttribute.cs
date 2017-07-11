using System;

namespace EasyConfig.Attributes
{
    public class ConfigurationAttribute : Attribute
    {
        public string Key { get; }

        public string Alias { get; }

        public ConfigurationSources ConfigurationSources;

        public ConfigurationAttribute(string key, string alias, ConfigurationSources configurationSources)
        {
            ConfigurationSources = configurationSources;
            Key = key;
            Alias = alias;
        }

    }
}