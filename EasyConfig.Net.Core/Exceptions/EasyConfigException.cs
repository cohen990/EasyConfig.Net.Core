using System;

namespace EasyConfig.Exceptions
{
    public class EasyConfigException : Exception
    {
        public EasyConfigException(string message) : base(message)
        {
        }
        public EasyConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EnumConfigParseException : EasyConfigException
    {
        public EnumConfigParseException(string failedParse, Type enumType)
            : base($"Failed to parse an enum of type '{enumType}' from '{failedParse}'")
        {
        }
    }
}