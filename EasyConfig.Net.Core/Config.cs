using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyConfig.Attributes;
using EasyConfig.Configuration;
using EasyConfig.Exceptions;

namespace EasyConfig
{
    public class Config
    {
        public static T Populate<T>(params string[] args) where T : new()
        {
            var parameters = new T();

            if (args == null) throw new ArgumentNullException(nameof(args));

            var argsDict = GetArgsDict(args);

            var allProps = new List<MemberConfiguration>();

            foreach (var fieldInfo in typeof(T).GetTypeInfo().DeclaredFields)
            {
                allProps.Add(GetMemberConfigurationPropertiesFromField(fieldInfo));
            }

            foreach (var propertyInfo in typeof(T).GetTypeInfo().DeclaredProperties)
            {
                allProps.Add(GetMemberConfigurationPropertiesFromProperty(propertyInfo));
            }

            foreach(var prop in allProps.Where(x => x != null)) {
                string value;

                var got = TryGet(argsDict,
                    prop.Key,
                    prop.Alias,
                    prop.ConfigurationSources,
                    out value);

                if (!got)
                {
                    if (prop.HasDefault)
                    {
                        if (prop.IsRequired)
                        {
                            throw new ConfigurationMissingException(prop.Key, prop.MemberType, prop.ConfigurationSources);
                        }
                        continue;
                    }

                    value = prop.DefaultValue.ToString();
                }

                SetValue(prop, value, ref parameters);
            }

            return parameters;
        }

        private static MemberConfiguration GetMemberConfigurationPropertiesFromProperty(PropertyInfo propertyInfo)
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

        private static MemberConfiguration GetMemberConfigurationPropertiesFromField(FieldInfo fieldInfo)
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

        private static bool TryGet(Dictionary<string, string> argsDict, string key, string alias, ConfigurationSources sources, out string value)
        {
            bool got = false;
            string val = string.Empty;

            if (sources.HasFlag(ConfigurationSources.CommandLine))
            {
                got = argsDict.TryGetValue(key, out val);
                if (!got)
                {
                    got = argsDict.TryGetValue(alias, out val);
                }
            }

            if (!got && sources.HasFlag(ConfigurationSources.Environment))
            {
                val = Environment.GetEnvironmentVariable(key);
                got = !string.IsNullOrWhiteSpace(val);
                if (!got)
                {
                    got = argsDict.TryGetValue(alias, out val);
                }
            }

            value = val;
            return got;
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
    }
}
