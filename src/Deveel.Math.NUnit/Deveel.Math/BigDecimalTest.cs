using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;

namespace Deveel.Math {
	[TestFixture]
	public class BigDecimalTest {
		BigInteger value = BigInteger.Parse("12345908");

		BigInteger value2 = BigInteger.Parse("12334560000");

		/**
		 * @tests java.math.BigDecimal#BigDecimal(java.math.BigInteger)
		 */
		[Test]
		public void test_ConstructorLjava_math_BigInteger() {
			BigDecimal big = new BigDecimal(value);
			Assert.IsTrue(big.UnscaledValue.Equals(value) && big.Scale == 0, "the BigDecimal value is not initialized properly");
		}

		/**
		 * @tests java.math.BigDecimal#BigDecimal(java.math.BigInteger, int)
		 */
		[Test]
		public void test_ConstructorLjava_math_BigIntegerI() {
			BigDecimal big = new BigDecimal(value2, 5);
			Assert.IsTrue(big.UnscaledValue.Equals(value2) && big.Scale == 5, "the BigDecimal value is not initialized properly");
			Assert.IsTrue(big.ToString().Equals("123345.60000"), "the BigDecimal value is not represented properly");
		}

		[Test]
		public void TestCompareToNull() {
			var big = new BigDecimal(123e04);
			Assert.IsNotNull(big);
			Assert.IsFalse(big == null);
		}

		/**
		 * @tests java.math.BigDecimal#BigDecimal(double)
		 */
		[Test]
		public void test_ConstructorD() {
			BigDecimal big = new BigDecimal(123E04);
			Assert.AreEqual("1230000", big.ToString(),
			                "the BigDecimal value taking a double argument is not initialized properly");
			big = new BigDecimal(1.2345E-12);
			Assert.AreEqual(1.2345E-12, big.ToDouble(), "the double representation is not correct");
			big = new BigDecimal(-12345E-3);
			Assert.AreEqual(-12.345, big.ToDouble(), "the double representation is not correct");
			big = new BigDecimal(5.1234567897654321e138);
			Assert.AreEqual(5.1234567897654321E138, big.ToDouble(), "the double representation is not correct");
			Assert.AreEqual(0, big.Scale, "the double representation is not correct");
			big = new BigDecimal(0.1);
			Assert.IsTrue(big.ToDouble() == 0.1, "the double representation of 0.1 bigDecimal is not correct");
			big = new BigDecimal(0.00345);
			Assert.IsTrue(big.ToDouble() == 0.00345, "the double representation of 0.00345 bigDecimal is not correct");
			// regression test for HARMONY-2429
			big = new BigDecimal(-0.0);
			Assert.IsTrue(big.Scale == 0, "the double representation of -0.0 bigDecimal is not correct");
		}

		/**
		 * @tests java.math.BigDecimal#BigDecimal(java.lang.String)
		 */
		[Test]
		public void test_ParseString() {
			BigDecimal big = BigDecimal.Parse("345.23499600293850");
			Assert.IsTrue(big.ToString().Equals("345.23499600293850") && big.Scale == 14,
						  "the BigDecimal value is not initialized properly");
			big = BigDecimal.Parse("-12345");
			Assert.IsTrue(big.ToString().Equals("-12345") && big.Scale == 0, "the BigDecimal value is not initialized properly");
			big = BigDecimal.Parse("123.");
			Assert.IsTrue(big.ToString().Equals("123") && big.Scale == 0, "the BigDecimal value is not initialized properly");

			BigDecimal.Parse("1.234E02");
		}

		/**
		 * @tests java.math.BigDecimal#BigDecimal(java.lang.String)
		 */
		[Test]
		public void test_constructor_String_plus_exp() {
			/*
			 * BigDecimal does not support a + sign in the exponent when converting
			 * from a String
			 */
			new BigDecimal(+23e-0);
			new BigDecimal(-23e+0);
		}

		/**
		 * @tests java.math.BigDecimal#BigDecimal(java.lang.String)
		 */
		[Test]
		public void test_constructor_String_empty() {
			try {
				BigDecimal.Parse("");
				Assert.Fail("FormatException expected");
			} catch (Exception e) {
			}
		}

		/**
		 * @tests java.math.BigDecimal#BigDecimal(java.lang.String)
		 */
		[Test]
		public void test_constructor_String_plus_minus_exp() {
			try {
				BigDecimal.Parse("+35e+-2");
				Assert.Fail("FormatException expected");
			} catch (FormatException e) {
			}

			try {
				BigDecimal.Parse("-35e-+2");
				Assert.Fail("NumberFormatException expected");
			} catch (FormatException e) {
			}
		}

		/**
		 * @tests java.math.BigDecimal#BigDecimal(char[])
		 */
		[Test]
		public void test_constructor_CC_plus_minus_exp() {
			try {
				BigDecimal.Parse("+35e+-2".ToCharArray());
				Assert.Fail("NumberFormatException expected");
			} catch (FormatException e) {
			}

			try {
				BigDecimal.Parse("-35e-+2".ToCharArray());
				Assert.Fail("NumberFormatException expected");
			} catch (FormatException e) {
			}
		}

		/**
		 * @tests java.math.BigDecimal#abs()
		 */
		[Test]
		public void test_abs() {
			BigDecimal big = BigDecimal.Parse("-1234");
			BigDecimal bigabs = big.Abs();
			Assert.IsTrue(bigabs.ToString().Equals("1234"), "the absolute value of -1234 is not 1234");
			big = new BigDecimal(BigInteger.Parse("2345"), 2);
			bigabs = big.Abs();
			Assert.IsTrue(bigabs.ToString().Equals("23.45"), "the absolute value of 23.45 is not 23.45");
		}

