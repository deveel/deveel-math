using System;

using NUnit.Framework;

namespace Deveel.Math {
	[TestFixture]
	public class BigIntegerTest {
		public BigIntegerTest() {
			twoToTheSeventy = two.Pow(70);
		}

		private BigInteger minusTwo = new BigInteger("-2", 10);

		private BigInteger minusOne = new BigInteger("-1", 10);

		private BigInteger zero = new BigInteger("0", 10);

		private BigInteger one = new BigInteger("1", 10);

		private BigInteger two = new BigInteger("2", 10);

		private BigInteger ten = new BigInteger("10", 10);

		private BigInteger sixteen = new BigInteger("16", 10);

		private BigInteger oneThousand = new BigInteger("1000", 10);

		private BigInteger aZillion = new BigInteger("100000000000000000000000000000000000000000000000000", 10);

		private BigInteger twoToTheTen = new BigInteger("1024", 10);

		private BigInteger twoToTheSeventy;

		private Random rand = new Random();

		private BigInteger bi;

		private BigInteger bi1;

		private BigInteger bi2;

		private BigInteger bi3;

		private BigInteger bi11;

		private BigInteger bi22;

		private BigInteger bi33;

		private BigInteger bi12;

		private BigInteger bi23;

		private BigInteger bi13;

		private BigInteger largePos;

		private BigInteger smallPos;

		private BigInteger largeNeg;

		private BigInteger smallNeg;

		private BigInteger[][] booleanPairs;

		[TestFixtureSetUp]
		public void SetUp() {
			bi1 = new BigInteger("2436798324768978", 16);
			bi2 = new BigInteger("4576829475724387584378543764555", 16);
			bi3 = new BigInteger("43987298363278574365732645872643587624387563245",
					16);

			bi33 = new BigInteger(
					"10730846694701319120609898625733976090865327544790136667944805934175543888691400559249041094474885347922769807001",
					10);
			bi22 = new BigInteger(
					"33301606932171509517158059487795669025817912852219962782230629632224456249",
					10);
			bi11 = new BigInteger("6809003003832961306048761258711296064", 10);
			bi23 = new BigInteger(
					"597791300268191573513888045771594235932809890963138840086083595706565695943160293610527214057",
					10);
			bi13 = new BigInteger(
					"270307912162948508387666703213038600031041043966215279482940731158968434008",
					10);
			bi12 = new BigInteger(
					"15058244971895641717453176477697767050482947161656458456", 10);

			largePos = new BigInteger(
					"834759814379857314986743298675687569845986736578576375675678998612743867438632986243982098437620983476924376",
					16);
			smallPos = new BigInteger("48753269875973284765874598630960986276", 16);
			largeNeg = new BigInteger(
					"-878824397432651481891353247987891423768534321387864361143548364457698487264387568743568743265873246576467643756437657436587436",
					16);
			smallNeg = new BigInteger("-567863254343798609857456273458769843", 16);
			booleanPairs = new BigInteger[4][];
			booleanPairs[0] = new BigInteger[] { largePos, smallPos };
			booleanPairs[1] = new BigInteger[] { largePos, smallNeg };
			booleanPairs[2] = new BigInteger[] { largeNeg, smallPos };
			booleanPairs[3] = new BigInteger[] { largeNeg, smallNeg };
			/*
			booleanPairs = new BigInteger[][] { { largePos, smallPos },
				{ largePos, smallNeg }, { largeNeg, smallPos },
				{ largeNeg, smallNeg } };
			*/
		}

		/**
 * @tests java.math.BigInteger#BigInteger(int, java.util.Random)
 */
		[Test]
		public void test_ConstructorILjava_util_Random() {
			// regression test for HARMONY-1047
			try {
				new BigInteger(Int32.MaxValue, (Random)null);
				Assert.Fail("OverflowException expected");
			} catch (OverflowException e) {
				// PASSED
			}

			bi = new BigInteger(70, rand);
			bi2 = new BigInteger(70, rand);
			Assert.IsTrue(bi.CompareTo(zero) >= 0, "Random number is negative");
			Assert.IsTrue(bi.CompareTo(twoToTheSeventy) < 0, "Random number is too big");
			Assert.IsTrue(!bi.Equals(bi2), "Two random numbers in a row are the same (might not be a bug but it very likely is)");
			Assert.IsTrue(new BigInteger(0, rand).Equals(BigInteger.Zero), "Not zero");
		}

		/**
 * @tests java.math.BigInteger#BigInteger(int, int, java.util.Random)
 */
		[Test]
		public void test_ConstructorIILjava_util_Random() {
			bi = new BigInteger(10, 5, rand);
			bi2 = new BigInteger(10, 5, rand);
			Assert.IsTrue(bi.CompareTo(zero) >= 0, "Random number one is negative");
			Assert.IsTrue(bi.CompareTo(twoToTheTen) < 0, "Random number one is too big");
			Assert.IsTrue(bi2.CompareTo(zero) >= 0, "Random number two is negative");
			Assert.IsTrue(bi2.CompareTo(twoToTheTen) < 0, "Random number two is too big");

			Random rand_b = new Random();
			BigInteger bi_b;
			int[] certainty = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, Int32.MinValue, Int32.MinValue + 1, -2, -1 };
			for (int i = 2; i <= 20; i++) {
				for (int c = 0; c < certainty.Length; c++) {
					bi_b = new BigInteger(i, c, rand_b); // Create BigInteger
					Assert.IsTrue(bi_b.BitLength == i, "Bit length incorrect");
				}
			}
		}

