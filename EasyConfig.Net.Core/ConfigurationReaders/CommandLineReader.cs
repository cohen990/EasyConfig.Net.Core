using System.Collections.Generic;
using System.Linq;

namespace EasyConfig.ConfigurationReaders
{
    public class CommandLineReader : ConfigurationReader
    {
        private readonly Dictionary<string, string> _commandLineArguments;

        public CommandLineReader(string[] args)
        {
            _commandLineArguments = GetArgsDict(args);
        }

        public bool TryGet(string key, out string value)
        {
            if (_commandLineArguments.TryGetValue(key, out value))
                return true;
            
            value = "";
            return false;
        }

        public bool CanBeUsedToReadFrom(ConfigurationSources sources)
        {
            return sources.HasFlag(ConfigurationSources.CommandLine);
        }

        private Dictionary<string, string> GetArgsDict(string[] args)
        {
            return GetArgumentsDictionary(GetKeyValuePairs(args));
        }

        private static Dictionary<string, string> GetArgumentsDictionary(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            var argsDict = new Dictionary<string, string>();

            foreach (var pair in keyValuePairs)
                argsDict.Add(pair.Key, pair.Value);
            
            return argsDict;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs(string[] args)
        {
            return args
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .Select(x => new KeyValuePair<string, string>(x[0], x[1]));
        }
    }
}