namespace EasyConfig.Attributes
{
    public class EnvironmentAttribute : ConfigurationAttribute
    {
        public EnvironmentAttribute(string key, string alias = "") : base(key ,alias, ConfigurationSources.Environment)
        {
        }
    }
}