using System;
using Microsoft.Extensions.Configuration;

namespace EasyConfig.ConfigurationReaders
{
    public class JsonFileReader : ConfigurationReader
    {
        private readonly IConfigurationRoot _config;

        public JsonFileReader(IConfigurationRoot config)
        {
            _config = config;
        }

        public bool TryGet(string key, string alias, out string value)
        {
            try
            {
                value = _config[key];
                return !string.IsNullOrWhiteSpace(value);
            }
            catch
            {
            }
            value = "";
            return false;
        }

        public bool CanBeUsedToReadFrom(ConfigurationSources sources)
        {
            return sources.HasFlag(ConfigurationSources.JsonConfig);
        }
    }
}