		/**
 * @tests java.math.BigInteger#BigInteger(byte[])
 */
		[Test]
		public void test_Constructor_B() {
			byte[] myByteArray;
			myByteArray = new byte[] { (byte)0x00, (byte)0xFF, (byte)0xFE };
			bi = new BigInteger(myByteArray);
			Assert.IsTrue(bi.Equals(BigInteger.Zero.SetBit(16).Subtract(two)), "Incorrect value for pos number");
			myByteArray = new byte[] { (byte)0xFF, (byte)0xFE };
			bi = new BigInteger(myByteArray);
			Assert.IsTrue(bi.Equals(minusTwo), "Incorrect value for neg number");
		}

		/**
		 * @tests java.math.BigInteger#BigInteger(int, byte[])
		 */
		[Test]
		public void test_ConstructorI_B() {
			byte[] myByteArray;
			myByteArray = new byte[] { (byte)0xFF, (byte)0xFE };
			bi = new BigInteger(1, myByteArray);
			Assert.IsTrue(bi.Equals(BigInteger.Zero.SetBit(16).Subtract(two)), "Incorrect value for pos number");
			bi = new BigInteger(-1, myByteArray);
			Assert.IsTrue(bi.Equals(BigInteger.Zero.SetBit(16).Subtract(two).Negate()), "Incorrect value for neg number");
			myByteArray = new byte[] { (byte)0, (byte)0 };
			bi = new BigInteger(0, myByteArray);
			Assert.IsTrue(bi.Equals(zero), "Incorrect value for zero");
			myByteArray = new byte[] { (byte)1 };
			try {
				new BigInteger(0, myByteArray);
				Assert.Fail("Failed to throw FormatException");
			} catch (FormatException e) {
				// correct
			}
		}

		/**
		 * @tests java.math.BigInteger#BigInteger(java.lang.String)
		 */
		[Test]
		public void test_constructor_String_empty() {
			try {
				new BigInteger("");
				Assert.Fail("Expected NumberFormatException for new BigInteger(\"\")");
			} catch (FormatException e) {
			}
		}

		/**
		 * @tests java.math.BigInteger#toByteArray()
		 */
		[Test]
		public void test_toByteArray() {
			byte[] myByteArray, anotherByteArray;
			myByteArray = new byte[] { 97, 33, 120, 124, 50, 2, 0, 0, 0, 12, 124,
				42 };
			anotherByteArray = new BigInteger(myByteArray).ToByteArray();
			Assert.IsTrue(myByteArray.Length == anotherByteArray.Length, "Incorrect byte array returned");
			for (int counter = myByteArray.Length - 1; counter >= 0; counter--) {
				Assert.IsTrue(myByteArray[counter] == anotherByteArray[counter], "Incorrect values in returned byte array");
			}
		}

		/**
		 * @tests java.math.BigInteger#isProbablePrime(int)
		 */
		[Test]
		public void test_isProbablePrimeI() {
			int fails = 0;
			bi = new BigInteger(20, 20, rand);
			if (!bi.IsProbablePrime(17)) {
				fails++;
			}
			bi = new BigInteger("4", 10);
			if (bi.IsProbablePrime(17)) {
				Assert.Fail("isProbablePrime failed for: " + bi.ToString());
			}
			bi = BigInteger.ValueOf(17L * 13L);
			if (bi.IsProbablePrime(17)) {
				Assert.Fail("isProbablePrime failed for: " + bi.ToString());
			}
			for (long a = 2; a < 1000; a++) {
				if (isPrime(a)) {
					Assert.IsTrue(BigInteger.ValueOf(a).IsProbablePrime(5), "false negative on prime number <1000");
				} else if (BigInteger.ValueOf(a).IsProbablePrime(17)) {
					Console.Out.WriteLine("isProbablePrime failed for: " + a);
					fails++;
				}
			}
			for (int a = 0; a < 1000; a++) {
				bi = BigInteger.ValueOf(rand.Next(1000000)).Multiply(BigInteger.ValueOf(rand.Next(1000000)));
				if (bi.IsProbablePrime(17)) {
					Console.Out.WriteLine("isProbablePrime failed for: " + bi.ToString());
					fails++;
				}
			}
			for (int a = 0; a < 200; a++) {
				bi = new BigInteger(70, rand).Multiply(new BigInteger(70, rand));
				if (bi.IsProbablePrime(17)) {
					Console.Out.WriteLine("isProbablePrime failed for: " + bi.ToString());
					fails++;
				}
			}
			Assert.IsTrue(fails <= 1, "Too many false positives - may indicate a problem");
		}

