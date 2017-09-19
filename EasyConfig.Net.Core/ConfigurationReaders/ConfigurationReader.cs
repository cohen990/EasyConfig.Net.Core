namespace EasyConfig.ConfigurationReaders
{
    public interface ConfigurationReader
    {
        bool TryGet(string key, string alias, out string value);
        bool CanBeUsedToReadFrom(ConfigurationSources sources);
    }
}