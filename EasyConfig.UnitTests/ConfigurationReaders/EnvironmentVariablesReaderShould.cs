using System;
using EasyConfig.ConfigurationReaders;
using NSubstitute;
using NUnit.Framework;
using static EasyConfig.ConfigurationSources;

namespace EasyConfig.UnitTests.ConfigurationReaders
{
    [TestFixture]
    public class EnvironmentVariablesReaderShould
    {
        private EnvironmentWrapper _environment;
        private EnvironmentVariablesReader _environmentVariablesReader;

        [SetUp]
        public void SetUp()
        {
            _environment = Substitute.For<EnvironmentWrapper>();
            _environmentVariablesReader = new EnvironmentVariablesReader(_environment);
        }
        
        [TestCase(EnvironmentVariables, true)]
        [TestCase(CommandLine, false)]
        [TestCase(JsonConfig, false)]
        [TestCase(CommandLine | JsonConfig, false)]
        [TestCase(EnvironmentVariables | CommandLine, true)]
        [TestCase(EnvironmentVariables | JsonConfig, true)]
        [TestCase(EnvironmentVariables | CommandLine | JsonConfig, true)]
        public void Only_Claim_To_Be_Useful_If_In_Sources(
            ConfigurationSources sources,
            bool expected)
        {
            var result = _environmentVariablesReader.CanBeUsedToReadFrom(sources);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Return_The_Environment_Variable()
        {
            var givenKey = GetAUniqueString();
            var environmentVariable = GetAUniqueString();
            _environment.GetEnvironmentVariable(givenKey).Returns(environmentVariable);
            
            string result = "";
            _environmentVariablesReader.TryGet(givenKey, out result);
            
            Assert.That(result, Is.EqualTo(environmentVariable));
        }

        [Test]
        public void Return_Empty_When_Value_Is_Not_Found()
        {
            var givenKey = GetAUniqueString();
            var environmentVariable = GetAUniqueString();
            
            string result = "";
            _environmentVariablesReader.TryGet(givenKey, out result);
            
            Assert.That(result, Is.Empty);
        }

        private static string GetAUniqueString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}