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
using System.Text;

namespace Deveel.Math {
	/// <summary>
	/// Static library that provides <see cref="BigInteger"/> base conversion from/to any
	/// integer represented in a <see cref="string"/>.
	/// </summary>
	static class Conversion {
		/// <summary>
		/// Holds the maximal exponent for each radix, so that radix<sup>digitFitInInt[radix]</sup>
		/// fits in an <c>int</c> (32 bits).
		/// </summary>
		internal static readonly int[] digitFitInInt = { -1, -1, 31, 19, 15, 13, 11,
            11, 10, 9, 9, 8, 8, 8, 8, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6,
            6, 6, 6, 6, 6, 6, 6, 5 };

		/// <summary>
		/// Precomputed maximal powers of radices (integer numbers from 2 to 36)
		/// that fit into unsigned int (32 bits). bigRadices[0] = 2 ^ 31,
		/// bigRadices[8] = 10 ^ 9, etc.
		/// </summary>
		internal static readonly int[] bigRadices = { -2147483648, 1162261467,
            1073741824, 1220703125, 362797056, 1977326743, 1073741824,
            387420489, 1000000000, 214358881, 429981696, 815730721, 1475789056,
            170859375, 268435456, 410338673, 612220032, 893871739, 1280000000,
            1801088541, 113379904, 148035889, 191102976, 244140625, 308915776,
            387420489, 481890304, 594823321, 729000000, 887503681, 1073741824,
            1291467969, 1544804416, 1838265625, 60466176 };

		/// <summary>
		/// Converts the given <see cref="BigInteger"/> to a string representation in the specified radix.
		/// </summary>
		/// <param name="val">The big integer value to convert.</param>
		/// <param name="radix">The radix (base) for the string representation.</param>
		/// <returns>A string representation of <paramref name="val"/> in the given <paramref name="radix"/>.</returns>
		/// <example>
		/// <code>
		/// BigInteger value = new BigInteger(255);
		/// string hex = Conversion.BigInteger2String(value, 16);
		/// // hex == "ff"
		/// </code>
		/// </example>
		public static string BigInteger2String(BigInteger val, int radix) {
			int sign = val.Sign;
			int numberLength = val.numberLength;
			int[] digits = val.Digits;

			if (sign == 0) {
				return "0";
			}
			if (numberLength == 1) {
				int highDigit = digits[numberLength - 1];
				long v = highDigit & 0xFFFFFFFFL;
				if (sign < 0) {
                    return "-" + Convert.ToString(v, radix);
				}
				return Convert.ToString(v, radix);
			}
            if ((radix == 10) || (radix < CharHelper.MIN_RADIX)
                    || (radix > CharHelper.MAX_RADIX))
            {
				return val.ToString();
			}
			double bitsForRadixDigit;
			bitsForRadixDigit = System.Math.Log(radix) / System.Math.Log(2);
			int resLengthInChars = (int)(BigMath.Abs(val).BitLength / bitsForRadixDigit + ((sign < 0) ? 1
					: 0)) + 1;

			char[] result = new char[resLengthInChars];
			int currentChar = resLengthInChars;
			int resDigit;
			if (radix != 16) {
				int[] temp = new int[numberLength];
				Array.Copy(digits, 0, temp, 0, numberLength);
				int tempLen = numberLength;
				int charsPerInt = digitFitInInt[radix];
				int i;
				int bigRadix = bigRadices[radix - 2];
				while (true) {
					resDigit = Division.DivideArrayByInt(temp, temp, tempLen, bigRadix);
					int previous = currentChar;
					do {
                        result[--currentChar] = CharHelper.forDigit(
								resDigit % radix, radix);
					} while (((resDigit /= radix) != 0) && (currentChar != 0));
					int delta = charsPerInt - previous + currentChar;
					for (i = 0; i < delta && currentChar > 0; i++) {
						result[--currentChar] = '0';
					}
					for (i = tempLen - 1; (i > 0) && (temp[i] == 0); i--) {
						;
					}
					tempLen = i + 1;
					if ((tempLen == 1) && (temp[0] == 0)) {
						break;
					}
				}
			} else {
				for (int i = 0; i < numberLength; i++) {
					for (int j = 0; (j < 8) && (currentChar > 0); j++) {
						resDigit = digits[i] >> (j << 2) & 0xf;
                        result[--currentChar] = CharHelper.forDigit(resDigit, 16);
					}
				}
			}
			while (result[currentChar] == '0') {
				currentChar++;
			}
			if (sign == -1) {
				result[--currentChar] = '-';
			}
			return new String(result, currentChar, resLengthInChars - currentChar);
		}

