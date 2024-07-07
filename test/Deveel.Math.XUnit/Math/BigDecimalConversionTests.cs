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
        [InlineData("4")]
        public static void ConvertToBoolean_ShouldThrowInvalidCastException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<InvalidCastException>(() => Convert.ToBoolean((object) bigDecimal));
        }

        [Theory]
        [InlineData("1304493.4")]
        [InlineData("0.433")]
        public static void ConvertToBoolean_ShouldThrowArithmeticException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<ArithmeticException>(() => Convert.ToBoolean((object) bigDecimal));
        }

        [Fact]
        public static void ConvertZeroToBoolean_ShouldReturnFalse()
        {
            var bigDecimal = BigDecimal.Zero;
            var result = Convert.ToBoolean((object) bigDecimal);
            Assert.False(result);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("0", 0)]
        [InlineData("254", 254)]
        public static void ConvertToByte_ShouldReturn(string value, byte expected)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = Convert.ToByte((object) bigDecimal);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("-1")]
        [InlineData("256")]
        public static void ConvertToByte_ShouldThrowCastException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<InvalidCastException>(() => Convert.ToByte((object) bigDecimal));
        }

        [Theory]
        [InlineData("56600490011.1345")]
        [InlineData("0.433")]
        public static void ConvertToByte_ShouldThrowArithmeticException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<ArithmeticException>(() => Convert.ToByte((object) bigDecimal));
        }

        [Theory]
        [InlineData("4533", 4533)]
        [InlineData("0", 0)]
        [InlineData("88", 'X')]
        public static void ConvertToChar_ShouldReturn(string value, char expected)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = Convert.ToChar((object) bigDecimal);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("65536")]
        [InlineData("56600490011.1345")]
        public static void ConvertToChar_ShouldThrowArithmeticException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<ArithmeticException>(() => Convert.ToChar((object) bigDecimal));
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("0", 0)]
        [InlineData("657488392", 657488392)]
        [InlineData("-657488392", -657488392)]
        public static void ConvertToInt32_ShouldReturn(string value, int expected)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = Convert.ToInt32((object) bigDecimal);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("2147483648")]
        [InlineData("-2147483649")]
        [InlineData("56600490011.1345")]
        public static void ConvertToInt32_ShouldThrowArithmeticException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<ArithmeticException>(() => Convert.ToInt32((object) bigDecimal));
        }
    }
}
