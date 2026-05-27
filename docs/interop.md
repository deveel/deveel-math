# Interoperability Guide

This page covers the relationship between Deveel.Math types and .NET built-in types, including `System.Numerics.BigInteger`, primitive numeric types, and detailed parsing behavior.

## Why a Separate BigInteger?

Deveel Math includes its own `BigInteger` implementation for several important reasons:

### Historical Context

Deveel Math originated as a port of the Java `java.math` package from [Apache Harmony](https://harmony.apache.org/). At the time of the original port, .NET did not have a mature `System.Numerics` namespace, and the library was developed to fill the gap for arbitrary-precision arithmetic in .NET applications.

### BigDecimal is the Primary Driver

The main reason Deveel Math exists today is **`BigDecimal`** — a type that .NET still does not provide natively. `BigDecimal` requires an internal `BigInteger` for its unscaled value. Using a consistent, co-designed `BigInteger` ensures:

- **Java-compatible behavior**: Both types follow the same semantics as Java's `java.math.BigDecimal` and `java.math.BigInteger`
- **Consistent internal representation**: `BigDecimal` and `BigInteger` share implementation details for optimal performance
- **Cross-platform consistency**: Results are identical whether running on .NET Framework, .NET Core, or .NET 8+

### Internal Representation Differences

| Aspect | Deveel.Math.BigInteger | System.Numerics.BigInteger |
|--------|----------------------|---------------------------|
| Internal format | Sign + magnitude (separate sign field) | Two's complement |
| Designed for | Integration with BigDecimal | Standalone integer arithmetic |
| Java compatibility | Yes | No |
| BigDecimal support | Yes (native) | No |

## When to Use Each Type

### Use Deveel.Math.BigInteger When

| Scenario | Reason |
|----------|--------|
| Working with `BigDecimal` | Deveel.Math.BigDecimal requires Deveel.Math.BigInteger internally |
| Java-compatible behavior needed | Results must match Java's `java.math` package semantics |
| Using Deveel ecosystem libraries | DeveelDB and other Deveel libraries expect Deveel.Math types |
| Primality testing required | `IsProbablePrime`, `NextProbablePrime`, `ProbablePrime` are only available here |
| Modular inverse needed | `ModInverse` is only available here |
| Radix-based parsing (base 2-36) | `Parse(string, int radix)` is only available here |

### Use System.Numerics.BigInteger When

| Scenario | Reason |
|----------|--------|
| Codebase already uses it | Avoids unnecessary conversions |
| Platform-specific optimizations needed | `System.Numerics` may have architecture-specific optimizations |
| Third-party library compatibility | Some libraries only accept `System.Numerics.BigInteger` |

## Converting Between BigInteger Types

### Conversion Methods

| Direction | Method | Syntax | Notes |
|-----------|--------|--------|-------|
| Deveel → System.Numerics | `ToSystemBigInteger()` | `var sys = deveel.ToSystemBigInteger();` | Lossless |
| Deveel → System.Numerics | Implicit operator | `System.Numerics.BigInteger sys = deveel;` | Lossless |
| System.Numerics → Deveel | `FromSystemBigInteger()` | `var deveel = BigInteger.FromSystemBigInteger(sys);` | Lossless |
| System.Numerics → Deveel | Explicit cast | `var deveel = (BigInteger)sys;` | Lossless |

### Practical Example

```csharp
// Working with a library that uses System.Numerics
var systemValue = SomeLibrary.ComputeLargeNumber(); // Returns System.Numerics.BigInteger

// Convert to Deveel.Math for BigDecimal operations
var deveelValue = BigInteger.FromSystemBigInteger(systemValue);
var decimalResult = new BigDecimal(deveelValue, 4); // Scale to 4 decimal places

// Convert back for the library
var systemResult = decimalResult.UnscaledValue.ToSystemBigInteger();
SomeLibrary.ProcessResult(systemResult);
```

## Primitive Type Conversions

### Primitives → BigInteger (Implicit)

| Source Type | Syntax | Always Succeeds | Example |
|-------------|--------|-----------------|---------|
| `byte` | `BigInteger x = (byte)255;` | Yes | `(byte)255` → `255` |
| `sbyte` | `BigInteger x = (sbyte)-128;` | Yes | `(sbyte)-128` → `-128` |
| `short` | `BigInteger x = (short)-32768;` | Yes | `(short)-32768` → `-32768` |
| `int` | `BigInteger x = 42;` | Yes | `42` → `42` |
| `long` | `BigInteger x = 42L;` | Yes | `9223372036854775807L` → `9223372036854775807` |

### BigInteger → Primitives (Explicit)

| Target Type | Syntax | Overflow Behavior | Example (BigInteger = 2^64 + 1) |
|-------------|--------|-------------------|----------------------------------|
| `short` | `(short)bigInt` | Truncates to 16 bits (modulo 2^16) | `1` |
| `int` | `(int)bigInt` | Truncates to 32 bits (modulo 2^32) | `1` |
| `long` | `(long)bigInt` | Truncates to 64 bits (modulo 2^64) | `1` |

### BigInteger → Floating Point (Implicit)

| Target Type | Syntax | Precision Limit | Example (BigInteger = 2^60 + 123456789) |
|-------------|--------|-----------------|------------------------------------------|
| `float` | `float f = bigInt;` | ~7 significant digits | `1.152922E+18` (loses precision) |
| `double` | `double d = bigInt;` | ~15 significant digits | `1.1529215046069691E+18` (loses precision) |
| `decimal` | `decimal dec = bigInt;` | Via `ToInt64()`; throws if > `long.MaxValue` | Throws if value exceeds 64-bit range |

### Primitives → BigDecimal

| Source Type | Syntax | Exact? | Notes |
|-------------|--------|--------|-------|
| `int` | `new BigDecimal(42)` | Yes | Scale = 0 |
| `long` | `new BigDecimal(42L)` | Yes | Scale = 0 |
| `double` | `new BigDecimal(0.1)` | **No** | See "Double Constructor Caveat" below |
| `BigInteger` | `new BigDecimal(bigInt)` | Yes | Scale = 0 |
| `BigInteger` | `new BigDecimal(bigInt, scale)` | Yes | Explicit scale |

### BigDecimal → Primitives

| Target Type | Method | Precision Notes | Example (BigDecimal = 12345.6789) |
|-------------|--------|-----------------|------------------------------------|
| `double` | `bd.ToDouble()` | ~15 significant digits | `12345.6789` (exact for this value) |
| `float` | `bd.ToSingle()` | ~7 significant digits | `12345.68` (rounded) |
| `int` | `(int)bd.UnscaledValue` | Truncates to 32 bits; **ignores scale** | `123456789` (not 12345!) |
| `long` | `(long)bd.UnscaledValue` | Truncates to 64 bits; **ignores scale** | `123456789` (not 12345!) |

**Important**: `BigDecimal` has no direct conversion to `int`/`long` that respects scale. To get the integral part:

```csharp
var bd = BigDecimal.Parse("123.45");
var integral = BigMath.DivideToIntegral(bd, BigDecimal.One);
long value = integral.UnscaledValue.ToInt64(); // 123
```

## Double Constructor Caveat

The `BigDecimal(double)` constructor does **not** produce exact decimal values for most inputs because the `double` itself is already an approximation:

| Input | `new BigDecimal(double)` Result | `BigDecimal.Parse(string)` Result |
|-------|-------------------------------|-----------------------------------|
| `0.1` | `0.1000000000000000055511151231257827021181583404541015625` | `0.1` |
| `0.2` | `0.200000000000000011102230246251565404236316680908203125` | `0.2` |
| `0.3` | `0.299999999999999988897769753748434595763683319091796875` | `0.3` |
| `1.1` | `1.100000000000000088817841970012523233890533447265625` | `1.1` |

**Recommendation**: Always use `BigDecimal.Parse(string)` for exact decimal values.

## Parsing Deep Dive

### BigDecimal Parsing

#### Supported String Formats

| Format | Example | Result | Scale |
|--------|---------|--------|-------|
| Plain decimal | `"123.456"` | `123.456` | 3 |
| Scientific (positive exponent) | `"1.23E+10"` | `12300000000` | 0 |
| Scientific (negative exponent) | `"1.23e-5"` | `0.0000123` | 7 |
| Negative value | `"-42.50"` | `-42.50` | 2 |
| Leading zeros | `"007.00"` | `7.00` | 2 |
| Trailing zeros preserved | `"100.00"` | `100.00` | 2 |

#### Parse Signatures

| Method | Signature | Description |
|--------|-----------|-------------|
| `Parse()` | `BigDecimal.Parse(string s)` | Parses with invariant culture (period as decimal separator) |
| `Parse()` | `BigDecimal.Parse(string s, MathContext mc)` | Parses with rounding applied |
| `Parse()` | `BigDecimal.Parse(string s, IFormatProvider provider)` | Parses with culture-specific format |
| `Parse()` | `BigDecimal.Parse(char[] chars, int offset, int length)` | Parses from char array slice |
| `TryParse()` | `BigDecimal.TryParse(string s, out BigDecimal value)` | Safe parsing; returns false on failure |
| `TryParse()` | `BigDecimal.TryParse(string s, MathContext mc, out BigDecimal value)` | Safe parsing with rounding |
| `TryParse()` | `BigDecimal.TryParse(char[] chars, out BigDecimal value)` | Safe parsing from char array |

### BigInteger Parsing

#### Supported Radices

| Radix | Name | Valid Characters | Example | Result |
|-------|------|-----------------|---------|--------|
| 2 | Binary | `0-1` | `"101010"` | `42` |
| 8 | Octal | `0-7` | `"52"` | `42` |
| 10 | Decimal | `0-9` | `"42"` | `42` |
| 16 | Hexadecimal | `0-9`, `a-f` | `"2a"` or `"2A"` | `42` |
| 36 | Base-36 | `0-9`, `a-z` | `"16"` | `42` (1×36 + 6) |

#### Parse Signatures

| Method | Signature | Description |
|--------|-----------|-------------|
| `Parse()` | `BigInteger.Parse(string s)` | Parses decimal string |
| `Parse()` | `BigInteger.Parse(string s, int radix)` | Parses in given base (2-36) |
| `TryParse()` | `BigInteger.TryParse(string s, out BigInteger value)` | Safe decimal parsing |
| `TryParse()` | `BigInteger.TryParse(string s, int radix, out BigInteger value)` | Safe parsing with radix |

### MathContext Parsing

| Method | Signature | Format |
|--------|-----------|--------|
| `Parse()` | `MathContext.Parse(string s)` | `"precision=<n> roundingMode=<mode>"` |
| `TryParse()` | `MathContext.TryParse(string s, out MathContext context)` | Same format; returns false on failure |

| Component | Valid Values | Case Sensitivity |
|-----------|-------------|------------------|
| `<n>` | Non-negative integer | N/A |
| `<mode>` | `Up`, `Down`, `Ceiling`, `Floor`, `HalfUp`, `HalfDown`, `HalfEven`, `Unnecessary` | Case-insensitive |

## Quick Reference: Conversion Summary

| From | To | Syntax | Lossless? |
|------|----|--------|-----------|
| `int` | `BigInteger` | `BigInteger x = 42;` | Yes |
| `long` | `BigInteger` | `BigInteger x = 42L;` | Yes |
| `BigInteger` | `int` | `(int)bigInt` | No (truncates) |
| `BigInteger` | `long` | `(long)bigInt` | No (truncates) |
| `BigInteger` | `double` | `double d = bigInt;` | No (precision loss for large values) |
| `Deveel.Math.BigInteger` | `System.Numerics.BigInteger` | `var sys = deveel;` | Yes |
| `System.Numerics.BigInteger` | `Deveel.Math.BigInteger` | `var deveel = (BigInteger)sys;` | Yes |
| `int` | `BigDecimal` | `new BigDecimal(42)` | Yes |
| `long` | `BigDecimal` | `new BigDecimal(42L)` | Yes |
| `double` | `BigDecimal` | `new BigDecimal(0.1)` | **No** (use `Parse("0.1")`) |
| `string` | `BigDecimal` | `BigDecimal.Parse("0.1")` | Yes |
| `BigDecimal` | `double` | `bd.ToDouble()` | No (precision loss for large values) |

## Example Usages

### Type Conversion Chain

```csharp
// Start with a primitive
int original = 42;

// Convert to BigInteger
BigInteger bigInt = original;

// Convert to BigDecimal with scale
BigDecimal bd = new BigDecimal(bigInt, 2); // 0.42

// Convert back to double
double backToDouble = bd.ToDouble(); // 0.42

Console.WriteLine($"Original: {original}");
Console.WriteLine($"BigDecimal: {bd}");
Console.WriteLine($"Back to double: {backToDouble}");
```

### Safe Parsing with TryParse

```csharp
// BigDecimal
if (BigDecimal.TryParse("123.456", out var bd))
{
    Console.WriteLine($"Parsed: {bd}");
}

// BigInteger with radix
if (BigInteger.TryParse("FF", 16, out var bi))
{
    Console.WriteLine($"Hex FF = {bi}"); // 255
}

// MathContext
if (MathContext.TryParse("precision=5 roundingMode=HalfUp", out var mc))
{
    Console.WriteLine($"Precision: {mc.Precision}"); // 5
}
```

## String Formatting Support

### BigDecimal Formatting

`BigDecimal` implements `IFormattable` and supports format specifiers in string interpolation:

| Syntax | Output Example (`1234567.89`) |
|--------|-------------------------------|
| `$"{bd}"` or `$"{bd:G}"` | `1234567.89` |
| `$"{bd:P}"` | `1234567.89` (plain, no scientific notation) |
| `$"{bd:E}"` | `1.23456789E+6` (engineering notation) |

**Unsupported**: Standard numeric formats like `F2`, `N2`, `C` are **not** recognized and will throw `ArgumentException`.

### BigInteger Formatting

`BigInteger` does **not** implement `IFormattable`. String interpolation always produces decimal output:

| Syntax | Output Example (`255`) |
|--------|------------------------|
| `$"{bi}"` | `255` |
| `$"{bi:X}"` | `255` (format specifier ignored, not hex!) |

To get non-decimal output, use `ToString(radix)`:

| Method | Output Example (`255`) |
|--------|------------------------|
| `bi.ToString()` | `255` |
| `bi.ToString(2)` | `11111111` |
| `bi.ToString(16)` | `ff` |
| `bi.ToString(36)` | `73` |

### Formatting Comparison

```csharp
var bd = BigDecimal.Parse("1234567.89");
var bi = BigInteger.Parse("12345678901234567890");

// BigDecimal: full format support
Console.WriteLine($"BigDecimal: {bd}");       // 1234567.89
Console.WriteLine($"BigDecimal: {bd:E}");     // 1.23456789E+6

// BigInteger: decimal only in interpolation
Console.WriteLine($"BigInteger: {bi}");       // 12345678901234567890

// BigInteger: use ToString(radix) for other bases
Console.WriteLine($"BigInteger hex: {bi.ToString(16)}"); // ab5bc919d2e89402
```
