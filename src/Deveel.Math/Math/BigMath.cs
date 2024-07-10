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
	/// Provides a set of methods for performing arithmetic operations on
	/// big integer and big decimal numbers.
	/// </summary>
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
			return Elementary.Subtract(a, b);
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

        /// <summary>
        /// Computes the bitwise AND-NOT operation between two numbers.
        /// </summary>
        /// <param name="value">
        /// The first term of the operation.
        /// </param>
        /// <param name="other">
        /// The value to be NOT'ed and then AND'ed with <paramref name="value"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// Evaluating <c>AndNot(x, value)</c> returns the same result as <c>And(x, Not(value))</c>
        /// </para>
		/// <para>
		/// <b>Implementation Note:</b> Usage of this method is not recommended as the current 
		/// implementation is not efficient.
        /// </para>
        /// </remarks>
        /// <returns></returns>
        public static BigInteger AndNot(BigInteger value, BigInteger other)
        {
            return Logical.AndNot(value, other);
		}

        /// <summary>
        /// Computes the bitwise NOT operation on the given number.
        /// </summary>
        /// <param name="value">
		/// The value to be NOT'ed.
		/// </param>
        /// <returns>
		/// Returns a new <see cref="BigInteger"/> whose value is the result of 
		/// the bitwise NOT operation
		/// </returns>
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

        /// <summary>
        /// Divides two <see cref="BigDecimal"/> numbers and returns the result
        /// of the division.
        /// </summary>
        /// <param name="a">
        /// The value that is to be divided.
        /// </param>
        /// <param name="b">
        /// The value to divide <paramref name="a"/> by.
        /// </param>
        /// <remarks>
        /// The scale of the result is the difference of the scales of <paramref name="a"/> and 
		/// <paramref name="b"/>. If the exact result requires more digits, 
		/// then the scale is adjusted accordingly. For example, <c>1/128 = 0.0078125</c> which 
		/// has a scale of <c>7</c> and precision <c>5</c>.
        /// </remarks>
        /// <returns>
		/// Returns an instance of <see cref="BigDecimal"/> that is the result of the division
		/// operation between the two numbers.
		/// </returns>
		/// <exception cref="ArithmeticException">
		/// Thrown if the value of <paramref name="b"/> is <c>0</c>, or if the result cannot be
		/// represented exactly.
		/// </exception>
        public static BigDecimal Divide(BigDecimal a, BigDecimal b) {
			return BigDecimalMath.Divide(a, b);
		}
 
		/// <summary>
		/// Divides two <see cref="BigDecimal"/> numbers and returns the result
		/// of the division, eventually rounded to the given scale and rounding mode.
		/// </summary>
		/// <param name="a">
		/// The value that is to be divided.
		/// </param>
		/// <param name="b">
		/// The value to divide <paramref name="a"/> by.
		/// </param>
		/// <param name="roundingMode">
		/// The rounding mode to be used to round the result.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="BigDecimal"/> that is the result of the division,
		/// eventually rounded to the given scale and rounding mode.
		/// </returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArithmeticException">
		/// Throw when <paramref name="b"/> is <c>0</c>, or if the <paramref name="roundingMode"/>
		/// is <see cref="RoundingMode.Unnecessary"/> and rounding is necessary according to the 
		/// scale of <paramref name="a"/>.
		/// </exception>
		public static BigDecimal Divide(BigDecimal a, BigDecimal b, RoundingMode roundingMode) {
			if (!Enum.IsDefined(typeof(RoundingMode), roundingMode))
				throw new ArgumentException();

			return Divide(a, b, a.Scale, roundingMode);
		}

		///**
		// * Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
		// * As scale of the result the parameter {@code scale} is used. If rounding
		// * is required to meet the specified scale, then the specified rounding mode
		// * {@code roundingMode} is applied.
		// *
		// * @param divisor
		// *            value by which {@code this} is divided.
		// * @param scale
		// *            the scale of the result returned.
		// * @param roundingMode
		// *            rounding mode to be used to round the result.
		// * @return {@code this / divisor} rounded according to the given rounding
		// *         mode.
		// * @throws NullPointerException
		// *             if {@code divisor == null} or {@code roundingMode == null}.
		// * @throws ArithmeticException
		// *             if {@code divisor == 0}.
		// * @throws ArithmeticException
		// *             if {@code roundingMode == RoundingMode.UNNECESSAR}Y and
		// *             rounding is necessary according to the given scale and given
		// *             precision.
		// */

		
		public static BigDecimal Divide(BigDecimal a, BigDecimal b, int scale, RoundingMode roundingMode) {
			return BigDecimalMath.Divide(a, b, scale, roundingMode);
		}

	//	/**
 //* Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
 //* The result is rounded according to the passed context {@code mc}. If the
 //* passed math context specifies precision {@code 0}, then this call is
 //* equivalent to {@code this.divide(divisor)}.
 //*
 //* @param divisor
 //*            value by which {@code this} is divided.
 //* @param mc
 //*            rounding mode and precision for the result of this operation.
 //* @return {@code this / divisor}.
 //* @throws NullPointerException
 //*             if {@code divisor == null} or {@code mc == null}.
 //* @throws ArithmeticException
 //*             if {@code divisor == 0}.
 //* @throws ArithmeticException
 //*             if {@code mc.getRoundingMode() == UNNECESSARY} and rounding
 //*             is necessary according {@code mc.getPrecision()}.
 //*/

		public static BigDecimal Divide(BigDecimal a, BigDecimal b, MathContext context) {
			return BigDecimalMath.Divide(a, b, context);
		}

	//	/**
 //* Returns a new {@code BigDecimal} whose value is the integral part of
 //* {@code this / divisor}. The quotient is rounded down towards zero to the
 //* next integer. For example, {@code 0.5/0.2 = 2}.
 //*
 //* @param divisor
 //*            value by which {@code this} is divided.
 //* @return integral part of {@code this / divisor}.
 //* @throws NullPointerException
 //*             if {@code divisor == null}.
 //* @throws ArithmeticException
 //*             if {@code divisor == 0}.
 //*/

		public static BigDecimal DivideToIntegral(BigDecimal a, BigDecimal b) {
			return BigDecimalMath.DivideToIntegralValue(a, b);
		}

	//	/**
 //* Returns a new {@code BigDecimal} whose value is the integral part of
 //* {@code this / divisor}. The quotient is rounded down towards zero to the
 //* next integer. The rounding mode passed with the parameter {@code mc} is
 //* not considered. But if the precision of {@code mc > 0} and the integral
 //* part requires more digits, then an {@code ArithmeticException} is thrown.
 //*
 //* @param divisor
 //*            value by which {@code this} is divided.
 //* @param mc
 //*            math context which determines the maximal precision of the
 //*            result.
 //* @return integral part of {@code this / divisor}.
 //* @throws NullPointerException
 //*             if {@code divisor == null} or {@code mc == null}.
 //* @throws ArithmeticException
 //*             if {@code divisor == 0}.
 //* @throws ArithmeticException
 //*             if {@code mc.getPrecision() > 0} and the result requires more
 //*             digits to be represented.
 //*/

		public static BigDecimal DivideToIntegral(BigDecimal a, BigDecimal b, MathContext context) {
			return BigDecimalMath.DivideToIntegralValue(a, b, context);
		}

	//	/**
 //* Returns a {@code BigDecimal} array which contains the integral part of
 //* {@code this / divisor} at index 0 and the remainder {@code this %
 //* divisor} at index 1. The quotient is rounded down towards zero to the
 //* next integer.
 //*
 //* @param divisor
 //*            value by which {@code this} is divided.
 //* @return {@code [this.divideToIntegralValue(divisor),
 //*         this.remainder(divisor)]}.
 //* @throws NullPointerException
 //*             if {@code divisor == null}.
 //* @throws ArithmeticException
 //*             if {@code divisor == 0}.
 //* @see #divideToIntegralValue
 //* @see #remainder
 //*/

		public static BigDecimal DivideAndRemainder(BigDecimal a, BigDecimal b, out BigDecimal remainder) {
			return BigDecimalMath.DivideAndRemainder(a, b, out remainder);
		}

	//	/**
 //* Returns a {@code BigDecimal} array which contains the integral part of
 //* {@code this / divisor} at index 0 and the remainder {@code this %
 //* divisor} at index 1. The quotient is rounded down towards zero to the
 //* next integer. The rounding mode passed with the parameter {@code mc} is
 //* not considered. But if the precision of {@code mc > 0} and the integral
 //* part requires more digits, then an {@code ArithmeticException} is thrown.
 //*
 //* @param divisor
 //*            value by which {@code this} is divided.
 //* @param mc
 //*            math context which determines the maximal precision of the
 //*            result.
 //* @return {@code [this.divideToIntegralValue(divisor),
 //*         this.remainder(divisor)]}.
 //* @throws NullPointerException
 //*             if {@code divisor == null}.
 //* @throws ArithmeticException
 //*             if {@code divisor == 0}.
 //* @see #divideToIntegralValue
 //* @see #remainder
 //*/
		public static BigDecimal DivideAndRemainder(BigDecimal a,
			BigDecimal b,
			MathContext context,
			out BigDecimal remainder) {
			return BigDecimalMath.DivideAndRemainder(a, b, context, out remainder);
		}

