# Advanced Math Operations

This page covers the `BigMath` static class and advanced mathematical operations available in Deveel Math.

## BigMath Static Class

`BigMath` is the central hub for arithmetic operations on `BigDecimal` and `BigInteger`. While operators (`+`, `-`, `*`, `/`) provide convenient syntax, `BigMath` methods offer additional overloads with `MathContext` support and specialized operations.

## Advanced BigDecimal Operations

### Division Operations

| Method | Signature | Description | Rounding |
|--------|-----------|-------------|----------|
| `DivideToIntegral()` | `DivideToIntegral(BigDecimal a, BigDecimal b)` | Integer part of `a / b`; quotient rounded toward zero | None |
| `DivideToIntegral()` | `DivideToIntegral(BigDecimal a, BigDecimal b, MathContext mc)` | Integer part with precision limit | Applied per `mc` |
| `DivideAndRemainder()` | `DivideAndRemainder(BigDecimal a, BigDecimal b, out BigDecimal remainder)` | Returns quotient; `remainder = a - quotient × b` | None |
| `DivideAndRemainder()` | `DivideAndRemainder(BigDecimal a, BigDecimal b, MathContext mc, out BigDecimal remainder)` | Returns quotient with precision limit | Applied per `mc` |
| `Remainder()` | `Remainder(BigDecimal a, BigDecimal b)` | `a - DivideToIntegral(a, b) × b` | None |
| `Remainder()` | `Remainder(BigDecimal a, BigDecimal b, MathContext mc)` | Remainder with precision limit on division step | Applied per `mc` |

### Power Operations

| Method | Signature | Description | Scale of Result |
|--------|-----------|-------------|-----------------|
| `Pow()` | `Pow(BigDecimal number, int n)` | `number^n`; exponent must be 0 ≤ n ≤ 999,999,999 | `number.Scale × n` |
| `Pow()` | `Pow(BigDecimal number, int n, MathContext mc)` | `number^n` with precision control | Rounded per `mc` |

**Note**: `Pow(x, 0)` returns `1` for any `x`, including zero.

### Value Operations

| Method | Signature | Description |
|--------|-----------|-------------|
| `Abs()` | `Abs(BigDecimal number)` | Absolute value; returns `number` if positive, `-number` if negative |
| `Abs()` | `Abs(BigDecimal number, MathContext mc)` | Absolute value with rounding |
| `Plus()` | `Plus(BigDecimal number)` | Unary plus; applies rounding via `MathContext.Unlimited` |
| `Plus()` | `Plus(BigDecimal number, MathContext mc)` | Unary plus with explicit rounding |
| `Negate()` | `Negate(BigDecimal number)` | Negation; returns `-number` |
| `Negate()` | `Negate(BigDecimal number, MathContext mc)` | Negation with rounding |
| `Round()` | `Round(BigDecimal number, MathContext mc)` | Rounds to specified precision and mode |
| `Min()` | `Min(BigDecimal a, BigDecimal b)` | Returns smaller value (by numeric comparison, ignoring scale) |
| `Max()` | `Max(BigDecimal a, BigDecimal b)` | Returns larger value (by numeric comparison, ignoring scale) |

### Scale Manipulation

| Method | Signature | Description | Effect on Unscaled Value |
|--------|-----------|-------------|--------------------------|
| `MovePointLeft()` | `MovePointLeft(BigDecimal number, int n)` | Moves decimal point left by `n` places | Unchanged |
| `MovePointRight()` | `MovePointRight(BigDecimal number, int n)` | Moves decimal point right by `n` places | Unchanged |
| `ScaleByPowerOfTen()` | `ScaleByPowerOfTen(BigDecimal number, int n)` | Multiplies by 10^n | Unchanged (only scale changes) |
| `StripTrailingZeros()` | `StripTrailingZeros(BigDecimal value)` | Removes trailing zeros from unscaled value | Reduced (zeros removed) |
| `Ulp()` | `Ulp(BigDecimal value)` | Unit in last place: `10^(-Scale)` | N/A (returns new BigDecimal) |

