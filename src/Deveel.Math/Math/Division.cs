﻿// 
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
 * Static library that provides all operations related with division and modular
 * arithmetic to {@link BigInteger}. Some methods are provided in both mutable
 * and immutable way. There are several variants provided listed below:
 * 
 * <ul type="circle">
 * <li> <b>Division</b>
 * <ul type="circle">
 * <li>{@link BigInteger} division and remainder by {@link BigInteger}.</li>
 * <li>{@link BigInteger} division and remainder by {@code int}.</li>
 * <li><i>gcd</i> between {@link BigInteger} numbers.</li>
 * </ul>
 * </li>
 * <li> <b>Modular arithmetic </b>
 * <ul type="circle">
 * <li>Modular exponentiation between {@link BigInteger} numbers.</li>
 * <li>Modular inverse of a {@link BigInteger} numbers.</li>
 * </ul>
 * </li>
 *</ul>
 */

	internal static class Division {
		/**
		 * Divides the array 'a' by the array 'b' and gets the quotient and the
		 * remainder. Implements the Knuth's division algorithm. See D. Knuth, The
		 * Art of Computer Programming, vol. 2. Steps D1-D8 correspond the steps in
		 * the algorithm description.
		 * 
		 * @param quot the quotient
		 * @param quotLength the quotient's length
		 * @param a the dividend
		 * @param aLength the dividend's length
		 * @param b the divisor
		 * @param bLength the divisor's length
		 * @return the remainder
		 */

		public static int[] Divide(int[] quot, int quotLength, int[] a, int aLength, int[] b, int bLength) {
			int[] normA = new int[aLength + 1]; // the normalized dividend
			// an extra byte is needed for correct shift
			int[] normB = new int[bLength + 1]; // the normalized divisor;
			int normBLength = bLength;
			/*
			 * Step D1: normalize a and b and put the results to a1 and b1 the
			 * normalized divisor's first digit must be >= 2^31
			 */
			int divisorShift = Utils.NumberOfLeadingZeros(b[bLength - 1]);
			if (divisorShift != 0) {
				BitLevel.ShiftLeft(normB, b, 0, divisorShift);
				BitLevel.ShiftLeft(normA, a, 0, divisorShift);
			} else {
				Array.Copy(a, 0, normA, 0, aLength);
				Array.Copy(b, 0, normB, 0, bLength);
			}
			int firstDivisorDigit = normB[normBLength - 1];
			// Step D2: set the quotient index
			int i = quotLength - 1;
			int j = aLength;

			while (i >= 0) {
				// Step D3: calculate a guess digit guessDigit
				int guessDigit = 0;
				if (normA[j] == firstDivisorDigit) {
					// set guessDigit to the largest unsigned int value
					guessDigit = -1;
				} else {
					long product = (((normA[j] & 0xffffffffL) << 32) + (normA[j - 1] & 0xffffffffL));
					long res = Division.DivideLongByInt(product, firstDivisorDigit);
					guessDigit = (int) res; // the quotient of divideLongByInt
					int rem = (int) (res >> 32); // the remainder of
					// divideLongByInt
					// decrease guessDigit by 1 while leftHand > rightHand
					if (guessDigit != 0) {
						long leftHand = 0;
						long rightHand = 0;
						bool rOverflowed = false;
						guessDigit++; // to have the proper value in the loop
						// below
						do {
							guessDigit--;
							if (rOverflowed)
								break;
							// leftHand always fits in an unsigned long
							leftHand = (guessDigit & 0xffffffffL)
							           *(normB[normBLength - 2] & 0xffffffffL);
							/*
							 * rightHand can overflow; in this case the loop
							 * condition will be true in the next step of the loop
							 */
							rightHand = ((long) rem << 32)
							            + (normA[j - 2] & 0xffffffffL);
							long longR = (rem & 0xffffffffL)
							             + (firstDivisorDigit & 0xffffffffL);
							/*
							 * checks that longR does not fit in an unsigned int;
							 * this ensures that rightHand will overflow unsigned
							 * long in the next step
							 */
							if (Utils.NumberOfLeadingZeros((int) Utils.URShift(longR, 32)) < 32)
								rOverflowed = true;
							else
								rem = (int) longR;
						} while ((leftHand ^ Int64.MinValue) > (rightHand ^ Int64.MinValue));

						//} while ((leftHand ^ Int64.MaxValue) > (rightHand ^ Int64.MaxValue));
						// while (((leftHand ^ 0x8000000000000000L) > (rightHand ^ 0x8000000000000000L))) ;
					}
				}
				// Step D4: multiply normB by guessDigit and subtract the production
				// from normA.
				if (guessDigit != 0) {
					int borrow = Division.MultiplyAndSubtract(normA, j - normBLength, normB, normBLength, guessDigit);
					// Step D5: check the borrow
					if (borrow != 0) {
						// Step D6: compensating addition
						guessDigit--;
						long carry = 0;
						for (int k = 0; k < normBLength; k++) {
							carry += (normA[j - normBLength + k] & 0xffffffffL)
							         + (normB[k] & 0xffffffffL);
							normA[j - normBLength + k] = (int) carry;
							carry = Utils.URShift(carry, 32);
						}
					}
				}
				if (quot != null)
					quot[i] = guessDigit;
				// Step D7
				j--;
				i--;
			}
			/*
			 * Step D8: we got the remainder in normA. Denormalize it id needed
			 */
			if (divisorShift != 0) {
				// reuse normB
				BitLevel.ShiftRight(normB, normBLength, normA, 0, divisorShift);
				return normB;
			}
			Array.Copy(normA, 0, normB, 0, bLength);
			return normA;
		}

		/**
		 * Divides an array by an integer value. Implements the Knuth's division
		 * algorithm. See D. Knuth, The Art of Computer Programming, vol. 2.
		 * 
		 * @param dest the quotient
		 * @param src the dividend
		 * @param srcLength the length of the dividend
		 * @param divisor the divisor
		 * @return remainder
		 */

		public static int DivideArrayByInt(int[] dest, int[] src, int srcLength, int divisor) {
			long rem = 0;
			long bLong = divisor & 0xffffffffL;

			for (int i = srcLength - 1; i >= 0; i--) {
				long temp = (rem << 32) | (src[i] & 0xffffffffL);
				long quot;
				if (temp >= 0) {
					quot = (temp/bLong);
					rem = (temp%bLong);
				} else {
					/*
					 * make the dividend positive shifting it right by 1 bit then
					 * get the quotient an remainder and correct them properly
					 */
					long aPos = Utils.URShift(temp, 1);
					long bPos = Utils.URShift(divisor, 1);
					quot = aPos/bPos;
					rem = aPos%bPos;
					// double the remainder and add 1 if a is odd
					rem = (rem << 1) + (temp & 1);
					if ((divisor & 1) != 0) {
						// the divisor is odd
						if (quot <= rem)
							rem -= quot;
						else {
							if (quot - rem <= bLong) {
								rem += bLong - quot;
								quot -= 1;
							} else {
								rem += (bLong << 1) - quot;
								quot -= 2;
							}
						}
					}
				}
				dest[i] = (int) (quot & 0xffffffffL);
			}
			return (int) rem;
		}

		/**
		 * Divides an array by an integer value. Implements the Knuth's division
		 * algorithm. See D. Knuth, The Art of Computer Programming, vol. 2.
		 * 
		 * @param src the dividend
		 * @param srcLength the length of the dividend
		 * @param divisor the divisor
		 * @return remainder
		 */

		public static int RemainderArrayByInt(int[] src, int srcLength, int divisor) {
			long result = 0;

			for (int i = srcLength - 1; i >= 0; i--) {
				long temp = (result << 32) + (src[i] & 0xffffffffL);
				long res = DivideLongByInt(temp, divisor);
				result = (int) (res >> 32);
			}
			return (int) result;
		}

		/**
		 * Divides a <code>BigInteger</code> by a signed <code>int</code> and
		 * returns the remainder.
		 * 
		 * @param dividend the BigInteger to be divided. Must be non-negative.
		 * @param divisor a signed int
		 * @return divide % divisor
		 */

		public static int Remainder(BigInteger dividend, int divisor) {
			return RemainderArrayByInt(dividend.Digits, dividend.numberLength, divisor);
		}

		/**
		 * Divides an unsigned long a by an unsigned int b. It is supposed that the
		 * most significant bit of b is set to 1, i.e. b < 0
		 * 
		 * @param a the dividend
		 * @param b the divisor
		 * @return the long value containing the unsigned integer remainder in the
		 *         left half and the unsigned integer quotient in the right half
		 */

		private static long DivideLongByInt(long a, int b) {
			long quot;
			long rem;
			long bLong = b & 0xffffffffL;

			if (a >= 0) {
				quot = (a/bLong);
				rem = (a%bLong);
			} else {
				/*
				 * Make the dividend positive shifting it right by 1 bit then get
				 * the quotient an remainder and correct them properly
				 */
				long aPos = Utils.URShift(a, 1);
				long bPos = Utils.URShift(b, 1);
				quot = aPos/bPos;
				rem = aPos%bPos;
				// double the remainder and add 1 if a is odd
				rem = (rem << 1) + (a & 1);
				if ((b & 1) != 0) {
					// the divisor is odd
					if (quot <= rem)
						rem -= quot;
					else {
						if (quot - rem <= bLong) {
							rem += bLong - quot;
							quot -= 1;
						} else {
							rem += (bLong << 1) - quot;
							quot -= 2;
						}
					}
				}
			}
			return (rem << 32) | (quot & 0xffffffffL);
		}

		/**
		 * Computes the quotient and the remainder after a division by an {@code int}
		 * number.
		 * 
		 * @return an array of the form {@code [quotient, remainder]}.
		 */

		public static BigInteger[] DivideAndRemainderByInteger(BigInteger val, int divisor, int divisorSign) {
			// res[0] is a quotient and res[1] is a remainder:
			int[] valDigits = val.Digits;
			int valLen = val.numberLength;
			int valSign = val.Sign;
			if (valLen == 1) {
				long a = (valDigits[0] & 0xffffffffL);
				long b = (divisor & 0xffffffffL);
				long quo = a/b;
				long rem = a%b;
				if (valSign != divisorSign)
					quo = -quo;
				if (valSign < 0)
					rem = -rem;
				return new BigInteger[] {
					BigInteger.FromInt64(quo),
					BigInteger.FromInt64(rem)
				};
			}
			int quotientLength = valLen;
			int quotientSign = ((valSign == divisorSign) ? 1 : -1);
			int[] quotientDigits = new int[quotientLength];
			int[] remainderDigits;
			remainderDigits = new int[] {
				Division.DivideArrayByInt(
					quotientDigits,
					valDigits,
					valLen,
					divisor)
			};
			BigInteger result0 = new BigInteger(quotientSign,
				quotientLength,
				quotientDigits);
			BigInteger result1 = new BigInteger(valSign, 1, remainderDigits);
			result0.CutOffLeadingZeroes();
			result1.CutOffLeadingZeroes();
			return new BigInteger[] {result0, result1};
		}

		/**
		 * Multiplies an array by int and subtracts it from a subarray of another
		 * array.
		 * 
		 * @param a the array to subtract from
		 * @param start the start element of the subarray of a
		 * @param b the array to be multiplied and subtracted
		 * @param bLen the length of b
		 * @param c the multiplier of b
		 * @return the carry element of subtraction
		 */

		public static int MultiplyAndSubtract(int[] a, int start, int[] b, int bLen, int c) {
			long carry0 = 0;
			long carry1 = 0;

			for (int i = 0; i < bLen; i++) {
				carry0 = Multiplication.UnsignedMultAddAdd(b[i], c, (int) carry0, 0);
				carry1 = (a[start + i] & 0xffffffffL) - (carry0 & 0xffffffffL) + carry1;
				a[start + i] = (int) carry1;
				carry1 >>= 32; // -1 or 0
				carry0 = Utils.URShift(carry0, 32);
			}

			carry1 = (a[start + bLen] & 0xffffffffL) - carry0 + carry1;
			a[start + bLen] = (int) carry1;
			return (int) (carry1 >> 32); // -1 or 0
		}

		/**
		 * @param m a positive modulus
		 * Return the greatest common divisor of op1 and op2,
		 * 
		 * @param op1
		 *            must be greater than zero
		 * @param op2
		 *            must be greater than zero
		 * @see BigInteger#gcd(BigInteger)
		 * @return {@code GCD(op1, op2)}
		 */

		public static BigInteger GcdBinary(BigInteger op1, BigInteger op2) {
			// PRE: (op1 > 0) and (op2 > 0)

			/*
			 * Divide both number the maximal possible times by 2 without rounding
					 * gcd(2*a, 2*b) = 2 * gcd(a,b)
			 */
			int lsb1 = op1.LowestSetBit;
			int lsb2 = op2.LowestSetBit;
			int pow2Count = System.Math.Min(lsb1, lsb2);

			BitLevel.InplaceShiftRight(op1, lsb1);
			BitLevel.InplaceShiftRight(op2, lsb2);

			BigInteger swap;
			// I want op2 > op1
			if (op1.CompareTo(op2) == BigInteger.GREATER) {
				swap = op1;
				op1 = op2;
				op2 = swap;
			}

			do {
				// INV: op2 >= op1 && both are odd unless op1 = 0

				// Optimization for small operands
				// (op2.bitLength() < 64) implies by INV (op1.bitLength() < 64)
				if ((op2.numberLength == 1)
				    || ((op2.numberLength == 2) && (op2.Digits[1] > 0))) {
					op2 = BigInteger.FromInt64(Division.GcdBinary(op1.ToInt64(),
						op2.ToInt64()));
					break;
				}

				// Implements one step of the Euclidean algorithm
				// To reduce one operand if it's much smaller than the other one
				if (op2.numberLength > op1.numberLength*1.2) {
					op2 = BigMath.Remainder(op2, op1);
					if (op2.Sign != 0)
						BitLevel.InplaceShiftRight(op2, op2.LowestSetBit);
				} else {
					// Use Knuth's algorithm of successive subtract and shifting
					do {
						Elementary.inplaceSubtract(op2, op1); // both are odd
						BitLevel.InplaceShiftRight(op2, op2.LowestSetBit); // op2 is even
					} while (op2.CompareTo(op1) >= BigInteger.EQUALS);
				}
				// now op1 >= op2
				swap = op2;
				op2 = op1;
				op1 = swap;
			} while (op1.Sign != 0);
			return op2 << pow2Count;
		}

		/**
		 * Performs the same as {@link #gcdBinary(BigInteger, BigInteger)}, but
		 * with numbers of 63 bits, represented in positives values of {@code long}
		 * type.
		 * 
		 * @param op1
		 *            a positive number
		 * @param op2
		 *            a positive number
		 * @see #gcdBinary(BigInteger, BigInteger)
		 * @return <code>GCD(op1, op2)</code>
		 */

		public static long GcdBinary(long op1, long op2) {
			// PRE: (op1 > 0) and (op2 > 0)
			int lsb1 = Utils.NumberOfTrailingZeros(op1);
			int lsb2 = Utils.NumberOfTrailingZeros(op2);
			int pow2Count = System.Math.Min(lsb1, lsb2);

			if (lsb1 != 0)
				op1 = Utils.URShift(op1, lsb1);
			if (lsb2 != 0)
				op2 = Utils.URShift(op2, lsb2);
			do {
				if (op1 >= op2) {
					op1 -= op2;
					op1 = Utils.URShift(op1, Utils.NumberOfTrailingZeros(op1));
				} else {
					op2 -= op1;
					op2 = Utils.URShift(op2, Utils.NumberOfTrailingZeros(op2));
				}
			} while (op1 != 0);
			return (op2 << pow2Count);
		}

		/**
		 * Calculates a.modInverse(p) Based on: Savas, E; Koc, C "The Montgomery Modular
		 * Inverse - Revised"
		 */

		public static BigInteger ModInverseMontgomery(BigInteger a, BigInteger p) {
			if (a.Sign == 0) {
				// ZERO hasn't inverse
				// math.19: BigInteger not invertible
				throw new ArithmeticException(Messages.math19);
			}

			if (!BigInteger.TestBit(p, 0)) {
				// montgomery inverse require even modulo
				return ModInverseLorencz(a, p);
			}

			int m = p.numberLength*32;
			// PRE: a \in [1, p - 1]
			BigInteger u, v, r, s;
			u = p.Copy(); // make copy to use inplace method
			v = a.Copy();
			int max = System.Math.Max(v.numberLength, u.numberLength);
			r = new BigInteger(1, 1, new int[max + 1]);
			s = new BigInteger(1, 1, new int[max + 1]);
			s.Digits[0] = 1;
			// s == 1 && v == 0

			int k = 0;

			int lsbu = u.LowestSetBit;
			int lsbv = v.LowestSetBit;
			int toShift;

			if (lsbu > lsbv) {
				BitLevel.InplaceShiftRight(u, lsbu);
				BitLevel.InplaceShiftRight(v, lsbv);
				BitLevel.InplaceShiftLeft(r, lsbv);
				k += lsbu - lsbv;
			} else {
				BitLevel.InplaceShiftRight(u, lsbu);
				BitLevel.InplaceShiftRight(v, lsbv);
				BitLevel.InplaceShiftLeft(s, lsbu);
				k += lsbv - lsbu;
			}

			r.Sign = 1;
			while (v.Sign > 0) {
				// INV v >= 0, u >= 0, v odd, u odd (except last iteration when v is even (0))

				while (u.CompareTo(v) > BigInteger.EQUALS) {
					Elementary.inplaceSubtract(u, v);
					toShift = u.LowestSetBit;
					BitLevel.InplaceShiftRight(u, toShift);
					Elementary.inplaceAdd(r, s);
					BitLevel.InplaceShiftLeft(s, toShift);
					k += toShift;
				}

				while (u.CompareTo(v) <= BigInteger.EQUALS) {
					Elementary.inplaceSubtract(v, u);
					if (v.Sign == 0)
						break;
					toShift = v.LowestSetBit;
					BitLevel.InplaceShiftRight(v, toShift);
					Elementary.inplaceAdd(s, r);
					BitLevel.InplaceShiftLeft(r, toShift);
					k += toShift;
				}
			}
			if (!u.IsOne) {
				// in u is stored the gcd
				// math.19: BigInteger not invertible.
				throw new ArithmeticException(Messages.math19);
			}
			if (r.CompareTo(p) >= BigInteger.EQUALS)
				Elementary.inplaceSubtract(r, p);

			r = p - r;

			// Have pair: ((BigInteger)r, (Integer)k) where r == a^(-1) * 2^k mod (module)		
			int n1 = CalcN(p);
			if (k > m) {
				r = MonPro(r, BigInteger.One, p, n1);
				k = k - m;
			}

			r = MonPro(r, BigInteger.GetPowerOfTwo(m - k), p, n1);
			return r;
		}

		/**
		 * Calculate the first digit of the inverse
		 */

		private static int CalcN(BigInteger a) {
			long m0 = a.Digits[0] & 0xFFFFFFFFL;
			long n2 = 1L; // this is a'[0]
			long powerOfTwo = 2L;
			do {
				if (((m0*n2) & powerOfTwo) != 0)
					n2 |= powerOfTwo;
				powerOfTwo <<= 1;
			} while (powerOfTwo < 0x100000000L);
			n2 = -n2;
			return (int) (n2 & 0xFFFFFFFFL);
		}

		/**
		 * @return bi == abs(2^exp)
		 */

		public static bool IsPowerOfTwo(BigInteger bi, int exp) {
			bool result = false;
			result = (exp >> 5 == bi.numberLength - 1)
			         && (bi.Digits[bi.numberLength - 1] == 1 << (exp & 31));
			if (result) {
				for (int i = 0; result && i < bi.numberLength - 1; i++)
					result = bi.Digits[i] == 0;
			}
			return result;
		}

		/**
		 * Calculate how many iteration of Lorencz's algorithm would perform the
		 * same operation
		 *
		 * @param bi
		 * @param n
		 * @return
		 */

		private static int HowManyIterations(BigInteger bi, int n) {
			int i = n - 1;
			if (bi.Sign > 0) {
				while (!BigInteger.TestBit(bi, i))
					i--;
				return n - 1 - i;
			} else {
				while (BigInteger.TestBit(bi, i))
					i--;
				return n - 1 - System.Math.Max(i, bi.LowestSetBit);
			}
		}

		/**
		 *
		 * Based on "New Algorithm for Classical Modular Inverse" Róbert Lórencz.
		 * LNCS 2523 (2002)
		 *
		 * @return a^(-1) mod m
		 */

		private static BigInteger ModInverseLorencz(BigInteger a, BigInteger modulo) {
			// PRE: a is coprime with modulo, a < modulo

			int max = System.Math.Max(a.numberLength, modulo.numberLength);
			int[] uDigits = new int[max + 1]; // enough place to make all the inplace operation
			int[] vDigits = new int[max + 1];
			Array.Copy(modulo.Digits, 0, uDigits, 0, modulo.numberLength);
			Array.Copy(a.Digits, 0, vDigits, 0, a.numberLength);
			BigInteger u = new BigInteger(modulo.Sign,
				modulo.numberLength,
				uDigits);
			BigInteger v = new BigInteger(a.Sign, a.numberLength, vDigits);

			BigInteger r = new BigInteger(0, 1, new int[max + 1]); // BigInteger.ZERO;
			BigInteger s = new BigInteger(1, 1, new int[max + 1]);
			s.Digits[0] = 1;
			// r == 0 && s == 1, but with enough place

			int coefU = 0, coefV = 0;
			int n = modulo.BitLength;
			int k;
			while (!IsPowerOfTwo(u, coefU) && !IsPowerOfTwo(v, coefV)) {
				// modification of original algorithm: I calculate how many times the algorithm will enter in the same branch of if
				k = HowManyIterations(u, n);

				if (k != 0) {
					BitLevel.InplaceShiftLeft(u, k);
					if (coefU >= coefV)
						BitLevel.InplaceShiftLeft(r, k);
					else {
						BitLevel.InplaceShiftRight(s, System.Math.Min(coefV - coefU, k));
						if (k - (coefV - coefU) > 0)
							BitLevel.InplaceShiftLeft(r, k - coefV + coefU);
					}
					coefU += k;
				}

				k = HowManyIterations(v, n);
				if (k != 0) {
					BitLevel.InplaceShiftLeft(v, k);
					if (coefV >= coefU)
						BitLevel.InplaceShiftLeft(s, k);
					else {
						BitLevel.InplaceShiftRight(r, System.Math.Min(coefU - coefV, k));
						if (k - (coefU - coefV) > 0)
							BitLevel.InplaceShiftLeft(s, k - coefU + coefV);
					}
					coefV += k;
				}

				if (u.Sign == v.Sign) {
					if (coefU <= coefV) {
						Elementary.completeInPlaceSubtract(u, v);
						Elementary.completeInPlaceSubtract(r, s);
					} else {
						Elementary.completeInPlaceSubtract(v, u);
						Elementary.completeInPlaceSubtract(s, r);
					}
				} else {
					if (coefU <= coefV) {
						Elementary.completeInPlaceAdd(u, v);
						Elementary.completeInPlaceAdd(r, s);
					} else {
						Elementary.completeInPlaceAdd(v, u);
						Elementary.completeInPlaceAdd(s, r);
					}
				}
				if (v.Sign == 0 || u.Sign == 0) {
					// math.19: BigInteger not invertible
					throw new ArithmeticException(Messages.math19);
				}
			}

			if (IsPowerOfTwo(v, coefV)) {
				r = s;
				if (v.Sign != u.Sign)
					u = -u;
			}
			if (BigInteger.TestBit(u, n)) {
				if (r.Sign < 0)
					r = -r;
				else
					r = modulo - r;
			}
			if (r.Sign < 0)
				r += modulo;

			return r;
		}

		private static BigInteger SquareAndMultiply(BigInteger x2, BigInteger a2, BigInteger exponent, BigInteger modulus, int n2) {
			BigInteger res = x2;
			for (int i = exponent.BitLength - 1; i >= 0; i--) {
				res = MonPro(res, res, modulus, n2);
				if (BitLevel.TestBit(exponent, i))
					res = MonPro(res, a2, modulus, n2);
			}
			return res;
		}

		/*Implements the Montgomery modular exponentiation based in <i>The sliding windows algorithm and the Mongomery
		 *Reduction</i>.
		 *@ar.org.fitc.ref "A. Menezes,P. van Oorschot, S. Vanstone - Handbook of Applied Cryptography";
		 *@see #oddModPow(BigInteger, BigInteger,
		 *                           BigInteger)
		 */

		private static BigInteger SlidingWindow(BigInteger x2, BigInteger a2, BigInteger exponent, BigInteger modulus, int n2) {
			// fill odd low pows of a2
			BigInteger[] pows = new BigInteger[8];
			BigInteger res = x2;
			int lowexp;
			BigInteger x3;
			int acc3;
			pows[0] = a2;

			x3 = MonPro(a2, a2, modulus, n2);
			for (int i = 1; i <= 7; i++)
				pows[i] = MonPro(pows[i - 1], x3, modulus, n2);

			for (int i = exponent.BitLength - 1; i >= 0; i--) {
				if (BitLevel.TestBit(exponent, i)) {
					lowexp = 1;
					acc3 = i;

					for (int j = System.Math.Max(i - 3, 0); j <= i - 1; j++) {
						if (BitLevel.TestBit(exponent, j)) {
							if (j < acc3) {
								acc3 = j;
								lowexp = (lowexp << (i - j)) ^ 1;
							} else
								lowexp = lowexp ^ (1 << (j - acc3));
						}
					}

					for (int j = acc3; j <= i; j++)
						res = MonPro(res, res, modulus, n2);
					res = MonPro(pows[(lowexp - 1) >> 1], res, modulus, n2);
					i = acc3;
				} else
					res = MonPro(res, res, modulus, n2);
			}
			return res;
		}

		/**
		 * Performs modular exponentiation using the Montgomery Reduction. It
		 * requires that all parameters be positive and the modulus be odd. >
		 * 
		 * @see BigInteger#modPow(BigInteger, BigInteger)
		 * @see #monPro(BigInteger, BigInteger, BigInteger, int)
		 * @see #slidingWindow(BigInteger, BigInteger, BigInteger, BigInteger,
		 *                      int)
		 * @see #squareAndMultiply(BigInteger, BigInteger, BigInteger, BigInteger,
		 *                      int)
		 */

		public static BigInteger OddModPow(BigInteger b,
			BigInteger exponent,
			BigInteger modulus) {
			// PRE: (base > 0), (exponent > 0), (modulus > 0) and (odd modulus)
			int k = (modulus.numberLength << 5); // r = 2^k
			// n-residue of base [base * r (mod modulus)]
			BigInteger a2 = (b << k) % modulus;
			// n-residue of base [1 * r (mod modulus)]
			BigInteger x2 = BigInteger.GetPowerOfTwo(k) % modulus;
			BigInteger res;
			// Compute (modulus[0]^(-1)) (mod 2^32) for odd modulus

			int n2 = CalcN(modulus);
			if (modulus.numberLength == 1)
				res = SquareAndMultiply(x2, a2, exponent, modulus, n2);
			else
				res = SlidingWindow(x2, a2, exponent, modulus, n2);

			return MonPro(res, BigInteger.One, modulus, n2);
		}

		/**
		 * Performs modular exponentiation using the Montgomery Reduction. It
		 * requires that all parameters be positive and the modulus be even. Based
		 * <i>The square and multiply algorithm and the Montgomery Reduction C. K.
		 * Koc - Montgomery Reduction with Even Modulus</i>. The square and
		 * multiply algorithm and the Montgomery Reduction.
		 * 
		 * @ar.org.fitc.ref "C. K. Koc - Montgomery Reduction with Even Modulus"
		 * @see BigInteger#modPow(BigInteger, BigInteger)
		 */

		public static BigInteger EvenModPow(BigInteger b,
			BigInteger exponent,
			BigInteger modulus) {
			// PRE: (base > 0), (exponent > 0), (modulus > 0) and (modulus even)
			// STEP 1: Obtain the factorization 'modulus'= q * 2^j.
			int j = modulus.LowestSetBit;
			BigInteger q = modulus >> j;

			// STEP 2: Compute x1 := base^exponent (mod q).
			BigInteger x1 = OddModPow(b, exponent, q);

			// STEP 3: Compute x2 := base^exponent (mod 2^j).
			BigInteger x2 = Pow2ModPow(b, exponent, j);

			// STEP 4: Compute q^(-1) (mod 2^j) and y := (x2-x1) * q^(-1) (mod 2^j)
			BigInteger qInv = ModPow2Inverse(q, j);
			BigInteger y = (x2 - x1) * qInv;
			InplaceModPow2(y, j);
			if (y.Sign < 0)
				y += BigInteger.GetPowerOfTwo(j);
			// STEP 5: Compute and return: x1 + q * y
			return x1 + (q * y);
		}

		/**
		 * It requires that all parameters be positive.
		 * 
		 * @return {@code base<sup>exponent</sup> mod (2<sup>j</sup>)}.
		 * @see BigInteger#modPow(BigInteger, BigInteger)
		 */

		private static BigInteger Pow2ModPow(BigInteger b, BigInteger exponent, int j) {
			// PRE: (base > 0), (exponent > 0) and (j > 0)
			BigInteger res = BigInteger.One;
			BigInteger e = exponent.Copy();
			BigInteger baseMod2toN = b.Copy();
			BigInteger res2;
			/*
			 * If 'base' is odd then it's coprime with 2^j and phi(2^j) = 2^(j-1);
			 * so we can reduce reduce the exponent (mod 2^(j-1)).
			 */
			if (BigInteger.TestBit(b, 0))
				InplaceModPow2(e, j - 1);
			InplaceModPow2(baseMod2toN, j);

			for (int i = e.BitLength - 1; i >= 0; i--) {
				res2 = res.Copy();
				InplaceModPow2(res2, j);
				res = res * res2;
				if (BitLevel.TestBit(e, i)) {
					res = res * baseMod2toN;
					InplaceModPow2(res, j);
				}
			}
			InplaceModPow2(res, j);
			return res;
		}

		private static void MonReduction(int[] res, BigInteger modulus, int n2) {
			/* res + m*modulus_digits */
			int[] modulusDigits = modulus.Digits;
			int modulusLen = modulus.numberLength;
			long outerCarry = 0;

			for (int i = 0; i < modulusLen; i++) {
				long innnerCarry = 0;
				int m = (int) Multiplication.UnsignedMultAddAdd(res[i], n2, 0, 0);
				for (int j = 0; j < modulusLen; j++) {
					innnerCarry = Multiplication.UnsignedMultAddAdd(m, modulusDigits[j], res[i + j], (int) innnerCarry);
					res[i + j] = (int) innnerCarry;
					innnerCarry = Utils.URShift(innnerCarry, 32);
				}

				outerCarry += (res[i + modulusLen] & 0xFFFFFFFFL) + innnerCarry;
				res[i + modulusLen] = (int) outerCarry;
				outerCarry = Utils.URShift(outerCarry, 32);
			}

			res[modulusLen << 1] = (int) outerCarry;

			/* res / r  */
			for (int j = 0; j < modulusLen + 1; j++)
				res[j] = res[j + modulusLen];
		}

		/**
		 * Implements the Montgomery Product of two integers represented by
		 * {@code int} arrays. The arrays are supposed in <i>little
		 * endian</i> notation.
		 * 
		 * @param a The first factor of the product.
		 * @param b The second factor of the product.
		 * @param modulus The modulus of the operations. Z<sub>modulus</sub>.
		 * @param n2 The digit modulus'[0].
		 * @ar.org.fitc.ref "C. K. Koc - Analyzing and Comparing Montgomery
		 *                  Multiplication Algorithms"
		 * @see #modPowOdd(BigInteger, BigInteger, BigInteger)
		 */

		private static BigInteger MonPro(BigInteger a, BigInteger b, BigInteger modulus, int n2) {
			int modulusLen = modulus.numberLength;
			int[] res = new int[(modulusLen << 1) + 1];
			Multiplication.MultArraysPap(a.Digits,
				System.Math.Min(modulusLen, a.numberLength),
				b.Digits,
				System.Math.Min(modulusLen, b.numberLength),
				res);
			MonReduction(res, modulus, n2);
			return FinalSubtraction(res, modulus);
		}

		/**
		 * Performs the final reduction of the Montgomery algorithm.
		 * @see monPro(BigInteger, BigInteger, BigInteger, long)
		 * @see monSquare(BigInteger, BigInteger, long)
		 */

		private static BigInteger FinalSubtraction(int[] res, BigInteger modulus) {
			// skipping leading zeros
			int modulusLen = modulus.numberLength;
			bool doSub = res[modulusLen] != 0;
			if (!doSub) {
				int[] modulusDigits = modulus.Digits;
				doSub = true;
				for (int i = modulusLen - 1; i >= 0; i--) {
					if (res[i] != modulusDigits[i]) {
						doSub = (res[i] != 0) && ((res[i] & 0xFFFFFFFFL) > (modulusDigits[i] & 0xFFFFFFFFL));
						break;
					}
				}
			}

			BigInteger result = new BigInteger(1, modulusLen + 1, res);

			// if (res >= modulusDigits) compute (res - modulusDigits)
			if (doSub)
				Elementary.inplaceSubtract(result, modulus);

			result.CutOffLeadingZeroes();
			return result;
		}

		/**
		 * @param x an odd positive number.
		 * @param n the exponent by which 2 is raised.
		 * @return {@code x<sup>-1</sup> (mod 2<sup>n</sup>)}.
		 */

		private static BigInteger ModPow2Inverse(BigInteger x, int n) {
			// PRE: (x > 0), (x is odd), and (n > 0)
			BigInteger y = new BigInteger(1, new int[1 << n]);
			y.numberLength = 1;
			y.Digits[0] = 1;
			y.Sign = 1;

			for (int i = 1; i < n; i++) {
				if (BitLevel.TestBit(x * y, i)) {
					// Adding 2^i to y (setting the i-th bit)
					y.Digits[i >> 5] |= (1 << (i & 31));
				}
			}
			return y;
		}

		/**
		 * Performs {@code x = x mod (2<sup>n</sup>)}.
		 * 
		 * @param x a positive number, it will store the result.
		 * @param n a positive exponent of {@code 2}.
		 */

		public static void InplaceModPow2(BigInteger x, int n) {
			// PRE: (x > 0) and (n >= 0)
			int fd = n >> 5;
			int leadingZeros;

			if ((x.numberLength < fd) || (x.BitLength <= n))
				return;
			leadingZeros = 32 - (n & 31);
			x.numberLength = fd + 1;
			x.Digits[fd] &= (leadingZeros < 32) ? (Utils.URShift(-1, leadingZeros)) : 0;
			x.CutOffLeadingZeroes();
		}
	}
}