using System;
using Xunit;

namespace Deveel.Math {
    public class BigDecimalMathTests {
        [Fact]
        public void BigDecimal_AddWithMathContext() {
            var a = BigDecimal.Parse("123.456");
            var b = BigDecimal.Parse("78.901");
            var mc = new MathContext(5, RoundingMode.HalfUp);
            var result = BigMath.Add(a, b, mc);
            Assert.Equal("202.36", result.ToString());
        }

        [Fact]
        public void BigDecimal_SubtractWithMathContext() {
            var a = BigDecimal.Parse("123.456");
            var b = BigDecimal.Parse("78.901");
            var mc = new MathContext(5, RoundingMode.HalfUp);
            var result = BigMath.Subtract(a, b, mc);
            Assert.Equal("44.555", result.ToString());
        }

        [Fact]
        public void BigDecimal_MovePointLeft() {
            var bd = BigDecimal.Parse("123.45");
            var result = BigMath.MovePointLeft(bd, 2);
            Assert.Equal("1.2345", result.ToString());
        }

        [Fact]
        public void BigDecimal_MovePointRight() {
            var bd = BigDecimal.Parse("1.2345");
            var result = BigMath.MovePointRight(bd, 2);
            Assert.Equal("123.45", result.ToString());
        }

        [Fact]
        public void BigDecimal_ScaleByPowerOfTen() {
            var bd = BigDecimal.Parse("1.2345");
            var result = BigMath.ScaleByPowerOfTen(bd, 2);
            Assert.Equal("123.45", result.ToString());
        }

        [Fact]
        public void BigDecimalMath_Add_MathContext_DifferentScale() {
            var a = BigDecimal.Parse("0.00123");
            var b = BigDecimal.Parse("45.6");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigMath.Add(a, b, mc);
            Assert.Equal("45.6", result.ToString());
        }

        [Fact]
        public void BigDecimalMath_Add_MathContext_ResultRounding() {
            var a = BigDecimal.Parse("1.23456");
            var b = BigDecimal.Parse("2.34567");
            var mc = new MathContext(3, RoundingMode.HalfEven);
            var result = BigMath.Add(a, b, mc);
            Assert.Equal("3.58", result.ToString());
        }

        [Fact]
        public void BigDecimalMath_Subtract_MathContext_DifferentScale() {
            var a = BigDecimal.Parse("100.0");
            var b = BigDecimal.Parse("0.001");
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var result = BigMath.Subtract(a, b, mc);
            Assert.Equal("100.0", result.ToString());
        }

        [Fact]
        public void BigDecimalMath_Subtract_MathContext_ResultRounding() {
            var a = BigDecimal.Parse("3.45678");
            var b = BigDecimal.Parse("1.23456");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigMath.Subtract(a, b, mc);
            Assert.Equal("2.22", result.ToString());
        }

        [Fact]
        public void BigMath_Multiply_WithMathContext() {
            var a = BigDecimal.Parse("1.2345");
            var b = BigDecimal.Parse("2.3456");
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var result = BigMath.Multiply(a, b, mc);
            Assert.Equal("2.896", result.ToString());
        }

        [Fact]
        public void BigMath_Divide_WithMathContext() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var mc = new MathContext(4, RoundingMode.HalfUp);
            var result = BigMath.Divide(a, b, mc);
            Assert.Equal("3.333", result.ToString());
        }

        [Fact]
        public void BigMath_DivideToIntegral() {
            var a = BigDecimal.Parse("10.5");
            var b = BigDecimal.Parse("3.0");
            var result = BigMath.DivideToIntegral(a, b);
            Assert.Equal("3", result.ToString());
        }

        [Fact]
        public void BigMath_DivideToIntegral_Exact() {
            var a = BigDecimal.Parse("9.0");
            var b = BigDecimal.Parse("3.0");
            var result = BigMath.DivideToIntegral(a, b);
            Assert.Equal("3", result.ToString());
        }

        [Fact]
        public void BigMath_DivideToIntegral_Negative() {
            var a = BigDecimal.Parse("-10.5");
            var b = BigDecimal.Parse("3.0");
            var result = BigMath.DivideToIntegral(a, b);
            Assert.Equal("-3", result.ToString());
        }

