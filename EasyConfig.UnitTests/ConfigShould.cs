using System;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace EasyConfig.UnitTests
{
    [TestFixture]
    public class ConfigShould
    {
        private const string EnvironmentVariableTestKey = 
            "EasyConfig_this_environment_variable_should_only_exist_during_these_tests";

        private Config _config;
        private Writer _writer;
        
        [SetUp]
        public void SetUp()
        {
            _writer = Substitute.For<Writer>();
            Environment.SetEnvironmentVariable(
                EnvironmentVariableTestKey,
                Guid.NewGuid().ToString());
            _config = new Config(_writer);
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
            Assert.Throws<ConfigurationPropertyException>(() =>
                _config.PopulateClass<WithAPrivateSetter>("property=value"));
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
            Assert.Throws<ConfigurationMissingException>(() => 
                _config.PopulateClass<TargettingNonExistentEnvironmentVariable>(
                    "defninitely_not_in_environment_variables=shouldstillthrow"));
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
            _config.WithJson("config.json");
            var config = _config.PopulateClass<WhichHasAnOverridableProperty>();

            Assert.That(config.Test, Is.EqualTo("in-config-json"));
        }

        [Test]
        public void Populate_An_Overridable_Property_With_Override_If_Available()
        {
            _config.WithJson("config.json");
            var config =
                _config.PopulateClass<WhichHasAnOverridableProperty>("InConfigJson=overriden");

            Assert.That(config.Test, Is.EqualTo("overriden"));
        }

        [Test]
        public void Populate_A_Property_Overriden_With_An_Alias()
        {
            _config.WithJson("config.json");
            var config =
                _config.PopulateClass<WhichHasAPropertyOverriddenWithAnAlias>(
                    "alternative_key=overriden");

            Assert.That(config.Test, Is.EqualTo("overriden"));
        }

        [Test]
        public void Fail_If_Required_Property_Cannot_Be_Populated()
        {
            try
            {
                _config.PopulateClass<WhichHasARequiredPropertyWithAnAlias>();
            }
            catch (OverridableConfigurationMissingException result)
            {
                Assert.That(result.Message.Contains("CommandLine"),
                    $"'{result.Message}' did not contain 'CommandLine'");
                Assert.That(result.Message.Contains("JsonConfig"),
                    $"'{result.Message}' did not contain 'JsonConfig'");
                Assert.That(result.Message.Contains("MissingConfiguration"),
                    $"'{result.Message}' did not contain 'MissingConfiguration'");
                Assert.That(result.Message.Contains("alternative-also-missing"),
                    $"'{result.Message}' did not contain 'alternative-also-missing'");
                return;
            }
            throw new AssertionException(
                "Config.Populate did not throw a OverridableConfigurationMissingException");
        }

        [Test]
        public void Populate_A_Class_Using_Provided_Json()
        {
            _config.WithJson("config.json");
            var config = _config.PopulateClass<WhichIsPopulatedFromJson>();

            Assert.That(config.Test, Is.EqualTo("in-config-json"));
        }

        [Test]
        public void Read_Correctly_From_A_Nested_Json_Object()
        {
            _config.WithJson("config.json");
            var config = _config.PopulateClass<WhichIsPopulatedByANestedJsonObject>();

            Assert.That(config.Test, Is.EqualTo("nested-object"));
        }

        [Test]
        public void Write_The_Discovered_Configuration_When_Populating_A_String()
        {
            var value = "config-value";
            _config.PopulateClass<WithAString>($"property={value}");
            _writer.Received().WriteConfigurationValue("property", value);
        }

        [Test]
        public void Write_The_Discovered_Configuration_When_Populating_An_Int()
        {
            var value = "1";
            _config.PopulateClass<WithAnInt>($"property={value}");
            _writer.Received().WriteConfigurationValue("property", value);
        }

        [Test]
        public void Write_The_Discovered_Configuration_When_Populating_A_Bool()
        {
            var value = "true";
            _config.PopulateClass<WithABool>($"property={value}");
            _writer.Received().WriteConfigurationValue("property", value);
        }

        [Test]
        public void Write_The_Discovered_Configuration_When_Populating_A_Uri()
        {
            var value = "https://www.google.com";
            _config.PopulateClass<WithAUri>($"property={value}");
            _writer.Received().WriteConfigurationValue("property", value);
        }

        [Test]
        public void Obfuscate_The_Configuration_When_Writing_A_Value_Should_Be_Hidden()
        {
            var value = "https://www.google.com";
            _config.PopulateClass<WithSensitiveInformation>($"property={value}");
            _writer.Received().ObfuscateConfigurationValue("property", value);
        }

        [Test]
        public void Ignore_Environment_If_Command_Line_Collides_With_It()
        {
            var value = "from-command-line";
            var result = _config.PopulateClass<CollidingWithEnvironmentVariable>(
                $"{EnvironmentVariableTestKey}={value}");
            
            Assert.That(result.Test, Is.EqualTo(value));
        }

        [Test]
        public void Ignore_Json_If_Command_Line_Collides_With_It()
        {
            var value = "from-command-line";
            var result = _config.PopulateClass<CollidingWithJsonConfig>(
                $"InConfigJson={value}");
            
            Assert.That(result.Test, Is.EqualTo(value));
        }

        [Test]
        public void Set_An_Enum()
        {
            var result = _config.PopulateClass<WithEnum>("property=value");

            Assert.That(result.Test, Is.EqualTo(MyEnum.value));
        }

        [Test]
        public void Set_An_Enum_Which_Is_Not_Default_Value_Of_Enum()
        {
            var result = _config.PopulateClass<WithEnum>("property=otherValue");

            Assert.That(result.Test, Is.EqualTo(MyEnum.otherValue));
        }

        [Test]
        public void Fail_If_Value_Doesnt_Match_Enum()
        {
            var value = "notanenumvalue";
            try
            {
                var result = _config.PopulateClass<WithEnum>($"property={value}");
            }
            catch (EnumConfigParseException e)
            {
                Assert.That(e.Message, Does.Contain(typeof(MyEnum).ToString()));
                Assert.That(e.Message, Does.Contain(value));
            }
        }

        [Test]
        public void Set_A_Day_Of_Week()
        {
            var result = _config.PopulateClass<WithDayOfWeek>("property=Wednesday");

            Assert.That(result.Test, Is.EqualTo(DayOfWeek.Wednesday));
        }

        [Test]
        public void Set_An_Enum_Case_Insensitively()
        {
            var result = _config.PopulateClass<WithDayOfWeek>("property=wednesday");

            Assert.That(result.Test, Is.EqualTo(DayOfWeek.Wednesday));
        }

        [Test]
        public void Set_Enum_To_Default_If_Not_Provided()
        {
            var result = _config.PopulateClass<WithDayOfWeek>("");

            Assert.That(result.Test, Is.EqualTo((DayOfWeek)0));
        }

        [Test]
        public void Set_A_Nullable_Enum()
        {
            var result = _config.PopulateClass<WithNullableDayOfWeek>("property=wednesday");

            Assert.That(result.Test.Value, Is.EqualTo(DayOfWeek.Wednesday));
        }

        [Test]
        public void Fail_If_Value_Doesnt_Match_Nullable_Enum()
        {
            var value = "notanenumvalue";
            try
            {
                var result = _config.PopulateClass<WithNullableDayOfWeek>($"property={value}");
            }
            catch (EnumConfigParseException e)
            {
                Assert.That(e.Message, Does.Contain(typeof(DayOfWeek).ToString()));
                Assert.That(e.Message, Does.Contain(value));
            }
        }

        [Test]
        public void Default_To_Null_If_Nullable_Enum_Not_Provided()
        {
            var result = _config.PopulateClass<WithNullableDayOfWeek>("");
            Assert.That(result.Test, Is.Null);
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

        private class WhichHasAPropertyOverriddenWithAnAlias
        {
            [JsonConfig("InConfigJson")]
            [OverriddenBy(ConfigurationSources.CommandLine, "alternative_key")]
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

        private class CollidingWithEnvironmentVariable
        {
            [CommandLine(EnvironmentVariableTestKey)]
            public string Test;
        }

        private class CollidingWithJsonConfig
        {
            [CommandLine("InConfigJson")]
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

        private class WithAString
        {
            [CommandLine("property")]
            public string Test;
        }

        private class WithABool
        {
            [CommandLine("property")]
            public bool Test;
        }

        private class WithAnInt
        {
            [CommandLine("property")]
            public int Test;
        }

        private class WithAUri
        {
            [CommandLine("property")]
            public Uri Test;
        }

        private class WithSensitiveInformation
        {
            [CommandLine("property"), SensitiveInformation]
            public string Test;
        }

        private class WithEnum
        {
            [CommandLine("property")]
            public MyEnum Test;

        }

        private class WithDayOfWeek
        {
            [CommandLine("property")]
            public DayOfWeek Test;
        }

        private class WithNullableDayOfWeek
        {
            [CommandLine("property")]
            public DayOfWeek? Test;
        }

        public enum MyEnum
        {
            value,
            otherValue
        }
    }
}
