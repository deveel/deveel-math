using System;
using System.IO;

using Xunit;

namespace Deveel.Math {
	public class BigDecimalTest {
		BigInteger value = BigInteger.Parse("12345908");

		BigInteger value2 = BigInteger.Parse("12334560000");

		[Fact]
		public void ConstructorBigInteger() {
			BigDecimal big = new BigDecimal(value);
			Assert.True(big.UnscaledValue.Equals(value) && big.Scale == 0, "the BigDecimal value is not initialized properly");
		}

		[Fact]
		public void ConstructorBigIntegerScale() {
			BigDecimal big = new BigDecimal(value2, 5);
			Assert.True(big.UnscaledValue.Equals(value2) && big.Scale == 5, "the BigDecimal value is not initialized properly");
			Assert.True(big.ToString().Equals("123345.60000"), "the BigDecimal value is not represented properly");
		}

		[Fact]
		public void TestCompareToNull() {
			var big = new BigDecimal(123e04);
			Assert.NotNull(big);
			Assert.False(big == null);
		}

		[Fact]
		public void ConstructorDouble() {
			BigDecimal big = new BigDecimal(123E04);
			Assert.Equal("1230000", big.ToString());
			big = new BigDecimal(1.2345E-12);
			Assert.Equal(1.2345E-12, big.ToDouble());
			big = new BigDecimal(-12345E-3);
			Assert.Equal(-12.345, big.ToDouble());
			big = new BigDecimal(5.1234567897654321e138);
			Assert.Equal(5.1234567897654321E138, big.ToDouble());
			Assert.Equal(0, big.Scale);
			big = new BigDecimal(0.1);
			Assert.True(big.ToDouble() == 0.1, "the double representation of 0.1 bigDecimal is not correct");
			big = new BigDecimal(0.00345);
			Assert.True(big.ToDouble() == 0.00345, "the double representation of 0.00345 bigDecimal is not correct");
			// regression test for HARMONY-2429
			big = new BigDecimal(-0.0);
			Assert.True(big.Scale == 0, "the double representation of -0.0 bigDecimal is not correct");
		}

		[Fact]
		public void ParseString() {
			BigDecimal big = BigDecimal.Parse("345.23499600293850");
			Assert.True(big.ToString().Equals("345.23499600293850") && big.Scale == 14,
						  "the BigDecimal value is not initialized properly");
			big = BigDecimal.Parse("-12345");
			Assert.True(big.ToString().Equals("-12345") && big.Scale == 0, "the BigDecimal value is not initialized properly");
			big = BigDecimal.Parse("123.");
			Assert.True(big.ToString().Equals("123") && big.Scale == 0, "the BigDecimal value is not initialized properly");

			BigDecimal.Parse("1.234E02");
		}


		[Fact]
		public void ParserStringPlusExp() {
			/*
			 * BigDecimal does not support a + sign in the exponent when converting
			 * from a String
			 */
			BigDecimal.Parse("+23e-0");
			BigDecimal.Parse("-23e+0");
		}

		[Fact]
		public void ParseStringEmpty() {
			Assert.Throws<FormatException>(() => BigDecimal.Parse(""));
		}

		[Fact]
		public void ParseStringPlusMinusExp() {
			Assert.Throws<FormatException>(() => BigDecimal.Parse("+35e+-2"));
			Assert.Throws<FormatException>(() => BigDecimal.Parse("-35e-+2"));
		}

		[Fact]
		public void ParseCharArrayPlusMinusExp() {
			Assert.Throws<FormatException>(() => BigDecimal.Parse("+35e+-2".ToCharArray()));
			Assert.Throws<FormatException>(() => BigDecimal.Parse("-35e-+2".ToCharArray()));
		}

		[Fact]
		public void Abs() {
			BigDecimal big = BigDecimal.Parse("-1234");
			BigDecimal bigabs = BigMath.Abs(big);
			Assert.True(bigabs.ToString().Equals("1234"), "the absolute value of -1234 is not 1234");
			big = new BigDecimal(BigInteger.Parse("2345"), 2);
			bigabs = BigMath.Abs(big);
			Assert.True(bigabs.ToString().Equals("23.45"), "the absolute value of 23.45 is not 23.45");
		}

		[Fact]
		public void AddBigDecimal() {
			BigDecimal add1 = BigDecimal.Parse("23.456");
			BigDecimal add2 = BigDecimal.Parse("3849.235");
			BigDecimal sum = BigMath.Add(add1, add2);
			Assert.True(sum.UnscaledValue.ToString().Equals("3872691") && sum.Scale == 3,
						  "the sum of 23.456 + 3849.235 is wrong");
			Assert.True(sum.ToString().Equals("3872.691"), "the sum of 23.456 + 3849.235 is not printed correctly");
			BigDecimal add3 = new BigDecimal(12.34E02D);
			Assert.True((BigMath.Add(add1, add3)).ToString().Equals("1257.456"), "the sum of 23.456 + 12.34E02 is not printed correctly");
		}

