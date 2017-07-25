using System;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;
using NUnit.Framework;
#pragma warning disable 649

namespace EasyConfig.UnitTests
{
    [TestFixture]
    public class ConfigTestsConfigFile
    {
        [Test]
        public void Populate_ConfigJson_GivenValid_Sets()
        {
            Config.UseJson("config.json");
            var config = Config.Populate<JsonConfigTest>();

            Assert.That(config.Test, Is.EqualTo("in-config-json"));
        }

        [Test]
        public void Populate_NestedObject_GivenValid_Sets()
        {
            Config.UseJson("config.json");
            var config = Config.Populate<NestedObject>();

            Assert.That(config.Test, Is.EqualTo("nested-object"));
        }

        private class JsonConfigTest
        {
            [JsonConfig("InConfigJson")]
            public string Test;
        }

        private class NestedObject
        {
            [JsonConfig("NestedObject:InConfigJson")]
            public string Test;
        }
    }
}
