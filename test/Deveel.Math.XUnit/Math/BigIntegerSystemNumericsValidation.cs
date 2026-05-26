using System;
using System.Linq;
using Xunit;
using SNBigInteger = System.Numerics.BigInteger;

namespace Deveel.Math {
    public class BigIntegerSystemNumericsValidation {
        private const int Seed = 42;
        private const int Iterations = 100;

        private static Random CreateRandom() => new Random(Seed);

        private static BigInteger RandomDmathBigInteger(Random rnd, int bits) {
            int byteLen = (bits + 7) / 8;
            byte[] bytes = new byte[byteLen + 1];
            rnd.NextBytes(bytes);
            bytes[byteLen] = 0;
            return new BigInteger(bytes);
        }

        private static SNBigInteger RandomSysBigInteger(Random rnd, int bits) {
            int byteLen = (bits + 7) / 8;
            byte[] bytes = new byte[byteLen + 1];
            rnd.NextBytes(bytes);
            bytes[byteLen] = 0;
            return new SNBigInteger(bytes);
        }

        [Fact]
        public void Add_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var b = RandomDmathBigInteger(rnd, 128);
                var sysA = a.ToSystemBigInteger();
                var sysB = b.ToSystemBigInteger();

                var result = (a + b).ToSystemBigInteger();
                var expected = sysA + sysB;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void Subtract_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var b = RandomDmathBigInteger(rnd, 128);
                var sysA = a.ToSystemBigInteger();
                var sysB = b.ToSystemBigInteger();

                var result = (a - b).ToSystemBigInteger();
                var expected = sysA - sysB;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void Multiply_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var b = RandomDmathBigInteger(rnd, 128);
                var sysA = a.ToSystemBigInteger();
                var sysB = b.ToSystemBigInteger();

                var result = (a * b).ToSystemBigInteger();
                var expected = sysA * sysB;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void Divide_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var b = RandomDmathBigInteger(rnd, 128);
                if (b.Sign == 0) continue;
                var a = RandomDmathBigInteger(rnd, 256);
                var sysA = a.ToSystemBigInteger();
                var sysB = b.ToSystemBigInteger();

                var result = (a / b).ToSystemBigInteger();
                var expected = sysA / sysB;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void Remainder_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var b = RandomDmathBigInteger(rnd, 128);
                if (b.Sign == 0) continue;
                var a = RandomDmathBigInteger(rnd, 256);
                var sysA = a.ToSystemBigInteger();
                var sysB = b.ToSystemBigInteger();

                var result = (a % b).ToSystemBigInteger();
                var expected = sysA % sysB;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void Negate_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var sysA = a.ToSystemBigInteger();

                var result = (-a).ToSystemBigInteger();
                var expected = -sysA;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void Abs_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var sysA = a.ToSystemBigInteger();

                var result = BigMath.Abs(a).ToSystemBigInteger();
                var expected = SNBigInteger.Abs(sysA);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void Pow_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < 20; i++) {
                var a = RandomDmathBigInteger(rnd, 32);
                int exp = rnd.Next(0, 10);
                var sysA = a.ToSystemBigInteger();

                var result = BigMath.Pow(a, exp).ToSystemBigInteger();
                var expected = SNBigInteger.Pow(sysA, exp);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void ShiftLeft_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 128);
                int shift = rnd.Next(1, 64);
                var sysA = a.ToSystemBigInteger();

                var result = (a << shift).ToSystemBigInteger();
                var expected = sysA << shift;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void ShiftRight_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 128);
                int shift = rnd.Next(1, 64);
                var sysA = a.ToSystemBigInteger();

                var result = (a >> shift).ToSystemBigInteger();
                var expected = sysA >> shift;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void BitwiseAnd_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var b = RandomDmathBigInteger(rnd, 128);
                var sysA = a.ToSystemBigInteger();
                var sysB = b.ToSystemBigInteger();

                var result = (a & b).ToSystemBigInteger();
                var expected = sysA & sysB;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void BitwiseOr_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var b = RandomDmathBigInteger(rnd, 128);
                var sysA = a.ToSystemBigInteger();
                var sysB = b.ToSystemBigInteger();

                var result = (a | b).ToSystemBigInteger();
                var expected = sysA | sysB;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void BitwiseXor_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var b = RandomDmathBigInteger(rnd, 128);
                var sysA = a.ToSystemBigInteger();
                var sysB = b.ToSystemBigInteger();

                var result = (a ^ b).ToSystemBigInteger();
                var expected = sysA ^ sysB;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void BitwiseNot_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var sysA = a.ToSystemBigInteger();

                var result = (~a).ToSystemBigInteger();
                var expected = ~sysA;

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void ToInt64_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 63);
                var sysA = a.ToSystemBigInteger();
                if (sysA > long.MaxValue || sysA < long.MinValue) continue;

                Assert.Equal((long)sysA, a.ToInt64());
            }
        }

        [Fact]
        public void ToInt32_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 31);
                var sysA = a.ToSystemBigInteger();
                if (sysA > int.MaxValue || sysA < int.MinValue) continue;

                Assert.Equal((int)sysA, a.ToInt32());
            }
        }

        [Fact]
        public void Comparison_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var b = RandomDmathBigInteger(rnd, 256);
                var sysA = a.ToSystemBigInteger();
                var sysB = b.ToSystemBigInteger();

                Assert.Equal(sysA < sysB, a < b);
                Assert.Equal(sysA > sysB, a > b);
                Assert.Equal(sysA <= sysB, a <= b);
                Assert.Equal(sysA >= sysB, a >= b);
                Assert.Equal(sysA == sysB, a == b);
                Assert.Equal(sysA != sysB, a != b);
            }
        }

        [Fact]
        public void Parse_ShouldMatchSystemNumerics() {
            var rnd = CreateRandom();
            for (int i = 0; i < Iterations; i++) {
                var a = RandomDmathBigInteger(rnd, 256);
                var sysA = a.ToSystemBigInteger();

                var parsed = BigInteger.Parse(sysA.ToString());
                var sysParsed = SNBigInteger.Parse(a.ToString());

                Assert.Equal(sysA, parsed.ToSystemBigInteger());
                Assert.Equal(sysParsed, SNBigInteger.Parse(parsed.ToString()));
            }
        }
    }
}
