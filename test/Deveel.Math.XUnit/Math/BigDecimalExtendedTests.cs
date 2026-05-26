using System;
using Xunit;

namespace Deveel.Math {
    public class BigDecimalExtendedTests {
        [Fact]
        public void ConstructorDoubleWithMathContext() {
            var mc = new MathContext(5, RoundingMode.HalfUp);
            var big = new BigDecimal(123.456789, mc);
            Assert.Equal("123.46", big.ToString());
        }

        [Fact]
        public void ConstructorBigIntegerWithMathContext_VerifyPrecision() {
            var mc = new MathContext(3, RoundingMode.Down);
            var big = new BigDecimal(BigInteger.Parse("12345908"), mc);
            Assert.Equal(3, big.Precision);
        }

        [Fact]
        public void ConstructorBigIntegerScaleWithMathContext_VerifyPrecision() {
            var mc = new MathContext(4, RoundingMode.HalfEven);
            var big = new BigDecimal(BigInteger.Parse("-789012345"), 4, mc);
            Assert.Equal(4, big.Precision);
        }

        [Fact]
        public void ConstructorIntWithMathContext_VerifyPrecision() {
            var mc = new MathContext(2, RoundingMode.Ceiling);
            var big = new BigDecimal(45678, mc);
            Assert.Equal(2, big.Precision);
        }

        [Fact]
        public void ConstructorLongWithMathContext_VerifyPrecision() {
            var mc = new MathContext(4, RoundingMode.Floor);
            var big = new BigDecimal(123456789L, mc);
            Assert.Equal(4, big.Precision);
        }

        [Fact]
        public void ConstructorWithMathContext_UnnecessaryRounding_Throws() {
            var mc = new MathContext(2, RoundingMode.Unnecessary);
            Assert.Throws<ArithmeticException>(() => new BigDecimal(12345, mc));
        }

        [Fact]
        public void ToBigIntegerExact_IntegerValue_ReturnsValue() {
            var big = new BigDecimal(BigInteger.Parse("12345"), 0);
            Assert.Equal(BigInteger.Parse("12345"), big.ToBigIntegerExact());
        }

        [Fact]
        public void ToBigIntegerExact_NegativeInteger_ReturnsValue() {
            var big = new BigDecimal(BigInteger.Parse("-12345"), 0);
            Assert.Equal(BigInteger.Parse("-12345"), big.ToBigIntegerExact());
        }

        [Fact]
        public void ToBigIntegerExact_FractionalValue_ThrowsArithmeticException() {
            var big = new BigDecimal(BigInteger.Parse("12345"), 2);
            Assert.Throws<ArithmeticException>(() => big.ToBigIntegerExact());
        }

        [Fact]
        public void ToBigIntegerExact_Zero_ReturnsZero() {
            Assert.Equal(BigInteger.Zero, BigDecimal.Zero.ToBigIntegerExact());
        }

        [Fact]
        public void ToBigIntegerExact_NegativeScale_ScalesUp() {
            var big = new BigDecimal(BigInteger.Parse("12345"), -2);
            Assert.Equal(BigInteger.Parse("1234500"), big.ToBigIntegerExact());
        }

        [Fact]
        public void ToInt64Exact_IntegerValue_ReturnsValue() {
            var big = new BigDecimal(12345L);
            Assert.Equal(12345L, big.ToInt64Exact());
        }

        [Fact]
        public void ToInt64Exact_NegativeValue_ReturnsValue() {
            var big = new BigDecimal(-12345L);
            Assert.Equal(-12345L, big.ToInt64Exact());
        }

        [Fact]
        public void ToInt64Exact_FractionalValue_ThrowsArithmeticException() {
            var big = new BigDecimal(BigInteger.Parse("12345"), 2);
            Assert.Throws<ArithmeticException>(() => big.ToInt64Exact());
        }

        [Fact]
        public void ToInt64Exact_OutOfRange_ThrowsArithmeticException() {
            var big = new BigDecimal(BigInteger.Parse("99999999999999999999"), 0);
            Assert.Throws<ArithmeticException>(() => big.ToInt64Exact());
        }

        [Fact]
        public void ToInt64Exact_Zero_ReturnsZero() {
            Assert.Equal(0L, BigDecimal.Zero.ToInt64Exact());
        }

        [Fact]
        public void ToInt32Exact_IntegerValue_ReturnsValue() {
            var big = new BigDecimal(12345);
            Assert.Equal(12345, big.ToInt32Exact());
        }

        [Fact]
        public void ToInt32Exact_FractionalValue_ThrowsArithmeticException() {
            var big = new BigDecimal(BigInteger.Parse("12345"), 2);
            Assert.Throws<ArithmeticException>(() => big.ToInt32Exact());
        }

        [Fact]
        public void ToInt32Exact_OutOfRange_ThrowsArithmeticException() {
            var big = new BigDecimal(BigInteger.Parse("99999999999999"), 0);
            Assert.Throws<ArithmeticException>(() => big.ToInt32Exact());
        }

        [Fact]
        public void ToInt32Exact_Zero_ReturnsZero() {
            Assert.Equal(0, BigDecimal.Zero.ToInt32Exact());
        }

