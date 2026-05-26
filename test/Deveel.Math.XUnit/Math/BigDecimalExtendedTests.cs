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

        [Fact]
        public void BigDecimal_MultiplyInstance() {
            var a = BigDecimal.Parse("10.5");
            var b = BigDecimal.Parse("2.0");
            var result = a.Multiply(b);
            Assert.Equal("21.00", result.ToString());
        }

        [Fact]
        public void BigDecimal_DivideInstance() {
            var a = BigDecimal.Parse("10.5");
            var b = BigDecimal.Parse("2.0");
            var result = a.Divide(b);
            Assert.Equal("5.25", result.ToString());
        }

        [Fact]
        public void BigDecimal_DivideWithRoundingMode() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var result = a.Divide(b, RoundingMode.HalfUp);
            Assert.Equal("3", result.ToString());
        }

        [Fact]
        public void BigDecimal_DivideWithScaleAndRoundingMode() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var result = a.Divide(b, 4, RoundingMode.HalfUp);
            Assert.Equal("3.3333", result.ToString());
        }

        [Fact]
        public void BigDecimal_DivideWithMathContext() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = a.Divide(b, mc);
            Assert.Equal("3.33", result.ToString());
        }

        [Fact]
        public void BigDecimal_OperatorMultiply() {
            var a = BigDecimal.Parse("3.5");
            var b = BigDecimal.Parse("2.0");
            Assert.Equal("7.00", (a * b).ToString());
        }

        [Fact]
        public void BigDecimal_OperatorDivide() {
            var a = BigDecimal.Parse("10.0");
            var b = BigDecimal.Parse("2.5");
            Assert.Equal("4", (a / b).ToString());
        }

        [Fact]
        public void BigDecimal_OperatorModulus() {
            var a = BigDecimal.Parse("10.0");
            var b = BigDecimal.Parse("3.0");
            Assert.Equal("1.0", (a % b).ToString());
        }

        [Fact]
        public void BigDecimal_OperatorInequality() {
            var a = BigDecimal.Parse("10.0");
            var b = BigDecimal.Parse("20.0");
            Assert.True(a != b);
            Assert.False(a != a);
        }

        [Fact]
        public void BigDecimal_ExplicitFromLong() {
            BigDecimal bd = (BigDecimal)123456789L;
            Assert.Equal(123456789L, (long)bd);
        }

        [Fact]
        public void BigDecimal_ExplicitFromInt() {
            BigDecimal bd = (BigDecimal)12345;
            Assert.Equal(12345, (int)bd);
        }

        [Fact]
        public void BigDecimal_ImplicitFromFloat() {
            BigDecimal bd = 3.14f;
            Assert.Equal(3.14f, (float)bd, 2);
        }

        [Fact]
        public void BigDecimal_ImplicitFromDouble() {
            BigDecimal bd = 3.14159;
            Assert.Equal(3.14159, (double)bd, 5);
        }

        [Fact]
        public void BigDecimal_ImplicitFromDecimal() {
            decimal d = 123.456m;
            BigDecimal bd = d;
            Assert.Equal(d, (decimal)bd);
        }

        [Fact]
        public void BigDecimal_Convertible_GetTypeCode() {
            var bd = BigDecimal.Parse("123.45");
            var conv = (System.IConvertible)bd;
            Assert.Equal(TypeCode.Object, conv.GetTypeCode());
        }

        [Fact]
        public void BigDecimal_ParseCharArray() {
            var chars = "456.78".ToCharArray();
            var result = BigDecimal.Parse(chars);
            Assert.Equal("456.78", result.ToString());
        }

        [Fact]
        public void BigDecimal_ParseCharArrayWithOffset() {
            var chars = " 789.12".ToCharArray();
            var result = BigDecimal.Parse(chars, 1, 6);
            Assert.Equal("789.12", result.ToString());
        }

        [Fact]
        public void BigDecimal_ParseCharArrayWithMathContext() {
            var chars = "123.456".ToCharArray();
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var result = BigDecimal.Parse(chars, mc);
            Assert.Equal("123.5", result.ToString());
        }

        [Fact]
        public void BigDecimal_TryParseCharArray_Fails() {
            var chars = "abc".ToCharArray();
            BigDecimal result;
            Assert.False(BigDecimal.TryParse(chars, out result));
        }

        [Fact]
        public void BigDecimal_TryParseCharArrayWithProvider() {
            var chars = "123.45".ToCharArray();
            var provider = new System.Globalization.CultureInfo("en-US");
            BigDecimal result;
            Assert.True(BigDecimal.TryParse(chars, provider, out result));
            Assert.Equal("123.45", result.ToString());
        }

        [Fact]
        public void BigDecimal_Serialization_GetObjectData() {
            var bd = BigDecimal.Parse("12345.6789");
            var info = new System.Runtime.Serialization.SerializationInfo(typeof(BigDecimal), new System.Runtime.Serialization.FormatterConverter());
            var context = new System.Runtime.Serialization.StreamingContext();
            ((System.Runtime.Serialization.ISerializable)bd).GetObjectData(info, context);
            Assert.NotNull(info);
        }

        [Fact]
        public void BigDecimal_TryParse_CharArray_MathContext() {
            var chars = "123.456".ToCharArray();
            var mc = new MathContext(3, RoundingMode.HalfUp);
            BigDecimal result;
            Assert.True(BigDecimal.TryParse(chars, mc, out result));
            Assert.Equal("123", result.ToString());
        }

        [Fact]
        public void BigDecimal_TryParse_CharArray_Offset_Length() {
            var chars = "X123.45Y".ToCharArray();
            BigDecimal result;
            Assert.True(BigDecimal.TryParse(chars, 1, 6, out result));
            Assert.Equal("123.45", result.ToString());
        }

        [Fact]
        public void BigDecimal_IConvertible_ToString() {
            var bd = BigDecimal.Parse("123.45");
            var conv = (System.IConvertible)bd;
            var result = conv.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal("123.45", result);
        }

        [Fact]
        public void BigDecimal_ToString_EngineeringString() {
            var bd = BigDecimal.Parse("12345.6789");
            var result = bd.ToString("E", System.Globalization.CultureInfo.InvariantCulture);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_ToString_PlainString() {
            var bd = BigDecimal.Parse("12345.6789");
            var result = bd.ToString("P", System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal("12345.6789", result);
        }

        [Fact]
        public void BigDecimal_ToString_PlainString_Zero() {
            var result = BigDecimal.Zero.ToString("P", System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal("0", result);
        }

        [Fact]
        public void BigDecimal_ToString_PlainString_Negative() {
            var bd = BigDecimal.Parse("-987.654");
            var result = bd.ToString("P", System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal("-987.654", result);
        }

        [Fact]
        public void BigDecimal_ToString_VariousScale() {
            var bd1 = new BigDecimal(BigInteger.FromInt64(12345), 2);
            Assert.Equal("123.45", bd1.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
            var bd2 = new BigDecimal(BigInteger.FromInt64(12345), -2);
            Assert.NotNull(bd2.ToString("G", System.Globalization.CultureInfo.InvariantCulture));
        }

        [Fact]
        public void BigDecimal_Precision_Large() {
            var bd = BigDecimal.Parse("123456789.123456789");
            var prec = bd.Precision;
            Assert.Equal(18, prec);
        }

        [Fact]
        public void BigDecimal_Precision_Zero() {
            Assert.Equal(1, BigDecimal.Zero.Precision);
        }

        [Fact]
        public void BigDecimal_ToDouble_Large() {
            var bd = BigDecimal.Parse("1e309");
            var d = bd.ToDouble();
            Assert.True(double.IsInfinity(d));
        }

        [Fact]
        public void BigDecimal_ToDouble_Small() {
            var bd = BigDecimal.Parse("1e-308");
            var d = bd.ToDouble();
            Assert.Equal(1e-308, d, 10);
        }

        [Fact]
        public void BigDecimal_Remainder_Negative() {
            var a = BigDecimal.Parse("-10.0");
            var b = BigDecimal.Parse("3.0");
            var result = a % b;
            Assert.Equal("-1.0", result.ToString());
        }

        [Fact]
        public void BigDecimal_Remainder_BothNegative() {
            var a = BigDecimal.Parse("-10.0");
            var b = BigDecimal.Parse("-3.0");
            var result = a % b;
            Assert.Equal("-1.0", result.ToString());
        }

        [Fact]
        public void BigDecimal_Remainder_ZeroDivisor() {
            var a = BigDecimal.Parse("10.0");
            var b = BigDecimal.Zero;
            Assert.Throws<ArithmeticException>(() => a % b);
        }

        [Fact]
        public void BigDecimal_Add_DifferentScales() {
            var a = BigDecimal.Parse("0.00001");
            var b = BigDecimal.Parse("100000");
            var result = BigMath.Add(a, b);
            Assert.Equal("100000.00001", result.ToString());
        }

        [Fact]
        public void BigDecimal_Add_Result_Zero() {
            var a = BigDecimal.Parse("5.0");
            var b = BigDecimal.Parse("-5.0");
            var result = BigMath.Add(a, b);
            Assert.Equal("0.0", result.ToString());
        }

        [Fact]
        public void BigDecimal_Subtract_Result_Zero() {
            var a = BigDecimal.Parse("3.14");
            var b = BigDecimal.Parse("3.14");
            var result = BigMath.Subtract(a, b);
            Assert.Equal("0.00", result.ToString());
        }

        [Fact]
        public void BigDecimal_Subtract_Negative_Result() {
            var a = BigDecimal.Parse("3.0");
            var b = BigDecimal.Parse("5.0");
            var result = BigMath.Subtract(a, b);
            Assert.Equal("-2.0", result.ToString());
        }

        [Fact]
        public void BigDecimal_Add_Through_MathContext_Edge() {
            var a = BigDecimal.Parse("0.9999");
            var b = BigDecimal.Parse("0.0001");
            var mc = new MathContext(2, RoundingMode.HalfUp);
            var result = BigMath.Add(a, b, mc);
            Assert.Equal("1.0", result.ToString());
        }

        [Fact]
        public void BigDecimal_ToString_General() {
            var bd = BigDecimal.Parse("123.456");
            var s = bd.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal("123.456", s);
        }

        [Fact]
        public void BigDecimal_ToString_Engineering_Large() {
            var bd = BigDecimal.Parse("12345.6789");
            var s = bd.ToString("E", System.Globalization.CultureInfo.InvariantCulture);
            Assert.NotNull(s);
            Assert.NotEqual("", s);
        }

        [Fact]
        public void BigDecimal_ToString_Engineering_NegativeScale() {
            var bd = new BigDecimal(BigInteger.FromInt64(123450), -3);
            var s = bd.ToString("E", System.Globalization.CultureInfo.InvariantCulture);
            Assert.NotNull(s);
        }

        [Fact]
        public void BigDecimal_ToString_Plain_Small() {
            var bd = BigDecimal.Parse("0.0000012345");
            var s = bd.ToString("P", System.Globalization.CultureInfo.InvariantCulture);
            Assert.NotNull(s);
        }

        [Fact]
        public void BigDecimal_ToString_InvalidFormat() {
            var bd = BigDecimal.Parse("1.0");
            Assert.Throws<ArgumentException>(() => bd.ToString("X", System.Globalization.CultureInfo.InvariantCulture));
        }

        [Fact]
        public void BigDecimal_Divide_Exact_DifferentScale() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Parse("0.5");
            var result = a.Divide(b);
            Assert.Equal("2", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_Integer_Result() {
            var a = BigDecimal.Parse("100");
            var b = BigDecimal.Parse("20");
            var result = a.Divide(b);
            Assert.Equal("5", result.ToString());
        }

        [Fact]
        public void BigDecimal_Negate_MathContext() {
            var bd = BigDecimal.Parse("123.456");
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var result = bd.Negate(mc);
            Assert.Equal("-123.5", result.ToString());
        }

        [Fact]
        public void BigDecimal_Negate_Zero() {
            var result = BigDecimal.Zero.Negate();
            Assert.Equal("0", result.ToString());
        }

        [Fact]
        public void BigDecimal_Negate_Positive() {
            var bd = BigDecimal.Parse("123.45");
            var result = bd.Negate();
            Assert.Equal("-123.45", result.ToString());
        }

        [Fact]
        public void BigDecimal_Negate_Negative() {
            var bd = BigDecimal.Parse("-123.45");
            var result = bd.Negate();
            Assert.Equal("123.45", result.ToString());
        }

        [Fact]
        public void BigDecimal_ToBigInteger_Exact() {
            var bd = BigDecimal.Parse("12345");
            var bi = bd.ToBigInteger();
            Assert.Equal(BigInteger.FromInt64(12345), bi);
        }

        [Fact]
        public void BigDecimal_ToBigInteger_Truncated() {
            var bd = BigDecimal.Parse("12345.678");
            var bi = bd.ToBigInteger();
            Assert.Equal(BigInteger.FromInt64(12345), bi);
        }

        [Fact]
        public void BigDecimal_ToBigIntegerExact() {
            var bd = BigDecimal.Parse("12345.000");
            var bi = bd.ToBigIntegerExact();
            Assert.Equal(BigInteger.FromInt64(12345), bi);
        }

        [Fact]
        public void BigDecimal_ToBigIntegerExact_Throws() {
            var bd = BigDecimal.Parse("12345.678");
            Assert.Throws<ArithmeticException>(() => bd.ToBigIntegerExact());
        }

        [Fact]
        public void BigDecimal_ToBigIntegerExact_Negative() {
            var bd = BigDecimal.Parse("-12345.000");
            var bi = bd.ToBigIntegerExact();
            Assert.Equal(BigInteger.FromInt64(-12345), bi);
        }

        [Fact]
        public void BigDecimal_RoundingBehavior() {
            var a = BigDecimal.Parse("1.5");
            var mc = new MathContext(1, RoundingMode.HalfUp);
            var result = BigMath.Round(a, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_RoundingBehavior_Unnecessary_NoRounding() {
            var a = BigDecimal.Parse("1.0");
            var mc = new MathContext(2, RoundingMode.Unnecessary);
            var result = BigMath.Round(a, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_RoundingBehavior_Unnecessary_Throws() {
            var a = BigDecimal.Parse("1.234");
            var mc = new MathContext(2, RoundingMode.Unnecessary);
            Assert.Throws<ArithmeticException>(() => BigMath.Round(a, mc));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToSByte_Throws() {
            var bd = BigDecimal.Parse("1.0");
            var conv = (System.IConvertible)bd;
            Assert.Throws<NotSupportedException>(() => conv.ToSByte(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToUInt16_Throws() {
            var bd = BigDecimal.Parse("1.0");
            var conv = (System.IConvertible)bd;
            Assert.Throws<NotSupportedException>(() => conv.ToUInt16(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToUInt32_Throws() {
            var bd = BigDecimal.Parse("1.0");
            var conv = (System.IConvertible)bd;
            Assert.Throws<NotSupportedException>(() => conv.ToUInt32(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToUInt64_Throws() {
            var bd = BigDecimal.Parse("1.0");
            var conv = (System.IConvertible)bd;
            Assert.Throws<NotSupportedException>(() => conv.ToUInt64(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToDateTime_Throws() {
            var bd = BigDecimal.Parse("1.0");
            var conv = (System.IConvertible)bd;
            Assert.Throws<NotSupportedException>(() => conv.ToDateTime(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToBoolean_True() {
            var bd = BigDecimal.Parse("1.0");
            var conv = (System.IConvertible)bd;
            Assert.True(conv.ToBoolean(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToBoolean_False() {
            var bd = BigDecimal.Parse("0.0");
            var conv = (System.IConvertible)bd;
            Assert.False(conv.ToBoolean(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToChar() {
            var bd = BigDecimal.Parse("65");
            var conv = (System.IConvertible)bd;
            Assert.Equal('A', conv.ToChar(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToByte() {
            var bd = BigDecimal.Parse("42");
            var conv = (System.IConvertible)bd;
            Assert.Equal((byte)42, conv.ToByte(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToInt16() {
            var bd = BigDecimal.Parse("12345");
            var conv = (System.IConvertible)bd;
            Assert.Equal((short)12345, conv.ToInt16(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToInt32() {
            var bd = BigDecimal.Parse("1234567890");
            var conv = (System.IConvertible)bd;
            Assert.Equal(1234567890, conv.ToInt32(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToInt64() {
            var bd = BigDecimal.Parse("1234567890123456789");
            var conv = (System.IConvertible)bd;
            Assert.Equal(1234567890123456789L, conv.ToInt64(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToSingle() {
            var bd = BigDecimal.Parse("3.14");
            var conv = (System.IConvertible)bd;
            Assert.Equal(3.14f, conv.ToSingle(null), 1);
        }

        [Fact]
        public void BigDecimal_IConvertible_ToDouble() {
            var bd = BigDecimal.Parse("3.14159");
            var conv = (System.IConvertible)bd;
            Assert.Equal(3.14159, conv.ToDouble(null), 4);
        }

        [Fact]
        public void BigDecimal_IConvertible_ToDecimal() {
            var bd = BigDecimal.Parse("123.456");
            var conv = (System.IConvertible)bd;
            Assert.Equal(123.456m, conv.ToDecimal(null));
        }

        [Fact]
        public void BigDecimal_IConvertible_ToString_WithProvider() {
            var bd = BigDecimal.Parse("123.45");
            var conv = (System.IConvertible)bd;
            var result = conv.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal("123.45", result);
        }

        [Fact]
        public void BigDecimal_IConvertible_ToType_ReturnsBigInteger() {
            var bd = BigDecimal.Parse("12345");
            var conv = (System.IConvertible)bd;
            var result = conv.ToType(typeof(BigInteger), null);
            Assert.IsType<BigInteger>(result);
        }

        [Fact]
        public void BigDecimal_IConvertible_ToType_Throws() {
            var bd = BigDecimal.Parse("12345");
            var conv = (System.IConvertible)bd;
            Assert.Throws<NotSupportedException>(() => conv.ToType(typeof(double), null));
        }

        [Fact]
        public void BigDecimal_TryParse_CharArray_MC_Provider() {
            var chars = "123.456".ToCharArray();
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var provider = System.Globalization.CultureInfo.InvariantCulture;
            BigDecimal result;
            Assert.True(BigDecimal.TryParse(chars, mc, provider, out result));
            Assert.Equal("123.5", result.ToString());
        }

        [Fact]
        public void BigDecimal_Parse_CharArray_MC_Provider() {
            var chars = "789.012".ToCharArray();
            var mc = new MathContext(5, RoundingMode.HalfUp);
            var provider = System.Globalization.CultureInfo.InvariantCulture;
            var result = BigDecimal.Parse(chars, mc, provider);
            Assert.Equal("789.01", result.ToString());
        }

        [Fact]
        public void BigDecimal_Parse_CharArray_Provider() {
            var chars = "456.789".ToCharArray();
            var provider = System.Globalization.CultureInfo.InvariantCulture;
            var result = BigDecimal.Parse(chars, provider);
            Assert.Equal("456.789", result.ToString());
        }

        [Fact]
        public void BigDecimal_Parse_CharArray_MC() {
            var chars = "123.456".ToCharArray();
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var result = BigDecimal.Parse(chars, mc);
            Assert.Equal("123.5", result.ToString());
        }

        [Fact]
        public void BigDecimal_TryParse_CharArray_Offset_Length_MC() {
            var chars = "XX123.456YY".ToCharArray();
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var provider = System.Globalization.CultureInfo.InvariantCulture;
            BigDecimal result;
            Assert.True(BigDecimal.TryParse(chars, 2, 7, mc, provider, out result));
            Assert.Equal("123.5", result.ToString());
        }

        [Fact]
        public void BigDecimal_TryParse_CharArray_Offset_Length_Provider() {
            var chars = "XX123.456YY".ToCharArray();
            var provider = System.Globalization.CultureInfo.InvariantCulture;
            BigDecimal result;
            Assert.True(BigDecimal.TryParse(chars, 2, 7, provider, out result));
            Assert.Equal("123.456", result.ToString());
        }

        [Fact]
        public void BigDecimal_Constructor_Double_MC() {
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var bd = new BigDecimal(3.14159, mc);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_BigInteger_MC() {
            var unscaled = BigInteger.FromInt64(123456);
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var bd = new BigDecimal(unscaled, mc);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_Int_MC() {
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var bd = new BigDecimal(12345, mc);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_Long_MC() {
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var bd = new BigDecimal(123456789L, mc);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_BigInteger_Scale_MC() {
            var unscaled = BigInteger.FromInt64(123456);
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var bd = new BigDecimal(unscaled, 2, mc);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_InplaceRound_ThroughRound() {
            var bd = BigDecimal.Parse("123.456");
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var result = BigMath.Round(bd, mc);
            Assert.Equal("123.5", result.ToString());
        }

        [Fact]
        public void BigDecimal_Equals_DifferentScale() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Parse("1.00");
            Assert.False(a.Equals(b));
        }

        [Fact]
        public void BigDecimal_Equals_SameValue() {
            var a = BigDecimal.Parse("1.00");
            var b = BigDecimal.Parse("1.00");
            Assert.True(a.Equals(b));
        }

        [Fact]
        public void BigDecimal_GetHashCode_SameValue() {
            var a = BigDecimal.Parse("123.45");
            var b = BigDecimal.Parse("123.45");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void BigDecimal_CompareTo_DifferentScale() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Parse("1.00");
            Assert.Equal(0, a.CompareTo(b));
        }

        [Fact]
        public void BigDecimal_OperatorUnaryPlus() {
            var a = BigDecimal.Parse("-123.45");
            var result = +a;
            Assert.Equal("-123.45", result.ToString());
        }

        [Fact]
        public void BigDecimal_OperatorUnaryMinus() {
            var a = BigDecimal.Parse("123.45");
            var result = -a;
            Assert.Equal("-123.45", result.ToString());
        }

        [Fact]
        public void BigDecimal_OperatorIncrement() {
            var a = BigDecimal.Parse("1.0");
            a++;
            Assert.Equal("2.0", a.ToString());
        }

        [Fact]
        public void BigDecimal_OperatorDecrement() {
            var a = BigDecimal.Parse("2.0");
            a--;
            Assert.Equal("1.0", a.ToString());
        }

        [Fact]
        public void BigDecimal_OperatorEquality() {
            var a = BigDecimal.Parse("1.00");
            var b = BigDecimal.Parse("1.00");
            Assert.True(a == b);
        }

        [Fact]
        public void BigDecimal_OperatorInequality_DifferentValues() {
            var a = BigDecimal.Parse("1.00");
            var b = BigDecimal.Parse("2.00");
            Assert.True(a != b);
        }

        [Fact]
        public void BigDecimal_OperatorLessThan() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Parse("2.0");
            Assert.True(a < b);
        }

        [Fact]
        public void BigDecimal_OperatorGreaterThan() {
            var a = BigDecimal.Parse("2.0");
            var b = BigDecimal.Parse("1.0");
            Assert.True(a > b);
        }

        [Fact]
        public void BigDecimal_OperatorLessThanOrEqual() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Parse("1.0");
            Assert.True(a <= b);
        }

        [Fact]
        public void BigDecimal_OperatorGreaterThanOrEqual() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Parse("1.0");
            Assert.True(a >= b);
        }

        [Fact]
        public void BigDecimal_Constructor_Double_Positive() {
            var bd = new BigDecimal(3.14159);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_Double_Negative() {
            var bd = new BigDecimal(-3.14159);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_Double_Zero() {
            var bd = new BigDecimal(0.0);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_Double_NaN_Throws() {
            Assert.Throws<FormatException>(() => new BigDecimal(double.NaN));
        }

        [Fact]
        public void BigDecimal_Constructor_Double_Infinity_Throws() {
            Assert.Throws<FormatException>(() => new BigDecimal(double.PositiveInfinity));
        }

        [Fact]
        public void BigDecimal_Constructor_Double_NegativeInfinity_Throws() {
            Assert.Throws<FormatException>(() => new BigDecimal(double.NegativeInfinity));
        }

        [Fact]
        public void BigDecimal_Constructor_BigInteger_Positive() {
            var unscaled = BigInteger.FromInt64(123456);
            var bd = new BigDecimal(unscaled);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_BigInteger_Negative() {
            var unscaled = BigInteger.FromInt64(-123456);
            var bd = new BigDecimal(unscaled);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_BigInteger_Scale() {
            var unscaled = BigInteger.FromInt64(123456);
            var bd = new BigDecimal(unscaled, 2);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_BigInteger_Scale_Negative() {
            var unscaled = BigInteger.FromInt64(12345);
            var bd = new BigDecimal(unscaled, -3);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Constructor_BigInteger_Scale_MC_ZeroPrecision() {
            var unscaled = BigInteger.FromInt64(123456789);
            var mc = new MathContext(0, RoundingMode.HalfUp);
            var bd = new BigDecimal(unscaled, 2, mc);
            Assert.NotNull(bd);
        }

        [Fact]
        public void BigDecimal_Parse_String_Provider() {
            var provider = System.Globalization.CultureInfo.InvariantCulture;
            var result = BigDecimal.Parse("123.456", provider);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Parse_String_MC_Provider() {
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var provider = System.Globalization.CultureInfo.InvariantCulture;
            var result = BigDecimal.Parse("123.456", mc, provider);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_TryParse_String_Provider() {
            var provider = System.Globalization.CultureInfo.InvariantCulture;
            BigDecimal result;
            Assert.True(BigDecimal.TryParse("123.456", provider, out result));
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_TryParse_String_MC_Provider() {
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var provider = System.Globalization.CultureInfo.InvariantCulture;
            BigDecimal result;
            Assert.True(BigDecimal.TryParse("123.456", mc, provider, out result));
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_TryParse_ScientificNotation() {
            BigDecimal result;
            Assert.True(BigDecimal.TryParse("1.23E+10", out result));
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_TryParse_NegativeScientificNotation() {
            BigDecimal result;
            Assert.True(BigDecimal.TryParse("-1.23E+10", out result));
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Parse_ScientificNotation() {
            var result = BigDecimal.Parse("1.23E+10");
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Parse_CharArray_LeadingZeros() {
            var chars = "00123.45".ToCharArray();
            var result = BigDecimal.Parse(chars);
            Assert.Equal("123.45", result.ToString());
        }

        [Fact]
        public void BigDecimal_Parse_CharArray_Negative() {
            var chars = "-123.45".ToCharArray();
            var result = BigDecimal.Parse(chars);
            Assert.Equal("-123.45", result.ToString());
        }

        [Fact]
        public void BigDecimal_Parse_CharArray_ScientificNotation() {
            var chars = "1.23E+10".ToCharArray();
            var result = BigDecimal.Parse(chars);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_TryParse_CharArray_Invalid() {
            var chars = "abc".ToCharArray();
            BigDecimal result;
            Assert.False(BigDecimal.TryParse(chars, out result));
        }

        [Fact]
        public void BigDecimal_TryParse_CharArray_Empty() {
            var chars = "".ToCharArray();
            BigDecimal result;
            Assert.False(BigDecimal.TryParse(chars, out result));
        }

        [Fact]
        public void BigDecimal_ToString_LargeNumber() {
            var bd = BigDecimal.Parse("123456789012345678901234567890.12345678901234567890");
            var s = bd.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
            Assert.False(string.IsNullOrEmpty(s));
        }

        [Fact]
        public void BigDecimal_ToString_VeryLargeScale() {
            var unscaled = BigInteger.Parse("123456789012345678901234567890");
            var bd = new BigDecimal(unscaled, 50);
            var s = bd.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
            Assert.False(string.IsNullOrEmpty(s));
        }

        [Fact]
        public void BigDecimal_ToString_NegativeLarge() {
            var bd = BigDecimal.Parse("-123456789012345678901234567890.12345678901234567890");
            var s = bd.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
            Assert.False(string.IsNullOrEmpty(s));
        }
    }
}
