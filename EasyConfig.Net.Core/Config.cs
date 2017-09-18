using System;
using System.Collections.Generic;
using System.Linq;
using EasyConfig.ConfigurationReaders;
using EasyConfig.Exceptions;
using EasyConfig.Members;
using Microsoft.Extensions.Configuration;

namespace EasyConfig
{
    public class Config
    {
        private readonly ConfigurationBuilder _builder;

        public Config()
        {
            _builder = new ConfigurationBuilder();
        }

        public T PopulateClass<T>(params string[] args) where T : new()
        {
            var commandLineReader = new CommandLineReader(args);
            var environmentVariablesReader = new EnvironmentVariablesReader();
            var jsonFileReader = new JsonFileReader(_builder.Build());
            var parameters = new T();

            var allMembers = GetAllMembers<T>();

            foreach (var member in allMembers.Where(x => x != null))
            {
                GetValueForMember(member, commandLineReader, environmentVariablesReader, jsonFileReader, ref parameters);
            }

            return parameters;
        }

        protected virtual List<Member> GetAllMembers<T>() where T : new()
        {
            var memberMaker = new MemberMaker();
            var allMembers = memberMaker.GetMembers<T>();
            return allMembers;
        }

        protected virtual void GetValueForMember<T>(
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
                    throw new OverridableConfigurationMissingException(
                        member.Key,
                        member.ConfigurationSources,
                        member.OverrideKey,
                        member.OverrideSource,
                        member.MemberType);
                }
                throw new ConfigurationMissingException(member.Key, member.MemberType, sources);
            }
        }

        private bool TryGet(
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

        private void SetValue<T>(Member member, string value, ref T result) where T : new()
        {
            if (member.MemberType == typeof(Uri))
            {
                Uri uri;

                if (!Uri.TryCreate(value, UriKind.Absolute, out uri))
                {
                    throw new ConfigurationTypeException(member.Key, typeof(Uri));
                }

                WriteConfigurationValue(member.Key, value, member.ShouldHideInLog);

                member.SetValue(result, new Uri(value));
            }
            else if (member.MemberType == typeof(int))
            {
                int i;
                if (!int.TryParse(value, out i))
                {
                    throw new ConfigurationTypeException(member.Key, typeof(int));
                }

                WriteConfigurationValue(member.Key, value, member.ShouldHideInLog);

                member.SetValue(result, i);
            }
            else if (member.MemberType == typeof(bool))
            {
                bool b;
                if (!bool.TryParse(value, out b))
                {
                    throw new ConfigurationTypeException(member.Key, typeof(bool));
                }

                WriteConfigurationValue(member.Key, value, member.ShouldHideInLog);

                member.SetValue(result, b);
            }
            else
            {
                WriteConfigurationValue(member.Key, value, member.ShouldHideInLog);

                member.SetValue(result, value);
            }
        }

        protected void WriteConfigurationValue(string key, string value, bool shouldHideInLog)
        {
            if (shouldHideInLog)
            {
                value = new string('*', value.Length);
            }

            WriteLine($"Using '{value}' for '{key}'");
        }

        protected void WriteLine(string content)
        {
            Console.WriteLine(content);
        }

        public void UseJson(string path)
        {
            _builder.AddJsonFile(path);
        }
    }
}
