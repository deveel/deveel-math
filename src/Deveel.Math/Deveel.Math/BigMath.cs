using System;

namespace Deveel.Math {
	public static class BigMath {
		/// <summary>
		/// Computes an addition between two big integer numbers
		/// </summary>
		/// <param name="a">The first term of the addition</param>
		/// <param name="b">The second term of the addition</param>
		/// <returns>Returns a new <see cref="BigInteger"/> that
		/// is the result of the addition of the two integers specified</returns>
		public static BigInteger Add(BigInteger a, BigInteger b) {
			return Elementary.add(a, b);
		}

		/// <summary>
		/// Subtracts a big integer value from another 
		/// </summary>
		/// <param name="a">The subtrahend value</param>
		/// <param name="b">The subtractor value</param>
		/// <returns>
		/// </returns>
		public static BigInteger Subtract(BigInteger a, BigInteger b) {
			return Elementary.subtract(a, b);
		}

		/// <summary>
		/// Shifts the given big integer on the right by the given distance
		/// </summary>
		/// <param name="value">The integer value to shif</param>
		/// <param name="n">The shift distance</param>
		/// <remarks>
		/// <para>
		/// For negative arguments, the result is also negative.The shift distance 
		/// may be negative which means that <paramref name="value"/> is shifted left.
		/// </para>
		/// <para>
		/// <strong>Note:</strong> Usage of this method on negative values is not recommended 
		/// as the current implementation is not efficient.
		/// </para>
		/// </remarks>
		/// <returns></returns>
		public static BigInteger ShiftRight(BigInteger value, int n) {
			if ((n == 0) || (value.Sign == 0)) {
				return value;
			}
			return ((n > 0)
				? BitLevel.ShiftRight(value, n)
				: BitLevel.ShiftLeft(
					value, -n));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="n"></param>
		/// <remarks>
		/// <para>
		/// The result is equivalent to <c>value * 2^n</c> if n is greater 
		/// than or equal to 0.
		/// The shift distance may be negative which means that <paramref name="value"/> is 
		/// shifted right.The result then corresponds to <c>floor(value / 2 ^ (-n))</c>.
		/// </para>
		/// <para>
		/// <strong>Note:</strong> Usage of this method on negative values is not recommended 
		/// as the current implementation is not efficient.
		/// </para>
		/// </remarks>
		/// <returns></returns>
		public static BigInteger ShiftLeft(BigInteger value, int n) {
			if ((n == 0) || (value.Sign == 0)) {
				return value;
			}
			return ((n > 0) ? BitLevel.ShiftLeft(value, n) : BitLevel.ShiftRight(value, -n));
		}

		/// <summary>
		/// Computes the bit per bit operator between two numbers
		/// </summary>
		/// <param name="a">The first term of the operation.</param>
		/// <param name="b">The second term of the oepration</param>
		/// <remarks>
		/// <strong>Note:</strong> Usage of this method is not recommended as 
		/// the current implementation is not efficient.
		/// </remarks>
		/// <returns>
		/// Returns a new <see cref="BigInteger"/> whose value is the result
		/// of an logical and between the given numbers.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// If either <paramref name="a"/> or <paramref name="b"/> is <c>null</c>.
		/// </exception>
		public static BigInteger And(BigInteger a, BigInteger b) {
			return Logical.And(a, b);
		}

		public static BigInteger Or(BigInteger a, BigInteger b) {
			return Logical.Or(a, b);
		}

		public static BigInteger XOr(BigInteger a, BigInteger b) {
			return Logical.Xor(a, b);
		}

		public static BigInteger Multiply(BigInteger a, BigInteger b) {
			// This let us to throw NullPointerException when val == null
			if (b.Sign == 0) {
				return BigInteger.Zero;
			}
			if (a.Sign == 0) {
				return BigInteger.Zero;
			}
			return Multiplication.Multiply(a, b);
		}

		/**
 * Returns a new {@code BigInteger} whose value is {@code this / divisor}.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @return {@code this / divisor}.
 * @throws NullPointerException
 *             if {@code divisor == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 */
		public static BigInteger Divide(BigInteger dividend, BigInteger divisor) {
			if (divisor.Sign == 0) {
				// math.17=BigInteger divide by zero
				throw new ArithmeticException(Messages.math17); //$NON-NLS-1$
			}
			int divisorSign = divisor.Sign;
			if (divisor.IsOne) {
				return ((divisor.Sign > 0) ? dividend : dividend.Negate());
			}
			int thisSign = dividend.Sign;
			int thisLen = dividend.numberLength;
			int divisorLen = divisor.numberLength;
			if (thisLen + divisorLen == 2) {
				long val = (dividend.digits[0] & 0xFFFFFFFFL)
				           / (divisor.digits[0] & 0xFFFFFFFFL);
				if (thisSign != divisorSign) {
					val = -val;
				}
				return BigInteger.FromInt64(val);
			}
			int cmp = ((thisLen != divisorLen) ? ((thisLen > divisorLen) ? 1 : -1)
				: Elementary.compareArrays(dividend.digits, divisor.digits, thisLen));
			if (cmp == BigInteger.EQUALS) {
				return ((thisSign == divisorSign) ? BigInteger.One : BigInteger.MinusOne);
			}
			if (cmp == BigInteger.LESS) {
				return BigInteger.Zero;
			}
			int resLength = thisLen - divisorLen + 1;
			int[] resDigits = new int[resLength];
			int resSign = ((thisSign == divisorSign) ? 1 : -1);
			if (divisorLen == 1) {
				Division.DivideArrayByInt(resDigits, dividend.digits, thisLen,
					divisor.digits[0]);
			} else {
				Division.Divide(resDigits, resLength, dividend.digits, thisLen,
					divisor.digits, divisorLen);
			}
			BigInteger result = new BigInteger(resSign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}
	}
}