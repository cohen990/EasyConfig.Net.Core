namespace EasyConfig.ConfigurationReaders
{
    public interface EnvironmentWrapper
    {
        string GetEnvironmentVariable(string key);
    }
}