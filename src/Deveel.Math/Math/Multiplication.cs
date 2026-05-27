// 
//  Copyright 2009-2024 Antonello Provenzano
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;

namespace Deveel.Math {
	/// <summary>
	/// Static library that provides all multiplication methods for <see cref="BigInteger"/>.
	/// </summary>
	static class Multiplication {
		/// <summary>
		/// Break point in digits (number of <c>int</c> elements)
		/// between Karatsuba and Pencil and Paper multiply.
		/// </summary>
		private const int WhenUseKaratsuba = 63;

		/// <summary>
		/// An array with powers of ten that fit in the type <c>int</c>.
		/// (<c>10^0, 10^1, ..., 10^9</c>).
		/// </summary>
		static readonly int[] TenPows = {
        1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000
    };

		/// <summary>
		/// An array with powers of five that fit in the type <c>int</c>.
		/// (<c>5^0, 5^1, ..., 5^13</c>).
		/// </summary>
		static readonly int[] FivePows = {
        1, 5, 25, 125, 625, 3125, 15625, 78125, 390625,
        1953125, 9765625, 48828125, 244140625, 1220703125
    };

		/// <summary>
		/// An array with the first powers of ten in <see cref="BigInteger"/> version.
		/// (<c>10^0, 10^1, ..., 10^31</c>).
		/// </summary>
		public static readonly BigInteger[] BigTenPows = new BigInteger[32];

		/// <summary>
		/// An array with the first powers of five in <see cref="BigInteger"/> version.
		/// (<c>5^0, 5^1, ..., 5^31</c>).
		/// </summary>
		public static readonly BigInteger[] BigFivePows = new BigInteger[32];


		static Multiplication() {
			int i;
			long fivePow = 1L;

			for (i = 0; i <= 18; i++) {
				BigFivePows[i] = BigInteger.FromInt64(fivePow);
				BigTenPows[i] = BigInteger.FromInt64(fivePow << i);
				fivePow *= 5;
			}
			for (; i < BigTenPows.Length; i++) {
				BigFivePows[i] = BigFivePows[i - 1] * BigFivePows[1];
				BigTenPows[i] = BigTenPows[i - 1] * BigInteger.Ten;
			}
		}

		/// <summary>
		/// Performs a multiplication of two <see cref="BigInteger"/> values and hides the
		/// algorithm used.
		/// </summary>
		/// <param name="x">The first factor.</param>
		/// <param name="y">The second factor.</param>
		/// <returns><paramref name="x"/> * <paramref name="y"/>.</returns>
		/// <example>
		/// <code>
		/// BigInteger a = new BigInteger(1234);
		/// BigInteger b = new BigInteger(5678);
		/// BigInteger result = Multiplication.Multiply(a, b);
		/// // result == 7006652
		/// </code>
		/// </example>
		public static BigInteger Multiply(BigInteger x, BigInteger y) {
			return Karatsuba(x, y);
		}

		/// <summary>
		/// Performs the multiplication with the Karatsuba algorithm.
		/// <b>Karatsuba's algorithm:</b>
		/// <code>
		/// u = u1 * B + u0
		/// v = v1 * B + v0
		/// u*v = (u1 * v1) * B^2 + ((u1 - u0) * (v0 - v1) + u1 * v1 + u0 * v0) * B + u0 * v0
		/// </code>
		/// </summary>
		/// <param name="op1">The first factor of the product.</param>
		/// <param name="op2">The second factor of the product.</param>
		/// <returns><paramref name="op1"/> * <paramref name="op2"/>.</returns>
		private static BigInteger Karatsuba(BigInteger op1, BigInteger op2) {
			BigInteger temp;
			if (op2.numberLength > op1.numberLength) {
				temp = op1;
				op1 = op2;
				op2 = temp;
			}
			if (op2.numberLength < WhenUseKaratsuba) {
				return MultiplyPap(op1, op2);
			}
			int ndiv2 = (int)(op1.numberLength & 0xFFFFFFFE) << 4;
			BigInteger upperOp1 = op1 >> ndiv2;
			BigInteger upperOp2 = op2 >> ndiv2;
			BigInteger lowerOp1 = op1 - (upperOp1 << ndiv2);
			BigInteger lowerOp2 = op2 - (upperOp2 << ndiv2);

			BigInteger upper = Karatsuba(upperOp1, upperOp2);
			BigInteger lower = Karatsuba(lowerOp1, lowerOp2);
			BigInteger middle = Karatsuba(upperOp1 - lowerOp1,
					lowerOp2 - upperOp2);
			middle = (middle + upper + lower);
			middle = middle << ndiv2;
			upper = upper << (ndiv2 << 1);

			return (upper + middle + lower);
		}

