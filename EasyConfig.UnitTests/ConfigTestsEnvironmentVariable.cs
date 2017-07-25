using System;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;
using NUnit.Framework;
#pragma warning disable 649

namespace EasyConfig.UnitTests
{
    [TestFixture]
    public class ConfigTestsEnvironmentVariable
    {
        private const string EnvironmentVariableTestKey = "EasyConfig_this_environment_variable_should_only_exist_during_these_tests";

        [SetUp]
        public void SetUp()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableTestKey, Guid.NewGuid().ToString());
        }

        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableTestKey, null);
        }

        [Test]
        public void Populate_InEnvironmentVariabless_RequiredParamInEnvironment_DoesntThrow()
        {
            Assert.DoesNotThrow(() => Config.Populate<InEnvironmentVariables>());
        }

        [Test]
        public void Populate_NotInEnvironmentVariables_RequiredParamNotInEnvironment_Throws()
        {
            Assert.Throws<ConfigurationMissingException>(() => Config.Populate<NotInEnvironmentVariables>());
        }

        [Test]
        public void Populate_NotInEnvironmentVariables_RequiredParamNotInEnvironmentButInArgs_Throws()
        {
            Assert.Throws<ConfigurationMissingException>(() => Config.Populate<NotInEnvironmentVariables>("defninitely_not_in_environment_variables=shouldstillthrow"));
        }

        [Test]
        public void Populate_NotRequired_ParamNotInEnvironmentButInArgs_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => Config.Populate<NotRequired>());
        }

        [Test]
        public void Populate_HasDefault_UsesDefault()
        {
            var config = Config.Populate<HasDefault>();

            Assert.That(config.Test, Is.EqualTo("defaulttest"));
        }

        private class InEnvironmentVariables
        {
            [Environment(EnvironmentVariableTestKey), Required]
            public string Test;
        }

        private class NotInEnvironmentVariables
        {
            [Environment("shouldnt_be_in_environment_variables"), Required]
            public string Test;
        }

        private class NotRequired
        {
            [Environment("shouldnt_be_in_environment_variables")]
            public string Test;
        }

        private class HasDefault
        {
            [Environment("shouldnt_be_in_environment_variables"), Default("defaulttest")]
            public string Test;
        }
    }
}
