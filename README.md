[![NuGet](https://img.shields.io/nuget/v/dmath.svg?label=dmath&logo=nuget)](https://www.nuget.org/packages/dmath/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/dmath?logo=nuget&label=downloads)](https://www.nuget.org/packages/dmath/)
[![Coverage](https://img.shields.io/codecov/c/github/deveel/deveel-math?logo=codecov)](https://codecov.io/gh/deveel/deveel-math)
[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-ffdd00?logo=buy-me-a-coffee&logoColor=black)](https://www.buymeacoffee.com/tsutomi)

# Deveel Math

This library is an __opinionated__ port of the _Java Math_ package provided as part of the [Apache Harmony](https://harmony.apache.org/) framework (now defunct), that can be used to handle big numbers and decimals in .NET applications.

## Why Deveel Math?

At the time of the development of this library, the .NET framework did not provide a native support for big numbers and decimals, and not even the `System.Numerics` namespace was available yet, which was limiting the operations that could be performed on big numbers.

In fact, during the development of the [DeveelDB](https://github.com/deveel/deveeldb) database engine, we needed a library that could handle big numbers and decimals in a more flexible way, and that could be used in a cross-platform environment.

Still today, the .NET framework does not provide a native support for big decimals, and the `System.Numerics` namespace is still limited to handling operations on big integers.

## What is Deveel Math?

This is a little effort to address this gap, providing the community with a library that can be used to handle big numbers and decimals in a more flexible way.

It doesn't have any ambition to replace the `System.Numerics` namespace, but it can be used as a complement to it, especially when dealing with big decimals.

Given the limited knowledge of the author in the field of numerical analysis, the library is subject to reviews and any contribution to improve the quality of the code is welcome.

## Quick Examples

### Financial Calculation with BigDecimal

```csharp
using Deveel.Math;

// Precise currency calculation with controlled rounding
var price = BigDecimal.Parse("19.99");
var quantity = new BigDecimal(3);
var taxRate = BigDecimal.Parse("0.0875"); // 8.75% tax

var subtotal = price * quantity;
var tax = (subtotal * taxRate).ScaleTo(2, RoundingMode.HalfUp); // Round tax to 2 decimals
var total = subtotal + tax;

Console.WriteLine($"Subtotal: {subtotal.ToString("P")}");  // 59.97
Console.WriteLine($"Tax: {tax.ToString("P")}");            // 5.25
Console.WriteLine($"Total: {total.ToString("P")}");        // 65.22
```

### Large Number Arithmetic with BigInteger

```csharp
using Deveel.Math;

// Working with numbers beyond long.MaxValue
var a = BigInteger.Parse("123456789012345678901234567890");
var b = BigInteger.Parse("987654321098765432109876543210");

var product = a * b;
var gcd = BigMath.Gcd(a, b);
var isPrime = BigInteger.IsProbablePrime(a, 100);

Console.WriteLine($"Product: {product}");
Console.WriteLine($"GCD: {gcd}");
Console.WriteLine($"Is 'a' probably prime? {isPrime}");
```

## Documentation

For detailed documentation, see the [docs](docs/index.md) folder:

| Topic | Description |
|-------|-------------|
| [Getting Started](docs/index.md) | Overview, installation, and quick start |
| [BigDecimal](docs/bigdecimal.md) | Creation, parsing, conversion, operators, arithmetic, scale & precision, formatting |
| [BigInteger](docs/biginteger.md) | Creation, parsing, conversion, operators, arithmetic, modular math, bit operations, primality |
| [MathContext & RoundingMode](docs/math-context.md) | Precision control, rounding strategies, predefined contexts |
| [Advanced Math Operations](docs/advanced-math.md) | BigMath static class, advanced division, powers, random generation |
| [Interoperability](docs/interop.md) | BigInteger vs System.Numerics, type conversions, primitive conversions, parsing |

## Performance

Deveel Math has been benchmarked against .NET built-in types across .NET 6.0 through 10.0. Key results on .NET 10.0:

| Area | Result |
|------|--------|
| **BigDecimal arithmetic** | 27-119 ns per operation (vs 1-47 ns for `decimal`), with 46-55% improvement from .NET 6.0 to 10.0 |
| **BigInteger arithmetic** | Competitive with `System.Numerics.BigInteger`; faster at division and 1024-bit+ multiplication |
| **BigInteger ModPow** | Up to **5.6x faster** than `System.Numerics` at 512-bit |
| **BigInteger parsing** | **2.3x faster** for large numbers (500 digits) |
| **Memory allocations** | Higher than native types (96 B - 24 KB per operation), but acceptable for arbitrary-precision workloads |

For detailed benchmark tables and analysis, see [Performance Benchmarks](docs/performance.md).

## How to Install It

The library is available as a NuGet package, and it can be installed in any .NET application that supports __.NET 8.0__, __.NET 9.0__, or __.NET 10.0__.

The binaries are available in two deployment streams:

| Type | Source | Package |
|------|--------|---------|
| Stable | _NuGet_ | [![NuGet](https://img.shields.io/nuget/v/dmath.svg?label=dmath&logo=nuget)](https://www.nuget.org/packages/dmath/) |
| Pre-Release | _GitHub_ | [![Static Badge](https://img.shields.io/badge/prerelease-yellow?logo=nuget&label=dmath)](https://github.com/deveel/deveel-math/pkgs/nuget/dmath)

To install the `dmath` library you can use the following command from the NuGet Package Manager Console on the root of your project:

```
PM> Install-Package dmath
```

or rather using the `dotnet` CLI:

```
$ dotnet add package dmath
```

## BigInteger vs System.Numerics

Deveel Math includes its own `BigInteger` implementation alongside `BigDecimal`. While .NET provides `System.Numerics.BigInteger`, Deveel.Math's `BigInteger` exists primarily to support `BigDecimal` (which .NET does not provide) and to maintain Java-compatible behavior.

**Key differences:**

- **Internal representation**: Deveel.Math uses sign + magnitude; `System.Numerics` uses two's complement
- **BigDecimal integration**: Deveel.Math.BigInteger is the native unscaled value type for BigDecimal
- **Java compatibility**: Results match Java's `java.math` package semantics
- **Additional features**: Primality testing (`IsProbablePrime`, `NextProbablePrime`), modular inverse (`ModInverse`), radix-based parsing (base 2-36)

**When to use which:**

- Use **Deveel.Math.BigInteger** when working with `BigDecimal`, when you need Java-compatible results, or when using Deveel ecosystem libraries
- Use **System.Numerics.BigInteger** when your codebase already uses it or when interoperating with third-party libraries that expect it

**Converting between them is straightforward:**

```csharp
// Deveel.Math -> System.Numerics (implicit)
System.Numerics.BigInteger sys = deveelMathBI;

// System.Numerics -> Deveel.Math (explicit)
var deveel = (BigInteger)sys;
// or
var deveel = BigInteger.FromSystemBigInteger(sys);
```

For a complete guide, see the [Interoperability documentation](docs/interop.md).

## Key Classes

#### BigDecimal

The `BigDecimal` class represents a big decimal number that can be used to perform arithmetic operations with arbitrary precision.

The class provides a set of methods to perform arithmetic operations, such as addition, subtraction, multiplication, division, and rounding.

##### Creating a BigDecimal

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

#### BigInteger

The `BigInteger` class represents an arbitrary-precision integer. It was originally ported from Apache Harmony but is now retained for compatibility and extended functionality beyond `System.Numerics.BigInteger`.

#### MathContext and RoundingMode

The `MathContext` class encapsulates settings that describe the number of digits and the rounding algorithm to be used for `BigDecimal` operations. The `RoundingMode` enumeration specifies the rounding behavior for operations that require rounding.

## Building and Testing

### Requirements

- .NET 8.0, 9.0, or 10.0 SDK
- xUnit v3 test runner (included in the test project)

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run tests with code coverage (generates Cobertura report)
dotnet test -- --coverage --coverage-output-format cobertura
```

### CI/CD

The project uses GitHub Actions for continuous integration:

- **Build Matrix**: Tests run on .NET 8.0, 9.0, and 10.0 across Ubuntu, Windows, and macOS
- **Code Coverage**: Reports are published to [Codecov](https://codecov.io/gh/deveel/deveel-math)
- **Versioning**: Uses GitVersion for semantic versioning
- **Publishing**: Pre-release packages are published to GitHub Packages; stable releases to NuGet

## Contributing

If you want to contribute to the development of this library, you can fork the repository and submit a pull request with your changes.

Please make sure to follow the coding style and conventions used in the project, and to provide a clear description of the changes you are proposing.

## Contributors

Thanks to everyone who has contributed to Deveel Math:

[![Contributors](https://contrib.rocks/image?repo=deveel/deveel-math)](https://github.com/deveel/deveel-math/graphs/contributors)

## License

The library is released under the terms of the [Apache License 2.0](LICENSE), and it is provided as-is without any warranty or support.
