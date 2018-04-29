using Microsoft.Extensions.Configuration;

namespace EasyConfig
{
    public class Config
    {
        private readonly Writer _writer;
        private readonly ConfigurationBuilder _builder;
        private static Config _config;

        public Config(Writer writer = null)
        {
            _writer = writer ?? new ConsoleWriter();
            _builder = new ConfigurationBuilder();
        }

        public Config WithJson(string path)
        {
            _builder.AddJsonFile(path);
            return this;
        }

        public T PopulateClass<T>(params string[] args) where T : new()
        {
            return new ClassPopulator<T>(_writer, _builder.Build()).PopulateClass(args);
        }

        public static T Populate<T>(params string[] args) where T : new()
        {
            return _config.PopulateClass<T>(args);
        }

        public static void UseJson(string path)
        {
            var config = new Config(new ConsoleWriter());

            config.WithJson(path);

            _config = config;
        }
    }
}
