using System;

namespace EasyConfig.Exceptions
{
    public class EasyConfigException : Exception
    {
        public EasyConfigException(string message) : base(message)
        {
        }
    }
}