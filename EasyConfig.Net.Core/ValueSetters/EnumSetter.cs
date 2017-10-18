using EasyConfig.Exceptions;
using EasyConfig.Members;
using System;

namespace EasyConfig.ValueSetters
{
    internal class EnumSetter : ValueSetter
    {
        private string value;

        public EnumSetter(string value)
        {
            this.value = value;
        }

        public override void SetTo<T>(Member member, T result)
        {
            object parseResult = ParseEnum(member);
            member.SetValue(result, parseResult);
        }

        private object ParseEnum(Member member)
        {
            try
            {
                return Enum.Parse(member.MemberType, value, true);
            }
            catch (Exception)
            {
                throw new EnumConfigParseException(value, member.MemberType);
            }
        }
    }
}