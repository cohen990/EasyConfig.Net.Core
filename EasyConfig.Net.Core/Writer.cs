namespace EasyConfig
{
    public interface Writer
    {
        void WriteConfigurationValue(string key, string value);
        void ObfuscateConfigurationValue(string key, string value);
    }
}