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

        public Dictionary<string, string> GetArgsDict(string[] args)
        {
            var split = args.Select(x => x.Split('='));
            var argsDict = new Dictionary<string, string>();

            foreach (var pair in split)
            {
                if (pair.Length != 2)
                {
                    continue;
                }

                argsDict[pair[0]] = pair[1];
            }

            return argsDict;
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
    }
}