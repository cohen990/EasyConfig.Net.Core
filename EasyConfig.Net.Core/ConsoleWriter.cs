using System;

namespace EasyConfig
{
    public class ConsoleWriter : Writer
    {
        public void WriteConfigurationValue(string key, string value)
        {
            WriteLine($"Using '{value}' for '{key}'");
        }
        
        public void ObfuscateConfigurationValue(string key, string value)
        {
            var obfuscatedValue = new string('*', 10);

            WriteLine($"Using '{obfuscatedValue}' for '{key}'");
        }

        protected virtual void WriteLine(string content)
        {
            Console.WriteLine(content);
        }
    }
}