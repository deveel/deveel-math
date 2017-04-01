using System;

namespace Deveel.Math {
	public sealed partial class BigInteger
#if !PORTABLE
		: IConvertible
#endif 
		{
#if !PORTABLE

		TypeCode IConvertible.GetTypeCode() {
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider) {
			throw new NotImplementedException();
		}

		char IConvertible.ToChar(IFormatProvider provider) {
			throw new NotSupportedException();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider) {
			throw new NotSupportedException();
		}

		byte IConvertible.ToByte(IFormatProvider provider) {
			int value = ToInt32();
			if (value > Byte.MaxValue || value < Byte.MinValue)
				throw new InvalidCastException();
			return (byte) value;
		}

		short IConvertible.ToInt16(IFormatProvider provider) {
			int value = ToInt32();
			if (value > Int16.MaxValue || value < Int16.MinValue)
				throw new InvalidCastException();
			return (short) value;
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
			throw new NotImplementedException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider) {
			throw new NotSupportedException();
		}

		string IConvertible.ToString(IFormatProvider provider) {
			return ToString();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
			if (conversionType == typeof(byte))
				return (this as IConvertible).ToByte(provider);
			if (conversionType == typeof(short))
				return (this as IConvertible).ToInt16(provider);
			if (conversionType == typeof(int))
				return ToInt32();
			if (conversionType == typeof(long))
				return ToInt64();
			if (conversionType == typeof(float))
				return ToSingle();
			if (conversionType == typeof(double))
				return ToDouble();
			if (conversionType == typeof(string))
				return ToString();
			if (conversionType == typeof(byte[]))
				return ToByteArray();

			throw new NotSupportedException();
		}

#endif

		/**
 * Returns the two's complement representation of this BigInteger in a byte
 * array.
 *
 * @return two's complement representation of {@code this}.
 */
		public byte[] ToByteArray() {
			if (sign == 0) {
				return new byte[] {0};
			}
			BigInteger temp = this;
			int bitLen = BitLength;
			int iThis = FirstNonzeroDigit;
			int bytesLen = (bitLen >> 3) + 1;
			/*
			 * Puts the little-endian int array representing the magnitude of this
			 * BigInteger into the big-endian byte array.
			 */
			byte[] bytes = new byte[bytesLen];
			int firstByteNumber = 0;
			int highBytes;
			int digitIndex = 0;
			int bytesInInteger = 4;
			int digit;
			int hB;

			if (bytesLen - (numberLength << 2) == 1) {
				bytes[0] = (byte) ((sign < 0) ? -1 : 0);
				highBytes = 4;
				firstByteNumber++;
			} else {
				hB = bytesLen & 3;
				highBytes = (hB == 0) ? 4 : hB;
			}

			digitIndex = iThis;
			bytesLen -= iThis << 2;

			if (sign < 0) {
				digit = -temp.digits[digitIndex];
				digitIndex++;
				if (digitIndex == numberLength) {
					bytesInInteger = highBytes;
				}
				for (int i = 0; i < bytesInInteger; i++, digit >>= 8) {
					bytes[--bytesLen] = (byte) digit;
				}
				while (bytesLen > firstByteNumber) {
					digit = ~temp.digits[digitIndex];
					digitIndex++;
					if (digitIndex == numberLength) {
						bytesInInteger = highBytes;
					}
					for (int i = 0; i < bytesInInteger; i++, digit >>= 8) {
						bytes[--bytesLen] = (byte) digit;
					}
				}
			} else {
				while (bytesLen > firstByteNumber) {
					digit = temp.digits[digitIndex];
					digitIndex++;
					if (digitIndex == numberLength) {
						bytesInInteger = highBytes;
					}
					for (int i = 0; i < bytesInInteger; i++, digit >>= 8) {
						bytes[--bytesLen] = (byte) digit;
					}
				}
			}
			return bytes;
		}

		public int ToInt32() {
			return (sign * digits[0]);
		}

		/**
		 * Returns this {@code BigInteger} as an long value. If {@code this} is too
		 * big to be represented as an long, then {@code this} % 2^64 is returned.
		 *
		 * @return this {@code BigInteger} as a long value.
		 */
		public long ToInt64() {
			long value = (numberLength > 1) ? (((long) digits[1]) << 32) | (digits[0] & 0xFFFFFFFFL) : (digits[0] & 0xFFFFFFFFL);
			return (sign * value);
		}

		/**
		 * Returns this {@code BigInteger} as an float value. If {@code this} is too
		 * big to be represented as an float, then {@code Float.POSITIVE_INFINITY}
		 * or {@code Float.NEGATIVE_INFINITY} is returned. Note, that not all
		 * integers x in the range [-Float.MAX_VALUE, Float.MAX_VALUE] can be
		 * represented as a float. The float representation has a mantissa of length
		 * 24. For example, 2^24+1 = 16777217 is returned as float 16777216.0.
		 *
		 * @return this {@code BigInteger} as a float value.
		 */
		public float ToSingle() {
			return (float) ToDouble();
		}

		/**
		 * Returns this {@code BigInteger} as an double value. If {@code this} is
		 * too big to be represented as an double, then {@code
		 * Double.POSITIVE_INFINITY} or {@code Double.NEGATIVE_INFINITY} is
		 * returned. Note, that not all integers x in the range [-Double.MAX_VALUE,
		 * Double.MAX_VALUE] can be represented as a double. The double
		 * representation has a mantissa of length 53. For example, 2^53+1 =
		 * 9007199254740993 is returned as double 9007199254740992.0.
		 *
		 * @return this {@code BigInteger} as a double value
		 */
		public double ToDouble() {
			return Conversion.BigInteger2Double(this);
		}

		public override String ToString() {
			return Conversion.ToDecimalScaledString(this, 0);
		}

		/**
		 * Returns a string containing a string representation of this {@code
		 * BigInteger} with base radix. If {@code radix < CharHelper.MIN_RADIX} or
		 * {@code radix > CharHelper.MAX_RADIX} then a decimal representation is
		 * returned. The CharHelpers of the string representation are generated with
		 * method {@code CharHelper.forDigit}.
		 *
		 * @param radix
		 *            base to be used for the string representation.
		 * @return a string representation of this with radix 10.
		 */
		public String ToString(int radix) {
			return Conversion.BigInteger2String(this, radix);
		}
	}
}