### Scale Method Details

| Method | Input Scale | Output Scale | Example |
|--------|-------------|--------------|---------|
| `MovePointLeft(bd, 2)` | 2 | 4 | `123.45` → `1.2345` |
| `MovePointRight(bd, 2)` | 2 | 0 | `123.45` → `12345` |
| `ScaleByPowerOfTen(bd, 3)` | 2 | -1 | `1.23` → `1230` |
| `StripTrailingZeros(bd)` | 5 | 2 | `123.45000` → `123.45` |

**Note**: `MovePointLeft/Right` and `ScaleByPowerOfTen` do not change the unscaled value — they only adjust the scale. `StripTrailingZeros` actually modifies the unscaled value by dividing out factors of 10.

## Advanced BigInteger Operations

### Modular Arithmetic

| Method | Signature | Description | Constraints |
|--------|-----------|-------------|-------------|
| `Mod()` | `Mod(BigInteger value, BigInteger m)` | `value mod m`; always returns non-negative result in range [0, m) | `m > 0` |
| `ModInverse()` | `ModInverse(BigInteger value, BigInteger m)` | Modular multiplicative inverse: `x` such that `value × x ≡ 1 (mod m)` | `m > 0`; `gcd(value, m) = 1` |
| `ModPow()` | `ModPow(BigInteger value, BigInteger exponent, BigInteger m)` | `(value^exponent) mod m`; uses efficient modular exponentiation | `m > 0` |

### Mod vs Remainder Comparison

| Expression | `%` Operator | `Mod()` Method |
|------------|-------------|----------------|
| `7 % 5` | 2 | 2 |
| `-7 % 5` | -2 | 3 |
| `7 % -5` | 2 | Throws (`m` must be positive) |
| `-7 % -5` | -2 | Throws (`m` must be positive) |

**Key difference**: `%` preserves the sign of the dividend; `Mod()` always returns a non-negative result.

### Power and GCD

| Method | Signature | Description |
|--------|-----------|-------------|
| `Pow()` | `Pow(BigInteger value, int exp)` | `value^exp`; throws if `exp < 0` |
| `Gcd()` | `Gcd(BigInteger a, BigInteger b)` | Greatest common divisor; always non-negative |
| `Min()` | `Min(BigInteger a, BigInteger b)` | Returns the smaller value |
| `Max()` | `Max(BigInteger a, BigInteger b)` | Returns the larger value |

### Bit-Level Operations

