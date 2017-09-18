using System;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;
using NUnit.Framework;
#pragma warning disable 649

namespace EasyConfig.UnitTests
{
    [TestFixture]
    public class ConfigShould
    {
        private const string EnvironmentVariableTestKey = "EasyConfig_this_environment_variable_should_only_exist_during_these_tests";
        private Config _config;

        [SetUp]
        public void SetUp()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableTestKey, Guid.NewGuid().ToString());
            _config = new Config();
        }

        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableTestKey, null);
        }

        [Test]
        public void Populate_A_Class_With_A_Public_Getter_And_Setter()
        {
            var config = _config.PopulateClass<WithAPublicGetterAndSetter>("property=value");

            Assert.That(config.Test, Is.EqualTo("value"));
        }

        [Test]
        public void Populate_A_Class_With_An_Alias_Defined()
        {
            var config = _config.PopulateClass<WithAnAliasDefined>("a=aliased");

            Assert.That(config.Test, Is.EqualTo("aliased"));
        }

        [Test]
        public void Fail_If_Given_A_Property_With_A_Private_Setter()
        {
            Assert.Throws<ConfigurationPropertyException>(() => _config.PopulateClass<WithAPrivateSetter>("property=value"));
        }

        [Test]
        public void Provide_A_Useful_Error_Message_If_It_Fails_On_A_Class_With_A_Private_Setter()
        {
            try
            {
                _config.PopulateClass<WithAPrivateSetter>("property=value");
            }
            catch (ConfigurationPropertyException result)
            {
                Assert.That(result.Message, Contains.Substring("'Test'"));
            }
        }

        [Test]
        public void Populate_A_Class_Which_Is_Populated_By_Environment_Variables()
        {
            Assert.DoesNotThrow(() => _config.PopulateClass<PopulatedByEnvironmentVariables>());
        }

        [Test]
        public void Fail_If_Key_Is_Not_In_Environment_Variables()
        {
            Assert.Throws<ConfigurationMissingException>(() => _config.PopulateClass<TargettingNonExistentEnvironmentVariable>("defninitely_not_in_environment_variables=shouldstillthrow"));
        }

        [Test]
        public void Fail_Quietly_If_Parameter_Is_Not_Required()
        {
            Assert.DoesNotThrow(() => _config.PopulateClass<WhichHasOptionalParameters>());
        }

        [Test]
        public void Populate_A_Default_Property_With_Its_Default_If_Override_Not_Provided()
        {
            var config = _config.PopulateClass<WhichHasADefaultValue>();

            Assert.That(config.Test, Is.EqualTo("defaulttest"));
        }

        [Test]
        public void Populate_An_Overridable_Property_Normally_If_Not_Overridden()
        {
            _config.UseJson("config.json");
            var config = _config.PopulateClass<WhichHasAnOverridableProperty>();

            Assert.That(config.Test, Is.EqualTo("in-config-json"));
        }

        [Test]
        public void Populate_An_Overridable_Property_With_Override_If_Available()
        {
            _config.UseJson("config.json");
            var config = _config.PopulateClass<WhichHasAnOverridableProperty>("InConfigJson=overriden");

            Assert.That(config.Test, Is.EqualTo("overriden"));
        }

        [Test]
        public void Populate_A_Property_Overriden_With_An_Alias()
        {
            _config.UseJson("config.json");
            var config = _config.PopulateClass<WhichHasAPropertyWithAnAlias>("alternative_key=overriden");

            Assert.That(config.Test, Is.EqualTo("overriden"));
        }

        [Test]
        public void Fail_If_Required_Property_Cannot_Be_Populated()
        {
            try
            {
                var config = _config.PopulateClass<WhichHasARequiredPropertyWithAnAlias>();
            }
            catch (OverridableConfigurationMissingException result)
            {
                Assert.That(result.Message.Contains("CommandLine"), $"'{result.Message}' did not contain 'CommandLine'");
                Assert.That(result.Message.Contains("JsonConfig"), $"'{result.Message}' did not contain 'JsonConfig'");
                Assert.That(result.Message.Contains("MissingConfiguration"), $"'{result.Message}' did not contain 'MissingConfiguration'");
                Assert.That(result.Message.Contains("alternative-also-missing"), $"'{result.Message}' did not contain 'alternative-also-missing'");
                return;
            }
            throw new AssertionException("Config.Populate did not throw a OverridableConfigurationMissingException");
        }

        [Test]
        public void Populate_A_Class_Using_Provided_Json()
        {
            _config.UseJson("config.json");
            var config = _config.PopulateClass<WhichIsPopulatedFromJson>();

            Assert.That(config.Test, Is.EqualTo("in-config-json"));
        }

        [Test]
        public void Read_Correctly_From_A_Nested_Json_Object()
        {
            _config.UseJson("config.json");
            var config = _config.PopulateClass<WhichIsPopulatedByANestedJsonObject>();

            Assert.That(config.Test, Is.EqualTo("nested-object"));
        }
        
        private class WithAnAliasDefined
        {
            [CommandLine("unaliased", "a")]
            public string Test;
        }

        private class WithAPublicGetterAndSetter
        {
            [CommandLine("property")]
            public string Test { get; set; }
        }

        private class WithAPrivateSetter
        {
            [CommandLine("property")]
            public string Test { get; }
        }

        private class WhichHasAnOverridableProperty
        {
            [JsonConfig("InConfigJson")]
            [OverriddenBy(ConfigurationSources.CommandLine)]
            public string Test { get; set; }
        }

        private class WhichHasAPropertyWithAnAlias
        {
            [JsonConfig("InConfigJson")]
            [OverriddenBy(ConfigurationSources.CommandLine, "alternative_key")]
            public string Test { get; set; }
        }

        private class WhichHasARequiredProperty
        {
            [JsonConfig("MissingConfiguration"), Required]
            [OverriddenBy(ConfigurationSources.CommandLine)]
            public string Test { get; set; }
        }

        private class WhichHasARequiredPropertyWithAnAlias
        {
            [JsonConfig("MissingConfiguration"), Required]
            [OverriddenBy(ConfigurationSources.CommandLine, "alternative-also-missing")]
            public string Test { get; set; }
        }

        private class WhichIsPopulatedFromJson
        {
            [JsonConfig("InConfigJson")]
            public string Test;
        }

        private class WhichIsPopulatedByANestedJsonObject
        {
            [JsonConfig("NestedObject.InConfigJson")]
            public string Test;
        }

        private class PopulatedByEnvironmentVariables
        {
            [Environment(EnvironmentVariableTestKey), Required]
            public string Test;
        }

        private class TargettingNonExistentEnvironmentVariable
        {
            [Environment("shouldnt_be_in_environment_variables"), Required]
            public string Test;
        }

        private class WhichHasOptionalParameters
        {
            [Environment("shouldnt_be_in_environment_variables")]
            public string Test;
        }

        private class WhichHasADefaultValue
        {
            [Environment("shouldnt_be_in_environment_variables"), Default("defaulttest")]
            public string Test;
        }
    }
}
