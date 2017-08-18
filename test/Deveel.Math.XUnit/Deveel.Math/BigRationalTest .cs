using System;
using System.Globalization;

using Xunit;
using Xunit.Extensions;

namespace Deveel.Math {
	public class BigRationalTest {
		private const String PiString =
			"3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651";

		[Fact]
		public void CastFromInt() {
			Assert.Equal(BigRational.Zero, (BigRational) 0);
			Assert.Equal(BigRational.One, (BigRational) 1);

			//assertEquals("0", valueOf(0).toString());
			//assertEquals("123", valueOf(123).toString());
			//assertEquals("-123", valueOf(-123).toString());
		}

		[Fact]
		public void ConstructRationalFromInt() {
			Assert.Equal(BigRational.Zero, new BigRational(0, 1));
			Assert.Equal(BigRational.Zero, new BigRational(0, 2));
			Assert.Equal(BigRational.Zero, new BigRational(0, -3));
			Assert.Equal(BigRational.One, new BigRational(1, 1));
			Assert.Equal(BigRational.One, BigMath.Reduce(new BigRational(2, 2)));

			Assert.Equal("0.5", new BigRational(1, 2).ToString());
			Assert.Equal(BigInteger.One, new BigRational(1, 2).Numerator.ToBigInteger());
			Assert.Equal((BigInteger) 2, new BigRational(1, 2).Denominator.ToBigInteger());

			Assert.Equal("1/2", new BigRational(1, 2).ToRationalString());
			Assert.Equal("2/4", new BigRational(2, 4).ToRationalString());
			Assert.Equal("1/2", new BigRational(2, 4).Reduce().ToRationalString()); // needs reduce
		}

		[Fact]
		public void ConstructIntegerRationalInt() {
			// since BigRational is a value-type, it cannot be compared with a "Same" assert
			// because there are no references in memory for it
			Assert.Equal(BigRational.Zero, BigRational.From(0, 0, 1));

			Assert.Equal("3.5", BigRational.From(3, 1, 2).ToString());
			Assert.Equal("-3.5", BigRational.From(-3, 1, 2).ToString());
		}

		[Fact]
		public void ConstructIntegerRationalIntDenominator0() {
			Assert.Throws<ArithmeticException>(() => BigRational.From(1, 2, 0));
		}

		[Fact]
		public void ConstructIntegerRationalIntAll0() {
			Assert.Throws<ArithmeticException>(() => BigRational.From(0, 0, 0));
		}

		[Fact]
		public void ConstructIntegerRationalIntNegativeFractionNumerator() {
			Assert.Throws<ArithmeticException>(() => BigRational.From(1, -2, 3));
		}

		[Fact]
		public void ConstructIntegerRationalIntNegativeFractionDenominator() {
			Assert.Throws<ArithmeticException>(() => BigRational.From(1, 2, -3));
		}

		[Fact]
		public void ConstructBigRationalBigInteger() {
			// since BigRational is a value-type, it cannot be compared with a "Same" assert
			// because there are no references in memory for it
			Assert.Equal(BigRational.Zero, new BigRational(BigInteger.Zero, BigInteger.One));
			Assert.Equal(BigRational.Zero, new BigRational(BigInteger.Zero, (BigInteger) 2));
			Assert.Equal(BigRational.Zero, new BigRational(BigInteger.Zero, (BigInteger) (-3)));
			Assert.Equal(BigRational.One, new BigRational(BigInteger.One, BigInteger.One));
			Assert.Equal(BigRational.One, new BigRational(BigInteger.Ten, BigInteger.Ten).Reduce()); // needs reduce

			Assert.Equal("1/10", new BigRational(BigInteger.One, BigInteger.Ten).ToRationalString());
		}

		[Fact]
		public static void ConstructRationalIntDivideByZero() {
			Assert.Throws<ArithmeticException>(() => new BigRational(3, 0));
		}

		[Fact]
		public static void ConstructFromDouble() {
			Assert.Equal(BigRational.Zero, new BigRational(0.0));
			Assert.Equal(BigRational.One, new BigRational(1.0));

			Assert.Equal("0", new BigRational(0.0).ToString());
			Assert.Equal("123", new BigRational(123).ToString());
			Assert.Equal("-123", new BigRational(-123).ToString());
			Assert.Equal("123.456", new BigRational(123.456).ToString());
			Assert.Equal("-123.456", new BigRational(-123.456).ToString());
		}

		[Fact]
		public static void ConstructDoublePositiveInfinity() {
			Assert.Throws<FormatException>(() => new BigRational(Double.PositiveInfinity));
		}

		[Fact]
		public static void ConstructDoubleNegativeInfinity() {
			Assert.Throws<FormatException>(() => new BigRational(Double.NegativeInfinity));
		}

		[Fact]
		public static void ConstructDoubleNaN() {
			Assert.Throws<FormatException>(() => new BigRational(Double.NaN));
		}

		[Fact]
		public static void ConstructBigInteger() {
			Assert.Equal(BigRational.Zero, new BigRational(BigInteger.Zero));
			Assert.Equal(BigRational.One, new BigRational(BigInteger.One));

			Assert.Equal("0", new BigRational(BigInteger.Zero).ToString());
			Assert.Equal("123", new BigRational((BigInteger)123).ToString());
			Assert.Equal("-123", new BigRational((BigInteger)(-123)).ToString());
		}

		[Fact]
		public static void ConstructBigDecimal() {
			Assert.Equal(BigRational.Zero, new BigRational(BigDecimal.Zero));
			Assert.Equal(BigRational.One, new BigRational(BigDecimal.One));

			Assert.Equal("0", new BigRational(BigDecimal.Parse("0")).ToString());
			Assert.Equal("123", new BigRational(BigDecimal.Parse("123")).ToString());
			Assert.Equal("-123", new BigRational(BigDecimal.Parse("-123")).ToString());
			Assert.Equal("123.456", new BigRational(BigDecimal.Parse("123.456")).ToString());
			Assert.Equal("-123.456", new BigRational(BigDecimal.Parse("-123.456")).ToString());
		}

		[Fact]
		public static void ConstructRationalBigDecimal() {
			Assert.Equal(BigRational.Zero, new BigRational(BigDecimal.Zero, BigDecimal.One));
			Assert.Equal(BigRational.Zero, new BigRational(BigDecimal.Zero, BigDecimal.Ten));
			Assert.Equal(BigRational.One, new BigRational(BigDecimal.One, BigDecimal.One));

			Assert.Equal("123", valueOf(new BigDecimal("123"), new BigDecimal("1")).toString());
			Assert.Equal("12.3", valueOf(new BigDecimal("123"), new BigDecimal("10")).toString());
			Assert.Equal("123", valueOf(new BigDecimal("12.3"), new BigDecimal("0.1")).toString());
			Assert.Equal("1230", valueOf(new BigDecimal("123"), new BigDecimal("0.1")).toString());
		}
	}
}