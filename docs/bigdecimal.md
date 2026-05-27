# BigDecimal

The `BigDecimal` class represents an immutable arbitrary-precision decimal number. It consists of an unscaled `BigInteger` value and a 32-bit integer scale, where the value equals `unscaledValue × 10^(-scale)`.

## Creation Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| Constructor | `new BigDecimal(int value)` | Creates from a 32-bit integer with scale 0 |
| Constructor | `new BigDecimal(long value)` | Creates from a 64-bit integer with scale 0 |
| Constructor | `new BigDecimal(double value)` | Creates from a double; **not exact** for values like 0.1 due to binary floating-point representation |
| Constructor | `new BigDecimal(BigInteger unscaledValue)` | Creates from a BigInteger with scale 0 |
| Constructor | `new BigDecimal(BigInteger unscaledValue, int scale)` | Creates with explicit unscaled value and scale |
| `Create()` | `BigDecimal.Create(long unscaledValue, int scale)` | Factory method; returns cached instances for common values |
| `Create()` | `BigDecimal.Create(long unscaledValue)` | Factory method; returns cached instances for values 0-10 |
| `Parse()` | `BigDecimal.Parse(string s)` | Parses a string representation; supports decimal and scientific notation |
| `Parse()` | `BigDecimal.Parse(string s, MathContext mc)` | Parses with rounding applied during construction |
| `Parse()` | `BigDecimal.Parse(string s, IFormatProvider provider)` | Parses with culture-specific format (e.g., comma as decimal separator) |
| `TryParse()` | `BigDecimal.TryParse(string s, out BigDecimal value)` | Safe parsing; returns false on invalid input |
| `TryParse()` | `BigDecimal.TryParse(string s, MathContext mc, out BigDecimal value)` | Safe parsing with rounding |
| `TryParse()` | `BigDecimal.TryParse(char[] chars, out BigDecimal value)` | Parses from character array |

## Constants

| Constant | Value | Description |
|----------|-------|-------------|
| `BigDecimal.Zero` | 0 | The constant zero |
| `BigDecimal.One` | 1 | The constant one |
| `BigDecimal.Ten` | 10 | The constant ten |

## Arithmetic Operators

| Operator | Signature | Description | Result Scale |
|----------|-----------|-------------|--------------|
| `+` (binary) | `operator +(BigDecimal a, BigDecimal b)` | Addition | `max(a.Scale, b.Scale)` |
| `-` (binary) | `operator -(BigDecimal a, BigDecimal b)` | Subtraction | `max(a.Scale, b.Scale)` |
| `*` | `operator *(BigDecimal a, BigDecimal b)` | Multiplication | `a.Scale + b.Scale` |
| `/` | `operator /(BigDecimal a, BigDecimal b)` | Division; **throws** if result is non-terminating | `a.Scale - b.Scale` |
| `%` | `operator %(BigDecimal a, BigDecimal b)` | Remainder: `a - (a / b).ToIntegral * b` | `max(a.Scale, b.Scale)` |
| `+` (unary) | `operator +(BigDecimal a)` | Unary plus; applies rounding via `MathContext.Unlimited` | Unchanged |
| `-` (unary) | `operator -(BigDecimal a)` | Negation | Unchanged |
| `++` | `operator ++(BigDecimal a)` | Increment by one; **note**: BigDecimal is immutable, returns new instance | Unchanged |
| `--` | `operator --(BigDecimal a)` | Decrement by one; **note**: BigDecimal is immutable, returns new instance | Unchanged |

## Comparison Operators

