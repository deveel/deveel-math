using Xunit;

namespace Deveel.Math
{
    public static class BigDecimalParsingTests
    {
        [Theory]
        [InlineData("1", true)]
        [InlineData("499499400303.00011922", true)]
        [InlineData("-2404003.3993930e32", true)]
        [InlineData("foo-bar", false)]
        public static void TryParse_ShouldReturn(string value, bool expected)
        {
            var result = BigDecimal.TryParse(value, out var bigDecimal);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("499499400303.00011922", true)]
        [InlineData("-2404003.3993930e32", true)]
        [InlineData("0.900231e145", true)]
        [InlineData("foo-bar", false)]
        public static void TryParseFromChars_ShouldReturn(string value, bool expected)
        {
            var result = BigDecimal.TryParse(value.ToCharArray(), out var bigDecimal);
            Assert.Equal(expected, result);
        }
    }
}
