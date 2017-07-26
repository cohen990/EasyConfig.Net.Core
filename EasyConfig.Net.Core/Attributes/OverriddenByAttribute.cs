using System;

namespace EasyConfig.Attributes
{
    public class OverriddenByAttribute : Attribute
    {
        public ConfigurationSources Source { get; }
        public string AlternativeKey { get; }

        public OverriddenByAttribute(ConfigurationSources source, string alternativeKey = null)
        {
            Source = source;
            AlternativeKey = alternativeKey;
        }
    }
}