| Operator | Signature | Description | Scale Sensitivity |
|----------|-----------|-------------|-------------------|
| `==` | `operator ==(BigDecimal a, BigDecimal b)` | Value equality; `10.0 == 10.00` is `true` | **Ignores scale** |
| `!=` | `operator !=(BigDecimal a, BigDecimal b)` | Value inequality | **Ignores scale** |
| `>` | `operator >(BigDecimal a, BigDecimal b)` | Greater than | **Ignores scale** |
| `<` | `operator <(BigDecimal a, BigDecimal b)` | Less than | **Ignores scale** |
| `>=` | `operator >=(BigDecimal a, BigDecimal b)` | Greater than or equal | **Ignores scale** |
| `<=` | `operator <=(BigDecimal a, BigDecimal b)` | Less than or equal | **Ignores scale** |

**Note**: `Equals()` (from `IEquatable`) compares both value **and** scale, so `10.0.Equals(10.00)` is `false`. Use `==` for value-only comparison.

## Bit Shift Operators

| Operator | Signature | Description |
|----------|-----------|-------------|
| `>>` | `operator >>(BigDecimal a, int n)` | Right shift; equivalent to integer division by 2^n on the unscaled value |
| `<<` | `operator <<(BigDecimal a, int n)` | Left shift; equivalent to integer multiplication by 2^n on the unscaled value |

## Arithmetic Methods

| Method | Signature | Description | Rounding |
|--------|-----------|-------------|----------|
| `Add()` | `Add(BigDecimal value)` | Returns `this + value` | None |
| `Add()` | `Add(BigDecimal value, MathContext mc)` | Returns `this + value` rounded to precision | Applied per `mc` |
| `Subtract()` | `Subtract(BigDecimal value)` | Returns `this - value` | None |
| `Subtract()` | `Subtract(BigDecimal value, MathContext mc)` | Returns `this - value` rounded to precision | Applied per `mc` |
| `Multiply()` | `Multiply(BigDecimal value)` | Returns `this × value` | None |
| `Multiply()` | `Multiply(BigDecimal value, MathContext mc)` | Returns `this × value` rounded to precision | Applied per `mc` |
| `Divide()` | `Divide(BigDecimal divisor)` | Returns `this / divisor` | None; throws if non-terminating |
| `Divide()` | `Divide(BigDecimal divisor, RoundingMode mode)` | Returns `this / divisor` rounded to own scale | Applied per `mode` |
| `Divide()` | `Divide(BigDecimal divisor, int scale, RoundingMode mode)` | Returns `this / divisor` with explicit scale | Applied per `mode` |
| `Divide()` | `Divide(BigDecimal divisor, MathContext mc)` | Returns `this / divisor` rounded to precision | Applied per `mc` |
| `Negate()` | `Negate()` | Returns `-this` | None |
| `Negate()` | `Negate(MathContext mc)` | Returns `-this` rounded to precision | Applied per `mc` |

## Scale and Precision Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `ScaleTo()` | `ScaleTo(int newScale, RoundingMode mode)` | Changes scale to `newScale`; rounds if reducing scale, pads with zeros if increasing |
| `Precision` (property) | `int Precision { get; }` | Number of significant decimal digits in the unscaled value |
| `Scale` (property) | `int Scale { get; }` | Number of digits after the decimal point; negative means the decimal point is to the right |
| `UnscaledValue` (property) | `BigInteger UnscaledValue { get; }` | The mantissa as a BigInteger; value = `UnscaledValue × 10^(-Scale)` |
| `Sign` (property) | `int Sign { get; }` | Returns -1 (negative), 0 (zero), or 1 (positive) |

## BigMath Static Methods for BigDecimal