		[Fact]
		public void CompareToBigDecimal() {
			BigDecimal comp1 = BigDecimal.Parse("1.00");
			BigDecimal comp2 = new BigDecimal(1.000000D);
			Assert.True(comp1.CompareTo(comp2) == 0, "1.00 and 1.000000 should be equal");
			BigDecimal comp3 = BigDecimal.Parse("1.02");
			Assert.True(comp3.CompareTo(comp1) == 1, "1.02 should be bigger than 1.00");
			BigDecimal comp4 = new BigDecimal(0.98D);
			Assert.True(comp4.CompareTo(comp1) == -1, "0.98 should be less than 1.00");
		}

        [Fact]
        public void CompareToBigDecimal2()
        {
            BigDecimal comp1 = BigDecimal.Parse("1.00", new MathContext(2));
            Assert.Equal(2, comp1.Precision);
            BigDecimal comp2 = new BigDecimal(100, 2);
            Assert.True(comp2.CompareTo(comp1) == 0, "1.0 and 1.00 should be equal, regardeless of the initializing math context");
            Assert.True(comp1.CompareTo(comp2) == 0, "1.00 and 1.0 should be equal, regardeless of the initializing math context");
        }

        [Fact]
        public void CompareToBigDecimal3()
        {
            BigDecimal comp1 = BigDecimal.Parse("-16.00");
            BigDecimal comp2 = new BigDecimal(-1600, 2);
            BigInteger comp3 = -16;
            Assert.True(comp1.CompareTo(comp3) == 0);
            Assert.True(comp2.CompareTo(comp3) == 0);
            Assert.True(comp1.CompareTo(comp2) == 0);
        }

        [Fact]
		public void DivideBigDecimalI() {
			BigDecimal divd1 = new BigDecimal(value, 2);
			BigDecimal divd2 = BigDecimal.Parse("2.335");
			BigDecimal divd3 = BigMath.Divide(divd1, divd2, RoundingMode.Up);
			Assert.True(divd3.ToString().Equals("52873.27") && divd3.Scale == divd1.Scale, "123459.08/2.335 is not correct");
			Assert.True(divd3.UnscaledValue.ToString().Equals("5287327"),
						  "the unscaledValue representation of 123459.08/2.335 is not correct");
			divd2 = new BigDecimal(123.4D);
			divd3 = BigMath.Divide(divd1, divd2, RoundingMode.Down);
			Assert.True(divd3.ToString().Equals("1000.47") && divd3.Scale == 2, "123459.08/123.4  is not correct");
			divd2 = new BigDecimal(000D);

			Assert.Throws<ArithmeticException>(() => BigMath. Divide(divd1, divd2, RoundingMode.Down));
		}

		[Fact]
		public void DivideBigDecimalII() {
			BigDecimal divd1 = new BigDecimal(value2, 4);
			BigDecimal divd2 = BigDecimal.Parse("0.0023");
			BigDecimal divd3 = BigMath.Divide(divd1, divd2, 3, RoundingMode.HalfUp);
			Assert.True(divd3.ToString().Equals("536285217.391") && divd3.Scale == 3, "1233456/0.0023 is not correct");
			divd2 = new BigDecimal(1345.5E-02D);
			divd3 = BigMath.Divide(divd1, divd2, 0, RoundingMode.Down);
			Assert.True(divd3.ToString().Equals("91672") && divd3.Scale == 0,
						  "1233456/13.455 is not correct or does not have the correct scale");
			divd2 = new BigDecimal(0000D);

			Assert.Throws<ArithmeticException>(() => BigMath.Divide(divd1, divd2, 4, RoundingMode.Down));
		}

		[Fact]
		public void ToDouble() {
			BigDecimal bigDB = new BigDecimal(-1.234E-112);
			//		Commenting out this part because it causes an endless loop (see HARMONY-319 and HARMONY-329)
			//		Assert.True(
			//				"the double representation of this BigDecimal is not correct",
			//				bigDB.ToDouble() == -1.234E-112);
			bigDB = new BigDecimal(5.00E-324);
			Assert.True(bigDB.ToDouble() == 5.00E-324, "the double representation of bigDecimal is not correct");
			bigDB = new BigDecimal(1.79E308);
			Assert.True(bigDB.ToDouble() == 1.79E308 && bigDB.Scale == 0,
						  "the double representation of bigDecimal is not correct");
			bigDB = new BigDecimal(-2.33E102);
			Assert.True(bigDB.ToDouble() == -2.33E102 && bigDB.Scale == 0,
						  "the double representation of bigDecimal -2.33E102 is not correct");
			bigDB = new BigDecimal(Double.MaxValue);
			bigDB = BigMath.Add(bigDB, bigDB);
			Assert.True(bigDB.ToDouble() == Double.PositiveInfinity,
						  "a  + number out of the double range should return infinity");
			bigDB = new BigDecimal(-Double.MaxValue);
			bigDB = BigMath.Add(bigDB, bigDB);
			Assert.True(bigDB.ToDouble() == Double.NegativeInfinity,
						  "a  - number out of the double range should return neg infinity");
		}

