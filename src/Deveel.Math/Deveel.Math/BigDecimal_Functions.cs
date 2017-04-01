using System;

namespace Deveel.Math {
	public sealed partial class BigDecimal {
		/**
 * Returns a new {@code BigDecimal} whose value is the absolute value of
 * {@code this}. The scale of the result is the same as the scale of this.
 *
 * @return {@code abs(this)}
 */

		public BigDecimal Abs() {
			return ((Sign < 0) ? Negate() : this);
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

		public BigDecimal Abs(MathContext mc) {
			return Round(mc).Abs();
		}

		/**
		 * Returns a new {@code BigDecimal} whose value is the {@code -this}. The
		 * scale of the result is the same as the scale of this.
		 *
		 * @return {@code -this}
		 */

		public BigDecimal Negate() {
			if (_bitLength < 63 || (_bitLength == 63 && smallValue != Int64.MinValue)) {
				return ValueOf(-smallValue, _scale);
			}
			return new BigDecimal(-GetUnscaledValue(), _scale);
		}

		/**
		 * Returns a new {@code BigDecimal} whose value is the {@code -this}. The
		 * result is rounded according to the passed context {@code mc}.
		 *
		 * @param mc
		 *            rounding mode and precision for the result of this operation.
		 * @return {@code -this}
		 */

		public BigDecimal Negate(MathContext mc) {
			return Round(mc).Negate();
		}

		/**
		 * Returns a new {@code BigDecimal} whose value is {@code +this}. The scale
		 * of the result is the same as the scale of this.
		 *
		 * @return {@code this}
		 */

		public BigDecimal Plus() {
			return this;
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
		public BigDecimal Plus(MathContext mc) {
			return Round(mc);
		}

		public BigDecimal Round(MathContext mc) {
			var thisBD = new BigDecimal(GetUnscaledValue(), _scale);

			thisBD.InplaceRound(mc);
			return thisBD;
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

		public BigDecimal SetScale(int newScale, RoundingMode roundingMode) {
			long diffScale = newScale - (long)_scale;
			// Let be:  'this' = [u,s]        
			if (diffScale == 0) {
				return this;
			}
			if (diffScale > 0) {
				// return  [u * 10^(s2 - s), newScale]
				if (diffScale < LongTenPow.Length &&
				    (_bitLength + LongTenPowBitLength[(int)diffScale]) < 64) {
					return ValueOf(smallValue * LongTenPow[(int)diffScale], newScale);
				}
				return new BigDecimal(Multiplication.MultiplyByTenPow(GetUnscaledValue(), (int)diffScale), newScale);
			}
			// diffScale < 0
			// return  [u,s] / [1,newScale]  with the appropriate scale and rounding
			if (_bitLength < 64 && -diffScale < LongTenPow.Length) {
				return DividePrimitiveLongs(smallValue, LongTenPow[(int)-diffScale], newScale, roundingMode);
			}
			return DivideBigIntegers(GetUnscaledValue(), Multiplication.PowerOf10(-diffScale), newScale, roundingMode);
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
		 * @throws IllegalArgumentException
		 *             if {@code roundingMode} is not a valid rounding mode.
		 * @throws ArithmeticException
		 *             if {@code roundingMode == ROUND_UNNECESSARY} and rounding is
		 *             necessary according to the given scale.
		 */

		public BigDecimal SetScale(int newScale, int roundingMode) {
			RoundingMode rm = (RoundingMode)roundingMode;
			if ((roundingMode < (int)RoundingMode.Up) ||
			    (roundingMode > (int)RoundingMode.Unnecessary)) {
				throw new ArgumentException("roundingMode");
			}
			return SetScale(newScale, (RoundingMode)roundingMode);
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

		public BigDecimal SetScale(int newScale) {
			return SetScale(newScale, RoundingMode.Unnecessary);
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

		public BigDecimal MovePointLeft(int n) {
			return MovePoint(_scale + (long)n);
		}

		private BigDecimal MovePoint(long newScale) {
			if (IsZero) {
				return GetZeroScaledBy(System.Math.Max(newScale, 0));
			}
			/* When:  'n'== Integer.MIN_VALUE  isn't possible to call to movePointRight(-n)  
			 * since  -Integer.MIN_VALUE == Integer.MIN_VALUE */
			if (newScale >= 0) {
				if (_bitLength < 64) {
					return ValueOf(smallValue, ToIntScale(newScale));
				}
				return new BigDecimal(GetUnscaledValue(), ToIntScale(newScale));
			}
			if (-newScale < LongTenPow.Length &&
			    _bitLength + LongTenPowBitLength[(int)-newScale] < 64) {
				return ValueOf(smallValue * LongTenPow[(int)-newScale], 0);
			}
			return new BigDecimal(Multiplication.MultiplyByTenPow(GetUnscaledValue(), (int)-newScale), 0);
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

		public BigDecimal MovePointRight(int n) {
			return MovePoint(_scale - (long)n);
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

		public BigDecimal ScaleByPowerOfTen(int n) {
			long newScale = _scale - (long)n;
			if (_bitLength < 64) {
				//Taking care when a 0 is to be scaled
				if (smallValue == 0) {
					return GetZeroScaledBy(newScale);
				}
				return ValueOf(smallValue, ToIntScale(newScale));
			}
			return new BigDecimal(GetUnscaledValue(), ToIntScale(newScale));
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

		public BigDecimal StripTrailingZeros() {
			int i = 1; // 1 <= i <= 18
			int lastPow = TenPow.Length - 1;
			long newScale = _scale;

			if (IsZero) {
				return BigDecimal.Parse("0");
			}
			BigInteger strippedBI = GetUnscaledValue();
			BigInteger quotient;
			BigInteger remainder;

			// while the number is even...
			while (!strippedBI.TestBit(0)) {
				// To divide by 10^i
				quotient = BigMath.DivideAndRemainder(strippedBI, TenPow[i], out remainder);
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
			return new BigDecimal(strippedBI, ToIntScale(newScale));
		}

		public BigDecimal Min(BigDecimal val) {
			return ((CompareTo(val) <= 0) ? this : val);
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

		public BigDecimal Max(BigDecimal val) {
			return ((CompareTo(val) >= 0) ? this : val);
		}
	}
}
