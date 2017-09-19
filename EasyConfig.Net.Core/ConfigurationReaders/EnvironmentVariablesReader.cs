namespace EasyConfig.ConfigurationReaders
{
    public class EnvironmentVariablesReader : ConfigurationReader
    {
        private readonly EnvironmentWrapper _environment;

        public EnvironmentVariablesReader(EnvironmentWrapper environment)
        {
            _environment = environment;
        }

        public bool TryGet(string key, out string value)
        {
            value = _environment.GetEnvironmentVariable(key);
            return !string.IsNullOrWhiteSpace(value);
        }

        public bool CanBeUsedToReadFrom(ConfigurationSources sources)
        {
            return sources.HasFlag(ConfigurationSources.EnvironmentVariables);
        }
    }
}