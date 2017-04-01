using System;
using System.Collections.Generic;
using System.Text;

namespace Deveel.Math {
	public sealed partial class BigInteger {
		/// <summary>
		/// Computes the absolute value of this <see cref="BigInteger"/>
		/// </summary>
		/// <returns>
		/// Returns an instance of <see cref="BigInteger"/> that represents the
		/// absolute value of this instance.
		/// </returns>
		public BigInteger Abs() {
			return ((sign < 0) ? new BigInteger(1, numberLength, digits) : this);
		}

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
		public BigInteger SetBit(int n) {
			if (!TestBit(n)) {
				return BitLevel.FlipBit(this, n);
			}
			return this;
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
		public BigInteger ClearBit(int n) {
			if (TestBit(n)) {
				return BitLevel.FlipBit(this, n);
			}
			return this;
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
		public BigInteger FlipBit(int n) {
			if (n < 0) {
				// math.15=Negative bit address
				throw new ArithmeticException(Messages.math15); //$NON-NLS-1$
			}
			return BitLevel.FlipBit(this, n);
		}

		/**
		 * Returns the position of the lowest set bit in the two's complement
		 * representation of this {@code BigInteger}. If all bits are zero (this=0)
		 * then -1 is returned as result.
		 * <p>
		 * <b>Implementation Note:</b> Usage of this method is not recommended as
		 * the current implementation is not efficient.
		 *
		 * @return position of lowest bit if {@code this != 0}, {@code -1} otherwise
		 */

		public BigInteger Min(BigInteger val) {
			return ((this.CompareTo(val) == LESS) ? this : val);
		}

		/**
		 * Returns the maximum of this {@code BigInteger} and {@code val}.
		 *
		 * @param val
		 *            value to be used to compute the maximum with {@code this}
		 * @return {@code max(this, val)}
		 * @throws NullPointerException
		 *             if {@code val == null}
		 */
		public BigInteger Max(BigInteger val) {
			return ((this.CompareTo(val) == GREATER) ? this : val);
		}

		/**
 * Returns a new {@code BigInteger} whose value is greatest common divisor
 * of {@code this} and {@code val}. If {@code this==0} and {@code val==0}
 * then zero is returned, otherwise the result is positive.
 *
 * @param val
 *            value with which the greatest common divisor is computed.
 * @return {@code gcd(this, val)}.
 * @throws NullPointerException
 *             if {@code val == null}.
 */
		public BigInteger Gcd(BigInteger val) {
			BigInteger val1 = Abs();
			BigInteger val2 = val.Abs();
			// To avoid a possible division by zero
			if (val1.Sign == 0) {
				return val2;
			} else if (val2.Sign == 0) {
				return val1;
			}

			// Optimization for small operands
			// (op2.bitLength() < 64) and (op1.bitLength() < 64)
			if (((val1.numberLength == 1) || ((val1.numberLength == 2) && (val1.digits[1] > 0)))
			    && (val2.numberLength == 1 || (val2.numberLength == 2 && val2.digits[1] > 0))) {
				return BigInteger.FromInt64(Division.GcdBinary(val1.ToInt64(), val2.ToInt64()));
			}

			return Division.GcdBinary(val1.Copy(), val2.Copy());

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
		public bool IsProbablePrime(int certainty) {
			return Primality.IsProbablePrime(Abs(), certainty);
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
		public BigInteger NextProbablePrime() {
			if (sign < 0) {
				// math.1A=start < 0: {0}
				throw new ArithmeticException(String.Format(Messages.math1A, this)); //$NON-NLS-1$
			}
			return Primality.NextProbablePrime(this);
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
	}
}
