using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyConfig.Attributes;
using EasyConfig.Configuration;
using EasyConfig.Exceptions;
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
            var config = Builder.Build();

            var parameters = new T();

            if (args == null) throw new ArgumentNullException(nameof(args));

            var argsDict = GetArgsDict(args);

            var allMemberConfigurations = new List<MemberConfiguration>();

            foreach (var fieldInfo in typeof(T).GetTypeInfo().DeclaredFields)
            {
                allMemberConfigurations.Add(GetMemberConfigurationFromField(fieldInfo));
            }

            foreach (var propertyInfo in typeof(T).GetTypeInfo().DeclaredProperties)
            {
                allMemberConfigurations.Add(GetMemberConfigurationFromProperty(propertyInfo));
            }

            foreach(var memberConfig in allMemberConfigurations.Where(x => x != null)) {
                string value;

                var got = TryGet(argsDict,
                    memberConfig.Key,
                    memberConfig.Alias,
                    memberConfig.ConfigurationSources,
                    config,
                    out value);

                if (!got)
                {
                    if (memberConfig.HasDefault)
                    {
                        if (memberConfig.IsRequired)
                        {
                            throw new ConfigurationMissingException(memberConfig.Key, memberConfig.MemberType, memberConfig.ConfigurationSources);
                        }
                        continue;
                    }

                    value = memberConfig.DefaultValue.ToString();
                }

                SetValue(memberConfig, value, ref parameters);
            }

            return parameters;
        }

        private static MemberConfiguration GetMemberConfigurationFromProperty(PropertyInfo propertyInfo)
        {
            var defaultAttribute = propertyInfo.GetCustomAttribute<DefaultAttribute>();
            var hasDefaultAttribute = defaultAttribute == null;
            var defaultValue = defaultAttribute?.Default;
            var required = propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;
            var configurationAttribute = propertyInfo.GetCustomAttribute<ConfigurationAttribute>();
            var shouldHideInLog = propertyInfo.GetCustomAttribute<SensitiveInformationAttribute>() != null;

            return new PropertyConfiguration(
                defaultValue,
                hasDefaultAttribute,
                required,
                shouldHideInLog,
                configurationAttribute,
                propertyInfo);
        }

        private static MemberConfiguration GetMemberConfigurationFromField(FieldInfo fieldInfo)
        {
            var configurationAttribute = fieldInfo.GetCustomAttribute<ConfigurationAttribute>();
            if (configurationAttribute == null)
            {
                return null;
            }

            var defaultAttribute = fieldInfo.GetCustomAttribute<DefaultAttribute>();
            var hasDefaultAttribute = defaultAttribute == null;
            var defaultValue = defaultAttribute?.Default;
            var required = fieldInfo.GetCustomAttribute<RequiredAttribute>() != null;
            var shouldHideInLog = fieldInfo.GetCustomAttribute<SensitiveInformationAttribute>() != null;

            return new FieldConfiguration(
                defaultValue,
                hasDefaultAttribute,
                required,
                shouldHideInLog,
                configurationAttribute,
                fieldInfo);
        }

        private static bool TryGet(
            Dictionary<string, string> argsDict, 
            string key, 
            string alias, 
            ConfigurationSources sources, 
            IConfigurationRoot jsonConfiguration, 
            out string value)
        {
            if (TryGetFromCommandLine(argsDict, key, alias, sources, out value))
            {
                return true;
            }

            if (TryGetFromEnvironment(key, sources, out value))
            {
                return true;
            }

            if (TryGetFromConfigFile(jsonConfiguration, key, sources, out value))
            {
                return true;
            }

            value = "";
            return false;
        }

        private static bool TryGetFromConfigFile(
            IConfigurationRoot config,
            string key,
            ConfigurationSources sources, 
            out string value)
        {
            if (sources.HasFlag(ConfigurationSources.ConfigFile))
            {
                try
                {
                    value = config[key];
                    return true;
                }
                catch
                {
                }
            }

            value = "";
            return false;
        }

        private static bool TryGetFromCommandLine(
            Dictionary<string, string> argsDict,
            string key,
            string alias, 
            ConfigurationSources sources,
            out string val)
        {
            if (sources.HasFlag(ConfigurationSources.CommandLine))
            {
                if (argsDict.TryGetValue(key, out val))
                {
                    return true;
                }

                if (argsDict.TryGetValue(alias, out val))
                {
                    return true;
                }
            }
            val = "";
            return false;
        }

        private static bool TryGetFromEnvironment(
            string key, 
            ConfigurationSources sources,
            out string val)
        {
            if (sources.HasFlag(ConfigurationSources.Environment))
            {
                val = Environment.GetEnvironmentVariable(key);
                return !string.IsNullOrWhiteSpace(val);
            }

            val = "";
            return false;
        }

        private static Dictionary<string, string> GetArgsDict(string[] args)
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

        private static void SetValue<T>(MemberConfiguration member, string value, ref T result) where T : new()
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
