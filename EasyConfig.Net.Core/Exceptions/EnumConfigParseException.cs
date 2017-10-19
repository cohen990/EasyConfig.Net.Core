using System;

namespace EasyConfig.Exceptions
{
    public class EnumConfigParseException : EasyConfigException
    {
        public EnumConfigParseException(string failedParse, Type enumType)
            : base($"Failed to parse an enum of type '{enumType}' from '{failedParse}'")
        {
        }
    }
}