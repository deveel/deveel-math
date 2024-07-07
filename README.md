 [![NuGet](https://img.shields.io/nuget/v/dmath.svg?label=dmath&logo=nuget)](https://www.nuget.org/packages/dmath/) ![NuGet Downloads](https://img.shields.io/nuget/dt/dmath?logo=nuget&label=downloads)


# Deveel Math

This library is an __opinionated__ port of the _Java Math_ package provided as part of the [Apache Harmony](https://harmony.apache.org/) framework (now defunct), that can be used to factorize big numbers and decimals in .NET applications.

## Why Deveel Math?

At the time of the development of this library, the .NET framework did not provide a native support for big numbers and decimals, and not even the `System.Numerics` namespace was available yet, which was limiting the operations that could be performed on big numbers.

In fact, during the development of the [DeveelDB](https://github.com/deveel/deveeldb) database engine, we needed a library that could handle big numbers and decimals in a more flexible way, and that could be used in a cross-platform environment.

Stil at today, the .NET framework does not provide a native support for big decimals, and the `System.Numerics` namespace is still limited to handling operations on big integers.

## What is Deveel Math?

This is a little effort to address this gap, providing the community with a library that can be used to handle big numbers and decimals in a more flexible way.

It doesn't have any ambition to replace the `System.Numerics` namespace, but it can be used as a complement to it, especially when dealing with big decimals.

Given the limited knowledge of the author in the field of numerical analysis, the library is subject to reviews and any contribution to improve the quality of the code is welcome.

## How to Install It

The library is available as a NuGet package, and it can be installed in any .NET application that supports the __.NET 6.0__ or later (prior support to _.NET 4.8_ and _.NET Standard 1.3 has been dropped).

The binaries are available in two deployment streams:

| Type | Source | Package |
|------|--------|---------|
| Stable | _NuGet_ | [![NuGet](https://img.shields.io/nuget/v/dmath.svg?label=dmath&logo=nuget)](https://www.nuget.org/packages/dmath/) |
| Pre-Release | _GitHub_ | [![Static Badge](https://img.shields.io/badge/prerelease-yellow?logo=nuget&label=dmath)](https://github.com/deveel/deveel-math/pkgs/nuget/dmath)
  |

To install the `dmath` library you can use the following command from the NuGet Package Manager Console on the root of your project:

```
PM> Install-Package dmath
```

or rather using the `dotnet` CLI:

```
$ dotnet add package dmath
```

__Note__: _Since version 2.0.x the library has been migrated to .NET 6.0 and the support for .NET Standard 1.3 has been dropped._

## BigDecimal

The `BigDecimal` class represents a big decimal number that can be used to perform arithmetic operations with arbitrary precision.

The class provides a set of methods to perform arithmetic operations, such as addition, subtraction, multiplication, division, and rounding.

### Creating a BigDecimal

To create a new `BigDecimal` instance, you can use one of the following constructors:

```csharp
// Creating an instance from an integer
var number = new BigDecimal(1234567890);

// Creating an instance from a long integer
var number = new BigDecimal(1234567890L);

// Creating an instance from a double
var number = new BigDecimal(1234567890.123456);
```

or rather from a string:

```csharp
// Parsing a string to a big decimal
var number = BigDecimal.Parse("1234567890");
```

## Contributing

If you want to contribute to the development of this library, you can fork the repository and submit a pull request with your changes.

Please make sure to follow the coding style and conventions used in the project, and to provide a clear description of the changes you are proposing.

## License

The library is released under the terms of the [Apache License 2.0](LICENSE), and it is provided as-is without any warranty or support.