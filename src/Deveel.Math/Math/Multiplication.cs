// 
//  Copyright 2009-2017  Deveel
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
	/**
 * Static library that provides all multiplication of {@link BigInteger} methods.
 */
	static class Multiplication {
		/**
		 * Break point in digits (number of {@code int} elements)
		 * between Karatsuba and Pencil and Paper multiply.
		 */
		private const int WhenUseKaratsuba = 63; // an heuristic value

		/**
		 * An array with powers of ten that fit in the type {@code int}.
		 * ({@code 10^0,10^1,...,10^9})
		 */
		static readonly int[] TenPows = {
        1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000
    };

		/**
		 * An array with powers of five that fit in the type {@code int}.
		 * ({@code 5^0,5^1,...,5^13})
		 */
		static readonly int[] FivePows = {
        1, 5, 25, 125, 625, 3125, 15625, 78125, 390625,
        1953125, 9765625, 48828125, 244140625, 1220703125
    };

		/**
		 * An array with the first powers of ten in {@code BigInteger} version.
		 * ({@code 10^0,10^1,...,10^31})
		 */
		public static readonly BigInteger[] BigTenPows = new BigInteger[32];

		/**
		 * An array with the first powers of five in {@code BigInteger} version.
		 * ({@code 5^0,5^1,...,5^31})
		 */
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

		/**
		 * Performs a multiplication of two BigInteger and hides the algorithm used.
		 * @see BigInteger#multiply(BigInteger)
		 */

		public static BigInteger Multiply(BigInteger x, BigInteger y) {
			return Karatsuba(x, y);
		}

		/**
		 * Performs the multiplication with the Karatsuba's algorithm.
		 * <b>Karatsuba's algorithm:</b>
		 *<tt>
		 *             u = u<sub>1</sub> * B + u<sub>0</sub><br>
		 *             v = v<sub>1</sub> * B + v<sub>0</sub><br>
		 *
		 *
		 *  u*v = (u<sub>1</sub> * v<sub>1</sub>) * B<sub>2</sub> + ((u<sub>1</sub> - u<sub>0</sub>) * (v<sub>0</sub> - v<sub>1</sub>) + u<sub>1</sub> * v<sub>1</sub> +
		 *  u<sub>0</sub> * v<sub>0</sub> ) * B + u<sub>0</sub> * v<sub>0</sub><br>
		 *</tt>
		 * @param op1 first factor of the product
		 * @param op2 second factor of the product
		 * @return {@code op1 * op2}
		 * @see #multiply(BigInteger, BigInteger)
		 */
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
			/*  Karatsuba:  u = u1*B + u0
			 *              v = v1*B + v0
			 *  u*v = (u1*v1)*B^2 + ((u1-u0)*(v0-v1) + u1*v1 + u0*v0)*B + u0*v0
			 */
			// ndiv2 = (op1.numberLength / 2) * 32
			int ndiv2 = (int)(op1.numberLength & 0xFFFFFFFE) << 4;
			BigInteger upperOp1 = op1 >> ndiv2;
			BigInteger upperOp2 = op2 >> ndiv2;
			BigInteger lowerOp1 = op1 - upperOp1 << ndiv2;
			BigInteger lowerOp2 = op2 - upperOp2 << ndiv2;

			BigInteger upper = Karatsuba(upperOp1, upperOp2);
			BigInteger lower = Karatsuba(lowerOp1, lowerOp2);
			BigInteger middle = Karatsuba(upperOp1 - lowerOp1,
					lowerOp2 - upperOp2);
			middle = (middle + upper + lower);
			middle = middle << ndiv2;
			upper = upper << (ndiv2 << 1);

			return (upper + middle + lower);
		}

		/**
		 * Multiplies two BigIntegers.
		 * Implements traditional scholar algorithm described by Knuth.
		 *
		 * <br><tt>
		 *         <table border="0">
		 * <tbody>
		 *
		 *
		 * <tr>
		 * <td align="center">A=</td>
		 * <td>a<sub>3</sub></td>
		 * <td>a<sub>2</sub></td>
		 * <td>a<sub>1</sub></td>
		 * <td>a<sub>0</sub></td>
		 * <td></td>
		 * <td></td>
		 * </tr>
		 *
		 *<tr>
		 * <td align="center">B=</td>
		 * <td></td>
		 * <td>b<sub>2</sub></td>
		 * <td>b<sub>1</sub></td>
		 * <td>b<sub>1</sub></td>
		 * <td></td>
		 * <td></td>
		 * </tr>
		 *
		 * <tr>
		 * <td></td>
		 * <td></td>
		 * <td></td>
		 * <td>b<sub>0</sub>*a<sub>3</sub></td>
		 * <td>b<sub>0</sub>*a<sub>2</sub></td>
		 * <td>b<sub>0</sub>*a<sub>1</sub></td>
		 * <td>b<sub>0</sub>*a<sub>0</sub></td>
		 * </tr>
		 *
		 * <tr>
		 * <td></td>
		 * <td></td>
		 * <td>b<sub>1</sub>*a<sub>3</sub></td>
		 * <td>b<sub>1</sub>*a<sub>2</sub></td>
		 * <td>b<sub>1</sub>*a1</td>
		 * <td>b<sub>1</sub>*a0</td>
		 * </tr>
		 *
		 * <tr>
		 * <td>+</td>
		 * <td>b<sub>2</sub>*a<sub>3</sub></td>
		 * <td>b<sub>2</sub>*a<sub>2</sub></td>
		 * <td>b<sub>2</sub>*a<sub>1</sub></td>
		 * <td>b<sub>2</sub>*a<sub>0</sub></td>
		 * </tr>
		 *
		 *<tr>
		 * <td></td>
		 *<td>______</td>
		 * <td>______</td>
		 * <td>______</td>
		 * <td>______</td>
		 * <td>______</td>
		 * <td>______</td>
		 *</tr>
		 *
		 * <tr>
		 *
		 * <td align="center">A*B=R=</td>
		 * <td align="center">r<sub>5</sub></td>
		 * <td align="center">r<sub>4</sub></td>
		 * <td align="center">r<sub>3</sub></td>
		 * <td align="center">r<sub>2</sub></td>
		 * <td align="center">r<sub>1</sub></td>
		 * <td align="center">r<sub>0</sub></td>
		 * <td></td>
		 * </tr>
		 *
		 * </tbody>
		 * </table>
		 *
		 *</tt>
		 *
		 * @param op1 first factor of the multiplication {@code  op1 >= 0}
		 * @param op2 second factor of the multiplication {@code  op2 >= 0}
		 * @return a {@code BigInteger} of value {@code  op1 * op2}
		 */
		private static BigInteger MultiplyPap(BigInteger a, BigInteger b) {
			// PRE: a >= b
			int aLen = a.numberLength;
			int bLen = b.numberLength;
			int resLength = aLen + bLen;
			int resSign = (a.Sign != b.Sign) ? -1 : 1;
			// A special case when both numbers don't exceed int
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
			// Common case
			MultArraysPap(aDigits, aLen, bDigits, bLen, resDigits);
			BigInteger result = new BigInteger(resSign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

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

		/**
		 * Multiplies an array of integers by an integer value
		 * and saves the result in {@code res}.
		 * @param a the array of integers
		 * @param aSize the number of elements of intArray to be multiplied
		 * @param factor the multiplier
		 * @return the top digit of production
		 */
		private static int MultiplyByInt(int[] res, int[] a, int aSize, int factor) {
			long carry = 0;
			for (int i = 0; i < aSize; i++) {
				carry = UnsignedMultAddAdd(a[i], factor, (int)carry, 0);
				res[i] = (int)carry;
				carry = Utils.URShift(carry, 32);
			}
			return (int)carry;
		}


		/**
		 * Multiplies an array of integers by an integer value.
		 * @param a the array of integers
		 * @param aSize the number of elements of intArray to be multiplied
		 * @param factor the multiplier
		 * @return the top digit of production
		 */
		public static int MultiplyByInt(int[] a, int aSize, int factor) {
			return MultiplyByInt(a, a, aSize, factor);
		}

		/**
		 * Multiplies a number by a positive integer.
		 * @param val an arbitrary {@code BigInteger}
		 * @param factor a positive {@code int} number
		 * @return {@code val * factor}
		 */
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
			// Common case
			int resLength = aNumberLength + 1;
			int[] resDigits = new int[resLength];

			resDigits[aNumberLength] = MultiplyByInt(resDigits, aDigits, aNumberLength, factor);
			BigInteger result = new BigInteger(resSign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		public static BigInteger Pow(BigInteger b, int exponent) {
			// PRE: exp > 0
			BigInteger res = BigInteger.One;
			BigInteger acc = b;

			for (; exponent > 1; exponent >>= 1) {
				if ((exponent & 1) != 0) {
					// if odd, multiply one more time by acc
					res = res * acc;
				}
				// acc = base^(2^i)
				//a limit where karatsuba performs a faster square than the square algorithm
				if (acc.numberLength == 1) {
					acc = acc * acc; // square
				} else {
					acc = new BigInteger(1, Square(acc.Digits, acc.numberLength, new int[acc.numberLength << 1]));
				}
			}
			// exponent == 1, multiply one more time
			res = res * acc;
			return res;
		}

		/**
		 *  Performs a<sup>2</sup>
		 *  @param a The number to square.
		 *  @param aLen The length of the number to square.
		 */
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

		/**
		 * Multiplies a number by a power of ten.
		 * This method is used in {@code BigDecimal} class.
		 * @param val the number to be multiplied
		 * @param exp a positive {@code long} exponent
		 * @return {@code val * 10<sup>exp</sup>}
		 */
		public static BigInteger MultiplyByTenPow(BigInteger val, long exp) {
			// PRE: exp >= 0
			return ((exp < TenPows.Length)
			? MultiplyByPositiveInt(val, TenPows[(int)exp])
			: val * PowerOf10(exp));
		}

		/**
		 * It calculates a power of ten, which exponent could be out of 32-bit range.
		 * Note that internally this method will be used in the worst case with
		 * an exponent equals to: {@code Integer.MAX_VALUE - Integer.MIN_VALUE}.
		 * @param exp the exponent of power of ten, it must be positive.
		 * @return a {@code BigInteger} with value {@code 10<sup>exp</sup>}.
		 */

		public static BigInteger PowerOf10(long exp) {
			// PRE: exp >= 0
			int intExp = (int)exp;
			// "SMALL POWERS"
			if (exp < BigTenPows.Length) {
				// The largest power that fit in 'long' type
				return BigTenPows[intExp];
			} else if (exp <= 50) {
				// To calculate:    10^exp
				return BigMath.Pow(BigInteger.Ten, intExp);
			} else if (exp <= 1000) {
				// To calculate:    5^exp * 2^exp
				return BigMath.Pow(BigFivePows[1], intExp) << intExp;
			}
			// "LARGE POWERS"
			/*
			 * To check if there is free memory to allocate a BigInteger of the
			 * estimated size, measured in bytes: 1 + [exp / log10(2)]
			 */
			var byteArraySize = 1 + (exp / 2.4082399653118496);

			//if (byteArraySize > System.Diagnostics.Process.GetCurrentProcess().PeakVirtualMemorySize64) {
			//	// math.01=power of ten too big
			//	throw new ArithmeticException(Messages.math01); //$NON-NLS-1$
			//}

			// More than 128Mb
			if (byteArraySize > (128 * 1024))
				throw new ArithmeticException(Messages.math01);

			if (exp <= Int32.MaxValue) {
				// To calculate:    5^exp * 2^exp
				return BigMath.Pow(BigFivePows[1], intExp) << intExp;
			}
			/*
			 * "HUGE POWERS"
			 * 
			 * This branch probably won't be executed since the power of ten is too
			 * big.
			 */
			// To calculate:    5^exp
			BigInteger powerOfFive = BigMath.Pow(BigFivePows[1], Int32.MaxValue);
			BigInteger res = powerOfFive;
			long longExp = exp - Int32.MaxValue;

			intExp = (int)(exp % Int32.MaxValue);
			while (longExp > Int32.MaxValue) {
				res = res * powerOfFive;
				longExp -= Int32.MaxValue;
			}
			res = res * BigMath.Pow(BigFivePows[1], intExp);
			// To calculate:    5^exp << exp
			res = res << Int32.MaxValue;
			longExp = exp - Int32.MaxValue;
			while (longExp > Int32.MaxValue) {
				res = res << Int32.MaxValue;
				longExp -= Int32.MaxValue;
			}
			res = res << intExp;
			return res;
		}

		/**
		 * Multiplies a number by a power of five.
		 * This method is used in {@code BigDecimal} class.
		 * @param val the number to be multiplied
		 * @param exp a positive {@code int} exponent
		 * @return {@code val * 5<sup>exp</sup>}
		 */

		public static BigInteger MultiplyByFivePow(BigInteger val, int exp) {
			// PRE: exp >= 0
			if (exp < FivePows.Length) {
				return MultiplyByPositiveInt(val, FivePows[exp]);
			} else if (exp < BigFivePows.Length) {
				return val * BigFivePows[exp];
			} else {// Large powers of five
				return val * BigMath.Pow(BigFivePows[1], exp);
			}
		}

		/**
		 * Computes the value unsigned ((uint)a*(uint)b + (uint)c + (uint)d). This
		 * method could improve the readability and performance of the code.
		 * 
		 * @param a
		 *            parameter 1
		 * @param b
		 *            parameter 2
		 * @param c
		 *            parameter 3
		 * @param d
		 *            parameter 4
		 * @return value of expression
		 */

		public static long UnsignedMultAddAdd(int a, int b, int c, int d) {
			return (a & 0xFFFFFFFFL) * (b & 0xFFFFFFFFL) + (c & 0xFFFFFFFFL) + (d & 0xFFFFFFFFL);
		}

	}
}