		[Fact]
		public void EqualsObject() {
			BigDecimal equal1 = new BigDecimal(1.00D);
			BigDecimal equal2 = BigDecimal.Parse("1.0");
			Assert.False(equal1.Equals(equal2), "1.00 and 1.0 should not be equal");
			equal2 = new BigDecimal(1.01D);
			Assert.False(equal1.Equals(equal2), "1.00 and 1.01 should not be equal");
			equal2 = BigDecimal.Parse("1.00");
			Assert.False(equal1.Equals(equal2), "1.00D and 1.00 should not be equal");
			BigInteger val = BigInteger.Parse("100");
			equal1 = BigDecimal.Parse("1.00");
			equal2 = new BigDecimal(val, 2);
			Assert.True(equal1.Equals(equal2), "1.00(string) and 1.00(bigInteger) should be equal");
			equal1 = new BigDecimal(100D);
			equal2 = BigDecimal.Parse("2.34576");
			Assert.False(equal1.Equals(equal2), "100D and 2.34576 should not be equal");
			Assert.False(equal1.Equals("23415"), "bigDecimal 100D does not equal string 23415");
		}

		[Fact]
		public void ToSingle() {
			BigDecimal fl1 = BigDecimal.Parse("234563782344567");
			Assert.True(fl1.ToSingle() == 234563782344567f, "the float representation of bigDecimal 234563782344567");
			BigDecimal fl2 = new BigDecimal(2.345E37);
			Assert.True(fl2.ToSingle() == 2.345E37F, "the float representation of bigDecimal 2.345E37");
			fl2 = new BigDecimal(-1.00E-44);
			Assert.True(fl2.ToSingle() == -1.00E-44F, "the float representation of bigDecimal -1.00E-44");
			fl2 = new BigDecimal(-3E12);
			Assert.True(fl2.ToSingle() == -3E12F, "the float representation of bigDecimal -3E12");
			fl2 = new BigDecimal(Double.MaxValue);
			Assert.True(fl2.ToSingle() == Single.PositiveInfinity,
						  "A number can't be represented by float should return infinity");
			fl2 = new BigDecimal(-Double.MaxValue);
			Assert.True(fl2.ToSingle() == Single.NegativeInfinity,
						  "A number can't be represented by float should return infinity");

		}


		[Fact]
		public void TestGetHashCode() {
			// anything that is equal must have the same hashCode
			BigDecimal hash = BigDecimal.Parse("1.00");
			BigDecimal hash2 = new BigDecimal(1.00D);
			Assert.True(hash.GetHashCode() != hash2.GetHashCode() && !hash.Equals(hash2),
			              "the hashCode of 1.00 and 1.00D is equal");
			hash2 = BigDecimal.Parse("1.0");
			Assert.True(hash.GetHashCode() != hash2.GetHashCode() && !hash.Equals(hash2),
			              "the hashCode of 1.0 and 1.00 is equal");
			BigInteger val = BigInteger.Parse("100");
			hash2 = new BigDecimal(val, 2);
			Assert.True(hash.GetHashCode() == hash2.GetHashCode() && hash.Equals(hash2),
			              "hashCode of 1.00 and 1.00(bigInteger) is not equal");
			hash = new BigDecimal(value, 2);
			hash2 = BigDecimal.Parse("-1233456.0000");
			Assert.True(hash.GetHashCode() != hash2.GetHashCode() && !hash.Equals(hash2),
			              "hashCode of 123459.08 and -1233456.0000 is not equal");
			hash2 = new BigDecimal(-value, 2);
			Assert.True(hash.GetHashCode() != hash2.GetHashCode() && !hash.Equals(hash2),
			              "hashCode of 123459.08 and -123459.08 is not equal");
		}

		[Fact]
		public void ToInt32() {
			BigDecimal int1 = new BigDecimal(value, 3);
			Assert.True((int)int1 == 12345, "the int value of 12345.908 is not 12345");
			int1 = BigDecimal.Parse("1.99");
			Assert.True((int)int1 == 1, "the int value of 1.99 is not 1");
			int1 = BigDecimal.Parse("23423419083091823091283933");
			// ran JDK and found representation for the above was -249268259
			Assert.True((int)int1 == -249268259, "the int value of 23423419083091823091283933 is wrong");
			int1 = new BigDecimal(-1235D);
			Assert.True((int)int1 == -1235, "the int value of -1235 is not -1235");
		}