		/// <summary>
		/// Builds the corresponding <see cref="string"/> representation of <paramref name="val"/>
		/// scaled by <paramref name="scale"/>.
		/// </summary>
		/// <param name="val">The big integer to convert.</param>
		/// <param name="scale">The number of digits to the right of the decimal point.</param>
		/// <returns>A scaled decimal string representation.</returns>
		/// <example>
		/// <code>
		/// BigInteger value = new BigInteger(12345);
		/// string s = Conversion.ToDecimalScaledString(value, 2);
		/// // s == "123.45"
		/// </code>
		/// </example>
		public static String ToDecimalScaledString(BigInteger val, int scale) {
			int sign = val.Sign;
			int numberLength = val.numberLength;
			int[] digits = val.Digits;
			int resLengthInChars;
			int currentChar;
			char[] result;

			if (sign == 0) {
				switch (scale) {
					case 0:
						return "0";
					case 1:
						return "0.0";
					case 2:
						return "0.00";
					case 3:
						return "0.000";
					case 4:
						return "0.0000";
					case 5:
						return "0.00000";
					case 6:
						return "0.000000";
					default: {
							StringBuilder result2 = new StringBuilder();
							if (scale < 0) {
								result2.Append("0E+");
							} else {
								result2.Append("0E");
							}
							result2.Append(-scale);
							return result2.ToString();
						}
				}
			}
			resLengthInChars = numberLength * 10 + 1 + 7;
			result = new char[resLengthInChars + 1];
			currentChar = resLengthInChars;
			if (numberLength == 1) {
				int highDigit = digits[0];
				if (highDigit < 0) {
					long v = highDigit & 0xFFFFFFFFL;
					do {
						long prev = v;
						v /= 10;
						result[--currentChar] = (char)(0x0030 + ((int)(prev - v * 10)));
					} while (v != 0);
				} else {
					int v = highDigit;
					do {
						int prev = v;
						v /= 10;
						result[--currentChar] = (char)(0x0030 + (prev - v * 10));
					} while (v != 0);
				}
			} else {
				int[] temp = new int[numberLength];
				int tempLen = numberLength;
				Array.Copy(digits, 0, temp, 0, tempLen);
				while (true) {
					long result11 = 0;
					for (int i1 = tempLen - 1; i1 >= 0; i1--) {
						long temp1 = (result11 << 32)
								+ (temp[i1] & 0xFFFFFFFFL);
						long res = DivideLongByBillion(temp1);
						temp[i1] = (int)res;
						result11 = (int)(res >> 32);
					}
					int resDigit = (int)result11;
					int previous = currentChar;
					do {
						result[--currentChar] = (char)(0x0030 + (resDigit % 10));
					} while (((resDigit /= 10) != 0) && (currentChar != 0));
					int delta = 9 - previous + currentChar;
					for (int i = 0; (i < delta) && (currentChar > 0); i++) {
						result[--currentChar] = '0';
					}
					int j = tempLen - 1;
					for (; temp[j] == 0; j--) {
						if (j == 0) {
							goto BIG_LOOP;
						}
					}
					tempLen = j + 1;
				}
			BIG_LOOP:
				while (result[currentChar] == '0') {
					currentChar++;
				}
			}
			bool negNumber = (sign < 0);
			int exponent = resLengthInChars - currentChar - scale - 1;
			if (scale == 0) {
				if (negNumber) {
					result[--currentChar] = '-';
				}
				return new String(result, currentChar, resLengthInChars
						- currentChar);
			}
			if ((scale > 0) && (exponent >= -6)) {
				if (exponent >= 0) {
					int insertPoint = currentChar + exponent;
					for (int j = resLengthInChars - 1; j >= insertPoint; j--) {
						result[j + 1] = result[j];
					}
					result[++insertPoint] = '.';
					if (negNumber) {
						result[--currentChar] = '-';
					}
					return new String(result, currentChar, resLengthInChars
							- currentChar + 1);
				}
				for (int j = 2; j < -exponent + 1; j++) {
					result[--currentChar] = '0';
				}
				result[--currentChar] = '.';
				result[--currentChar] = '0';
				if (negNumber) {
					result[--currentChar] = '-';
				}
				return new String(result, currentChar, resLengthInChars
						- currentChar);
			}
			int startPoint = currentChar + 1;
			int endPoint = resLengthInChars;
			StringBuilder result1 = new StringBuilder(16 + endPoint - startPoint);
			if (negNumber) {
				result1.Append('-');
			}
			if (endPoint - startPoint >= 1) {
				result1.Append(result[currentChar]);
				result1.Append('.');
				result1.Append(result, currentChar + 1, resLengthInChars
						- currentChar - 1);
			} else {
				result1.Append(result, currentChar, resLengthInChars
						- currentChar);
			}
			result1.Append('E');
			if (exponent > 0) {
				result1.Append('+');
			}
			result1.Append(Convert.ToString(exponent));
			return result1.ToString();
		}

