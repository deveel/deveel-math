using System;

namespace Deveel.Math {
	public sealed partial class BigInteger {
		/// <summary>
		/// Computes the negation of this <see cref="BigInteger"/>.
		/// </summary>
		/// <returns>
		/// Returns an instance of <see cref="BigInteger"/> that is the negated value
		/// of this instance.
		/// </returns>
		public BigInteger Negate() {
			return ((sign == 0) ? this : new BigInteger(-sign, numberLength, digits));
		}

		/**
		 * Returns a new {@code BigInteger} whose value is {@code this >> n}. For
		 * negative arguments, the result is also negative. The shift distance may
		 * be negative which means that {@code this} is shifted left.
		 * <p>
		 * <b>Implementation Note:</b> Usage of this method on negative values is
		 * not recommended as the current implementation is not efficient.
		 *
		 * @param n
		 *            shift distance
		 * @return {@code this >> n} if {@code n >= 0}; {@code this << (-n)}
		 *         otherwise
		 */
		public BigInteger ShiftRight(int n) {
			if ((n == 0) || (sign == 0)) {
				return this;
			}
			return ((n > 0) ? BitLevel.ShiftRight(this, n) : BitLevel.ShiftLeft(
				this, -n));
		}

		/**
		 * Returns a new {@code BigInteger} whose value is {@code this << n}. The
		 * result is equivalent to {@code this * 2^n} if n >= 0. The shift distance
		 * may be negative which means that {@code this} is shifted right. The
		 * result then corresponds to {@code floor(this / 2^(-n))}.
		 * <p>
		 * <b>Implementation Note:</b> Usage of this method on negative values is
		 * not recommended as the current implementation is not efficient.
		 *
		 * @param n
		 *            shift distance.
		 * @return {@code this << n} if {@code n >= 0}; {@code this >> (-n)}.
		 *         otherwise
		 */
		public BigInteger ShiftLeft(int n) {
			if ((n == 0) || (sign == 0)) {
				return this;
			}
			return ((n > 0) ? BitLevel.ShiftLeft(this, n) : BitLevel.ShiftRight(this, -n));
		}

