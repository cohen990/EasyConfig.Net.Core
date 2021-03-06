﻿using System;
using EasyConfig.Attributes;
using EasyConfig.Exceptions;

namespace EasyConfig.Net.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config().WithJson("config.json");
            try
            {
                var configurationValues = config.PopulateClass<SampleConfig>(args);
            }
            catch (EasyConfigException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    internal class SampleConfig
    {
        // Can be supplied by environment or command line. If not supplied, throws a ConfigurationMissingException
        [CommandLine("uri_required"), Required]
        public Uri Endpoint1;

        [Environment("uri_not_required")]
        public Uri Endpoint2;

        [CommandLine("string_required_commandline"), Required]
        public string Username;

        [CommandLine("string_sensitive_required"), Required, SensitiveInformation]
        public string Password;

        [CommandLine("int_default"), Default(1000)]
        public int Defaultable;

        [JsonConfig("InConfigJson")]
        public string InJsonConfig;

        [JsonConfig("NestedObject.InConfigJson")]
        public string NestedJsonConfig { get; set; }

        [CommandLine("bool_example")]
        public bool BoolExample { get; set; }

        [CommandLine("aliased", "a")]
        public bool Aliased { get; set; }

        [JsonConfig("Overridable")]
        [OverriddenBy(ConfigurationSources.CommandLine)]
        public bool Overridable;

        [JsonConfig("OverridableAlternativeKey")]
        [OverriddenBy(ConfigurationSources.CommandLine, "alternative-key")]
        public string OverridableAlternativeKey { get; set; }

        [JsonConfig("OverridableRequired")]
        [OverriddenBy(ConfigurationSources.CommandLine, "overridable-required")]
        [Required]
        public string OverridableRequired { get; set; }

        [CommandLine("day-of-week"), Required]
        public DayOfWeek EnumExample { get; set; }

        // will default to (DayOfWeek)0 if not provided
        [CommandLine("day-of-week-not-required")]
        public DayOfWeek EnumNotRequiredExample { get; set; }

        // will default to null if not provided
        [CommandLine("day-of-week-nullable")]
        public DayOfWeek? NullableEnumExample { get; set; }
    }
}