		[Fact]
		public void ToInt64() {
			BigDecimal long1 = new BigDecimal(-value2, 0);
			Assert.True((long) long1 == -12334560000L, "the long value of 12334560000 is not 12334560000");
			long1 = new BigDecimal(-1345.348E-123D);
			Assert.True((long) long1 == 0, "the long value of -1345.348E-123D is not zero");
			long1 = BigDecimal.Parse("31323423423419083091823091283933");
			// ran JDK and found representation for the above was
			// -5251313250005125155
			Assert.True((long) long1 == -5251313250005125155L, "the long value of 31323423423419083091823091283933 is wrong");
		}

		[Fact]
		public void MaxBigDecimal() {
			BigDecimal max1 = new BigDecimal(value2, 1);
			BigDecimal max2 = new BigDecimal(value2, 4);
			Assert.True(BigMath.Max(max1, max2).Equals(max1), "1233456000.0 is not greater than 1233456");
			max1 = new BigDecimal(-1.224D);
			max2 = new BigDecimal(-1.2245D);
			Assert.True(BigMath.Max( max1, max2).Equals(max1), "-1.224 is not greater than -1.2245");
			max1 = new BigDecimal(123E18);
			max2 = new BigDecimal(123E19);
			Assert.True(BigMath.Max(max1, max2).Equals(max2), "123E19 is the not the max");
		}

		[Fact]
		public void MinBigDecimal() {
			BigDecimal min1 = new BigDecimal(-12345.4D);
			BigDecimal min2 = new BigDecimal(-12345.39D);
			Assert.True(BigMath.Min(min1, min2).Equals(min1), "-12345.39 should have been returned");
			min1 = new BigDecimal(value2, 5);
			min2 = new BigDecimal(value2, 0);
			Assert.True(BigMath.Min(min1, min2).Equals(min1), "123345.6 should have been returned");
		}

		[Fact]
		public void MovePointLeftI() {
			BigDecimal movePtLeft = BigDecimal.Parse("123456265.34");
			BigDecimal alreadyMoved = BigMath. MovePointLeft(movePtLeft, 5);
			Assert.True(alreadyMoved.Scale == 7 && alreadyMoved.ToString().Equals("1234.5626534"), "move point left 5 failed");
			movePtLeft = new BigDecimal(-value2, 0);
			alreadyMoved = BigMath.MovePointLeft(movePtLeft, 12);
			Assert.True(alreadyMoved.Scale == 12 && alreadyMoved.ToString().Equals("-0.012334560000"),
			              "move point left 12 failed");
			movePtLeft = new BigDecimal(123E18);
			alreadyMoved = BigMath.MovePointLeft(movePtLeft, 2);
			Assert.True(alreadyMoved.Scale == movePtLeft.Scale + 2 && alreadyMoved.ToDouble() == 1.23E18,
			              "move point left 2 failed");
			movePtLeft = new BigDecimal(1.123E-12);
			alreadyMoved = BigMath.MovePointLeft(movePtLeft, 3);
			Assert.True(alreadyMoved.Scale == movePtLeft.Scale + 3 && alreadyMoved.ToDouble() == 1.123E-15,
			              "move point left 3 failed");
			movePtLeft = new BigDecimal(value, 2);
			alreadyMoved = BigMath.MovePointLeft(movePtLeft, - 2);
			Assert.True(alreadyMoved.Scale == movePtLeft.Scale - 2 && alreadyMoved.ToString().Equals("12345908"),
			              "move point left -2 failed");
		}

		[Fact]
		public void MovePointRightI() {
			BigDecimal movePtRight = BigDecimal.Parse("-1.58796521458");
			BigDecimal alreadyMoved = BigMath.MovePointRight(movePtRight, 8);
			Assert.True(alreadyMoved.Scale == 3 && alreadyMoved.ToString().Equals("-158796521.458"),
			              "move point right 8 failed");
			movePtRight = new BigDecimal(value, 2);
			alreadyMoved = BigMath.MovePointRight(movePtRight, 4);
			Assert.True(alreadyMoved.Scale == 0 && alreadyMoved.ToString().Equals("1234590800"), "move point right 4 failed");
			movePtRight = new BigDecimal(134E12);
			alreadyMoved = BigMath.MovePointRight(movePtRight, 2);
			Assert.True(alreadyMoved.Scale == 0 && alreadyMoved.ToString().Equals("13400000000000000"),
			              "move point right 2 failed");
			movePtRight = new BigDecimal(-3.4E-10);
			alreadyMoved = BigMath.MovePointRight(movePtRight, 5);
			Assert.True(alreadyMoved.Scale == movePtRight.Scale - 5 && alreadyMoved.ToDouble() == -0.000034,
			              "move point right 5 failed");
			alreadyMoved = BigMath.MovePointRight(alreadyMoved, - 5);
			Assert.True(alreadyMoved.Equals(movePtRight), "move point right -5 failed");
		}

