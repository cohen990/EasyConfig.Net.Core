using System;
using EasyConfig.Exceptions;
using EasyConfig.Members;

namespace EasyConfig.ValueSetters
{
    public class UriSetter : ValueSetter
    {
        private readonly string _uriToSet;

        public UriSetter(string uriToSet)
        {
            _uriToSet = uriToSet;
        }

        public override void SetTo<T>(Member member, T result)
        {
            member.SetValue(result, ExtractUri(member, _uriToSet));
        }

        private static Uri ExtractUri(Member member, string input)
        {
            Uri result;

            if (!Uri.TryCreate(input, UriKind.Absolute, out result))
            {
                throw new ConfigurationTypeException(member.Key, typeof(Uri));
            }
            
            return result;
        }
    }
}