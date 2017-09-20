using EasyConfig.Members;

namespace EasyConfig.ValueSetters
{
    public class StringSetter : ValueSetter
    {
        private readonly string _valueToSet;

        public StringSetter(string valueToSet)
        {
            _valueToSet = valueToSet;
        }

        public override void SetTo<T>(Member member, T result)
        {
            member.SetValue(result, _valueToSet);
        }
    }
}