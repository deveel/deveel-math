# MathContext & RoundingMode

This page covers the two types that control precision and rounding behavior in Deveel Math operations.

## MathContext

`MathContext` is an immutable object describing settings for numerical operations on `BigDecimal`: the number of significant digits (precision) and the rounding strategy.

### Constructors

| Constructor | Signature | Description |
|-------------|-----------|-------------|
| `MathContext(int precision)` | `new MathContext(int precision)` | Creates with given precision; uses `RoundingMode.HalfUp` as default |
| `MathContext(int precision, RoundingMode roundingMode)` | `new MathContext(int precision, RoundingMode roundingMode)` | Creates with explicit precision and rounding mode |

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Precision` | `int` | Number of significant digits to use; 0 means unlimited precision |
| `RoundingMode` | `RoundingMode` | Strategy applied when result must be rounded to fit the precision |

### Predefined Contexts

| Constant | Precision | Rounding Mode | Standard |
|----------|-----------|---------------|----------|
| `MathContext.Decimal32` | 7 | `HalfEven` | IEEE 754r single decimal precision |
| `MathContext.Decimal64` | 16 | `HalfEven` | IEEE 754r double decimal precision |
| `MathContext.Decimal128` | 34 | `HalfEven` | IEEE 754r quadruple decimal precision |
| `MathContext.Unlimited` | 0 | `HalfUp` | No precision limit; rounding mode is irrelevant |

### Parsing Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `Parse()` | `MathContext.Parse(string s)` | Parses string in format `"precision=<n> roundingMode=<mode>"`; throws on invalid input |
| `TryParse()` | `MathContext.TryParse(string s, out MathContext context)` | Safe parsing; returns false on invalid input |

### String Format

The expected format is: `"precision=<n> roundingMode=<mode>"` where:
- `<n>` is a non-negative integer
- `<mode>` is a case-insensitive `RoundingMode` enum name

```csharp
var mc = MathContext.Parse("precision=7 roundingMode=HalfEven");
```

### Equality

| Method | Description |
|--------|-------------|
| `Equals(MathContext other)` | Returns `true` if both precision and rounding mode match |
| `GetHashCode()` | Hash code derived from precision and rounding mode |

## RoundingMode

The `RoundingMode` enumeration defines strategies for reducing precision when a result has more digits than allowed.

### Rounding Mode Behaviors

| Mode | Direction | Ties (exactly 0.5) | Round(2.5) | Round(-2.5) | Round(2.4) | Round(-2.4) |
|------|-----------|-------------------|------------|-------------|------------|-------------|
| `Up` | Away from zero | Away from zero | 3 | -3 | 3 | -3 |
| `Down` | Toward zero | Toward zero | 2 | -2 | 2 | -2 |
| `Ceiling` | Toward +∞ | Toward +∞ | 3 | -2 | 3 | -2 |
| `Floor` | Toward -∞ | Toward -∞ | 2 | -3 | 2 | -3 |
| `HalfUp` | Nearest | Away from zero | 3 | -3 | 2 | -2 |
| `HalfDown` | Nearest | Toward zero | 2 | -2 | 2 | -2 |
| `HalfEven` | Nearest | To even neighbor | 2 | -2 | 2 | -2 |
| `Unnecessary` | N/A | Throws if rounding needed | Throws | Throws | 2 | -2 |

### Detailed Descriptions

| Mode | Description | Use Case |
|------|-------------|----------|
| `Up` | Always rounds away from zero. The absolute value of the result is always greater than or equal to the absolute value of the input. | Conservative estimation where you want to ensure the result is never smaller in magnitude |
| `Down` | Always rounds toward zero (truncation). The absolute value of the result is always less than or equal to the absolute value of the input. | Simple truncation; discarding fractional parts without bias |
| `Ceiling` | Rounds toward positive infinity. For positive values, behaves like `Up`; for negative values, behaves like `Down`. | When you need an upper bound on the result |
| `Floor` | Rounds toward negative infinity. For positive values, behaves like `Down`; for negative values, behaves like `Up`. | When you need a lower bound on the result |
| `HalfUp` | Rounds to the nearest neighbor. If equidistant (exactly 0.5), rounds away from zero. This is the "common" rounding taught in schools. | General-purpose rounding; familiar behavior |
| `HalfDown` | Rounds to the nearest neighbor. If equidistant (exactly 0.5), rounds toward zero. | When you want ties to break conservatively toward zero |
| `HalfEven` | Rounds to the nearest neighbor. If equidistant (exactly 0.5), rounds to the nearest even digit. Also known as "banker's rounding." | IEEE 754 default; minimizes cumulative rounding bias in repeated calculations |
| `Unnecessary` | No rounding is performed. If the result cannot be represented exactly within the specified precision, an `ArithmeticException` is thrown. | When exact results are required and any rounding is unacceptable |

### HalfEven (Banker's Rounding) Detail

`HalfEven` rounds ties to the nearest even digit, which distributes rounding bias evenly over many operations:

| Input | HalfEven Result | Reason |
|-------|-----------------|--------|
| 2.5 | 2 | 2 is even |
| 3.5 | 4 | 4 is even |
| 4.5 | 4 | 4 is even |
| 5.5 | 6 | 6 is even |
| 2.4 | 2 | Nearest (not a tie) |
| 2.6 | 3 | Nearest (not a tie) |

This is the default for IEEE 754r contexts (`Decimal32`, `Decimal64`, `Decimal128`) because it minimizes cumulative rounding errors in financial and scientific calculations.

### When Rounding is Applied

Rounding occurs in the following situations:

| Situation | Method/Operation | Description |
|-----------|-----------------|-------------|
| Division | `BigDecimal.Divide(divisor, MathContext)` | Result rounded to specified precision |
| Division | `BigDecimal.Divide(divisor, scale, RoundingMode)` | Result rounded to specified scale |
| Explicit rounding | `BigMath.Round(value, MathContext)` | Rounds value to precision |
| Construction | `new BigDecimal(double, MathContext)` | Rounds during construction from double |
| Negation | `BigDecimal.Negate(MathContext)` | Rounds negated value |
| Addition | `BigDecimal.Add(other, MathContext)` | Rounds sum |
| Subtraction | `BigDecimal.Subtract(other, MathContext)` | Rounds difference |
| Multiplication | `BigDecimal.Multiply(other, MathContext)` | Rounds product |
| Parsing | `BigDecimal.Parse(string, MathContext)` | Rounds during parsing |

## Example Usages

### Creating and Using MathContext

```csharp
var mc = new MathContext(5, RoundingMode.HalfUp);
var value = BigDecimal.Parse("123.456789");