		/**
		 * @tests java.math.BigInteger#equals(java.lang.Object)
		 */
		[Test]
		public void test_equalsLjava_lang_Object() {
			Assert.IsTrue(zero.Equals(BigInteger.ValueOf(0)), "0=0");
			Assert.IsTrue(BigInteger.ValueOf(-123).Equals(BigInteger.ValueOf(-123)), "-123=-123");
			Assert.IsTrue(!zero.Equals(one), "0=1");
			Assert.IsTrue(!zero.Equals(minusOne), "0=-1");
			Assert.IsTrue(!one.Equals(minusOne), "1=-1");
			Assert.IsTrue(bi3.Equals(bi3), "bi3=bi3");
			Assert.IsTrue(bi3.Equals(bi3.Negate().Negate()), "bi3=copy of bi3");
			Assert.IsTrue(!bi3.Equals(bi2), "bi3=bi2");
		}

		/**
		 * @tests java.math.BigInteger#compareTo(java.math.BigInteger)
		 */
		[Test]
		public void test_compareToLjava_math_BigInteger() {
			Assert.IsTrue(one.CompareTo(two) < 0, "Smaller number returned >= 0");
			Assert.IsTrue(two.CompareTo(one) > 0, "Larger number returned >= 0");
			Assert.IsTrue(one.CompareTo(one) == 0, "Equal numbers did not return 0");
			Assert.IsTrue(two.Negate().CompareTo(one) < 0, "Neg number messed things up");
		}

		/**
		 * @tests java.math.BigInteger#intValue()
		 */
		[Test]
		public void test_intValue() {
			Assert.IsTrue(twoToTheSeventy.ToInt32() == 0, "Incorrect ToInt32 for 2**70");
			Assert.IsTrue(two.ToInt32() == 2, "Incorrect ToInt32 for 2");
		}

		/**
		 * @tests java.math.BigInteger#longValue()
		 */
		[Test]
		public void test_longValue() {
			Assert.IsTrue(twoToTheSeventy.ToInt64() == 0, "Incorrect ToInt64 for 2**70");
			Assert.IsTrue(two.ToInt64() == 2, "Incorrect ToInt64 for 2");
		}

		/**
		 * @tests java.math.BigInteger#valueOf(long)
		 */
		[Test]
		public void test_valueOfJ() {
			Assert.IsTrue(BigInteger.ValueOf(2L).Equals(two), "Incurred number returned for 2");
			Assert.IsTrue(BigInteger.ValueOf(200L).Equals(BigInteger.ValueOf(139).Add(BigInteger.ValueOf(61))),
						  "Incurred number returned for 200");
		}

		/**
		 * @tests java.math.BigInteger#add(java.math.BigInteger)
		 */
		[Test]
		public void test_addLjava_math_BigInteger() {
			Assert.IsTrue(aZillion.Add(aZillion).Add(aZillion.Negate()).Equals(aZillion), "Incorrect sum--wanted a zillion");
			Assert.IsTrue(zero.Add(zero).Equals(zero), "0+0");
			Assert.IsTrue(zero.Add(one).Equals(one), "0+1");
			Assert.IsTrue(one.Add(zero).Equals(one), "1+0");
			Assert.IsTrue(one.Add(one).Equals(two), "1+1");
			Assert.IsTrue(zero.Add(minusOne).Equals(minusOne), "0+(-1)");
			Assert.IsTrue(minusOne.Add(zero).Equals(minusOne), "(-1)+0");
			Assert.IsTrue(minusOne.Add(minusOne).Equals(minusTwo), "(-1)+(-1)");
			Assert.IsTrue(one.Add(minusOne).Equals(zero), "1+(-1)");
			Assert.IsTrue(minusOne.Add(one).Equals(zero), "(-1)+1");

			for (int i = 0; i < 200; i++) {
				BigInteger midbit = zero.SetBit(i);
				Assert.IsTrue(midbit.Add(midbit).Equals(zero.SetBit(i + 1)), "add fails to carry on bit " + i);
			}
			BigInteger bi2p3 = bi2.Add(bi3);
			BigInteger bi3p2 = bi3.Add(bi2);
			Assert.IsTrue(bi2p3.Equals(bi3p2), "bi2p3=bi3p2");

			// add large positive + small positive

			// add large positive + small negative

			// add large negative + small positive

			// add large negative + small negative
		}

		/**
		 * @tests java.math.BigInteger#negate()
		 */
		[Test]
		public void test_negate() {
			Assert.IsTrue(zero.Negate().Equals(zero), "Single negation of zero did not result in zero");
			Assert.IsTrue(!aZillion.Negate().Equals(aZillion), "Single negation resulted in original nonzero number");
			Assert.IsTrue(aZillion.Negate().Negate().Equals(aZillion), "Double negation did not result in original number");

			Assert.IsTrue(zero.Negate().Equals(zero), "0.neg");
			Assert.IsTrue(one.Negate().Equals(minusOne), "1.neg");
			Assert.IsTrue(two.Negate().Equals(minusTwo), "2.neg");
			Assert.IsTrue(minusOne.Negate().Equals(one), "-1.neg");
			Assert.IsTrue(minusTwo.Negate().Equals(two), "-2.neg");
			Assert.IsTrue(
				unchecked(BigInteger.ValueOf(0x62EB40FEF85AA9EBL*2).Negate().Equals(BigInteger.ValueOf(-0x62EB40FEF85AA9EBL*2))),
				"0x62EB40FEF85AA9EBL*2.neg");
			for (int i = 0; i < 200; i++) {
				BigInteger midbit = zero.SetBit(i);
				BigInteger negate = midbit.Negate();
				Assert.IsTrue(negate.Negate().Equals(midbit), "negate negate");
				Assert.IsTrue(midbit.Negate().Add(midbit).Equals(zero), "neg fails on bit " + i);
			}
		}

