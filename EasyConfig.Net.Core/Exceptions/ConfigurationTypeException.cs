using System;

namespace EasyConfig.Exceptions
{
    public class ConfigurationTypeException : EasyConfigException
    {
        public ConfigurationTypeException(string configKey, Type type): base($"Configuration parameter found with '{configKey}' cannot be parsed into '{type}'")
        {
        }
    }
}