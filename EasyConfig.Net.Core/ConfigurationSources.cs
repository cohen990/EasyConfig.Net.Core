using System;

namespace EasyConfig
{
    [Flags]
    public enum ConfigurationSources
    {
        Environment,
        CommandLine,
        JsonConfig
    }
}