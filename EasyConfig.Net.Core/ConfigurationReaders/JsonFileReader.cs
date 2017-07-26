using System;
using Microsoft.Extensions.Configuration;

namespace EasyConfig.ConfigurationReaders
{
    public class JsonFileReader : IConfigurationReader
    {
        private readonly IConfigurationRoot _config;

        public JsonFileReader(IConfigurationRoot config)
        {
            _config = config;
        }

        public bool TryGet(string key, string alias, ConfigurationSources sources, out string value)
        {
            if (sources.HasFlag(ConfigurationSources.JsonConfig))
            {
                try
                {
                    value = _config[key];
                    return !string.IsNullOrWhiteSpace(value);
                }
                catch
                {
                }
            }

            value = "";
            return false;
        }
    }
}