namespace EasyConfig.ConfigurationReaders
{
    public class SystemEnvironmentWrapper : EnvironmentWrapper
    {
        public string GetEnvironmentVariable(string key)
        {
            return System.Environment.GetEnvironmentVariable(key);
        }
    }
}