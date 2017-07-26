using System;

namespace EasyConfig.ConfigurationReaders
{
    public class EnvironmentVariablesReader : IConfigurationReader
    {
        public bool TryGet(string key, string alias, ConfigurationSources sources, out string value)
        {
            if (sources.HasFlag(ConfigurationSources.Environment))
            {
                value = Environment.GetEnvironmentVariable(key);
                return !string.IsNullOrWhiteSpace(value);
            }

            value = "";
            return false;
        }
    }
}