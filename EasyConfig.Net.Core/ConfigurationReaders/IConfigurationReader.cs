namespace EasyConfig.ConfigurationReaders
{
    public interface IConfigurationReader
    {
        bool TryGet(string key, string alias, ConfigurationSources sources, out string value);
    }
}