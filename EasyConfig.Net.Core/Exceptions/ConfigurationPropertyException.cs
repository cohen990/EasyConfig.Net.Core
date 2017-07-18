using System;

namespace EasyConfig.Exceptions
{
    public class ConfigurationPropertyException : EasyConfigException
    {
        public ConfigurationPropertyException(string propertyName, Exception innerException)
            : base($"The property '{propertyName}' needs a public setter.", innerException)
        {
        }
    }
}