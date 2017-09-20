namespace EasyConfig.ConfigurationReaders
{
    public interface ConfigurationReader
    {
        string Get(string key);
        bool CanBeUsedToReadFrom(ConfigurationSources sources);
    }
}