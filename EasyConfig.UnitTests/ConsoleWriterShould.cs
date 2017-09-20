using System;
using NUnit.Framework;

namespace EasyConfig.UnitTests
{
    [TestFixture]
    public class ConsoleWriterShould
    {
        private static string _writtenContent;

        [Test]
        public void Write_Value()
        {
            var writer = new TestConsoleWriter();
            var key = GetARandomString();
            var value = GetARandomString();
            
            writer.WriteConfigurationValue(key, value);
            
            Assert.That(_writtenContent, Does.Contain(key));
            Assert.That(_writtenContent, Does.Contain(value));
        }

        [Test]
        public void Not_Write_Obfuscated_Value()
        {
            var writer = new TestConsoleWriter();
            var key = GetARandomString();
            var valueToObfuscate = GetARandomString();
            
            writer.ObfuscateConfigurationValue(key, valueToObfuscate);
            
            Assert.That(_writtenContent, Does.Contain(key));
            Assert.That(_writtenContent, Does.Not.Contain(valueToObfuscate));
        }

        private static string GetARandomString()
        {
            return Guid.NewGuid().ToString();
        }

        private class TestConsoleWriter : ConsoleWriter
        {
            protected override void WriteLine(string content)
            {
                _writtenContent = content;
            }
        }
    }
}