using System;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;
using NUnit.Framework;
#pragma warning disable 649

namespace EasyConfig.UnitTests
{
    [TestFixture]
    public class ConfigTestOverriddenBy
    {
        [Test]
        public void Populate_IsOverriden_NotGivenOverride_DoesntOverride()
        {
            Config.UseJson("config.json");
            var config = Config.Populate<IsOverridden>();

            Assert.That(config.Test, Is.EqualTo("in-config-json"));
        }

        [Test]
        public void Populate_IsOverriden_GivenOverride_Overrides()
        {
            Config.UseJson("config.json");
            var config = Config.Populate<IsOverridden>("InConfigJson=overriden");

            Assert.That(config.Test, Is.EqualTo("overriden"));
        }

        [Test]
        public void Populate_IsOverriden_GivenOverrideWithAlternativeKey_Overrides()
        {
            Config.UseJson("config.json");
            var config = Config.Populate<AlternativeKey>("alternative_key=overriden");

            Assert.That(config.Test, Is.EqualTo("overriden"));
        }

        [Test]
        public void Populate_Required_Missing_Throws()
        {
            try
            {
                var config = Config.Populate<Required>();
                throw new AssertionException("Config.Populate did not throw a ConfigurationMissingException");
            }
            catch (ConfigurationMissingException result)
            {
                Assert.That(result.Message.Contains("CommandLine"), $"'{result.Message}' did not contain 'CommandLine'");
                Assert.That(result.Message.Contains("JsonConfig"), $"'{result.Message}' did not contain 'JsonConfig'");
            }

        }

        private class IsOverridden
        {
            [JsonConfig("InConfigJson")]
            [OverriddenBy(ConfigurationSources.CommandLine)]
            public string Test { get; set; }
        }

        private class AlternativeKey
        {
            [JsonConfig("InConfigJson")]
            [OverriddenBy(ConfigurationSources.CommandLine, "alternative_key")]
            public string Test { get; set; }
        }

        private class Required
        {
            [JsonConfig("MissingConfiguration"), Required]
            [OverriddenBy(ConfigurationSources.CommandLine)]
            public string Test { get; set; }
        }
    }
}