		[Fact]
		public void MultiplyBigDecimal() {
			BigDecimal multi1 = new BigDecimal(value, 5);
			BigDecimal multi2 = new BigDecimal(2.345D);
			BigDecimal result = BigMath.Multiply(multi1, multi2);
			Assert.True(result.ToString().StartsWith("289.51154260") && result.Scale == multi1.Scale + multi2.Scale,
			              "123.45908 * 2.345 is not correct: " + result);
			multi1 = BigDecimal.Parse("34656");
			multi2 = BigDecimal.Parse("-2");
			result = BigMath.Multiply(multi1, multi2);
			Assert.True(result.ToString().Equals("-69312") && result.Scale == 0, "34656 * 2 is not correct");
			multi1 = new BigDecimal(-2.345E-02);
			multi2 = new BigDecimal(-134E130);
			result = BigMath.Multiply(multi1, multi2);
			Assert.True(result.ToDouble() == 3.1422999999999997E130 && result.Scale == multi1.Scale + multi2.Scale,
			              "-2.345E-02 * -134E130 is not correct " + result.ToDouble());
			multi1 = BigDecimal.Parse("11235");
			multi2 = BigDecimal.Parse("0");
			result = BigMath.Multiply(multi1, multi2);
			Assert.True(result.ToDouble() == 0 && result.Scale == 0, "11235 * 0 is not correct");
			multi1 = BigDecimal.Parse("-0.00234");
			multi2 = new BigDecimal(13.4E10);
			result = BigMath.Multiply(multi1, multi2);
			Assert.True(result.ToDouble() == -313560000 && result.Scale == multi1.Scale + multi2.Scale,
			              "-0.00234 * 13.4E10 is not correct");
		}

		[Fact]
		public void Negate() {
			BigDecimal negate1 = new BigDecimal(value2, 7);
			Assert.True((-negate1).ToString().Equals("-1233.4560000"), "the negate of 1233.4560000 is not -1233.4560000");
			negate1 = BigDecimal.Parse("-23465839");
			Assert.True((-negate1).ToString().Equals("23465839"), "the negate of -23465839 is not 23465839");
			negate1 = new BigDecimal(-3.456E6);
			Assert.True((-(-negate1)).Equals(negate1), "the negate of -3.456E6 is not 3.456E6");
		}

		[Fact]
		public void Scale() {
			BigDecimal scale1 = new BigDecimal(value2, 8);
			Assert.True(scale1.Scale == 8, "the scale of the number 123.34560000 is wrong");
			BigDecimal scale2 = BigDecimal.Parse("29389.");
			Assert.True(scale2.Scale == 0, "the scale of the number 29389. is wrong");
			BigDecimal scale3 = new BigDecimal(3.374E13);
			Assert.True(scale3.Scale == 0, "the scale of the number 3.374E13 is wrong");
			BigDecimal scale4 = BigDecimal.Parse("-3.45E-203");
			// note the scale is calculated as 15 digits of 345000.... + exponent -
			// 1. -1 for the 3
			Assert.True(scale4.Scale == 205, "the scale of the number -3.45E-203 is wrong: " + scale4.Scale);
			scale4 = BigDecimal.Parse("-345.4E-200");
			Assert.True(scale4.Scale == 201, "the scale of the number -345.4E-200 is wrong");
		}

		[Fact]
		public void SetScaleI() {
			// rounding mode defaults to zero
			BigDecimal setScale1 = new BigDecimal(value, 3);
			BigDecimal setScale2 = BigMath.Scale(setScale1, 5);
			BigInteger setresult = BigInteger.Parse("1234590800");
			Assert.True(setScale2.UnscaledValue.Equals(setresult) && setScale2.Scale == 5,
			              "the number 12345.908 after setting scale is wrong");

			Assert.Throws<ArithmeticException>(() => BigMath.Scale(setScale1, 2, RoundingMode.Unnecessary));
		}