		/// <summary>
		/// Builds a scaled decimal string representation of a 32-bit number.
		/// </summary>
		/// <param name="value">The long value to convert.</param>
		/// <param name="scale">The number of digits to the right of the decimal point.</param>
		/// <returns>A scaled decimal string representation of the value.</returns>
		public static String ToDecimalScaledString(long value, int scale) {
			int resLengthInChars;
			int currentChar;
			char[] result;
			bool negNumber = value < 0;
			if (negNumber) {
				value = -value;
			}
			if (value == 0) {
				switch (scale) {
					case 0: return "0";
					case 1: return "0.0";
					case 2: return "0.00";
					case 3: return "0.000";
					case 4: return "0.0000";
					case 5: return "0.00000";
					case 6: return "0.000000";
					default:
						StringBuilder result2 = new StringBuilder();
						if (scale < 0) {
							result2.Append("0E+");
						} else {
							result2.Append("0E");
						}
						result2.Append((scale == Int32.MinValue) ? "2147483648" : Convert.ToString(-scale));
						return result2.ToString();
				}
			}
			resLengthInChars = 18;
			result = new char[resLengthInChars + 1];
			currentChar = resLengthInChars;
			long v = value;
			do {
				long prev = v;
				v /= 10;
				result[--currentChar] = (char)(0x0030 + (prev - v * 10));
			} while (v != 0);

			long exponent = (long)resLengthInChars - (long)currentChar - scale - 1L;
			if (scale == 0) {
				if (negNumber) {
					result[--currentChar] = '-';
				}
				return new String(result, currentChar, resLengthInChars - currentChar);
			}
			if (scale > 0 && exponent >= -6) {
				if (exponent >= 0) {
					int insertPoint = currentChar + (int)exponent;
					for (int j = resLengthInChars - 1; j >= insertPoint; j--) {
						result[j + 1] = result[j];
					}
					result[++insertPoint] = '.';
					if (negNumber) {
						result[--currentChar] = '-';
					}
					return new String(result, currentChar, resLengthInChars - currentChar + 1);
				}
				for (int j = 2; j < -exponent + 1; j++) {
					result[--currentChar] = '0';
				}
				result[--currentChar] = '.';
				result[--currentChar] = '0';
				if (negNumber) {
					result[--currentChar] = '-';
				}
				return new String(result, currentChar, resLengthInChars - currentChar);
			}
			int startPoint = currentChar + 1;
			int endPoint = resLengthInChars;
			StringBuilder result1 = new StringBuilder(16 + endPoint - startPoint);
			if (negNumber) {
				result1.Append('-');
			}
			if (endPoint - startPoint >= 1) {
				result1.Append(result[currentChar]);
				result1.Append('.');
				result1.Append(result, currentChar + 1, resLengthInChars - currentChar - 1);
			} else {
				result1.Append(result, currentChar, resLengthInChars - currentChar);
			}
			result1.Append('E');
			if (exponent > 0) {
				result1.Append('+');
			}
			result1.Append(Convert.ToString(exponent));
			return result1.ToString();
		}

