using EasyConfig.Exceptions;
using EasyConfig.Members;
using System;

namespace EasyConfig.ValueSetters
{
    public class NullableEnumSetter : EnumSetter
    {
        public NullableEnumSetter(string value) : base(value)
        {
        }

        protected override object ParseEnum(Type nullableEnumType)
        {
            return base.ParseEnum(Nullable.GetUnderlyingType(nullableEnumType));
        }
    }

    public class EnumSetter : ValueSetter
    {
        private string value;

        public EnumSetter(string value)
        {
            this.value = value;
        }

        public override void SetTo<T>(Member member, T result)
        {
            object parseResult = ParseEnum(member.MemberType);
            member.SetValue(result, parseResult);
        }

        protected virtual object ParseEnum(Type enumType)
        {
            try
            {
                return Enum.Parse(enumType, value, true);
            }
            catch (Exception)
            {
                throw new EnumConfigParseException(value, enumType);
            }
        }
    }
}