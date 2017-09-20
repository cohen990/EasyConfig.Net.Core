using System;
using EasyConfig.Members;

namespace EasyConfig.ValueSetters
{
    public abstract class ValueSetter
    {
        public abstract void SetTo<T>(Member member, T result);

        public static ValueSetter GetAppropriateSetter(Member member, string value)
        {
            if (member.MemberType == typeof(Uri))
                return new UriSetter(value);

            if (member.MemberType == typeof(int))
                return new IntSetter(value);

            if (member.MemberType == typeof(bool))
                return new BoolSetter(value);

            return new StringSetter(value);
        }
    }
}