		[Fact]
		public void SetScaleII() {
			BigDecimal setScale1 = new BigDecimal(2.323E102);
			BigDecimal setScale2 = BigMath.Scale(setScale1, 4);
			Assert.True(setScale2.Scale == 4, "the number 2.323E102 after setting scale is wrong");
			Assert.True(setScale2.ToDouble() == 2.323E102, "the representation of the number 2.323E102 is wrong");
			setScale1 = BigDecimal.Parse("-1.253E-12");
			setScale2 = BigMath.Scale(setScale1, 17, RoundingMode.Ceiling);
			Assert.True(setScale2.Scale == 17, "the number -1.253E-12 after setting scale is wrong");
			Assert.True(setScale2.ToString().Equals("-1.25300E-12"),
			              "the representation of the number -1.253E-12 after setting scale is wrong, " + setScale2);

			// testing rounding Mode ROUND_CEILING
			setScale1 = new BigDecimal(value, 4);
			setScale2 = BigMath.Scale(setScale1, 1, RoundingMode.Ceiling);
			Assert.True(setScale2.ToString().Equals("1234.6") && setScale2.Scale == 1,
			              "the number 1234.5908 after setting scale to 1/ROUND_CEILING is wrong");
			BigDecimal setNeg = new BigDecimal(-value, 4);
			setScale2 = BigMath.Scale(setNeg, 1, RoundingMode.Ceiling);
			Assert.True(setScale2.ToString().Equals("-1234.5") && setScale2.Scale == 1,
			              "the number -1234.5908 after setting scale to 1/ROUND_CEILING is wrong");

			// testing rounding Mode ROUND_DOWN
			setScale2 = BigMath.Scale(setNeg, 1, RoundingMode.Down);
			Assert.True(setScale2.ToString().Equals("-1234.5") && setScale2.Scale == 1,
			              "the number -1234.5908 after setting scale to 1/ROUND_DOWN is wrong");
			setScale1 = new BigDecimal(value, 4);
			setScale2 = BigMath.Scale(setScale1, 1, RoundingMode.Down);
			Assert.True(setScale2.ToString().Equals("1234.5") && setScale2.Scale == 1,
			              "the number 1234.5908 after setting scale to 1/ROUND_DOWN is wrong");

			// testing rounding Mode ROUND_FLOOR
			setScale2 = BigMath.Scale(setScale1, 1, RoundingMode.Floor);
			Assert.True(setScale2.ToString().Equals("1234.5") && setScale2.Scale == 1,
			              "the number 1234.5908 after setting scale to 1/ROUND_FLOOR is wrong");
			setScale2 = BigMath.Scale(setNeg, 1, RoundingMode.Floor);
			Assert.True(setScale2.ToString().Equals("-1234.6") && setScale2.Scale == 1,
			              "the number -1234.5908 after setting scale to 1/ROUND_FLOOR is wrong");

			// testing rounding Mode ROUND_HALF_DOWN
			setScale2 = BigMath.Scale(setScale1, 3, RoundingMode.HalfDown);
			Assert.True(setScale2.ToString().Equals("1234.591") && setScale2.Scale == 3,
			              "the number 1234.5908 after setting scale to 3/ROUND_HALF_DOWN is wrong");
			setScale1 = new BigDecimal(BigInteger.Parse("12345000"), 5);
			setScale2 = BigMath.Scale(setScale1, 1, RoundingMode.HalfDown);
			Assert.True(setScale2.ToString().Equals("123.4") && setScale2.Scale == 1,
			              "the number 123.45908 after setting scale to 1/ROUND_HALF_DOWN is wrong");
			setScale2 = BigMath.Scale(BigDecimal.Parse("-1234.5000"), 0, RoundingMode.HalfDown);
			Assert.True(setScale2.ToString().Equals("-1234") && setScale2.Scale == 0,
			              "the number -1234.5908 after setting scale to 0/ROUND_HALF_DOWN is wrong");

			// testing rounding Mode ROUND_HALF_EVEN
			setScale1 = new BigDecimal(1.2345789D);
			setScale2 = BigMath.Scale(setScale1, 4, RoundingMode.HalfEven);
			Assert.True(setScale2.ToDouble() == 1.2346D && setScale2.Scale == 4,
			              "the number 1.2345789 after setting scale to 4/ROUND_HALF_EVEN is wrong");
			setNeg = new BigDecimal(-1.2335789D);
			setScale2 = BigMath.Scale(setNeg, 2, RoundingMode.HalfEven);
			Assert.True(setScale2.ToDouble() == -1.23D && setScale2.Scale == 2,
			              "the number -1.2335789 after setting scale to 2/ROUND_HALF_EVEN is wrong");
			setScale2 = BigMath.Scale(BigDecimal.Parse("1.2345000"), 3, RoundingMode.HalfEven);
			Assert.True(setScale2.ToDouble() == 1.234D && setScale2.Scale == 3,
			              "the number 1.2345789 after setting scale to 3/ROUND_HALF_EVEN is wrong");
			setScale2 = BigMath.Scale(BigDecimal.Parse("-1.2345000"), 3, RoundingMode.HalfEven);
			Assert.True(setScale2.ToDouble() == -1.234D && setScale2.Scale == 3,
			              "the number -1.2335789 after setting scale to 3/ROUND_HALF_EVEN is wrong");

			// testing rounding Mode ROUND_HALF_UP
			setScale1 = BigDecimal.Parse("134567.34650");
			setScale2 = BigMath.Scale(setScale1, 3, RoundingMode.HalfUp);
			Assert.True(setScale2.ToString().Equals("134567.347") && setScale2.Scale == 3,
			              "the number 134567.34658 after setting scale to 3/ROUND_HALF_UP is wrong");
			setNeg = BigDecimal.Parse("-1234.4567");
			setScale2 = BigMath.Scale(setNeg, 0, RoundingMode.HalfUp);
			Assert.True(setScale2.ToString().Equals("-1234") && setScale2.Scale == 0,
			              "the number -1234.4567 after setting scale to 0/ROUND_HALF_UP is wrong");

			Assert.Throws<ArithmeticException>(() => BigMath.Scale(setScale1, 3, RoundingMode.Unnecessary));

			// testing rounding Mode ROUND_UP
			setScale1 = BigDecimal.Parse("100000.374");
			setScale2 = BigMath.Scale(setScale1, 2, RoundingMode.Up);
			Assert.True(setScale2.ToString().Equals("100000.38") && setScale2.Scale == 2,
			              "the number 100000.374 after setting scale to 2/ROUND_UP is wrong");
			setNeg = new BigDecimal(-134.34589D);
			setScale2 = BigMath.Scale(setNeg, 2, RoundingMode.Up);
			Assert.True(setScale2.ToDouble() == -134.35D && setScale2.Scale == 2,
			              "the number -134.34589 after setting scale to 2/ROUND_UP is wrong");

			Assert.Throws<ArgumentException>(() => BigMath.Scale(setScale1, 0, (RoundingMode) 123));
		}