//		/**
//* Returns a new {@code BigDecimal} whose value is {@code this % divisor}.
//* <p>
//* The remainder is defined as {@code this -
//* this.divideToIntegralValue(divisor) * divisor}.
//*
//* @param divisor
//*            value by which {@code this} is divided.
//* @return {@code this % divisor}.
//* @throws NullPointerException
//*             if {@code divisor == null}.
//* @throws ArithmeticException
//*             if {@code divisor == 0}.
//*/
		public static BigDecimal Remainder(BigDecimal a, BigDecimal b) {
			BigDecimal remainder;
			DivideAndRemainder(a, b, out remainder);
			return remainder;
		}

	//	/**
 //* Returns a new {@code BigDecimal} whose value is {@code this % divisor}.
 //* <p>
 //* The remainder is defined as {@code this -
 //* this.divideToIntegralValue(divisor) * divisor}.
 //* <p>
 //* The specified rounding mode {@code mc} is used for the division only.
 //*
 //* @param divisor
 //*            value by which {@code this} is divided.
 //* @param mc
 //*            rounding mode and precision to be used.
 //* @return {@code this % divisor}.
 //* @throws NullPointerException
 //*             if {@code divisor == null}.
 //* @throws ArithmeticException
 //*             if {@code divisor == 0}.
 //* @throws ArithmeticException
 //*             if {@code mc.getPrecision() > 0} and the result of {@code
 //*             this.divideToIntegralValue(divisor, mc)} requires more digits
 //*             to be represented.
 //*/
		public static BigDecimal Remainder(BigDecimal a, BigDecimal b, MathContext context) {
			BigDecimal remainder;
			DivideAndRemainder(a, b, context, out remainder);
			return remainder;

		}

        /// <summary>
        /// Multiplies two <see cref="BigDecimal"/> numbers and the result
		/// scale is the sum of the scales of the two numbers.
        /// </summary>
        /// <param name="value">
        /// The <see cref="BigDecimal"/> number to be multiplied.
        /// </param>
        /// <param name="multiplicand">
        /// The <see cref="BigDecimal"/> number to multiply <paramref name="value"/> by.
        /// </param>
        /// <returns>
        /// Returns an instance of <see cref="BigDecimal"/> that is the result of the multiplication 
        /// of the two numbers, whose scale is the sum of the scales of the two numbers.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> or <paramref name="multiplicand"/> is <c>null</c>.
        /// </exception>
        public static BigDecimal Multiply(BigDecimal value, BigDecimal multiplicand) {
			return BigDecimalMath.Multiply(value, multiplicand);
		}

        /// <summary>
        /// Multiplies two <see cref="BigDecimal"/> numbers and rounds the result
		/// to the given precision.
        /// </summary>
        /// <param name="value">
		/// The <see cref="BigDecimal"/> number to be multiplied.
		/// </param>
        /// <param name="multiplicand">
		/// The <see cref="BigDecimal"/> number to multiply <paramref name="value"/> by.
		/// </param>
        /// <param name="mc">
		/// The <see cref="MathContext"/> object that specifies the precision and rounding mode
		/// applied to the result of the operation.
		/// </param>
        /// <returns>
		/// Returns an instance of <see cref="BigDecimal"/> that is the result of the multiplication 
		/// of the two numbers, rounded to the given precision.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="value"/>, <paramref name="multiplicand"/> or <paramref name="mc"/> is <c>null</c>.
		/// </exception>
        public static BigDecimal Multiply(BigDecimal value, BigDecimal multiplicand, MathContext mc) {
			BigDecimal result = Multiply(value, multiplicand);

			result.InplaceRound(mc);
			return result;
        }

        /// <summary>
        /// Performs the power operation on a <see cref="BigDecimal"/> number
		/// for the given exponent.
        /// </summary>
        /// <param name="number">
		/// The <see cref="BigDecimal"/> number to raise to the power of <paramref name="exp"/>.
		/// </param>
        /// <param name="exp">
		/// The exponent to which <paramref name="number"/> is raised.
		/// </param>
		/// <remarks>
		/// <para>
		/// The scale of the result is <paramref name="exp"/> times the scales 
		/// of <paramref name="number"/>.
		/// </para>
		/// <para>
		/// <c>BigMath.Pow(x, 0)</c> returns <c>1</c>, even if <c>c == 0</c>.
		/// </para>
		/// <para>
		/// Implementation Note: The implementation is based on the ANSI standard X3.274-1996 algorithm.
        /// </para>
        /// </remarks>
        /// <returns>
		/// </returns>
		/// <exception cref="ArithmeticException">
		/// Thrown when <paramref name="exp"/> is less than 0 or greater than 999999999.
		/// </exception>
        public static BigDecimal Pow(BigDecimal number, int exp) {
			return BigDecimalMath.Pow(number, exp);
		}

        /// <summary>
        /// Performs the power operation on a <see cref="BigDecimal"/> number
        /// for the given exponent.
        /// </summary>
        /// <param name="number">
        /// The <see cref="BigDecimal"/> number to raise to the power of <paramref name="exp"/>.
        /// </param>
        /// <param name="exp">
        /// The exponent to which <paramref name="number"/> is raised.
        /// </param>
		/// <param name="context">
		/// The <see cref="MathContext"/> object that specifies the precision and rounding mode
		/// for the result of the operation.
		/// </param>
        /// <remarks>
        /// <para>
        /// The scale of the result is <paramref name="exp"/> times the scales 
        /// of <paramref name="number"/>.
        /// </para>
        /// <para>
        /// <c>BigMath.Pow(x, 0)</c> returns <c>1</c>, even if <c>c == 0</c>.
        /// </para>
        /// <para>
        /// Implementation Note: The implementation is based on the ANSI standard X3.274-1996 algorithm.
        /// </para>
        /// </remarks>
        /// <returns>
        /// </returns>
        /// <exception cref="ArithmeticException">
        /// Thrown when <paramref name="exp"/> is less than 0 or greater than 999999999.
        /// </exception>
        public static BigDecimal Pow(BigDecimal number, int exp, MathContext context) {
			return BigDecimalMath.Pow(number, exp, context);
		}


        /// <summary>
        /// Returns the absolute value of the given <see cref="BigDecimal"/>,
		/// with the same scale as the original number.
        /// </summary>
        /// <param name="number">
		/// The <see cref="BigDecimal"/> number to compute the absolute value of.
		/// </param>
        /// <returns>
		/// Returns an instance of <see cref="BigDecimal"/> that represents the
		/// absolute value of the given number.
		/// </returns>
        public static BigDecimal Abs(BigDecimal number) {
			return ((number.Sign < 0) ? -number : number);
		}

        /// <summary>
        /// Returns the absolute value of the given <see cref="BigDecimal"/>,
        /// with the same scale as the original number.
        /// </summary>
        /// <param name="number">
        /// The <see cref="BigDecimal"/> number to compute the absolute value of.
        /// </param>
		/// <param name="mc">
		/// The <see cref="MathContext"/> object that specifies the precision and 
		/// rounding mode for the result of the operation.
		/// </param>
        /// <returns>
        /// Returns an instance of <see cref="BigDecimal"/> that represents the
        /// absolute value of the given number, and rounded according to the
		/// specified precision and rounding mode.
        /// </returns>
        public static BigDecimal Abs(BigDecimal number, MathContext mc) {
			return Abs(Round(number, mc));
		}

		/// <summary>
		/// Returns a new <see cref="BigDecimal"/> whose value represents
		/// an unary plus of the given number.
		/// </summary>
		/// <param name="number">
		/// The <see cref="BigDecimal"/> number to apply the unary plus to.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="BigDecimal"/> that represents the
		/// unary plus of the given number.
		/// </returns>
		public static BigDecimal Plus(BigDecimal number) {
			return Plus(number, null);
		}

		/// <remarks>
		/// Returns a new <see cref="BigDecimal"/> whose value is
		/// the unary plus of the given number, rounded according to the
		/// specified precision and rounding mode.
		/// </remarks>
		/// <param name="number">
		/// The <see cref="BigDecimal"/> number to be computed.
		/// </param>
		/// <param name="mc">
		/// Rounding mode and precision for the result of this operation.
		/// </param>
		/// <remarks>
		/// The result is rounded according to the passed context <paramref name="mc"/>.
		/// </remarks>
		/// <returns>
		/// Returns this decimal value rounded.
		/// </returns>
		public static BigDecimal Plus(BigDecimal number, MathContext mc) {
			return Round(number, mc);
		}

		/// <summary>
		/// Returns a new <see cref="BigDecimal"/> whose value is the negation
		/// of the given number, with the same scale as the original number.
		/// </summary>
		/// <param name="number">
		/// The <see cref="BigDecimal"/> number to negate.
		/// </param>
		/// <returns>
		/// Returns a new <see cref="BigDecimal"/> that is the negation of the given number,
		/// with the same scale as the original number.
		/// </returns>
		[Obsolete("Use the Negate() method in BigDecimal instead")]
        public static BigDecimal Negate(BigDecimal number) {
			return number.Negate();
		}

        /// <summary>
        /// Returns a new <see cref="BigDecimal"/> whose value is the negation
        /// of the given number, rounded according to the specified precision
        /// scale and rounding mode.
        /// </summary>
        /// <param name="number">
        /// The <see cref="BigDecimal"/> number to negate.
        /// </param>
        /// <param name="mc">
        /// The <see cref="MathContext"/> object that specifies the precision and
        /// rounding mode for the result of the operation.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that is the negation of the given number,
        /// rounded according to the specified precision scale and rounding mode.
        /// </returns>
        [Obsolete("Use the Negate() method in BigDecimal instead")]
        public static BigDecimal Negate(BigDecimal number, MathContext mc) {
			return number.Negate(mc);
		}

        /// <summary>
        /// Returns a new number whose value is rounded according to the passed context.
        /// </summary>
        /// <param name="number">
		/// The number to be rounded.
		/// </param>
        /// <param name="mc">
		/// The rounding mode and precision for the result of this operation.
		/// </param>
        /// <returns>
		/// Returns a new <see cref="BigDecimal"/> that is the rounded value 
		/// of the given number, according to the specified precision and 
		/// rounding mode.
		/// </returns>
		/// <exception cref="ArithmeticException">
		/// Thrown when the precision of the <paramref name="mc"/> is greater than 0, the
		/// rounding mode is <see cref="RoundingMode.Unnecessary"/> and the result cannot be
		/// represented exactly within the given precision.
		/// </exception>
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

        /// <summary>
        /// Calculates the power of ten of the given number.
        /// </summary>
        /// <param name="number">
        /// The number to elevate to the power of ten.
        /// </param>
        /// <param name="n">
        /// The number of places the decimal point has to be moved.
        /// </param>
        /// <remarks>
        /// <para>
        /// The scale of the result is <see cref="BigDecimal.Scale"/> minus 
        /// the value of <paramref name="n"/>. The precision of the result is 
        /// the precision of the given <paramref name="number"/>.
        /// </para>
        /// <para>
        /// This method has the same effect as <see cref="MovePointRight(BigDecimal, int)"/>, 
		/// except that the precision is not changed.
        /// </para>
        /// </remarks>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that is the result of 
        /// the power of ten operation.
        /// </returns>
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

	//	/**
 //* Returns a new {@code BigInteger} whose value is {@code this % divisor}.
 //* Regarding signs this methods has the same behavior as the % operator on
 //* int's, i.e. the sign of the remainder is the same as the sign of this.
 //*
 //* @param divisor
 //*            value by which {@code this} is divided.
 //* @return {@code this % divisor}.
 //* @throws NullPointerException
 //*             if {@code divisor == null}.
 //* @throws ArithmeticException
 //*             if {@code divisor == 0}.
 //*/
		public static BigInteger Remainder(BigInteger dividend, BigInteger divisor) {
			return BigIntegerMath.Remainder(dividend, divisor);
		}

	//	/**
 //* Returns a {@code BigInteger} array which contains {@code this / divisor}
 //* at index 0 and {@code this % divisor} at index 1.
 //*
 //* @param divisor
 //*            value by which {@code this} is divided.
 //* @return {@code [this / divisor, this % divisor]}.
 //* @throws NullPointerException
 //*             if {@code divisor == null}.
 //* @throws ArithmeticException
 //*             if {@code divisor == 0}.
 //* @see #divide
 //* @see #remainder
 //*/
		public static BigInteger DivideAndRemainder(BigInteger dividend, BigInteger divisor, out BigInteger remainder) {
			return BigIntegerMath.DivideAndRemainder(dividend, divisor, out remainder);
		}

	//	/**
 //* Returns a new {@code BigInteger} whose value is {@code this mod m}. The
 //* modulus {@code m} must be positive. The result is guaranteed to be in the
 //* interval {@code [0, m)} (0 inclusive, m exclusive). The behavior of this
 //* function is not equivalent to the behavior of the % operator defined for
 //* the built-in {@code int}'s.
 //*
 //* @param m
 //*            the modulus.
 //* @return {@code this mod m}.
 //* @throws NullPointerException
 //*             if {@code m == null}.
 //* @throws ArithmeticException
 //*             if {@code m < 0}.
 //*/
		public static BigInteger Mod(BigInteger value, BigInteger m) {
			return BigIntegerMath.Mod(value, m);
		}

	//	/**
 //* Returns a new {@code BigInteger} whose value is {@code 1/this mod m}. The
 //* modulus {@code m} must be positive. The result is guaranteed to be in the
 //* interval {@code [0, m)} (0 inclusive, m exclusive). If {@code this} is
 //* not relatively prime to m, then an exception is thrown.
 //*
 //* @param m
 //*            the modulus.
 //* @return {@code 1/this mod m}.
 //* @throws NullPointerException
 //*             if {@code m == null}
 //* @throws ArithmeticException
 //*             if {@code m < 0 or} if {@code this} is not relatively prime
 //*             to {@code m}
 //*/
		public static BigInteger ModInverse(BigInteger value, BigInteger m) {
			return BigIntegerMath.ModInverse(value, m);
		}

	//	/**
 //* Returns a new {@code BigInteger} whose value is {@code this^exponent mod
 //* m}. The modulus {@code m} must be positive. The result is guaranteed to
 //* be in the interval {@code [0, m)} (0 inclusive, m exclusive). If the
 //* exponent is negative, then {@code this.modInverse(m)^(-exponent) mod m)}
 //* is computed. The inverse of this only exists if {@code this} is
 //* relatively prime to m, otherwise an exception is thrown.
 //*
 //* @param exponent
 //*            the exponent.
 //* @param m
 //*            the modulus.
 //* @return {@code this^exponent mod val}.
 //* @throws NullPointerException
 //*             if {@code m == null} or {@code exponent == null}.
 //* @throws ArithmeticException
 //*             if {@code m < 0} or if {@code exponent<0} and this is not
 //*             relatively prime to {@code m}.
 //*/
		public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger m) {
			return BigIntegerMath.ModPow(value, exponent, m);
		}

	//	/**
 //* Returns a new {@code BigInteger} whose value is {@code this ^ exp}.
 //*
 //* @param exp
 //*            exponent to which {@code this} is raised.
 //* @return {@code this ^ exp}.
 //* @throws ArithmeticException
 //*             if {@code exp < 0}.
 //*/
		public static BigInteger Pow(BigInteger value, int exp) {
			return BigIntegerMath.Pow(value, exp);
		}

	//	/**
 //* Returns the position of the lowest set bit in the two's complement
 //* representation of this {@code BigInteger}. If all bits are zero (this=0)
 //* then -1 is returned as result.
 //* <p>
 //* <b>Implementation Note:</b> Usage of this method is not recommended as
 //* the current implementation is not efficient.
 //*
 //* @return position of lowest bit if {@code this != 0}, {@code -1} otherwise
 //*/

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

        /// <summary>
        /// Scales the given number to given scale, using the 
        /// specified rounding mode.
        /// </summary>
		/// <param name="number">
		/// The number to be scaled to the given scale.
		/// </param>
        /// <param name="newScale">
        /// The new scale of the number to be returned.
        /// </param>
        /// <param name="roundingMode">
        /// The mode to be used to round the result.
        /// </param>
        /// <remarks>
        /// <para>
        /// If the new scale is greater than the old scale, then additional zeros are 
        /// added to the unscaled value: in this case no rounding is necessary.
        /// </para>
        /// <para>
        /// If the new scale is smaller than the old scale, then trailing digits are 
        /// removed. If these trailing digits are not zero, then the remaining unscaled 
        /// value has to be rounded. For this rounding operation the specified rounding 
        /// mode is used.
        /// </para>
        /// </remarks>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> instance with the same value as this instance,
        /// but with the scale of the given value and the rounding mode specified.
        /// </returns>
        /// <exception cref="ArithmeticException">
        /// Thrown if the rounding mode is <see cref="RoundingMode.Unnecessary"/> and the
        /// result cannot be represented within the given precision without rounding.
        /// </exception>
        [Obsolete("User the BigDecimal.ScaleTo() method instead")]
		public static BigDecimal Scale(BigDecimal number, int newScale, RoundingMode roundingMode) {
			return number.ScaleTo(newScale, roundingMode);
		}

        /// <summary>
        /// Scale the given number to the given scale.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="newScale"></param>
        /// <remarks>
        /// If the new scale is greater than the old scale, then additional zeros are added to 
        /// the unscaled value. If the new scale is smaller than the old scale, then trailing 
        /// zeros are removed.
        /// </remarks>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> instance with the same value as this instance,
        /// and with the scale of the given value.
        /// </returns>
        /// <exception cref="ArithmeticException">
        /// If the trailing digits are not zeros
        /// </exception>
        [Obsolete("Use the BigDecimal.ScaleTo() method instead")]
		public static BigDecimal Scale(BigDecimal number, int newScale) {
			return number.ScaleTo(newScale, RoundingMode.Unnecessary);
		}

        /// <summary>
        /// Moves the decimal point of the given number to the left 
        /// by the given number of places.
        /// </summary>
        /// <param name="number">
        /// The number to move the decimal point of.
        /// </param>
        /// <param name="n">
        /// The number of places to move the decimal point.
        /// </param>
        /// <remarks>
        /// <para>
        /// If <paramref name="n"/> is less than 0 then the decimal point is 
        /// moved by <c>-<paramref name="n"/></c> places to the right.
        /// </para>
        /// <para>
        /// The result is obtained by changing its scale. If the scale of the result
        /// becomes negative, then its precision is increased such that the scale is
        /// zero.
        /// </para>
        /// <para>
        /// Note, that <c>MovePointLeft(number, 0)</c> returns a result which is 
		/// mathematically equivalent, but which has the <see cref="BigDecimal.Scale"/>
		/// greater or equal to 0.
        /// </para>
        /// </remarks>
        /// <returns>
		/// Returns a new <see cref="BigDecimal"/> instance with the decimal point moved
		/// to the left by the given number of places.
		/// </returns>
        public static BigDecimal MovePointLeft(BigDecimal number, int n) {
			return BigDecimalMath.MovePoint(number, number.Scale + (long) n);
		}

        /// <summary>
        /// Moves the decimal point of the given number to the right 
        /// by the given number of places.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="n"></param>
        /// <remarks>
        /// <para>
        /// If <paramref name="n"/> is less than 0 then the decimal point is 
        /// moved by <c>-<paramref name="n"/></c> places to the left.
        /// </para>
        /// <para>
        /// The result is obtained by changing its scale. If the scale of the result
        /// becomes negative, then its precision is increased such that the scale is
        /// zero.
        /// </para>
        /// <para>
        /// Note, that <c>MovePointRight(number, 0)</c> returns a result which is 
        /// mathematically equivalent, but which has the <see cref="BigDecimal.Scale"/>
        /// greater or equal to 0.
        /// </para>
        /// </remarks>
        /// <returns></returns>
        public static BigDecimal MovePointRight(BigDecimal number, int n) {
			return BigDecimalMath.MovePoint(number, number.Scale - (long) n);
        }

        /// <summary>
        /// Returns the unit in the last place (ULP) of the given <see cref="BigDecimal"/>.
        /// </summary>
        /// <param name="value">
		/// The <see cref="BigDecimal"/> number to compute the ULP of.
		/// </param>
		/// <remarks>
		/// <para>
		/// An ULP is the distance to the nearest big decimal with the same precision.
        /// </para>
		/// <para>
		/// The amount of a rounding error in the evaluation of a floating-point 
		/// operation is often expressed in ULPs. An error of 1 ULP is often seen 
		/// as a tolerable error.
        /// </para>
		/// <para>
		/// For <see cref="BigDecimal"/> the ULP of a number is simply 10^(-scale).
		/// </para>
		/// <para>
		/// For example, <c>Ulp(new BigDecimal(0.1))</c> returns <c>1E-55</c>.
		/// </para>
        /// </remarks>
        /// <returns></returns>
        public static BigDecimal Ulp(BigDecimal value) {
			return BigDecimal.Create(1, value.Scale);
		}

		/// <summary>
		/// Adds a value to the current instance of <see cref="BigDecimal"/>,
		/// rounding the result according to the provided context.
		/// </summary>
		/// <param name="value">
		/// The <see cref="BigDecimal"/> instance to which the value is added.
		/// </param>
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
		/// <param name="value">
		/// The value from which to substract.
		/// </param>
		/// <param name="subtrahend">
		/// The value to be subtracted from the <paramref name="value"/>.
		/// </param>
        /// <remarks>
        /// </remarks>
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
        /// <param name="value">
		/// The <see cref="BigDecimal"/> number to be subtracted from.
		/// </param>
        /// <param name="subtrahend">
		/// The value to be subtracted from this <see cref="BigDecimal"/>.
		/// </param>
        /// <param name="mc">The context used to round the result of this operation.</param>
        /// <remarks>
        /// <para>
        /// This overload rounds the result of the operation to the <paramref name="mc">context</paramref>
        /// provided as argument.
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// Removes trailing zeros from the given number.
        /// </summary>
        /// <param name="value">
        /// The number from which to remove the trailing zeros.
        /// </param>
        /// <remarks>
        /// If the unscaled value of <paramref name="value"/> has n trailing zeros, 
		/// then the scale and the precision of the result has been reduced by n.
        /// </remarks>
        /// <returns>
        /// Returns a new instance of <see cref="BigDecimal"/> equivalent
        /// to the given number, where the trailing zeros of the unscaled value
        /// have been removed.
        /// </returns>
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