var rounded = BigMath.Round(value, mc);
Console.WriteLine(rounded); // 123.46
```

### Division with Rounding

```csharp
var one = BigDecimal.One;
var three = new BigDecimal(3);

// Without rounding: throws ArithmeticException (non-terminating)
// var result = one / three;

// With explicit scale and mode
var result1 = one.Divide(three, 4, RoundingMode.HalfUp);
Console.WriteLine(result1); // 0.3333

// With MathContext
var mc = new MathContext(6, RoundingMode.HalfEven);
var result2 = one.Divide(three, mc);
Console.WriteLine(result2); // 0.333333
```

### Predefined Contexts

```csharp
var value = BigDecimal.Parse("123.45678901234567890");

var rounded32 = BigMath.Round(value, MathContext.Decimal32);
Console.WriteLine(rounded32); // 123.4568 (7 digits)

var rounded64 = BigMath.Round(value, MathContext.Decimal64);
Console.WriteLine(rounded64); // 123.4567890123457 (16 digits)

var rounded128 = BigMath.Round(value, MathContext.Decimal128);
Console.WriteLine(rounded128); // 123.45678901234567890 (34 digits)
```

### Comparing Rounding Modes

```csharp
var value = BigDecimal.Parse("2.5");

Console.WriteLine(value.ScaleTo(0, RoundingMode.Up));       // 3
Console.WriteLine(value.ScaleTo(0, RoundingMode.Down));     // 2
Console.WriteLine(value.ScaleTo(0, RoundingMode.Ceiling));  // 3
Console.WriteLine(value.ScaleTo(0, RoundingMode.Floor));    // 2
Console.WriteLine(value.ScaleTo(0, RoundingMode.HalfUp));   // 3
Console.WriteLine(value.ScaleTo(0, RoundingMode.HalfDown)); // 2
Console.WriteLine(value.ScaleTo(0, RoundingMode.HalfEven)); // 2
```

### Parsing MathContext

```csharp
var mc = MathContext.Parse("precision=10 roundingMode=HalfUp");
Console.WriteLine(mc.Precision);     // 10
Console.WriteLine(mc.RoundingMode);  // HalfUp
```
