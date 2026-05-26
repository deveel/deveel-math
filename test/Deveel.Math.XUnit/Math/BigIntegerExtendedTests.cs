using System;
using Xunit;
using SN = System.Numerics;

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
            var value = BigInteger.FromInt64(15);
            var result = BigInteger.ClearBit(value, 1);
            Assert.Equal(BigInteger.FromInt64(13), result);
        }

        [Fact]
        public void ClearBit_AlreadyZero_ReturnsSame() {
            var value = BigInteger.FromInt64(8);
            var result = BigInteger.ClearBit(value, 0);
            Assert.Equal(BigInteger.FromInt64(8), result);
        }

        [Fact]
        public void ClearBit_AllBits_ReturnsZero() {
            var value = BigInteger.FromInt64(1);
            var result = BigInteger.ClearBit(value, 0);
            Assert.Equal(BigInteger.Zero, result);
        }

        [Fact]
        public void FlipBit_ZeroToOne_ReturnsValueWithBitSet() {
            var value = BigInteger.FromInt64(8);
            var result = BigInteger.FlipBit(value, 0);
            Assert.Equal(BigInteger.FromInt64(9), result);
        }

        [Fact]
        public void FlipBit_OneToZero_ReturnsValueWithBitCleared() {
            var value = BigInteger.FromInt64(9);
            var result = BigInteger.FlipBit(value, 3);
            Assert.Equal(BigInteger.FromInt64(1), result);
        }

        [Fact]
        public void FlipBit_ZeroValue_ReturnsPowerOfTwo() {
            var result = BigInteger.FlipBit(BigInteger.Zero, 4);
            Assert.Equal(BigInteger.FromInt64(16), result);
        }

        [Fact]
        public void FlipBit_BeyondBitLength_ExtendsNumber() {
            var value = BigInteger.FromInt64(1);
            var result = BigInteger.FlipBit(value, 10);
            Assert.Equal(BigInteger.FromInt64(1025), result);
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
            Assert.True(result.BitLength >= 9);
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

        [Fact]
        public void BigInteger_ImplicitFromByte() {
            BigInteger bi = (byte)42;
            Assert.Equal(42L, (long)bi);
        }

        [Fact]
        public void BigInteger_ImplicitFromSByte() {
            BigInteger bi = (sbyte)(-42);
            Assert.Equal(-42L, (long)bi);
        }

        [Fact]
        public void BigInteger_ImplicitFromShort() {
            BigInteger bi = (short)(-32000);
            Assert.Equal(-32000L, (long)bi);
        }

        [Fact]
        public void BigInteger_ImplicitFromLong() {
            BigInteger bi = 123456789012345L;
            Assert.Equal(123456789012345L, (long)bi);
        }

        [Fact]
        public void BigInteger_ExplicitToFloat() {
            BigInteger bi = BigInteger.FromInt64(12345);
            Assert.Equal(12345f, (float)bi);
        }

        [Fact]
        public void BigInteger_ExplicitToDouble() {
            BigInteger bi = BigInteger.FromInt64(12345);
            Assert.Equal(12345.0, (double)bi);
        }

        [Fact]
        public void BigInteger_ExplicitToDecimal() {
            BigInteger bi = BigInteger.FromInt64(12345);
            Assert.Equal(12345m, (decimal)bi);
        }

        [Fact]
        public void BigInteger_ImplicitToFloat() {
            BigInteger bi = BigInteger.FromInt64(12345);
            float f = bi;
            Assert.Equal(12345f, f);
        }

        [Fact]
        public void BigInteger_ImplicitToDouble() {
            BigInteger bi = BigInteger.FromInt64(12345);
            double d = bi;
            Assert.Equal(12345.0, d);
        }

        [Fact]
        public void BigInteger_ImplicitToDecimal() {
            BigInteger bi = BigInteger.FromInt64(12345);
            decimal d = bi;
            Assert.Equal(12345m, d);
        }

        [Fact]
        public void BigInteger_Increment() {
            BigInteger bi = BigInteger.FromInt64(5);
            bi++;
            Assert.Equal(BigInteger.FromInt64(6), bi);
        }

        [Fact]
        public void BigInteger_Decrement() {
            BigInteger bi = BigInteger.FromInt64(5);
            bi--;
            Assert.Equal(BigInteger.FromInt64(4), bi);
        }

        [Fact]
        public void BigInteger_Equals() {
            var a = BigInteger.FromInt64(42);
            var b = BigInteger.FromInt64(42);
            var c = BigInteger.FromInt64(43);
            Assert.True(a.Equals(b));
            Assert.False(a.Equals(c));
            Assert.False(a.Equals(null));
            Assert.False(a.Equals("not a bigint"));
        }

        [Fact]
        public void BigInteger_ExplicitToInt16() {
            BigInteger bi = 12345;
            short s = (short)bi;
            Assert.Equal((short)12345, s);
        }

        [Fact]
        public void BigInteger_ExplicitToInt32() {
            BigInteger bi = 12345;
            int i = (int)bi;
            Assert.Equal(12345, i);
        }

        [Fact]
        public void BigInteger_ToString_Radix() {
            var bi = BigInteger.FromInt64(255);
            Assert.Equal("ff", bi.ToString(16));
            Assert.Equal("377", bi.ToString(8));
            Assert.Equal("11111111", bi.ToString(2));
            Assert.Equal("255", bi.ToString(10));
        }

        [Fact]
        public void BigInteger_ToString_Radix_Negative() {
            var bi = BigInteger.FromInt64(-255);
            Assert.Equal("-ff", bi.ToString(16));
        }

        [Fact]
        public void BigInteger_ToString_Radix_Zero() {
            Assert.Equal("0", BigInteger.Zero.ToString(10));
        }

        [Fact]
        public void BigInteger_ToString_Radix_Large() {
            var bi = BigInteger.Parse("12345678901234567890");
            var s = bi.ToString(16);
            Assert.False(string.IsNullOrEmpty(s));
        }

        [Fact]
        public void BigInteger_ToString_Radix_All() {
            var bi = BigInteger.Parse("4294967296");
            for (int r = 2; r <= 36; r++) {
                var s = bi.ToString(r);
                Assert.False(string.IsNullOrEmpty(s), $"ToString({r}) failed: got empty/null");
            }
        }

        [Fact]
        public void BigInteger_ToString_MaxRadix() {
            var bi = BigInteger.Parse("4294967296");
            var s = bi.ToString(36).ToLowerInvariant();
            Assert.False(string.IsNullOrEmpty(s));
        }

        [Fact]
        public void BigInteger_ToByteArray_Zero() {
            var bytes = BigInteger.Zero.ToByteArray();
            Assert.Equal(new byte[] { 0 }, bytes);
        }

        [Fact]
        public void BigInteger_ToByteArray_Positive() {
            var bi = BigInteger.FromInt64(0x1234567890L);
            var bytes = bi.ToByteArray();
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void BigInteger_ToByteArray_Negative() {
            var bi = BigInteger.FromInt64(-12345);
            var bytes = bi.ToByteArray();
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void BigInteger_Abs_MinValue() {
            var min = BigInteger.FromInt64(long.MinValue);
            var abs = BigMath.Abs(min);
            Assert.True(abs.Sign > 0);
        }

        [Fact]
        public void BigInteger_CompareTo_Equals() {
            var a = BigInteger.FromInt64(42);
            Assert.Equal(0, a.CompareTo(BigInteger.FromInt64(42)));
            Assert.True(a.CompareTo(BigInteger.FromInt64(41)) > 0);
            Assert.True(a.CompareTo(BigInteger.FromInt64(43)) < 0);
        }

        [Fact]
        public void BigInteger_ConvertViaIConvertible() {
            var n = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)n;
            Assert.Equal(TypeCode.Object, conv.GetTypeCode());
        }

        [Fact]
        public void BigInteger_Explicit_From_SNBigInteger() {
            var sn = SN.BigInteger.Parse("12345678901234567890");
            var bi = (BigInteger)sn;
            Assert.Equal(sn.ToString(), bi.ToString());
        }

        [Fact]
        public void BigInteger_ShiftLeft_Large() {
            var bi = BigInteger.FromInt64(1);
            var result = bi << 100;
            Assert.True(result.Sign > 0);
        }

        [Fact]
        public void BigInteger_ShiftRight_Negative() {
            var bi = BigInteger.FromInt64(-256);
            var result = bi >> 4;
            Assert.Equal(BigInteger.FromInt64(-256 >> 4), result);
        }

        [Fact]
        public void BigInteger_Add_Large() {
            var a = BigInteger.Parse("12345678901234567890");
            var b = BigInteger.Parse("-9876543210987654321");
            var snA = SN.BigInteger.Parse("12345678901234567890");
            var snB = SN.BigInteger.Parse("-9876543210987654321");
            var r = BigMath.Add(a, b);
            var snR = snA + snB;
            Assert.Equal(snR.ToString(), r.ToString());
        }

        [Fact]
        public void BigInteger_Subtract_Negative() {
            var a = BigInteger.FromInt64(-5);
            var b = BigInteger.FromInt64(10);
            var result = BigMath.Subtract(a, b);
            Assert.Equal(BigInteger.FromInt64(-15), result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(-42)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue + 1)]
        public void BigInteger_Not(long v) {
            var bi = BigInteger.FromInt64(v);
            var expected = (long)~v;
            var result = (long)~bi;
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(-1, -1)]
        [InlineData(42, -1)]
        [InlineData(-1, 42)]
        [InlineData(12345, 67890)]
        [InlineData(-12345, 67890)]
        [InlineData(12345, -67890)]
        [InlineData(-12345, -67890)]
        [InlineData(int.MaxValue, 1)]
        [InlineData(int.MinValue, -1)]
        [InlineData(long.MaxValue, long.MinValue)]
        [InlineData(long.MinValue, long.MaxValue)]
        public void BigInteger_And(long a, long b) {
            var biA = BigInteger.FromInt64(a);
            var biB = BigInteger.FromInt64(b);
            var expected = (long)(a & b);
            var result = (long)(biA & biB);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(-1, -1)]
        [InlineData(42, -1)]
        [InlineData(-1, 42)]
        [InlineData(12345, 67890)]
        [InlineData(-12345, 67890)]
        [InlineData(12345, -67890)]
        [InlineData(-12345, -67890)]
        [InlineData(int.MaxValue, -1)]
        [InlineData(int.MinValue + 1, 1)]
        [InlineData(long.MaxValue, long.MinValue + 1)]
        [InlineData(long.MinValue + 1, long.MaxValue)]
        public void BigInteger_Or(long a, long b) {
            var biA = BigInteger.FromInt64(a);
            var biB = BigInteger.FromInt64(b);
            var expected = (long)(a | b);
            var result = (long)(biA | biB);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(-1, -1)]
        [InlineData(42, -1)]
        [InlineData(-1, 42)]
        [InlineData(12345, 67890)]
        [InlineData(-12345, 67890)]
        [InlineData(12345, -67890)]
        [InlineData(-12345, -67890)]
        [InlineData(int.MaxValue, int.MinValue)]
        [InlineData(long.MaxValue, long.MinValue)]
        public void BigInteger_Xor(long a, long b) {
            var biA = BigInteger.FromInt64(a);
            var biB = BigInteger.FromInt64(b);
            var expected = (long)(a ^ b);
            var result = (long)(biA ^ biB);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(-1, -1)]
        [InlineData(42, 17)]
        [InlineData(-42, 17)]
        [InlineData(42, -17)]
        [InlineData(-42, -17)]
        [InlineData(long.MaxValue, 1)]
        [InlineData(long.MinValue, -1)]
        public void BigInteger_AndNot(long a, long b) {
            var biA = BigInteger.FromInt64(a);
            var biB = BigInteger.FromInt64(b);
            var expected = (long)(a & ~b);
            var result = (long)BigMath.AndNot(biA, biB);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void BigInteger_Large_And() {
            var a = BigInteger.Parse("12345678901234567890");
            var b = BigInteger.Parse("9876543210987654321");
            var snA = SN.BigInteger.Parse("12345678901234567890");
            var snB = SN.BigInteger.Parse("9876543210987654321");
            var snR = snA & snB;
            var r = a & b;
            Assert.Equal(snR.ToString(), r.ToString());
        }

        [Fact]
        public void BigInteger_Large_Or() {
            var a = BigInteger.Parse("12345678901234567890");
            var b = BigInteger.Parse("9876543210987654321");
            var snA = SN.BigInteger.Parse("12345678901234567890");
            var snB = SN.BigInteger.Parse("9876543210987654321");
            var snR = snA | snB;
            var r = a | b;
            Assert.Equal(snR.ToString(), r.ToString());
        }

        [Fact]
        public void BigInteger_Large_Xor() {
            var a = BigInteger.Parse("12345678901234567890");
            var b = BigInteger.Parse("9876543210987654321");
            var snA = SN.BigInteger.Parse("12345678901234567890");
            var snB = SN.BigInteger.Parse("9876543210987654321");
            var snR = snA ^ snB;
            var r = a ^ b;
            Assert.Equal(snR.ToString(), r.ToString());
        }

        [Fact]
        public void BigInteger_Large_Negative_And() {
            var a = BigInteger.Parse("-12345678901234567890");
            var b = BigInteger.Parse("9876543210987654321");
            var snA = SN.BigInteger.Parse("-12345678901234567890");
            var snB = SN.BigInteger.Parse("9876543210987654321");
            var snR = snA & snB;
            var r = a & b;
            Assert.Equal(snR.ToString(), r.ToString());
        }

        [Fact]
        public void BigInteger_Large_Both_Negative_And() {
            var a = BigInteger.Parse("-12345678901234567890");
            var b = BigInteger.Parse("-9876543210987654321");
            var snA = SN.BigInteger.Parse("-12345678901234567890");
            var snB = SN.BigInteger.Parse("-9876543210987654321");
            var snR = snA & snB;
            var r = a & b;
            Assert.Equal(snR.ToString(), r.ToString());
        }

        [Fact]
        public void BigInteger_And_Negative() {
            var a = BigInteger.FromInt64(-12345);
            var b = BigInteger.FromInt64(67890);
            var result = a & b;
            Assert.Equal(BigInteger.FromInt64(-12345 & 67890), result);
        }

        [Fact]
        public void BigInteger_Or_Negative() {
            var a = BigInteger.FromInt64(-12345);
            var b = BigInteger.FromInt64(67890);
            var result = a | b;
            Assert.Equal(BigInteger.FromInt64(-12345 | 67890), result);
        }

        [Fact]
        public void BigInteger_Xor_Negative() {
            var a = BigInteger.FromInt64(-12345);
            var b = BigInteger.FromInt64(67890);
            var result = a ^ b;
            Assert.Equal(BigInteger.FromInt64(-12345 ^ 67890), result);
        }

        [Fact]
        public void BigInteger_AndNot_Negative() {
            var a = BigInteger.FromInt64(-12345);
            var b = BigInteger.FromInt64(67890);
            var result = BigMath.AndNot(a, b);
            Assert.Equal(BigInteger.FromInt64(-12345 & ~67890), result);
        }

        [Fact]
        public void BigInteger_And_BothNegative() {
            var a = BigInteger.FromInt64(-12345);
            var b = BigInteger.FromInt64(-67890);
            var result = a & b;
            Assert.Equal(BigInteger.FromInt64(-12345 & -67890), result);
        }

        [Fact]
        public void BigInteger_Or_BothNegative() {
            var a = BigInteger.FromInt64(-12345);
            var b = BigInteger.FromInt64(-67890);
            var result = a | b;
            Assert.Equal(BigInteger.FromInt64(-12345 | -67890), result);
        }

        [Fact]
        public void BigInteger_Xor_BothNegative() {
            var a = BigInteger.FromInt64(-12345);
            var b = BigInteger.FromInt64(-67890);
            var result = a ^ b;
            Assert.Equal(BigInteger.FromInt64(-12345 ^ -67890), result);
        }

        [Fact]
        public void BigInteger_AndNot_BothNegative() {
            var a = BigInteger.FromInt64(-12345);
            var b = BigInteger.FromInt64(-67890);
            var result = BigMath.AndNot(a, b);
            Assert.Equal(BigInteger.FromInt64(-12345 & ~-67890), result);
        }

        [Fact]
        public void BigInteger_Xor_Positive() {
            var a = BigInteger.FromInt64(12345);
            var b = BigInteger.FromInt64(67890);
            var result = a ^ b;
            Assert.Equal(BigInteger.FromInt64(12345 ^ 67890), result);
        }

        [Fact]
        public void BigInteger_Xor_NegPositive() {
            var a = BigInteger.FromInt64(-12345);
            var b = BigInteger.FromInt64(67890);
            var result = a ^ b;
            Assert.Equal(BigInteger.FromInt64(-12345 ^ 67890), result);
        }

        [Fact]
        public void BigInteger_Xor_PosNegative() {
            var a = BigInteger.FromInt64(12345);
            var b = BigInteger.FromInt64(-67890);
            var result = a ^ b;
            Assert.Equal(BigInteger.FromInt64(12345 ^ -67890), result);
        }

        [Fact]
        public void BigInteger_Not_Positive() {
            Assert.Equal(BigInteger.FromInt64(~42), ~BigInteger.FromInt64(42));
        }

        [Fact]
        public void BigInteger_Not_Negative() {
            Assert.Equal(BigInteger.FromInt64(~(-42)), ~BigInteger.FromInt64(-42));
        }

        [Fact]
        public void BigInteger_Not_Zero() {
            Assert.Equal(BigInteger.FromInt64(~0), ~BigInteger.Zero);
        }

        [Fact]
        public void BigInteger_FlipBit_LargePosition() {
            var bi = BigInteger.FromInt64(1);
            var result = BigInteger.FlipBit(bi, 100);
            Assert.True(result > bi);
        }

        [Fact]
        public void BigInteger_FlipBit_Negative_LargePosition() {
            var bi = BigInteger.FromInt64(-1);
            var result = BigInteger.FlipBit(bi, 100);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigInteger_FlipBit_ClearBit() {
            var bi = BigInteger.Parse("4294967295");
            var result = BigInteger.FlipBit(bi, 0);
            Assert.Equal(BigInteger.Parse("4294967294"), result);
        }

        [Fact]
        public void BigInteger_BitCount_Large() {
            var bi = BigInteger.Parse(new string('1', 100));
            var count = bi.BitCount;
            Assert.True(count > 0);
        }

        [Fact]
        public void BigInteger_BitCount_SparseBits() {
            var bi = BigInteger.Parse("9223372036854775809");
            var count = bi.BitCount;
            Assert.True(count > 0);
        }

        [Fact]
        public void BigInteger_ShiftRight_LargeOffset() {
            var bi = BigInteger.Parse(new string('9', 100));
            var result = bi >> 500;
            Assert.NotNull(result);
        }

        [Fact]
        public void BigInteger_ShiftRight_Negative_LargeOffset() {
            var bi = BigInteger.Parse("-" + new string('9', 100));
            var result = bi >> 500;
            Assert.NotNull(result);
        }

        [Fact]
        public void BigInteger_Large_And_VeryLarge() {
            var a = BigInteger.Parse(new string('9', 200));
            var b = BigInteger.Parse(new string('7', 200));
            var result = a & b;
            var snA = SN.BigInteger.Parse(new string('9', 200));
            var snB = SN.BigInteger.Parse(new string('7', 200));
            Assert.Equal((snA & snB).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Large_Or_VeryLarge() {
            var a = BigInteger.Parse(new string('9', 200));
            var b = BigInteger.Parse(new string('7', 200));
            var result = a | b;
            var snA = SN.BigInteger.Parse(new string('9', 200));
            var snB = SN.BigInteger.Parse(new string('7', 200));
            Assert.Equal((snA | snB).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Large_Xor_VeryLarge() {
            var a = BigInteger.Parse(new string('9', 200));
            var b = BigInteger.Parse(new string('7', 200));
            var result = a ^ b;
            var snA = SN.BigInteger.Parse(new string('9', 200));
            var snB = SN.BigInteger.Parse(new string('7', 200));
            Assert.Equal((snA ^ snB).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Large_AndNot_VeryLarge() {
            var a = BigInteger.Parse(new string('9', 200));
            var b = BigInteger.Parse(new string('7', 200));
            var result = BigMath.AndNot(a, b);
            var snA = SN.BigInteger.Parse(new string('9', 200));
            var snB = SN.BigInteger.Parse(new string('7', 200));
            Assert.Equal((snA & ~snB).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Large_Not_VeryLarge() {
            var a = BigInteger.Parse(new string('9', 200));
            var result = ~a;
            var sn = SN.BigInteger.Parse(new string('9', 200));
            Assert.Equal((~sn).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Large_Negative_And_VeryLarge() {
            var a = BigInteger.Parse("-" + new string('9', 200));
            var b = BigInteger.Parse("-" + new string('7', 200));
            var result = a & b;
            var snA = SN.BigInteger.Parse("-" + new string('9', 200));
            var snB = SN.BigInteger.Parse("-" + new string('7', 200));
            Assert.Equal((snA & snB).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Large_Negative_Or_VeryLarge() {
            var a = BigInteger.Parse("-" + new string('9', 200));
            var b = BigInteger.Parse("-" + new string('7', 200));
            var result = a | b;
            var snA = SN.BigInteger.Parse("-" + new string('9', 200));
            var snB = SN.BigInteger.Parse("-" + new string('7', 200));
            Assert.Equal((snA | snB).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Large_Negative_Xor_VeryLarge() {
            var a = BigInteger.Parse("-" + new string('9', 200));
            var b = BigInteger.Parse("-" + new string('7', 200));
            var result = a ^ b;
            var snA = SN.BigInteger.Parse("-" + new string('9', 200));
            var snB = SN.BigInteger.Parse("-" + new string('7', 200));
            Assert.Equal((snA ^ snB).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Large_MixedSign_And() {
            var a = BigInteger.Parse(new string('9', 200));
            var b = BigInteger.Parse("-" + new string('7', 200));
            var result = a & b;
            var snA = SN.BigInteger.Parse(new string('9', 200));
            var snB = SN.BigInteger.Parse("-" + new string('7', 200));
            Assert.Equal((snA & snB).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Large_MixedSign_Or() {
            var a = BigInteger.Parse(new string('9', 200));
            var b = BigInteger.Parse("-" + new string('7', 200));
            var result = a | b;
            var snA = SN.BigInteger.Parse(new string('9', 200));
            var snB = SN.BigInteger.Parse("-" + new string('7', 200));
            Assert.Equal((snA | snB).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Large_MixedSign_Xor() {
            var a = BigInteger.Parse(new string('9', 200));
            var b = BigInteger.Parse("-" + new string('7', 200));
            var result = a ^ b;
            var snA = SN.BigInteger.Parse(new string('9', 200));
            var snB = SN.BigInteger.Parse("-" + new string('7', 200));
            Assert.Equal((snA ^ snB).ToString(), result.ToString());
        }

        [Fact]
        public void BigInteger_Serialization_GetObjectData() {
            var bi = BigInteger.Parse("12345678901234567890");
            var info = new System.Runtime.Serialization.SerializationInfo(typeof(BigInteger), new System.Runtime.Serialization.FormatterConverter());
            var context = new System.Runtime.Serialization.StreamingContext();
            ((System.Runtime.Serialization.ISerializable)bi).GetObjectData(info, context);
            Assert.NotNull(info);
        }

        [Fact]
        public void BigInteger_Serialization_GetObjectData_Small() {
            var bi = BigInteger.FromInt64(12345);
            var info = new System.Runtime.Serialization.SerializationInfo(typeof(BigInteger), new System.Runtime.Serialization.FormatterConverter());
            var context = new System.Runtime.Serialization.StreamingContext();
            ((System.Runtime.Serialization.ISerializable)bi).GetObjectData(info, context);
            Assert.NotNull(info);
        }

        [Fact]
        public void BigInteger_IConvertible_ToBoolean_Throws() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Throws<NotImplementedException>(() => conv.ToBoolean(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToChar_Throws() {
            var bi = BigInteger.FromInt64(65);
            var conv = (System.IConvertible)bi;
            Assert.Throws<NotSupportedException>(() => conv.ToChar(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToSByte_Throws() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Throws<NotSupportedException>(() => conv.ToSByte(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToByte() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Equal((byte)42, conv.ToByte(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToInt16() {
            var bi = BigInteger.FromInt64(12345);
            var conv = (System.IConvertible)bi;
            Assert.Equal((short)12345, conv.ToInt16(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToUInt16_Throws() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Throws<NotSupportedException>(() => conv.ToUInt16(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToInt32() {
            var bi = BigInteger.FromInt64(1234567890);
            var conv = (System.IConvertible)bi;
            Assert.Equal(1234567890, conv.ToInt32(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToUInt32_Throws() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Throws<NotSupportedException>(() => conv.ToUInt32(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToInt64() {
            var bi = BigInteger.FromInt64(1234567890123456789);
            var conv = (System.IConvertible)bi;
            Assert.Equal(1234567890123456789L, conv.ToInt64(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToUInt64_Throws() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Throws<NotSupportedException>(() => conv.ToUInt64(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToSingle() {
            var bi = BigInteger.FromInt64(12345);
            var conv = (System.IConvertible)bi;
            Assert.Equal(12345f, conv.ToSingle(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToDouble() {
            var bi = BigInteger.FromInt64(12345);
            var conv = (System.IConvertible)bi;
            Assert.Equal(12345.0, conv.ToDouble(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToDecimal_Throws() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Throws<NotImplementedException>(() => conv.ToDecimal(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToDateTime_Throws() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Throws<NotSupportedException>(() => conv.ToDateTime(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToString_WithProvider() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Equal("42", conv.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        [Fact]
        public void BigInteger_IConvertible_ToType_ByteArray() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            var result = conv.ToType(typeof(byte[]), null);
            Assert.IsType<byte[]>(result);
        }

        [Fact]
        public void BigInteger_IConvertible_ToType_Throws() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Throws<NotSupportedException>(() => conv.ToType(typeof(DateTime), null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToType_AllFormats() {
            var bi = BigInteger.FromInt64(42);
            var conv = (System.IConvertible)bi;
            Assert.Equal((byte)42, conv.ToType(typeof(byte), null));
            Assert.Equal((short)42, conv.ToType(typeof(short), null));
            Assert.Equal(42, conv.ToType(typeof(int), null));
            Assert.Equal(42L, conv.ToType(typeof(long), null));
            Assert.Equal(42f, conv.ToType(typeof(float), null));
            Assert.Equal(42.0, conv.ToType(typeof(double), null));
            Assert.Equal("42", conv.ToType(typeof(string), null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToByte_Overflow() {
            var bi = BigInteger.FromInt64(256);
            var conv = (System.IConvertible)bi;
            Assert.Throws<InvalidCastException>(() => conv.ToByte(null));
        }

        [Fact]
        public void BigInteger_IConvertible_ToInt16_Overflow() {
            var bi = BigInteger.FromInt64(32768);
            var conv = (System.IConvertible)bi;
            Assert.Throws<InvalidCastException>(() => conv.ToInt16(null));
        }

        [Fact]
        public void BigInteger_Constructor_FromString() {
            var bi = BigInteger.Parse("12345678901234567890");
            Assert.NotNull(bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromStringNegative() {
            var bi = BigInteger.Parse("-12345678901234567890");
            Assert.NotNull(bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromByteArray() {
            var bytes = new byte[] { 0x78, 0x56, 0x34, 0x12 };
            var bi = new BigInteger(bytes);
            Assert.NotNull(bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromByteArrayNegative() {
            var bytes = new byte[] { 0x80 };
            var bi = new BigInteger(bytes);
            Assert.True(bi.Sign < 0);
        }

        [Fact]
        public void BigInteger_Constructor_FromStringRadix() {
            var bi = BigInteger.Parse("ff", 16);
            Assert.Equal(BigInteger.FromInt64(255), bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromStringRadixNegative() {
            var bi = BigInteger.Parse("-ff", 16);
            Assert.Equal(BigInteger.FromInt64(-255), bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromString_Zero() {
            var bi = BigInteger.Parse("0");
            Assert.Equal(BigInteger.Zero, bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromString_SingleDigit() {
            var bi = BigInteger.Parse("7");
            Assert.Equal(BigInteger.FromInt64(7), bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromString_NegativeSingleDigit() {
            var bi = BigInteger.Parse("-7");
            Assert.Equal(BigInteger.FromInt64(-7), bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromString_MaxInt() {
            var bi = BigInteger.Parse("2147483647");
            Assert.Equal(BigInteger.FromInt64(int.MaxValue), bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromString_MinInt() {
            var bi = BigInteger.Parse("-2147483648");
            Assert.Equal(BigInteger.FromInt64(int.MinValue), bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromString_MaxLong() {
            var bi = BigInteger.Parse("9223372036854775807");
            Assert.Equal(BigInteger.FromInt64(long.MaxValue), bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromString_MinLong() {
            var bi = BigInteger.Parse("-9223372036854775808");
            Assert.Equal(BigInteger.FromInt64(long.MinValue), bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromString_JustAboveMaxLong() {
            var bi = BigInteger.Parse("9223372036854775808");
            Assert.True(bi > BigInteger.FromInt64(long.MaxValue));
        }

        [Fact]
        public void BigInteger_Constructor_FromString_JustBelowMinLong() {
            var bi = BigInteger.Parse("-9223372036854775809");
            Assert.True(bi < BigInteger.FromInt64(long.MinValue));
        }

        [Fact]
        public void BigInteger_Constructor_FromString_LeadingZeros() {
            var bi = BigInteger.Parse("000123");
            Assert.Equal(BigInteger.FromInt64(123), bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromString_NegativeLeadingZeros() {
            var bi = BigInteger.Parse("-000123");
            Assert.Equal(BigInteger.FromInt64(-123), bi);
        }

        [Fact]
        public void BigInteger_Constructor_FromString_Invalid_Throws() {
            Assert.Throws<FormatException>(() => BigInteger.Parse("abc"));
        }

        [Fact]
        public void BigInteger_Constructor_FromString_Null_Throws() {
            Assert.Throws<FormatException>(() => BigInteger.Parse(null));
        }

        [Fact]
        public void BigInteger_Constructor_FromString_Empty_Throws() {
            Assert.Throws<FormatException>(() => BigInteger.Parse(""));
        }

        [Fact]
        public void BigInteger_Constructor_FromString_Whitespace_Throws() {
            Assert.Throws<FormatException>(() => BigInteger.Parse("  "));
        }

        [Fact]
        public void BigInteger_ToDouble_LargePositive() {
            var bi = BigInteger.Parse("123456789012345678901234567890");
            var d = bi.ToDouble();
            Assert.True(d > 0);
        }

        [Fact]
        public void BigInteger_ToDouble_LargeNegative() {
            var bi = BigInteger.Parse("-123456789012345678901234567890");
            var d = bi.ToDouble();
            Assert.True(d < 0);
        }

        [Fact]
        public void BigInteger_ToDouble_MaxValue() {
            var bi = BigInteger.Parse("1" + new string('0', 308));
            var d = bi.ToDouble();
            Assert.True(double.IsInfinity(d) || d > 0);
        }

        [Fact]
        public void BigInteger_Multiply_Large_Karatsuba() {
            var a = BigInteger.Parse("12345678901234567890");
            var b = BigInteger.Parse("9876543210987654321");
            var result = BigMath.Multiply(a, b);
            Assert.NotNull(result);
            Assert.True(result.Sign > 0);
        }

        [Fact]
        public void BigInteger_Multiply_Karatsuba_Edge() {
            var a = BigInteger.Parse("9999999999999999999");
            var b = BigInteger.Parse("9999999999999999999");
            var result = BigMath.Multiply(a, b);
            Assert.NotNull(result);
        }

        [Fact]
        public void BigInteger_Multiply_Karatsuba_Trigger() {
            var a = BigInteger.Parse(new string('9', 500));
            var b = BigInteger.Parse(new string('7', 500));
            var result = BigMath.Multiply(a, b);
            Assert.NotNull(result);
            Assert.True(result.Sign > 0);
        }

        [Fact]
        public void BigInteger_Multiply_Karatsuba_Negative() {
            var a = BigInteger.Parse("-" + new string('9', 500));
            var b = BigInteger.Parse(new string('7', 500));
            var result = BigMath.Multiply(a, b);
            Assert.NotNull(result);
            Assert.True(result.Sign < 0);
        }

        [Fact]
        public void BigInteger_Multiply_Karatsuba_BothNegative() {
            var a = BigInteger.Parse("-" + new string('9', 500));
            var b = BigInteger.Parse("-" + new string('7', 500));
            var result = BigMath.Multiply(a, b);
            Assert.NotNull(result);
            Assert.True(result.Sign > 0);
        }

        [Fact]
        public void BigInteger_ToString_MultiDigit_Radix10() {
            var bi = BigInteger.Parse("1234567890123456789012345678901234567890");
            var s = bi.ToString(10);
            Assert.Equal("1234567890123456789012345678901234567890", s);
        }

        [Fact]
        public void BigInteger_ToString_MultiDigit_Radix8() {
            var bi = BigInteger.Parse("1234567890123456789012345678901234567890");
            var s = bi.ToString(8);
            Assert.False(string.IsNullOrEmpty(s));
        }

        [Fact]
        public void BigInteger_ToString_MultiDigit_Radix2() {
            var bi = BigInteger.Parse("1234567890123456789012345678901234567890");
            var s = bi.ToString(2);
            Assert.False(string.IsNullOrEmpty(s));
        }

        [Fact]
        public void BigInteger_ToString_MultiDigit_Negative_Radix10() {
            var bi = BigInteger.Parse("-1234567890123456789012345678901234567890");
            var s = bi.ToString(10);
            Assert.Equal("-1234567890123456789012345678901234567890", s);
        }

        [Fact]
        public void BigInteger_NextProbablePrime_Large() {
            var bi = BigInteger.Parse("100000000000000000000000000000");
            var next = BigInteger.NextProbablePrime(bi);
            Assert.True(next > bi);
        }

        [Fact]
        public void BigInteger_NextProbablePrime_VeryLarge() {
            var bi = BigInteger.Parse(new string('9', 100));
            var next = BigInteger.NextProbablePrime(bi);
            Assert.True(next > bi);
        }
    }
}
