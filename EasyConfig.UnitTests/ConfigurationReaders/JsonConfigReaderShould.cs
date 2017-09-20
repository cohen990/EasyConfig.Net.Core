using System;
using System.Collections.Generic;
using EasyConfig.ConfigurationReaders;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using static EasyConfig.ConfigurationSources;

namespace EasyConfig.UnitTests.ConfigurationReaders
{
    [TestFixture]
    public class JsonConfigReaderShould
    {
        private IConfigurationRoot _configurationRoot;
        private JsonConfigReader _jsonConfigReader;

        [SetUp]
        public void SetUp()
        {
            _configurationRoot = Substitute.For<IConfigurationRoot>();
            _jsonConfigReader = new JsonConfigReader(_configurationRoot);
        }
        
        [TestCase(JsonConfig, true)]
        [TestCase(CommandLine, false)]
        [TestCase(EnvironmentVariables, false)]
        [TestCase(CommandLine | EnvironmentVariables, false)]
        [TestCase(JsonConfig | CommandLine, true)]
        [TestCase(JsonConfig | EnvironmentVariables, true)]
        [TestCase(JsonConfig | EnvironmentVariables | JsonConfig, true)]
        public void Only_Claim_To_Be_Useful_If_In_Sources(
            ConfigurationSources sources,
            bool expected)
        {
            var result = _jsonConfigReader.CanBeUsedToReadFrom(sources);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Return_Value_Provided_In_Config()
        {
            string providedValue = GetAUniqueString();
            string providedKey = GetAUniqueString();

            _configurationRoot[providedKey]
                .Returns(providedValue);
            
            var result = _jsonConfigReader.Get(providedKey);
            
            Assert.That(result, Is.EqualTo(providedValue));
        }

        [Test]
        public void Return_Empty_If_Value_Is_Not_Found()
        {
            string providedValue = GetAUniqueString();
            string providedKey = GetAUniqueString();

            _configurationRoot
                .When(x => { CallsIndexer(x, providedKey); })
                .Do(x => throw new KeyNotFoundException());
            
            var result = _jsonConfigReader.Get(providedKey);
            
            Assert.That(result, Is.Empty);
        }

        private static void CallsIndexer(IConfigurationRoot x, string providedKey)
        {
            var temp = x[providedKey];
        }

        private string GetAUniqueString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}