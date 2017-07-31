using System;
using EasyConfig.Exceptions;
using NUnit.Framework;

namespace EasyConfig.UnitTests.Exceptions
{
    [TestFixture]
    class OverridableConfigurationMissingExceptionTests
    {
        [Test]
        public void ctor_GivenNull_DoesntThrow()
        {
            Assert.DoesNotThrow(() => new OverridableConfigurationMissingException(null, 0, null, 0, null));
        }

        [Test]
        public void ctor_GivenConfigKey_IncludesInErrorMessage()
        {
            var key = Guid.NewGuid().ToString();
            var result = new OverridableConfigurationMissingException(key, 0, null, 0, null).Message;

            Assert.That(result, Does.Contain(key));
        }

        [Test]
        public void ctor_GivenType_IncludesInErrorMessage()
        {
            var type = GetType();
            var result = new OverridableConfigurationMissingException(null, 0, null, 0, type).Message;

            Assert.That(result, Does.Contain(type.ToString()));
        }

        [Test]
        public void ctor_GivenSource_IncludesInErrorMessage()
        {
            var source = ConfigurationSources.JsonConfig;
            var result = new OverridableConfigurationMissingException(null, source, null, 0, null).Message;

            Assert.That(result, Does.Contain(source.ToString()));
        }

        [Test]
        public void ctor_GivenMultipleSources_IncludesAllInErrorMessage()
        {
            var sources = ConfigurationSources.CommandLine | ConfigurationSources.Environment;

            var result = new OverridableConfigurationMissingException(null, sources, null, 0, null).Message;

            Assert.That(result, Does.Contain(ConfigurationSources.CommandLine.ToString()));
            Assert.That(result, Does.Contain(ConfigurationSources.Environment.ToString()));
        }

        [Test]
        public void ctor_GivenOverrideKey_IncludesInErrorMessage()
        {
            var key = Guid.NewGuid().ToString();
            var result = new OverridableConfigurationMissingException(null, 0, key, 0, null).Message;

            Assert.That(result, Does.Contain(key));
        }

        [Test]
        public void ctor_GivenOverrideSources_IncludesAllInErrorMessage()
        {
            var sources = ConfigurationSources.CommandLine | ConfigurationSources.Environment;

            var result = new OverridableConfigurationMissingException(null, 0, null, sources, null).Message;

            Assert.That(result, Does.Contain(ConfigurationSources.CommandLine.ToString()));
            Assert.That(result, Does.Contain(ConfigurationSources.Environment.ToString()));
        }
    }
}
