using System.Collections.Generic;
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

        public string Get(string key)
        {
            try
            {
                return _jsonConfig[key];
            }
            catch (KeyNotFoundException e)
            {
                return "";
            }
        }

        public bool CanBeUsedToReadFrom(ConfigurationSources sources)
        {
            return sources.HasFlag(ConfigurationSources.JsonConfig);
        }
    }
}