		/**
		 * @tests java.math.BigDecimal#add(java.math.BigDecimal)
		 */
		[Test]
		public void test_addLjava_math_BigDecimal() {
			BigDecimal add1 = BigDecimal.Parse("23.456");
			BigDecimal add2 = BigDecimal.Parse("3849.235");
			BigDecimal sum = add1.Add(add2);
			Assert.IsTrue(sum.UnscaledValue.ToString().Equals("3872691") && sum.Scale == 3,
						  "the sum of 23.456 + 3849.235 is wrong");
			Assert.IsTrue(sum.ToString().Equals("3872.691"), "the sum of 23.456 + 3849.235 is not printed correctly");
			BigDecimal add3 = new BigDecimal(12.34E02D);
			Assert.IsTrue((add1.Add(add3)).ToString().Equals("1257.456"), "the sum of 23.456 + 12.34E02 is not printed correctly");
		}

		/**
		 * @tests java.math.BigDecimal#compareTo(java.math.BigDecimal)
		 */
		[Test]
		public void test_compareToLjava_math_BigDecimal() {
			BigDecimal comp1 = BigDecimal.Parse("1.00");
			BigDecimal comp2 = new BigDecimal(1.000000D);
			Assert.IsTrue(comp1.CompareTo(comp2) == 0, "1.00 and 1.000000 should be equal");
			BigDecimal comp3 = BigDecimal.Parse("1.02");
			Assert.IsTrue(comp3.CompareTo(comp1) == 1, "1.02 should be bigger than 1.00");
			BigDecimal comp4 = new BigDecimal(0.98D);
			Assert.IsTrue(comp4.CompareTo(comp1) == -1, "0.98 should be less than 1.00");
		}

		/**
		 * @tests java.math.BigDecimal#divide(java.math.BigDecimal, int)
		 */
		[Test]
		public void test_divideLjava_math_BigDecimalI() {
			BigDecimal divd1 = new BigDecimal(value, 2);
			BigDecimal divd2 = BigDecimal.Parse("2.335");
			BigDecimal divd3 = divd1.Divide(divd2, RoundingMode.Up);
			Assert.IsTrue(divd3.ToString().Equals("52873.27") && divd3.Scale == divd1.Scale, "123459.08/2.335 is not correct");
			Assert.IsTrue(divd3.UnscaledValue.ToString().Equals("5287327"),
						  "the unscaledValue representation of 123459.08/2.335 is not correct");
			divd2 = new BigDecimal(123.4D);
			divd3 = divd1.Divide(divd2, RoundingMode.Down);
			Assert.IsTrue(divd3.ToString().Equals("1000.47") && divd3.Scale == 2, "123459.08/123.4  is not correct");
			divd2 = new BigDecimal(000D);

			try {
				divd1.Divide(divd2, RoundingMode.Down);
				Assert.Fail("divide by zero is not caught");
			} catch (ArithmeticException e) {
			}
		}

		/**
		 * @tests java.math.BigDecimal#divide(java.math.BigDecimal, int, int)
		 */
		[Test]
		public void test_divideLjava_math_BigDecimalII() {
			BigDecimal divd1 = new BigDecimal(value2, 4);
			BigDecimal divd2 = BigDecimal.Parse("0.0023");
			BigDecimal divd3 = divd1.Divide(divd2, 3, RoundingMode.HalfUp);
			Assert.IsTrue(divd3.ToString().Equals("536285217.391") && divd3.Scale == 3, "1233456/0.0023 is not correct");
			divd2 = new BigDecimal(1345.5E-02D);
			divd3 = divd1.Divide(divd2, 0, RoundingMode.Down);
			Assert.IsTrue(divd3.ToString().Equals("91672") && divd3.Scale == 0,
						  "1233456/13.455 is not correct or does not have the correct scale");
			divd2 = new BigDecimal(0000D);

			try {
				divd1.Divide(divd2, 4, RoundingMode.Down);
				Assert.Fail("divide by zero is not caught");
			} catch (ArithmeticException e) {
			}
		}

		/**
		 * @tests java.math.BigDecimal#doubleValue()
		 */
		[Test]
		public void test_doubleValue() {
			BigDecimal bigDB = new BigDecimal(-1.234E-112);
			//		Commenting out this part because it causes an endless loop (see HARMONY-319 and HARMONY-329)
			//		Assert.IsTrue(
			//				"the double representation of this BigDecimal is not correct",
			//				bigDB.ToDouble() == -1.234E-112);
			bigDB = new BigDecimal(5.00E-324);
			Assert.IsTrue(bigDB.ToDouble() == 5.00E-324, "the double representation of bigDecimal is not correct");
			bigDB = new BigDecimal(1.79E308);
			Assert.IsTrue(bigDB.ToDouble() == 1.79E308 && bigDB.Scale == 0,
						  "the double representation of bigDecimal is not correct");
			bigDB = new BigDecimal(-2.33E102);
			Assert.IsTrue(bigDB.ToDouble() == -2.33E102 && bigDB.Scale == 0,
						  "the double representation of bigDecimal -2.33E102 is not correct");
			bigDB = new BigDecimal(Double.MaxValue);
			bigDB = bigDB.Add(bigDB);
			Assert.IsTrue(bigDB.ToDouble() == Double.PositiveInfinity,
						  "a  + number out of the double range should return infinity");
			bigDB = new BigDecimal(-Double.MaxValue);
			bigDB = bigDB.Add(bigDB);
			Assert.IsTrue(bigDB.ToDouble() == Double.NegativeInfinity,
						  "a  - number out of the double range should return neg infinity");
		}

