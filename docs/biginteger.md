# BigInteger

The `BigInteger` class represents an immutable arbitrary-precision integer. It provides functionality equivalent to `System.Numerics.BigInteger` with additional features like primality testing and modular arithmetic.

## Creation Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| Constructor | `new BigInteger(int numBits, Random rnd)` | Random non-negative integer in range [0, 2^numBits - 1] |
| Constructor | `new BigInteger(int bitLength, int certainty, Random rnd)` | Random probable prime; probability of being prime > 1 - 1/2^certainty |
| Constructor | `new BigInteger(int signum, byte[] magnitude)` | From sign (-1, 0, 1) and big-endian magnitude bytes |
| Constructor | `new BigInteger(byte[] val)` | From two's complement byte array (big-endian) |
| `FromInt64()` | `BigInteger.FromInt64(long value)` | Factory method; returns cached instances for 0-10 |
| `Parse()` | `BigInteger.Parse(string s)` | Parses decimal string |
| `Parse()` | `BigInteger.Parse(string s, int radix)` | Parses string in given base (2-36) |
| `TryParse()` | `BigInteger.TryParse(string s, out BigInteger value)` | Safe decimal parsing |
| `TryParse()` | `BigInteger.TryParse(string s, int radix, out BigInteger value)` | Safe parsing with radix |

## Constants

| Constant | Value | Description |
|----------|-------|-------------|
| `BigInteger.Zero` | 0 | The constant zero |
| `BigInteger.One` | 1 | The constant one |
| `BigInteger.Ten` | 10 | The constant ten |

## Arithmetic Operators

| Operator | Signature | Description | Result |
|----------|-----------|-------------|--------|
| `+` | `operator +(BigInteger a, BigInteger b)` | Addition | `a + b` |
| `-` | `operator -(BigInteger a, BigInteger b)` | Subtraction | `a - b` |
| `*` | `operator *(BigInteger a, BigInteger b)` | Multiplication | `a × b` |
| `/` | `operator /(BigInteger a, BigInteger b)` | Integer division (truncates toward zero) | `floor(a / b)` for positive results |
| `%` | `operator %(BigInteger a, BigInteger b)` | Remainder; sign follows dividend | `a - (a / b) × b` |
| `-` (unary) | `operator -(BigInteger a)` | Negation | `-a` |
| `++` | `operator ++(BigInteger a)` | Increment by one; **note**: immutable, returns new instance | `a + 1` |
| `--` | `operator --(BigInteger a)` | Decrement by one; **note**: immutable, returns new instance | `a - 1` |

## Bitwise Operators

