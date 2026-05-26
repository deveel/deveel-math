using System;
using Xunit;
using SN = System.Numerics;

namespace Deveel.Math {
    public class CrossValidationTests {
        private static readonly long[] TestValues = {
            0, 1, -1, 2, -2, 3, -3, 0xFFFFFFFFL, -0xFFFFFFFFL,
            0x7FFFFFFFL, -0x7FFFFFFFL, 0x80000000L, -0x80000000L,
            0x1FFFFFFFFL, -0x1FFFFFFFFL,
            0x7FFFFFFFFFFFFFFFL, -0x7FFFFFFFFFFFFFFFL,
            long.MaxValue, long.MinValue + 1
        };

        public static System.Collections.Generic.IEnumerable<object[]> GetPairs() {
            foreach (var a in TestValues)
                foreach (var b in TestValues)
                    if (a != b)
                        yield return new object[] { a, b };
        }

        [Theory]
        [MemberData(nameof(GetPairs))]
        public void CrossValidate_And(long a, long b) {
            var biA = BigInteger.FromInt64(a);
            var biB = BigInteger.FromInt64(b);
            var snA = new SN.BigInteger(a);
            var snB = new SN.BigInteger(b);
            var expected = snA & snB;
            var actual = biA & biB;
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [MemberData(nameof(GetPairs))]
        public void CrossValidate_Or(long a, long b) {
            var biA = BigInteger.FromInt64(a);
            var biB = BigInteger.FromInt64(b);
            var snA = new SN.BigInteger(a);
            var snB = new SN.BigInteger(b);
            var expected = snA | snB;
            var actual = biA | biB;
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [MemberData(nameof(GetPairs))]
        public void CrossValidate_Xor(long a, long b) {
            var biA = BigInteger.FromInt64(a);
            var biB = BigInteger.FromInt64(b);
            var snA = new SN.BigInteger(a);
            var snB = new SN.BigInteger(b);
            var expected = snA ^ snB;
            var actual = biA ^ biB;
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [MemberData(nameof(GetPairs))]
        public void CrossValidate_AndNot(long a, long b) {
            var biA = BigInteger.FromInt64(a);
            var biB = BigInteger.FromInt64(b);
            var snA = new SN.BigInteger(a);
            var snB = new SN.BigInteger(b);
            var expected = snA & ~snB;
            var actual = BigMath.AndNot(biA, biB);
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(2)]
        [InlineData(-2)]
        [InlineData(0x7FFFFFFF)]
        [InlineData(-0x7FFFFFFF)]
        [InlineData(0x7FFFFFFFFFFFFFFF)]
        [InlineData(-0x7FFFFFFFFFFFFFFF)]
        public void CrossValidate_Not(long v) {
            var bi = BigInteger.FromInt64(v);
            var sn = new SN.BigInteger(v);
            var expected = ~sn;
            var actual = ~bi;
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void CrossValidate_Large_And() {
            var a = BigInteger.Parse("123456789012345678901234567890");
            var b = BigInteger.Parse("-987654321098765432109876543210");
            var snA = SN.BigInteger.Parse("123456789012345678901234567890");
            var snB = SN.BigInteger.Parse("-987654321098765432109876543210");
            Assert.Equal((snA & snB).ToString(), (a & b).ToString());
        }

        [Fact]
        public void CrossValidate_Large_Or() {
            var a = BigInteger.Parse("123456789012345678901234567890");
            var b = BigInteger.Parse("-987654321098765432109876543210");
            var snA = SN.BigInteger.Parse("123456789012345678901234567890");
            var snB = SN.BigInteger.Parse("-987654321098765432109876543210");
            Assert.Equal((snA | snB).ToString(), (a | b).ToString());
        }

        [Fact]
        public void CrossValidate_Large_Xor() {
            var a = BigInteger.Parse("123456789012345678901234567890");
            var b = BigInteger.Parse("-987654321098765432109876543210");
            var snA = SN.BigInteger.Parse("123456789012345678901234567890");
            var snB = SN.BigInteger.Parse("-987654321098765432109876543210");
            Assert.Equal((snA ^ snB).ToString(), (a ^ b).ToString());
        }

        [Fact]
        public void CrossValidate_Large_AndNot() {
            var a = BigInteger.Parse("123456789012345678901234567890");
            var b = BigInteger.Parse("-987654321098765432109876543210");
            var snA = SN.BigInteger.Parse("123456789012345678901234567890");
            var snB = SN.BigInteger.Parse("-987654321098765432109876543210");
            Assert.Equal((snA & ~snB).ToString(), BigMath.AndNot(a, b).ToString());
        }

        [Fact]
        public void CrossValidate_Large_Not() {
            var a = BigInteger.Parse("-123456789012345678901234567890");
            var sn = SN.BigInteger.Parse("-123456789012345678901234567890");
            Assert.Equal((~sn).ToString(), (~a).ToString());
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(1, 31)]
        [InlineData(1, 32)]
        [InlineData(1, 63)]
        [InlineData(1, 100)]
        [InlineData(-1, 1)]
        [InlineData(-1, 32)]
        [InlineData(-1, 63)]
        [InlineData(0x7FFFFFFF, 1)]
        [InlineData(0x7FFFFFFF, 5)]
        [InlineData(0x80000000L, 1)]
        [InlineData(long.MaxValue, 1)]
        [InlineData(long.MinValue + 1, 1)]
        public void CrossValidate_ShiftLeft(long v, int n) {
            var bi = BigInteger.FromInt64(v);
            var sn = new SN.BigInteger(v);
            var expected = sn << n;
            var actual = bi << n;
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [InlineData(256, 4)]
        [InlineData(-256, 4)]
        [InlineData(0x7FFFFFFF, 4)]
        [InlineData(-0x80000000L, 4)]
        [InlineData(long.MaxValue, 10)]
        [InlineData(long.MinValue + 1, 10)]
        public void CrossValidate_ShiftRight(long v, int n) {
            var bi = BigInteger.FromInt64(v);
            var sn = new SN.BigInteger(v);
            var expected = sn >> n;
            var actual = bi >> n;
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(0x7FFFFFFF)]
        [InlineData(-0x7FFFFFFF)]
        [InlineData(0x80000000L)]
        [InlineData(-0x80000000L)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue + 1)]
        public void CrossValidate_Abs(long v) {
            var bi = BigInteger.FromInt64(v);
            var sn = new SN.BigInteger(v);
            Assert.Equal(SN.BigInteger.Abs(sn).ToString(), BigMath.Abs(bi).ToString());
        }

        [Fact]
        public void CrossValidate_ToString_MultiDigit() {
            var vals = new[] { "4294967296", "-4294967296", "12345678901234567890", "-12345678901234567890" };
            var radixes = new[] { 2, 10, 16 };
            foreach (var s in vals) {
                var bi = BigInteger.Parse(s);
                foreach (int r in radixes) {
                    var str = bi.ToString(r);
                    Assert.False(string.IsNullOrEmpty(str));
                }
            }
        }

        [Fact]
        public void BigInteger_Shift_CrossValidate() {
            var vals = new[] { 1L, 42L, -42L, 0x7FFFFFFFL, -0x80000000L, long.MaxValue, long.MinValue };
            foreach (var v in vals) {
                var bi = BigInteger.FromInt64(v);
                var sn = (SN.BigInteger)bi;
                var shifted1 = (BigInteger)(sn << 5);
                var shifted2 = bi << 5;
                Assert.Equal(shifted1.ToString(), shifted2.ToString());
            }
        }
    }
}
