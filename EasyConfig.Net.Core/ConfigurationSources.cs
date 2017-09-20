using System;

namespace EasyConfig
{
    [Flags]
    public enum ConfigurationSources
    {
        EnvironmentVariables= 1,
        CommandLine = 2,
        JsonConfig = 4
    }
}