		/**
		 * @tests java.math.BigDecimal#equals(java.lang.Object)
		 */
		[Test]
		public void test_equalsLjava_lang_Object() {
			BigDecimal equal1 = new BigDecimal(1.00D);
			BigDecimal equal2 = BigDecimal.Parse("1.0");
			Assert.IsFalse(equal1.Equals(equal2), "1.00 and 1.0 should not be equal");
			equal2 = new BigDecimal(1.01D);
			Assert.IsFalse(equal1.Equals(equal2), "1.00 and 1.01 should not be equal");
			equal2 = BigDecimal.Parse("1.00");
			Assert.IsFalse(equal1.Equals(equal2), "1.00D and 1.00 should not be equal");
			BigInteger val = BigInteger.Parse("100");
			equal1 = BigDecimal.Parse("1.00");
			equal2 = new BigDecimal(val, 2);
			Assert.IsTrue(equal1.Equals(equal2), "1.00(string) and 1.00(bigInteger) should be equal");
			equal1 = new BigDecimal(100D);
			equal2 = BigDecimal.Parse("2.34576");
			Assert.IsFalse(equal1.Equals(equal2), "100D and 2.34576 should not be equal");
			Assert.IsFalse(equal1.Equals("23415"), "bigDecimal 100D does not equal string 23415");
		}

		/**
		 * @tests java.math.BigDecimal#floatValue()
		 */
		[Test]
		public void test_floatValue() {
			BigDecimal fl1 = BigDecimal.Parse("234563782344567");
			Assert.IsTrue(fl1.ToSingle() == 234563782344567f, "the float representation of bigDecimal 234563782344567");
			BigDecimal fl2 = new BigDecimal(2.345E37);
			Assert.IsTrue(fl2.ToSingle() == 2.345E37F, "the float representation of bigDecimal 2.345E37");
			fl2 = new BigDecimal(-1.00E-44);
			Assert.IsTrue(fl2.ToSingle() == -1.00E-44F, "the float representation of bigDecimal -1.00E-44");
			fl2 = new BigDecimal(-3E12);
			Assert.IsTrue(fl2.ToSingle() == -3E12F, "the float representation of bigDecimal -3E12");
			fl2 = new BigDecimal(Double.MaxValue);
			Assert.IsTrue(fl2.ToSingle() == Single.PositiveInfinity,
						  "A number can't be represented by float should return infinity");
			fl2 = new BigDecimal(-Double.MaxValue);
			Assert.IsTrue(fl2.ToSingle() == Single.NegativeInfinity,
						  "A number can't be represented by float should return infinity");

		}

		/**
	 * @tests java.math.BigDecimal#GetHashCode()
	 */
		[Test]
		public void test_hashCode() {
			// anything that is equal must have the same hashCode
			BigDecimal hash = BigDecimal.Parse("1.00");
			BigDecimal hash2 = new BigDecimal(1.00D);
			Assert.IsTrue(hash.GetHashCode() != hash2.GetHashCode() && !hash.Equals(hash2),
			              "the hashCode of 1.00 and 1.00D is equal");
			hash2 = BigDecimal.Parse("1.0");
			Assert.IsTrue(hash.GetHashCode() != hash2.GetHashCode() && !hash.Equals(hash2),
			              "the hashCode of 1.0 and 1.00 is equal");
			BigInteger val = BigInteger.Parse("100");
			hash2 = new BigDecimal(val, 2);
			Assert.IsTrue(hash.GetHashCode() == hash2.GetHashCode() && hash.Equals(hash2),
			              "hashCode of 1.00 and 1.00(bigInteger) is not equal");
			hash = new BigDecimal(value, 2);
			hash2 = BigDecimal.Parse("-1233456.0000");
			Assert.IsTrue(hash.GetHashCode() != hash2.GetHashCode() && !hash.Equals(hash2),
			              "hashCode of 123459.08 and -1233456.0000 is not equal");
			hash2 = new BigDecimal(value.Negate(), 2);
			Assert.IsTrue(hash.GetHashCode() != hash2.GetHashCode() && !hash.Equals(hash2),
			              "hashCode of 123459.08 and -123459.08 is not equal");
		}

		/**
		 * @tests java.math.BigDecimal#intValue()
		 */
		[Test]
		public void test_intValue() {
			BigDecimal int1 = new BigDecimal(value, 3);
			Assert.IsTrue(int1.ToInt32() == 12345, "the int value of 12345.908 is not 12345");
			int1 = BigDecimal.Parse("1.99");
			Assert.IsTrue(int1.ToInt32() == 1, "the int value of 1.99 is not 1");
			int1 = BigDecimal.Parse("23423419083091823091283933");
			// ran JDK and found representation for the above was -249268259
			Assert.IsTrue(int1.ToInt32() == -249268259, "the int value of 23423419083091823091283933 is wrong");
			int1 = new BigDecimal(-1235D);
			Assert.IsTrue(int1.ToInt32() == -1235, "the int value of -1235 is not -1235");
		}

