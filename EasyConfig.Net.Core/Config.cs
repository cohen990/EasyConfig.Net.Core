using System;
using System.Collections.Generic;
using System.Linq;
using EasyConfig.ConfigurationReaders;
using EasyConfig.Exceptions;
using EasyConfig.Members;
using EasyConfig.ValueSetters;
using Microsoft.Extensions.Configuration;

namespace EasyConfig
{
    public class Config
    {
        private readonly Writer _writer;
        private readonly ConfigurationBuilder _builder;

        public Config(Writer writer)
        {
            _writer = writer;
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
            var setter = ValueSetter.GetAppropriateSetter(member, value);
                
            setter.SetTo(member, result);

            if (member.ShouldHideInLog)
            {
                _writer.ObfuscateConfigurationValue(member.Key, value);
                return;
            }
            
            _writer.WriteConfigurationValue(member.Key, value);
        }

        public void UseJson(string path)
        {
            _builder.AddJsonFile(path);
        }
    }
}
