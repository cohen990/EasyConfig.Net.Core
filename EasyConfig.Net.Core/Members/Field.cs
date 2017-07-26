using System.Reflection;
using EasyConfig.Attributes;

namespace EasyConfig.Members
{
    public class Field : Member
    {
        private readonly FieldInfo _fieldInfo;

        public Field(
            object defaultValue,
            bool isRequired,
            bool shouldHideInLog,
            ConfigurationAttribute configurationAttribute,
            ConfigurationSources? overrideSource,
            string overrideKey,
            FieldInfo fieldInfo)
            : base(
                  defaultValue,
                  isRequired,
                  shouldHideInLog,
                  configurationAttribute,
                  overrideSource,
                  overrideKey,
                  fieldInfo.FieldType)
        {
            _fieldInfo = fieldInfo;
        }

        public override void SetValue(object instance, object value)
        {
            _fieldInfo.SetValue(instance, value);
        }
    }
}
