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
using System.Globalization;
using System.Text;

namespace Deveel.Math {
	public sealed partial class BigDecimal
#if !PORTABLE
		: IConvertible
#endif 
		{

#if !PORTABLE
		TypeCode IConvertible.GetTypeCode() {
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider) {
			int value = ToInt32();
			if (value == 1)
				return true;
			if (value == 0)
				return false;
			throw new InvalidCastException();
		}

		char IConvertible.ToChar(IFormatProvider provider) {
			short value = ToInt16Exact();
			return (char)value;
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider) {
			throw new NotSupportedException();
		}

		byte IConvertible.ToByte(IFormatProvider provider) {
			int value = ToInt32();
			if (value > Byte.MaxValue || value < Byte.MinValue)
				throw new InvalidCastException();
			return (byte)value;
		}

		short IConvertible.ToInt16(IFormatProvider provider) {
			return ToInt16Exact();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider) {
			throw new NotSupportedException();
		}

		int IConvertible.ToInt32(IFormatProvider provider) {
			return ToInt32();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider) {
			throw new NotSupportedException();
		}

		long IConvertible.ToInt64(IFormatProvider provider) {
			return ToInt64();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider) {
			throw new NotSupportedException();
		}

		float IConvertible.ToSingle(IFormatProvider provider) {
			return ToSingle();
		}

		double IConvertible.ToDouble(IFormatProvider provider) {
			return ToDouble();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider) {
			return ToDecimal();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider) {
			throw new NotSupportedException();
		}

		string IConvertible.ToString(IFormatProvider provider) {
			return ToString(provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
			if (conversionType == typeof(BigInteger))
				return ToBigInteger();

			throw new NotSupportedException();
		}

#endif

		public override string ToString() {
			if (toStringImage != null) {
				return toStringImage;
			}

			return ToString(null);
		}

		public string ToString(IFormatProvider provider) {
			if (provider == null)
				provider = NumberFormatInfo.InvariantInfo;

			return DecimalString.ToString(this, provider);
		}


		/// <summary>
		/// Returns a string representation of this number,
		/// including all significant digits of this value
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the scale is negative or if <c>scale - precision >= 6</c> 
		/// then engineering notation is used. Engineering notation is 
		/// similar to the scientific notation except that the exponent 
		/// is made to be a multiple of 3 such that the integer part 
		/// is &gt= 1 and &lt 1000.
		/// </para>
		/// <para>
		/// This overload uses the invariant culture to resolve the
		/// format information for the string.
		/// </para>
		/// </remarks>
		/// <returns>
		/// Returns a string representation of this number in engineering 
		/// notation if necessary.
		/// </returns>
		public String ToEngineeringString() {
			return ToEngineeringString(null);
		}

		/// <summary>
		/// Returns a string representation of this number,
		/// including all significant digits of this value
		/// </summary>
		/// <param name="provider">The provider used to resolve the
		/// format information to use.</param>
		/// <remarks>
		/// <para>
		/// If the scale is negative or if <c>scale - precision >= 6</c> 
		/// then engineering notation is used. Engineering notation is 
		/// similar to the scientific notation except that the exponent 
		/// is made to be a multiple of 3 such that the integer part 
		/// is &gt= 1 and &lt 1000.
		/// </para>
		/// </remarks>
		/// <returns>
		/// Returns a string representation of this number in engineering 
		/// notation if necessary.
		/// </returns>
		public String ToEngineeringString(IFormatProvider provider) {
			return DecimalString.ToEngineeringString(this, provider);
		}

		/**
		 * Returns a string representation of this {@code BigDecimal}. No scientific
		 * notation is used. This methods adds zeros where necessary.
		 * <p>
		 * If this string representation is used to create a new instance, this
		 * instance is generally not identical to {@code this} as the precision
		 * changes.
		 * <p>
		 * {@code x.equals(new BigDecimal(x.toPlainString())} usually returns
		 * {@code false}.
		 * <p>
		 * {@code x.compareTo(new BigDecimal(x.toPlainString())} returns {@code 0}.
		 *
		 * @return a string representation of {@code this} without exponent part.
		 */

		public String ToPlainString(IFormatProvider provider) {
			if (provider == null)
				provider = CultureInfo.InvariantCulture;

			return DecimalString.ToPlainString(this, provider);
		}

		public String ToPlainString() {
			return ToPlainString(null);
		}

		/**
 * Returns this {@code BigDecimal} as a big integer instance. A fractional
 * part is discarded.
 *
 * @return this {@code BigDecimal} as a big integer instance.
 */

		public BigInteger ToBigInteger() {
			if ((_scale == 0) || (IsZero)) {
				return GetUnscaledValue();
			} else if (_scale < 0) {
				return GetUnscaledValue() * Multiplication.PowerOf10(-(long) _scale);
			} else {
				// (scale > 0)
				return GetUnscaledValue() / Multiplication.PowerOf10(_scale);
			}
		}

		/**
		 * Returns this {@code BigDecimal} as a big integer instance if it has no
		 * fractional part. If this {@code BigDecimal} has a fractional part, i.e.
		 * if rounding would be necessary, an {@code ArithmeticException} is thrown.
		 *
		 * @return this {@code BigDecimal} as a big integer value.
		 * @throws ArithmeticException
		 *             if rounding is necessary.
		 */

		public BigInteger ToBigIntegerExact() {
			if ((_scale == 0) || (IsZero)) {
				return GetUnscaledValue();
			} else if (_scale < 0) {
				return GetUnscaledValue() * Multiplication.PowerOf10(-(long) _scale);
			} else {
				// (scale > 0)
				BigInteger integer;
				BigInteger fraction;

				// An optimization before do a heavy division
				if ((_scale > AproxPrecision()) || (_scale > GetUnscaledValue().LowestSetBit)) {
					// math.08=Rounding necessary
					throw new ArithmeticException(Messages.math08); //$NON-NLS-1$
				}

				integer = BigMath.DivideAndRemainder(GetUnscaledValue(), Multiplication.PowerOf10(_scale), out fraction);
				if (fraction.Sign != 0) {
					// It exists a non-zero fractional part 
					// math.08=Rounding necessary
					throw new ArithmeticException(Messages.math08); //$NON-NLS-1$
				}

				return integer;
			}
		}

		/**
		 * Returns this {@code BigDecimal} as an long value. Any fractional part is
		 * discarded. If the integral part of {@code this} is too big to be
		 * represented as an long, then {@code this} % 2^64 is returned.
		 *
		 * @return this {@code BigDecimal} as a long value.
		 */

		public long ToInt64() {
			/* If scale <= -64 there are at least 64 trailing bits zero in 10^(-scale).
			 * If the scale is positive and very large the long value could be zero. */
			return ((_scale <= -64) || (_scale > AproxPrecision()) ? 0L : ToBigInteger().ToInt64());
		}

		/**
		 * Returns this {@code BigDecimal} as a long value if it has no fractional
		 * part and if its value fits to the int range ([-2^{63}..2^{63}-1]). If
		 * these conditions are not met, an {@code ArithmeticException} is thrown.
		 *
		 * @return this {@code BigDecimal} as a long value.
		 * @throws ArithmeticException
		 *             if rounding is necessary or the number doesn't fit in a long.
		 */

		public long ToInt64Exact() {
			return ValueExact(64);
		}

		/**
		 * Returns this {@code BigDecimal} as an int value. Any fractional part is
		 * discarded. If the integral part of {@code this} is too big to be
		 * represented as an int, then {@code this} % 2^32 is returned.
		 *
		 * @return this {@code BigDecimal} as a int value.
		 */

		public int ToInt32() {
			/* If scale <= -32 there are at least 32 trailing bits zero in 10^(-scale).
			 * If the scale is positive and very large the long value could be zero. */
			return ((_scale <= -32) || (_scale > AproxPrecision()) ? 0 : ToBigInteger().ToInt32());
		}

		/**
		 * Returns this {@code BigDecimal} as a int value if it has no fractional
		 * part and if its value fits to the int range ([-2^{31}..2^{31}-1]). If
		 * these conditions are not met, an {@code ArithmeticException} is thrown.
		 *
		 * @return this {@code BigDecimal} as a int value.
		 * @throws ArithmeticException
		 *             if rounding is necessary or the number doesn't fit in a int.
		 */

		public int ToInt32Exact() {
			return (int) ValueExact(32);
		}

		/**
		 * Returns this {@code BigDecimal} as a short value if it has no fractional
		 * part and if its value fits to the short range ([-2^{15}..2^{15}-1]). If
		 * these conditions are not met, an {@code ArithmeticException} is thrown.
		 *
		 * @return this {@code BigDecimal} as a short value.
		 * @throws ArithmeticException
		 *             if rounding is necessary of the number doesn't fit in a
		 *             short.
		 */

		public short ToInt16Exact() {
			return (short) ValueExact(16);
		}

		/**
		 * Returns this {@code BigDecimal} as a byte value if it has no fractional
		 * part and if its value fits to the byte range ([-128..127]). If these
		 * conditions are not met, an {@code ArithmeticException} is thrown.
		 *
		 * @return this {@code BigDecimal} as a byte value.
		 * @throws ArithmeticException
		 *             if rounding is necessary or the number doesn't fit in a byte.
		 */

		public byte ToByteExact() {
			return (byte) ValueExact(8);
		}

		/**
		 * Returns this {@code BigDecimal} as a float value. If {@code this} is too
		 * big to be represented as an float, then {@code Float.POSITIVE_INFINITY}
		 * or {@code Float.NEGATIVE_INFINITY} is returned.
		 * <p>
		 * Note, that if the unscaled value has more than 24 significant digits,
		 * then this decimal cannot be represented exactly in a float variable. In
		 * this case the result is rounded.
		 * <p>
		 * For example, if the instance {@code x1 = new BigDecimal("0.1")} cannot be
		 * represented exactly as a float, and thus {@code x1.equals(new
		 * BigDecimal(x1.folatValue())} returns {@code false} for this case.
		 * <p>
		 * Similarly, if the instance {@code new BigDecimal(16777217)} is converted
		 * to a float, the result is {@code 1.6777216E}7.
		 *
		 * @return this {@code BigDecimal} as a float value.
		 */

		public float ToSingle() {
			/* A similar code like in ToDouble() could be repeated here,
			 * but this simple implementation is quite efficient. */
			float floatResult = Sign;
			long powerOfTwo = this._bitLength - (long) (_scale / Log10Of2);
			if ((powerOfTwo < -149) || (floatResult == 0.0f)) {
				// Cases which 'this' is very small
				floatResult *= 0.0f;
			} else if (powerOfTwo > 129) {
				// Cases which 'this' is very large
				floatResult *= Single.PositiveInfinity;
			} else {
				floatResult = (float) ToDouble();
			}
			return floatResult;
		}

		/**
		 * Returns this {@code BigDecimal} as a double value. If {@code this} is too
		 * big to be represented as an float, then {@code Double.POSITIVE_INFINITY}
		 * or {@code Double.NEGATIVE_INFINITY} is returned.
		 * <p>
		 * Note, that if the unscaled value has more than 53 significant digits,
		 * then this decimal cannot be represented exactly in a double variable. In
		 * this case the result is rounded.
		 * <p>
		 * For example, if the instance {@code x1 = new BigDecimal("0.1")} cannot be
		 * represented exactly as a double, and thus {@code x1.equals(new
		 * BigDecimal(x1.ToDouble())} returns {@code false} for this case.
		 * <p>
		 * Similarly, if the instance {@code new BigDecimal(9007199254740993L)} is
		 * converted to a double, the result is {@code 9.007199254740992E15}.
		 * <p>
		 *
		 * @return this {@code BigDecimal} as a double value.
		 */

		public double ToDouble() {
			int sign = Sign;
			int exponent = 1076; // bias + 53
			int lowestSetBit;
			int discardedSize;
			long powerOfTwo = this._bitLength - (long) (_scale / Log10Of2);
			long bits; // IEEE-754 Standard
			long tempBits; // for temporal calculations     
			BigInteger mantisa;

			if ((powerOfTwo < -1074) || (sign == 0)) {
				// Cases which 'this' is very small            
				return (sign * 0.0d);
			} else if (powerOfTwo > 1025) {
				// Cases which 'this' is very large            
				return (sign * Double.PositiveInfinity);
			}

			mantisa = BigMath.Abs(GetUnscaledValue());

			// Let be:  this = [u,s], with s > 0
			if (_scale <= 0) {
				// mantisa = abs(u) * 10^s
				mantisa = mantisa * Multiplication.PowerOf10(-_scale);
			} else {
				// (scale > 0)
				BigInteger quotient;
				BigInteger remainder;
				BigInteger powerOfTen = Multiplication.PowerOf10(_scale);
				int k = 100 - (int) powerOfTwo;
				int compRem;

				if (k > 0) {
					/* Computing (mantisa * 2^k) , where 'k' is a enough big
					 * power of '2' to can divide by 10^s */
					mantisa = mantisa << k;
					exponent -= k;
				}

				// Computing (mantisa * 2^k) / 10^s
				quotient = BigMath.DivideAndRemainder(mantisa, powerOfTen, out remainder);

				// To check if the fractional part >= 0.5
				compRem = remainder.ShiftLeftOneBit().CompareTo(powerOfTen);

				// To add two rounded bits at end of mantisa
				mantisa = (quotient << 2) + BigInteger.FromInt64((compRem * (compRem + 3)) / 2 + 1);
				exponent -= 2;
			}
			lowestSetBit = mantisa.LowestSetBit;
			discardedSize = mantisa.BitLength - 54;
			if (discardedSize > 0) {
				// (n > 54)
				// mantisa = (abs(u) * 10^s) >> (n - 54)
				bits = (mantisa >> discardedSize).ToInt64();
				tempBits = bits;

				// #bits = 54, to check if the discarded fraction produces a carry             
				if ((((bits & 1) == 1) && (lowestSetBit < discardedSize))
				    || ((bits & 3) == 3)) {
					bits += 2;
				}
			} else {
				// (n <= 54)
				// mantisa = (abs(u) * 10^s) << (54 - n)                
				bits = mantisa.ToInt64() << -discardedSize;
				tempBits = bits;

				// #bits = 54, to check if the discarded fraction produces a carry:
				if ((bits & 3) == 3) {
					bits += 2;
				}
			}

			// Testing bit 54 to check if the carry creates a new binary digit
			if ((bits & 0x40000000000000L) == 0) {
				// To drop the last bit of mantisa (first discarded)
				bits >>= 1;

				// exponent = 2^(s-n+53+bias)
				exponent += discardedSize;
			} else {
				// #bits = 54
				bits >>= 2;
				exponent += discardedSize + 1;
			}

			// To test if the 53-bits number fits in 'double'            
			if (exponent > 2046) {
				// (exponent - bias > 1023)
				return (sign * Double.PositiveInfinity);
			}

			if (exponent <= 0) {
				// (exponent - bias <= -1023)
				// Denormalized numbers (having exponent == 0)
				if (exponent < -53) {
					// exponent - bias < -1076
					return (sign * 0.0d);
				}

				// -1076 <= exponent - bias <= -1023 
				// To discard '- exponent + 1' bits
				bits = tempBits >> 1;
				tempBits = bits & Utils.URShift(-1L, (63 + exponent));
				bits >>= (-exponent);

				// To test if after discard bits, a new carry is generated
				if (((bits & 3) == 3) || (((bits & 1) == 1) && (tempBits != 0)
				                          && (lowestSetBit < discardedSize))) {
					bits += 1;
				}
				exponent = 0;
				bits >>= 1;
			}

			// Construct the 64 double bits: [sign(1), exponent(11), mantisa(52)]
			// bits = (long)((ulong)sign & 0x8000000000000000L) | ((long)exponent << 52) | (bits & 0xFFFFFFFFFFFFFL);
			bits = sign & Int64.MinValue | ((long) exponent << 52) | (bits & 0xFFFFFFFFFFFFFL);
			return BitConverter.Int64BitsToDouble(bits);
		}

		// TODO: must be verified
		public decimal ToDecimal() {
			var scaleDivisor = BigMath.Pow(BigInteger.FromInt64(10), _scale);
			var remainder = BigMath.Remainder(GetUnscaledValue(), scaleDivisor);
			var scaledValue = GetUnscaledValue() / scaleDivisor;

			var leftOfDecimal = (decimal) scaledValue;
			var rightOfDecimal = (remainder) / ((decimal) scaleDivisor);

			return leftOfDecimal + rightOfDecimal;
		}
	}
}