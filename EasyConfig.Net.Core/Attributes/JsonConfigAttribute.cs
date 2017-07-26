namespace EasyConfig.Attributes
{
    public class JsonConfigAttribute : ConfigurationAttribute
    {
        public JsonConfigAttribute(string key) : base(key.Replace('.', ':'), "", ConfigurationSources.JsonConfig)
        {
        }
    }
}