| Method | Signature | Description |
|--------|-----------|-------------|
| `DivideToIntegral()` | `DivideToIntegral(BigDecimal a, BigDecimal b)` | Returns integer part of division (quotient rounded toward zero) |
| `DivideToIntegral()` | `DivideToIntegral(BigDecimal a, BigDecimal b, MathContext mc)` | Returns integer part with precision limit |
| `DivideAndRemainder()` | `DivideAndRemainder(BigDecimal a, BigDecimal b, out BigDecimal remainder)` | Returns both quotient and remainder |
| `DivideAndRemainder()` | `DivideAndRemainder(BigDecimal a, BigDecimal b, MathContext mc, out BigDecimal remainder)` | Returns both with precision limit |
| `Remainder()` | `Remainder(BigDecimal a, BigDecimal b)` | Returns `a - DivideToIntegral(a, b) × b` |
| `Remainder()` | `Remainder(BigDecimal a, BigDecimal b, MathContext mc)` | Returns remainder with precision limit |
| `Pow()` | `Pow(BigDecimal number, int n)` | Returns `number^n`; scale = `number.Scale × n` |
| `Pow()` | `Pow(BigDecimal number, int n, MathContext mc)` | Returns `number^n` with precision control |
| `Abs()` | `Abs(BigDecimal number)` | Returns absolute value |
| `Abs()` | `Abs(BigDecimal number, MathContext mc)` | Returns absolute value with rounding |
| `Plus()` | `Plus(BigDecimal number)` | Returns value with rounding via `MathContext.Unlimited` |
| `Plus()` | `Plus(BigDecimal number, MathContext mc)` | Returns value rounded to precision |
| `Round()` | `Round(BigDecimal number, MathContext mc)` | Rounds to specified precision and mode |
| `Min()` | `Min(BigDecimal a, BigDecimal b)` | Returns the smaller value (by numeric comparison) |
| `Max()` | `Max(BigDecimal a, BigDecimal b)` | Returns the larger value (by numeric comparison) |
| `MovePointLeft()` | `MovePointLeft(BigDecimal number, int n)` | Moves decimal point left by `n` places (increases scale by `n`) |
| `MovePointRight()` | `MovePointRight(BigDecimal number, int n)` | Moves decimal point right by `n` places (decreases scale by `n`) |
| `ScaleByPowerOfTen()` | `ScaleByPowerOfTen(BigDecimal number, int n)` | Multiplies by 10^n by adjusting scale only (unscaled value unchanged) |
| `StripTrailingZeros()` | `StripTrailingZeros(BigDecimal value)` | Removes trailing zeros from unscaled value; reduces scale accordingly |
| `Ulp()` | `Ulp(BigDecimal value)` | Returns unit in last place: `10^(-Scale)` |

## Formatting

`BigDecimal` implements `IFormattable`, which means it supports format strings in `ToString()`, string interpolation, and `string.Format()`.

### Format Specifiers

| Specifier | Name | Description | Example (`1234567.89`) |
|-----------|------|-------------|------------------------|
| `G` (or empty) | General | Fixed-point or scientific notation depending on scale | `1234567.89` |
| `P` | Plain | Always fixed-point; never uses scientific notation | `1234567.89` |
| `E` | Engineering | Scientific notation with exponent as multiple of 3 | `1.23456789E+6` |

### Formatting Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `ToString()` | `ToString()` | Default string representation (equivalent to `ToString("G")`) |
| `ToString()` | `ToString(string format)` | Formatted string using specified format specifier |
| `ToString()` | `ToString(string format, IFormatProvider provider)` | Formatted string with culture-specific formatting |

### String Interpolation Support

| Syntax | Equivalent To | Description |
|--------|---------------|-------------|
| `$"{bd}"` | `bd.ToString()` | Default formatting (General) |
| `$"{bd:G}"` | `bd.ToString("G")` | General format |
| `$"{bd:P}"` | `bd.ToString("P")` | Plain format (no scientific notation) |
| `$"{bd:E}"` | `bd.ToString("E")` | Engineering notation |

**Note**: `BigDecimal` does **not** support standard numeric format specifiers like `F2`, `N2`, `C`, etc. Only `G`, `P`, and `E` are recognized. Passing an unrecognized format string throws `ArgumentException`.

## Example Usages

### Financial Calculation with Rounding

