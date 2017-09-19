using Microsoft.Extensions.Configuration;

namespace EasyConfig
{
    public class Config
    {
        private readonly Writer _writer;
        private readonly ConfigurationBuilder _builder;

        public Config(Writer writer)
        {
            _writer = writer;
            _builder = new ConfigurationBuilder();
        }

        public void UseJson(string path)
        {
            _builder.AddJsonFile(path);
        }

        public T PopulateClass<T>(params string[] args) where T : new()
        {
            return new ClassPopulator<T>(_writer, _builder).PopulateClass(args);
        }
    }
}
