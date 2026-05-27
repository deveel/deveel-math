# Deveel Math Documentation

Welcome to the Deveel Math documentation. This guide covers arbitrary-precision arithmetic in .NET using the `BigDecimal` and `BigInteger` types.

## What is Deveel Math?

Deveel Math is a .NET library providing arbitrary-precision decimal and integer arithmetic. It is a port of the Java `java.math` package from [Apache Harmony](https://harmony.apache.org/), adapted for the .NET ecosystem.

| Type | Description |
|------|-------------|
| `BigDecimal` | Arbitrary-precision decimal numbers with configurable precision and rounding |
| `BigInteger` | Arbitrary-precision integers with arithmetic, bitwise, and primality support |

## Why Use Deveel Math?

| Feature | .NET Built-in | Deveel Math |
|---------|--------------|-------------|
| BigInteger | `System.Numerics.BigInteger` | `Deveel.Math.BigInteger` |
| BigDecimal | **Not available** | `Deveel.Math.BigDecimal` |
| Configurable rounding | No | Yes (`MathContext`, `RoundingMode`) |
| IEEE 754r precision formats | No | Yes (`Decimal32`, `Decimal64`, `Decimal128`) |
| Java-compatible behavior | No | Yes |
| Primality testing | No | Yes (`IsProbablePrime`, `NextProbablePrime`) |

## Installation

```bash
dotnet add package dmath
```

## Quick Start

### BigDecimal Example

```csharp
using Deveel.Math;

var price = BigDecimal.Parse("19.99");
var quantity = new BigDecimal(3);
var taxRate = BigDecimal.Parse("0.0875");

var subtotal = price * quantity;
var tax = (subtotal * taxRate).ScaleTo(2, RoundingMode.HalfUp);
var total = subtotal + tax;

Console.WriteLine($"Total: {total}"); // 65.22
```

### BigInteger Example

```csharp
using Deveel.Math;

var a = BigInteger.Parse("123456789012345678901234567890");
var b = BigInteger.Parse("987654321098765432109876543210");

Console.WriteLine($"Product: {a * b}");
Console.WriteLine($"GCD: {BigMath.Gcd(a, b)}");
```

## Documentation Pages

| Page | Content |
|------|---------|
| [BigDecimal](bigdecimal.md) | Creation, parsing, conversion, operators, arithmetic methods, scale & precision, formatting |
| [BigInteger](biginteger.md) | Creation, parsing, conversion, operators, arithmetic, modular math, bit operations, primality |
| [MathContext & RoundingMode](math-context.md) | Precision control, all 8 rounding modes, predefined IEEE 754r contexts |
| [Advanced Math Operations](advanced-math.md) | BigMath static class, advanced division, powers, random generation, primality testing |
| [Interoperability](interop.md) | BigInteger vs System.Numerics, type conversion tables, primitive conversions, parsing deep dive |

## Building & Testing

```bash
dotnet restore && dotnet build && dotnet test
```

## License

[Apache License 2.0](../LICENSE)
