# EasyConfig.Net.Core

Easily manage configuration within your .net core application

[![Build Status](https://travis-ci.org/cohen990/EasyConfig.Net.Core.svg?branch=master)](https://www.travis-ci.org/cohen990/EasyConfig.Net.Core)

[![NuGet Version and Downloads count](https://buildstats.info/nuget/EasyConfig.Net.Core)](https://www.nuget.org/packages/EasyConfig.Net.Core)

# Prerequisites
- Install dot net core for your platform

## Run the Sample
- cd to working directory
- `cd EasyConfig.Net.Sample`
- `dotnet restore`
- `dotnet run uri_required=http://www.google.com string_required_commandline=string1 string_sensitive_required=sensitiveinformation overridable-required=overriden-in-command-line day-of-week=tuesday`

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

## V4
Cannot guarantee that the internal functionality remains the same. Large refactors of the internal code have taken place and efforts have been taken to preserve the original contract, but the risk of some nuanced detail changing forces me to update the major version number again.

The old contract is still honoured, but if you have been relying on a quirk of the library then I cannot guarantee that it will still operate the same way.

## V3
Has been upgraded to dotnet core 2.0
