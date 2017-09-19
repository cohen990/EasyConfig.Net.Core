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

        public bool TryGet(string key, string alias, out string value)
        {
            if (_commandLineArguments.TryGetValue(key, out value))
            {
                return true;
            }

            if (_commandLineArguments.TryGetValue(alias, out value))
            {
                return true;
            }
            
            value = "";
            return false;
        }

        public bool CanBeUsedToReadFrom(ConfigurationSources sources)
        {
            return sources.HasFlag(ConfigurationSources.CommandLine);
        }

        private Dictionary<string, string> GetArgsDict(string[] args)
        {
            var keyValuePairs = GetKeyValuePairs(args);

            return GetArgumentsDictionary(keyValuePairs);
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
            var keyValuePairs = args
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .Select(x => new KeyValuePair<string, string>(x[0], x[1]));
            return keyValuePairs;
        }
    }
}