		/// <summary>
		/// Multiplies two BigIntegers using the traditional scholar algorithm described by Knuth.
		/// </summary>
		/// <param name="a">The first factor (must be &gt;= 0).</param>
		/// <param name="b">The second factor (must be &gt;= 0).</param>
		/// <returns>A <see cref="BigInteger"/> of value <paramref name="a"/> * <paramref name="b"/>.</returns>
		private static BigInteger MultiplyPap(BigInteger a, BigInteger b) {
			int aLen = a.numberLength;
			int bLen = b.numberLength;
			int resLength = aLen + bLen;
			int resSign = (a.Sign != b.Sign) ? -1 : 1;
			if (resLength == 2) {
				long val = UnsignedMultAddAdd(a.Digits[0], b.Digits[0], 0, 0);
				int valueLo = (int)val;
				int valueHi = (int)Utils.URShift(val, 32);
				return ((valueHi == 0)
				? new BigInteger(resSign, valueLo)
				: new BigInteger(resSign, 2, new int[] { valueLo, valueHi }));
			}
			int[] aDigits = a.Digits;
			int[] bDigits = b.Digits;
			int[] resDigits = new int[resLength];
			MultArraysPap(aDigits, aLen, bDigits, bLen, resDigits);
			BigInteger result = new BigInteger(resSign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Multiplies two digit arrays using the pencil-and-paper algorithm and stores
		/// the result in <paramref name="resDigits"/>.
		/// </summary>
		/// <param name="aDigits">The first factor digits.</param>
		/// <param name="aLen">The length of <paramref name="aDigits"/>.</param>
		/// <param name="bDigits">The second factor digits.</param>
		/// <param name="bLen">The length of <paramref name="bDigits"/>.</param>
		/// <param name="resDigits">The result array.</param>
		public static void MultArraysPap(int[] aDigits, int aLen, int[] bDigits, int bLen, int[] resDigits) {
			if (aLen == 0 || bLen == 0) return;

			if (aLen == 1) {
				resDigits[bLen] = MultiplyByInt(resDigits, bDigits, bLen, aDigits[0]);
			} else if (bLen == 1) {
				resDigits[aLen] = MultiplyByInt(resDigits, aDigits, aLen, bDigits[0]);
			} else {
				MultPap(aDigits, bDigits, resDigits, aLen, bLen);
			}
		}

		private static void MultPap(int[] a, int[] b, int[] t, int aLen, int bLen) {
			if (a == b && aLen == bLen) {
				Square(a, aLen, t);
				return;
			}

			for (int i = 0; i < aLen; i++) {
				long carry = 0;
				int aI = a[i];
				for (int j = 0; j < bLen; j++) {
					carry = UnsignedMultAddAdd(aI, b[j], t[i + j], (int)carry);
					t[i + j] = (int)carry;
					carry = Utils.URShift(carry, 32);
				}
				t[i + bLen] = (int)carry;
			}
		}

		/// <summary>
		/// Multiplies an array of integers by an integer value and saves the result in <paramref name="res"/>.
		/// </summary>
		/// <param name="res">The result array.</param>
		/// <param name="a">The array of integers.</param>
		/// <param name="aSize">The number of elements of <paramref name="a"/> to be multiplied.</param>
		/// <param name="factor">The multiplier.</param>
		/// <returns>The top digit of the product.</returns>
		private static int MultiplyByInt(int[] res, int[] a, int aSize, int factor) {
			long carry = 0;
			for (int i = 0; i < aSize; i++) {
				carry = UnsignedMultAddAdd(a[i], factor, (int)carry, 0);
				res[i] = (int)carry;
				carry = Utils.URShift(carry, 32);
			}
			return (int)carry;
		}

		/// <summary>
		/// Multiplies an array of integers by an integer value.
		/// </summary>
		/// <param name="a">The array of integers.</param>
		/// <param name="aSize">The number of elements of <paramref name="a"/> to be multiplied.</param>
		/// <param name="factor">The multiplier.</param>
		/// <returns>The top digit of the product.</returns>
		public static int MultiplyByInt(int[] a, int aSize, int factor) {
			return MultiplyByInt(a, a, aSize, factor);
		}

		/// <summary>
		/// Multiplies a number by a positive integer.
		/// </summary>
		/// <param name="val">An arbitrary <see cref="BigInteger"/>.</param>
		/// <param name="factor">A positive <c>int</c> number.</param>
		/// <returns><paramref name="val"/> * <paramref name="factor"/>.</returns>
		/// <example>
		/// <code>
		/// BigInteger value = new BigInteger(123);
		/// BigInteger result = Multiplication.MultiplyByPositiveInt(value, 5);
		/// // result == 615
		/// </code>
		/// </example>
		public static BigInteger MultiplyByPositiveInt(BigInteger val, int factor) {
			int resSign = val.Sign;
			if (resSign == 0) {
				return BigInteger.Zero;
			}
			int aNumberLength = val.numberLength;
			int[] aDigits = val.Digits;

			if (aNumberLength == 1) {
				long res = UnsignedMultAddAdd(aDigits[0], factor, 0, 0);
				int resLo = (int)res;
				int resHi = (int)Utils.URShift(res, 32);
				return ((resHi == 0)
				? new BigInteger(resSign, resLo)
				: new BigInteger(resSign, 2, new int[] { resLo, resHi }));
			}
			int resLength = aNumberLength + 1;
			int[] resDigits = new int[resLength];

			resDigits[aNumberLength] = MultiplyByInt(resDigits, aDigits, aNumberLength, factor);
			BigInteger result = new BigInteger(resSign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Computes <paramref name="b"/> raised to the power of <paramref name="exponent"/>.
		/// </summary>
		/// <param name="b">The base.</param>
		/// <param name="exponent">The exponent (must be &gt; 0).</param>
		/// <returns><c>b^exponent</c>.</returns>
		/// <example>
		/// <code>
		/// BigInteger base = new BigInteger(2);
		/// BigInteger result = Multiplication.Pow(base, 10);
		/// // result == 1024
		/// </code>
		/// </example>
		public static BigInteger Pow(BigInteger b, int exponent) {
			BigInteger res = BigInteger.One;
			BigInteger acc = b;

			for (; exponent > 1; exponent >>= 1) {
				if ((exponent & 1) != 0) {
					res = res * acc;
				}
				if (acc.numberLength == 1) {
					acc = acc * acc;
				} else {
					acc = new BigInteger(1, Square(acc.Digits, acc.numberLength, new int[acc.numberLength << 1]));
				}
			}
			res = res * acc;
			return res;
		}

		/// <summary>
		/// Performs a<sup>2</sup>.
		/// </summary>
		/// <param name="a">The number to square.</param>
		/// <param name="aLen">The length of the number to square.</param>
		/// <param name="res">The result array.</param>
		/// <returns>The squared result array.</returns>
		private static int[] Square(int[] a, int aLen, int[] res) {
			long carry;

			for (int i = 0; i < aLen; i++) {
				carry = 0;
				for (int j = i + 1; j < aLen; j++) {
					carry = UnsignedMultAddAdd(a[i], a[j], res[i + j], (int)carry);
					res[i + j] = (int)carry;
					carry = Utils.URShift(carry, 32);
				}
				res[i + aLen] = (int)carry;
			}

			BitLevel.ShiftLeftOneBit(res, res, aLen << 1);

			carry = 0;
			for (int i = 0, index = 0; i < aLen; i++, index++) {
				carry = UnsignedMultAddAdd(a[i], a[i], res[index], (int)carry);
				res[index] = (int)carry;
				carry = Utils.URShift(carry, 32);
				index++;
				carry += res[index] & 0xFFFFFFFFL;
				res[index] = (int)carry;
				carry = Utils.URShift(carry, 32);
			}
			return res;
		}

		/// <summary>
		/// Multiplies a number by a power of ten.
		/// </summary>
		/// <param name="val">The number to be multiplied.</param>
		/// <param name="exp">A positive <c>long</c> exponent.</param>
		/// <returns><paramref name="val"/> * 10<sup>exp</sup>.</returns>
		public static BigInteger MultiplyByTenPow(BigInteger val, long exp) {
			return ((exp < TenPows.Length)
			? MultiplyByPositiveInt(val, TenPows[(int)exp])
			: val * PowerOf10(exp));
		}

		/// <summary>
		/// Calculates a power of ten, which exponent could be out of 32-bit range.
		/// </summary>
		/// <param name="exp">The exponent of the power of ten (must be positive).</param>
		/// <returns>A <see cref="BigInteger"/> with value 10<sup>exp</sup>.</returns>
		public static BigInteger PowerOf10(long exp) {
			int intExp = (int)exp;
			if (exp < BigTenPows.Length) {
				return BigTenPows[intExp];
			} else if (exp <= 50) {
				return BigMath.Pow(BigInteger.Ten, intExp);
			} else if (exp <= 1000) {
				return BigMath.Pow(BigFivePows[1], intExp) << intExp;
			}
			var byteArraySize = 1 + (exp / 2.4082399653118496);

			if (byteArraySize > (128 * 1024))
				throw new ArithmeticException(Messages.math01);

			if (exp <= Int32.MaxValue) {
				return BigMath.Pow(BigFivePows[1], intExp) << intExp;
			}
			BigInteger powerOfFive = BigMath.Pow(BigFivePows[1], Int32.MaxValue);
			BigInteger res = powerOfFive;
			long longExp = exp - Int32.MaxValue;

			intExp = (int)(exp % Int32.MaxValue);
			while (longExp > Int32.MaxValue) {
				res = res * powerOfFive;
				longExp -= Int32.MaxValue;
			}
			res = res * BigMath.Pow(BigFivePows[1], intExp);
			res = res << Int32.MaxValue;
			longExp = exp - Int32.MaxValue;
			while (longExp > Int32.MaxValue) {
				res = res << Int32.MaxValue;
				longExp -= Int32.MaxValue;
			}
			res = res << intExp;
			return res;
		}

		/// <summary>
		/// Multiplies a number by a power of five.
		/// </summary>
		/// <param name="val">The number to be multiplied.</param>
		/// <param name="exp">A positive <c>int</c> exponent.</param>
		/// <returns><paramref name="val"/> * 5<sup>exp</sup>.</returns>
		public static BigInteger MultiplyByFivePow(BigInteger val, int exp) {
			if (exp < FivePows.Length) {
				return MultiplyByPositiveInt(val, FivePows[exp]);
			} else if (exp < BigFivePows.Length) {
				return val * BigFivePows[exp];
			} else {
				return val * BigMath.Pow(BigFivePows[1], exp);
			}
		}

		/// <summary>
		/// Computes the value <c>(uint)a * (uint)b + (uint)c + (uint)d</c>.
		/// </summary>
		/// <param name="a">Parameter 1.</param>
		/// <param name="b">Parameter 2.</param>
		/// <param name="c">Parameter 3.</param>
		/// <param name="d">Parameter 4.</param>
		/// <returns>The value of the expression.</returns>
		public static long UnsignedMultAddAdd(int a, int b, int c, int d) {
			return (a & 0xFFFFFFFFL) * (b & 0xFFFFFFFFL) + (c & 0xFFFFFFFFL) + (d & 0xFFFFFFFFL);
		}

	}
}
