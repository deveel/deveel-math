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
using System.Collections.Generic;
using System.Text;

namespace Deveel.Math {
	public sealed partial class BigInteger {
		/**
		 * Tests whether the bit at position n in {@code this} is set. The result is
		 * equivalent to {@code this & (2^n) != 0}.
		 * <p>
		 * <b>Implementation Note:</b> Usage of this method is not recommended as
		 * the current implementation is not efficient.
		 *
		 * @param n
		 *            position where the bit in {@code this} has to be inspected.
		 * @return {@code this & (2^n) != 0}.
		 * @throws ArithmeticException
		 *             if {@code n < 0}.
		 */
		public bool TestBit(int n) {
			if (n == 0) {
				return ((digits[0] & 1) != 0);
			}
			if (n < 0) {
				// math.15=Negative bit address
				throw new ArithmeticException(Messages.math15); //$NON-NLS-1$
			}
			int intCount = n >> 5;
			if (intCount >= numberLength) {
				return (sign < 0);
			}
			int digit = digits[intCount];
			n = (1 << (n & 31)); // int with 1 set to the needed position
			if (sign < 0) {
				int firstNonZeroDigit = FirstNonzeroDigit;
				if (intCount < firstNonZeroDigit) {
					return false;
				} else if (firstNonZeroDigit == intCount) {
					digit = -digit;
				} else {
					digit = ~digit;
				}
			}
			return ((digit & n) != 0);
		}

		/**
		 * Returns a new {@code BigInteger} which has the same binary representation
		 * as {@code this} but with the bit at position n set. The result is
		 * equivalent to {@code this | 2^n}.
		 * <p>
		 * <b>Implementation Note:</b> Usage of this method is not recommended as
		 * the current implementation is not efficient.
		 *
		 * @param n
		 *            position where the bit in {@code this} has to be set.
		 * @return {@code this | 2^n}.
		 * @throws ArithmeticException
		 *             if {@code n < 0}.
		 */
		public static BigInteger SetBit(BigInteger value, int n) {
			if (!value.TestBit(n)) {
				return BitLevel.FlipBit(value, n);
			}
			return value;
		}

		/**
		 * Returns a new {@code BigInteger} which has the same binary representation
		 * as {@code this} but with the bit at position n cleared. The result is
		 * equivalent to {@code this & ~(2^n)}.
		 * <p>
		 * <b>Implementation Note:</b> Usage of this method is not recommended as
		 * the current implementation is not efficient.
		 *
		 * @param n
		 *            position where the bit in {@code this} has to be cleared.
		 * @return {@code this & ~(2^n)}.
		 * @throws ArithmeticException
		 *             if {@code n < 0}.
		 */
		public static BigInteger ClearBit(BigInteger value, int n) {
			if (value.TestBit(n)) {
				return BitLevel.FlipBit(value, n);
			}
			return value;
		}


		/**
		 * Tests whether this {@code BigInteger} is probably prime. If {@code true}
		 * is returned, then this is prime with a probability beyond
		 * (1-1/2^certainty). If {@code false} is returned, then this is definitely
		 * composite. If the argument {@code certainty} <= 0, then this method
		 * returns true.
		 *
		 * @param certainty
		 *            tolerated primality uncertainty.
		 * @return {@code true}, if {@code this} is probably prime, {@code false}
		 *         otherwise.
		 */
		public static bool IsProbablePrime(BigInteger value, int certainty) {
			return Primality.IsProbablePrime(BigMath.Abs(value), certainty);
		}

		/**
		 * Returns the smallest integer x > {@code this} which is probably prime as
		 * a {@code BigInteger} instance. The probability that the returned {@code
		 * BigInteger} is prime is beyond (1-1/2^80).
		 *
		 * @return smallest integer > {@code this} which is robably prime.
		 * @throws ArithmeticException
		 *             if {@code this < 0}.
		 */
		public static BigInteger NextProbablePrime(BigInteger value) {
			if (value.Sign < 0) {
				// math.1A=start < 0: {0}
				throw new ArithmeticException(String.Format(Messages.math1A, value)); //$NON-NLS-1$
			}
			return Primality.NextProbablePrime(value);
		}

		/**
		 * Returns a random positive {@code BigInteger} instance in the range [0,
		 * 2^(bitLength)-1] which is probably prime. The probability that the
		 * returned {@code BigInteger} is prime is beyond (1-1/2^80).
		 * <p>
		 * <b>Implementation Note:</b> Currently {@code rnd} is ignored.
		 *
		 * @param bitLength
		 *            length of the new {@code BigInteger} in bits.
		 * @param rnd
		 *            random generator used to generate the new {@code BigInteger}.
		 * @return probably prime random {@code BigInteger} instance.
		 * @throws IllegalArgumentException
		 *             if {@code bitLength < 2}.
		 */
		public static BigInteger ProbablePrime(int bitLength, Random rnd) {
			return new BigInteger(bitLength, 100, rnd);
		}

		/**
* Returns a new {@code BigInteger} which has the same binary representation
* as {@code this} but with the bit at position n flipped. The result is
* equivalent to {@code this ^ 2^n}.
* <p>
* <b>Implementation Note:</b> Usage of this method is not recommended as
* the current implementation is not efficient.
*
* @param n
*            position where the bit in {@code this} has to be flipped.
* @return {@code this ^ 2^n}.
* @throws ArithmeticException
*             if {@code n < 0}.
*/
		public static BigInteger FlipBit(BigInteger value, int n) {
			if (n < 0) {
				// math.15=Negative bit address
				throw new ArithmeticException(Messages.math15); //$NON-NLS-1$
			}
			return BitLevel.FlipBit(value, n);
		}
	}
}