		/**
		 * @tests java.math.BigDecimal#longValue()
		 */
		[Test]
		public void test_longValue() {
			BigDecimal long1 = new BigDecimal(value2.Negate(), 0);
			Assert.IsTrue(long1.ToInt64() == -12334560000L, "the long value of 12334560000 is not 12334560000");
			long1 = new BigDecimal(-1345.348E-123D);
			Assert.IsTrue(long1.ToInt64() == 0, "the long value of -1345.348E-123D is not zero");
			long1 = BigDecimal.Parse("31323423423419083091823091283933");
			// ran JDK and found representation for the above was
			// -5251313250005125155
			Assert.IsTrue(long1.ToInt64() == -5251313250005125155L, "the long value of 31323423423419083091823091283933 is wrong");
		}

		/**
		 * @tests java.math.BigDecimal#max(java.math.BigDecimal)
		 */
		[Test]
		public void test_maxLjava_math_BigDecimal() {
			BigDecimal max1 = new BigDecimal(value2, 1);
			BigDecimal max2 = new BigDecimal(value2, 4);
			Assert.IsTrue(max1.Max(max2).Equals(max1), "1233456000.0 is not greater than 1233456");
			max1 = new BigDecimal(-1.224D);
			max2 = new BigDecimal(-1.2245D);
			Assert.IsTrue(max1.Max(max2).Equals(max1), "-1.224 is not greater than -1.2245");
			max1 = new BigDecimal(123E18);
			max2 = new BigDecimal(123E19);
			Assert.IsTrue(max1.Max(max2).Equals(max2), "123E19 is the not the max");
		}

		/**
		 * @tests java.math.BigDecimal#min(java.math.BigDecimal)
		 */
		public void test_minLjava_math_BigDecimal() {
			BigDecimal min1 = new BigDecimal(-12345.4D);
			BigDecimal min2 = new BigDecimal(-12345.39D);
			Assert.IsTrue(min1.Min(min2).Equals(min1), "-12345.39 should have been returned");
			min1 = new BigDecimal(value2, 5);
			min2 = new BigDecimal(value2, 0);
			Assert.IsTrue(min1.Min(min2).Equals(min1), "123345.6 should have been returned");
		}

		/**
		 * @tests java.math.BigDecimal#movePointLeft(int)
		 */
		[Test]
		public void test_movePointLeftI() {
			BigDecimal movePtLeft = BigDecimal.Parse("123456265.34");
			BigDecimal alreadyMoved = movePtLeft.MovePointLeft(5);
			Assert.IsTrue(alreadyMoved.Scale == 7 && alreadyMoved.ToString().Equals("1234.5626534"), "move point left 5 failed");
			movePtLeft = new BigDecimal(value2.Negate(), 0);
			alreadyMoved = movePtLeft.MovePointLeft(12);
			Assert.IsTrue(alreadyMoved.Scale == 12 && alreadyMoved.ToString().Equals("-0.012334560000"),
			              "move point left 12 failed");
			movePtLeft = new BigDecimal(123E18);
			alreadyMoved = movePtLeft.MovePointLeft(2);
			Assert.IsTrue(alreadyMoved.Scale == movePtLeft.Scale + 2 && alreadyMoved.ToDouble() == 1.23E18,
			              "move point left 2 failed");
			movePtLeft = new BigDecimal(1.123E-12);
			alreadyMoved = movePtLeft.MovePointLeft(3);
			Assert.IsTrue(alreadyMoved.Scale == movePtLeft.Scale + 3 && alreadyMoved.ToDouble() == 1.123E-15,
			              "move point left 3 failed");
			movePtLeft = new BigDecimal(value, 2);
			alreadyMoved = movePtLeft.MovePointLeft(-2);
			Assert.IsTrue(alreadyMoved.Scale == movePtLeft.Scale - 2 && alreadyMoved.ToString().Equals("12345908"),
			              "move point left -2 failed");
		}

		/**
		 * @tests java.math.BigDecimal#movePointRight(int)
		 */
		[Test]
		public void test_movePointRightI() {
			BigDecimal movePtRight = BigDecimal.Parse("-1.58796521458");
			BigDecimal alreadyMoved = movePtRight.MovePointRight(8);
			Assert.IsTrue(alreadyMoved.Scale == 3 && alreadyMoved.ToString().Equals("-158796521.458"),
			              "move point right 8 failed");
			movePtRight = new BigDecimal(value, 2);
			alreadyMoved = movePtRight.MovePointRight(4);
			Assert.IsTrue(alreadyMoved.Scale == 0 && alreadyMoved.ToString().Equals("1234590800"), "move point right 4 failed");
			movePtRight = new BigDecimal(134E12);
			alreadyMoved = movePtRight.MovePointRight(2);
			Assert.IsTrue(alreadyMoved.Scale == 0 && alreadyMoved.ToString().Equals("13400000000000000"),
			              "move point right 2 failed");
			movePtRight = new BigDecimal(-3.4E-10);
			alreadyMoved = movePtRight.MovePointRight(5);
			Assert.IsTrue(alreadyMoved.Scale == movePtRight.Scale - 5 && alreadyMoved.ToDouble() == -0.000034,
			              "move point right 5 failed");
			alreadyMoved = alreadyMoved.MovePointRight(-5);
			Assert.IsTrue(alreadyMoved.Equals(movePtRight), "move point right -5 failed");
		}