		/**
		 * @tests java.math.BigInteger#signum()
		 */
		[Test]
		public void test_signum() {
			Assert.IsTrue(two.Sign == 1, "Wrong positive signum");
			Assert.IsTrue(zero.Sign == 0, "Wrong zero signum");
			Assert.IsTrue(zero.Negate().Sign == 0, "Wrong neg zero signum");
			Assert.IsTrue(two.Negate().Sign == -1, "Wrong neg signum");
		}

		/**
		 * @tests java.math.BigInteger#abs()
		 */
		[Test]
		public void test_abs() {
			Assert.IsTrue(aZillion.Negate().Abs().Equals(aZillion.Abs()), "Invalid number returned for zillion");
			Assert.IsTrue(zero.Negate().Abs().Equals(zero), "Invalid number returned for zero neg");
			Assert.IsTrue(zero.Abs().Equals(zero), "Invalid number returned for zero");
			Assert.IsTrue(two.Negate().Abs().Equals(two), "Invalid number returned for two");
		}

		/**
		 * @tests java.math.BigInteger#pow(int)
		 */
		[Test]
		public void test_powI() {
			Assert.IsTrue(two.Pow(10).Equals(twoToTheTen), "Incorrect exponent returned for 2**10");
			Assert.IsTrue(two.Pow(30).Multiply(two.Pow(40)).Equals(twoToTheSeventy), "Incorrect exponent returned for 2**70");
			Assert.IsTrue(ten.Pow(50).Equals(aZillion), "Incorrect exponent returned for 10**50");
		}

		/**
		 * @tests java.math.BigInteger#modInverse(java.math.BigInteger)
		 */
		[Test]
		public void test_modInverseLjava_math_BigInteger() {
			BigInteger a = zero, mod, inv;
			for (int j = 3; j < 50; j++) {
				mod = BigInteger.ValueOf(j);
				for (int i = -j + 1; i < j; i++) {
					try {
						a = BigInteger.ValueOf(i);
						inv = a.ModInverse(mod);
						Assert.IsTrue(one.Equals(a.Multiply(inv).Mod(mod)), "bad inverse: " + a + " inv mod " + mod + " equals " + inv);
						Assert.IsTrue(inv.CompareTo(mod) < 0, "inverse greater than modulo: " + a + " inv mod " + mod + " equals " + inv);
						Assert.IsTrue(inv.CompareTo(BigInteger.Zero) >= 0, "inverse less than zero: " + a + " inv mod " + mod + " equals " + inv);
					} catch (ArithmeticException) {
						Assert.IsTrue(!one.Equals(a.Gcd(mod)), "should have found inverse for " + a + " mod " + mod);
					}
				}
			}
			for (int j = 1; j < 10; j++) {
				mod = bi2.Add(BigInteger.ValueOf(j));
				for (int i = 0; i < 20; i++) {
					try {
						a = bi3.Add(BigInteger.ValueOf(i));
						inv = a.ModInverse(mod);
						Assert.IsTrue(one.Equals(a.Multiply(inv).Mod(mod)), "bad inverse: " + a + " inv mod " + mod + " equals " + inv);
						Assert.IsTrue(inv.CompareTo(mod) < 0, "inverse greater than modulo: " + a + " inv mod " + mod + " equals " + inv);
						Assert.IsTrue(inv.CompareTo(BigInteger.Zero) >= 0, "inverse less than zero: " + a + " inv mod " + mod + " equals " + inv);
					} catch (ArithmeticException e) {
						Assert.IsTrue(!one.Equals(a.Gcd(mod)), "should have found inverse for " + a + " mod " + mod);
					}
				}
			}
		}