        [Fact]
        public void ToInt16Exact_IntegerValue_ReturnsValue() {
            var big = new BigDecimal((short)32000);
            Assert.Equal((short)32000, big.ToInt16Exact());
        }

        [Fact]
        public void ToInt16Exact_OutOfRange_ThrowsArithmeticException() {
            var big = new BigDecimal(99999);
            Assert.Throws<ArithmeticException>(() => big.ToInt16Exact());
        }

        [Fact]
        public void ToByteExact_IntegerValue_ReturnsValue() {
            var big = new BigDecimal(100);
            Assert.Equal((byte)100, big.ToByteExact());
        }

        [Fact]
        public void ToByteExact_OutOfRange_ThrowsArithmeticException() {
            var big = new BigDecimal(200);
            Assert.Throws<ArithmeticException>(() => big.ToByteExact());
        }

        [Fact]
        public void ToSByteExact_IntegerValue_ReturnsValue() {
            var big = new BigDecimal(100);
            Assert.Equal((sbyte)100, big.ToSByteExact());
        }

        [Fact]
        public void ToSByteExact_OutOfRange_ThrowsArithmeticException() {
            var big = new BigDecimal(200);
            Assert.Throws<ArithmeticException>(() => big.ToSByteExact());
        }

        [Fact]
        public void ScaleTo_SameScale_ReturnsEqual() {
            var big = new BigDecimal(BigInteger.Parse("12345"), 2);
            var result = big.ScaleTo(2, RoundingMode.Unnecessary);
            Assert.Equal(big, result);
        }

        [Fact]
        public void ScaleTo_IncreaseScale_AppendsZeros() {
            var big = new BigDecimal(BigInteger.Parse("12345"), 2);
            var result = big.ScaleTo(5, RoundingMode.Unnecessary);
            Assert.Equal(5, result.Scale);
            Assert.Equal("123.45000", result.ToString());
        }

        [Fact]
        public void ScaleTo_DecreaseScale_RoundsDown() {
            var big = new BigDecimal(BigInteger.Parse("12345"), 3);
            var result = big.ScaleTo(1, RoundingMode.Down);
            Assert.Equal(1, result.Scale);
            Assert.Equal("12.3", result.ToString());
        }

        [Fact]
        public void ScaleTo_DecreaseScale_RoundsHalfUp() {
            var big = new BigDecimal(BigInteger.Parse("12355"), 2);
            var result = big.ScaleTo(1, RoundingMode.HalfUp);
            Assert.Equal(1, result.Scale);
            Assert.Equal("123.6", result.ToString());
        }

        [Fact]
        public void ScaleTo_UnnecessaryRounding_ThrowsOnFraction() {
            var big = new BigDecimal(BigInteger.Parse("12345"), 4);
            Assert.Throws<ArithmeticException>(() => big.ScaleTo(2, RoundingMode.Unnecessary));
        }

        [Fact]
        public void ScaleTo_InvalidRoundingMode_Throws() {
            var big = new BigDecimal(BigInteger.Parse("12345"), 2);
            Assert.Throws<ArgumentException>(() => big.ScaleTo(1, (RoundingMode)999));
        }

        [Fact]
        public void Negate_WithMathContext_Rounds() {
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var big = new BigDecimal(BigInteger.Parse("12345"), 0);
            var result = big.Negate(mc);
            Assert.Equal(3, result.Precision);
        }

        [Fact]
        public void Negate_WithMathContext_Precision1() {
            var mc = new MathContext(1, RoundingMode.HalfUp);
            var big = new BigDecimal(BigInteger.Parse("9876"), 0);
            var result = big.Negate(mc);
            Assert.Equal(1, result.Precision);
        }

        [Fact]
        public void UnaryPlus_ReturnsSame() {
            var big = BigDecimal.Parse("123.45");
            var result = +big;
            Assert.Equal(big, result);
        }

        [Fact]
        public void UnaryPlus_Negative_ReturnsSame() {
            var big = BigDecimal.Parse("-123.45");
            var result = +big;
            Assert.Equal(big, result);
        }

        [Fact]
        public void GreaterThanOrEqual_ComparesCorrectly() {
            var a = BigDecimal.Parse("10.5");
            var b = BigDecimal.Parse("5.5");
            Assert.True(a >= b);
            Assert.True(a >= a);
            Assert.False(b >= a);
        }

        [Fact]
        public void LessThanOrEqual_ComparesCorrectly() {
            var a = BigDecimal.Parse("5.5");
            var b = BigDecimal.Parse("10.5");
            Assert.True(a <= b);
            Assert.True(a <= a);
            Assert.False(b <= a);
        }

        [Fact]
        public void EqualValues_SatisfyBothGEAndLE() {
            var a = BigDecimal.Parse("7.0");
            var b = BigDecimal.Parse("7.00");
            Assert.True(a >= b);
            Assert.True(a <= b);
        }

        [Fact]
        public void Increment_AddsOne() {
            var big = BigDecimal.Parse("123.45");
            var result = ++big;
            Assert.Equal("124.45", result.ToString());
        }