        [Fact]
        public void BigMath_DivideToIntegral_WithMC() {
            var a = BigDecimal.Parse("10.5");
            var b = BigDecimal.Parse("3.0");
            var mc = new MathContext(5, RoundingMode.Down);
            var result = BigMath.DivideToIntegral(a, b);
            Assert.Equal("3", result.ToString());
        }

        [Fact]
        public void BigMath_Pow_BigDecimal_Zero() {
            var result = BigMath.Pow(BigDecimal.Parse("10"), 0);
            Assert.Equal("1", result.ToString());
        }

        [Fact]
        public void BigMath_Pow_BigDecimal_One() {
            var result = BigMath.Pow(BigDecimal.Parse("2.5"), 1);
            Assert.Equal("2.5", result.ToString());
        }

        [Fact]
        public void BigMath_Pow_BigDecimal_Small() {
            var result = BigMath.Pow(BigDecimal.Parse("2"), 3);
            Assert.Equal("8", result.ToString());
        }

        [Fact]
        public void BigMath_Min_BigDecimal() {
            var a = BigDecimal.Parse("10.5");
            var b = BigDecimal.Parse("20.5");
            var result = BigMath.Min(a, b);
            Assert.Equal("10.5", result.ToString());
            Assert.Equal(a, BigMath.Min(b, a));
        }

        [Fact]
        public void BigMath_Max_BigDecimal() {
            var a = BigDecimal.Parse("10.5");
            var b = BigDecimal.Parse("20.5");
            Assert.Equal("20.5", BigMath.Max(a, b).ToString());
        }

        [Fact]
        public void BigMath_Negate_BigDecimal() {
            var bd = BigDecimal.Parse("123.45");
#pragma warning disable CS0618
            var result = BigMath.Negate(bd);
#pragma warning restore CS0618
            Assert.Equal("-123.45", result.ToString());
        }

        [Fact]
        public void BigMath_Negate_BigDecimal_WithMC() {
            var bd = BigDecimal.Parse("123.456");
            var mc = new MathContext(3, RoundingMode.HalfUp);
#pragma warning disable CS0618
            var result = BigMath.Negate(bd, mc);
#pragma warning restore CS0618
            Assert.Equal("-123", result.ToString());
        }

        [Fact]
        public void BigDecimal_Round_Up() {
            var bd = BigDecimal.Parse("1.2345");
            var mc = new MathContext(3, RoundingMode.Up);
            var result = BigMath.Round(bd, mc);
            Assert.Equal("1.24", result.ToString());
        }

        [Fact]
        public void BigDecimal_Round_Down() {
            var bd = BigDecimal.Parse("1.2345");
            var mc = new MathContext(3, RoundingMode.Down);
            var result = BigMath.Round(bd, mc);
            Assert.Equal("1.23", result.ToString());
        }

        [Fact]
        public void BigDecimal_Round_Ceiling() {
            var bd = BigDecimal.Parse("1.2345");
            var mc = new MathContext(3, RoundingMode.Ceiling);
            var result = BigMath.Round(bd, mc);
            Assert.Equal("1.24", result.ToString());
        }

        [Fact]
        public void BigDecimal_Round_Floor() {
            var bd = BigDecimal.Parse("1.2345");
            var mc = new MathContext(3, RoundingMode.Floor);
            var result = BigMath.Round(bd, mc);
            Assert.Equal("1.23", result.ToString());
        }

