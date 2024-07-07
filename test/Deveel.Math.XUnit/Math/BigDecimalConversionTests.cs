using System;

using Xunit;

namespace Deveel.Math
{
    public static class BigDecimalConversionTests
    {
        [Theory]
        [InlineData("1")]
        [InlineData("-1")]
        public static void ConvertToBoolean_ShouldReturnTrue(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = Convert.ToBoolean(bigDecimal);
            Assert.True(result);
        }

        [Fact]
        public static void ConvertZeroToBoolean_ShouldReturnFalse()
        {
            var bigDecimal = BigDecimal.Zero;
            var result = Convert.ToBoolean(bigDecimal);
            Assert.False(result);
        }

        [Theory]
        [InlineData("0.1")]
        [InlineData("-0.1")]
        public static void ConvertFractionToBoolean_ShouldThrow(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<ArithmeticException>(() => Convert.ToBoolean(bigDecimal));
        }
    }
}
