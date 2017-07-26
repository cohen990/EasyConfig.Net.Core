using System.Collections.Generic;
using System.Reflection;
using EasyConfig.Attributes;

namespace EasyConfig.Members
{
    public class MemberMaker
    {
        public Member Make(PropertyInfo propertyInfo)
        {
            var defaultAttribute = propertyInfo.GetCustomAttribute<DefaultAttribute>();
            var defaultValue = defaultAttribute?.Default;
            var required = propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;
            var configurationAttribute = propertyInfo.GetCustomAttribute<ConfigurationAttribute>();
            var shouldHideInLog = propertyInfo.GetCustomAttribute<SensitiveInformationAttribute>() != null;
            var overrideSource = propertyInfo.GetCustomAttribute<OverriddenByAttribute>()?.Source;
            var overrideKey = propertyInfo.GetCustomAttribute<OverriddenByAttribute>()?.AlternativeKey;

            return new Property(
                defaultValue,
                required,
                shouldHideInLog,
                configurationAttribute,
                overrideSource,
                overrideKey,
                propertyInfo);
        }

        public Member Make(FieldInfo fieldInfo)
        {
            var configurationAttribute = fieldInfo.GetCustomAttribute<ConfigurationAttribute>();
            if (configurationAttribute == null)
            {
                return null;
            }

            var defaultAttribute = fieldInfo.GetCustomAttribute<DefaultAttribute>();
            var defaultValue = defaultAttribute?.Default;
            var required = fieldInfo.GetCustomAttribute<RequiredAttribute>() != null;
            var shouldHideInLog = fieldInfo.GetCustomAttribute<SensitiveInformationAttribute>() != null;
            var overrideSource = fieldInfo.GetCustomAttribute<OverriddenByAttribute>()?.Source;
            var overrideKey = fieldInfo.GetCustomAttribute<OverriddenByAttribute>()?.AlternativeKey;

            return new Field(
                defaultValue,
                required,
                shouldHideInLog,
                configurationAttribute,
                overrideSource,
                overrideKey,
                fieldInfo);
        }

        public List<Member> GetMembers<T>(MemberMaker memberMaker) where T : new()
        {
            var allMembers = new List<Member>();
            foreach (var fieldInfo in typeof(T).GetTypeInfo().DeclaredFields)
            {
                allMembers.Add(memberMaker.Make(fieldInfo));
            }

            foreach (var propertyInfo in typeof(T).GetTypeInfo().DeclaredProperties)
            {
                allMembers.Add(memberMaker.Make(propertyInfo));
            }

            return allMembers;
        }
    }
}