		[Fact]
		public void Signum() {
			BigDecimal sign = new BigDecimal(123E-104);
			Assert.True(sign.Sign == 1, "123E-104 is not positive in signum()");
			sign = BigDecimal.Parse("-1234.3959");
			Assert.True(sign.Sign == -1, "-1234.3959 is not negative in signum()");
			sign = new BigDecimal(000D);
			Assert.True(sign.Sign == 0, "000D is not zero in signum()");
		}

		[Fact]
		public void SubtractBigDecimal() {
			BigDecimal sub1 = BigDecimal.Parse("13948");
			BigDecimal sub2 = BigDecimal.Parse("2839.489");
			BigDecimal result =  BigMath.Subtract(sub1, sub2);
			Assert.True(result.ToString().Equals("11108.511") && result.Scale == 3, "13948 - 2839.489 is wrong: " + result);
			BigDecimal result2 = BigMath.Subtract(sub2, sub1);
			Assert.True(result2.ToString().Equals("-11108.511") && result2.Scale == 3, "2839.489 - 13948 is wrong");
			Assert.True(result.Equals(-result2), "13948 - 2839.489 is not the negative of 2839.489 - 13948");
			sub1 = new BigDecimal(value, 1);
			sub2 = BigDecimal.Parse("0");
			result = BigMath.Subtract(sub1, sub2);
			Assert.True(result.Equals(sub1), "1234590.8 - 0 is wrong");
			sub1 = new BigDecimal(1.234E-03);
			sub2 = new BigDecimal(3.423E-10);
			result = BigMath.Subtract(sub1, sub2);
			Assert.True(result.ToDouble() == 0.0012339996577, "1.234E-03 - 3.423E-10 is wrong, " + result.ToDouble());
			sub1 = new BigDecimal(1234.0123);
			sub2 = new BigDecimal(1234.0123000);
			result = BigMath.Subtract(sub1, sub2);
			Assert.True(result.ToDouble() == 0.0, "1234.0123 - 1234.0123000 is wrong, " + result.ToDouble());
		}

		[Fact]
		public void ToBigInteger() {
			BigDecimal sub1 = BigDecimal.Parse("-29830.989");
			BigInteger result = sub1.ToBigInteger();

			Assert.True(result.ToString().Equals("-29830"), "the bigInteger equivalent of -29830.989 is wrong");
			sub1 = new BigDecimal(-2837E10);
			result = sub1.ToBigInteger();
			Assert.True(result.ToDouble() == -2837E10, "the bigInteger equivalent of -2837E10 is wrong");
			sub1 = new BigDecimal(2.349E-10);
			result = sub1.ToBigInteger();
			Assert.True(result.Equals(BigInteger.Zero), "the bigInteger equivalent of 2.349E-10 is wrong");
			sub1 = new BigDecimal(value2, 6);
			result = sub1.ToBigInteger();
			Assert.True(result.ToString().Equals("12334"), "the bigInteger equivalent of 12334.560000 is wrong");
		}

		[Fact]
		public void TestToString() {
			BigDecimal toString1 = BigDecimal.Parse("1234.000");
			Assert.True(toString1.ToString().Equals("1234.000"), "the ToString representation of 1234.000 is wrong");
			toString1 = BigDecimal.Parse("-123.4E-5");
			Assert.True(toString1.ToString().Equals("-0.001234"),
			              "the ToString representation of -123.4E-5 is wrong: " + toString1);
			toString1 = BigDecimal.Parse("-1.455E-20");
			Assert.True(toString1.ToString().Equals("-1.455E-20"), "the ToString representation of -1.455E-20 is wrong");
			toString1 = new BigDecimal(value2, 4);
			Assert.True(toString1.ToString().Equals("1233456.0000"), "the ToString representation of 1233456.0000 is wrong");
		}

