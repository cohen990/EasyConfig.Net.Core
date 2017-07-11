namespace EasyConfig.Attributes
{
    public class EnvironmentOrCommandLineAttribute : ConfigurationAttribute
    {
        public EnvironmentOrCommandLineAttribute(string key, string alias = "") : base(key, alias, ConfigurationSources.CommandLine | ConfigurationSources.Environment)
        {
        }
    }
}