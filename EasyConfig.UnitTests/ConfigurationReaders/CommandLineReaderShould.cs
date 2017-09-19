using System;
using EasyConfig.ConfigurationReaders;
using NUnit.Framework;
using static EasyConfig.ConfigurationSources;

namespace EasyConfig.UnitTests.ConfigurationReaders
{
    [TestFixture]
    public class CommandLineReaderShould
    {
        [TestCase(CommandLine, true)]
        [TestCase(EnvironmentVariables, false)]
        [TestCase(JsonConfig, false)]
        [TestCase(EnvironmentVariables | JsonConfig, false)]
        [TestCase(CommandLine | EnvironmentVariables, true)]
        [TestCase(CommandLine | JsonConfig, true)]
        [TestCase(CommandLine | EnvironmentVariables | JsonConfig, true)]
        public void Only_Claim_To_Be_Useful_If_In_Sources(
            ConfigurationSources sources,
            bool expected)
        {
            var commandLineReader = new CommandLineReader(new string[0]);
            var result = commandLineReader.CanBeUsedToReadFrom(sources);

            Assert.That(result, Is.EqualTo(expected));
        }
        
        [Test]
        public void Return_Value_Provided_In_Args()
        {
            string givenValue = GetAUniqueString();
            string givenKey = GetAUniqueString();
            
            var reader = new CommandLineReader(new []{$"{givenKey}={givenValue}"});

            var result = "";
            reader.TryGet(givenKey, out result);
            Assert.That(result, Is.EqualTo(givenValue));
        }

        [Test]
        public void Return_Empty_If_Value_Not_Found()
        {
            string givenValue = GetAUniqueString();
            string givenKey = GetAUniqueString();
            
            var reader = new CommandLineReader(new []{$"{GetAUniqueString()}={givenValue}"});

            var result = "";
            reader.TryGet(givenKey, out result);
            
            Assert.That(result, Is.Empty);
        }

        private static string GetAUniqueString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}