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
	public sealed partial class BigDecimal {
		//private static BigDecimal DivideBigIntegers(BigInteger scaledDividend, BigInteger scaledDivisor, int scale,
		//	RoundingMode roundingMode) {
		//	BigInteger remainder;
		//	BigInteger quotient = BigMath.DivideAndRemainder(scaledDividend, scaledDivisor, out remainder);
		//	if (remainder.Sign == 0) {
		//		return new BigDecimal(quotient, scale);
		//	}
		//	int sign = scaledDividend.Sign * scaledDivisor.Sign;
		//	int compRem; // 'compare to remainder'
		//	if (scaledDivisor.BitLength < 63) {
		//		// 63 in order to avoid out of long after <<1
		//		long rem = remainder.ToInt64();
		//		long divisor = scaledDivisor.ToInt64();
		//		compRem = LongCompareTo(System.Math.Abs(rem) << 1, System.Math.Abs(divisor));
		//		// To look if there is a carry
		//		compRem = RoundingBehavior(BigInteger.TestBit(quotient, 0) ? 1 : 0,
		//			sign * (5 + compRem), roundingMode);

		//	} else {
		//		// Checking if:  remainder * 2 >= scaledDivisor 
		//		compRem = BigMath.Abs(remainder).ShiftLeftOneBit().CompareTo(BigMath.Abs(scaledDivisor));
		//		compRem = RoundingBehavior(BigInteger.TestBit(quotient, 0) ? 1 : 0,
		//			sign * (5 + compRem), roundingMode);
		//	}
		//	if (compRem != 0) {
		//		if (quotient.BitLength < 63) {
		//			return ValueOf(quotient.ToInt64() + compRem, scale);
		//		}
		//		quotient += BigInteger.FromInt64(compRem);
		//		return new BigDecimal(quotient, scale);
		//	}
		//	// Constructing the result with the appropriate unscaled value
		//	return new BigDecimal(quotient, scale);
		//}

		//private static BigDecimal DividePrimitiveLongs(long scaledDividend, long scaledDivisor, int scale,
		//	RoundingMode roundingMode) {
		//	long quotient = scaledDividend / scaledDivisor;
		//	long remainder = scaledDividend % scaledDivisor;
		//	int sign = System.Math.Sign(scaledDividend) * System.Math.Sign(scaledDivisor);
		//	if (remainder != 0) {
		//		// Checking if:  remainder * 2 >= scaledDivisor
		//		int compRem; // 'compare to remainder'
		//		compRem = LongCompareTo(System.Math.Abs(remainder) << 1, System.Math.Abs(scaledDivisor));
		//		// To look if there is a carry
		//		quotient += RoundingBehavior(((int)quotient) & 1, sign * (5 + compRem), roundingMode);
		//	}
		//	// Constructing the result with the appropriate unscaled value
		//	return ValueOf(quotient, scale);
		//}

		///**
		// * Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
		// * The scale of the result is the scale of {@code this}. If rounding is
		// * required to meet the specified scale, then the specified rounding mode
		// * {@code roundingMode} is applied.
		// *
		// * @param divisor
		// *            value by which {@code this} is divided.
		// * @param roundingMode
		// *            rounding mode to be used to round the result.
		// * @return {@code this / divisor} rounded according to the given rounding
		// *         mode.
		// * @throws NullPointerException
		// *             if {@code divisor == null}.
		// * @throws IllegalArgumentException
		// *             if {@code roundingMode} is not a valid rounding mode.
		// * @throws ArithmeticException
		// *             if {@code divisor == 0}.
		// * @throws ArithmeticException
		// *             if {@code roundingMode == ROUND_UNNECESSARY} and rounding is
		// *             necessary according to the scale of this.
		// */

		//public BigDecimal Divide(BigDecimal divisor, int roundingMode) {
		//	if (!Enum.IsDefined(typeof(RoundingMode), roundingMode))
		//		throw new ArgumentException();

		//	return Divide(divisor, _scale, (RoundingMode)roundingMode);
		//}

		///**
		// * Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
		// * The scale of the result is the scale of {@code this}. If rounding is
		// * required to meet the specified scale, then the specified rounding mode
		// * {@code roundingMode} is applied.
		// *
		// * @param divisor
		// *            value by which {@code this} is divided.
		// * @param roundingMode
		// *            rounding mode to be used to round the result.
		// * @return {@code this / divisor} rounded according to the given rounding
		// *         mode.
		// * @throws NullPointerException
		// *             if {@code divisor == null} or {@code roundingMode == null}.
		// * @throws ArithmeticException
		// *             if {@code divisor == 0}.
		// * @throws ArithmeticException
		// *             if {@code roundingMode == RoundingMode.UNNECESSARY} and
		// *             rounding is necessary according to the scale of this.
		// */

		//public BigDecimal Divide(BigDecimal divisor, RoundingMode roundingMode) {
		//	return Divide(divisor, _scale, roundingMode);
		//}

		///**
		// * Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
		// * The scale of the result is the difference of the scales of {@code this}
		// * and {@code divisor}. If the exact result requires more digits, then the
		// * scale is adjusted accordingly. For example, {@code 1/128 = 0.0078125}
		// * which has a scale of {@code 7} and precision {@code 5}.
		// *
		// * @param divisor
		// *            value by which {@code this} is divided.
		// * @return {@code this / divisor}.
		// * @throws NullPointerException
		// *             if {@code divisor == null}.
		// * @throws ArithmeticException
		// *             if {@code divisor == 0}.
		// * @throws ArithmeticException
		// *             if the result cannot be represented exactly.
		// */

		//public BigDecimal Divide(BigDecimal divisor) {
		//	BigInteger p = this.GetUnscaledValue();
		//	BigInteger q = divisor.GetUnscaledValue();
		//	BigInteger gcd; // greatest common divisor between 'p' and 'q'
		//	BigInteger quotient;
		//	BigInteger remainder;
		//	long diffScale = (long)_scale - divisor._scale;
		//	int newScale; // the new scale for final quotient
		//	int k; // number of factors "2" in 'q'
		//	int l = 0; // number of factors "5" in 'q'
		//	int i = 1;
		//	int lastPow = FivePow.Length - 1;

		//	if (divisor.IsZero) {
		//		// math.04=Division by zero
		//		throw new ArithmeticException(Messages.math04); //$NON-NLS-1$
		//	}
		//	if (p.Sign == 0) {
		//		return GetZeroScaledBy(diffScale);
		//	}
		//	// To divide both by the GCD
		//	gcd = BigMath.Gcd(p, q);
		//	p = p / gcd;
		//	q = q / gcd;
		//	// To simplify all "2" factors of q, dividing by 2^k
		//	k = q.LowestSetBit;
		//	q = q >> k;
		//	// To simplify all "5" factors of q, dividing by 5^l
		//	do {
		//		quotient = BigMath.DivideAndRemainder(q, FivePow[i], out remainder);
		//		if (remainder.Sign == 0) {
		//			l += i;
		//			if (i < lastPow) {
		//				i++;
		//			}
		//			q = quotient;
		//		} else {
		//			if (i == 1) {
		//				break;
		//			}
		//			i = 1;
		//		}
		//	} while (true);
		//	// If  abs(q) != 1  then the quotient is periodic
		//	if (!BigMath.Abs(q).Equals(BigInteger.One)) {
		//		// math.05=Non-terminating decimal expansion; no exact representable decimal result.
		//		throw new ArithmeticException(Messages.math05); //$NON-NLS-1$
		//	}
		//	// The sign of the is fixed and the quotient will be saved in 'p'
		//	if (q.Sign < 0) {
		//		p = -p;
		//	}
		//	// Checking if the new scale is out of range
		//	newScale = ToIntScale(diffScale + System.Math.Max(k, l));
		//	// k >= 0  and  l >= 0  implies that  k - l  is in the 32-bit range
		//	i = k - l;

		//	p = (i > 0)
		//		? Multiplication.MultiplyByFivePow(p, i)
		//		: p << -i;
		//	return new BigDecimal(p, newScale);
		//}

		///**
		// * Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
		// * The result is rounded according to the passed context {@code mc}. If the
		// * passed math context specifies precision {@code 0}, then this call is
		// * equivalent to {@code this.divide(divisor)}.
		// *
		// * @param divisor
		// *            value by which {@code this} is divided.
		// * @param mc
		// *            rounding mode and precision for the result of this operation.
		// * @return {@code this / divisor}.
		// * @throws NullPointerException
		// *             if {@code divisor == null} or {@code mc == null}.
		// * @throws ArithmeticException
		// *             if {@code divisor == 0}.
		// * @throws ArithmeticException
		// *             if {@code mc.getRoundingMode() == UNNECESSARY} and rounding
		// *             is necessary according {@code mc.getPrecision()}.
		// */

		//public BigDecimal Divide(BigDecimal divisor, MathContext mc) {
		//	/* Calculating how many zeros must be append to 'dividend'
		//	 * to obtain a  quotient with at least 'mc.precision()' digits */
		//	long traillingZeros = mc.Precision + 2L
		//	                      + divisor.AproxPrecision() - AproxPrecision();
		//	long diffScale = (long)_scale - divisor._scale;
		//	long newScale = diffScale; // scale of the final quotient
		//	int compRem; // to compare the remainder
		//	int i = 1; // index   
		//	int lastPow = TenPow.Length - 1; // last power of ten
		//	BigInteger integerQuot; // for temporal results
		//	BigInteger quotient = GetUnscaledValue();
		//	BigInteger remainder;
		//	// In special cases it reduces the problem to call the dual method
		//	if ((mc.Precision == 0) || (IsZero) || (divisor.IsZero))
		//		return Divide(divisor);

		//	if (traillingZeros > 0) {
		//		// To append trailing zeros at end of dividend
		//		quotient = GetUnscaledValue() * Multiplication.PowerOf10(traillingZeros);
		//		newScale += traillingZeros;
		//	}
		//	quotient = BigMath.DivideAndRemainder(quotient, divisor.GetUnscaledValue(), out remainder);
		//	integerQuot = quotient;
		//	// Calculating the exact quotient with at least 'mc.precision()' digits
		//	if (remainder.Sign != 0) {
		//		// Checking if:   2 * remainder >= divisor ?
		//		compRem = remainder.ShiftLeftOneBit().CompareTo(divisor.GetUnscaledValue());
		//		// quot := quot * 10 + r;     with 'r' in {-6,-5,-4, 0,+4,+5,+6}
		//		integerQuot = (integerQuot * BigInteger.Ten)+
		//			BigInteger.FromInt64(quotient.Sign * (5 + compRem));
		//		newScale++;
		//	} else {
		//		// To strip trailing zeros until the preferred scale is reached
		//		while (!BigInteger.TestBit(integerQuot, 0)) {
		//			quotient = BigMath.DivideAndRemainder(integerQuot, TenPow[i], out remainder);
		//			if ((remainder.Sign == 0)
		//			    && (newScale - i >= diffScale)) {
		//				newScale -= i;
		//				if (i < lastPow) {
		//					i++;
		//				}
		//				integerQuot = quotient;
		//			} else {
		//				if (i == 1) {
		//					break;
		//				}
		//				i = 1;
		//			}
		//		}
		//	}
		//	// To perform rounding
		//	return new BigDecimal(integerQuot, ToIntScale(newScale), mc);
		//}

		///**
		// * Returns a new {@code BigDecimal} whose value is the integral part of
		// * {@code this / divisor}. The quotient is rounded down towards zero to the
		// * next integer. For example, {@code 0.5/0.2 = 2}.
		// *
		// * @param divisor
		// *            value by which {@code this} is divided.
		// * @return integral part of {@code this / divisor}.
		// * @throws NullPointerException
		// *             if {@code divisor == null}.
		// * @throws ArithmeticException
		// *             if {@code divisor == 0}.
		// */

		//public BigDecimal DivideToIntegralValue(BigDecimal divisor) {
		//	BigInteger integralValue; // the integer of result
		//	BigInteger powerOfTen; // some power of ten
		//	BigInteger quotient;
		//	BigInteger remainder;
		//	long newScale = (long)_scale - divisor._scale;
		//	long tempScale = 0;
		//	int i = 1;
		//	int lastPow = TenPow.Length - 1;

		//	if (divisor.IsZero) {
		//		// math.04=Division by zero
		//		throw new ArithmeticException(Messages.math04); //$NON-NLS-1$
		//	}
		//	if ((divisor.AproxPrecision() + newScale > this.AproxPrecision() + 1L)
		//	    || (this.IsZero)) {
		//		/* If the divisor's integer part is greater than this's integer part,
		//		 * the result must be zero with the appropriate scale */
		//		integralValue = BigInteger.Zero;
		//	} else if (newScale == 0) {
		//		integralValue = GetUnscaledValue() / divisor.GetUnscaledValue();
		//	} else if (newScale > 0) {
		//		powerOfTen = Multiplication.PowerOf10(newScale);
		//		integralValue = GetUnscaledValue() / (divisor.GetUnscaledValue() * powerOfTen);
		//		integralValue = integralValue * powerOfTen;
		//	} else {
		//		// (newScale < 0)
		//		powerOfTen = Multiplication.PowerOf10(-newScale);
		//		integralValue = (GetUnscaledValue() * powerOfTen) / divisor.GetUnscaledValue();
		//		// To strip trailing zeros approximating to the preferred scale
		//		while (!BigInteger.TestBit(integralValue, 0)) {
		//			quotient = BigMath.DivideAndRemainder(integralValue, TenPow[i], out remainder);
		//			if ((remainder.Sign == 0)
		//			    && (tempScale - i >= newScale)) {
		//				tempScale -= i;
		//				if (i < lastPow) {
		//					i++;
		//				}
		//				integralValue = quotient;
		//			} else {
		//				if (i == 1) {
		//					break;
		//				}
		//				i = 1;
		//			}
		//		}
		//		newScale = tempScale;
		//	}
		//	return ((integralValue.Sign == 0)
		//		? GetZeroScaledBy(newScale)
		//		: new BigDecimal(integralValue, ToIntScale(newScale)));
		//}

		///**
		// * Returns a new {@code BigDecimal} whose value is the integral part of
		// * {@code this / divisor}. The quotient is rounded down towards zero to the
		// * next integer. The rounding mode passed with the parameter {@code mc} is
		// * not considered. But if the precision of {@code mc > 0} and the integral
		// * part requires more digits, then an {@code ArithmeticException} is thrown.
		// *
		// * @param divisor
		// *            value by which {@code this} is divided.
		// * @param mc
		// *            math context which determines the maximal precision of the
		// *            result.
		// * @return integral part of {@code this / divisor}.
		// * @throws NullPointerException
		// *             if {@code divisor == null} or {@code mc == null}.
		// * @throws ArithmeticException
		// *             if {@code divisor == 0}.
		// * @throws ArithmeticException
		// *             if {@code mc.getPrecision() > 0} and the result requires more
		// *             digits to be represented.
		// */

		//public BigDecimal DivideToIntegralValue(BigDecimal divisor, MathContext mc) {
		//	int mcPrecision = mc.Precision;
		//	int diffPrecision = Precision - divisor.Precision;
		//	int lastPow = TenPow.Length - 1;
		//	long diffScale = (long)_scale - divisor._scale;
		//	long newScale = diffScale;
		//	long quotPrecision = diffPrecision - diffScale + 1;
		//	BigInteger quotient;
		//	BigInteger remainder;
		//	// In special cases it call the dual method
		//	if ((mcPrecision == 0) || (IsZero) || (divisor.IsZero)) {
		//		return DivideToIntegralValue(divisor);
		//	}
		//	// Let be:   this = [u1,s1]   and   divisor = [u2,s2]
		//	if (quotPrecision <= 0) {
		//		quotient = BigInteger.Zero;
		//	} else if (diffScale == 0) {
		//		// CASE s1 == s2:  to calculate   u1 / u2 
		//		quotient = GetUnscaledValue() / divisor.GetUnscaledValue();
		//	} else if (diffScale > 0) {
		//		// CASE s1 >= s2:  to calculate   u1 / (u2 * 10^(s1-s2)  
		//		quotient = GetUnscaledValue() / (divisor.GetUnscaledValue() * Multiplication.PowerOf10(diffScale));
		//		// To chose  10^newScale  to get a quotient with at least 'mc.precision()' digits
		//		newScale = System.Math.Min(diffScale, System.Math.Max(mcPrecision - quotPrecision + 1, 0));
		//		// To calculate: (u1 / (u2 * 10^(s1-s2)) * 10^newScale
		//		quotient = quotient * Multiplication.PowerOf10(newScale);
		//	} else {
		//		// CASE s2 > s1:   
		//		/* To calculate the minimum power of ten, such that the quotient 
		//		 *   (u1 * 10^exp) / u2   has at least 'mc.precision()' digits. */
		//		long exp = System.Math.Min(-diffScale, System.Math.Max((long)mcPrecision - diffPrecision, 0));
		//		long compRemDiv;
		//		// Let be:   (u1 * 10^exp) / u2 = [q,r]  
		//		quotient = BigMath.DivideAndRemainder(GetUnscaledValue() * Multiplication.PowerOf10(exp),
		//			divisor.GetUnscaledValue(), out remainder);
		//		newScale += exp; // To fix the scale
		//		exp = -newScale; // The remaining power of ten
		//		// If after division there is a remainder...
		//		if ((remainder.Sign != 0) && (exp > 0)) {
		//			// Log10(r) + ((s2 - s1) - exp) > mc.precision ?
		//			compRemDiv = (new BigDecimal(remainder)).Precision
		//			             + exp - divisor.Precision;
		//			if (compRemDiv == 0) {
		//				// To calculate:  (r * 10^exp2) / u2
		//				remainder = (remainder * Multiplication.PowerOf10(exp)) / divisor.GetUnscaledValue();
		//				compRemDiv = System.Math.Abs(remainder.Sign);
		//			}
		//			if (compRemDiv > 0) {
		//				// The quotient won't fit in 'mc.precision()' digits
		//				// math.06=Division impossible
		//				throw new ArithmeticException(Messages.math06); //$NON-NLS-1$
		//			}
		//		}
		//	}
		//	// Fast return if the quotient is zero
		//	if (quotient.Sign == 0) {
		//		return GetZeroScaledBy(diffScale);
		//	}
		//	BigInteger strippedBI = quotient;
		//	BigDecimal integralValue = new BigDecimal(quotient);
		//	long resultPrecision = integralValue.Precision;
		//	int i = 1;
		//	// To strip trailing zeros until the specified precision is reached
		//	while (!BigInteger.TestBit(strippedBI, 0)) {
		//		quotient = BigMath.DivideAndRemainder(strippedBI, TenPow[i], out remainder);
		//		if ((remainder.Sign == 0) &&
		//		    ((resultPrecision - i >= mcPrecision)
		//		     || (newScale - i >= diffScale))) {
		//			resultPrecision -= i;
		//			newScale -= i;
		//			if (i < lastPow) {
		//				i++;
		//			}
		//			strippedBI = quotient;
		//		} else {
		//			if (i == 1) {
		//				break;
		//			}
		//			i = 1;
		//		}
		//	}
		//	// To check if the result fit in 'mc.precision()' digits
		//	if (resultPrecision > mcPrecision) {
		//		// math.06=Division impossible
		//		throw new ArithmeticException(Messages.math06); //$NON-NLS-1$
		//	}
		//	integralValue._scale = ToIntScale(newScale);
		//	integralValue.SetUnscaledValue(strippedBI);
		//	return integralValue;
		//}

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

		public BigDecimal Remainder(BigDecimal divisor) {
			BigDecimal remainder;
			DivideAndRemainder(divisor, out remainder);
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

		public BigDecimal Remainder(BigDecimal divisor, MathContext mc) {
			BigDecimal remainder;
			DivideAndRemainder(divisor, mc, out remainder);
			return remainder;
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

		public BigDecimal DivideAndRemainder(BigDecimal divisor, out BigDecimal remainder) {
			var quotient = BigDecimalMath.DivideToIntegralValue(this, divisor);
			remainder = Subtract(quotient.Multiply(divisor));
			return quotient;
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

		public BigDecimal DivideAndRemainder(BigDecimal divisor, MathContext mc, out BigDecimal remainder) {
			var quotient = BigDecimalMath.DivideToIntegralValue(this, divisor, mc);
			remainder = Subtract(quotient.Multiply(divisor));
			return quotient;
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

		public BigDecimal Pow(int n) {
			if (n == 0) {
				return One;
			}
			if ((n < 0) || (n > 999999999)) {
				// math.07=Invalid Operation
				throw new ArithmeticException(Messages.math07); //$NON-NLS-1$
			}
			long newScale = _scale * (long)n;
			// Let be: this = [u,s]   so:  this^n = [u^n, s*n]
			return ((IsZero)
				? GetZeroScaledBy(newScale)
				: new BigDecimal(BigMath.Pow(GetUnscaledValue(), n), ToIntScale(newScale)));
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

		public BigDecimal Pow(int n, MathContext mc) {
			// The ANSI standard X3.274-1996 algorithm
			int m = System.Math.Abs(n);
			int mcPrecision = mc.Precision;
			int elength = (int)System.Math.Log10(m) + 1; // decimal digits in 'n'
			int oneBitMask; // mask of bits
			BigDecimal accum; // the single accumulator
			MathContext newPrecision = mc; // MathContext by default

			// In particular cases, it reduces the problem to call the other 'pow()'
			if ((n == 0) || ((IsZero) && (n > 0))) {
				return Pow(n);
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
			accum = Round(newPrecision);
			oneBitMask = Utils.HighestOneBit(m) >> 1;

			while (oneBitMask > 0) {
				accum = accum.Multiply(accum, newPrecision);
				if ((m & oneBitMask) == oneBitMask) {
					accum = accum.Multiply(this, newPrecision);
				}
				oneBitMask >>= 1;
			}
			// If 'n' is negative, the value is divided into 'ONE'
			if (n < 0) {
				accum = BigDecimalMath.Divide(One, accum, newPrecision);
			}
			// The final value is rounded to the destination precision
			accum.InplaceRound(mc);
			return accum;
		}

		#region Arithmetic Operators

		public static BigDecimal operator +(BigDecimal a, BigDecimal b) {
			// In case of implicit operators apply the precision of the dividend
			return a.Add(b, new MathContext(a.Precision));
		}

		public static BigDecimal operator -(BigDecimal a, BigDecimal b) {
			// In case of implicit operators apply the precision of the dividend
			return a.Subtract(b, new MathContext(a.Precision));
		}

		public static BigDecimal operator /(BigDecimal a, BigDecimal b) {
			// In case of implicit operators apply the precision of the dividend
			return BigDecimalMath.Divide(a, b, new MathContext(a.Precision));
		}

		public static BigDecimal operator %(BigDecimal a, BigDecimal b) {
			// In case of implicit operators apply the precision of the dividend
			return a.Remainder(b, new MathContext(a.Precision));
		}

		public static BigDecimal operator *(BigDecimal a, BigDecimal b) {
			// In case of implicit operators apply the precision of the dividend
			return a.Multiply(b, new MathContext(a.Precision));
		}

		public static BigDecimal operator +(BigDecimal a) {
			return a.Plus();
		}

		public static BigDecimal operator -(BigDecimal a) {
			return a.Negate();
		}

		public static bool operator ==(BigDecimal a, BigDecimal b) {
			if ((object)a == null && (object)b == null)
				return true;
			if ((object)a == null)
				return false;
			return a.Equals(b);
		}

		public static bool operator !=(BigDecimal a, BigDecimal b) {
			return !(a == b);
		}

		public static bool operator >(BigDecimal a, BigDecimal b) {
			return a.CompareTo(b) < 0;
		}

		public static bool operator <(BigDecimal a, BigDecimal b) {
			return a.CompareTo(b) > 0;
		}

		public static bool operator >=(BigDecimal a, BigDecimal b) {
			return a == b || a > b;
		}

		public static bool operator <=(BigDecimal a, BigDecimal b) {
			return a == b || a < b;
		}

		#endregion

		#region Implicit Operators

		public static implicit operator Int16(BigDecimal d) {
			return d.ToInt16Exact();
		}

		public static implicit operator Int32(BigDecimal d) {
			return d.ToInt32();
		}

		public static implicit operator Int64(BigDecimal d) {
			return d.ToInt64();
		}

		public static implicit operator Single(BigDecimal d) {
			return d.ToSingle();
		}

		public static implicit operator Double(BigDecimal d) {
			return d.ToDouble();
		}

		public static implicit operator BigInteger(BigDecimal d) {
			return d.ToBigInteger();
		}

		public static implicit operator String(BigDecimal d) {
			return d.ToString();
		}

		public static implicit operator BigDecimal(long value) {
			return new BigDecimal(value);
		}

		public static implicit operator BigDecimal(double value) {
			return new BigDecimal(value);
		}

		public static implicit operator BigDecimal(int value) {
			return new BigDecimal(value);
		}

		public static implicit operator BigDecimal(BigInteger value) {
			return new BigDecimal(value);
		}

		#endregion
	}
}
