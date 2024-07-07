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
	/**
 *  The operations are:
 * <ul type="circle">
 * <li>Bit counting</li>
 * <li>Bit testing</li>
 * <li>Getting of the lowest bit set</li>
 * </ul>
 * 
 */

	/// <summary>
	/// Static library that provides all the <b>bit level</b> operations for <see cref="BigInteger"/>.
	/// </summary>
	/// <remarks>
	/// The operations are:
	/// <list type="buller">
	/// <item><see cref="BigInteger.ShiftLeft">Left Shifting</see></item>
	/// <item><see cref="BigInteger.ShiftRight">Right Shifting</see></item>
	/// <item><see cref="BigInteger.ClearBit">Bit Clearing</see></item>
	/// <item><see cref="BigInteger.SetBit">Bit Setting</see></item>
	/// </list>
	/// <para>
	/// All operations are provided in immutable way, and some in both mutable and immutable.
	/// </para>
	/// </remarks>
	internal static class BitLevel {
		public static int BitLength(BigInteger val) {
			if (val.Sign == 0) {
				return 0;
			}
			int bLength = (val.numberLength << 5);
			int highDigit = val.Digits[val.numberLength - 1];

			if (val.Sign < 0) {
				int i = val.FirstNonZeroDigit;
				// We reduce the problem to the positive case.
				if (i == val.numberLength - 1) {
					highDigit--;
				}
			}
			// Subtracting all sign bits
			bLength -= Utils.NumberOfLeadingZeros(highDigit);
			return bLength;
		}

		public static int BitCount(BigInteger val) {
			int bCount = 0;

			if (val.Sign == 0) {
				return 0;
			}

			int i = val.FirstNonZeroDigit;
			;
			if (val.Sign > 0) {
				for (; i < val.numberLength; i++) {
					bCount += Utils.BitCount(val.Digits[i]);
				}
			} else {
// (sign < 0)
				// this digit absorbs the carry
				bCount += Utils.BitCount(-val.Digits[i]);
				for (i++; i < val.numberLength; i++) {
					bCount += Utils.BitCount(~val.Digits[i]);
				}
				// We take the complement sum:
				bCount = (val.numberLength << 5) - bCount;
			}
			return bCount;
		}

		/// <summary>
		/// Performs a fast bit testing for positive numbers.
		/// </summary>
		/// <remarks>
		/// The bit to to be tested must be in the range <c>[0, val.BitLength - 1]</c>
		/// </remarks>
		public static bool TestBit(BigInteger val, int n) {
			// PRE: 0 <= n < val.bitLength()
			return ((val.Digits[n >> 5] & (1 << (n & 31))) != 0);
		}

		/// <summary>
		/// Check if there are 1s in the lowest bits of this <see cref="BigInteger"/>
		/// </summary>
		/// <param name="numberOfBits">The number of the lowest bits to check</param>
		/// <param name="digits">The digital representation of the integer</param>
		/// <returns>
		/// Return false if all bits are zeros, true otherwise
		/// </returns>
		public static bool NonZeroDroppedBits(int numberOfBits, int[] digits) {
			int intCount = numberOfBits >> 5;
			int bitCount = numberOfBits & 31;
			int i;

			for (i = 0; (i < intCount) && (digits[i] == 0); i++) {
				;
			}
			return ((i != intCount) || (digits[i] << (32 - bitCount) != 0));
		}

		public static BigInteger ShiftLeft(BigInteger source, int count) {
			int intCount = count >> 5;
			count &= 31; // %= 32
			int resLength = source.numberLength + intCount + ((count == 0) ? 0 : 1);
			int[] resDigits = new int[resLength];

			ShiftLeft(resDigits, source.Digits, intCount, count);
			var result = new BigInteger(source.Sign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		// val should have enough place (and one digit more)

		/// <summary>
		/// Performs <c>val &lt;&lt;= count<c></c>.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="count"></param>
		public static void InplaceShiftLeft(BigInteger val, int count) {
			int intCount = count >> 5; // count of integers
			val.numberLength += intCount
			                    + (Utils.NumberOfLeadingZeros(val.Digits[val.numberLength - 1])
			                       - (count & 31) >= 0
				                    ? 0
				                    : 1);
			ShiftLeft(val.Digits, val.Digits, intCount, count & 31);
			val.CutOffLeadingZeroes();
			val.UnCache();
		}

		/**
		 * Abstractly shifts left an array of integers in little endian (i.e. shift
		 * it right). Total shift distance in bits is intCount * 32 + count
		 * 
		 * @param result the destination array
		 * @param source the source array
		 * @param intCount the shift distance in integers
		 * @param count an additional shift distance in bits
		 */

		public static void ShiftLeft(int[] result, int[] source, int intCount, int count) {
			if (count == 0) {
				Array.Copy(source, 0, result, intCount, result.Length - intCount);
			} else {
				int rightShiftCount = 32 - count;

				result[result.Length - 1] = 0;
				for (int i = result.Length - 1; i > intCount; i--) {
					result[i] |= Utils.URShift(source[i - intCount - 1], rightShiftCount);
					result[i - 1] = source[i - intCount - 1] << count;
				}
			}

			for (int i = 0; i < intCount; i++) {
				result[i] = 0;
			}
		}

		/**
		 * Shifts the source digits left one bit, creating a value whose magnitude
		 * is doubled.
		 *
		 * @param result an array of digits that will hold the computed result when
		 *      this method returns. The size of this array is {@code srcLen + 1},
		 *      and the format is the same as {@link BigInteger#digits}.
		 * @param source the array of digits to shift left, in the same format as
		 *      {@link BigInteger#digits}.
		 * @param srcLen the length of {@code source}; may be less than {@code
		 *      source.length}
		 */

		public static void ShiftLeftOneBit(int[] result, int[] source, int srcLen) {
			int carry = 0;
			for (int i = 0; i < srcLen; i++) {
				int val = source[i];
				result[i] = (val << 1) | carry;
				carry = Utils.URShift(val, 31);
			}
			if (carry != 0) {
				result[srcLen] = carry;
			}
		}

		public static BigInteger ShiftLeftOneBit(BigInteger source) {
			int srcLen = source.numberLength;
			int resLen = srcLen + 1;
			int[] resDigits = new int[resLen];
			ShiftLeftOneBit(resDigits, source.Digits, srcLen);
			BigInteger result = new BigInteger(source.Sign, resLen, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/** @see BigInteger#shiftRight(int) */

		public static BigInteger ShiftRight(BigInteger source, int count) {
			int intCount = count >> 5; // count of integers
			count &= 31; // count of remaining bits
			if (intCount >= source.numberLength) {
				return ((source.Sign < 0) ? BigInteger.MinusOne : BigInteger.Zero);
			}
			int i;
			int resLength = source.numberLength - intCount;
			int[] resDigits = new int[resLength + 1];

			ShiftRight(resDigits, resLength, source.Digits, intCount, count);
			if (source.Sign < 0) {
				// Checking if the dropped bits are zeros (the remainder equals to
				// 0)
				for (i = 0; (i < intCount) && (source.Digits[i] == 0); i++) {
					;
				}
				// If the remainder is not zero, add 1 to the result
				if ((i < intCount)
				    || ((count > 0) && ((source.Digits[i] << (32 - count)) != 0))) {
					for (i = 0; (i < resLength) && (resDigits[i] == -1); i++) {
						resDigits[i] = 0;
					}
					if (i == resLength) {
						resLength++;
					}
					resDigits[i]++;
				}
			}
			BigInteger result = new BigInteger(source.Sign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/**
		 * Performs {@code val >>= count} where {@code val} is a positive number.
		 */

		public static void InplaceShiftRight(BigInteger val, int count) {
			int sign = val.Sign;
			if (count == 0 || val.Sign == 0)
				return;
			int intCount = count >> 5; // count of integers
			val.numberLength -= intCount;
			if (!ShiftRight(val.Digits, val.numberLength, val.Digits, intCount, count & 31)
			    && sign < 0) {
				// remainder not zero: add one to the result
				int i;
				for (i = 0; (i < val.numberLength) && (val.Digits[i] == -1); i++) {
					val.Digits[i] = 0;
				}
				if (i == val.numberLength) {
					val.numberLength++;
				}
				val.Digits[i]++;
			}
			val.CutOffLeadingZeroes();
			val.UnCache();
		}

		/**
		 * Shifts right an array of integers. Total shift distance in bits is
		 * intCount * 32 + count.
		 * 
		 * @param result
		 *            the destination array
		 * @param resultLen
		 *            the destination array's length
		 * @param source
		 *            the source array
		 * @param intCount
		 *            the number of elements to be shifted
		 * @param count
		 *            the number of bits to be shifted
		 * @return dropped bit's are all zero (i.e. remaider is zero)
		 */

		public static bool ShiftRight(int[] result, int resultLen, int[] source, int intCount, int count) {
			int i;
			bool allZero = true;
			for (i = 0; i < intCount; i++)
				allZero &= source[i] == 0;
			if (count == 0) {
				Array.Copy(source, intCount, result, 0, resultLen);
				i = resultLen;
			} else {
				int leftShiftCount = 32 - count;

				allZero &= (source[i] << leftShiftCount) == 0;
				for (i = 0; i < resultLen - 1; i++) {
					result[i] = Utils.URShift(source[i + intCount], count) |
					            (source[i + intCount + 1] << leftShiftCount);
				}
				result[i] = Utils.URShift(source[i + intCount], count);
				i++;
			}

			return allZero;
		}


		/**
		 * Performs a flipBit on the BigInteger, returning a BigInteger with the the
		 * specified bit flipped.
		 * @param intCount: the index of the element of the digits array where the operation will be performed
		 * @param bitNumber: the bit's position in the intCount element
		 */

		public static BigInteger FlipBit(BigInteger val, int n) {
			int resSign = (val.Sign == 0) ? 1 : val.Sign;
			int intCount = n >> 5;
			int bitN = n & 31;
			int resLength = System.Math.Max(intCount + 1, val.numberLength) + 1;
			int[] resDigits = new int[resLength];
			int i;

			int bitNumber = 1 << bitN;
			Array.Copy(val.Digits, 0, resDigits, 0, val.numberLength);

			if (val.Sign < 0) {
				if (intCount >= val.numberLength) {
					resDigits[intCount] = bitNumber;
				} else {
					//val.sign<0 y intCount < val.numberLength
					int firstNonZeroDigit = val.FirstNonZeroDigit;
					if (intCount > firstNonZeroDigit) {
						resDigits[intCount] ^= bitNumber;
					} else if (intCount < firstNonZeroDigit) {
						resDigits[intCount] = -bitNumber;
						for (i = intCount + 1; i < firstNonZeroDigit; i++) {
							resDigits[i] = -1;
						}
						resDigits[i] = resDigits[i]--;
					} else {
						i = intCount;
						resDigits[i] = -((-resDigits[intCount]) ^ bitNumber);
						if (resDigits[i] == 0) {
							for (i++; resDigits[i] == -1; i++) {
								resDigits[i] = 0;
							}
							resDigits[i]++;
						}
					}
				}
			} else {
//case where val is positive
				resDigits[intCount] ^= bitNumber;
			}
			BigInteger result = new BigInteger(resSign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}
	}
}