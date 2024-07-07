using System;

using Xunit;

namespace Deveel.Math
{
    public static class BigDecimalConversionTests
    {
        [Theory]
        [InlineData("1", true)]
        [InlineData("0", false)]
        public static void ConvertToBoolean_ShouldReturn(string value, bool expected)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = Convert.ToBoolean((object) bigDecimal);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("-1")]
        [InlineData("3.1")]
        [InlineData("56600490011.1345")]
        public static void ConvertToBoolean_ShouldThrowInvalidCastException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<InvalidCastException>(() => Convert.ToBoolean((object) bigDecimal));
        }

        [Fact]
        public static void ConvertZeroToBoolean_ShouldReturnFalse()
        {
            var bigDecimal = BigDecimal.Zero;
            var result = Convert.ToBoolean((object) bigDecimal);
            Assert.False(result);
        }
    }
}