        [Fact]
        public void Increment_Zero_ReturnsOne() {
            var big = BigDecimal.Zero;
            var result = ++big;
            Assert.Equal(BigDecimal.One, result);
        }

        [Fact]
        public void Increment_Negative_AddsOne() {
            var big = BigDecimal.Parse("-5.5");
            var result = ++big;
            Assert.Equal("-4.5", result.ToString());
        }

        [Fact]
        public void Decrement_SubtractsOne() {
            var big = BigDecimal.Parse("123.45");
            var result = --big;
            Assert.Equal("122.45", result.ToString());
        }

        [Fact]
        public void Decrement_One_ReturnsZero() {
            var big = BigDecimal.One;
            var result = --big;
            Assert.Equal(BigDecimal.Zero, result);
        }

        [Fact]
        public void Decrement_Negative_SubtractsOne() {
            var big = BigDecimal.Parse("-5.5");
            var result = --big;
            Assert.Equal("-6.5", result.ToString());
        }

        [Fact]
        public void ShiftLeft_ShiftsBigIntegerValue() {
            var big = new BigDecimal(BigInteger.Parse("123"), 0);
            var result = big << 2;
            Assert.Equal("492", result.ToString());
        }

        [Fact]
        public void ShiftRight_ShiftsBigIntegerValue() {
            var big = new BigDecimal(BigInteger.Parse("123"), 0);
            var result = big >> 2;
            Assert.Equal("30", result.ToString());
        }

        [Fact]
        public void ShiftLeft_Zero_ReturnsZero() {
            var big = BigDecimal.Zero;
            var result = big << 10;
            Assert.Equal(BigDecimal.Zero, result);
        }

        [Fact]
        public void ShiftRight_Zero_ReturnsZero() {
            var big = BigDecimal.Zero;
            var result = big >> 10;
            Assert.Equal(BigDecimal.Zero, result);
        }

        [Fact]
        public void ExplicitCastToChar_ReturnsChar() {
            var big = new BigDecimal(65);
            Assert.Equal('A', (char)big);
        }

        [Fact]
        public void ExplicitCastToSByte_ReturnsSByte() {
            var big = new BigDecimal(100);
            Assert.Equal((sbyte)100, (sbyte)big);
        }

        [Fact]
        public void ExplicitCastToByte_ReturnsByte() {
            var big = new BigDecimal(200);
            Assert.Equal((byte)200, (byte)big);
        }

        [Fact]
        public void ExplicitCastToShort_ReturnsShort() {
            var big = new BigDecimal(32000);
            Assert.Equal((short)32000, (short)big);
        }

        [Fact]
        public void ImplicitCastFromBigInteger_ReturnsBigDecimal() {
            BigInteger bi = BigInteger.Parse("12345");
            BigDecimal bd = bi;
            Assert.Equal("12345", bd.ToString());
        }

        [Fact]
        public void ExplicitCastFromLong_ReturnsBigDecimal() {
            BigDecimal bd = new BigDecimal(123456789L);
            Assert.Equal(123456789L, (long)bd);
        }

        [Fact]
        public void ExplicitCastFromInt_ReturnsBigDecimal() {
            BigDecimal bd = new BigDecimal(12345);
            Assert.Equal(12345, (int)bd);
        }

        [Fact]
        public void Parse_CharArray_ReturnsBigDecimal() {
            var chars = "123.45".ToCharArray();
            var result = BigDecimal.Parse(chars);
            Assert.Equal("123.45", result.ToString());
        }

        [Fact]
        public void Parse_CharArrayWithMathContext_Rounds() {
            var chars = "123.456".ToCharArray();
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var result = BigDecimal.Parse(chars, mc);
            Assert.Equal("123.5", result.ToString());
        }

        [Fact]
        public void TryParse_CharArrayWithOffset_ReturnsTrue() {
            var chars = " 123.45".ToCharArray();
            BigDecimal result;
            Assert.True(BigDecimal.TryParse(chars, 1, 6, out result));
            Assert.Equal("123.45", result.ToString());
        }

        [Fact]
        public void TryParse_CharArrayWithMathContext_ReturnsTrue() {
            var chars = "456.789".ToCharArray();
            var mc = new MathContext(3, RoundingMode.Down);
            BigDecimal result;
            Assert.True(BigDecimal.TryParse(chars, mc, out result));
            Assert.Equal("456", result.ToString());
        }

        [Fact]
        public void Parse_WithMathContext_Rounds() {
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigDecimal.Parse("123.456", mc);
            Assert.Equal("123", result.ToString());
        }

        [Fact]
        public void TryParse_WithMathContext_ReturnsTrue() {
            var mc = new MathContext(3, RoundingMode.Down);
            BigDecimal result;
            Assert.True(BigDecimal.TryParse("789.123", mc, out result));
            Assert.Equal("789", result.ToString());
        }

        [Fact]
        public void TryParse_WithFormatProvider_ReturnsTrue() {
            var provider = new System.Globalization.CultureInfo("en-US");
            BigDecimal result;
            Assert.True(BigDecimal.TryParse("123.45", provider, out result));
            Assert.Equal("123.45", result.ToString());
        }
    }
}