| Operator | Signature | Description | Example (`a=0b1010`, `b=0b1100`) |
|----------|-----------|-------------|-----------------------------------|
| `&` | `operator &(BigInteger a, BigInteger b)` | Bitwise AND | `0b1000` (8) |
| `\|` | `operator \|(BigInteger a, BigInteger b)` | Bitwise OR | `0b1110` (14) |
| `^` | `operator ^(BigInteger a, BigInteger b)` | Bitwise XOR | `0b0110` (6) |
| `~` | `operator ~(BigInteger a)` | Bitwise NOT (two's complement) | `-11` |
| `<<` | `operator <<(BigInteger a, int n)` | Left shift; `a × 2^n` | `a << 2` = 40 |
| `>>` | `operator >>(BigInteger a, int n)` | Right shift; `floor(a / 2^n)` | `a >> 2` = 2 |

## Comparison Operators

| Operator | Signature | Description |
|----------|-----------|-------------|
| `==` | `operator ==(BigInteger a, BigInteger b)` | Equality; true if same numeric value |
| `!=` | `operator !=(BigInteger a, BigInteger b)` | Inequality |
| `>` | `operator >(BigInteger a, BigInteger b)` | Greater than |
| `<` | `operator <(BigInteger a, BigInteger b)` | Less than |
| `>=` | `operator >=(BigInteger a, BigInteger b)` | Greater than or equal |
| `<=` | `operator <=(BigInteger a, BigInteger b)` | Less than or equal |

## Implicit Conversion Operators (Primitives → BigInteger)

| Source Type | Target | Syntax | Notes |
|-------------|--------|--------|-------|
| `byte` | `BigInteger` | `BigInteger x = (byte)255;` | Always succeeds |
| `sbyte` | `BigInteger` | `BigInteger x = (sbyte)-128;` | Always succeeds |
| `short` | `BigInteger` | `BigInteger x = (short)-32768;` | Always succeeds |
| `int` | `BigInteger` | `BigInteger x = 42;` | Always succeeds |
| `long` | `BigInteger` | `BigInteger x = 42L;` | Always succeeds |

## Explicit Conversion Operators (BigInteger → Primitives)

| Target Type | Syntax | Behavior on Overflow |
|-------------|--------|---------------------|
| `short` | `(short)bigInt` | Truncates to 16 bits (modulo 2^16) |
| `int` | `(int)bigInt` | Truncates to 32 bits (modulo 2^32) |
| `long` | `(long)bigInt` | Truncates to 64 bits (modulo 2^64) |

## Implicit Conversion Operators (BigInteger → Floating Point)

| Target Type | Syntax | Precision Notes |
|-------------|--------|-----------------|
| `float` | `float f = bigInt;` | ~7 significant digits; larger values lose precision |
| `double` | `double d = bigInt;` | ~15 significant digits; larger values lose precision |
| `decimal` | `decimal dec = bigInt;` | Via `ToInt64()`; throws if value exceeds `long` range |

## System.Numerics Interop Operators

| Direction | Operator | Syntax | Notes |
|-----------|----------|--------|-------|
| Deveel → System.Numerics | Implicit | `System.Numerics.BigInteger x = deveel;` | Lossless conversion |
| System.Numerics → Deveel | Explicit | `var deveel = (BigInteger)system;` | Lossless conversion |

## Arithmetic Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `Add()` | `BigMath.Add(BigInteger a, BigInteger b)` | Returns `a + b` |
| `Subtract()` | `BigMath.Subtract(BigInteger a, BigInteger b)` | Returns `a - b` |
| `Multiply()` | `BigMath.Multiply(BigInteger a, BigInteger b)` | Returns `a × b` |
| `Divide()` | `BigMath.Divide(BigInteger a, BigInteger b)` | Integer division; throws if `b == 0` |
| `Remainder()` | `BigMath.Remainder(BigInteger a, BigInteger b)` | Remainder; sign follows dividend |
| `DivideAndRemainder()` | `BigMath.DivideAndRemainder(BigInteger a, BigInteger b, out BigInteger remainder)` | Returns quotient; `remainder` output parameter |
| `Negate()` | `BigMath.Negate(BigInteger value)` | Returns `-value` |
| `Abs()` | `BigMath.Abs(BigInteger value)` | Returns absolute value |
| `Pow()` | `BigMath.Pow(BigInteger value, int exp)` | Returns `value^exp`; throws if `exp < 0` |
| `Gcd()` | `BigMath.Gcd(BigInteger a, BigInteger b)` | Greatest common divisor; always non-negative |
| `Min()` | `BigMath.Min(BigInteger a, BigInteger b)` | Returns the smaller value |
| `Max()` | `BigMath.Max(BigInteger a, BigInteger b)` | Returns the larger value |

## Modular Arithmetic Methods

| Method | Signature | Description | Constraints |
|--------|-----------|-------------|-------------|
| `Mod()` | `BigMath.Mod(BigInteger value, BigInteger m)` | `value mod m`; always returns non-negative result | `m > 0` |
| `ModInverse()` | `BigMath.ModInverse(BigInteger value, BigInteger m)` | Modular multiplicative inverse: `x` such that `value × x ≡ 1 (mod m)` | `m > 0`; `value` and `m` must be coprime |
| `ModPow()` | `BigMath.ModPow(BigInteger value, BigInteger exponent, BigInteger m)` | `(value^exponent) mod m`; efficient modular exponentiation | `m > 0` |

## Bit Operation Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `TestBit()` | `BigInteger.TestBit(BigInteger value, int n)` | Returns `true` if bit at position `n` is set; equivalent to `(value & 2^n) != 0` |
| `SetBit()` | `BigInteger.SetBit(BigInteger value, int n)` | Returns `value \| 2^n` |
| `ClearBit()` | `BigInteger.ClearBit(BigInteger value, int n)` | Returns `value & ~2^n` |
| `FlipBit()` | `BigInteger.FlipBit(BigInteger value, int n)` | Returns `value ^ 2^n` |
| `And()` | `BigMath.And(BigInteger a, BigInteger b)` | Bitwise AND |
| `Or()` | `BigMath.Or(BigInteger a, BigInteger b)` | Bitwise OR |
| `XOr()` | `BigMath.XOr(BigInteger a, BigInteger b)` | Bitwise XOR |
| `Not()` | `BigMath.Not(BigInteger value)` | Bitwise NOT |
| `AndNot()` | `BigMath.AndNot(BigInteger value, BigInteger other)` | `value & ~other` |
| `ShiftLeft()` | `BigMath.ShiftLeft(BigInteger value, int n)` | `value × 2^n` |
| `ShiftRight()` | `BigMath.ShiftRight(BigInteger value, int n)` | `floor(value / 2^n)` |

## Bit Properties

| Property | Type | Description |
|----------|------|-------------|
| `BitLength` | `int` | Number of bits in minimal two's complement representation, excluding sign bit |
| `BitCount` | `int` | Number of bits that differ from the sign bit |
| `LowestSetBit` | `int` | Index of the rightmost set bit; returns -1 if value is zero |

## Primality Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `IsProbablePrime()` | `BigInteger.IsProbablePrime(BigInteger value, int certainty)` | Returns `true` if probably prime; probability of being prime > 1 - 1/2^certainty |
| `NextProbablePrime()` | `BigInteger.NextProbablePrime(BigInteger value)` | Returns smallest prime greater than `value`; throws if `value < 0` |
| `ProbablePrime()` | `BigInteger.ProbablePrime(int bitLength, Random rnd)` | Returns random probable prime with given bit length (certainty = 80) |

## Conversion Methods

| Method | Description | Notes |
|--------|-------------|-------|
| `ToInt32()` | Returns lower 32 bits as signed int | Truncates if value exceeds int range |
| `ToInt64()` | Returns lower 64 bits as signed long | Truncates if value exceeds long range |
| `ToDouble()` | Returns value as double | May lose precision for values with >15 significant digits |
| `ToSingle()` | Returns value as float | May lose precision for values with >7 significant digits |
| `ToByteArray()` | Returns two's complement byte array (big-endian) | Most significant byte first |
| `ToString()` | Returns decimal string representation | Base 10 |
| `ToString(int radix)` | Returns string in given base (2-36) | Digits beyond 9 use lowercase a-z |
| `ToSystemBigInteger()` | Converts to `System.Numerics.BigInteger` | Lossless |
| `FromSystemBigInteger()` | Creates from `System.Numerics.BigInteger` | Lossless |

## String Representation

`BigInteger` does **not** implement `IFormattable`. It provides only `ToString()` (decimal) and `ToString(int radix)` (custom base).

### String Methods

| Method | Signature | Description | Example (`42`) |
|--------|-----------|-------------|----------------|
| `ToString()` | `ToString()` | Returns decimal string representation | `"42"` |
| `ToString(int radix)` | `ToString(int radix)` | Returns string in given base (2-36) | `ToString(2)` → `"101010"`, `ToString(16)` → `"2a"` |

### String Interpolation Support

| Syntax | Description |
|--------|-------------|
| `$"{bi}"` | Calls `ToString()` — decimal representation |
| `$"{bi:G}"` | **Not supported** — `BigInteger` does not implement `IFormattable`; format specifier is ignored |
| `$"{bi:X}"` | **Not supported** — standard hex format is not recognized; falls back to `ToString()` |

**Note**: Unlike `BigDecimal`, `BigInteger` does not support format specifiers in string interpolation. To get a hexadecimal or binary representation, use `ToString(radix)`:

```csharp
var bi = new BigInteger(255);

// String interpolation (decimal only)
Console.WriteLine($"Value: {bi}");        // "Value: 255"

// Custom radix for non-decimal output
Console.WriteLine($"Binary: {bi.ToString(2)}");   // "Binary: 11111111"
Console.WriteLine($"Hex: {bi.ToString(16)}");     // "Hex: ff"
Console.WriteLine($"Base36: {bi.ToString(36)}");  // "Base36: 73"
```

## Example Usages

### Basic Arithmetic

```csharp
var a = new BigInteger(100);
var b = new BigInteger(7);

Console.WriteLine(a + b);          // 107
Console.WriteLine(a - b);          // 93
Console.WriteLine(a * b);          // 700
Console.WriteLine(a / b);          // 14 (integer division)
Console.WriteLine(a % b);          // 2 (remainder)
```

### Modular Arithmetic

```csharp
// ModInverse: find x such that 3 * x ≡ 1 (mod 11)
var a = new BigInteger(3);
var m = new BigInteger(11);
var inverse = BigMath.ModInverse(a, m);
Console.WriteLine(inverse); // 4 (because 3 * 4 = 12 ≡ 1 mod 11)

// ModPow: compute 2^100 mod 1000 efficiently
var baseVal = new BigInteger(2);
var exp = new BigInteger(100);
var result = BigMath.ModPow(baseVal, exp, m);
Console.WriteLine(result); // 376
```

### Primality Testing

```csharp
var candidate = BigInteger.Parse("123456789012345678901234567891");

// Check if probably prime (certainty = 100)
if (BigInteger.IsProbablePrime(candidate, 100))
{
    Console.WriteLine("Probably prime");
}

// Find next prime after 100
var next = BigInteger.NextProbablePrime(new BigInteger(100));
Console.WriteLine(next); // 101

// Generate a 256-bit random prime
var random = new Random();
var prime = new BigInteger(256, 80, random);
```

### Bit Operations

```csharp
var value = new BigInteger(0b1010); // 10

Console.WriteLine(BigInteger.TestBit(value, 1)); // True (bit 1 is set)
Console.WriteLine(BigInteger.TestBit(value, 0)); // False (bit 0 is not set)

var set = BigInteger.SetBit(value, 0);
Console.WriteLine(set); // 11 (0b1011)

var cleared = BigInteger.ClearBit(value, 1);
Console.WriteLine(cleared); // 8 (0b1000)
```

### Parsing with Different Radices

```csharp
var binary = BigInteger.Parse("101010", 2);
Console.WriteLine(binary); // 42

var hex = BigInteger.Parse("2A", 16);
Console.WriteLine(hex); // 42

var base36 = BigInteger.Parse("16", 36);
Console.WriteLine(base36); // 42 (1*36 + 6)
```

### System.Numerics Interop

```csharp
// Deveel.Math -> System.Numerics
var deveel = BigInteger.Parse("12345678901234567890");
var system = deveel.ToSystemBigInteger();

// System.Numerics -> Deveel.Math
var back = BigInteger.FromSystemBigInteger(system);
Console.WriteLine(back == deveel); // True
```

### String Formatting

```csharp
var value = BigInteger.Parse("12345678901234567890");

// Default decimal representation
Console.WriteLine(value);                 // 12345678901234567890
Console.WriteLine(value.ToString());      // 12345678901234567890
Console.WriteLine($"Value: {value}");     // Value: 12345678901234567890

// Binary representation
Console.WriteLine(value.ToString(2));     // 1010101101011011110010010001100111010010111010001001010000010010

// Hexadecimal representation
Console.WriteLine(value.ToString(16));    // ab5bc919d2e89402

// Base-36 representation
Console.WriteLine(value.ToString(36));    // 2lcp67h5xg2

// Formatting in a report
Console.WriteLine($"Decimal: {value}");
Console.WriteLine($"Hex:     0x{value.ToString(16)}");
Console.WriteLine($"Binary:  0b{value.ToString(2)}");
```
