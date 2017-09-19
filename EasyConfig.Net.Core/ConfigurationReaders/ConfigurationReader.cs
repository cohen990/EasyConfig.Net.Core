namespace EasyConfig.ConfigurationReaders
{
    public interface ConfigurationReader
    {
        bool TryGet(string key, out string value);
        bool CanBeUsedToReadFrom(ConfigurationSources sources);
    }
}