| Method | Signature | Description |
|--------|-----------|-------------|
| `And()` | `And(BigInteger a, BigInteger b)` | Bitwise AND: `a & b` |
| `Or()` | `Or(BigInteger a, BigInteger b)` | Bitwise OR: `a \| b` |
| `XOr()` | `XOr(BigInteger a, BigInteger b)` | Bitwise XOR: `a ^ b` |
| `Not()` | `Not(BigInteger value)` | Bitwise NOT: `~value` (two's complement) |
| `AndNot()` | `AndNot(BigInteger value, BigInteger other)` | `value & ~other` |
| `ShiftLeft()` | `ShiftLeft(BigInteger value, int n)` | `value × 2^n` for n ≥ 0; `floor(value / 2^(-n))` for n < 0 |
| `ShiftRight()` | `ShiftRight(BigInteger value, int n)` | `floor(value / 2^n)` for n ≥ 0; `value × 2^(-n)` for n < 0 |

## Random Number Generation

| Constructor | Signature | Description |
|-------------|-----------|-------------|
| `BigInteger(int, Random)` | `new BigInteger(int numBits, Random rnd)` | Random non-negative integer in [0, 2^numBits - 1] |
| `BigInteger(int, int, Random)` | `new BigInteger(int bitLength, int certainty, Random rnd)` | Random probable prime; probability > 1 - 1/2^certainty |
| `ProbablePrime()` | `BigInteger.ProbablePrime(int bitLength, Random rnd)` | Static factory for random probable prime (certainty = 80) |

### Random Generation Constraints

| Constructor | Minimum `numBits` | Minimum `bitLength` | Output Range |
|-------------|-------------------|---------------------|--------------|
| `BigInteger(numBits, rnd)` | 0 | N/A | [0, 2^numBits - 1] |
| `BigInteger(bitLength, certainty, rnd)` | N/A | 2 | [2^(bitLength-1), 2^bitLength - 1] (probable prime) |

## Primality Testing

| Method | Signature | Description |
|--------|-----------|-------------|
| `IsProbablePrime()` | `IsProbablePrime(BigInteger value, int certainty)` | Returns `true` if probably prime; false if definitely composite |
| `NextProbablePrime()` | `NextProbablePrime(BigInteger value)` | Smallest prime > `value`; throws if `value < 0` |

### Certainty Levels

| Certainty | Probability of Being Prime | Use Case |
|-----------|---------------------------|----------|
| 10 | > 99.9% | Quick checks, non-critical applications |
| 50 | > 99.999999999999999999% | General use |
| 80 | > 1 - 1/2^80 | Default for `ProbablePrime()` |
| 100 | > 1 - 1/2^100 | Cryptographic applications |

## Example Usages

### DivideAndRemainder

```csharp
var a = BigDecimal.Parse("17.5");
var b = BigDecimal.Parse("3");

var quotient = BigMath.DivideAndRemainder(a, b, out var remainder);
Console.WriteLine($"Quotient: {quotient}");   // 5
Console.WriteLine($"Remainder: {remainder}"); // 2.5
```

### Modular Arithmetic

```csharp
// ModInverse: find x such that 3 * x ≡ 1 (mod 11)
var a = new BigInteger(3);
var m = new BigInteger(11);
var inverse = BigMath.ModInverse(a, m);
Console.WriteLine(inverse); // 4

// Verify: 3 * 4 = 12 ≡ 1 (mod 11)
Console.WriteLine((a * inverse) % m); // 1
```

### Modular Exponentiation

```csharp
// Compute 2^100 mod 1000 efficiently
var baseVal = new BigInteger(2);
var exp = new BigInteger(100);
var modulus = new BigInteger(1000);

var result = BigMath.ModPow(baseVal, exp, modulus);
Console.WriteLine(result); // 376
```

### GCD

```csharp
var a = new BigInteger(48);
var b = new BigInteger(18);

var gcd = BigMath.Gcd(a, b);
Console.WriteLine(gcd); // 6

// Verify: 48 = 6 * 8, 18 = 6 * 3
```

### Random Prime Generation

```csharp
var random = new Random();

// Generate a 256-bit probable prime
var prime = new BigInteger(256, 80, random);
Console.WriteLine($"Bit length: {prime.BitLength}");
Console.WriteLine($"Is probable prime: {BigInteger.IsProbablePrime(prime, 80)}");
```

### Scale Manipulation Comparison

```csharp
var value = BigDecimal.Parse("1.23");

// MovePointLeft: only changes scale
var left = BigMath.MovePointLeft(value, 2);
Console.WriteLine(left);              // 0.0123
Console.WriteLine(left.UnscaledValue); // 123 (unchanged)

// ScaleByPowerOfTen: only changes scale
var scaled = BigMath.ScaleByPowerOfTen(value, 2);
Console.WriteLine(scaled);              // 123
Console.WriteLine(scaled.UnscaledValue); // 123 (unchanged)

// Actual multiplication: changes unscaled value
var multiplied = value * BigDecimal.Parse("100");
Console.WriteLine(multiplied);              // 123.00
Console.WriteLine(multiplied.UnscaledValue); // 12300 (changed)
```
