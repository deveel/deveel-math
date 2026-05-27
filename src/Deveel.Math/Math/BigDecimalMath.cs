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
	/// Provides static mathematical operations for <see cref="BigDecimal"/> values,
	/// including division, multiplication, exponentiation, scaling, and addition/subtraction
	/// with various rounding and precision controls.
	/// </summary>
	static class BigDecimalMath {
		/// <summary>
		/// Divides two <see cref="BigInteger"/> values representing scaled operands and returns the result
		/// as a <see cref="BigDecimal"/> with the specified scale and rounding mode.
		/// </summary>
		/// <param name="scaledDividend">The scaled dividend <see cref="BigInteger"/>.</param>
		/// <param name="scaledDivisor">The scaled divisor <see cref="BigInteger"/>.</param>
		/// <param name="scale">The scale of the resulting <see cref="BigDecimal"/>.</param>
		/// <param name="roundingMode">The <see cref="RoundingMode"/> to apply when the division has a non-terminating decimal expansion.</param>
		/// <returns>A <see cref="BigDecimal"/> representing the quotient with the specified scale.</returns>
		public static BigDecimal DivideBigIntegers(BigInteger scaledDividend, BigInteger scaledDivisor, int scale,
			RoundingMode roundingMode) {
			BigInteger remainder;
			BigInteger quotient = BigMath.DivideAndRemainder(scaledDividend, scaledDivisor, out remainder);
			if (remainder.Sign == 0) {
				return new BigDecimal(quotient, scale);
			}
			int sign = scaledDividend.Sign * scaledDivisor.Sign;
			int compRem; // 'compare to remainder'
			if (scaledDivisor.BitLength < 63) {
				// 63 in order to avoid out of long after <<1
				long rem = remainder.ToInt64();
				long divisor = scaledDivisor.ToInt64();
				compRem = BigDecimal.LongCompareTo(System.Math.Abs(rem) << 1, System.Math.Abs(divisor));
				// To look if there is a carry
				compRem = BigDecimal.RoundingBehavior(BigInteger.TestBit(quotient, 0) ? 1 : 0,
					sign * (5 + compRem), roundingMode);

			} else {
				// Checking if:  remainder * 2 >= scaledDivisor 
				compRem = BigMath.Abs(remainder).ShiftLeftOneBit().CompareTo(BigMath.Abs(scaledDivisor));
				compRem = BigDecimal.RoundingBehavior(BigInteger.TestBit(quotient, 0) ? 1 : 0,
					sign * (5 + compRem), roundingMode);
			}
			if (compRem != 0) {
				if (quotient.BitLength < 63) {
					return BigDecimal.Create(quotient.ToInt64() + compRem, scale);
				}
				quotient += BigInteger.FromInt64(compRem);
				return new BigDecimal(quotient, scale);
			}
			// Constructing the result with the appropriate unscaled value
			return new BigDecimal(quotient, scale);
		}

		/// <summary>
		/// Divides two <see cref="long"/> values representing scaled operands and returns the result
		/// as a <see cref="BigDecimal"/> with the specified scale and rounding mode.
		/// </summary>
		/// <param name="scaledDividend">The scaled dividend as a <see cref="long"/>.</param>
		/// <param name="scaledDivisor">The scaled divisor as a <see cref="long"/>.</param>
		/// <param name="scale">The scale of the resulting <see cref="BigDecimal"/>.</param>
		/// <param name="roundingMode">The <see cref="RoundingMode"/> to apply when the division has a non-terminating decimal expansion.</param>
		/// <returns>A <see cref="BigDecimal"/> representing the quotient with the specified scale.</returns>
		public static BigDecimal DividePrimitiveLongs(long scaledDividend, long scaledDivisor, int scale,
			RoundingMode roundingMode) {
			long quotient = scaledDividend / scaledDivisor;
			long remainder = scaledDividend % scaledDivisor;
			int sign = System.Math.Sign(scaledDividend) * System.Math.Sign(scaledDivisor);
			if (remainder != 0) {
				// Checking if:  remainder * 2 >= scaledDivisor
				int compRem; // 'compare to remainder'
				compRem = BigDecimal.LongCompareTo(System.Math.Abs(remainder) << 1, System.Math.Abs(scaledDivisor));
				// To look if there is a carry
				quotient += BigDecimal.RoundingBehavior(((int)quotient) & 1, sign * (5 + compRem), roundingMode);
			}
			// Constructing the result with the appropriate unscaled value
			return BigDecimal.Create(quotient, scale);
		}

		/// <summary>
		/// Divides one <see cref="BigDecimal"/> by another, returning the exact quotient
		/// if the division has a terminating decimal expansion.
		/// </summary>
		/// <param name="dividend">The <see cref="BigDecimal"/> value to be divided.</param>
		/// <param name="divisor">The <see cref="BigDecimal"/> value to divide by.</param>
		/// <returns>The exact quotient as a <see cref="BigDecimal"/>.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="divisor"/> is zero, or when the division
		/// result has a non-terminating decimal expansion.</exception>
		/// <example>
		/// <code>
		/// BigDecimal result = BigDecimalMath.Divide(
		///     new BigDecimal("10"),
		///     new BigDecimal("2"));
		/// // result is 5
		/// </code>
		/// </example>
		public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor) {
			BigInteger p = dividend.UnscaledValue;
			BigInteger q = divisor.UnscaledValue;
			BigInteger gcd; // greatest common divisor between 'p' and 'q'
			BigInteger quotient;
			BigInteger remainder;
			long diffScale = (long)dividend.Scale - divisor.Scale;
			int newScale; // the new scale for final quotient
			int k; // number of factors "2" in 'q'
			int l = 0; // number of factors "5" in 'q'
			int i = 1;
			int lastPow = BigDecimal.FivePow.Length - 1;

			if (divisor.IsZero) {
				// math.04=Division by zero
				throw new ArithmeticException(Messages.math04); //$NON-NLS-1$
			}
			if (p.Sign == 0) {
				return BigDecimal.GetZeroScaledBy(diffScale);
			}
			// To divide both by the GCD
			gcd = BigMath.Gcd(p, q);
			p = p / gcd;
			q = q / gcd;
			// To simplify all "2" factors of q, dividing by 2^k
			k = q.LowestSetBit;
			q = q >> k;
			// To simplify all "5" factors of q, dividing by 5^l
			do {
				quotient = BigMath.DivideAndRemainder(q, BigDecimal.FivePow[i], out remainder);
				if (remainder.Sign == 0) {
					l += i;
					if (i < lastPow) {
						i++;
					}
					q = quotient;
				} else {
					if (i == 1) {
						break;
					}
					i = 1;
				}
			} while (true);
			// If  abs(q) != 1  then the quotient is periodic
			if (!BigMath.Abs(q).Equals(BigInteger.One)) {
				// math.05=Non-terminating decimal expansion; no exact representable decimal result.
				throw new ArithmeticException(Messages.math05); //$NON-NLS-1$
			}
			// The sign of the is fixed and the quotient will be saved in 'p'
			if (q.Sign < 0) {
				p = -p;
			}
			// Checking if the new scale is out of range
			newScale = BigDecimal.ToIntScale(diffScale + System.Math.Max(k, l));
			// k >= 0  and  l >= 0  implies that  k - l  is in the 32-bit range
			i = k - l;

			p = (i > 0)
				? Multiplication.MultiplyByFivePow(p, i)
				: p << -i;
			return new BigDecimal(p, newScale);
		}

		/// <summary>
		/// Divides one <see cref="BigDecimal"/> by another with the precision and rounding specified
		/// by the given <see cref="MathContext"/>.
		/// </summary>
		/// <param name="dividend">The <see cref="BigDecimal"/> value to be divided.</param>
		/// <param name="divisor">The <see cref="BigDecimal"/> value to divide by.</param>
		/// <param name="mc">The <see cref="MathContext"/> that specifies the precision and rounding mode.</param>
		/// <returns>The quotient as a <see cref="BigDecimal"/> rounded according to the specified context.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="divisor"/> is zero.</exception>
		public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor, MathContext mc) {
			/* Calculating how many zeros must be append to 'dividend'
			 * to obtain a  quotient with at least 'mc.precision()' digits */
			long traillingZeros = mc.Precision + 2L
			                      + divisor.AproxPrecision() - dividend.AproxPrecision();
			long diffScale = (long)dividend.Scale - divisor.Scale;
			long newScale = diffScale; // scale of the final quotient
			int compRem; // to compare the remainder
			int i = 1; // index   
			int lastPow = BigDecimal.TenPow.Length - 1; // last power of ten
			BigInteger integerQuot; // for temporal results
			BigInteger quotient = dividend.UnscaledValue;
			BigInteger remainder;
			// In special cases it reduces the problem to call the dual method
			if ((mc.Precision == 0) || (dividend.IsZero) || (divisor.IsZero))
				return Divide(dividend, divisor);

			if (traillingZeros > 0) {
				// To append trailing zeros at end of dividend
				quotient = dividend.UnscaledValue * Multiplication.PowerOf10(traillingZeros);
				newScale += traillingZeros;
			}
			quotient = BigMath.DivideAndRemainder(quotient, divisor.UnscaledValue, out remainder);
			integerQuot = quotient;
			// Calculating the exact quotient with at least 'mc.precision()' digits
			if (remainder.Sign != 0) {
				// Checking if:   2 * remainder >= divisor ?
				compRem = remainder.ShiftLeftOneBit().CompareTo(divisor.UnscaledValue);
				// quot := quot * 10 + r;     with 'r' in {-6,-5,-4, 0,+4,+5,+6}
				integerQuot = (integerQuot * BigInteger.Ten) +
				              BigInteger.FromInt64(quotient.Sign * (5 + compRem));
				newScale++;
			} else {
				// To strip trailing zeros until the preferred scale is reached
				while (!BigInteger.TestBit(integerQuot, 0)) {
					quotient = BigMath.DivideAndRemainder(integerQuot, BigDecimal.TenPow[i], out remainder);
					if ((remainder.Sign == 0)
					    && (newScale - i >= diffScale)) {
						newScale -= i;
						if (i < lastPow) {
							i++;
						}
						integerQuot = quotient;
					} else {
						if (i == 1) {
							break;
						}
						i = 1;
					}
				}
			}
			// To perform rounding
			return new BigDecimal(integerQuot, BigDecimal.ToIntScale(newScale), mc);
		}

		/// <summary>
		/// Divides one <see cref="BigDecimal"/> by another, returning the integral part of the quotient.
		/// </summary>
		/// <param name="dividend">The <see cref="BigDecimal"/> value to be divided.</param>
		/// <param name="divisor">The <see cref="BigDecimal"/> value to divide by.</param>
		/// <returns>The integral part of the quotient as a <see cref="BigDecimal"/>.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="divisor"/> is zero.</exception>
		public static BigDecimal DivideToIntegralValue(BigDecimal dividend, BigDecimal divisor) {
			BigInteger integralValue; // the integer of result
			BigInteger powerOfTen; // some power of ten
			BigInteger quotient;
			BigInteger remainder;
			long newScale = (long)dividend.Scale - divisor.Scale;
			long tempScale = 0;
			int i = 1;
			int lastPow = BigDecimal.TenPow.Length - 1;

			if (divisor.IsZero) {
				// math.04=Division by zero
				throw new ArithmeticException(Messages.math04); //$NON-NLS-1$
			}
			if ((divisor.AproxPrecision() + newScale > dividend.AproxPrecision() + 1L)
			    || (dividend.IsZero)) {
				/* If the divisor's integer part is greater than this's integer part,
				 * the result must be zero with the appropriate scale */
				integralValue = BigInteger.Zero;
			} else if (newScale == 0) {
				integralValue = dividend.UnscaledValue / divisor.UnscaledValue;
			} else if (newScale > 0) {
				powerOfTen = Multiplication.PowerOf10(newScale);
				integralValue = dividend.UnscaledValue / (divisor.UnscaledValue * powerOfTen);
				integralValue = integralValue * powerOfTen;
			} else {
				// (newScale < 0)
				powerOfTen = Multiplication.PowerOf10(-newScale);
				integralValue = (dividend.UnscaledValue * powerOfTen) / divisor.UnscaledValue;
				// To strip trailing zeros approximating to the preferred scale
				while (!BigInteger.TestBit(integralValue, 0)) {
					quotient = BigMath.DivideAndRemainder(integralValue, BigDecimal.TenPow[i], out remainder);
					if ((remainder.Sign == 0)
					    && (tempScale - i >= newScale)) {
						tempScale -= i;
						if (i < lastPow) {
							i++;
						}
						integralValue = quotient;
					} else {
						if (i == 1) {
							break;
						}
						i = 1;
					}
				}
				newScale = tempScale;
			}
			return ((integralValue.Sign == 0)
				? BigDecimal.GetZeroScaledBy(newScale)
				: new BigDecimal(integralValue, BigDecimal.ToIntScale(newScale)));
		}

		/// <summary>
		/// Divides one <see cref="BigDecimal"/> by another, returning the integral part of the quotient
		/// with the precision specified by the given <see cref="MathContext"/>.
		/// </summary>
		/// <param name="dividend">The <see cref="BigDecimal"/> value to be divided.</param>
		/// <param name="divisor">The <see cref="BigDecimal"/> value to divide by.</param>
		/// <param name="mc">The <see cref="MathContext"/> that specifies the precision and rounding mode.</param>
		/// <returns>The integral part of the quotient as a <see cref="BigDecimal"/>.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="divisor"/> is zero, or when the quotient
		/// cannot be represented within the specified precision.</exception>
		public static BigDecimal DivideToIntegralValue(BigDecimal dividend, BigDecimal divisor, MathContext mc) {
			int mcPrecision = mc.Precision;
			int diffPrecision = dividend.Precision - divisor.Precision;
			int lastPow = BigDecimal.TenPow.Length - 1;
			long diffScale = (long)dividend.Scale - divisor.Scale;
			long newScale = diffScale;
			long quotPrecision = diffPrecision - diffScale + 1;
			BigInteger quotient;
			BigInteger remainder;
			// In special cases it call the dual method
			if ((mcPrecision == 0) || (dividend.IsZero) || (divisor.IsZero)) {
				return DivideToIntegralValue(dividend, divisor);
			}
			// Let be:   this = [u1,s1]   and   divisor = [u2,s2]
			if (quotPrecision <= 0) {
				quotient = BigInteger.Zero;
			} else if (diffScale == 0) {
				// CASE s1 == s2:  to calculate   u1 / u2 
				quotient = dividend.UnscaledValue / divisor.UnscaledValue;
			} else if (diffScale > 0) {
				// CASE s1 >= s2:  to calculate   u1 / (u2 * 10^(s1-s2)  
				quotient = dividend.UnscaledValue / (divisor.UnscaledValue * Multiplication.PowerOf10(diffScale));
				// To chose  10^newScale  to get a quotient with at least 'mc.precision()' digits
				newScale = System.Math.Min(diffScale, System.Math.Max(mcPrecision - quotPrecision + 1, 0));
				// To calculate: (u1 / (u2 * 10^(s1-s2)) * 10^newScale
				quotient = quotient * Multiplication.PowerOf10(newScale);
			} else {
				// CASE s2 > s1:   
				/* To calculate the minimum power of ten, such that the quotient 
				 *   (u1 * 10^exp) / u2   has at least 'mc.precision()' digits. */
				long exp = System.Math.Min(-diffScale, System.Math.Max((long)mcPrecision - diffPrecision, 0));
				long compRemDiv;
				// Let be:   (u1 * 10^exp) / u2 = [q,r]  
				quotient = BigMath.DivideAndRemainder(dividend.UnscaledValue * Multiplication.PowerOf10(exp),
					divisor.UnscaledValue, out remainder);
				newScale += exp; // To fix the scale
				exp = -newScale; // The remaining power of ten
				// If after division there is a remainder...
				if ((remainder.Sign != 0) && (exp > 0)) {
					// Log10(r) + ((s2 - s1) - exp) > mc.precision ?
					compRemDiv = (new BigDecimal(remainder)).Precision
					             + exp - divisor.Precision;
					if (compRemDiv == 0) {
						// To calculate:  (r * 10^exp2) / u2
						remainder = (remainder * Multiplication.PowerOf10(exp)) / divisor.UnscaledValue;
						compRemDiv = System.Math.Abs(remainder.Sign);
					}
					if (compRemDiv > 0) {
						// The quotient won't fit in 'mc.precision()' digits
						// math.06=Division impossible
						throw new ArithmeticException(Messages.math06); //$NON-NLS-1$
					}
				}
			}
			// Fast return if the quotient is zero
			if (quotient.Sign == 0) {
				return BigDecimal.GetZeroScaledBy(diffScale);
			}
			BigInteger strippedBI = quotient;
			BigDecimal integralValue = new BigDecimal(quotient);
			long resultPrecision = integralValue.Precision;
			int i = 1;
			// To strip trailing zeros until the specified precision is reached
			while (!BigInteger.TestBit(strippedBI, 0)) {
				quotient = BigMath.DivideAndRemainder(strippedBI, BigDecimal.TenPow[i], out remainder);
				if ((remainder.Sign == 0) &&
				    ((resultPrecision - i >= mcPrecision)
				     || (newScale - i >= diffScale))) {
					resultPrecision -= i;
					newScale -= i;
					if (i < lastPow) {
						i++;
					}
					strippedBI = quotient;
				} else {
					if (i == 1) {
						break;
					}
					i = 1;
				}
			}
			// To check if the result fit in 'mc.precision()' digits
			if (resultPrecision > mcPrecision) {
				// math.06=Division impossible
				throw new ArithmeticException(Messages.math06); //$NON-NLS-1$
			}
			integralValue.Scale = BigDecimal.ToIntScale(newScale);
			integralValue.SetUnscaledValue(strippedBI);
			return integralValue;
		}


		/// <summary>
		/// Divides one <see cref="BigDecimal"/> by another, returning the quotient with the specified scale and rounding mode.
		/// </summary>
		/// <param name="dividend">The <see cref="BigDecimal"/> value to be divided.</param>
		/// <param name="divisor">The <see cref="BigDecimal"/> value to divide by.</param>
		/// <param name="scale">The scale of the resulting <see cref="BigDecimal"/>.</param>
		/// <param name="roundingMode">The <see cref="RoundingMode"/> to apply.</param>
		/// <returns>The quotient as a <see cref="BigDecimal"/> with the specified scale.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="divisor"/> is zero.</exception>
		/// <example>
		/// <code>
		/// BigDecimal result = BigDecimalMath.Divide(
		///     new BigDecimal("10"),
		///     new BigDecimal("3"),
		///     2,
		///     RoundingMode.HalfUp);
		/// // result is 3.33
		/// </code>
		/// </example>
		public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor, int scale, RoundingMode roundingMode) {
			// Let be: this = [u1,s1]  and  divisor = [u2,s2]
			if (divisor.IsZero) {
				// math.04=Division by zero
				throw new ArithmeticException(Messages.math04); //$NON-NLS-1$
			}

			long diffScale = ((long)dividend.Scale - divisor.Scale) - scale;
			if (dividend.BitLength < 64 && divisor.BitLength < 64) {
				if (diffScale == 0)
					return DividePrimitiveLongs(dividend.SmallValue, divisor.SmallValue, scale, roundingMode);
				if (diffScale > 0) {
					if (diffScale < BigDecimal.LongTenPow.Length &&
					    divisor.BitLength + BigDecimal.LongTenPowBitLength[(int)diffScale] < 64) {
						return DividePrimitiveLongs(dividend.SmallValue, divisor.SmallValue * BigDecimal.LongTenPow[(int)diffScale], scale, roundingMode);
					}
				} else {
					// diffScale < 0
					if (-diffScale < BigDecimal.LongTenPow.Length &&
					    dividend.BitLength + BigDecimal.LongTenPowBitLength[(int)-diffScale] < 64) {
						return DividePrimitiveLongs(dividend.SmallValue * BigDecimal.LongTenPow[(int)-diffScale], divisor.SmallValue, scale, roundingMode);
					}

				}
			}
			BigInteger scaledDividend = dividend.UnscaledValue;
			BigInteger scaledDivisor = divisor.UnscaledValue; // for scaling of 'u2'

			if (diffScale > 0) {
				// Multiply 'u2'  by:  10^((s1 - s2) - scale)
				scaledDivisor = Multiplication.MultiplyByTenPow(scaledDivisor, (int)diffScale);
			} else if (diffScale < 0) {
				// Multiply 'u1'  by:  10^(scale - (s1 - s2))
				scaledDividend = Multiplication.MultiplyByTenPow(scaledDividend, (int)-diffScale);
			}
			return DivideBigIntegers(scaledDividend, scaledDivisor, scale, roundingMode);
		}

		/// <summary>
		/// Divides one <see cref="BigDecimal"/> by another, returning the integral quotient and the remainder.
		/// </summary>
		/// <param name="dividend">The <see cref="BigDecimal"/> value to be divided.</param>
		/// <param name="divisor">The <see cref="BigDecimal"/> value to divide by.</param>
		/// <param name="remainder">When this method returns, contains the <see cref="BigDecimal"/> remainder.</param>
		/// <returns>The integral part of the quotient as a <see cref="BigDecimal"/>.</returns>
		public static BigDecimal DivideAndRemainder(BigDecimal dividend, BigDecimal divisor, out BigDecimal remainder) {
			var quotient = DivideToIntegralValue(dividend, divisor);
			remainder = Subtract(dividend, Multiply(quotient, divisor));
			return quotient;
		}

		/// <summary>
		/// Divides one <see cref="BigDecimal"/> by another with the specified <see cref="MathContext"/>,
		/// returning the integral quotient and the remainder.
		/// </summary>
		/// <param name="dividend">The <see cref="BigDecimal"/> value to be divided.</param>
		/// <param name="divisor">The <see cref="BigDecimal"/> value to divide by.</param>
		/// <param name="mc">The <see cref="MathContext"/> that specifies the precision and rounding mode.</param>
		/// <param name="remainder">When this method returns, contains the <see cref="BigDecimal"/> remainder.</param>
		/// <returns>The integral part of the quotient as a <see cref="BigDecimal"/>.</returns>
		public static BigDecimal DivideAndRemainder(BigDecimal dividend, BigDecimal divisor, MathContext mc, out BigDecimal remainder) {
			var quotient = DivideToIntegralValue(dividend, divisor, mc);
			remainder = Subtract(dividend, Multiply(quotient, divisor));
			return quotient;
		}

		/// <summary>
		/// Multiplies one <see cref="BigDecimal"/> by another. The scale of the result is the sum of the scales of the two operands.
		/// </summary>
		/// <param name="value">The first <see cref="BigDecimal"/> value to multiply.</param>
		/// <param name="multiplicand">The second <see cref="BigDecimal"/> value to multiply.</param>
		/// <returns>A <see cref="BigDecimal"/> representing the product of the two values.</returns>
		/// <example>
		/// <code>
		/// BigDecimal result = BigDecimalMath.Multiply(
		///     new BigDecimal("2.5"),
		///     new BigDecimal("4"));
		/// // result is 10.0
		/// </code>
		/// </example>
		public static BigDecimal Multiply(BigDecimal value, BigDecimal multiplicand) {
			long newScale = (long)value.Scale + multiplicand.Scale;

			if ((value.IsZero) || (multiplicand.IsZero)) {
				return BigDecimal.GetZeroScaledBy(newScale);
			}
			/* Let be: this = [u1,s1] and multiplicand = [u2,s2] so:
			 * this x multiplicand = [ s1 * s2 , s1 + s2 ] */
			if (value.BitLength + multiplicand.BitLength < 64) {
				return BigDecimal.Create(value.SmallValue * multiplicand.SmallValue, BigDecimal.ToIntScale(newScale));
			}
			return new BigDecimal(value.UnscaledValue * multiplicand.UnscaledValue, BigDecimal.ToIntScale(newScale));
		}


		/// <summary>
		/// Raises the specified <see cref="BigDecimal"/> to the specified power.
		/// </summary>
		/// <param name="number">The <see cref="BigDecimal"/> base value.</param>
		/// <param name="n">The exponent, which must be between 0 and 999999999 inclusive.</param>
		/// <returns>The result of <paramref name="number"/> raised to the power of <paramref name="n"/>.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="n"/> is negative or exceeds 999999999.</exception>
		public static BigDecimal Pow(BigDecimal number, int n) {
			if (n == 0) {
				return BigDecimal.One;
			}
			if ((n < 0) || (n > 999999999)) {
				// math.07=Invalid Operation
				throw new ArithmeticException(Messages.math07); //$NON-NLS-1$
			}
			long newScale = number.Scale * (long)n;
			// Let be: this = [u,s]   so:  this^n = [u^n, s*n]
			return ((number.IsZero)
				? BigDecimal.GetZeroScaledBy(newScale)
				: new BigDecimal(BigMath.Pow(number.UnscaledValue, n), BigDecimal.ToIntScale(newScale)));
		}

		/// <summary>
		/// Raises the specified <see cref="BigDecimal"/> to the specified power with the given <see cref="MathContext"/> precision.
		/// </summary>
		/// <param name="number">The <see cref="BigDecimal"/> base value.</param>
		/// <param name="n">The exponent (can be negative when a <see cref="MathContext"/> is provided).</param>
		/// <param name="mc">The <see cref="MathContext"/> that specifies the precision and rounding mode.</param>
		/// <returns>The result of <paramref name="number"/> raised to the power of <paramref name="n"/>, rounded to the specified precision.</returns>
		/// <exception cref="ArithmeticException">Thrown when the operation is invalid (e.g., exponent exceeds limits or precision is insufficient).</exception>
		/// <example>
		/// <code>
		/// MathContext mc = new MathContext(5, RoundingMode.HalfUp);
		/// BigDecimal result = BigDecimalMath.Pow(
		///     new BigDecimal("2"),
		///     10,
		///     mc);
		/// // result is 1024.0
		/// </code>
		/// </example>
		public static BigDecimal Pow(BigDecimal number, int n, MathContext mc) {
			// The ANSI standard X3.274-1996 algorithm
			int m = System.Math.Abs(n);
			int mcPrecision = mc.Precision;
			int elength = (int)System.Math.Log10(m) + 1; // decimal digits in 'n'
			int oneBitMask; // mask of bits
			BigDecimal accum; // the single accumulator
			MathContext newPrecision = mc; // MathContext by default

			// In particular cases, it reduces the problem to call the other 'pow()'
			if ((n == 0) || ((number.IsZero) && (n > 0))) {
				return Pow(number, n);
			}
			if ((m > 999999999) || ((mcPrecision == 0) && (n < 0))
			    || ((mcPrecision > 0) && (elength > mcPrecision))) {
				// math.07=Invalid Operation
				throw new ArithmeticException(Messages.math07); //$NON-NLS-1$
			}
			if (mcPrecision > 0) {
				newPrecision = new MathContext(mcPrecision + elength + 1,
					mc.RoundingMode);
			}
			// The result is calculated as if 'n' were positive        
			accum = BigMath.Round(number, newPrecision);
			oneBitMask = Utils.HighestOneBit(m) >> 1;

			while (oneBitMask > 0) {
				accum = BigMath.Multiply(accum, accum, newPrecision);
				if ((m & oneBitMask) == oneBitMask) {
					accum = BigMath.Multiply(accum, number, newPrecision);
				}
				oneBitMask >>= 1;
			}
			// If 'n' is negative, the value is divided into 'ONE'
			if (n < 0) {
				accum = Divide(BigDecimal.One, accum, newPrecision);
			}
			// The final value is rounded to the destination precision
			accum.InplaceRound(mc);
			return accum;
		}

		/// <summary>
		/// Returns a <see cref="BigDecimal"/> with the specified scale, rounding the unscaled value if necessary.
		/// </summary>
		/// <param name="number">The <see cref="BigDecimal"/> value to rescale.</param>
		/// <param name="newScale">The target scale for the result.</param>
		/// <param name="roundingMode">The <see cref="RoundingMode"/> to apply when scaling down (i.e., reducing the scale).</param>
		/// <returns>A <see cref="BigDecimal"/> with the specified scale, rounded as necessary.</returns>
		public static BigDecimal Scale(BigDecimal number, int newScale, RoundingMode roundingMode) {
			long diffScale = newScale - (long)number.Scale;
			// Let be:  'number' = [u,s]        
			if (diffScale == 0) {
				return number;
			}
			if (diffScale > 0) {
				// return  [u * 10^(s2 - s), newScale]
				if (diffScale < BigDecimal.LongTenPow.Length &&
				    (number.BitLength + BigDecimal.LongTenPowBitLength[(int)diffScale]) < 64) {
					return BigDecimal.Create(number.SmallValue * BigDecimal.LongTenPow[(int)diffScale], newScale);
				}
				return new BigDecimal(Multiplication.MultiplyByTenPow(number.UnscaledValue, (int)diffScale), newScale);
			}
			// diffScale < 0
			// return  [u,s] / [1,newScale]  with the appropriate scale and rounding
			if (number.BitLength < 64 && -diffScale < BigDecimal.LongTenPow.Length) {
				return DividePrimitiveLongs(number.SmallValue, BigDecimal.LongTenPow[(int)-diffScale], newScale, roundingMode);
			}

			return DivideBigIntegers(number.UnscaledValue, Multiplication.PowerOf10(-diffScale), newScale, roundingMode);
		}

		/// <summary>
		/// Moves the decimal point of the specified <see cref="BigDecimal"/> to a new scale.
		/// </summary>
		/// <param name="number">The <see cref="BigDecimal"/> value.</param>
		/// <param name="newScale">The new scale for the result (negative values move the decimal point to the right).</param>
		/// <returns>A <see cref="BigDecimal"/> with the decimal point moved to the specified scale.</returns>
		public static BigDecimal MovePoint(BigDecimal number, long newScale) {
			if (number.IsZero) {
				return BigDecimal.GetZeroScaledBy(System.Math.Max(newScale, 0));
			}
			/* When:  'n'== Integer.MIN_VALUE  isn't possible to call to movePointRight(-n)  
			 * since  -Integer.MIN_VALUE == Integer.MIN_VALUE */
			if (newScale >= 0) {
				if (number.BitLength < 64) {
					return BigDecimal.Create(number.SmallValue, BigDecimal.ToIntScale(newScale));
				}
				return new BigDecimal(number.UnscaledValue, BigDecimal.ToIntScale(newScale));
			}
			if (-newScale < BigDecimal.LongTenPow.Length &&
			    number.BitLength + BigDecimal.LongTenPowBitLength[(int)-newScale] < 64) {
				return BigDecimal.Create(number.SmallValue * BigDecimal.LongTenPow[(int)-newScale], 0);
			}
			return new BigDecimal(Multiplication.MultiplyByTenPow(number.UnscaledValue, (int)-newScale), 0);
		}

		/// <summary>
		/// Adds two <see cref="BigDecimal"/> values together.
		/// </summary>
		/// <param name="value">The first <see cref="BigDecimal"/> summand.</param>
		/// <param name="augend">The second <see cref="BigDecimal"/> summand.</param>
		/// <returns>A <see cref="BigDecimal"/> representing the sum of the two values.</returns>
		public static BigDecimal Add(BigDecimal value, BigDecimal augend) {
			int diffScale = value.Scale - augend.Scale;
			// Fast return when some operand is zero
			if (value.IsZero) {
				if (diffScale <= 0)
					return augend;
				if (augend.IsZero)
					return value;
			} else if (augend.IsZero) {
				if (diffScale >= 0)
					return value;
			}
			// Let be:  this = [u1,s1]  and  augend = [u2,s2]
			if (diffScale == 0) {
				// case s1 == s2: [u1 + u2 , s1]
				if (System.Math.Max(value.BitLength, augend.BitLength) + 1 < 64) {
					return BigDecimal.Create(value.SmallValue + augend.SmallValue, value.Scale);
				}
				return new BigDecimal(value.UnscaledValue + augend.UnscaledValue, value.Scale);
			}
			if (diffScale > 0)
				// case s1 > s2 : [(u1 + u2) * 10 ^ (s1 - s2) , s1]
				return AddAndMult10(value, augend, diffScale);

			// case s2 > s1 : [(u2 + u1) * 10 ^ (s2 - s1) , s2]
			return AddAndMult10(augend, value, -diffScale);
		}

		/// <summary>
		/// Adds two <see cref="BigDecimal"/> values after scaling the second by a power of ten.
		/// </summary>
		/// <param name="thisValue">The first <see cref="BigDecimal"/> value (the one with the larger scale).</param>
		/// <param name="augend">The second <see cref="BigDecimal"/> value to scale and add.</param>
		/// <param name="diffScale">The difference in scale between the two values.</param>
		/// <returns>A <see cref="BigDecimal"/> representing the sum.</returns>
		private static BigDecimal AddAndMult10(BigDecimal thisValue, BigDecimal augend, int diffScale) {
			if (diffScale < BigDecimal.LongTenPow.Length &&
			    System.Math.Max(thisValue.BitLength, augend.BitLength + BigDecimal.LongTenPowBitLength[diffScale]) + 1 < 64) {
				return BigDecimal.Create(thisValue.SmallValue + augend.SmallValue * BigDecimal.LongTenPow[diffScale], thisValue.Scale);
			}
			return new BigDecimal(
				thisValue.UnscaledValue + Multiplication.MultiplyByTenPow(augend.UnscaledValue, diffScale),
				thisValue.Scale);
		}

		/// <summary>
		/// Adds two <see cref="BigDecimal"/> values with the precision specified by the given <see cref="MathContext"/>.
		/// </summary>
		/// <param name="value">The first <see cref="BigDecimal"/> summand.</param>
		/// <param name="augend">The second <see cref="BigDecimal"/> summand.</param>
		/// <param name="mc">The <see cref="MathContext"/> that specifies the precision and rounding mode.</param>
		/// <returns>A <see cref="BigDecimal"/> representing the sum, rounded to the specified precision.</returns>
		public static BigDecimal Add(BigDecimal value, BigDecimal augend, MathContext mc) {
			BigDecimal larger; // operand with the largest unscaled value
			BigDecimal smaller; // operand with the smallest unscaled value
			BigInteger tempBi;
			long diffScale = (long)value.Scale - augend.Scale;

			// Some operand is zero or the precision is infinity  
			if ((augend.IsZero) || (value.IsZero) || (mc.Precision == 0)) {
				return BigMath.Round(Add(value, augend), mc);
			}
			// Cases where there is room for optimizations
			if (value.AproxPrecision() < diffScale - 1) {
				larger = augend;
				smaller = value;
			} else if (augend.AproxPrecision() < -diffScale - 1) {
				larger = value;
				smaller = augend;
			} else {
				// No optimization is done 
				return BigMath.Round(Add(value, augend), mc);
			}
			if (mc.Precision >= larger.AproxPrecision()) {
				// No optimization is done
				return BigMath.Round(Add(value, augend), mc);
			}

			// Cases where it's unnecessary to add two numbers with very different scales 
			var largerSignum = larger.Sign;
			if (largerSignum == smaller.Sign) {
				tempBi = Multiplication.MultiplyByPositiveInt(larger.UnscaledValue, 10) +
				         BigInteger.FromInt64(largerSignum);
			} else {
				tempBi = larger.UnscaledValue - BigInteger.FromInt64(largerSignum);
				tempBi = Multiplication.MultiplyByPositiveInt(tempBi, 10) +
				         BigInteger.FromInt64(largerSignum * 9);
			}
			// Rounding the improved adding 
			larger = new BigDecimal(tempBi, larger.Scale + 1);
			return BigMath.Round(larger, mc);
		}

		/// <summary>
		/// Subtracts one <see cref="BigDecimal"/> from another.
		/// </summary>
		/// <param name="value">The <see cref="BigDecimal"/> value to subtract from (minuend).</param>
		/// <param name="subtrahend">The <see cref="BigDecimal"/> value to subtract (subtrahend).</param>
		/// <returns>A <see cref="BigDecimal"/> representing the difference.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="subtrahend"/> is <c>null</c>.</exception>
		/// <example>
		/// <code>
		/// BigDecimal result = BigDecimalMath.Subtract(
		///     new BigDecimal("5.5"),
		///     new BigDecimal("2.3"));
		/// // result is 3.2
		/// </code>
		/// </example>
		public static BigDecimal Subtract(BigDecimal value, BigDecimal subtrahend) {
			if (subtrahend == null)
				throw new ArgumentNullException("subtrahend");

			int diffScale = value.Scale - subtrahend.Scale;

			// Fast return when some operand is zero
			if (value.IsZero) {
				if (diffScale <= 0) {
					return -subtrahend;
				}
				if (subtrahend.IsZero) {
					return value;
				}
			} else if (subtrahend.IsZero) {
				if (diffScale >= 0) {
					return value;
				}
			}
			// Let be: this = [u1,s1] and subtrahend = [u2,s2] so:
			if (diffScale == 0) {
				// case s1 = s2 : [u1 - u2 , s1]
				if (System.Math.Max(value.BitLength, subtrahend.BitLength) + 1 < 64) {
					return BigDecimal.Create(value.SmallValue - subtrahend.SmallValue, value.Scale);
				}
				return new BigDecimal(value.UnscaledValue - subtrahend.UnscaledValue, value.Scale);
			}
			if (diffScale > 0) {
				// case s1 > s2 : [ u1 - u2 * 10 ^ (s1 - s2) , s1 ]
				if (diffScale < BigDecimal.LongTenPow.Length &&
				    System.Math.Max(value.BitLength, subtrahend.BitLength + BigDecimal.LongTenPowBitLength[diffScale]) + 1 < 64) {
					return BigDecimal.Create(value.SmallValue - subtrahend.SmallValue * BigDecimal.LongTenPow[diffScale], value.Scale);
				}
				return new BigDecimal(
					value.UnscaledValue - Multiplication.MultiplyByTenPow(subtrahend.UnscaledValue, diffScale),
					value.Scale);
			}

			// case s2 > s1 : [ u1 * 10 ^ (s2 - s1) - u2 , s2 ]
			diffScale = -diffScale;
			if (diffScale < BigDecimal.LongTenPow.Length &&
			    System.Math.Max(value.BitLength + BigDecimal.LongTenPowBitLength[diffScale], subtrahend.BitLength) + 1 < 64) {
				return BigDecimal.Create(value.SmallValue * BigDecimal.LongTenPow[diffScale] - subtrahend.SmallValue, subtrahend.Scale);
			}

			return new BigDecimal(Multiplication.MultiplyByTenPow(value.UnscaledValue, diffScale) -
			                      subtrahend.UnscaledValue, subtrahend.Scale);
		}

		/// <summary>
		/// Subtracts one <see cref="BigDecimal"/> from another with the precision specified by the given <see cref="MathContext"/>.
		/// </summary>
		/// <param name="value">The <see cref="BigDecimal"/> value to subtract from (minuend).</param>
		/// <param name="subtrahend">The <see cref="BigDecimal"/> value to subtract (subtrahend).</param>
		/// <param name="mc">The <see cref="MathContext"/> that specifies the precision and rounding mode.</param>
		/// <returns>A <see cref="BigDecimal"/> representing the difference, rounded to the specified precision.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="subtrahend"/> or <paramref name="mc"/> is <c>null</c>.</exception>
		public static BigDecimal Subtract(BigDecimal value, BigDecimal subtrahend, MathContext mc) {
			if (subtrahend == null)
				throw new ArgumentNullException("subtrahend");
			if (mc == null)
				throw new ArgumentNullException("mc");

			long diffScale = subtrahend.Scale - (long)value.Scale;

			// Some operand is zero or the precision is infinity  
			if ((subtrahend.IsZero) || (value.IsZero) || (mc.Precision == 0))
				return BigMath.Round(Subtract(value, subtrahend), mc);

			// Now:   this != 0   and   subtrahend != 0
			if (subtrahend.AproxPrecision() < diffScale - 1) {
				// Cases where it is unnecessary to subtract two numbers with very different scales
				if (mc.Precision < value.AproxPrecision()) {
					var thisSignum = value.Sign;
					BigInteger tempBI;
					if (thisSignum != subtrahend.Sign) {
						tempBI = Multiplication.MultiplyByPositiveInt(value.UnscaledValue, 10) +
						         BigInteger.FromInt64(thisSignum);
					} else {
						tempBI = value.UnscaledValue - BigInteger.FromInt64(thisSignum);
						tempBI = Multiplication.MultiplyByPositiveInt(tempBI, 10) +
						         BigInteger.FromInt64(thisSignum * 9);
					}
					// Rounding the improved subtracting
					var leftOperand = new BigDecimal(tempBI, value.Scale + 1); // it will be only the left operand (this) 
					return BigMath.Round(leftOperand, mc);
				}
			}

			// No optimization is done
			return BigMath.Round(Subtract(value, subtrahend), mc);
		}

	}
}