		internal BigInteger ShiftLeftOneBit() {
			return (sign == 0) ? this : BitLevel.ShiftLeftOneBit(this);
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
		public BigInteger Not() {
			return Logical.Not(this);
		}

		/// <summary>
		/// Computes the bit per bit operator between this number
		/// and the given one.
		/// </summary>
		/// <param name="val">The value to be and'ed with the current.</param>
		/// <remarks>
		/// <b>Implementation Note:</b> Usage of this method is not recommended as 
		/// the current implementation is not efficient.
		/// </remarks>
		/// <returns>
		/// Returns a new <see cref="BigInteger"/> whose value is <c>this & <paramref name="val"/></c>.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="val"/> is <c>null</c>.
		/// </exception>
		public BigInteger And(BigInteger val) {
			return Logical.And(this, val);
		}

		/**
		 * Returns a new {@code BigInteger} whose value is {@code this | val}.
		 * <p>
		 * <b>Implementation Note:</b> Usage of this method is not recommended as
		 * the current implementation is not efficient.
		 *
		 * @param val
		 *            value to be or'ed with {@code this}.
		 * @return {@code this | val}.
		 * @throws NullPointerException
		 *             if {@code val == null}.
		 */
		public BigInteger Or(BigInteger val) {
			return Logical.Or(this, val);
		}

		/**
		 * Returns a new {@code BigInteger} whose value is {@code this ^ val}.
		 * <p>
		 * <b>Implementation Note:</b> Usage of this method is not recommended as
		 * the current implementation is not efficient.
		 *
		 * @param val
		 *            value to be xor'ed with {@code this}
		 * @return {@code this ^ val}
		 * @throws NullPointerException
		 *             if {@code val == null}
		 */
		public BigInteger XOr(BigInteger val) {
			return Logical.Xor(this, val);
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
		public BigInteger AndNot(BigInteger val) {
			return Logical.AndNot(this, val);
		}

		/**
 * Returns a new {@code BigInteger} whose value is {@code this * val}.
 *
 * @param val
 *            value to be multiplied with {@code this}.
 * @return {@code this * val}.
 * @throws NullPointerException
 *             if {@code val == null}.
 */
		public BigInteger Multiply(BigInteger val) {
			// This let us to throw NullPointerException when val == null
			if (val.sign == 0) {
				return Zero;
			}
			if (sign == 0) {
				return Zero;
			}
			return Multiplication.Multiply(this, val);
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
		public BigInteger Pow(int exp) {
			if (exp < 0) {
				// math.16=Negative exponent
				throw new ArithmeticException(Messages.math16); //$NON-NLS-1$
			}
			if (exp == 0) {
				return One;
			} else if (exp == 1 || Equals(One) || Equals(Zero)) {
				return this;
			}

			// if even take out 2^x factor which we can
			// calculate by shifting.
			if (!TestBit(0)) {
				int x = 1;
				while (!TestBit(x)) {
					x++;
				}
				return GetPowerOfTwo(x * exp).Multiply(this.ShiftRight(x).Pow(exp));
			}
			return Multiplication.Pow(this, exp);
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
		public BigInteger DivideAndRemainder(BigInteger divisor, out BigInteger remainder) {
			int divisorSign = divisor.sign;
			if (divisorSign == 0) {
				// math.17=BigInteger divide by zero
				throw new ArithmeticException(Messages.math17); //$NON-NLS-1$
			}
			int divisorLen = divisor.numberLength;
			int[] divisorDigits = divisor.digits;
			if (divisorLen == 1) {
				var values = Division.DivideAndRemainderByInteger(this, divisorDigits[0], divisorSign);
				remainder = values[1];
				return values[0];
			}

			int[] thisDigits = digits;
			int thisLen = numberLength;
			int cmp = (thisLen != divisorLen) ? ((thisLen > divisorLen) ? 1 : -1)
				: Elementary.compareArrays(thisDigits, divisorDigits, thisLen);
			if (cmp < 0) {
				remainder = this;
				return Zero;
			}
			int thisSign = sign;
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
		public BigInteger Divide(BigInteger divisor) {
			if (divisor.sign == 0) {
				// math.17=BigInteger divide by zero
				throw new ArithmeticException(Messages.math17); //$NON-NLS-1$
			}
			int divisorSign = divisor.sign;
			if (divisor.IsOne) {
				return ((divisor.sign > 0) ? this : this.Negate());
			}
			int thisSign = sign;
			int thisLen = numberLength;
			int divisorLen = divisor.numberLength;
			if (thisLen + divisorLen == 2) {
				long val = (digits[0] & 0xFFFFFFFFL)
				           / (divisor.digits[0] & 0xFFFFFFFFL);
				if (thisSign != divisorSign) {
					val = -val;
				}
				return FromInt64(val);
			}
			int cmp = ((thisLen != divisorLen) ? ((thisLen > divisorLen) ? 1 : -1)
				: Elementary.compareArrays(digits, divisor.digits, thisLen));
			if (cmp == EQUALS) {
				return ((thisSign == divisorSign) ? One : MinusOne);
			}
			if (cmp == LESS) {
				return Zero;
			}
			int resLength = thisLen - divisorLen + 1;
			int[] resDigits = new int[resLength];
			int resSign = ((thisSign == divisorSign) ? 1 : -1);
			if (divisorLen == 1) {
				Division.DivideArrayByInt(resDigits, digits, thisLen,
					divisor.digits[0]);
			} else {
				Division.Divide(resDigits, resLength, digits, thisLen,
					divisor.digits, divisorLen);
			}
			BigInteger result = new BigInteger(resSign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
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
		public BigInteger Remainder(BigInteger divisor) {
			if (divisor.sign == 0) {
				// math.17=BigInteger divide by zero
				throw new ArithmeticException(Messages.math17); //$NON-NLS-1$
			}
			int thisLen = numberLength;
			int divisorLen = divisor.numberLength;
			if (((thisLen != divisorLen) ? ((thisLen > divisorLen) ? 1 : -1)
				    : Elementary.compareArrays(digits, divisor.digits, thisLen)) == LESS) {
				return this;
			}
			int resLength = divisorLen;
			int[] resDigits = new int[resLength];
			if (resLength == 1) {
				resDigits[0] = Division.RemainderArrayByInt(digits, thisLen,
					divisor.digits[0]);
			} else {
				int qLen = thisLen - divisorLen + 1;
				resDigits = Division.Divide(null, qLen, digits, thisLen,
					divisor.digits, divisorLen);
			}
			BigInteger result = new BigInteger(sign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
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
		public BigInteger ModInverse(BigInteger m) {
			if (m.sign <= 0) {
				// math.18=BigInteger: modulus not positive
				throw new ArithmeticException(Messages.math18); //$NON-NLS-1$
			}
			// If both are even, no inverse exists
			if (!(TestBit(0) || m.TestBit(0))) {
				// math.19=BigInteger not invertible.
				throw new ArithmeticException(Messages.math19); //$NON-NLS-1$
			}
			if (m.IsOne) {
				return Zero;
			}

			// From now on: (m > 1)
			BigInteger res = Division.ModInverseMontgomery(Abs().Mod(m), m);
			if (res.sign == 0) {
				// math.19=BigInteger not invertible.
				throw new ArithmeticException(Messages.math19); //$NON-NLS-1$
			}

			res = ((sign < 0) ? m - res : res);
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
		public BigInteger ModPow(BigInteger exponent, BigInteger m) {
			if (m.sign <= 0) {
				// math.18=BigInteger: modulus not positive
				throw new ArithmeticException(Messages.math18); //$NON-NLS-1$
			}
			BigInteger b = this;

			if (m.IsOne | (exponent.sign > 0 & b.sign == 0)) {
				return BigInteger.Zero;
			}
			if (b.sign == 0 && exponent.sign == 0) {
				return BigInteger.One;
			}
			if (exponent.sign < 0) {
				b = ModInverse(m);
				exponent = exponent.Negate();
			}
			// From now on: (m > 0) and (exponent >= 0)
			BigInteger res = (m.TestBit(0)) ? Division.OddModPow(b.Abs(),
				exponent, m) : Division.EvenModPow(b.Abs(), exponent, m);
			if ((b.sign < 0) && exponent.TestBit(0)) {
				// -b^e mod m == ((-1 mod m) * (b^e mod m)) mod m
				res = (m - BigInteger.One).Multiply(res).Mod(m);
			}
			// else exponent is even, so base^exp is positive
			return res;
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
		public BigInteger Mod(BigInteger m) {
			if (m.sign <= 0) {
				// math.18=BigInteger: modulus not positive
				throw new ArithmeticException(Messages.math18); //$NON-NLS-1$
			}
			BigInteger rem = Remainder(m);
			return ((rem.sign < 0) ? rem + m : rem);
		}

		public static BigInteger operator +(BigInteger a, BigInteger b) {
			if (a == null)
				throw new InvalidOperationException();
			return BigMath.Add(a, b);
		}

		public static BigInteger operator -(BigInteger a, BigInteger b) {
			if (a == null)
				throw new InvalidOperationException();

			return BigMath.Subtract(a, b);
		}

		public static BigInteger operator *(BigInteger a, BigInteger b) {
			return a.Multiply(b);
		}

		public static BigInteger operator /(BigInteger a, BigInteger b) {
			return a.Divide(b);
		}

		public static BigInteger operator %(BigInteger a, BigInteger b) {
			return a.Mod(b);
		}

		public static BigInteger operator &(BigInteger a, BigInteger b) {
			return a.And(b);
		}

		public static BigInteger operator |(BigInteger a, BigInteger b) {
			return a.Or(b);
		}

		public static BigInteger operator ^(BigInteger a, BigInteger b) {
			return a.XOr(b);
		}

		public static BigInteger operator ~(BigInteger a) {
			return a.Not();
		}

		public static BigInteger operator -(BigInteger a) {
			return a.Negate();
		}

		public static BigInteger operator >>(BigInteger a, int b) {
			return a.ShiftRight(b);
		}

		public static BigInteger operator <<(BigInteger a, int b) {
			return a.ShiftLeft(b);
		}

		public static bool operator >(BigInteger a, BigInteger b) {
			return a.CompareTo(b) > 0;
		}

		public static bool operator <(BigInteger a, BigInteger b) {
			return a.CompareTo(b) < 0;
		}

		public static bool operator ==(BigInteger a, BigInteger b) {
			if ((object)a == null && (object)b == null)
				return true;
			if ((object)a == null)
				return false;
			return a.Equals(b);
		}

		public static bool operator !=(BigInteger a, BigInteger b) {
			return !(a == b);
		}

		public static bool operator >=(BigInteger a, BigInteger b) {
			return a == b || a > b;
		}

		public static bool operator <=(BigInteger a, BigInteger b) {
			return a == b || a < b;
		}

		#region Implicit Operators

		public static implicit operator Int32(BigInteger i) {
			return i.ToInt32();
		}

		public static implicit operator Int64(BigInteger i) {
			return i.ToInt64();
		}

		public static implicit operator Single(BigInteger i) {
			return i.ToSingle();
		}

		public static implicit operator Double(BigInteger i) {
			return i.ToDouble();
		}

		public static implicit operator String(BigInteger i) {
			return i.ToString();
		}

		public static implicit operator BigInteger(int value) {
			return FromInt64(value);
		}

		public static implicit operator BigInteger(long value) {
			return FromInt64(value);
		}

		#endregion
	}
}
