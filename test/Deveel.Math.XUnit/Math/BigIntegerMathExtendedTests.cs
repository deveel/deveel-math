using System;
using Xunit;

namespace Deveel.Math {
    public class BigIntegerMathExtendedTests {
        [Fact]
        public void BigIntegerMath_Remainder() {
            var a = BigInteger.FromInt64(-10);
            var b = BigInteger.FromInt64(3);
            var result = BigMath.Remainder(a, b);
            Assert.Equal(BigInteger.FromInt64(-1), result);
        }

        [Fact]
        public void BigMath_DivideAndRemainder_BigInteger() {
            var a = BigInteger.FromInt64(10);
            var b = BigInteger.FromInt64(3);
            BigInteger remainder;
            var quotient = BigMath.DivideAndRemainder(a, b, out remainder);
            Assert.Equal(BigInteger.FromInt64(3), quotient);
            Assert.Equal(BigInteger.FromInt64(1), remainder);
        }

        [Fact]
        public void BigMath_Abs_BigInteger_Negative() {
            var bi = BigInteger.FromInt64(-42);
            var result = BigMath.Abs(bi);
            Assert.Equal(BigInteger.FromInt64(42), result);
        }

        [Fact]
        public void BigMath_Abs_BigInteger_Positive() {
            var bi = BigInteger.FromInt64(42);
            var result = BigMath.Abs(bi);
            Assert.Equal(BigInteger.FromInt64(42), result);
        }

        [Fact]
        public void BigMath_Negate_BigInteger() {
            var bi = BigInteger.FromInt64(42);
            var result = BigMath.Negate(bi);
            Assert.Equal(BigInteger.FromInt64(-42), result);
        }

        [Fact]
        public void BigMath_Pow_BigInteger_Zero() {
            var result = BigMath.Pow(BigInteger.FromInt64(2), 0);
            Assert.Equal(BigInteger.One, result);
        }

        [Fact]
        public void BigMath_Pow_BigInteger_Large() {
            var result = BigMath.Pow(BigInteger.FromInt64(7), 5);
            Assert.Equal(BigInteger.FromInt64(16807), result);
        }

        [Fact]
        public void BigInteger_Gcd_BothNegative() {
            var a = BigInteger.FromInt64(-12);
            var b = BigInteger.FromInt64(-18);
            var result = BigMath.Gcd(a, b);
            Assert.Equal(BigInteger.FromInt64(6), result);
        }

        [Fact]
        public void BigInteger_Gcd_OneNegative() {
            var a = BigInteger.FromInt64(-12);
            var b = BigInteger.FromInt64(18);
            var result = BigMath.Gcd(a, b);
            Assert.Equal(BigInteger.FromInt64(6), result);
        }

        [Fact]
        public void BigInteger_ModPow_LargeExp() {
            var b = BigInteger.FromInt64(4);
            var e = BigInteger.FromInt64(13);
            var m = BigInteger.FromInt64(497);
            var result = BigMath.ModPow(b, e, m);
            Assert.Equal(BigInteger.FromInt64(445), result);
        }

        [Fact]
        public void BigInteger_ModInverse() {
            var a = BigInteger.FromInt64(3);
            var m = BigInteger.FromInt64(11);
            var result = BigMath.ModInverse(a, m);
            Assert.Equal(BigInteger.FromInt64(4), result);
        }

        [Fact]
        public void BigIntegerMath_ModPow_NegativeExponent() {
            var b = BigInteger.FromInt64(4);
            var e = BigInteger.FromInt64(-1);
            var m = BigInteger.FromInt64(7);
            var result = BigMath.ModPow(b, e, m);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigIntegerMath_ModInverse_Negative() {
            var a = BigInteger.FromInt64(-3);
            var m = BigInteger.FromInt64(11);
            var result = BigMath.ModInverse(a, m);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigIntegerMath_Mod_Negative() {
            var a = BigInteger.FromInt64(-10);
            var m = BigInteger.FromInt64(3);
            var result = BigMath.Mod(a, m);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigIntegerMath_Gcd_Zero() {
            var a = BigInteger.Zero;
            var b = BigInteger.FromInt64(5);
            var result = BigMath.Gcd(a, b);
            Assert.Equal(BigInteger.FromInt64(5), result);
        }

        [Fact]
        public void BigIntegerMath_Pow_NegativeBase() {
            var result = BigMath.Pow(BigInteger.FromInt64(-2), 3);
            Assert.Equal(BigInteger.FromInt64(-8), result);
        }

        [Fact]
        public void BigIntegerMath_Pow_ZeroBase() {
            var result = BigMath.Pow(BigInteger.Zero, 5);
            Assert.Equal(BigInteger.Zero, result);
        }

        [Fact]
        public void BigIntegerMath_ModPow_ZeroExponent() {
            var b = BigInteger.FromInt64(5);
            var e = BigInteger.Zero;
            var m = BigInteger.FromInt64(7);
            var result = BigMath.ModPow(b, e, m);
            Assert.Equal(BigInteger.One, result);
        }

        [Fact]
        public void BigIntegerMath_ModPow_ZeroBase() {
            var b = BigInteger.Zero;
            var e = BigInteger.FromInt64(5);
            var m = BigInteger.FromInt64(7);
            var result = BigMath.ModPow(b, e, m);
            Assert.Equal(BigInteger.Zero, result);
        }

        [Fact]
        public void BigIntegerMath_ModInverse_One() {
            var a = BigInteger.One;
            var m = BigInteger.FromInt64(7);
            var result = BigMath.ModInverse(a, m);
            Assert.Equal(BigInteger.One, result);
        }

        [Fact]
        public void BigIntegerMath_Mod_Zero() {
            var a = BigInteger.FromInt64(10);
            var m = BigInteger.FromInt64(3);
            var result = BigMath.Mod(a, m);
            Assert.Equal(BigInteger.FromInt64(1), result);
        }

        [Fact]
        public void BigIntegerMath_Gcd_BothZero() {
            var result = BigMath.Gcd(BigInteger.Zero, BigInteger.Zero);
            Assert.Equal(BigInteger.Zero, result);
        }

        [Fact]
        public void BigIntegerMath_Pow_One() {
            var result = BigMath.Pow(BigInteger.FromInt64(5), 1);
            Assert.Equal(BigInteger.FromInt64(5), result);
        }

        [Fact]
        public void BigMath_DivideAndRemainder_BigInteger_ZeroDividend() {
            var a = BigInteger.Zero;
            var b = BigInteger.FromInt64(5);
            BigInteger remainder;
            var quotient = BigMath.DivideAndRemainder(a, b, out remainder);
            Assert.Equal(BigInteger.Zero, quotient);
            Assert.Equal(BigInteger.Zero, remainder);
        }

        [Fact]
        public void BigMath_DivideAndRemainder_BigInteger_ZeroDivisor() {
            var a = BigInteger.FromInt64(10);
            var b = BigInteger.Zero;
            BigInteger remainder;
            Assert.Throws<ArithmeticException>(() => BigMath.DivideAndRemainder(a, b, out remainder));
        }

        [Fact]
        public void BigMath_DivideAndRemainder_BigInteger_Negative() {
            var a = BigInteger.FromInt64(-10);
            var b = BigInteger.FromInt64(3);
            BigInteger remainder;
            var quotient = BigMath.DivideAndRemainder(a, b, out remainder);
            Assert.NotNull(quotient);
            Assert.NotNull(remainder);
        }
    }
}