		/**
		 * @tests java.math.BigDecimal#multiply(java.math.BigDecimal)
		 */
		[Test]
		public void test_multiplyLjava_math_BigDecimal() {
			BigDecimal multi1 = new BigDecimal(value, 5);
			BigDecimal multi2 = new BigDecimal(2.345D);
			BigDecimal result = multi1.Multiply(multi2);
			Assert.IsTrue(result.ToString().StartsWith("289.51154260") && result.Scale == multi1.Scale + multi2.Scale,
			              "123.45908 * 2.345 is not correct: " + result);
			multi1 = BigDecimal.Parse("34656");
			multi2 = BigDecimal.Parse("-2");
			result = multi1.Multiply(multi2);
			Assert.IsTrue(result.ToString().Equals("-69312") && result.Scale == 0, "34656 * 2 is not correct");
			multi1 = new BigDecimal(-2.345E-02);
			multi2 = new BigDecimal(-134E130);
			result = multi1.Multiply(multi2);
			Assert.IsTrue(result.ToDouble() == 3.1422999999999997E130 && result.Scale == multi1.Scale + multi2.Scale,
			              "-2.345E-02 * -134E130 is not correct " + result.ToDouble());
			multi1 = BigDecimal.Parse("11235");
			multi2 = BigDecimal.Parse("0");
			result = multi1.Multiply(multi2);
			Assert.IsTrue(result.ToDouble() == 0 && result.Scale == 0, "11235 * 0 is not correct");
			multi1 = BigDecimal.Parse("-0.00234");
			multi2 = new BigDecimal(13.4E10);
			result = multi1.Multiply(multi2);
			Assert.IsTrue(result.ToDouble() == -313560000 && result.Scale == multi1.Scale + multi2.Scale,
			              "-0.00234 * 13.4E10 is not correct");
		}

		/**
		 * @tests java.math.BigDecimal#negate()
		 */
		[Test]
		public void test_negate() {
			BigDecimal negate1 = new BigDecimal(value2, 7);
			Assert.IsTrue(negate1.Negate().ToString().Equals("-1233.4560000"), "the negate of 1233.4560000 is not -1233.4560000");
			negate1 = BigDecimal.Parse("-23465839");
			Assert.IsTrue(negate1.Negate().ToString().Equals("23465839"), "the negate of -23465839 is not 23465839");
			negate1 = new BigDecimal(-3.456E6);
			Assert.IsTrue(negate1.Negate().Negate().Equals(negate1), "the negate of -3.456E6 is not 3.456E6");
		}

		/**
		 * @tests java.math.BigDecimal#scale()
		 */
		[Test]
		public void test_scale() {
			BigDecimal scale1 = new BigDecimal(value2, 8);
			Assert.IsTrue(scale1.Scale == 8, "the scale of the number 123.34560000 is wrong");
			BigDecimal scale2 = BigDecimal.Parse("29389.");
			Assert.IsTrue(scale2.Scale == 0, "the scale of the number 29389. is wrong");
			BigDecimal scale3 = new BigDecimal(3.374E13);
			Assert.IsTrue(scale3.Scale == 0, "the scale of the number 3.374E13 is wrong");
			BigDecimal scale4 = BigDecimal.Parse("-3.45E-203");
			// note the scale is calculated as 15 digits of 345000.... + exponent -
			// 1. -1 for the 3
			Assert.IsTrue(scale4.Scale == 205, "the scale of the number -3.45E-203 is wrong: " + scale4.Scale);
			scale4 = BigDecimal.Parse("-345.4E-200");
			Assert.IsTrue(scale4.Scale == 201, "the scale of the number -345.4E-200 is wrong");
		}

		/**
		 * @tests java.math.BigDecimal#setScale(int)
		 */
		[Test]
		public void test_setScaleI() {
			// rounding mode defaults to zero
			BigDecimal setScale1 = new BigDecimal(value, 3);
			BigDecimal setScale2 = setScale1.SetScale(5);
			BigInteger setresult = BigInteger.Parse("1234590800");
			Assert.IsTrue(setScale2.UnscaledValue.Equals(setresult) && setScale2.Scale == 5,
			              "the number 12345.908 after setting scale is wrong");

			try {
				setScale2 = setScale1.SetScale(2, RoundingMode.Unnecessary);
				Assert.Fail("arithmetic Exception not caught as a result of loosing precision");
			} catch (ArithmeticException e) {
			}
		}