		/**
		 * @tests java.math.BigInteger#shiftRight(int)
		 */
		[Test]
		public void test_shiftRightI() {
			Assert.IsTrue(BigInteger.ValueOf(1).ShiftRight(0).Equals(BigInteger.One), "1 >> 0");
			Assert.IsTrue(BigInteger.ValueOf(1).ShiftRight(1).Equals(BigInteger.Zero), "1 >> 1");
			Assert.IsTrue(BigInteger.ValueOf(1).ShiftRight(63).Equals(BigInteger.Zero), "1 >> 63");
			Assert.IsTrue(BigInteger.ValueOf(1).ShiftRight(64).Equals(BigInteger.Zero), "1 >> 64");
			Assert.IsTrue(BigInteger.ValueOf(1).ShiftRight(65).Equals(BigInteger.Zero), "1 >> 65");
			Assert.IsTrue(BigInteger.ValueOf(1).ShiftRight(1000).Equals(BigInteger.Zero), "1 >> 1000");
			Assert.IsTrue(BigInteger.ValueOf(-1).ShiftRight(0).Equals(minusOne), "-1 >> 0");
			Assert.IsTrue(BigInteger.ValueOf(-1).ShiftRight(1).Equals(minusOne), "-1 >> 1");
			Assert.IsTrue(BigInteger.ValueOf(-1).ShiftRight(63).Equals(minusOne), "-1 >> 63");
			Assert.IsTrue(BigInteger.ValueOf(-1).ShiftRight(64).Equals(minusOne), "-1 >> 64");
			Assert.IsTrue(BigInteger.ValueOf(-1).ShiftRight(65).Equals(minusOne), "-1 >> 65");
			Assert.IsTrue(BigInteger.ValueOf(-1).ShiftRight(1000).Equals(minusOne), "-1 >> 1000");

			BigInteger a = BigInteger.One;
			BigInteger c = bi3;
			BigInteger E = bi3.Negate();
			BigInteger e = E;
			for (int i = 0; i < 200; i++) {
				BigInteger b = BigInteger.Zero.SetBit(i);
				Assert.IsTrue(a.Equals(b), "a==b");
				a = a.ShiftLeft(1);
				Assert.IsTrue(a.Sign >= 0, "a non-neg");

				BigInteger d = bi3.ShiftRight(i);
				Assert.IsTrue(c.Equals(d), "c==d");
				c = c.ShiftRight(1);
				Assert.IsTrue(d.Divide(two).Equals(c), ">>1 == /2");
				Assert.IsTrue(c.Sign >= 0, "c non-neg");

				BigInteger f = E.ShiftRight(i);
				Assert.IsTrue(e.Equals(f), "e==f");
				e = e.ShiftRight(1);
				Assert.IsTrue(f.Subtract(one).Divide(two).Equals(e), ">>1 == /2");
				Assert.IsTrue(e.Sign == -1, "e negative");

				Assert.IsTrue(b.ShiftRight(i).Equals(one), "b >> i");
				Assert.IsTrue(b.ShiftRight(i + 1).Equals(zero), "b >> i+1");
				Assert.IsTrue(b.ShiftRight(i - 1).Equals(two), "b >> i-1");
			}
		}

		/**
		 * @tests java.math.BigInteger#shiftLeft(int)
		 */
		[Test]
		public void test_shiftLeftI() {
			Assert.IsTrue(one.ShiftLeft(0).Equals(one), "1 << 0");
			Assert.IsTrue(one.ShiftLeft(1).Equals(two), "1 << 1");
			Assert.IsTrue(one.ShiftLeft(63).Equals(new BigInteger("8000000000000000", 16)), "1 << 63");
			Assert.IsTrue(one.ShiftLeft(64).Equals(new BigInteger("10000000000000000", 16)), "1 << 64");
			Assert.IsTrue(one.ShiftLeft(65).Equals(new BigInteger("20000000000000000", 16)), "1 << 65");
			Assert.IsTrue(minusOne.ShiftLeft(0).Equals(minusOne), "-1 << 0");
			Assert.IsTrue(minusOne.ShiftLeft(1).Equals(minusTwo), "-1 << 1");
			Assert.IsTrue(minusOne.ShiftLeft(63).Equals(new BigInteger("-9223372036854775808")), "-1 << 63");
			Assert.IsTrue(minusOne.ShiftLeft(64).Equals(new BigInteger("-18446744073709551616")), "-1 << 64");
			Assert.IsTrue(minusOne.ShiftLeft(65).Equals(new BigInteger("-36893488147419103232")), "-1 << 65");

			BigInteger a = bi3;
			BigInteger c = minusOne;
			for (int i = 0; i < 200; i++) {
				BigInteger b = bi3.ShiftLeft(i);
				Assert.IsTrue(a.Equals(b), "a==b");
				Assert.IsTrue(a.ShiftRight(i).Equals(bi3), "a >> i == bi3");
				a = a.ShiftLeft(1);
				Assert.IsTrue(b.Multiply(two).Equals(a), "<<1 == *2");
				Assert.IsTrue(a.Sign >= 0, "a non-neg");
				Assert.IsTrue(a.BitCount == b.BitCount, "a.bitCount==b.bitCount");

				BigInteger d = minusOne.ShiftLeft(i);
				Assert.IsTrue(c.Equals(d), "c==d");
				c = c.ShiftLeft(1);
				Assert.IsTrue(d.Multiply(two).Equals(c), "<<1 == *2 negative");
				Assert.IsTrue(c.Sign == -1, "c negative");
				Assert.IsTrue(d.ShiftRight(i).Equals(minusOne), "d >> i == minusOne");
			}
		}

		/**
		 * @tests java.math.BigInteger#multiply(java.math.BigInteger)
		 */
		[Test]
		public void test_multiplyLjava_math_BigInteger() {
            SetUp();
			Assert.IsTrue(aZillion.Add(aZillion).Add(aZillion).Equals(aZillion.Multiply(new BigInteger("3", 10))),
						  "Incorrect sum--wanted three zillion");

			Assert.IsTrue(zero.Multiply(zero).Equals(zero), "0*0");
			Assert.IsTrue(zero.Multiply(one).Equals(zero), "0*1");
			Assert.IsTrue(one.Multiply(zero).Equals(zero), "1*0");
			Assert.IsTrue(one.Multiply(one).Equals(one), "1*1");
			Assert.IsTrue(zero.Multiply(minusOne).Equals(zero), "0*(-1)");
			Assert.IsTrue(minusOne.Multiply(zero).Equals(zero), "(-1)*0");
			Assert.IsTrue(minusOne.Multiply(minusOne).Equals(one), "(-1)*(-1)");
			Assert.IsTrue(one.Multiply(minusOne).Equals(minusOne), "1*(-1)");
			Assert.IsTrue(minusOne.Multiply(one).Equals(minusOne), "(-1)*1");

			testAllMults(bi1, bi1, bi11);
			testAllMults(bi2, bi2, bi22);
			testAllMults(bi3, bi3, bi33);
			testAllMults(bi1, bi2, bi12);
			testAllMults(bi1, bi3, bi13);
			testAllMults(bi2, bi3, bi23);
		}

