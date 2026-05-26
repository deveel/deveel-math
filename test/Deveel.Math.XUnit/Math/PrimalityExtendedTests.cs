using System;
using Xunit;

namespace Deveel.Math {
    public class PrimalityExtendedTests {
        [Fact]
        public void BigInteger_NextProbablePrime() {
            Assert.Equal(BigInteger.FromInt64(2), BigInteger.NextProbablePrime(BigInteger.Zero));
            Assert.Equal(BigInteger.FromInt64(3), BigInteger.NextProbablePrime(BigInteger.FromInt64(2)));
            Assert.Equal(BigInteger.FromInt64(5), BigInteger.NextProbablePrime(BigInteger.FromInt64(3)));
            Assert.Equal(BigInteger.FromInt64(11), BigInteger.NextProbablePrime(BigInteger.FromInt64(10)));
        }
    }
}