		/**
		 * @tests java.math.BigDecimal#setScale(int, int)
		 */
		[Test]
		public void test_setScaleII() {
			BigDecimal setScale1 = new BigDecimal(2.323E102);
			BigDecimal setScale2 = setScale1.SetScale(4);
			Assert.IsTrue(setScale2.Scale == 4, "the number 2.323E102 after setting scale is wrong");
			Assert.IsTrue(setScale2.ToDouble() == 2.323E102, "the representation of the number 2.323E102 is wrong");
			setScale1 = BigDecimal.Parse("-1.253E-12");
			setScale2 = setScale1.SetScale(17, RoundingMode.Ceiling);
			Assert.IsTrue(setScale2.Scale == 17, "the number -1.253E-12 after setting scale is wrong");
			Assert.IsTrue(setScale2.ToString().Equals("-1.25300E-12"),
			              "the representation of the number -1.253E-12 after setting scale is wrong, " + setScale2);

			// testing rounding Mode ROUND_CEILING
			setScale1 = new BigDecimal(value, 4);
			setScale2 = setScale1.SetScale(1, RoundingMode.Ceiling);
			Assert.IsTrue(setScale2.ToString().Equals("1234.6") && setScale2.Scale == 1,
			              "the number 1234.5908 after setting scale to 1/ROUND_CEILING is wrong");
			BigDecimal setNeg = new BigDecimal(value.Negate(), 4);
			setScale2 = setNeg.SetScale(1, RoundingMode.Ceiling);
			Assert.IsTrue(setScale2.ToString().Equals("-1234.5") && setScale2.Scale == 1,
			              "the number -1234.5908 after setting scale to 1/ROUND_CEILING is wrong");

			// testing rounding Mode ROUND_DOWN
			setScale2 = setNeg.SetScale(1, RoundingMode.Down);
			Assert.IsTrue(setScale2.ToString().Equals("-1234.5") && setScale2.Scale == 1,
			              "the number -1234.5908 after setting scale to 1/ROUND_DOWN is wrong");
			setScale1 = new BigDecimal(value, 4);
			setScale2 = setScale1.SetScale(1, RoundingMode.Down);
			Assert.IsTrue(setScale2.ToString().Equals("1234.5") && setScale2.Scale == 1,
			              "the number 1234.5908 after setting scale to 1/ROUND_DOWN is wrong");

			// testing rounding Mode ROUND_FLOOR
			setScale2 = setScale1.SetScale(1, RoundingMode.Floor);
			Assert.IsTrue(setScale2.ToString().Equals("1234.5") && setScale2.Scale == 1,
			              "the number 1234.5908 after setting scale to 1/ROUND_FLOOR is wrong");
			setScale2 = setNeg.SetScale(1, RoundingMode.Floor);
			Assert.IsTrue(setScale2.ToString().Equals("-1234.6") && setScale2.Scale == 1,
			              "the number -1234.5908 after setting scale to 1/ROUND_FLOOR is wrong");

			// testing rounding Mode ROUND_HALF_DOWN
			setScale2 = setScale1.SetScale(3, RoundingMode.HalfDown);
			Assert.IsTrue(setScale2.ToString().Equals("1234.591") && setScale2.Scale == 3,
			              "the number 1234.5908 after setting scale to 3/ROUND_HALF_DOWN is wrong");
			setScale1 = new BigDecimal(BigInteger.Parse("12345000"), 5);
			setScale2 = setScale1.SetScale(1, RoundingMode.HalfDown);
			Assert.IsTrue(setScale2.ToString().Equals("123.4") && setScale2.Scale == 1,
			              "the number 123.45908 after setting scale to 1/ROUND_HALF_DOWN is wrong");
			setScale2 = BigDecimal.Parse("-1234.5000").SetScale(0, RoundingMode.HalfDown);
			Assert.IsTrue(setScale2.ToString().Equals("-1234") && setScale2.Scale == 0,
			              "the number -1234.5908 after setting scale to 0/ROUND_HALF_DOWN is wrong");

			// testing rounding Mode ROUND_HALF_EVEN
			setScale1 = new BigDecimal(1.2345789D);
			setScale2 = setScale1.SetScale(4, RoundingMode.HalfEven);
			Assert.IsTrue(setScale2.ToDouble() == 1.2346D && setScale2.Scale == 4,
			              "the number 1.2345789 after setting scale to 4/ROUND_HALF_EVEN is wrong");
			setNeg = new BigDecimal(-1.2335789D);
			setScale2 = setNeg.SetScale(2, RoundingMode.HalfEven);
			Assert.IsTrue(setScale2.ToDouble() == -1.23D && setScale2.Scale == 2,
			              "the number -1.2335789 after setting scale to 2/ROUND_HALF_EVEN is wrong");
			setScale2 = BigDecimal.Parse("1.2345000").SetScale(3,
					RoundingMode.HalfEven);
			Assert.IsTrue(setScale2.ToDouble() == 1.234D && setScale2.Scale == 3,
			              "the number 1.2345789 after setting scale to 3/ROUND_HALF_EVEN is wrong");
			setScale2 = BigDecimal.Parse("-1.2345000").SetScale(3,
					RoundingMode.HalfEven);
			Assert.IsTrue(setScale2.ToDouble() == -1.234D && setScale2.Scale == 3,
			              "the number -1.2335789 after setting scale to 3/ROUND_HALF_EVEN is wrong");

			// testing rounding Mode ROUND_HALF_UP
			setScale1 = BigDecimal.Parse("134567.34650");
			setScale2 = setScale1.SetScale(3, RoundingMode.HalfUp);
			Assert.IsTrue(setScale2.ToString().Equals("134567.347") && setScale2.Scale == 3,
			              "the number 134567.34658 after setting scale to 3/ROUND_HALF_UP is wrong");
			setNeg = BigDecimal.Parse("-1234.4567");
			setScale2 = setNeg.SetScale(0, RoundingMode.HalfUp);
			Assert.IsTrue(setScale2.ToString().Equals("-1234") && setScale2.Scale == 0,
			              "the number -1234.4567 after setting scale to 0/ROUND_HALF_UP is wrong");

			// testing rounding Mode ROUND_UNNECESSARY
			try {
				setScale1.SetScale(3, RoundingMode.Unnecessary);
				Assert.Fail("arithmetic Exception not caught for round unnecessary");
			} catch (ArithmeticException e) {
			}

			// testing rounding Mode ROUND_UP
			setScale1 = BigDecimal.Parse("100000.374");
			setScale2 = setScale1.SetScale(2, RoundingMode.Up);
			Assert.IsTrue(setScale2.ToString().Equals("100000.38") && setScale2.Scale == 2,
			              "the number 100000.374 after setting scale to 2/ROUND_UP is wrong");
			setNeg = new BigDecimal(-134.34589D);
			setScale2 = setNeg.SetScale(2, RoundingMode.Up);
			Assert.IsTrue(setScale2.ToDouble() == -134.35D && setScale2.Scale == 2,
			              "the number -134.34589 after setting scale to 2/ROUND_UP is wrong");

			// testing invalid rounding modes
			try {
				setScale2 = setScale1.SetScale(0, -123);
				Assert.Fail("ArgumentException is not caught for wrong rounding mode");
			} catch (ArgumentException e) {
			}
		}

