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
	/// Implements logical operations over <see cref="BigInteger"/>:
	/// <list type="bullet">
	/// <item><description><c>not</c></description></item>
	/// <item><description><c>and</c></description></item>
	/// <item><description><c>andNot</c></description></item>
	/// <item><description><c>or</c></description></item>
	/// <item><description><c>xor</c></description></item>
	/// </list>
	/// </summary>
	static class Logical {

		/// <summary>
		/// Computes the bitwise NOT of <paramref name="val"/>.
		/// </summary>
		/// <param name="val">The big integer.</param>
		/// <returns><c>~val</c>.</returns>
		/// <example>
		/// <code>
		/// BigInteger val = new BigInteger(5);  // 0101
		/// BigInteger result = Logical.Not(val); // ~0101 = ...11111010 = -6
		/// // result == -6
		/// </code>
		/// </example>
		public static BigInteger Not(BigInteger val) {
			if (val.Sign == 0) {
				return BigInteger.MinusOne;
			}
			if (val.Equals(BigInteger.MinusOne)) {
				return BigInteger.Zero;
			}
			int[] resDigits = new int[val.numberLength + 1];
			int i;

			if (val.Sign > 0) {
				if (val.Digits[val.numberLength - 1] != -1) {
					for (i = 0; val.Digits[i] == -1; i++) {
						;
					}
				} else {
					for (i = 0; (i < val.numberLength) && (val.Digits[i] == -1); i++) {
						;
					}
					if (i == val.numberLength) {
						resDigits[i] = 1;
						return new BigInteger(-val.Sign, i + 1, resDigits);
					}
				}
			} else {
				for (i = 0; val.Digits[i] == 0; i++) {
					resDigits[i] = -1;
				}
			}
			resDigits[i] = val.Digits[i] + val.Sign;
			for (i++; i < val.numberLength; i++) {
				resDigits[i] = val.Digits[i];
			}
			return new BigInteger(-val.Sign, i, resDigits);
		}

		/// <summary>
		/// Computes the bitwise AND of <paramref name="val"/> and <paramref name="that"/>.
		/// </summary>
		/// <param name="val">The first operand.</param>
		/// <param name="that">The second operand.</param>
		/// <returns><c>val &amp; that</c>.</returns>
		/// <example>
		/// <code>
		/// BigInteger a = new BigInteger(6);  // 0110
		/// BigInteger b = new BigInteger(3);  // 0011
		/// BigInteger result = Logical.And(a, b); // 0110 &amp; 0011 = 0010 = 2
		/// // result == 2
		/// </code>
		/// </example>
		public static BigInteger And(BigInteger val, BigInteger that) {
			if (that.Sign == 0 || val.Sign == 0) {
				return BigInteger.Zero;
			}
			if (that.Equals(BigInteger.MinusOne)) {
				return val;
			}
			if (val.Equals(BigInteger.MinusOne)) {
				return that;
			}

			if (val.Sign > 0) {
				if (that.Sign > 0) {
					return AndPositive(val, that);
				} else {
					return AndDiffSigns(val, that);
				}
			} else {
				if (that.Sign > 0) {
					return AndDiffSigns(that, val);
				} else if (val.numberLength > that.numberLength) {
					return AndNegative(val, that);
				} else {
					return AndNegative(that, val);
				}
			}
		}

		/// <summary>
		/// Computes the bitwise AND of two positive numbers.
		/// </summary>
		/// <returns>Sign = 1, magnitude = val.magnitude &amp; that.magnitude.</returns>
		private static BigInteger AndPositive(BigInteger val, BigInteger that) {
			int resLength = System.Math.Min(val.numberLength, that.numberLength);
			int i = System.Math.Max(val.FirstNonZeroDigit, that.FirstNonZeroDigit);

			if (i >= resLength) {
				return BigInteger.Zero;
			}

			int[] resDigits = new int[resLength];
			for (; i < resLength; i++) {
				resDigits[i] = val.Digits[i] & that.Digits[i];
			}

			BigInteger result = new BigInteger(1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Computes the bitwise AND of a positive and a negative number.
		/// </summary>
		/// <returns>Sign = positive.magnitude &amp; magnitude = -negative.magnitude.</returns>
		private static BigInteger AndDiffSigns(BigInteger positive, BigInteger negative) {
			int iPos = positive.FirstNonZeroDigit;
			int iNeg = negative.FirstNonZeroDigit;

			if (iNeg >= positive.numberLength) {
				return BigInteger.Zero;
			}
			int resLength = positive.numberLength;
			int[] resDigits = new int[resLength];

			int i = System.Math.Max(iPos, iNeg);
			if (i == iNeg) {
				resDigits[i] = -negative.Digits[i] & positive.Digits[i];
				i++;
			}
			int limit = System.Math.Min(negative.numberLength, positive.numberLength);
			for (; i < limit; i++) {
				resDigits[i] = ~negative.Digits[i] & positive.Digits[i];
			}
			if (i >= negative.numberLength) {
				for (; i < positive.numberLength; i++) {
					resDigits[i] = positive.Digits[i];
				}
			}

			BigInteger result = new BigInteger(1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Computes the bitwise AND of two negative numbers.
		/// </summary>
		/// <returns>Sign = -1, magnitude = -(-longer.magnitude &amp; -shorter.magnitude).</returns>
		private static BigInteger AndNegative(BigInteger longer, BigInteger shorter) {
			int iLonger = longer.FirstNonZeroDigit;
			int iShorter = shorter.FirstNonZeroDigit;

			if (iLonger >= shorter.numberLength) {
				return longer;
			}

			int resLength;
			int[] resDigits;
			int i = System.Math.Max(iShorter, iLonger);
			int digit;
			if (iShorter > iLonger) {
				digit = -shorter.Digits[i] & ~longer.Digits[i];
			} else if (iShorter < iLonger) {
				digit = ~shorter.Digits[i] & -longer.Digits[i];
			} else {
				digit = -shorter.Digits[i] & -longer.Digits[i];
			}
			if (digit == 0) {
				for (i++; i < shorter.numberLength && (digit = ~(longer.Digits[i] | shorter.Digits[i])) == 0; i++)
					;
				if (digit == 0) {
					for (; i < longer.numberLength && (digit = ~longer.Digits[i]) == 0; i++)
						;
					if (digit == 0) {
						resLength = longer.numberLength + 1;
						resDigits = new int[resLength];
						resDigits[resLength - 1] = 1;

						return new BigInteger(-1, resLength, resDigits);
					}
				}
			}
			resLength = longer.numberLength;
			resDigits = new int[resLength];
			resDigits[i] = -digit;
			for (i++; i < shorter.numberLength; i++) {
				resDigits[i] = longer.Digits[i] | shorter.Digits[i];
			}
			for (; i < longer.numberLength; i++) {
				resDigits[i] = longer.Digits[i];
			}

			BigInteger result = new BigInteger(-1, resLength, resDigits);
			return result;
		}

		/// <summary>
		/// Computes the bitwise AND-NOT of <paramref name="val"/> and <paramref name="that"/>.
		/// </summary>
		/// <param name="val">The first operand.</param>
		/// <param name="that">The second operand.</param>
		/// <returns><c>val &amp; ~that</c>.</returns>
		/// <example>
		/// <code>
		/// BigInteger a = new BigInteger(6);  // 0110
		/// BigInteger b = new BigInteger(5);  // 0101
		/// BigInteger result = Logical.AndNot(a, b); // 0110 &amp; ~0101 = 0110 &amp; 1010 = 0010 = 2
		/// // result == 2
		/// </code>
		/// </example>
		public static BigInteger AndNot(BigInteger val, BigInteger that) {
			if (that.Sign == 0) {
				return val;
			}
			if (val.Sign == 0) {
				return BigInteger.Zero;
			}
			if (val.Equals(BigInteger.MinusOne)) {
				return ~that;
			}
			if (that.Equals(BigInteger.MinusOne)) {
				return BigInteger.Zero;
			}

			if (val.Sign > 0) {
				if (that.Sign > 0) {
					return AndNotPositive(val, that);
				} else {
					return AndNotPositiveNegative(val, that);
				}
			} else {
				if (that.Sign > 0) {
					return AndNotNegativePositive(val, that);
				} else {
					return AndNotNegative(val, that);
				}
			}
		}

		/// <summary>
		/// Computes AND-NOT for two positive numbers.
		/// </summary>
		/// <returns>Sign = 1, magnitude = val.magnitude &amp; ~that.magnitude.</returns>
		private static BigInteger AndNotPositive(BigInteger val, BigInteger that) {
			int[] resDigits = new int[val.numberLength];

			int limit = System.Math.Min(val.numberLength, that.numberLength);
			int i;
			for (i = val.FirstNonZeroDigit; i < limit; i++) {
				resDigits[i] = val.Digits[i] & ~that.Digits[i];
			}
			for (; i < val.numberLength; i++) {
				resDigits[i] = val.Digits[i];
			}

			BigInteger result = new BigInteger(1, val.numberLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Computes AND-NOT for a positive and a negative number.
		/// </summary>
		/// <returns>Sign = 1, magnitude = positive.magnitude &amp; ~(-negative.magnitude).</returns>
		private static BigInteger AndNotPositiveNegative(BigInteger positive, BigInteger negative) {
			int iNeg = negative.FirstNonZeroDigit;
			int iPos = positive.FirstNonZeroDigit;

			if (iNeg >= positive.numberLength) {
				return positive;
			}

			int resLength = System.Math.Min(positive.numberLength, negative.numberLength);
			int[] resDigits = new int[resLength];

			int i = iPos;
			for (; i < iNeg; i++) {
				resDigits[i] = positive.Digits[i];
			}
			if (i == iNeg) {
				resDigits[i] = positive.Digits[i] & (negative.Digits[i] - 1);
				i++;
			}
			for (; i < resLength; i++) {
				resDigits[i] = positive.Digits[i] & negative.Digits[i];
			}

			BigInteger result = new BigInteger(1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Computes AND-NOT for a negative and a positive number.
		/// </summary>
		/// <returns>Sign = -1, magnitude = -(-negative.magnitude &amp; ~positive.magnitude).</returns>
		private static BigInteger AndNotNegativePositive(BigInteger negative, BigInteger positive) {
			int resLength;
			int[] resDigits;
			int limit;
			int digit;

			int iNeg = negative.FirstNonZeroDigit;
			int iPos = positive.FirstNonZeroDigit;

			if (iNeg >= positive.numberLength) {
				return negative;
			}

			resLength = System.Math.Max(negative.numberLength, positive.numberLength);
			int i = iNeg;
			if (iPos > iNeg) {
				resDigits = new int[resLength];
				limit = System.Math.Min(negative.numberLength, iPos);
				for (; i < limit; i++) {
					resDigits[i] = negative.Digits[i];
				}
				if (i == negative.numberLength) {
					for (i = iPos; i < positive.numberLength; i++) {
						resDigits[i] = positive.Digits[i];
					}
				}
			} else {
				digit = -negative.Digits[i] & ~positive.Digits[i];
				if (digit == 0) {
					limit = System.Math.Min(positive.numberLength, negative.numberLength);
					for (i++; i < limit && (digit = ~(negative.Digits[i] | positive.Digits[i])) == 0; i++)
						;
					if (digit == 0) {
						for (; i < positive.numberLength && (digit = ~positive.Digits[i]) == 0; i++)
							;
						for (; i < negative.numberLength && (digit = ~negative.Digits[i]) == 0; i++)
							;
						if (digit == 0) {
							resLength++;
							resDigits = new int[resLength];
							resDigits[resLength - 1] = 1;

							return new BigInteger(-1, resLength, resDigits);
						}
					}
				}
				resDigits = new int[resLength];
				resDigits[i] = -digit;
				i++;
			}

			limit = System.Math.Min(positive.numberLength, negative.numberLength);
			for (; i < limit; i++) {
				resDigits[i] = negative.Digits[i] | positive.Digits[i];
			}
			for (; i < negative.numberLength; i++) {
				resDigits[i] = negative.Digits[i];
			}
			for (; i < positive.numberLength; i++) {
				resDigits[i] = positive.Digits[i];
			}

			BigInteger result = new BigInteger(-1, resLength, resDigits);
			return result;
		}

		/// <summary>
		/// Computes AND-NOT for two negative numbers.
		/// </summary>
		/// <returns>Sign = 1, magnitude = -val.magnitude &amp; ~(-that.magnitude).</returns>
		private static BigInteger AndNotNegative(BigInteger val, BigInteger that) {
			int iVal = val.FirstNonZeroDigit;
			int iThat = that.FirstNonZeroDigit;

			if (iVal >= that.numberLength) {
				return BigInteger.Zero;
			}

			int resLength = that.numberLength;
			int[] resDigits = new int[resLength];
			int limit;
			int i = iVal;
			if (iVal < iThat) {
				resDigits[i] = -val.Digits[i];
				limit = System.Math.Min(val.numberLength, iThat);
				for (i++; i < limit; i++) {
					resDigits[i] = ~val.Digits[i];
				}
				if (i == val.numberLength) {
					for (; i < iThat; i++) {
						resDigits[i] = -1;
					}
					resDigits[i] = that.Digits[i] - 1;
				} else {
					resDigits[i] = ~val.Digits[i] & (that.Digits[i] - 1);
				}
			} else if (iThat < iVal) {
				resDigits[i] = -val.Digits[i] & that.Digits[i];
			} else {
				resDigits[i] = -val.Digits[i] & (that.Digits[i] - 1);
			}

			limit = System.Math.Min(val.numberLength, that.numberLength);
			for (i++; i < limit; i++) {
				resDigits[i] = ~val.Digits[i] & that.Digits[i];
			}
			for (; i < that.numberLength; i++) {
				resDigits[i] = that.Digits[i];
			}

			BigInteger result = new BigInteger(1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Computes the bitwise OR of <paramref name="val"/> and <paramref name="that"/>.
		/// </summary>
		/// <param name="val">The first operand.</param>
		/// <param name="that">The second operand.</param>
		/// <returns><c>val | that</c>.</returns>
		/// <example>
		/// <code>
		/// BigInteger a = new BigInteger(4);  // 0100
		/// BigInteger b = new BigInteger(3);  // 0011
		/// BigInteger result = Logical.Or(a, b); // 0100 | 0011 = 0111 = 7
		/// // result == 7
		/// </code>
		/// </example>
		public static BigInteger Or(BigInteger val, BigInteger that) {
			if (that.Equals(BigInteger.MinusOne) || val.Equals(BigInteger.MinusOne)) {
				return BigInteger.MinusOne;
			}
			if (that.Sign == 0) {
				return val;
			}
			if (val.Sign == 0) {
				return that;
			}

			if (val.Sign > 0) {
				if (that.Sign > 0) {
					if (val.numberLength > that.numberLength) {
						return OrPositive(val, that);
					} else {
						return OrPositive(that, val);
					}
				} else {
					return OrDiffSigns(val, that);
				}
			} else {
				if (that.Sign > 0) {
					return OrDiffSigns(that, val);
				} else if (that.FirstNonZeroDigit > val.FirstNonZeroDigit) {
					return OrNegative(that, val);
				} else {
					return OrNegative(val, that);
				}
			}
		}

		/// <summary>
		/// Computes the bitwise OR of two positive numbers.
		/// </summary>
		/// <returns>Sign = 1, magnitude = longer.magnitude | shorter.magnitude.</returns>
		private static BigInteger OrPositive(BigInteger longer, BigInteger shorter) {
			int resLength = longer.numberLength;
			int[] resDigits = new int[resLength];

			int i = System.Math.Min(longer.FirstNonZeroDigit, shorter.FirstNonZeroDigit);
			for (i = 0; i < shorter.numberLength; i++) {
				resDigits[i] = longer.Digits[i] | shorter.Digits[i];
			}
			for (; i < resLength; i++) {
				resDigits[i] = longer.Digits[i];
			}

			BigInteger result = new BigInteger(1, resLength, resDigits);
			return result;
		}

		/// <summary>
		/// Computes the bitwise OR of two negative numbers.
		/// </summary>
		/// <returns>Sign = -1, magnitude = -(-val.magnitude | -that.magnitude).</returns>
		private static BigInteger OrNegative(BigInteger val, BigInteger that) {
			int iThat = that.FirstNonZeroDigit;
			int iVal = val.FirstNonZeroDigit;
			int i;

			if (iVal >= that.numberLength) {
				return that;
			} else if (iThat >= val.numberLength) {
				return val;
			}

			int resLength = System.Math.Min(val.numberLength, that.numberLength);
			int[] resDigits = new int[resLength];

			if (iThat == iVal) {
				resDigits[iVal] = -(-val.Digits[iVal] | -that.Digits[iVal]);
				i = iVal;
			} else {
				for (i = iThat; i < iVal; i++) {
					resDigits[i] = that.Digits[i];
				}
				resDigits[i] = that.Digits[i] & (val.Digits[i] - 1);
			}

			for (i++; i < resLength; i++) {
				resDigits[i] = val.Digits[i] & that.Digits[i];
			}

			BigInteger result = new BigInteger(-1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Computes the bitwise OR of a positive and a negative number.
		/// </summary>
		/// <returns>Sign = -1, magnitude = -(positive.magnitude | -negative.magnitude).</returns>
		private static BigInteger OrDiffSigns(BigInteger positive, BigInteger negative) {
			int iNeg = negative.FirstNonZeroDigit;
			int iPos = positive.FirstNonZeroDigit;
			int i;
			int limit;

			if (iPos >= negative.numberLength) {
				return negative;
			}
			int resLength = negative.numberLength;
			int[] resDigits = new int[resLength];

			if (iNeg < iPos) {
				for (i = iNeg; i < iPos; i++) {
					resDigits[i] = negative.Digits[i];
				}
			} else if (iPos < iNeg) {
				i = iPos;
				resDigits[i] = -positive.Digits[i];
				limit = System.Math.Min(positive.numberLength, iNeg);
				for (i++; i < limit; i++) {
					resDigits[i] = ~positive.Digits[i];
				}
				if (i != positive.numberLength) {
					resDigits[i] = ~(-negative.Digits[i] | positive.Digits[i]);
				} else {
					for (; i < iNeg; i++) {
						resDigits[i] = -1;
					}
					resDigits[i] = negative.Digits[i] - 1;
				}
				i++;
			} else {
				i = iPos;
				resDigits[i] = -(-negative.Digits[i] | positive.Digits[i]);
				i++;
			}
			limit = System.Math.Min(negative.numberLength, positive.numberLength);
			for (; i < limit; i++) {
				resDigits[i] = negative.Digits[i] & ~positive.Digits[i];
			}
			for (; i < negative.numberLength; i++) {
				resDigits[i] = negative.Digits[i];
			}

			BigInteger result = new BigInteger(-1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Computes the bitwise XOR of <paramref name="val"/> and <paramref name="that"/>.
		/// </summary>
		/// <param name="val">The first operand.</param>
		/// <param name="that">The second operand.</param>
		/// <returns><c>val ^ that</c>.</returns>
		/// <example>
		/// <code>
		/// BigInteger a = new BigInteger(5);  // 0101
		/// BigInteger b = new BigInteger(3);  // 0011
		/// BigInteger result = Logical.Xor(a, b); // 0101 ^ 0011 = 0110 = 6
		/// // result == 6
		/// </code>
		/// </example>
		public static BigInteger Xor(BigInteger val, BigInteger that) {
			if (that.Sign == 0) {
				return val;
			}
			if (val.Sign == 0) {
				return that;
			}
			if (that.Equals(BigInteger.MinusOne)) {
				return ~val;
			}
			if (val.Equals(BigInteger.MinusOne)) {
				return ~that;
			}

			if (val.Sign > 0) {
				if (that.Sign > 0) {
					if (val.numberLength > that.numberLength) {
						return XorPositive(val, that);
					} else {
						return XorPositive(that, val);
					}
				} else {
					return XorDiffSigns(val, that);
				}
			} else {
				if (that.Sign > 0) {
					return XorDiffSigns(that, val);
				} else if (that.FirstNonZeroDigit > val.FirstNonZeroDigit) {
					return XorNegative(that, val);
				} else {
					return XorNegative(val, that);
				}
			}
		}

		/// <summary>
		/// Computes the bitwise XOR of two positive numbers.
		/// </summary>
		/// <returns>Sign = 0, magnitude = longer.magnitude ^ shorter.magnitude.</returns>
		private static BigInteger XorPositive(BigInteger longer, BigInteger shorter) {
			int resLength = longer.numberLength;
			int[] resDigits = new int[resLength];
			int i = System.Math.Min(longer.FirstNonZeroDigit, shorter.FirstNonZeroDigit);
			for (; i < shorter.numberLength; i++) {
				resDigits[i] = longer.Digits[i] ^ shorter.Digits[i];
			}
			for (; i < longer.numberLength; i++) {
				resDigits[i] = longer.Digits[i];
			}

			BigInteger result = new BigInteger(1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Computes the bitwise XOR of two negative numbers.
		/// </summary>
		/// <returns>Sign = 0, magnitude = -val.magnitude ^ -that.magnitude.</returns>
		private static BigInteger XorNegative(BigInteger val, BigInteger that) {
			int resLength = System.Math.Max(val.numberLength, that.numberLength);
			int[] resDigits = new int[resLength];
			int iVal = val.FirstNonZeroDigit;
			int iThat = that.FirstNonZeroDigit;
			int i = iThat;
			int limit;

			if (iVal == iThat) {
				resDigits[i] = -val.Digits[i] ^ -that.Digits[i];
			} else {
				resDigits[i] = -that.Digits[i];
				limit = System.Math.Min(that.numberLength, iVal);
				for (i++; i < limit; i++) {
					resDigits[i] = ~that.Digits[i];
				}
				if (i == that.numberLength) {
					for (; i < iVal; i++) {
						resDigits[i] = -1;
					}
					resDigits[i] = val.Digits[i] - 1;
				} else {
					resDigits[i] = -val.Digits[i] ^ ~that.Digits[i];
				}
			}

			limit = System.Math.Min(val.numberLength, that.numberLength);
			for (i++; i < limit; i++) {
				resDigits[i] = val.Digits[i] ^ that.Digits[i];
			}
			for (; i < val.numberLength; i++) {
				resDigits[i] = val.Digits[i];
			}
			for (; i < that.numberLength; i++) {
				resDigits[i] = that.Digits[i];
			}

			BigInteger result = new BigInteger(1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Computes the bitwise XOR of a positive and a negative number.
		/// </summary>
		/// <returns>Sign = 1, magnitude = -(positive.magnitude ^ -negative.magnitude).</returns>
		private static BigInteger XorDiffSigns(BigInteger positive, BigInteger negative) {
			int resLength = System.Math.Max(negative.numberLength, positive.numberLength);
			int[] resDigits;
			int iNeg = negative.FirstNonZeroDigit;
			int iPos = positive.FirstNonZeroDigit;
			int i;
			int limit;

			if (iNeg < iPos) {
				resDigits = new int[resLength];
				i = iNeg;
				resDigits[i] = negative.Digits[i];
				limit = System.Math.Min(negative.numberLength, iPos);
				for (i++; i < limit; i++) {
					resDigits[i] = negative.Digits[i];
				}
				if (i == negative.numberLength) {
					for (; i < positive.numberLength; i++) {
						resDigits[i] = positive.Digits[i];
					}
				}
			} else if (iPos < iNeg) {
				resDigits = new int[resLength];
				i = iPos;
				resDigits[i] = -positive.Digits[i];
				limit = System.Math.Min(positive.numberLength, iNeg);
				for (i++; i < limit; i++) {
					resDigits[i] = ~positive.Digits[i];
				}
				if (i == iNeg) {
					resDigits[i] = ~(positive.Digits[i] ^ -negative.Digits[i]);
					i++;
				} else {
					for (; i < iNeg; i++) {
						resDigits[i] = -1;
					}
					for (; i < negative.numberLength; i++) {
						resDigits[i] = negative.Digits[i];
					}
				}
			} else {
				int digit;
				i = iNeg;
				digit = positive.Digits[i] ^ -negative.Digits[i];
				if (digit == 0) {
					limit = System.Math.Min(positive.numberLength, negative.numberLength);
					for (i++; i < limit && (digit = positive.Digits[i] ^ ~negative.Digits[i]) == 0; i++)
						;
					if (digit == 0) {
						for (; i < positive.numberLength && (digit = ~positive.Digits[i]) == 0; i++)
							;
						for (; i < negative.numberLength && (digit = ~negative.Digits[i]) == 0; i++)
							;
						if (digit == 0) {
							resLength = resLength + 1;
							resDigits = new int[resLength];
							resDigits[resLength - 1] = 1;

							return new BigInteger(-1, resLength, resDigits);
						}
					}
				}
				resDigits = new int[resLength];
				resDigits[i] = -digit;
				i++;
			}

			limit = System.Math.Min(negative.numberLength, positive.numberLength);
			for (; i < limit; i++) {
				resDigits[i] = ~(~negative.Digits[i] ^ positive.Digits[i]);
			}
			for (; i < positive.numberLength; i++) {
				resDigits[i] = positive.Digits[i];
			}
			for (; i < negative.numberLength; i++) {
				resDigits[i] = negative.Digits[i];
			}

			BigInteger result = new BigInteger(-1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}
	}
}
