using System;
using System.Globalization;

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
        [InlineData("56600490011.1345")]
        [InlineData("0.433")]
        public static void ConvertToByte_ShouldThrowArithmeticException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<ArithmeticException>(() => Convert.ToByte((object) bigDecimal));
        }

        [Theory]
        [InlineData("256")]
        public static void ConvertToByte_ShouldThrowCastException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<InvalidCastException>(() => Convert.ToByte((object)bigDecimal));
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
        [InlineData("56600490011.1345")]
        public static void ConvertToInt32_ShouldThrowArithmeticException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<ArithmeticException>(() => Convert.ToInt32((object) bigDecimal));
        }

        [Theory]
        [InlineData("2147483648")]
        [InlineData("-2147483649")]
        public static void ConvertToInt32_ShouldThrowCastException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<InvalidCastException>(() => Convert.ToInt32((object)bigDecimal));
        }



        [Theory]
        [InlineData("1", 1)]
        [InlineData("0", 0)]
        [InlineData("657488392", 657488392)]
        [InlineData("-657488392", -657488392)]
        public static void ConvertToInt64_ShouldReturn(string value, long expected)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = Convert.ToInt64((object) bigDecimal);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("9223372036854775808")]
        [InlineData("-9223372036854775809")]
        [InlineData("56600490011.1345")]
        public static void ConvertToInt64_ShouldThrow(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<ArithmeticException>(() => Convert.ToInt64((object) bigDecimal));
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("0", 0)]
        [InlineData("32767", 32767)]
        [InlineData("-32768", -32768)]
        public static void ConvertToInt16_ShouldReturn(string value, short expected)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = Convert.ToInt16((object) bigDecimal);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("56600490011.1345")]
        public static void ConvertToInt16_ShouldThrowArithmeticException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<ArithmeticException>(() => Convert.ToInt16((object) bigDecimal));
        }

        [Theory]
        [InlineData("32769")]
        [InlineData("-32770")]
        public static void ConvertToInt16_ShouldThrowCastException(string value)
        {
            var bigDecimal = BigDecimal.Parse(value);
            Assert.Throws<InvalidCastException>(() => Convert.ToInt16((object)bigDecimal));
        }


        [Theory]
        [InlineData("1", 1)]
        [InlineData("0", 0)]
        [InlineData("254", 254)]
        [InlineData("456.02", 456.02)]
        [InlineData("-45634.032", -45634.032)]
        public static void ConvertToSingle_ShouldReturn(string value, float expected)
        {            
            var bigDecimal = BigDecimal.Parse(value);
            var result = Convert.ToSingle((object) bigDecimal);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("3.4028234663852886E+38", 3.4028234E+38)]
        [InlineData("-3.4028234663852886E+38", -3.4028234E+38)]
        [InlineData("56600490011.1345", 5.660049E+10)]
        public static void ConvertToSingle_ShouldRoundDown(string value, float expected)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = Convert.ToSingle((object) bigDecimal);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("0", 0)]
        [InlineData("254", 254)]
        [InlineData("45687.023442", 45687.023442)]
        [InlineData("-45634.032", -45634.032)]
        public static void ConvertToDouble_ShouldReturn(string value, double expected)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = Convert.ToDouble((object) bigDecimal);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1.7976931348623157E+3", "1797.6931348623157")]
        [InlineData("15660049945.904029474", "15660049945.904029474")]
        [InlineData("4.585774E+20", "458577400000000000000")]
        public static void ToPlainString_InvariantCulture_ShouldReturn(string value, string expected)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = bigDecimal.ToString("P");
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1.7976931348623157E+3", "1797.6931348623157")]
        [InlineData("15660049945.904029474", "15660049945.904029474")]
        [InlineData("4.585774E+20", "458.5774E+18")]
        public static void ToEngineeringString_InvariantCulture_ShouldReturn(string value, string expected)
        {
            var bigDecimal = BigDecimal.Parse(value);
            var result = bigDecimal.ToString("E");
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("3463.999301", "3463.999301")]
        public static void ToString_InvariantCulture_ShouldReturn(string value, string expected)
        {
            var bigDeciaml = BigDecimal.Parse(value);
            var result = bigDeciaml.ToString();
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("3463.999301", "en-US", "{0:G}", "3463.999301")]
        [InlineData("67849950554E+10", "en-US", "{0:P}", "678499505540000000000")]
        [InlineData("67849950554E+10", "en-US", "{0:E}", "678.49950554E+18")]
        [InlineData("3463.999301", "fr-FR", "{0:G}", "3463,999301")]
        public static void StringFormat_ShouldReturn(string value, string culture, string format, string expected)
        {
            var cultureInfo = new CultureInfo(culture);
            var bigDecimal = BigDecimal.Parse(value);
            var result = String.Format(cultureInfo, format, bigDecimal);
            Assert.Equal(expected, result);
        }
    }
}