        [Fact]
        public void BigDecimal_DivideByTwo() {
            var a = BigDecimal.Parse("10.5");
            var result = BigMath.Divide(a, BigDecimal.Parse("2"));
            Assert.Equal("5.25", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_RoundingUp() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var result = BigMath.Divide(a, b, RoundingMode.Up);
            Assert.Equal("4", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_RoundingCeiling() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var result = BigMath.Divide(a, b, RoundingMode.Ceiling);
            Assert.Equal("4", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_RoundingFloor() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var result = BigMath.Divide(a, b, RoundingMode.Floor);
            Assert.Equal("3", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_RoundingDown() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var result = BigMath.Divide(a, b, RoundingMode.Down);
            Assert.Equal("3", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_RoundingHalfEven() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var result = BigMath.Divide(a, b, 3, RoundingMode.HalfEven);
            Assert.Equal("3.333", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_RoundingUnnecessary() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("2");
            var result = BigMath.Divide(a, b, RoundingMode.Unnecessary);
            Assert.Equal("5", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_ZeroDividend() {
            var a = BigDecimal.Zero;
            var b = BigDecimal.Parse("5.0");
            var result = BigMath.Divide(a, b);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Divide_DivisorHasFactorOf5() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Parse("0.5");
            var result = BigMath.Divide(a, b);
            Assert.Equal("2", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_DivisorHasMultipleFactorsOf5() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Parse("0.2");
            var result = BigMath.Divide(a, b);
            Assert.Equal("5", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_DivisorHasFactorOf25() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Parse("0.25");
            var result = BigMath.Divide(a, b);
            Assert.Equal("4", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_NegativeDivisor() {
            var a = BigDecimal.Parse("10.0");
            var b = BigDecimal.Parse("-2.0");
            var result = BigMath.Divide(a, b);
            Assert.Equal("-5", result.ToString());
        }

        [Fact]
        public void BigDecimal_Divide_MC_ZeroPrecision() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var mc = new MathContext(0, RoundingMode.HalfUp);
            Assert.Throws<ArithmeticException>(() => BigMath.Divide(a, b, mc));
        }

        [Fact]
        public void BigDecimal_Divide_MC_ZeroDividend() {
            var a = BigDecimal.Zero;
            var b = BigDecimal.Parse("5.0");
            var mc = new MathContext(5, RoundingMode.HalfUp);
            var result = BigMath.Divide(a, b, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Divide_MC_ZeroDivisor_Throws() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Zero;
            var mc = new MathContext(5, RoundingMode.HalfUp);
            Assert.Throws<ArithmeticException>(() => BigMath.Divide(a, b, mc));
        }

        [Fact]
        public void BigDecimal_Divide_MC_DiffScalePositive() {
            var a = BigDecimal.Parse("1.000");
            var b = BigDecimal.Parse("0.5");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigMath.Divide(a, b, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Divide_MC_DiffScaleNegative() {
            var a = BigDecimal.Parse("1");
            var b = BigDecimal.Parse("0.0005");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigMath.Divide(a, b, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_DivideToIntegral_PositiveScale() {
            var a = BigDecimal.Parse("100");
            var b = BigDecimal.Parse("0.5");
            var result = BigMath.DivideToIntegral(a, b);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_DivideToIntegral_NegativeScale() {
            var a = BigDecimal.Parse("0.5");
            var b = BigDecimal.Parse("100");
            var result = BigMath.DivideToIntegral(a, b);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_DivideToIntegral_MC_ZeroPrecision() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var mc = new MathContext(0, RoundingMode.HalfUp);
            var result = BigMath.DivideToIntegral(a, b, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Pow_NegativeExponent() {
            var a = BigDecimal.Parse("2.0");
            Assert.Throws<ArithmeticException>(() => BigMath.Pow(a, -1));
        }

        [Fact]
        public void BigDecimal_Pow_MC_NegativeExponent() {
            var a = BigDecimal.Parse("2.0");
            var mc = new MathContext(5, RoundingMode.HalfUp);
            var result = BigMath.Pow(a, -2, mc);
            Assert.Equal("0.25", result.ToString());
        }

        [Fact]
        public void BigDecimal_Pow_MC_ZeroExponent() {
            var a = BigDecimal.Parse("2.0");
            var mc = new MathContext(5, RoundingMode.HalfUp);
            var result = BigMath.Pow(a, 0, mc);
            Assert.Equal("1", result.ToString());
        }

        [Fact]
        public void BigDecimal_MovePointLeft_Zero() {
            var result = BigMath.MovePointLeft(BigDecimal.Zero, 5);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_MovePointRight_Zero() {
            var result = BigMath.MovePointRight(BigDecimal.Zero, 5);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_MovePointLeft_Large() {
            var a = BigDecimal.Parse("12345678901234567890.1234567890");
            var result = BigMath.MovePointLeft(a, 5);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Add_ZeroBoth() {
            var result = BigMath.Add(BigDecimal.Zero, BigDecimal.Zero);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Add_ZeroFirst_PositiveScaleDiff() {
            var a = BigDecimal.Zero;
            var b = BigDecimal.Parse("0.001");
            var result = BigMath.Add(a, b);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Add_ZeroSecond_PositiveScaleDiff() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Zero;
            var result = BigMath.Add(a, b);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Add_MC_ZeroAugend() {
            var a = BigDecimal.Parse("1.2345");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigMath.Add(a, BigDecimal.Zero, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Add_MC_Optimization_SameSign() {
            var a = BigDecimal.Parse("1e50");
            var b = BigDecimal.Parse("1e-50");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigMath.Add(a, b, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Add_MC_Optimization_DiffSign() {
            var a = BigDecimal.Parse("1e50");
            var b = BigDecimal.Parse("-1e-50");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigMath.Add(a, b, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Subtract_NullThrows() {
            var a = BigDecimal.Parse("1.0");
            Assert.Throws<ArgumentNullException>(() => BigMath.Subtract(a, null));
        }

        [Fact]
        public void BigDecimal_Subtract_ZeroFirst() {
            var result = BigMath.Subtract(BigDecimal.Zero, BigDecimal.Parse("5.0"));
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Subtract_ZeroBoth() {
            var result = BigMath.Subtract(BigDecimal.Zero, BigDecimal.Zero);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Subtract_ZeroSecond_PositiveScaleDiff() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Zero;
            var result = BigMath.Subtract(a, b);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Subtract_MC_NullThrows() {
            var a = BigDecimal.Parse("1.0");
            var b = BigDecimal.Parse("0.5");
            Assert.Throws<ArgumentNullException>(() => BigMath.Subtract(a, b, null));
        }

        [Fact]
        public void BigDecimal_Subtract_MC_ZeroSubtrahend() {
            var a = BigDecimal.Parse("1.2345");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigMath.Subtract(a, BigDecimal.Zero, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Scale_IncreaseScale_Large() {
            var a = BigDecimal.Parse("12345678901234567890");
            var result = BigMath.Scale(a, 5, RoundingMode.HalfUp);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Scale_DecreaseScale_Small() {
            var a = BigDecimal.Parse("123.456");
            var result = BigMath.Scale(a, 0, RoundingMode.HalfUp);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Scale_DecreaseScale_Large() {
            var a = BigDecimal.Parse("12345678901234567890.12345");
            var result = BigMath.Scale(a, 0, RoundingMode.HalfUp);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Scale_LargeDecrease() {
            var a = BigDecimal.Parse("12345678901234567890.1234567890");
            var result = BigMath.Scale(a, -100, RoundingMode.HalfUp);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Divide_ScaleAndRounding_DiffScaleNonZero() {
            var a = BigDecimal.Parse("10");
            var b = BigDecimal.Parse("3");
            var result = BigMath.Divide(a, b, 5, RoundingMode.HalfUp);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Divide_MC_QuotientPrecisionZero() {
            var a = BigDecimal.Parse("0.001");
            var b = BigDecimal.Parse("1000");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigMath.Divide(a, b, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Divide_MC_SameScale() {
            var a = BigDecimal.Parse("10.0");
            var b = BigDecimal.Parse("3.0");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            var result = BigMath.Divide(a, b, mc);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_DivideToIntegral_MC_ResultTooLarge_Throws() {
            var a = BigDecimal.Parse("12345");
            var b = BigDecimal.Parse("1");
            var mc = new MathContext(3, RoundingMode.HalfUp);
            Assert.Throws<ArithmeticException>(() => BigMath.DivideToIntegral(a, b, mc));
        }

        [Fact]
        public void BigDecimal_Divide_PrimitiveLongs() {
            var a = BigDecimal.Create(123456L, 3);
            var b = BigDecimal.Create(789L, 1);
            var result = BigMath.Divide(a, b, 2, RoundingMode.HalfUp);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Divide_PrimitiveLongs_SameScale() {
            var a = BigDecimal.Create(123456L, 3);
            var b = BigDecimal.Create(789L, 3);
            var result = BigMath.Divide(a, b, 0, RoundingMode.HalfUp);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Pow_LargeExponent() {
            var a = BigDecimal.Parse("10");
            var result = BigMath.Pow(a, 100);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Pow_LargeExponent_Negative() {
            var a = BigDecimal.Parse("-10");
            var result = BigMath.Pow(a, 100);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigDecimal_Multiply_MC() {
            var a = BigDecimal.Parse("12345678901234567890");
            var b = BigDecimal.Parse("9876543210987654321");
            var mc = new MathContext(10, RoundingMode.HalfUp);
            var result = BigMath.Multiply(a, b, mc);
            Assert.NotNull(result);
        }
    }
}
