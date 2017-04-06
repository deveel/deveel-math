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

		/**
		 * Returns a new {@code BigInteger} whose value is {@code this & ~val}.
		 * Evaluating {@code x.andNot(val)} returns the same result as {@code
		 * x.and(val.not())}.
		 * <p>
		 * <b>Implementation Note:</b> Usage of this method is not recommended as
		 * the current implementation is not efficient.
		 *
		 * @param val
		 *            value to be not'ed and then and'ed with {@code this}.
		 * @return {@code this & ~val}.
		 * @throws NullPointerException
		 *             if {@code val == null}.
		 */
		public static BigInteger AndNot(BigInteger value, BigInteger other) {
			return Logical.AndNot(value, other);
		}

		/**
* Returns a new {@code BigInteger} whose value is {@code ~this}. The result
* of this operation is {@code -this-1}.
* <p>
* <b>Implementation Note:</b> Usage of this method is not recommended as
* the current implementation is not efficient.
*
* @return {@code ~this}.
*/
		public static BigInteger Not(BigInteger value) {
			return Logical.Not(value);
		}

		/// <summary>
		/// Computes the negation of this <see cref="BigInteger"/>.
		/// </summary>
		/// <returns>
		/// Returns an instance of <see cref="BigInteger"/> that is the negated value
		/// of this instance.
		/// </returns>
		public static BigInteger Negate(BigInteger value) {
			return ((value.Sign == 0) ? value : new BigInteger(-value.Sign, value.numberLength, value.digits));
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
				return ((divisor.Sign > 0) ? dividend : -dividend);
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
			int cmp = ((thisLen != divisorLen)
				? ((thisLen > divisorLen) ? 1 : -1)
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

		public static BigDecimal Divide(BigDecimal a, BigDecimal b) {
			return BigDecimalMath.Divide(a, b);
		}

		public static BigDecimal Divide(BigDecimal a, BigDecimal b, RoundingMode roundingMode) {
			return BigDecimalMath.Divide(a, b, roundingMode);
		}

		public static BigDecimal Divide(BigDecimal a, BigDecimal b, int roundingMode) {
			return BigDecimalMath.Divide(a, b, roundingMode);
		}

		public static BigDecimal Divide(BigDecimal a, BigDecimal b, int scale, RoundingMode roundingMode) {
			return BigDecimalMath.Divide(a, b, scale, roundingMode);
		}

		public static BigDecimal Divide(BigDecimal a, BigDecimal b, MathContext context) {
			return BigDecimalMath.Divide(a, b, context);
		}

		public static BigDecimal DivideToIntegral(BigDecimal a, BigDecimal b) {
			return BigDecimalMath.DivideToIntegralValue(a, b);
		}

		public static BigDecimal DivideToIntegral(BigDecimal a, BigDecimal b, MathContext context) {
			return BigDecimalMath.DivideToIntegralValue(a, b, context);
		}

		public static BigDecimal DivideAndRemainder(BigDecimal a, BigDecimal b, out BigDecimal remainder) {
			return BigDecimalMath.DivideAndRemainder(a, b, out remainder);
		}

		public static BigDecimal DivideAndRemainder(BigDecimal a, BigDecimal b, MathContext context, out BigDecimal remainder) {
			return BigDecimalMath.DivideAndRemainder(a, b, context, out remainder);
		}

		public static BigDecimal Remainder(BigDecimal a, BigDecimal b) {
			return BigDecimalMath.Remainder(a, b);
		}

		public static BigDecimal Remainder(BigDecimal a, BigDecimal b, MathContext context) {
			return BigDecimalMath.Remainder(a, b, context);
		}

		public static BigDecimal Pow(BigDecimal number, int exp) {
			return BigDecimalMath.Pow(number, exp);
		}

		public static BigDecimal Pow(BigDecimal number, int exp, MathContext context) {
			return BigDecimalMath.Pow(number, exp, context);
		}

		public static BigDecimal Abs(BigDecimal number) {
			return BigDecimalMath.Abs(number);
		}

		public static BigDecimal Abs(BigDecimal number, MathContext context) {
			return BigDecimalMath.Abs(number, context);
		}

		public static BigDecimal Plus(BigDecimal number) {
			return BigDecimalMath.Plus(number);
		}

		public static BigDecimal Plus(BigDecimal number, MathContext context) {
			return BigDecimalMath.Plus(number, context);
		}

		public static BigDecimal Negate(BigDecimal number) {
			return BigDecimalMath.Negate(number);
		}

		public static BigDecimal Negate(BigDecimal number, MathContext context) {
			return BigDecimalMath.Negate(number, context);
		}

		/**
 * Returns a new {@code BigInteger} whose value is {@code this % divisor}.
 * Regarding signs this methods has the same behavior as the % operator on
 * int's, i.e. the sign of the remainder is the same as the sign of this.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @return {@code this % divisor}.
 * @throws NullPointerException
 *             if {@code divisor == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 */
		public static BigInteger Remainder(BigInteger dividend, BigInteger divisor) {
			if (divisor.Sign == 0) {
				// math.17=BigInteger divide by zero
				throw new ArithmeticException(Messages.math17); //$NON-NLS-1$
			}
			int thisLen = dividend.numberLength;
			int divisorLen = divisor.numberLength;
			if (((thisLen != divisorLen)
				    ? ((thisLen > divisorLen) ? 1 : -1)
				    : Elementary.compareArrays(dividend.digits, divisor.digits, thisLen)) == BigInteger.LESS) {
				return dividend;
			}
			int resLength = divisorLen;
			int[] resDigits = new int[resLength];
			if (resLength == 1) {
				resDigits[0] = Division.RemainderArrayByInt(dividend.digits, thisLen,
					divisor.digits[0]);
			} else {
				int qLen = thisLen - divisorLen + 1;
				resDigits = Division.Divide(null, qLen, dividend.digits, thisLen,
					divisor.digits, divisorLen);
			}
			BigInteger result = new BigInteger(dividend.Sign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/**
 * Returns a {@code BigInteger} array which contains {@code this / divisor}
 * at index 0 and {@code this % divisor} at index 1.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @return {@code [this / divisor, this % divisor]}.
 * @throws NullPointerException
 *             if {@code divisor == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 * @see #divide
 * @see #remainder
 */
		public static BigInteger DivideAndRemainder(BigInteger dividend, BigInteger divisor, out BigInteger remainder) {
			int divisorSign = divisor.Sign;
			if (divisorSign == 0) {
				// math.17=BigInteger divide by zero
				throw new ArithmeticException(Messages.math17); //$NON-NLS-1$
			}
			int divisorLen = divisor.numberLength;
			int[] divisorDigits = divisor.digits;
			if (divisorLen == 1) {
				var values = Division.DivideAndRemainderByInteger(dividend, divisorDigits[0], divisorSign);
				remainder = values[1];
				return values[0];
			}

			int[] thisDigits = dividend.digits;
			int thisLen = dividend.numberLength;
			int cmp = (thisLen != divisorLen)
				? ((thisLen > divisorLen) ? 1 : -1)
				: Elementary.compareArrays(thisDigits, divisorDigits, thisLen);
			if (cmp < 0) {
				remainder = dividend;
				return BigInteger.Zero;
			}
			int thisSign = dividend.Sign;
			int quotientLength = thisLen - divisorLen + 1;
			int remainderLength = divisorLen;
			int quotientSign = ((thisSign == divisorSign) ? 1 : -1);
			int[] quotientDigits = new int[quotientLength];
			int[] remainderDigits = Division.Divide(quotientDigits, quotientLength,
				thisDigits, thisLen, divisorDigits, divisorLen);

			var quotient = new BigInteger(quotientSign, quotientLength, quotientDigits);
			remainder = new BigInteger(thisSign, remainderLength, remainderDigits);
			quotient.CutOffLeadingZeroes();
			remainder.CutOffLeadingZeroes();

			return quotient;
		}

		/**
 * Returns a new {@code BigInteger} whose value is {@code this mod m}. The
 * modulus {@code m} must be positive. The result is guaranteed to be in the
 * interval {@code [0, m)} (0 inclusive, m exclusive). The behavior of this
 * function is not equivalent to the behavior of the % operator defined for
 * the built-in {@code int}'s.
 *
 * @param m
 *            the modulus.
 * @return {@code this mod m}.
 * @throws NullPointerException
 *             if {@code m == null}.
 * @throws ArithmeticException
 *             if {@code m < 0}.
 */
		public static BigInteger Mod(BigInteger value, BigInteger m) {
			if (m.Sign <= 0) {
				// math.18=BigInteger: modulus not positive
				throw new ArithmeticException(Messages.math18); //$NON-NLS-1$
			}
			BigInteger rem = BigMath.Remainder(value, m);
			return ((rem.Sign < 0) ? rem + m : rem);
		}

		/**
 * Returns a new {@code BigInteger} whose value is {@code 1/this mod m}. The
 * modulus {@code m} must be positive. The result is guaranteed to be in the
 * interval {@code [0, m)} (0 inclusive, m exclusive). If {@code this} is
 * not relatively prime to m, then an exception is thrown.
 *
 * @param m
 *            the modulus.
 * @return {@code 1/this mod m}.
 * @throws NullPointerException
 *             if {@code m == null}
 * @throws ArithmeticException
 *             if {@code m < 0 or} if {@code this} is not relatively prime
 *             to {@code m}
 */
		public static BigInteger ModInverse(BigInteger value, BigInteger m) {
			if (m.Sign <= 0) {
				// math.18=BigInteger: modulus not positive
				throw new ArithmeticException(Messages.math18); //$NON-NLS-1$
			}
			// If both are even, no inverse exists
			if (!(BigInteger.TestBit(value, 0) || 
				BigInteger.TestBit(m, 0))) {
				// math.19=BigInteger not invertible.
				throw new ArithmeticException(Messages.math19); //$NON-NLS-1$
			}
			if (m.IsOne) {
				return BigInteger.Zero;
			}

			// From now on: (m > 1)
			BigInteger res = Division.ModInverseMontgomery(Abs(value) % m, m);
			if (res.Sign == 0) {
				// math.19=BigInteger not invertible.
				throw new ArithmeticException(Messages.math19); //$NON-NLS-1$
			}

			res = ((value.Sign < 0) ? m - res : res);
			return res;

		}

		/**
 * Returns a new {@code BigInteger} whose value is {@code this^exponent mod
 * m}. The modulus {@code m} must be positive. The result is guaranteed to
 * be in the interval {@code [0, m)} (0 inclusive, m exclusive). If the
 * exponent is negative, then {@code this.modInverse(m)^(-exponent) mod m)}
 * is computed. The inverse of this only exists if {@code this} is
 * relatively prime to m, otherwise an exception is thrown.
 *
 * @param exponent
 *            the exponent.
 * @param m
 *            the modulus.
 * @return {@code this^exponent mod val}.
 * @throws NullPointerException
 *             if {@code m == null} or {@code exponent == null}.
 * @throws ArithmeticException
 *             if {@code m < 0} or if {@code exponent<0} and this is not
 *             relatively prime to {@code m}.
 */
		public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger m) {
			if (m.Sign <= 0) {
				// math.18=BigInteger: modulus not positive
				throw new ArithmeticException(Messages.math18); //$NON-NLS-1$
			}
			BigInteger b = value;

			if (m.IsOne | (exponent.Sign > 0 & b.Sign == 0)) {
				return BigInteger.Zero;
			}
			if (b.Sign == 0 && exponent.Sign == 0) {
				return BigInteger.One;
			}
			if (exponent.Sign < 0) {
				b = BigMath.ModInverse(value, m);
				exponent = -exponent;
			}
			// From now on: (m > 0) and (exponent >= 0)
			BigInteger res = (BigInteger.TestBit(m, 0))
				? Division.OddModPow(Abs(b),
					exponent, m)
				: Division.EvenModPow(Abs(b), exponent, m);
			if ((b.Sign < 0) && BigInteger.TestBit(exponent, 0)) {
				// -b^e mod m == ((-1 mod m) * (b^e mod m)) mod m
				res = ((m - BigInteger.One) * res) % m;
			}
			// else exponent is even, so base^exp is positive
			return res;
		}

		/**
 * Returns a new {@code BigInteger} whose value is {@code this ^ exp}.
 *
 * @param exp
 *            exponent to which {@code this} is raised.
 * @return {@code this ^ exp}.
 * @throws ArithmeticException
 *             if {@code exp < 0}.
 */
		public static BigInteger Pow(BigInteger value, int exp) {
			if (exp < 0) {
				// math.16=Negative exponent
				throw new ArithmeticException(Messages.math16); //$NON-NLS-1$
			}
			if (exp == 0) {
				return BigInteger.One;
			} else if (exp == 1 ||
			           value.Equals(BigInteger.One) || value.Equals(BigInteger.Zero)) {
				return value;
			}

			// if even take out 2^x factor which we can
			// calculate by shifting.
			if (!BigInteger.TestBit(value, 0)) {
				int x = 1;
				while (!BigInteger.TestBit(value, x)) {
					x++;
				}

				return BigInteger.GetPowerOfTwo(x * exp) * (Pow(value >> x, exp));
			}

			return Multiplication.Pow(value, exp);
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

		public static BigInteger Min(BigInteger a, BigInteger b) {
			return ((a.CompareTo(b) == BigInteger.LESS) ? a : b);
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
		public static BigInteger Max(BigInteger a, BigInteger b) {
			return ((a.CompareTo(b) == BigInteger.GREATER) ? a : b);
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
		public static BigInteger Gcd(BigInteger a, BigInteger b) {
			BigInteger val1 = Abs(a);
			BigInteger val2 = Abs(b);
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

		/// <summary>
		/// Computes the absolute value of the given <see cref="BigInteger"/>
		/// </summary>
		/// <returns>
		/// Returns an instance of <see cref="BigInteger"/> that represents the
		/// absolute value of this instance.
		/// </returns>
		public static BigInteger Abs(BigInteger value) {
			return ((value.Sign < 0) ? new BigInteger(1, value.numberLength, value.digits) : value);
		}
	}
}