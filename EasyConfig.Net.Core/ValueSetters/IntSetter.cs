using EasyConfig.Exceptions;
using EasyConfig.Members;

namespace EasyConfig.ValueSetters
{
    public class IntSetter : ValueSetter
    {
        private readonly string _valueToSet;

        public IntSetter(string valueToSet)
        {
            _valueToSet = valueToSet;
        }

        public override void SetTo<T>(Member member, T result)
        {
            member.SetValue(result, ExtractInt(member, _valueToSet));
        }

        private static int ExtractInt(Member member, string input)
        {
            int result;
            if (!int.TryParse(input, out result))
            {
                throw new ConfigurationTypeException(member.Key, typeof(int));
            }
            return result;
        }
    }
}