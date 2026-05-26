using System;
using Xunit;

namespace Deveel.Math {
    public class BigIntegerExtendedTests {
        [Fact]
        public void GetHashCode_SameValue_ReturnsSame() {
            var a = BigInteger.FromInt64(12345);
            var b = BigInteger.FromInt64(12345);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DifferentValues_ReturnsDifferent() {
            var a = BigInteger.FromInt64(12345);
            var b = BigInteger.FromInt64(54321);
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_LargeValues_Consistent() {
            var a = BigInteger.Parse("12345678901234567890");
            var b = BigInteger.Parse("12345678901234567890");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void GetHashCode_Zero_ReturnsSame() {
            Assert.Equal(BigInteger.Zero.GetHashCode(), BigInteger.Zero.GetHashCode());
        }

        [Fact]
        public void ClearBit_AtPosition_ReturnsValueWithBitCleared() {
            var value = BigInteger.FromInt64(15); // 1111
            var result = BigInteger.ClearBit(value, 1);
            Assert.Equal(BigInteger.FromInt64(13), result); // 1101
        }

        [Fact]
        public void ClearBit_AlreadyZero_ReturnsSame() {
            var value = BigInteger.FromInt64(8); // 1000
            var result = BigInteger.ClearBit(value, 0);
            Assert.Equal(BigInteger.FromInt64(8), result);
        }

        [Fact]
        public void ClearBit_AllBits_ReturnsZero() {
            var value = BigInteger.FromInt64(1); // 1
            var result = BigInteger.ClearBit(value, 0);
            Assert.Equal(BigInteger.Zero, result);
        }

        [Fact]
        public void FlipBit_ZeroToOne_ReturnsValueWithBitSet() {
            var value = BigInteger.FromInt64(8); // 1000
            var result = BigInteger.FlipBit(value, 0);
            Assert.Equal(BigInteger.FromInt64(9), result); // 1001
        }

        [Fact]
        public void FlipBit_OneToZero_ReturnsValueWithBitCleared() {
            var value = BigInteger.FromInt64(9); // 1001
            var result = BigInteger.FlipBit(value, 3);
            Assert.Equal(BigInteger.FromInt64(1), result); // 0001
        }

        [Fact]
        public void FlipBit_ZeroValue_ReturnsPowerOfTwo() {
            var result = BigInteger.FlipBit(BigInteger.Zero, 4);
            Assert.Equal(BigInteger.FromInt64(16), result);
        }

        [Fact]
        public void FlipBit_BeyondBitLength_ExtendsNumber() {
            var value = BigInteger.FromInt64(1); // 1
            var result = BigInteger.FlipBit(value, 10);
            Assert.Equal(BigInteger.FromInt64(1025), result); // 1 + 1024
        }

        [Fact]
        public void NextProbablePrime_AfterTwo_ReturnsThree() {
            var result = BigInteger.NextProbablePrime(BigInteger.FromInt64(2));
            Assert.Equal(BigInteger.FromInt64(3), result);
        }

        [Fact]
        public void NextProbablePrime_AfterThree_ReturnsFive() {
            var result = BigInteger.NextProbablePrime(BigInteger.FromInt64(3));
            Assert.Equal(BigInteger.FromInt64(5), result);
        }

        [Fact]
        public void NextProbablePrime_AfterTen_ReturnsEleven() {
            var result = BigInteger.NextProbablePrime(BigInteger.FromInt64(10));
            Assert.Equal(BigInteger.FromInt64(11), result);
        }

        [Fact]
        public void NextProbablePrime_AfterThirteen_ReturnsSeventeen() {
            var result = BigInteger.NextProbablePrime(BigInteger.FromInt64(13));
            Assert.Equal(BigInteger.FromInt64(17), result);
        }

        [Fact]
        public void NextProbablePrime_LargeNumber_ReturnsPrime() {
            var result = BigInteger.NextProbablePrime(BigInteger.FromInt64(1000));
            Assert.True(BigInteger.IsProbablePrime(result, 10));
        }

        [Fact]
        public void NextProbablePrime_Zero_ReturnsTwo() {
            var result = BigInteger.NextProbablePrime(BigInteger.Zero);
            Assert.Equal(BigInteger.FromInt64(2), result);
        }

        [Fact]
        public void ProbablePrime_GeneratesPrimeOfGivenBitLength() {
            var result = BigInteger.ProbablePrime(10, new Random(42));
            Assert.True(BigInteger.IsProbablePrime(result, 10));
            Assert.True(result.BitLength >= 9); // allow some slack
        }

        [Fact]
        public void ProbablePrime_DifferentSeeds_DifferentResults() {
            var a = BigInteger.ProbablePrime(10, new Random(42));
            var b = BigInteger.ProbablePrime(10, new Random(123));
            Assert.NotEqual(a, b);
        }

        [Fact]
        public void TryParse_ValidString_ReturnsTrue() {
            BigInteger result;
            Assert.True(BigInteger.TryParse("12345", out result));
            Assert.Equal(BigInteger.FromInt64(12345), result);
        }

        [Fact]
        public void TryParse_NegativeString_ReturnsTrue() {
            BigInteger result;
            Assert.True(BigInteger.TryParse("-12345", out result));
            Assert.Equal(BigInteger.FromInt64(-12345), result);
        }

        [Fact]
        public void TryParse_InvalidString_ReturnsFalse() {
            BigInteger result;
            Assert.False(BigInteger.TryParse("abc", out result));
        }

        [Fact]
        public void TryParse_EmptyString_ReturnsFalse() {
            BigInteger result;
            Assert.False(BigInteger.TryParse("", out result));
        }

        [Fact]
        public void TryParse_WithRadix_ValidString_ReturnsTrue() {
            BigInteger result;
            Assert.True(BigInteger.TryParse("ff", 16, out result));
            Assert.Equal(BigInteger.FromInt64(255), result);
        }

        [Fact]
        public void TryParse_WithRadix_Binary_ReturnsTrue() {
            BigInteger result;
            Assert.True(BigInteger.TryParse("1010", 2, out result));
            Assert.Equal(BigInteger.FromInt64(10), result);
        }

        [Fact]
        public void TryParse_WithRadix_InvalidString_ReturnsFalse() {
            BigInteger result;
            Assert.False(BigInteger.TryParse("xyz", 16, out result));
        }

        [Fact]
        public void TryParse_WithRadix_InvalidRadix_ReturnsFalse() {
            BigInteger result;
            Assert.False(BigInteger.TryParse("10", 37, out result));
        }

        [Fact]
        public void FromSystemBigInteger_ConvertsCorrectly() {
            var sn = new System.Numerics.BigInteger(12345);
            var result = BigInteger.FromSystemBigInteger(sn);
            Assert.Equal(BigInteger.FromInt64(12345), result);
        }

        [Fact]
        public void FromSystemBigInteger_Zero_ReturnsZero() {
            var sn = System.Numerics.BigInteger.Zero;
            var result = BigInteger.FromSystemBigInteger(sn);
            Assert.Equal(BigInteger.Zero, result);
        }

        [Fact]
        public void FromSystemBigInteger_LargeValue_ConvertsCorrectly() {
            var sn = System.Numerics.BigInteger.Parse("123456789012345678901234567890");
            var result = BigInteger.FromSystemBigInteger(sn);
            Assert.Equal("123456789012345678901234567890", result.ToString());
        }

        [Fact]
        public void ImplicitOperator_ToSystemBigInteger_Converts() {
            var dmath = BigInteger.FromInt64(12345);
            System.Numerics.BigInteger sn = dmath;
            Assert.Equal(12345L, (long)sn);
        }

        [Fact]
        public void ExplicitOperator_FromSystemBigInteger_Converts() {
            var sn = new System.Numerics.BigInteger(12345);
            var dmath = (BigInteger)sn;
            Assert.Equal(BigInteger.FromInt64(12345), dmath);
        }
    }
}
