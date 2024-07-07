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
			return Elementary.Add(a, b);
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
			return BigIntegerMath.ShiftRight(value, n);
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
			return BigIntegerMath.ShiftLeft(value, n);
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
			return BigIntegerMath.Negate(value);
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
			return BigIntegerMath.Divide(dividend, divisor);
		}


		/**
 * Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
 * The scale of the result is the difference of the scales of {@code this}
 * and {@code divisor}. If the exact result requires more digits, then the
 * scale is adjusted accordingly. For example, {@code 1/128 = 0.0078125}
 * which has a scale of {@code 7} and precision {@code 5}.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @return {@code this / divisor}.
 * @throws NullPointerException
 *             if {@code divisor == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 * @throws ArithmeticException
 *             if the result cannot be represented exactly.
 */
		public static BigDecimal Divide(BigDecimal a, BigDecimal b) {
			return BigDecimalMath.Divide(a, b);
		}

		/**
 * Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
 * The scale of the result is the scale of {@code this}. If rounding is
 * required to meet the specified scale, then the specified rounding mode
 * {@code roundingMode} is applied.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @param roundingMode
 *            rounding mode to be used to round the result.
 * @return {@code this / divisor} rounded according to the given rounding
 *         mode.
 * @throws NullPointerException
 *             if {@code divisor == null} or {@code roundingMode == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 * @throws ArithmeticException
 *             if {@code roundingMode == RoundingMode.UNNECESSARY} and
 *             rounding is necessary according to the scale of this.
 */
		public static BigDecimal Divide(BigDecimal a, BigDecimal b, RoundingMode roundingMode) {
			if (!Enum.IsDefined(typeof(RoundingMode), roundingMode))
				throw new ArgumentException();

			return Divide(a, b, a.Scale, roundingMode);
		}

		/**
		 * Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
		 * As scale of the result the parameter {@code scale} is used. If rounding
		 * is required to meet the specified scale, then the specified rounding mode
		 * {@code roundingMode} is applied.
		 *
		 * @param divisor
		 *            value by which {@code this} is divided.
		 * @param scale
		 *            the scale of the result returned.
		 * @param roundingMode
		 *            rounding mode to be used to round the result.
		 * @return {@code this / divisor} rounded according to the given rounding
		 *         mode.
		 * @throws NullPointerException
		 *             if {@code divisor == null} or {@code roundingMode == null}.
		 * @throws ArithmeticException
		 *             if {@code divisor == 0}.
		 * @throws ArithmeticException
		 *             if {@code roundingMode == RoundingMode.UNNECESSAR}Y and
		 *             rounding is necessary according to the given scale and given
		 *             precision.
		 */

		public static BigDecimal Divide(BigDecimal a, BigDecimal b, int scale, RoundingMode roundingMode) {
			return BigDecimalMath.Divide(a, b, scale, roundingMode);
		}

		/**
 * Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
 * The result is rounded according to the passed context {@code mc}. If the
 * passed math context specifies precision {@code 0}, then this call is
 * equivalent to {@code this.divide(divisor)}.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @param mc
 *            rounding mode and precision for the result of this operation.
 * @return {@code this / divisor}.
 * @throws NullPointerException
 *             if {@code divisor == null} or {@code mc == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 * @throws ArithmeticException
 *             if {@code mc.getRoundingMode() == UNNECESSARY} and rounding
 *             is necessary according {@code mc.getPrecision()}.
 */

		public static BigDecimal Divide(BigDecimal a, BigDecimal b, MathContext context) {
			return BigDecimalMath.Divide(a, b, context);
		}

		/**
 * Returns a new {@code BigDecimal} whose value is the integral part of
 * {@code this / divisor}. The quotient is rounded down towards zero to the
 * next integer. For example, {@code 0.5/0.2 = 2}.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @return integral part of {@code this / divisor}.
 * @throws NullPointerException
 *             if {@code divisor == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 */

		public static BigDecimal DivideToIntegral(BigDecimal a, BigDecimal b) {
			return BigDecimalMath.DivideToIntegralValue(a, b);
		}

		/**
 * Returns a new {@code BigDecimal} whose value is the integral part of
 * {@code this / divisor}. The quotient is rounded down towards zero to the
 * next integer. The rounding mode passed with the parameter {@code mc} is
 * not considered. But if the precision of {@code mc > 0} and the integral
 * part requires more digits, then an {@code ArithmeticException} is thrown.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @param mc
 *            math context which determines the maximal precision of the
 *            result.
 * @return integral part of {@code this / divisor}.
 * @throws NullPointerException
 *             if {@code divisor == null} or {@code mc == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 * @throws ArithmeticException
 *             if {@code mc.getPrecision() > 0} and the result requires more
 *             digits to be represented.
 */

		public static BigDecimal DivideToIntegral(BigDecimal a, BigDecimal b, MathContext context) {
			return BigDecimalMath.DivideToIntegralValue(a, b, context);
		}

		/**
 * Returns a {@code BigDecimal} array which contains the integral part of
 * {@code this / divisor} at index 0 and the remainder {@code this %
 * divisor} at index 1. The quotient is rounded down towards zero to the
 * next integer.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @return {@code [this.divideToIntegralValue(divisor),
 *         this.remainder(divisor)]}.
 * @throws NullPointerException
 *             if {@code divisor == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 * @see #divideToIntegralValue
 * @see #remainder
 */

		public static BigDecimal DivideAndRemainder(BigDecimal a, BigDecimal b, out BigDecimal remainder) {
			return BigDecimalMath.DivideAndRemainder(a, b, out remainder);
		}

		/**
 * Returns a {@code BigDecimal} array which contains the integral part of
 * {@code this / divisor} at index 0 and the remainder {@code this %
 * divisor} at index 1. The quotient is rounded down towards zero to the
 * next integer. The rounding mode passed with the parameter {@code mc} is
 * not considered. But if the precision of {@code mc > 0} and the integral
 * part requires more digits, then an {@code ArithmeticException} is thrown.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @param mc
 *            math context which determines the maximal precision of the
 *            result.
 * @return {@code [this.divideToIntegralValue(divisor),
 *         this.remainder(divisor)]}.
 * @throws NullPointerException
 *             if {@code divisor == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 * @see #divideToIntegralValue
 * @see #remainder
 */
		public static BigDecimal DivideAndRemainder(BigDecimal a,
			BigDecimal b,
			MathContext context,
			out BigDecimal remainder) {
			return BigDecimalMath.DivideAndRemainder(a, b, context, out remainder);
		}

		/**
* Returns a new {@code BigDecimal} whose value is {@code this % divisor}.
* <p>
* The remainder is defined as {@code this -
* this.divideToIntegralValue(divisor) * divisor}.
*
* @param divisor
*            value by which {@code this} is divided.
* @return {@code this % divisor}.
* @throws NullPointerException
*             if {@code divisor == null}.
* @throws ArithmeticException
*             if {@code divisor == 0}.
*/
		public static BigDecimal Remainder(BigDecimal a, BigDecimal b) {
			BigDecimal remainder;
			DivideAndRemainder(a, b, out remainder);
			return remainder;
		}

		/**
 * Returns a new {@code BigDecimal} whose value is {@code this % divisor}.
 * <p>
 * The remainder is defined as {@code this -
 * this.divideToIntegralValue(divisor) * divisor}.
 * <p>
 * The specified rounding mode {@code mc} is used for the division only.
 *
 * @param divisor
 *            value by which {@code this} is divided.
 * @param mc
 *            rounding mode and precision to be used.
 * @return {@code this % divisor}.
 * @throws NullPointerException
 *             if {@code divisor == null}.
 * @throws ArithmeticException
 *             if {@code divisor == 0}.
 * @throws ArithmeticException
 *             if {@code mc.getPrecision() > 0} and the result of {@code
 *             this.divideToIntegralValue(divisor, mc)} requires more digits
 *             to be represented.
 */
		public static BigDecimal Remainder(BigDecimal a, BigDecimal b, MathContext context) {
			BigDecimal remainder;
			DivideAndRemainder(a, b, context, out remainder);
			return remainder;

		}

		/**
* Returns a new {@code BigDecimal} whose value is {@code this *
* multiplicand}. The scale of the result is the sum of the scales of the
* two arguments.
*
* @param multiplicand
*            value to be multiplied with {@code this}.
* @return {@code this * multiplicand}.
* @throws NullPointerException
*             if {@code multiplicand == null}.
*/

		public static BigDecimal Multiply(BigDecimal value, BigDecimal multiplicand) {
			return BigDecimalMath.Multiply(value, multiplicand);
		}

		/**
 * Returns a new {@code BigDecimal} whose value is {@code this *
 * multiplicand}. The result is rounded according to the passed context
 * {@code mc}.
 *
 * @param multiplicand
 *            value to be multiplied with {@code this}.
 * @param mc
 *            rounding mode and precision for the result of this operation.
 * @return {@code this * multiplicand}.
 * @throws NullPointerException
 *             if {@code multiplicand == null} or {@code mc == null}.
 */

			public static BigDecimal Multiply(BigDecimal value, BigDecimal multiplicand, MathContext mc) {
			BigDecimal result = Multiply(value, multiplicand);

			result.InplaceRound(mc);
			return result;
		}


		/**
* Returns a new {@code BigDecimal} whose value is {@code this ^ n}. The
* scale of the result is {@code n} times the scales of {@code this}.
* <p>
* {@code x.pow(0)} returns {@code 1}, even if {@code x == 0}.
* <p>
* Implementation Note: The implementation is based on the ANSI standard
* X3.274-1996 algorithm.
*
* @param n
*            exponent to which {@code this} is raised.
* @return {@code this ^ n}.
* @throws ArithmeticException
*             if {@code n < 0} or {@code n > 999999999}.
*/
		public static BigDecimal Pow(BigDecimal number, int exp) {
			return BigDecimalMath.Pow(number, exp);
		}

		/**
 * Returns a new {@code BigDecimal} whose value is {@code this ^ n}. The
 * result is rounded according to the passed context {@code mc}.
 * <p>
 * Implementation Note: The implementation is based on the ANSI standard
 * X3.274-1996 algorithm.
 *
 * @param n
 *            exponent to which {@code this} is raised.
 * @param mc
 *            rounding mode and precision for the result of this operation.
 * @return {@code this ^ n}.
 * @throws ArithmeticException
 *             if {@code n < 0} or {@code n > 999999999}.
 */
		public static BigDecimal Pow(BigDecimal number, int exp, MathContext context) {
			return BigDecimalMath.Pow(number, exp, context);
		}

		/**
* Returns a new {@code BigDecimal} whose value is the absolute value of
* {@code this}. The scale of the result is the same as the scale of this.
*
* @return {@code abs(this)}
*/
		public static BigDecimal Abs(BigDecimal number) {
			return ((number.Sign < 0) ? -number : number);
		}

		/**
		 * Returns a new {@code BigDecimal} whose value is the absolute value of
		 * {@code this}. The result is rounded according to the passed context
		 * {@code mc}.
		 *
		 * @param mc
		 *            rounding mode and precision for the result of this operation.
		 * @return {@code abs(this)}
		 */

		public static BigDecimal Abs(BigDecimal value, MathContext mc) {
			return Abs(Round(value, mc));
		}

		/**
		 * Returns a new {@code BigDecimal} whose value is {@code +this}. The scale
		 * of the result is the same as the scale of this.
		 *
		 * @return {@code this}
		 */

		public static BigDecimal Plus(BigDecimal number) {
			return number;
		}

		/// <remarks>
		/// Returns a new <see cref="BigDecimal"/> whose value is <c>+this</c>.
		/// </remarks>
		/// <param name="mc">Rounding mode and precision for the result of this operation.</param>
		/// <remarks>
		/// The result is rounded according to the passed context <paramref name="mc"/>.
		/// </remarks>
		/// <returns>
		/// Returns this decimal value rounded.
		/// </returns>
		public static BigDecimal Plus(BigDecimal number, MathContext mc) {
			return Round(number, mc);
		}

		/**
 * Returns a new {@code BigDecimal} whose value is the {@code -this}. The
 * scale of the result is the same as the scale of this.
 *
 * @return {@code -this}
 */

		public static BigDecimal Negate(BigDecimal number) {
			if (number.BitLength < 63 || (number.BitLength == 63 && number.SmallValue != Int64.MinValue)) {
				return BigDecimal.Create(-number.SmallValue, number.Scale);
			}

			return new BigDecimal(-number.UnscaledValue, number.Scale);
		}

		/**
		 * Returns a new {@code BigDecimal} whose value is the {@code -this}. The
		 * result is rounded according to the passed context {@code mc}.
		 *
		 * @param mc
		 *            rounding mode and precision for the result of this operation.
		 * @return {@code -this}
		 */

		public static BigDecimal Negate(BigDecimal number, MathContext mc) {
			return Negate(Round(number, mc));
		}

		/**
 * Returns a new {@code BigDecimal} whose value is {@code this}, rounded
 * according to the passed context {@code mc}.
 * <p>
 * If {@code mc.precision = 0}, then no rounding is performed.
 * <p>
 * If {@code mc.precision > 0} and {@code mc.roundingMode == UNNECESSARY},
 * then an {@code ArithmeticException} is thrown if the result cannot be
 * represented exactly within the given precision.
 *
 * @param mc
 *            rounding mode and precision for the result of this operation.
 * @return {@code this} rounded according to the passed context.
 * @throws ArithmeticException
 *             if {@code mc.precision > 0} and {@code mc.roundingMode ==
 *             UNNECESSARY} and this cannot be represented within the given
 *             precision.
 */
		public static BigDecimal Round(BigDecimal number, MathContext mc) {
			var thisBD = new BigDecimal(number.UnscaledValue, number.Scale);

			thisBD.InplaceRound(mc);
			return thisBD;
		}

		public static BigDecimal Min(BigDecimal a, BigDecimal val) {
			return ((a.CompareTo(val) <= 0) ? a : val);
		}

		/**
		 * Returns the maximum of this {@code BigDecimal} and {@code val}.
		 *
		 * @param val
		 *            value to be used to compute the maximum with this.
		 * @return {@code max(this, val}.
		 * @throws NullPointerException
		 *             if {@code val == null}.
		 */

		public static BigDecimal Max(BigDecimal a, BigDecimal val) {
			return ((a.CompareTo(val) >= 0) ? a : val);
		}


		/**
 * Returns a new {@code BigDecimal} whose value is {@code this} 10^{@code n}.
 * The scale of the result is {@code this.scale()} - {@code n}.
 * The precision of the result is the precision of {@code this}.
 * <p>
 * This method has the same effect as {@link #movePointRight}, except that
 * the precision is not changed.
 *
 * @param n
 *            number of places the decimal point has to be moved.
 * @return {@code this * 10^n}
 */

		public static BigDecimal ScaleByPowerOfTen(BigDecimal number, int n) {
			long newScale = number.Scale - (long) n;
			if (number.BitLength < 64) {
				//Taking care when a 0 is to be scaled
				if (number.SmallValue == 0) {
					return BigDecimal.GetZeroScaledBy(newScale);
				}

				return BigDecimal.Create(number.SmallValue, BigDecimal.ToIntScale(newScale));
			}

			return new BigDecimal(number.UnscaledValue, BigDecimal.ToIntScale(newScale));
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
			return BigIntegerMath.Remainder(dividend, divisor);
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
			return BigIntegerMath.DivideAndRemainder(dividend, divisor, out remainder);
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
			return BigIntegerMath.Mod(value, m);
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
			return BigIntegerMath.ModInverse(value, m);
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
			return BigIntegerMath.ModPow(value, exponent, m);
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
			return BigIntegerMath.Pow(value, exp);
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
			return BigIntegerMath.Gcd(a, b);
		}

		/// <summary>
		/// Computes the absolute value of the given <see cref="BigInteger"/>
		/// </summary>
		/// <returns>
		/// Returns an instance of <see cref="BigInteger"/> that represents the
		/// absolute value of this instance.
		/// </returns>
		public static BigInteger Abs(BigInteger value) {
			return BigIntegerMath.Abs(value);
		}

		/**
* Returns a new {@code BigDecimal} instance with the specified scale.
* <p>
* If the new scale is greater than the old scale, then additional zeros are
* added to the unscaled value. In this case no rounding is necessary.
* <p>
* If the new scale is smaller than the old scale, then trailing digits are
* removed. If these trailing digits are not zero, then the remaining
* unscaled value has to be rounded. For this rounding operation the
* specified rounding mode is used.
*
* @param newScale
*            scale of the result returned.
* @param roundingMode
*            rounding mode to be used to round the result.
* @return a new {@code BigDecimal} instance with the specified scale.
* @throws NullPointerException
*             if {@code roundingMode == null}.
* @throws ArithmeticException
*             if {@code roundingMode == ROUND_UNNECESSARY} and rounding is
*             necessary according to the given scale.
*/

		public static BigDecimal Scale(BigDecimal number, int newScale, RoundingMode roundingMode) {
			if (!Enum.IsDefined(typeof(RoundingMode), roundingMode))
				throw new ArgumentException();

			return BigDecimalMath.Scale(number, newScale, roundingMode);
		}

		/**
 * Returns a new {@code BigDecimal} instance with the specified scale. If
 * the new scale is greater than the old scale, then additional zeros are
 * added to the unscaled value. If the new scale is smaller than the old
 * scale, then trailing zeros are removed. If the trailing digits are not
 * zeros then an ArithmeticException is thrown.
 * <p>
 * If no exception is thrown, then the following equation holds: {@code
 * x.setScale(s).compareTo(x) == 0}.
 *
 * @param newScale
 *            scale of the result returned.
 * @return a new {@code BigDecimal} instance with the specified scale.
 * @throws ArithmeticException
 *             if rounding would be necessary.
 */
		public static BigDecimal Scale(BigDecimal number, int newScale) {
			return Scale(number, newScale, RoundingMode.Unnecessary);
		}

		/**
* Returns a new {@code BigDecimal} instance where the decimal point has
* been moved {@code n} places to the left. If {@code n < 0} then the
* decimal point is moved {@code -n} places to the right.
* <p>
* The result is obtained by changing its scale. If the scale of the result
* becomes negative, then its precision is increased such that the scale is
* zero.
* <p>
* Note, that {@code movePointLeft(0)} returns a result which is
* mathematically equivalent, but which has {@code scale >= 0}.
*
* @param n
*            number of placed the decimal point has to be moved.
* @return {@code this * 10^(-n}).
*/

		public static BigDecimal MovePointLeft(BigDecimal number, int n) {
			return BigDecimalMath.MovePoint(number, number.Scale + (long) n);
		}

		/**
 * Returns a new {@code BigDecimal} instance where the decimal point has
 * been moved {@code n} places to the right. If {@code n < 0} then the
 * decimal point is moved {@code -n} places to the left.
 * <p>
 * The result is obtained by changing its scale. If the scale of the result
 * becomes negative, then its precision is increased such that the scale is
 * zero.
 * <p>
 * Note, that {@code movePointRight(0)} returns a result which is
 * mathematically equivalent, but which has scale >= 0.
 *
 * @param n
 *            number of placed the decimal point has to be moved.
 * @return {@code this * 10^n}.
 */

		public static BigDecimal MovePointRight(BigDecimal number, int n) {
			return BigDecimalMath.MovePoint(number, number.Scale - (long) n);
		}

		/**
 * Returns the unit in the last place (ULP) of this {@code BigDecimal}
 * instance. An ULP is the distance to the nearest big decimal with the same
 * precision.
 * <p>
 * The amount of a rounding error in the evaluation of a floating-point
 * operation is often expressed in ULPs. An error of 1 ULP is often seen as
 * a tolerable error.
 * <p>
 * For class {@code BigDecimal}, the ULP of a number is simply 10^(-scale).
 * <p>
 * For example, {@code new BigDecimal(0.1).ulp()} returns {@code 1E-55}.
 *
 * @return unit in the last place (ULP) of this {@code BigDecimal} instance.
 */

		public static BigDecimal Ulp(BigDecimal value) {
			return BigDecimal.Create(1, value.Scale);
		}

		/// <summary>
		/// Adds a value to the current instance of <see cref="BigDecimal"/>,
		/// rounding the result according to the provided context.
		/// </summary>
		/// <param name="augend">The value to be added to this instance.</param>
		/// <param name="mc">The rounding mode and precision for the result of 
		/// this operation.</param>
		/// <returns>
		/// Returns a new <see cref="BigDecimal"/> whose value is <c>this + <paramref name="augend"/></c>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// If the given <paramref name="augend"/> or <paramref name="mc"/> is <c>null</c>.
		/// </exception>
		public static BigDecimal Add(BigDecimal value, BigDecimal augend, MathContext mc) {
			return BigDecimalMath.Add(value, augend, mc);
		}

		/// <summary>
		/// Adds a value to the current instance of <see cref="BigDecimal"/>.
		/// The scale of the result is the maximum of the scales of the two arguments.
		/// </summary>
		/// <param name="augend">The value to be added to this instance.</param>
		/// <returns>
		/// Returns a new {@code BigDecimal} whose value is <c>this + <paramref name="augend"/></c>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// If the given <paramref name="augend"/> is <c>null</c>.
		/// </exception>
		public static BigDecimal Add(BigDecimal value, BigDecimal augend) {
			return BigDecimalMath.Add(value, augend);
		}

		/// <summary>
		/// Subtracts the given value from this instance of <see cref="BigDecimal"/>.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="subtrahend">The value to be subtracted from this <see cref="BigDecimal"/>.</param>
		/// <returns>
		/// Returns an instance of <see cref="BigDecimal"/> that is the result of the
		/// subtraction of the given <paramref name="subtrahend"/> from this instance.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// If the given <paramref name="subtrahend"/> is <c>null</c>.
		/// </exception>
		public static BigDecimal Subtract(BigDecimal value, BigDecimal subtrahend) {
			return BigDecimalMath.Subtract(value, subtrahend);
		}

		/// <summary>
		/// Subtracts the given value from this instance of <see cref="BigDecimal"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This overload rounds the result of the operation to the <paramref name="mc">context</paramref>
		/// provided as argument.
		/// </para>
		/// </remarks>
		/// <param name="subtrahend">The value to be subtracted from this <see cref="BigDecimal"/>.</param>
		/// <param name="mc">The context used to round the result of this operation.</param>
		/// <returns>
		/// Returns an instance of <see cref="BigDecimal"/> that is the result of the
		/// subtraction of the given <paramref name="subtrahend"/> from this instance.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// If either of the given <paramref name="subtrahend"/> or <paramref name="mc"/> are <c>null</c>.
		/// </exception>
		public static BigDecimal Subtract(BigDecimal value, BigDecimal subtrahend, MathContext mc) {
			return BigDecimalMath.Subtract(value, subtrahend, mc);
		}

		/**
 * Returns a new {@code BigDecimal} instance with the same value as {@code
 * this} but with a unscaled value where the trailing zeros have been
 * removed. If the unscaled value of {@code this} has n trailing zeros, then
 * the scale and the precision of the result has been reduced by n.
 *
 * @return a new {@code BigDecimal} instance equivalent to this where the
 *         trailing zeros of the unscaled value have been removed.
 */

		public static BigDecimal StripTrailingZeros(BigDecimal value) {
			int i = 1; // 1 <= i <= 18
			int lastPow = BigDecimal.TenPow.Length - 1;
			long newScale = value.Scale;

			if (value.IsZero) {
				return BigDecimal.Parse("0");
			}
			BigInteger strippedBI = value.UnscaledValue;
			BigInteger quotient;
			BigInteger remainder;

			// while the number is even...
			while (!BigInteger.TestBit(strippedBI, 0)) {
				// To divide by 10^i
				quotient = DivideAndRemainder(strippedBI, BigDecimal.TenPow[i], out remainder);
				// To look the remainder
				if (remainder.Sign == 0) {
					// To adjust the scale
					newScale -= i;
					if (i < lastPow) {
						// To set to the next power
						i++;
					}
					strippedBI = quotient;
				} else {
					if (i == 1) {
						// 'this' has no more trailing zeros
						break;
					}
					// To set to the smallest power of ten
					i = 1;
				}
			}
			return new BigDecimal(strippedBI, BigDecimal.ToIntScale(newScale));
		}

	}
}