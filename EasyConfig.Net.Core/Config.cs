using System;
using System.Linq;
using EasyConfig.ConfigurationReaders;
using EasyConfig.Exceptions;
using EasyConfig.Members;
using Microsoft.Extensions.Configuration;

namespace EasyConfig
{
    public class Config
    {
        private static readonly ConfigurationBuilder Builder;

        static Config()
        {
            Builder = new ConfigurationBuilder();
        }

        public static T Populate<T>(params string[] args) where T : new()
        {
            var commandLineReader = new CommandLineReader(args);
            var environmentVariablesReader = new EnvironmentVariablesReader();
            var jsonFileReader = new JsonFileReader(Builder.Build());
            var memberMaker = new MemberMaker();
            var parameters = new T();

            var allMembers = memberMaker.GetMembers<T>(memberMaker);

            foreach (var member in allMembers.Where(x => x != null))
            {
                GetValueForMember(member, commandLineReader, environmentVariablesReader, jsonFileReader, ref parameters);
            }

            return parameters;
        }

        private static void GetValueForMember<T>(
            Member member,
            CommandLineReader commandLineReader,
            EnvironmentVariablesReader environmentVariablesReader,
            JsonFileReader jsonFileReader,
            ref T parameters) where T : new()
        {
            string value;
            if (member.IsOverridable)
            {
                if (TryGet(member.OverrideKey ?? member.Key,
                    "",
                    member.OverrideSource,
                    commandLineReader,
                    environmentVariablesReader,
                    jsonFileReader,
                    out value))
                {
                    SetValue(member, value, ref parameters);
                    return;
                }
            }

            if (TryGet(
                member.Key,
                member.Alias,
                member.ConfigurationSources,
                commandLineReader,
                environmentVariablesReader,
                jsonFileReader,
                out value))
            {
                SetValue(member, value, ref parameters);
                return;
            }

            if (member.HasDefault)
            {
                SetValue(member, member.DefaultValue.ToString(), ref parameters);
                return;
            }

            if (member.IsRequired)
            {
                var sources = member.ConfigurationSources;
                if (member.IsOverridable)
                {
                    sources |= member.OverrideSource;
                }
                throw new ConfigurationMissingException(member.Key, member.MemberType, sources);
            }
        }

        private static bool TryGet(
            string key,
            string alias,
            ConfigurationSources sources,
            CommandLineReader commandLineReader,
            EnvironmentVariablesReader environmentVariablesReader,
            JsonFileReader jsonFileReader,
            out string value)
        {
            if (commandLineReader.TryGet(key, alias, sources, out value))
            {
                return true;
            }

            if (environmentVariablesReader.TryGet(key, alias, sources, out value))
            {
                return true;
            }

            if (jsonFileReader.TryGet(key, alias, sources, out value))
            {
                return true;
            }

            value = "";
            return false;
        }

        private static void SetValue<T>(Member member, string value, ref T result) where T : new()
        {
            if (member.MemberType == typeof(Uri))
            {
                Uri uri;

                if (!Uri.TryCreate(value, UriKind.Absolute, out uri))
                {
                    throw new ConfigurationTypeException(member.Key, typeof(Uri));
                }

                LogConfigurationValue(member.Key, value, member.ShouldHideInLog);

                member.SetValue(result, new Uri(value));
            }
            else if (member.MemberType == typeof(int))
            {
                int i;
                if (!int.TryParse(value, out i))
                {
                    throw new ConfigurationTypeException(member.Key, typeof(int));
                }

                LogConfigurationValue(member.Key, value, member.ShouldHideInLog);

                member.SetValue(result, i);
            }
            else
            {
                LogConfigurationValue(member.Key, value, member.ShouldHideInLog);

                member.SetValue(result, value);
            }
        }

        private static void LogConfigurationValue(string key, string value, bool shouldHideInLog)
        {
            if (shouldHideInLog)
            {
                value = new string('*', value.Length);
            }

            Log($"Using '{value}' for '{key}'");
        }

        private static void Log(string content)
        {
            Console.WriteLine(content);
        }

        public static void UseJson(string path)
        {
            Builder.AddJsonFile(path);
        }
    }
}