		/**
		 * @tests java.math.BigDecimal#signum()
		 */
		[Test]
		public void test_signum() {
			BigDecimal sign = new BigDecimal(123E-104);
			Assert.IsTrue(sign.Sign == 1, "123E-104 is not positive in signum()");
			sign = BigDecimal.Parse("-1234.3959");
			Assert.IsTrue(sign.Sign == -1, "-1234.3959 is not negative in signum()");
			sign = new BigDecimal(000D);
			Assert.IsTrue(sign.Sign == 0, "000D is not zero in signum()");
		}

		/**
		 * @tests java.math.BigDecimal#subtract(java.math.BigDecimal)
		 */
		[Test]
		public void test_subtractLjava_math_BigDecimal() {
			BigDecimal sub1 = BigDecimal.Parse("13948");
			BigDecimal sub2 = BigDecimal.Parse("2839.489");
			BigDecimal result = sub1.Subtract(sub2);
			Assert.IsTrue(result.ToString().Equals("11108.511") && result.Scale == 3, "13948 - 2839.489 is wrong: " + result);
			BigDecimal result2 = sub2.Subtract(sub1);
			Assert.IsTrue(result2.ToString().Equals("-11108.511") && result2.Scale == 3, "2839.489 - 13948 is wrong");
			Assert.IsTrue(result.Equals(result2.Negate()), "13948 - 2839.489 is not the negative of 2839.489 - 13948");
			sub1 = new BigDecimal(value, 1);
			sub2 = BigDecimal.Parse("0");
			result = sub1.Subtract(sub2);
			Assert.IsTrue(result.Equals(sub1), "1234590.8 - 0 is wrong");
			sub1 = new BigDecimal(1.234E-03);
			sub2 = new BigDecimal(3.423E-10);
			result = sub1.Subtract(sub2);
			Assert.IsTrue(result.ToDouble() == 0.0012339996577, "1.234E-03 - 3.423E-10 is wrong, " + result.ToDouble());
			sub1 = new BigDecimal(1234.0123);
			sub2 = new BigDecimal(1234.0123000);
			result = sub1.Subtract(sub2);
			Assert.IsTrue(result.ToDouble() == 0.0, "1234.0123 - 1234.0123000 is wrong, " + result.ToDouble());
		}

		/**
		 * @tests java.math.BigDecimal#toBigInteger()
		 */
		[Test]
		public void test_toBigInteger() {
			BigDecimal sub1 = BigDecimal.Parse("-29830.989");
			BigInteger result = sub1.ToBigInteger();

			Assert.IsTrue(result.ToString().Equals("-29830"), "the bigInteger equivalent of -29830.989 is wrong");
			sub1 = new BigDecimal(-2837E10);
			result = sub1.ToBigInteger();
			Assert.IsTrue(result.ToDouble() == -2837E10, "the bigInteger equivalent of -2837E10 is wrong");
			sub1 = new BigDecimal(2.349E-10);
			result = sub1.ToBigInteger();
			Assert.IsTrue(result.Equals(BigInteger.Zero), "the bigInteger equivalent of 2.349E-10 is wrong");
			sub1 = new BigDecimal(value2, 6);
			result = sub1.ToBigInteger();
			Assert.IsTrue(result.ToString().Equals("12334"), "the bigInteger equivalent of 12334.560000 is wrong");
		}

		/**
		 * @tests java.math.BigDecimal#ToString()
		 */
		[Test]
		public void test_toString() {
			BigDecimal toString1 = BigDecimal.Parse("1234.000");
			Assert.IsTrue(toString1.ToString().Equals("1234.000"), "the ToString representation of 1234.000 is wrong");
			toString1 = BigDecimal.Parse("-123.4E-5");
			Assert.IsTrue(toString1.ToString().Equals("-0.001234"),
			              "the ToString representation of -123.4E-5 is wrong: " + toString1);
			toString1 = BigDecimal.Parse("-1.455E-20");
			Assert.IsTrue(toString1.ToString().Equals("-1.455E-20"), "the ToString representation of -1.455E-20 is wrong");
			toString1 = new BigDecimal(value2, 4);
			Assert.IsTrue(toString1.ToString().Equals("1233456.0000"), "the ToString representation of 1233456.0000 is wrong");
		}

		/**
		 * @tests java.math.BigDecimal#unscaledValue()
		 */
		[Test]
		public void test_unscaledValue() {
			BigDecimal unsVal = BigDecimal.Parse("-2839485.000");
			Assert.IsTrue(unsVal.UnscaledValue.ToString().Equals("-2839485000"), "the unscaledValue of -2839485.000 is wrong");
			unsVal = new BigDecimal(123E10);
			Assert.IsTrue(unsVal.UnscaledValue.ToString().Equals("1230000000000"), "the unscaledValue of 123E10 is wrong");
			unsVal = BigDecimal.Parse("-4.56E-13");
			Assert.IsTrue(unsVal.UnscaledValue.ToString().Equals("-456"),
			              "the unscaledValue of -4.56E-13 is wrong: " + unsVal.UnscaledValue);
			unsVal = new BigDecimal(value, 3);
			Assert.IsTrue(unsVal.UnscaledValue.ToString().Equals("12345908"), "the unscaledValue of 12345.908 is wrong");

		}

