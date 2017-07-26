using System;

namespace EasyConfig.Attributes
{
    public class OverridenByAttribute : Attribute
    {
        public ConfigurationSources Source { get; }
        public string AlternativeKey { get; }

        public OverridenByAttribute(ConfigurationSources source, string alternativeKey = null)
        {
            Source = source;
            AlternativeKey = alternativeKey;
        }
    }
}