		/**
		 * @tests java.math.BigInteger#divide(java.math.BigInteger)
		 */
		[Test]
		public void test_divideLjava_math_BigInteger() {
			testAllDivs(bi33, bi3);
			testAllDivs(bi22, bi2);
			testAllDivs(bi11, bi1);
			testAllDivs(bi13, bi1);
			testAllDivs(bi13, bi3);
			testAllDivs(bi12, bi1);
			testAllDivs(bi12, bi2);
			testAllDivs(bi23, bi2);
			testAllDivs(bi23, bi3);
			testAllDivs(largePos, bi1);
			testAllDivs(largePos, bi2);
			testAllDivs(largePos, bi3);
			testAllDivs(largeNeg, bi1);
			testAllDivs(largeNeg, bi2);
			testAllDivs(largeNeg, bi3);
			testAllDivs(largeNeg, largePos);
			testAllDivs(largePos, largeNeg);
			testAllDivs(bi3, bi3);
			testAllDivs(bi2, bi2);
			testAllDivs(bi1, bi1);
			testDivRanges(bi1);
			testDivRanges(bi2);
			testDivRanges(bi3);
			testDivRanges(smallPos);
			testDivRanges(largePos);
			testDivRanges(new BigInteger("62EB40FEF85AA9EB", 16));
			testAllDivs(BigInteger.ValueOf(0xCC0225953CL), BigInteger
					.ValueOf(0x1B937B765L));

			try {
				largePos.Divide(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}

			try {
				bi1.Divide(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}

			try {
				bi3.Negate().Divide(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}

			try {
				zero.Divide(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}
		}

		/**
		 * @tests java.math.BigInteger#remainder(java.math.BigInteger)
		 */
		[Test]
		public void test_remainderLjava_math_BigInteger() {
			try {
				largePos.Remainder(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}

			try {
				bi1.Remainder(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}

			try {
				bi3.Negate().Remainder(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}

			try {
				zero.Remainder(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}
		}

		/**
		 * @tests java.math.BigInteger#mod(java.math.BigInteger)
		 */
		[Test]
		public void test_modLjava_math_BigInteger() {
			try {
				largePos.Mod(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}

			try {
				bi1.Mod(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}

			try {
				bi3.Negate().Mod(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}

			try {
				zero.Mod(zero);
				Assert.Fail("ArithmeticException expected");
			} catch (ArithmeticException e) {
			}
		}

		/**
		 * @tests java.math.BigInteger#divideAndRemainder(java.math.BigInteger)
		 */
		[Test]
		public void test_divideAndRemainderLjava_math_BigInteger() {
			BigInteger remainder;

			Assert.Throws<ArithmeticException>(() => largePos.DivideAndRemainder(zero, out remainder));
			Assert.Throws<ArithmeticException>(() => bi1.DivideAndRemainder(zero, out remainder));
			Assert.Throws<ArithmeticException>(() => bi3.Negate().DivideAndRemainder(zero, out remainder));
			Assert.Throws<ArithmeticException>(() => zero.DivideAndRemainder(zero, out remainder));
		}

		/**
		 * @tests java.math.BigInteger#BigInteger(java.lang.String)
		 */
		[Test]
		public void test_ConstructorLjava_lang_String() {
			Assert.IsTrue(new BigInteger("0").Equals(BigInteger.ValueOf(0)), "new(0)");
			Assert.IsTrue(new BigInteger("1").Equals(BigInteger.ValueOf(1)), "new(1)");
			Assert.IsTrue(new BigInteger("12345678901234").Equals(BigInteger.ValueOf(12345678901234L)), "new(12345678901234)");
			Assert.IsTrue(new BigInteger("-1").Equals(BigInteger.ValueOf(-1)), "new(-1)");
			Assert.IsTrue(new BigInteger("-12345678901234").Equals(BigInteger.ValueOf(-12345678901234L)), "new(-12345678901234)");
		}

		/**
		 * @tests java.math.BigInteger#BigInteger(java.lang.String, int)
		 */
		[Test]
		public void test_ConstructorLjava_lang_StringI() {
			Assert.IsTrue(new BigInteger("0", 16).Equals(BigInteger.ValueOf(0)), "new(0,16)");
			Assert.IsTrue(new BigInteger("1", 16).Equals(BigInteger.ValueOf(1)), "new(1,16)");
			Assert.IsTrue(new BigInteger("ABF345678901234", 16).Equals(BigInteger.ValueOf(0xABF345678901234L)), "new(ABF345678901234,16)");
			Assert.IsTrue(new BigInteger("abf345678901234", 16).Equals(BigInteger.ValueOf(0xABF345678901234L)), "new(abf345678901234,16)");
			Assert.IsTrue(new BigInteger("-1", 16).Equals(BigInteger.ValueOf(-1)), "new(-1,16)");
			Assert.IsTrue(new BigInteger("-ABF345678901234", 16).Equals(BigInteger.ValueOf(-0xABF345678901234L)), "new(-ABF345678901234,16)");
			Assert.IsTrue(new BigInteger("-abf345678901234", 16).Equals(BigInteger.ValueOf(-0xABF345678901234L)), "new(-abf345678901234,16)");
			Assert.IsTrue(new BigInteger("-101010101", 2).Equals(BigInteger.ValueOf(-341)), "new(-101010101,2)");
		}

		/**
		 * @tests java.math.BigInteger#ToString()
		 */
		[Test]
		public void test_toString() {
			Assert.IsTrue("0".Equals(BigInteger.ValueOf(0).ToString()), "0.ToString");
			Assert.IsTrue("1".Equals(BigInteger.ValueOf(1).ToString()), "1.ToString");
			Assert.IsTrue("12345678901234".Equals(BigInteger.ValueOf(12345678901234L).ToString()), "12345678901234.ToString");
			Assert.IsTrue("-1".Equals(BigInteger.ValueOf(-1).ToString()), "-1.ToString");
			Assert.IsTrue("-12345678901234".Equals(BigInteger.ValueOf(-12345678901234L).ToString()), "-12345678901234.ToString");
		}

		/**
		 * @tests java.math.BigInteger#ToString(int)
		 */
		[Test]
		public void test_toStringI() {
			Assert.IsTrue("0".Equals(BigInteger.ValueOf(0).ToString(16)), "0.ToString(16)");
			Assert.IsTrue("1".Equals(BigInteger.ValueOf(1).ToString(16)), "1.ToString(16)");
			Assert.IsTrue("abf345678901234".Equals(BigInteger.ValueOf(0xABF345678901234L).ToString(16)), "ABF345678901234.ToString(16)");
			Assert.IsTrue("-1".Equals(BigInteger.ValueOf(-1).ToString(16)), "-1.ToString(16)");
			Assert.IsTrue("-abf345678901234".Equals(BigInteger.ValueOf(-0xABF345678901234L).ToString(16)), "-ABF345678901234.ToString(16)");
			Assert.IsTrue("-101010101".Equals(BigInteger.ValueOf(-341).ToString(2)), "-101010101.ToString(2)");
		}

		/**
		 * @tests java.math.BigInteger#and(java.math.BigInteger)
		 */
		[Test]
		public void test_andLjava_math_BigInteger() {
			foreach (BigInteger[] element in booleanPairs) {
				BigInteger i1 = element[0], i2 = element[1];
				BigInteger res = i1.And(i2);
				Assert.IsTrue(res.Equals(i2.And(i1)), "symmetry of and");
				int len = System.Math.Max(i1.BitLength, i2.BitLength) + 66;
				for (int i = 0; i < len; i++) {
					Assert.IsTrue((i1.TestBit(i) && i2.TestBit(i)) == res.TestBit(i), "and");
				}
			}
		}

		/**
		 * @tests java.math.BigInteger#or(java.math.BigInteger)
		 */
		[Test]
		public void test_orLjava_math_BigInteger() {
			foreach (BigInteger[] element in booleanPairs) {
				BigInteger i1 = element[0], i2 = element[1];
				BigInteger res = i1.Or(i2);
				Assert.IsTrue(res.Equals(i2.Or(i1)), "symmetry of or");
				int len = System.Math.Max(i1.BitLength, i2.BitLength) + 66;
				for (int i = 0; i < len; i++) {
					Assert.IsTrue((i1.TestBit(i) || i2.TestBit(i)) == res.TestBit(i), "or");
				}
			}
		}

		/**
		 * @tests java.math.BigInteger#xor(java.math.BigInteger)
		 */
		[Test]
		public void test_xorLjava_math_BigInteger() {
			foreach (BigInteger[] element in booleanPairs) {
				BigInteger i1 = element[0], i2 = element[1];
				BigInteger res = i1.XOr(i2);
				Assert.IsTrue(res.Equals(i2.XOr(i1)), "symmetry of xor");
				int len = System.Math.Max(i1.BitLength, i2.BitLength) + 66;
				for (int i = 0; i < len; i++) {
					Assert.IsTrue((i1.TestBit(i) ^ i2.TestBit(i)) == res.TestBit(i), "xor");
				}
			}
		}

		/**
		 * @tests java.math.BigInteger#not()
		 */
		[Test]
		public void test_not() {
			foreach (BigInteger[] element in booleanPairs) {
				BigInteger i1 = element[0];
				BigInteger res = i1.Not();
				int len = i1.BitLength + 66;
				for (int i = 0; i < len; i++) {
					Assert.IsTrue(!i1.TestBit(i) == res.TestBit(i), "not");
				}
			}
		}

		/**
		 * @tests java.math.BigInteger#andNot(java.math.BigInteger)
		 */
		[Test]
		public void test_andNotLjava_math_BigInteger() {
			foreach (BigInteger[] element in booleanPairs) {
				BigInteger i1 = element[0], i2 = element[1];
				BigInteger res = i1.AndNot(i2);
				int len = System.Math.Max(i1.BitLength, i2.BitLength) + 66;
				for (int i = 0; i < len; i++) {
					Assert.IsTrue((i1.TestBit(i) && !i2.TestBit(i)) == res.TestBit(i), "andNot");
				}
				// asymmetrical
				i1 = element[1];
				i2 = element[0];
				res = i1.AndNot(i2);
				for (int i = 0; i < len; i++) {
					Assert.IsTrue((i1.TestBit(i) && !i2.TestBit(i)) == res.TestBit(i), "andNot reversed");
				}
			}
			//regression for HARMONY-4653
			try {
				BigInteger.Zero.AndNot(null);
				Assert.Fail("should throw NPE");
			} catch (Exception e) {
				//expected
			}
			BigInteger bi = new BigInteger(0, new byte[] { });
			Assert.AreEqual(BigInteger.Zero, bi.AndNot(BigInteger.Zero));
		}

		private void testDiv(BigInteger i1, BigInteger i2) {
			BigInteger q = i1.Divide(i2);
			BigInteger r = i1.Remainder(i2);
			BigInteger remainder;
			BigInteger quotient = i1.DivideAndRemainder(i2, out remainder);

			Assert.IsTrue(q.Equals(quotient), "Divide and DivideAndRemainder do not agree");
			Assert.IsTrue(r.Equals(remainder), "Remainder and DivideAndRemainder do not agree");
			Assert.IsTrue(q.Sign != 0 || q.Equals(zero), "signum and equals(zero) do not agree on quotient");
			Assert.IsTrue(r.Sign != 0 || r.Equals(zero), "signum and equals(zero) do not agree on remainder");
			Assert.IsTrue(q.Sign == 0 || q.Sign == i1.Sign * i2.Sign, "wrong sign on quotient");
			Assert.IsTrue(r.Sign == 0 || r.Sign == i1.Sign, "wrong sign on remainder");
			Assert.IsTrue(r.Abs().CompareTo(i2.Abs()) < 0, "remainder out of range");
			Assert.IsTrue(q.Abs().Add(one).Multiply(i2.Abs()).CompareTo(i1.Abs()) > 0, "quotient too small");
			Assert.IsTrue(q.Abs().Multiply(i2.Abs()).CompareTo(i1.Abs()) <= 0, "quotient too large");
			BigInteger p = q.Multiply(i2);
			BigInteger a = p.Add(r);
			Assert.IsTrue(a.Equals(i1), "(a/b)*b+(a%b) != a");
			try {
				BigInteger mod = i1.Mod(i2);
				Assert.IsTrue(mod.Sign >= 0, "mod is negative");
				Assert.IsTrue(mod.Abs().CompareTo(i2.Abs()) < 0, "mod out of range");
				Assert.IsTrue(r.Sign < 0 || r.Equals(mod), "positive remainder == mod");
				Assert.IsTrue(r.Sign >= 0 || r.Equals(mod.Subtract(i2)), "negative remainder == mod - divisor");
			} catch (ArithmeticException e) {
				Assert.IsTrue(i2.Sign <= 0, "mod fails on negative divisor only");
			}
		}

		private void testDivRanges(BigInteger i) {
			BigInteger bound = i.Multiply(two);
			for (BigInteger j = bound.Negate(); j.CompareTo(bound) <= 0; j = j
					.Add(i)) {
				BigInteger innerbound = j.Add(two);
				BigInteger k = j.Subtract(two);
				for (; k.CompareTo(innerbound) <= 0; k = k.Add(one)) {
					testDiv(k, i);
				}
			}
		}

		private bool isPrime(long b) {
			if (b == 2) {
				return true;
			}
			// check for div by 2
			if ((b & 1L) == 0) {
				return false;
			}
			long maxlen = ((long)System.Math.Sqrt(b)) + 2;
			for (long x = 3; x < maxlen; x += 2) {
				if (b % x == 0) {
					return false;
				}
			}
			return true;
		}

		private void testAllMults(BigInteger i1, BigInteger i2, BigInteger ans) {
			Assert.IsTrue(i1.Multiply(i2).Equals(ans), "i1*i2=ans");
			Assert.IsTrue(i2.Multiply(i1).Equals(ans), "i2*i1=ans");
			Assert.IsTrue(i1.Negate().Multiply(i2).Equals(ans.Negate()), "-i1*i2=-ans");
			Assert.IsTrue(i2.Negate().Multiply(i1).Equals(ans.Negate()), "-i2*i1=-ans");
			Assert.IsTrue(i1.Multiply(i2.Negate()).Equals(ans.Negate()), "i1*-i2=-ans");
			Assert.IsTrue(i2.Multiply(i1.Negate()).Equals(ans.Negate()), "i2*-i1=-ans");
			Assert.IsTrue(i1.Negate().Multiply(i2.Negate()).Equals(ans), "-i1*-i2=ans");
			Assert.IsTrue(i2.Negate().Multiply(i1.Negate()).Equals(ans), "-i2*-i1=ans");
		}

		private void testAllDivs(BigInteger i1, BigInteger i2) {
			testDiv(i1, i2);
			testDiv(i1.Negate(), i2);
			testDiv(i1, i2.Negate());
			testDiv(i1.Negate(), i2.Negate());
		}
	}
}