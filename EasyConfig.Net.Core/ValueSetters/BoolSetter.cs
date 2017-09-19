using EasyConfig.Exceptions;
using EasyConfig.Members;

namespace EasyConfig.ValueSetters
{
    public class BoolSetter : ValueSetter
    {
        private readonly string _valueToSet;

        public BoolSetter(string valueToSet)
        {
            _valueToSet = valueToSet;
        }
        
        public void SetTo<T>(Member member, T result)
        {
            member.SetValue(result, ExtractBool(member, _valueToSet));
        }

        private static bool ExtractBool(Member member, string input)
        {
            bool result;
            if (!bool.TryParse(input, out result))
            {
                throw new ConfigurationTypeException(member.Key, typeof(bool));
            }
            return result;
        }
    }
}