		[Fact]
		public void UnscaledValue() {
			BigDecimal unsVal = BigDecimal.Parse("-2839485.000");
			Assert.True(unsVal.UnscaledValue.ToString().Equals("-2839485000"), "the unscaledValue of -2839485.000 is wrong");
			unsVal = new BigDecimal(123E10);
			Assert.True(unsVal.UnscaledValue.ToString().Equals("1230000000000"), "the unscaledValue of 123E10 is wrong");
			unsVal = BigDecimal.Parse("-4.56E-13");
			Assert.True(unsVal.UnscaledValue.ToString().Equals("-456"),
			              "the unscaledValue of -4.56E-13 is wrong: " + unsVal.UnscaledValue);
			unsVal = new BigDecimal(value, 3);
			Assert.True(unsVal.UnscaledValue.ToString().Equals("12345908"), "the unscaledValue of 12345.908 is wrong");

		}

		[Fact]
		public void ValueOfJ() {
			BigDecimal valueOfL = BigDecimal.Create(9223372036854775806L);
			Assert.True(valueOfL.UnscaledValue.ToString().Equals("9223372036854775806") && valueOfL.Scale == 0,
			              "the bigDecimal equivalent of 9223372036854775806 is wrong");
			Assert.True(valueOfL.ToString().Equals("9223372036854775806"),
			              "the ToString representation of 9223372036854775806 is wrong");
			valueOfL = BigDecimal.Create(0L);
			Assert.True(valueOfL.UnscaledValue.ToString().Equals("0") && valueOfL.Scale == 0,
			              "the bigDecimal equivalent of 0 is wrong");
		}

		[Fact]
		public void ValueOfJI() {
			BigDecimal valueOfJI = BigDecimal.Create(9223372036854775806L, 5);
			Assert.True(valueOfJI.UnscaledValue.ToString().Equals("9223372036854775806") && valueOfJI.Scale == 5,
			              "the bigDecimal equivalent of 92233720368547.75806 is wrong");
			Assert.True(valueOfJI.ToString().Equals("92233720368547.75806"),
			              "the ToString representation of 9223372036854775806 is wrong");
			valueOfJI = BigDecimal.Create(1234L, 8);
			Assert.True(valueOfJI.UnscaledValue.ToString().Equals("1234") && valueOfJI.Scale == 8,
			              "the bigDecimal equivalent of 92233720368547.75806 is wrong");
			Assert.True(valueOfJI.ToString().Equals("0.00001234"),
			              "the ToString representation of 9223372036854775806 is wrong");
			valueOfJI = BigDecimal.Create(0, 3);
			Assert.True(valueOfJI.UnscaledValue.ToString().Equals("0") && valueOfJI.Scale == 3,
			              "the bigDecimal equivalent of 92233720368547.75806 is wrong");
			Assert.True(valueOfJI.ToString().Equals("0.000"), "the ToString representation of 9223372036854775806 is wrong");

		}

		[Fact]
		public void StripTrailingZero() {
			BigDecimal sixhundredtest = BigDecimal.Parse("600.0");
			Assert.True(BigMath.StripTrailingZeros(sixhundredtest).Scale == -2, "stripTrailingZero failed for 600.0");

			/* Single digit, no trailing zero, odd number */
			BigDecimal notrailingzerotest = BigDecimal.Parse("1");
			Assert.True(BigMath.StripTrailingZeros(notrailingzerotest).Scale == 0, "stripTrailingZero failed for 1");

			/* Zero */
			//regression for HARMONY-4623, NON-BUG DIFF with RI
			BigDecimal zerotest =BigDecimal.Parse("0.0000");
			Assert.True(BigMath.StripTrailingZeros(zerotest).Scale == 0, "stripTrailingZero failed for 0.0000");
		}

		[Fact]
		public void MathContextConstruction() {
			String a = "-12380945E+61";
			BigDecimal aNumber = BigDecimal.Parse(a);
			int precision = 6;
			RoundingMode rm = RoundingMode.HalfDown;
			MathContext mcIntRm = new MathContext(precision, rm);
			MathContext mcStr = MathContext.Parse("precision=6 roundingMode=HALFDOWN");
			MathContext mcInt = new MathContext(precision);
			BigDecimal res = BigMath.Abs(aNumber, mcInt);
			Assert.Equal(res, BigDecimal.Parse("1.23809E+68"));

			Assert.Equal(mcIntRm, mcStr);

			Assert.Equal(mcInt.Equals(mcStr), false);

			Assert.Equal(mcIntRm.GetHashCode(), mcStr.GetHashCode());

			Assert.Equal(mcIntRm.ToString(), "precision=6 roundingMode=HalfDown");
		}
	}
}