		/**
		 * @tests java.math.BigDecimal#valueOf(long)
		 */
		[Test]
		public void test_valueOfJ() {
			BigDecimal valueOfL = BigDecimal.ValueOf(9223372036854775806L);
			Assert.IsTrue(valueOfL.UnscaledValue.ToString().Equals("9223372036854775806") && valueOfL.Scale == 0,
			              "the bigDecimal equivalent of 9223372036854775806 is wrong");
			Assert.IsTrue(valueOfL.ToString().Equals("9223372036854775806"),
			              "the ToString representation of 9223372036854775806 is wrong");
			valueOfL = BigDecimal.ValueOf(0L);
			Assert.IsTrue(valueOfL.UnscaledValue.ToString().Equals("0") && valueOfL.Scale == 0,
			              "the bigDecimal equivalent of 0 is wrong");
		}

		/**
		 * @tests java.math.BigDecimal#valueOf(long, int)
		 */
		[Test]
		public void test_valueOfJI() {
			BigDecimal valueOfJI = BigDecimal.ValueOf(9223372036854775806L, 5);
			Assert.IsTrue(valueOfJI.UnscaledValue.ToString().Equals("9223372036854775806") && valueOfJI.Scale == 5,
			              "the bigDecimal equivalent of 92233720368547.75806 is wrong");
			Assert.IsTrue(valueOfJI.ToString().Equals("92233720368547.75806"),
			              "the ToString representation of 9223372036854775806 is wrong");
			valueOfJI = BigDecimal.ValueOf(1234L, 8);
			Assert.IsTrue(valueOfJI.UnscaledValue.ToString().Equals("1234") && valueOfJI.Scale == 8,
			              "the bigDecimal equivalent of 92233720368547.75806 is wrong");
			Assert.IsTrue(valueOfJI.ToString().Equals("0.00001234"),
			              "the ToString representation of 9223372036854775806 is wrong");
			valueOfJI = BigDecimal.ValueOf(0, 3);
			Assert.IsTrue(valueOfJI.UnscaledValue.ToString().Equals("0") && valueOfJI.Scale == 3,
			              "the bigDecimal equivalent of 92233720368547.75806 is wrong");
			Assert.IsTrue(valueOfJI.ToString().Equals("0.000"), "the ToString representation of 9223372036854775806 is wrong");

		}

		[Test]
		public void test_BigDecimal_serialization() {
			// Regression for HARMONY-1896
			char[] input = { '1', '5', '6', '7', '8', '7', '.', '0', '0' };
			BigDecimal bd = BigDecimal.Parse(input, 0, 9);

			MemoryStream bos = new MemoryStream();
			BinaryFormatter oos = new BinaryFormatter();
			oos.Serialize(bos, bd);

			MemoryStream bis = new MemoryStream(bos.ToArray());
			BigDecimal nbd = (BigDecimal)oos.Deserialize(bis);

			Assert.AreEqual(bd.ToInt32(), nbd.ToInt32());
			Assert.AreEqual(bd.ToDouble(), nbd.ToDouble(), 0.0);
			Assert.AreEqual(bd.ToString(), nbd.ToString());
		}

		/**
		 * @tests java.math.BigDecimal#stripTrailingZero(long)
		 */
		[Test]
		public void test_stripTrailingZero() {
			BigDecimal sixhundredtest = BigDecimal.Parse("600.0");
			Assert.IsTrue(((sixhundredtest.StripTrailingZeros()).Scale == -2), "stripTrailingZero failed for 600.0");

			/* Single digit, no trailing zero, odd number */
			BigDecimal notrailingzerotest = BigDecimal.Parse("1");
			Assert.IsTrue(((notrailingzerotest.StripTrailingZeros()).Scale == 0), "stripTrailingZero failed for 1");

			/* Zero */
			//regression for HARMONY-4623, NON-BUG DIFF with RI
			BigDecimal zerotest =BigDecimal.Parse("0.0000");
			Assert.IsTrue(((zerotest.StripTrailingZeros()).Scale == 0), "stripTrailingZero failed for 0.0000");
		}

		[Test]
		public void testMathContextConstruction() {
			String a = "-12380945E+61";
			BigDecimal aNumber = BigDecimal.Parse(a);
			int precision = 6;
			RoundingMode rm = RoundingMode.HalfDown;
			MathContext mcIntRm = new MathContext(precision, rm);
			MathContext mcStr = new MathContext("precision=6 roundingMode=HALFDOWN");
			MathContext mcInt = new MathContext(precision);
			BigDecimal res = aNumber.Abs(mcInt);
			Assert.AreEqual(res, BigDecimal.Parse("1.23809E+68"), "MathContext Constructor with int precision failed");

			Assert.AreEqual(mcIntRm, mcStr, "Equal MathContexts are not Equal ");

			Assert.AreEqual(mcInt.Equals(mcStr), false, "Different MathContext are reported as Equal ");

			Assert.AreEqual(mcIntRm.GetHashCode(), mcStr.GetHashCode(), "Equal MathContexts have different hashcodes ");

			Assert.AreEqual(mcIntRm.ToString(), "precision=6 roundingMode=HalfDown",
			                "MathContext.ToString() returning incorrect value");
		}
	}
}