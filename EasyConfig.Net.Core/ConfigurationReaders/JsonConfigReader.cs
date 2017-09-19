using System;
using Microsoft.Extensions.Configuration;

namespace EasyConfig.ConfigurationReaders
{
    public class JsonConfigReader : ConfigurationReader
    {
        private readonly IConfigurationRoot _jsonConfig;

        public JsonConfigReader(IConfigurationRoot jsonConfig)
        {
            _jsonConfig = jsonConfig;
        }

        public bool TryGet(string key, out string value)
        {
            try
            {
                value = _jsonConfig[key];
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