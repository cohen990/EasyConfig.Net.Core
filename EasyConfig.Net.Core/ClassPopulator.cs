using System.Collections.Generic;
using System.Linq;
using EasyConfig.ConfigurationReaders;
using EasyConfig.Exceptions;
using EasyConfig.Members;
using EasyConfig.ValueSetters;
using Microsoft.Extensions.Configuration;

namespace EasyConfig
{
    public class ClassPopulator<T> where T : new()
    {
        private readonly Writer _writer;
        private readonly IConfigurationRoot _configuration;
        private T _toPopulate;

        public ClassPopulator()
        {
        }

        public ClassPopulator(Writer writer, IConfigurationRoot configuration)
        {
            _writer = writer;
            _configuration = configuration;
            _toPopulate = new T();
        }

        public T PopulateClass(params string[] args)
        {
            var readers = new ConfigurationReader[]
            {
                new EnvironmentVariablesReader(new SystemEnvironmentWrapper()),
                new JsonConfigReader(_configuration),
                new CommandLineReader(args),
            };
            
            var allMembers = GetAllMembers();

            foreach (var member in allMembers.Where(x => x != null))
            {
                GetValueForMember(member, readers, ref _toPopulate);
            }

            return _toPopulate;
        }

        protected virtual List<Member> GetAllMembers() 
        {
            var memberMaker = new MemberMaker();
            var allMembers = memberMaker.GetMembers<T>();
            return allMembers;
        }

        protected virtual void GetValueForMember(
            Member member,
            ConfigurationReader[] readers,
            ref T toPopulate)
        {
            string value;
            if (member.IsOverridable)
            {
                if (TryGet(
                    member.OverrideKey ?? member.Key,
                    member.OverrideSource,
                    readers,
                    out value))
                {
                    SetValue(member, value, toPopulate);
                    return;
                }
            }
            if (member.HasAlias)
            {
                if (TryGet(
                    member.Alias,
                    member.ConfigurationSources,
                    readers,
                    out value))
                {
                    SetValue(member, value, toPopulate);
                    return;
                }
            }
            if (TryGet(
                member.Key,
                member.ConfigurationSources,
                readers,
                out value))
            {
                SetValue(member, value, toPopulate);
                return;
            }

            if (member.HasDefault)
            {
                SetValue(member, member.DefaultValue.ToString(), toPopulate);
                return;
            }

            if (member.IsRequired)
            {
                Fail(member);
            }
        }

        private bool TryGet(
            string key,
            ConfigurationSources sources,
            ConfigurationReader[] readers,
            out string value)
        {
            foreach (var reader in readers)
            {
                if (!reader.CanBeUsedToReadFrom(sources))
                    continue;
                
                value = reader.Get(key);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return true;
                }
            }

            value = "";
            return false;
        }

        private void SetValue(Member member, string value, T toPopulate)
        {
            var setter = ValueSetter.GetAppropriateSetter(member, value);
                
            setter.SetTo(member, toPopulate);

            WriteValue(member, value);
        }

        private void WriteValue(Member member, string value)
        {
            if (member.ShouldHideInLog)
            {
                _writer.ObfuscateConfigurationValue(member.Key, value);
                return;
            }

            _writer.WriteConfigurationValue(member.Key, value);
        }

        private static void Fail(Member member)
        {
            if (member.IsOverridable)
            {
                throw new OverridableConfigurationMissingException(
                    member.Key,
                    member.ConfigurationSources,
                    member.OverrideKey,
                    member.OverrideSource,
                    member.MemberType);
            }
            
            throw new ConfigurationMissingException(
                member.Key,
                member.MemberType,
                member.ConfigurationSources);
        }
    }
}