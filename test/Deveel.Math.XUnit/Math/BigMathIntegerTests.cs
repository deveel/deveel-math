using System;
using Xunit;

namespace Deveel.Math {
    public class BigMathIntegerTests {
        [Fact]
        public void Min_FirstSmaller_ReturnsFirst() {
            var a = BigInteger.FromInt64(10);
            var b = BigInteger.FromInt64(20);
            Assert.Equal(a, BigMath.Min(a, b));
        }

        [Fact]
        public void Min_FirstLarger_ReturnsSecond() {
            var a = BigInteger.FromInt64(30);
            var b = BigInteger.FromInt64(20);
            Assert.Equal(b, BigMath.Min(a, b));
        }

        [Fact]
        public void Min_Equal_ReturnsEither() {
            var a = BigInteger.FromInt64(15);
            var b = BigInteger.FromInt64(15);
            Assert.Equal(a, BigMath.Min(a, b));
        }

        [Fact]
        public void Min_Negative_ReturnsMoreNegative() {
            var a = BigInteger.FromInt64(-10);
            var b = BigInteger.FromInt64(-5);
            Assert.Equal(a, BigMath.Min(a, b));
        }

        [Fact]
        public void Max_FirstLarger_ReturnsFirst() {
            var a = BigInteger.FromInt64(30);
            var b = BigInteger.FromInt64(20);
            Assert.Equal(a, BigMath.Max(a, b));
        }

        [Fact]
        public void Max_FirstSmaller_ReturnsSecond() {
            var a = BigInteger.FromInt64(10);
            var b = BigInteger.FromInt64(20);
            Assert.Equal(b, BigMath.Max(a, b));
        }

        [Fact]
        public void Max_Equal_ReturnsEither() {
            var a = BigInteger.FromInt64(15);
            var b = BigInteger.FromInt64(15);
            Assert.Equal(a, BigMath.Max(a, b));
        }

        [Fact]
        public void Max_Negative_ReturnsLessNegative() {
            var a = BigInteger.FromInt64(-10);
            var b = BigInteger.FromInt64(-5);
            Assert.Equal(b, BigMath.Max(a, b));
        }

        [Fact]
        public void Min_WithZeroAndPositive_ReturnsZero() {
            var a = BigInteger.Zero;
            var b = BigInteger.FromInt64(100);
            Assert.Equal(a, BigMath.Min(a, b));
        }

        [Fact]
        public void Max_WithNegativeAndZero_ReturnsZero() {
            var a = BigInteger.FromInt64(-100);
            var b = BigInteger.Zero;
            Assert.Equal(b, BigMath.Max(a, b));
        }

        [Fact]
        public void Min_LargeNumbers_ReturnsCorrect() {
            var a = BigInteger.Parse("99999999999999999999");
            var b = BigInteger.Parse("100000000000000000000");
            Assert.Equal(a, BigMath.Min(a, b));
        }

        [Fact]
        public void Max_LargeNumbers_ReturnsCorrect() {
            var a = BigInteger.Parse("99999999999999999999");
            var b = BigInteger.Parse("100000000000000000000");
            Assert.Equal(b, BigMath.Max(a, b));
        }

        [Fact]
        public void ModPow_PositiveValues_ReturnsCorrect() {
            var value = BigInteger.FromInt64(7);
            var exp = BigInteger.FromInt64(3);
            var mod = BigInteger.FromInt64(13);
            var result = BigMath.ModPow(value, exp, mod);
            Assert.Equal(BigInteger.FromInt64(5), result); // 7^3 = 343, 343 % 13 = 5
        }

        [Fact]
        public void ModPow_ExponentZero_ReturnsOne() {
            var value = BigInteger.FromInt64(7);
            var exp = BigInteger.Zero;
            var mod = BigInteger.FromInt64(13);
            var result = BigMath.ModPow(value, exp, mod);
            Assert.Equal(BigInteger.One, result);
        }

        [Fact]
        public void ModPow_ModOne_ReturnsZero() {
            var value = BigInteger.FromInt64(7);
            var exp = BigInteger.FromInt64(5);
            var mod = BigInteger.One;
            var result = BigMath.ModPow(value, exp, mod);
            Assert.Equal(BigInteger.Zero, result);
        }

        [Fact]
        public void ModPow_LargeExponent_ReturnsCorrect() {
            var value = BigInteger.FromInt64(2);
            var exp = BigInteger.FromInt64(10);
            var mod = BigInteger.FromInt64(1000);
            var result = BigMath.ModPow(value, exp, mod);
            Assert.Equal(BigInteger.FromInt64(24), result); // 2^10 = 1024, 1024 % 1000 = 24
        }

        [Fact]
        public void ModPow_NegativeMod_Throws() {
            var value = BigInteger.FromInt64(7);
            var exp = BigInteger.FromInt64(3);
            var mod = BigInteger.FromInt64(-13);
            Assert.Throws<ArithmeticException>(() => BigMath.ModPow(value, exp, mod));
        }

        [Fact]
        public void ModPow_ZeroMod_Throws() {
            var value = BigInteger.FromInt64(7);
            var exp = BigInteger.FromInt64(3);
            var mod = BigInteger.Zero;
            Assert.Throws<ArithmeticException>(() => BigMath.ModPow(value, exp, mod));
        }

        [Fact]
        public void ModPow_LargeValues_MatchesSystemNumerics() {
            var value = BigInteger.FromInt64(12345);
            var exp = BigInteger.FromInt64(50);
            var mod = BigInteger.FromInt64(99989);
            var expected = System.Numerics.BigInteger.ModPow(
                value.ToSystemBigInteger(),
                exp.ToSystemBigInteger(),
                mod.ToSystemBigInteger());
            Assert.Equal(expected, BigMath.ModPow(value, exp, mod).ToSystemBigInteger());
        }
    }
}