		/// <summary>
		/// Divides a long value by 1,000,000,000 (one billion) and returns the
		/// quotient and remainder packed into a single <see cref="long"/>.
		/// </summary>
		/// <param name="a">The dividend.</param>
		/// <returns>
		/// A long where the lower 32 bits contain the quotient and the upper 32 bits
		/// contain the remainder.
		/// </returns>
		public static long DivideLongByBillion(long a) {
			long quot;
			long rem;

			if (a >= 0) {
				long bLong = 1000000000L;
				quot = (a / bLong);
				rem = (a % bLong);
			} else {
				long aPos = Utils.URShift(a, 1);
				long bPos = Utils.URShift(1000000000L, 1);
				quot = aPos / bPos;
				rem = aPos % bPos;
				rem = (rem << 1) + (a & 1);
			}
			return ((rem << 32) | (quot & 0xFFFFFFFFL));
		}

		/// <summary>
		/// Converts the given <see cref="BigInteger"/> to a <see cref="double"/> value.
		/// </summary>
		/// <param name="val">The big integer to convert.</param>
		/// <returns>The double-precision floating-point representation of <paramref name="val"/>.</returns>
		/// <example>
		/// <code>
		/// BigInteger value = new BigInteger(123456789);
		/// double d = Conversion.BigInteger2Double(value);
		/// // d == 123456789.0
		/// </code>
		/// </example>
		public static double BigInteger2Double(BigInteger val) {
			if ((val.numberLength < 2)
					|| ((val.numberLength == 2) && (val.Digits[1] > 0))) {
				return val.ToInt64();
			}
			if (val.numberLength > 32) {
				return ((val.Sign > 0) ? Double.PositiveInfinity
						: Double.NegativeInfinity);
			}
			int bitLen = BigMath.Abs(val).BitLength;
			long exponent = bitLen - 1;
			int delta = bitLen - 54;
			long lVal = (BigMath.Abs(val) >> delta).ToInt64();
			long mantissa = lVal & 0x1FFFFFFFFFFFFFL;
			if (exponent == 1023) {
				if (mantissa == 0X1FFFFFFFFFFFFFL) {
					return ((val.Sign > 0) ? Double.PositiveInfinity
							: Double.NegativeInfinity);
				}
				if (mantissa == 0x1FFFFFFFFFFFFEL) {
					return ((val.Sign > 0) ? Double.MaxValue : -Double.MaxValue);
				}
			}
			if (((mantissa & 1) == 1)
					&& (((mantissa & 2) == 2) || BitLevel.NonZeroDroppedBits(delta,
							val.Digits))) {
				mantissa += 2;
			}
			mantissa >>= 1;
            long resSign = (val.Sign < 0) ? Int64.MinValue : 0;
			exponent = ((1023 + exponent) << 52) & 0x7FF0000000000000L;
			long result = resSign | exponent | mantissa;
			return BitConverter.Int64BitsToDouble(result);
		}
	}
}
