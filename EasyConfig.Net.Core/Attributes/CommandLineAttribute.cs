namespace EasyConfig.Attributes
{
    public class CommandLineAttribute : ConfigurationAttribute
    {
        public CommandLineAttribute(string key, string alias = "") : base(key, alias, ConfigurationSources.CommandLine)
        {
        }
    }
}