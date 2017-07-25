using System;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;

namespace EasyConfig.Net.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.UseJson("config.json");
            try
            {
                var config = Config.Populate<SampleConfig>(args);
            }
            catch (EasyConfigException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    internal class SampleConfig
    {
        // Can be supplied by environment or command line. If not supplied, throws a ConfigurationMissingException
        [EnvironmentOrCommandLine("uri_required"), Required]
        public Uri Endpoint1;

        [EnvironmentOrCommandLine("uri_not_required")]
        public Uri Endpoint2;

        [CommandLine("string_required_commandline"), Required]
        public string Username;

        [CommandLine("string_sensitive_required"), Required, SensitiveInformation]
        public string Password;

        [CommandLine("int_default"), Default(1000)]
        public int Defaultable;

        [JsonConfig("InConfigJson")]
        public string InJsonConfig;

        [JsonConfig("NestedObject.InConfigJson")]
        public string NestedJsonConfig { get; set; }

        [CommandLine("bool_example")]
        public bool BoolExample { get; set; }

        [CommandLine("aliased", "a")]
        public bool Aliased { get; set; }
    }
}