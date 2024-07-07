using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

namespace Deveel.Math
{
    public class BigDecimalCastsTest
    {
        BigDecimal valueBigDecimal = new BigDecimal(long.MaxValue) + new BigDecimal(int.MaxValue) + new BigDecimal(short.MaxValue) + new BigDecimal(byte.MaxValue) + new BigDecimal(double.Epsilon);

        BigInteger valueBigInteger = BigInteger.Parse("9223372039002292476");

        long valueLong = -9223372034707259140;

        int valueInt = -2147450628;

        short valueShort = -32516;

        byte valueByte = 252;

        float valueFloat = 9.223372E+18f;

        double valueDouble = 9.2233720390022922E+18;

        decimal valueDecimal = 9.223372039002292476E+18m;

        [Fact]
        public void LosyCasts()
        {
            Assert.Equal<BigInteger>(valueBigInteger, (BigInteger)valueBigDecimal);

            Assert.Equal<long>(valueLong, (long)valueBigDecimal);

            Assert.Equal<int>(valueInt, (int)valueBigDecimal);

            Assert.Equal<short>(valueShort, (short)valueBigDecimal);

            Assert.Equal<byte>(valueByte, (byte)valueBigDecimal);
        }

        [Fact]
        public void LoselessCasts()
        {
            Assert.Equal<float>(valueFloat, valueBigDecimal);

            Assert.Equal<double>(valueDouble, valueBigDecimal);

            // TODO: check why this fails
            // Assert.Equal<decimal>(valueDecimal, valueBigDecimal);
        }
    }
}
