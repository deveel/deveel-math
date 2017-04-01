// 
//  Copyright 2009-2014  Deveel
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
using System.IO;
#if !PORTABLE
using System.Runtime.Serialization;
#endif

namespace Deveel.Math {
	/// <summary>
	/// This class represents immutable integer numbers of arbitrary length.
	/// </summary>
	/// <remarks>
	/// Large numbers are typically used in security applications and therefore 
	/// BigIntegers offer dedicated functionality like the generation of large 
	/// prime numbers or the computation of modular inverse.
	/// <para>
	/// Since the class was modeled to offer all the functionality as the 
	/// {@link Integer} class does, it provides even methods that operate bitwise 
	/// on a two's complement representation of large integers. Note however that 
	/// the implementations favors an internal representation where magnitude and 
	/// sign are treated separately. Hence such operations are inefficient and 
	/// should be discouraged. In simple words: Do NOT implement any bit fields 
	/// based on BigInteger.
	/// </para>
	/// </remarks>
#if !PORTABLE
	[Serializable]
#endif
	public sealed partial class BigInteger : IComparable<BigInteger>, IEquatable<BigInteger>
#if !PORTABLE
		, ISerializable
#endif
	{

		/* Fields used for the internal representation. */

		/**
		 * The magnitude of this big integer. This array holds unsigned little
		 * endian digits. For example:
		 *   {@code 13} is represented as [ 13 ]
		 *   {@code -13} is represented as [ 13 ]
		 *   {@code 2^32 + 13} is represented as [ 13, 1 ]
		 *   {@code 2^64 + 13} is represented as [ 13, 0, 1 ]
		 *   {@code 2^31} is represented as [ Integer.MIN_VALUE ]
		 * The magnitude array may be longer than strictly necessary, which results
		 * in additional trailing zeros.
		 */
#if !PORTABLE
		[NonSerialized] 
#endif
		internal int[] digits;

		/** The length of this in measured in ints. Can be less than digits.length(). */
#if !PORTABLE
		[NonSerialized]
#endif
		internal int numberLength;

		/** The sign of this. */
#if !PORTABLE
		[NonSerialized] 
#endif
		private int sign;


		/// <summary>
		/// The constant value <c>0</c> as <see cref="BigInteger"/>
		/// </summary>
		public static readonly BigInteger Zero = new BigInteger(0, 0);

		/// <summary>
		/// The constant value <c>1</c> as <see cref="BigInteger"/>
		/// </summary>
		public static readonly BigInteger One = new BigInteger(1, 1);

		/// <summary>
		/// The constant value <c>10</c> as <see cref="BigInteger"/>
		/// </summary>
		public static readonly BigInteger Ten = new BigInteger(1, 10);

		/// <summary>
		/// The constant value <c>-1</c> as <see cref="BigInteger"/>
		/// </summary>
		internal static readonly BigInteger MinusOne = new BigInteger(-1, 1);

		/** The {@code BigInteger} constant 0 used for comparison. */
		internal static readonly int EQUALS = 0;

		/** The {@code BigInteger} constant 1 used for comparison. */
		internal static readonly int GREATER = 1;

		/** The {@code BigInteger} constant -1 used for comparison. */
		internal static readonly int LESS = -1;

		/** All the {@code BigInteger} numbers in the range [0,10] are cached. */
		static readonly BigInteger[] SmallValues = { Zero, One, new BigInteger(1, 2),
            new BigInteger(1, 3), new BigInteger(1, 4), new BigInteger(1, 5),
            new BigInteger(1, 6), new BigInteger(1, 7), new BigInteger(1, 8),
            new BigInteger(1, 9), Ten };

		static readonly BigInteger[] TwoPows;

		static BigInteger() {
			TwoPows = new BigInteger[32];
			for (int i = 0; i < TwoPows.Length; i++) {
				TwoPows[i] = BigInteger.FromInt64(1L << i);
			}
		}

		private BigInteger() {
		}

#if !PORTABLE
		[NonSerialized]
#endif
		private int firstNonzeroDigit = -2;

		/** Cache for the hash code. */
#if !PORTABLE
		[NonSerialized]
#endif
		private int hashCode = 0;

#if !PORTABLE
		#region Serializable

		private BigInteger(SerializationInfo info, StreamingContext context) {
			sign = info.GetInt32("sign");
			byte[] magn = (byte[]) info.GetValue("magnitude", typeof(byte[]));
			PutBytesPositiveToIntegers(magn);
			CutOffLeadingZeroes();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("sign", sign);
			byte[] magn = Abs().ToByteArray();
			info.AddValue("magnitude", magn, typeof(byte[]));
		}

		#endregion
#endif

		#region .ctor

		/// <summary>
		/// Constructs a random non-negative big integer instance in the range [0, 2^(numBits)-1]
		/// </summary>
		/// <param name="numBits">The maximum length of the new <see cref="BigInteger"/> in bits.</param>
		/// <param name="rnd">An optional random number generator to be used.</param>
		/// <exception cref="ArgumentException">
		/// If the given <paramref name="numBits"/> value is less than 0.
		/// </exception>
		public BigInteger(int numBits, Random rnd) {
			if (numBits < 0) {
				// math.1B=numBits must be non-negative
				throw new ArgumentException(Messages.math1B); //$NON-NLS-1$
			}
			if (numBits == 0) {
				sign = 0;
				numberLength = 1;
				digits = new int[] {0};
			} else {
				sign = 1;
				numberLength = (numBits + 31) >> 5;
				digits = new int[numberLength];
				for (int i = 0; i < numberLength; i++) {
					digits[i] = rnd.Next();
				}
				// Using only the necessary bits
				digits[numberLength - 1] = Utils.URShift(digits[numberLength - 1], (-numBits) & 31);
				CutOffLeadingZeroes();
			}
		}

		/// <summary>
		/// Constructs a random <see cref="BigInteger"/> instance in the 
		/// range [0, 2^(bitLength)-1] which is probably prime.
		/// </summary>
		/// <param name="bitLength">The length of the new big integer in bits.</param>
		/// <param name="certainty">The tolerated primality uncertainty.</param>
		/// <param name="random">An optional random generator to be used.</param>
		/// <remarks>
		/// The probability that the returned <see cref="BigInteger"/> is prime 
		/// is beyond(1-1/2^certainty).
		/// </remarks>
		/// <exception cref="ArithmeticException">
		/// If the given <paramref name="bitLength"/> is smaller than 2.
		/// </exception>
		public BigInteger(int bitLength, int certainty, Random random) {
			if (bitLength < 2) {
				// math.1C=bitLength < 2
				throw new ArithmeticException(Messages.math1C); //$NON-NLS-1$
			}
			BigInteger me = Primality.ConsBigInteger(bitLength, certainty, random);
			sign = me.sign;
			numberLength = me.numberLength;
			digits = me.digits;
		}

		/// <summary>
		/// Constructs a new {@code BigInteger} instance with the given sign and the given magnitude. 
		/// </summary>
		/// <param name="signum">The sign as an integer (-1 for negative, 0 for zero, 1 for positive).</param>
		/// <param name="magnitude">The byte array that describes the magnitude, where the most significant
		/// byte is the first.</param>
		/// <exception cref="ArgumentNullException">
		/// If the provided <paramref name="magnitude"/> provided is <c>null</c>.
		/// </exception>
		/// <exception cref="FormatException">
		/// If the provided <paramref name="signum"/> is different from -1, 0 or 1 or
		/// if the sign is 0 and the magnitude contains non-zero entries.
		/// </exception>
		public BigInteger(int signum, byte[] magnitude) {
			if (magnitude == null)
				throw new ArgumentNullException("magnitude");

			if ((signum < -1) || (signum > 1))
				// math.13=Invalid signum value
				throw new FormatException(Messages.math13); //$NON-NLS-1$

			if (signum == 0) {
				foreach (byte element in magnitude) {

					if (element != 0) {
						// math.14=signum-magnitude mismatch
						throw new FormatException(Messages.math14); //$NON-NLS-1$
					}
				}
			}

			if (magnitude.Length == 0) {
				sign = 0;
				numberLength = 1;
				digits = new int[] { 0 };
			} else {
				sign = signum;
				PutBytesPositiveToIntegers(magnitude);
				CutOffLeadingZeroes();
			}
		}

		/// <summary>
		/// Constructs a new <see cref="BigInteger"/> from the given two's 
		/// complement representation.
		/// </summary>
		/// <param name="val">The two's complement representation of the new big integer.</param>
		/// <remarks>
		/// The most significant byte is the entry at index 0. The most significant 
		/// bit of this entry determines the sign of the new <see cref="BigInteger"/> instance.
		/// The given array must not be empty.
		/// </remarks>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="val"/> 
		/// is <c>null</c></exception>
		/// <exception cref="FormatException">If the length of <paramref name="val"/> is zero</exception>
		public BigInteger(byte[] val) {
			if (val == null)
				throw new ArgumentNullException("val");
			if (val.Length == 0) {
				// math.12=Zero length BigInteger
				throw new FormatException(Messages.math12); //$NON-NLS-1$
			}
			if (val[0] > sbyte.MaxValue) {
				sign = -1;
				PutBytesNegativeToIntegers(val);
			} else {
				sign = 1;
				PutBytesPositiveToIntegers(val);
			}
			CutOffLeadingZeroes();
		}
		
		/// <summary>
		/// Constructs a number which array is of size 1.
		/// </summary>
		/// <param name="sign">The sign of the number</param>
		/// <param name="value">The only one digit of array</param>
		internal BigInteger(int sign, int value) {
			this.sign = sign;
			numberLength = 1;
			digits = new int[] { value };
		}

		/// <summary>
		/// Constructs a number without to create new space.
		/// </summary>
		/// <param name="sign">The sign of the number</param>
		/// <param name="numberLength">The length of the internal array</param>
		/// <param name="digits">A reference of some array created before</param>
		/// <remarks>
		/// This construct should be used only if the three fields of 
		/// representation are known.
		/// </remarks>
		internal BigInteger(int sign, int numberLength, int[] digits) {
			this.sign = sign;
			this.numberLength = numberLength;
			this.digits = digits;
		}

		/// <summary>
		/// Creates a new <see cref="BigInteger"/> whose value is equal to the 
		/// specified <see cref="long"/>.
		/// </summary>
		/// <param name="sign">The sign of the number</param>
		/// <param name="val">The value of the big integer</param>
		internal BigInteger(int sign, long val) {
			// PRE: (val >= 0) && (sign >= -1) && (sign <= 1)
			this.sign = sign;
			if (((ulong)val & 0xFFFFFFFF00000000L) == 0) {
				// It fits in one 'int'
				numberLength = 1;
				digits = new int[] { (int)val };
			} else {
				numberLength = 2;
				digits = new int[] { (int)val, (int)(val >> 32) };
			}
		}

		/// <summary>
		/// Creates a new <see cref="BigInteger"/> with the given sign and magnitude.
		/// </summary>
		/// <param name="signum">The sign of the number represented by <paramref name="digits"/></param>
		/// <param name="digits">The magnitude of the number</param>
		/// <remarks>
		/// This constructor does not create a copy, so any changes to the reference will 
		/// affect the new number.
		/// </remarks>
		internal BigInteger(int signum, int[] digits) {
			if (digits.Length == 0) {
				sign = 0;
				numberLength = 1;
				this.digits = new int[] { 0 };
			} else {
				sign = signum;
				numberLength = digits.Length;
				this.digits = digits;
				CutOffLeadingZeroes();
			}
		}

		#endregion

		public int Sign {
			get { return sign; }
			internal set { sign = value; }
		}

		public int BitLength {
			get { return BitLevel.BitLength(this); }
		}

		public int LowestSetBit {
			get {
				if (sign == 0) {
					return -1;
				}
				// (sign != 0) implies that exists some non zero digit
				int i = FirstNonzeroDigit;
				return ((i << 5) + Utils.NumberOfTrailingZeros(digits[i]));
			}
		}

		/// <summary>
		/// Gets the number of bits in the binary representation of this 
		/// integer which differ from the sign bit.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If this integer is positive the result is equivalent to the number of bits 
		/// set in the binary representation of this. If this integer is negative the 
		/// result is equivalent to the number of bits set in the binary representation 
		/// of <c>-this-1</c>.
		/// </para>
		/// <para>
		/// <strong>Note:</strong> Usage of this property is not recommended as the current 
		/// implementation is not efficient.
		/// </para>
		/// </remarks>
		public int BitCount {
			get { return BitLevel.BitCount(this); }
		}

		internal int FirstNonzeroDigit {
			get {
				if (firstNonzeroDigit == -2) {
					int i;
					if (this.sign == 0) {
						i = -1;
					} else {
						for (i = 0; digits[i] == 0; i++) {
							// Empty
						}
					}
					firstNonzeroDigit = i;
				}
				return firstNonzeroDigit;
			}
		}

		internal int[] Digits {
			get { return digits; }
		}


		/// <inheritdoc cref="IComparable{T}.CompareTo"/>
		public int CompareTo(BigInteger other) {
			if (sign > other.sign) {
				return GREATER;
			}
			if (sign < other.sign) {
				return LESS;
			}
			if (numberLength > other.numberLength) {
				return sign;
			}
			if (numberLength < other.numberLength) {
				return -other.sign;
			}
			// Equal sign and equal numberLength
			return (sign * Elementary.compareArrays(digits, other.digits,
					numberLength));
		}

		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() {
			if (hashCode != 0) {
				return hashCode;
			}
			for (int i = 0; i < digits.Length; i++) {
				hashCode = (int) (hashCode * 33 + (digits[i] & 0xffffffff));
			}
			hashCode = hashCode * sign;
			return hashCode;
		}

		/// <inheritdoc cref="object.Equals(object)"/>
		public override bool Equals(object obj) {
			if (ReferenceEquals(this, obj))
				return true;
			if (!(obj is BigInteger))
				return false;
			return Equals((BigInteger)obj);
		}

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public bool Equals(BigInteger other) {
			if (other == null)
				return false;

			return sign == other.sign &&
			       numberLength == other.numberLength &&
			       EqualsArrays(other.digits);
		}

		bool EqualsArrays(int[] b) {
			int i;
			for (i = numberLength - 1; (i >= 0) && (digits[i] == b[i]); i--) {
				// Empty
			}
			return i < 0;
		}

		// Decreases 'numberLength' if there are zero high elements.

		internal void CutOffLeadingZeroes() {
			while ((numberLength > 0) && (digits[--numberLength] == 0)) {
				// Empty
			}
			if (digits[numberLength++] == 0) {
				sign = 0;
			}
		}

		/// <summary>
		/// Gets a boolean indicating if this instance absolute 
		/// value is equivalent to <c>1</c>.
		/// </summary>
		public bool IsOne {
			get { return ((numberLength == 1) && (digits[0] == 1)); }
		}

		/**
		 * Puts a big-endian byte array into a little-endian int array.
		 */
		private void PutBytesPositiveToIntegers(byte[] byteValues) {
			int bytesLen = byteValues.Length;
			int highBytes = bytesLen & 3;
			numberLength = (bytesLen >> 2) + ((highBytes == 0) ? 0 : 1);
			digits = new int[numberLength];
			int i = 0;
			// Put bytes to the int array starting from the end of the byte array
			while (bytesLen > highBytes) {
				digits[i++] = (byteValues[--bytesLen] & 0xFF)
						| (byteValues[--bytesLen] & 0xFF) << 8
						| (byteValues[--bytesLen] & 0xFF) << 16
						| (byteValues[--bytesLen] & 0xFF) << 24;
			}
			// Put the first bytes in the highest element of the int array
			for (int j = 0; j < bytesLen; j++) {
				digits[i] = (digits[i] << 8) | (byteValues[j] & 0xFF);
			}
		}

		/**
		 * Puts a big-endian byte array into a little-endian applying two
		 * complement.
		 */
		private void PutBytesNegativeToIntegers(byte[] byteValues) {
			int bytesLen = byteValues.Length;
			int highBytes = bytesLen & 3;
			numberLength = (bytesLen >> 2) + ((highBytes == 0) ? 0 : 1);
			digits = new int[numberLength];
			int i = 0;
			// Setting the sign
			digits[numberLength - 1] = -1;
			// Put bytes to the int array starting from the end of the byte array
			while (bytesLen > highBytes) {
				digits[i] = (byteValues[--bytesLen] & 0xFF)
						| (byteValues[--bytesLen] & 0xFF) << 8
						| (byteValues[--bytesLen] & 0xFF) << 16
						| (byteValues[--bytesLen] & 0xFF) << 24;
				if (digits[i] != 0) {
					digits[i] = -digits[i];
					firstNonzeroDigit = i;
					i++;
					while (bytesLen > highBytes) {
						digits[i] = (byteValues[--bytesLen] & 0xFF)
								| (byteValues[--bytesLen] & 0xFF) << 8
								| (byteValues[--bytesLen] & 0xFF) << 16
								| (byteValues[--bytesLen] & 0xFF) << 24;
						digits[i] = ~digits[i];
						i++;
					}
					break;
				}
				i++;
			}
			if (highBytes != 0) {
				// Put the first bytes in the highest element of the int array
				if (firstNonzeroDigit != -2) {
					for (int j = 0; j < bytesLen; j++) {
						digits[i] = (digits[i] << 8) | (byteValues[j] & 0xFF);
					}
					digits[i] = ~digits[i];
				} else {
					for (int j = 0; j < bytesLen; j++) {
						digits[i] = (digits[i] << 8) | (byteValues[j] & 0xFF);
					}
					digits[i] = -digits[i];
				}
			}
		}

		/*
		 * Returns a copy of the current instance to achieve immutability
		 */

		internal BigInteger Copy() {
			int[] copyDigits = new int[numberLength];
			Array.Copy(digits, 0, copyDigits, 0, numberLength);
			return new BigInteger(sign, numberLength, copyDigits);
		}

		internal void UnCache() {
			firstNonzeroDigit = -2;
		}

		internal static BigInteger GetPowerOfTwo(int exp) {
			if (exp < TwoPows.Length) {
				return TwoPows[exp];
			}
			int intCount = exp >> 5;
			int bitN = exp & 31;
			int[] resDigits = new int[intCount + 1];
			resDigits[intCount] = 1 << bitN;
			return new BigInteger(1, intCount + 1, resDigits);
		}

#region Conversions

		/// <summary>
		/// A utility for constructing a big integer from a long
		/// </summary>
		/// <param name="value">The source value of the conversion</param>
		/// <returns>
		/// Returns an instance of <see cref="BigInteger"/> that is created
		/// from the source value specified.
		/// </returns>
		public static BigInteger FromInt64(long value) {
			if (value < 0) {
				if (value != -1) {
					return new BigInteger(-1, -value);
				}
				return MinusOne;
			} else if (value <= 10) {
				return SmallValues[(int)value];
			} else {// (val > 10)
				return new BigInteger(1, value);
			}
		}

		private static bool TryParse(string s, int radix, out BigInteger value, out Exception exception) {
			if (String.IsNullOrEmpty(s)) {
				exception = new FormatException(Messages.math11);
				value = null;
				return false;
			}
			if ((radix < CharHelper.MIN_RADIX) || (radix > CharHelper.MAX_RADIX)) {
				// math.11=Radix out of range
				exception = new FormatException(Messages.math12);
				value = null;
				return false;
			}

			int sign;
			int[] digits;
			int numberLength;
			int stringLength = s.Length;
			int startChar;
			int endChar = stringLength;

			if (s[0] == '-') {
				sign = -1;
				startChar = 1;
				stringLength--;
			} else {
				sign = 1;
				startChar = 0;
			}
			/*
			 * We use the following algorithm: split a string into portions of n
			 * char and convert each portion to an integer according to the
			 * radix. Then convert an exp(radix, n) based number to binary using the
			 * multiplication method. See D. Knuth, The Art of Computer Programming,
			 * vol. 2.
			 */

			try {
				int charsPerInt = Conversion.digitFitInInt[radix];
				int bigRadixDigitsLength = stringLength / charsPerInt;
				int topChars = stringLength % charsPerInt;

				if (topChars != 0) {
					bigRadixDigitsLength++;
				}
				digits = new int[bigRadixDigitsLength];
				// Get the maximal power of radix that fits in int
				int bigRadix = Conversion.bigRadices[radix - 2];
				// Parse an input string and accumulate the BigInteger's magnitude
				int digitIndex = 0; // index of digits array
				int substrEnd = startChar + ((topChars == 0) ? charsPerInt : topChars);
				int newDigit;

				for (int substrStart = startChar; substrStart < endChar; substrStart = substrEnd, substrEnd = substrStart
						+ charsPerInt) {
					int bigRadixDigit = Convert.ToInt32(s.Substring(substrStart, substrEnd - substrStart), radix);
					newDigit = Multiplication.MultiplyByInt(digits, digitIndex, bigRadix);
					newDigit += Elementary.inplaceAdd(digits, digitIndex, bigRadixDigit);
					digits[digitIndex++] = newDigit;
				}

				numberLength = digitIndex;
			} catch (Exception ex) {
				exception = ex;
				value = null;
				return false;
			}

			value = new BigInteger();
			value.sign = sign;
			value.numberLength = numberLength;
			value.digits = digits;
			value.CutOffLeadingZeroes();
			exception = null;
			return true;
		}

		public static BigInteger Parse(string s) {
			return Parse(s, 10);
		}

		public static bool TryParse(string s, out BigInteger value) {
			return TryParse(s, 10, out value);
		}

		public static bool TryParse(string s, int radix, out BigInteger value) {
			Exception error;
			return TryParse(s, radix, out value, out error);
		}

		public static BigInteger Parse(string s, int radix) {
			Exception error;
			BigInteger i;
			if (!TryParse(s, radix, out i, out error))
				throw error;

			return i;
		}

#endregion
	}
}