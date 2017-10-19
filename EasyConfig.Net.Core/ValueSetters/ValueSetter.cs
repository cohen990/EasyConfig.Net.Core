using System;
using EasyConfig.Members;

namespace EasyConfig.ValueSetters
{
    public abstract class ValueSetter
    {
        public abstract void SetTo<T>(Member member, T result);

        public static ValueSetter GetAppropriateSetter(Member member, string value)
        {
            if (isNullableEnum(member))
                return new NullableEnumSetter(value);

            if (member.MemberType.IsEnum)
                return new EnumSetter(value);

            if (member.MemberType == typeof(Uri))
                return new UriSetter(value);

            if (member.MemberType == typeof(int))
                return new IntSetter(value);

            if (member.MemberType == typeof(bool))
                return new BoolSetter(value);

            return new StringSetter(value);
        }

        private static bool isNullableEnum(Member member)
        {
            var underlyingType = Nullable.GetUnderlyingType(member.MemberType);
            return underlyingType != null && underlyingType.IsEnum;
        }
    }
}