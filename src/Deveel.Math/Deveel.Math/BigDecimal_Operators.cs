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
			return BigMath.Remainder(a, b, new MathContext(a.Precision));
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
