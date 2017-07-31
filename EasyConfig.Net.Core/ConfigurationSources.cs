using System;

namespace EasyConfig
{
    [Flags]
    public enum ConfigurationSources
    {
        Environment= 1,
        CommandLine = 2,
        JsonConfig = 4
    }
}