namespace EasyConfig.ConfigurationReaders
{
    public class EnvironmentVariablesReader : ConfigurationReader
    {
        private readonly EnvironmentWrapper _environment;

        public EnvironmentVariablesReader(EnvironmentWrapper environment)
        {
            _environment = environment;
        }

        public string Get(string key)
        {
            return _environment.GetEnvironmentVariable(key);
        }

        public bool CanBeUsedToReadFrom(ConfigurationSources sources)
        {
            return sources.HasFlag(ConfigurationSources.EnvironmentVariables);
        }
    }
}