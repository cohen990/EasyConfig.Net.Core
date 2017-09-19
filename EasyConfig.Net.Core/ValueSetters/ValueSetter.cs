using EasyConfig.Members;

namespace EasyConfig.ValueSetters
{
    public interface ValueSetter
    {
        void SetTo<T>(Member member, T result);
    }
}