```csharp
var price = BigDecimal.Parse("19.99");
var quantity = new BigDecimal(3);
var taxRate = BigDecimal.Parse("0.0875");

var subtotal = price * quantity;
var tax = (subtotal * taxRate).ScaleTo(2, RoundingMode.HalfUp);
var total = subtotal + tax;

Console.WriteLine($"Subtotal: {subtotal:P}");  // 59.97
Console.WriteLine($"Tax: {tax:P}");            // 5.25
Console.WriteLine($"Total: {total:P}");        // 65.22
```

### Precise Division with MathContext

```csharp
var one = BigDecimal.One;
var three = new BigDecimal(3);

// This would throw: one / three is non-terminating
// var result = one / three;

// Use MathContext for controlled precision
var mc = new MathContext(10, RoundingMode.HalfEven);
var result = one.Divide(three, mc);

Console.WriteLine($"Result: {result}");            // 0.3333333333
Console.WriteLine($"Plain: {result:P}");           // 0.3333333333
Console.WriteLine($"Engineering: {result:E}");     // 333.3333333E-3
```

### Scale Manipulation

```csharp
var value = BigDecimal.Parse("123.4567");

var rounded = value.ScaleTo(2, RoundingMode.HalfUp);
Console.WriteLine($"Rounded: {rounded:P}"); // 123.46

var expanded = value.ScaleTo(6, RoundingMode.HalfUp);
Console.WriteLine($"Expanded: {expanded:P}"); // 123.456700

var stripped = BigMath.StripTrailingZeros(expanded);
Console.WriteLine($"Stripped: {stripped:P}"); // 123.4567
```

### Move Decimal Point

```csharp
var value = BigDecimal.Parse("123.45");

var left = BigMath.MovePointLeft(value, 2);
Console.WriteLine($"Left: {left:P}");   // 1.2345

var right = BigMath.MovePointRight(value, 2);
Console.WriteLine($"Right: {right:P}"); // 12345
```

### Formatting with String Interpolation

```csharp
var normal = BigDecimal.Parse("1234567.89");
var verySmall = BigDecimal.Parse("0.00000123");
var veryLarge = BigDecimal.Parse("123456789012345678901234567890.123");

// General format (default in interpolation)
Console.WriteLine($"Normal: {normal}");        // 1234567.89
Console.WriteLine($"Small: {verySmall}");      // 0.00000123
Console.WriteLine($"Large: {veryLarge}");      // 1.23456789012345678901234567890123E+29

// Plain format (always fixed-point, never scientific)
Console.WriteLine($"Normal (P): {normal:P}");        // 1234567.89
Console.WriteLine($"Small (P): {verySmall:P}");      // 0.00000123
Console.WriteLine($"Large (P): {veryLarge:P}");      // 123456789012345678901234567890.123

// Engineering notation (exponent is multiple of 3)
Console.WriteLine($"Normal (E): {normal:E}");        // 1.23456789E+6
Console.WriteLine($"Small (E): {verySmall:E}");      // 1.23E-6
Console.WriteLine($"Large (E): {veryLarge:E}");      // 123.456789012345678901234567890123E+27
```

### Formatting with string.Format

```csharp
var value = BigDecimal.Parse("99.95");

var general = string.Format("{0:G}", value);   // 99.95
var plain   = string.Format("{0:P}", value);   // 99.95
var eng     = string.Format("{0:E}", value);   // 99.95

// With alignment and custom text
var report = string.Format("Price: {0,10:P} USD", value);
Console.WriteLine(report); // "Price:      99.95 USD"
```

### Format Specifier Comparison

```csharp
var values = new[]
{
    BigDecimal.Parse("0.001"),
    BigDecimal.Parse("123.45"),
    BigDecimal.Parse("1234567.89"),
    BigDecimal.Parse("1.23E+20"),
};

Console.WriteLine($"{"Value",-25} {"G",-25} {"P",-35} {"E",-25}");
Console.WriteLine(new string('-', 115));

foreach (var v in values)
{
    Console.WriteLine($"{v.ToString(),-25} {v.ToString("G"),-25} {v.ToString("P"),-35} {v.ToString("E"),-25}");
}
```
