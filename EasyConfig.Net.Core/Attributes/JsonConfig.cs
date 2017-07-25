namespace EasyConfig.Attributes
{
    public class JsonConfig : ConfigurationAttribute
    {
        public JsonConfig(string key) : base(key, "", ConfigurationSources.ConfigFile)
        {
        }
    }
}