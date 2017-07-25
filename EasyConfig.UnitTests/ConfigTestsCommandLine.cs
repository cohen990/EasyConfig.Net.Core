using System;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;
using NUnit.Framework;
#pragma warning disable 649

namespace EasyConfig.UnitTests
{
    [TestFixture]
    public class ConfigTestsCommandLine
    {
        [Test]
        public void Populate_AliasDefined_GivenValid_Sets()
        {
            var config = Config.Populate<AliasDefined>("a=aliased");

            Assert.That(config.Test, Is.EqualTo("aliased"));
        }

        [Test]
        public void Populate_PublicGetterSetter_GivenValid_Sets()
        {
            var config = Config.Populate<PublicGetterSetter>("property=value");

            Assert.That(config.Test, Is.EqualTo("value"));
        }

        [Test]
        public void Populate_PublicGetter_GivenValid_ThrowsConfigurationPropertyException()
        {
            Assert.Throws<ConfigurationPropertyException>(() => Config.Populate<PublicGetter>("property=value"));
        }

        [Test]
        public void Populate_PublicGetter_GivenValid_HasUsefulExceptionMessage()
        {
            try
            {
                Config.Populate<PublicGetter>("property=value");
            }
            catch (ConfigurationPropertyException result)
            {
                Assert.That(result.Message, Contains.Substring("'Test'"));
            }
        }

        private class AliasDefined
        {
            [CommandLine("unaliased", "a")]
            public string Test;
        }

        private class PublicGetterSetter
        {
            [CommandLine("property")]
            public string Test { get; set; }
        }

        private class PublicGetter
        {
            [CommandLine("property")]
            public string Test { get; }
        }
    }
}
