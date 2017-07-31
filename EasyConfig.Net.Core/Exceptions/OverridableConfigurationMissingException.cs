using System;
using System.Collections.Generic;
using System.Text;

namespace EasyConfig.Exceptions
{
    public class OverridableConfigurationMissingException : EasyConfigException
    {
        public OverridableConfigurationMissingException(
            string key,
            ConfigurationSources sources,
            string overrideKey,
            ConfigurationSources overrideSources,
            Type type)
            : base(GetMessage(key, sources, overrideKey, overrideSources, type))
        {
        }
        public static string GetMessage(
            string key,
            ConfigurationSources sources,
            string overrideKey,
            ConfigurationSources overrideSources, 
            Type type) {

            var messageBuilder = new StringBuilder();

            messageBuilder.Append($"ERROR: Configuration for key '{key}' was not found. ");
            messageBuilder.Append($"Please supply a {type} at one of the following sources:");

            foreach (var configurationLocation in GetLocations(sources))
            {
                messageBuilder.Append($"{Environment.NewLine}{configurationLocation}");
            }

            messageBuilder.Append($"{Environment.NewLine}It can also be overriden with the key '{overrideKey}' at one of the following sources:");

            foreach (var configurationLocation in GetLocations(overrideSources))
            {
                messageBuilder.Append($"{Environment.NewLine}{configurationLocation}");
            }

            return messageBuilder.ToString();
        }

        static IEnumerable<ConfigurationSources> GetLocations(ConfigurationSources input)
        {
            foreach (ConfigurationSources value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }
    }
}