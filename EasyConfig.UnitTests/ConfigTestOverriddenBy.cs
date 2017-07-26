using System;
using EasyConfig.Attributes;
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

        private class IsOverridden
        {
            [JsonConfig("InConfigJson")]
            [OverridenBy(ConfigurationSources.CommandLine)]
            public string Test { get; set; }
        }

        private class AlternativeKey
        {
            [JsonConfig("InConfigJson")]
            [OverridenBy(ConfigurationSources.CommandLine, "alternative_key")]
            public string Test { get; set; }
        }
    }
}
