# EasyConfig.Net.Core

Easily manage configuration within your .net core application

![Build Status](https://travis-ci.org/cohen990/EasyConfig.Net.Core.svg?branch=master)

[![NuGet Version and Downloads count](https://buildstats.info/nuget/EasyConfig.Net.Core)](https://www.nuget.org/packages/EasyConfig.Net.Core)

# Prerequisites
- Install dot net core for your platform

## Run the Sample
- cd to working directory
- `cd EasyConfig.Net.Sample`
- `dotnet restore`
- `dotnet run uri_required=http://www.google.com string_required_commandline=string1 string_sensitive_required=sensitiveinformation`

## Run the Tests
- cd to working directory
- `cd EasyConfig.UnitTests`
- `dotnet restore`
- `dotnet test`

## Features

* Strongly typed parameters
	* Uri
	* String
	* Int
* Configuration Sources
	* Environment Variables
	* Command Line Arguments
	* Json Config files
* Overridable Configuration

### Coming Soon

* Other missing parameter types

# Things you need to know about versions

## V3
Has been upgraded to dotnet core 2.0
