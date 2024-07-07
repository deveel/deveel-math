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
 * The library implements some logical operations over {@code BigInteger}. The
 * operations provided are listed below.
 * <ul type="circle">
 * <li>not</li>
 * <li>and</li>
 * <li>andNot</li>
 * <li>or</li>
 * <li>xor</li>
 * </ul>
 */
	static class Logical {

		/** @see BigInteger#not() */

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
				// ~val = -val + 1
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
				// Here a carry 1 was generated
			} else {// (val.sign < 0)
				// ~val = -val - 1
				for (i = 0; val.Digits[i] == 0; i++) {
					resDigits[i] = -1;
				}
				// Here a borrow -1 was generated
			}
			// Now, the carry/borrow can be absorbed
			resDigits[i] = val.Digits[i] + val.Sign;
			// Copying the remaining unchanged digit
			for (i++; i < val.numberLength; i++) {
				resDigits[i] = val.Digits[i];
			}
			return new BigInteger(-val.Sign, i, resDigits);
		}

		/** @see BigInteger#and(BigInteger) */

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

		/** @return sign = 1, magnitude = val.magnitude & that.magnitude*/
		private static BigInteger AndPositive(BigInteger val, BigInteger that) {
			// PRE: both arguments are positive
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

		/** @return sign = positive.magnitude & magnitude = -negative.magnitude */
		private static BigInteger AndDiffSigns(BigInteger positive, BigInteger negative) {
			// PRE: positive is positive and negative is negative
			int iPos = positive.FirstNonZeroDigit;
			int iNeg = negative.FirstNonZeroDigit;

			// Look if the trailing zeros of the negative will "blank" all
			// the positive digits
			if (iNeg >= positive.numberLength) {
				return BigInteger.Zero;
			}
			int resLength = positive.numberLength;
			int[] resDigits = new int[resLength];

			// Must start from max(iPos, iNeg)
			int i = System.Math.Max(iPos, iNeg);
			if (i == iNeg) {
				resDigits[i] = -negative.Digits[i] & positive.Digits[i];
				i++;
			}
			int limit = System.Math.Min(negative.numberLength, positive.numberLength);
			for (; i < limit; i++) {
				resDigits[i] = ~negative.Digits[i] & positive.Digits[i];
			}
			// if the negative was shorter must copy the remaining digits
			// from positive
			if (i >= negative.numberLength) {
				for (; i < positive.numberLength; i++) {
					resDigits[i] = positive.Digits[i];
				}
			} // else positive ended and must "copy" virtual 0's, do nothing then

			BigInteger result = new BigInteger(1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/** @return sign = -1, magnitude = -(-longer.magnitude & -shorter.magnitude)*/
		private static BigInteger AndNegative(BigInteger longer, BigInteger shorter) {
			// PRE: longer and shorter are negative
			// PRE: longer has at least as many digits as shorter
			int iLonger = longer.FirstNonZeroDigit;
			int iShorter = shorter.FirstNonZeroDigit;

			// Does shorter matter?
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
					;  // digit = ~longer.digits[i] & ~shorter.digits[i]
				if (digit == 0) {
					// shorter has only the remaining virtual sign bits
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
				// resDigits[i] = ~(~longer.digits[i] & ~shorter.digits[i];)
				resDigits[i] = longer.Digits[i] | shorter.Digits[i];
			}
			// shorter has only the remaining virtual sign bits
			for (; i < longer.numberLength; i++) {
				resDigits[i] = longer.Digits[i];
			}

			BigInteger result = new BigInteger(-1, resLength, resDigits);
			return result;
		}

		/** @see BigInteger#andNot(BigInteger) */

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

			//if val == that, return 0

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

		/** @return sign = 1, magnitude = val.magnitude & ~that.magnitude*/
		private static BigInteger AndNotPositive(BigInteger val, BigInteger that) {
			// PRE: both arguments are positive
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

		/** @return sign = 1, magnitude = positive.magnitude & ~(-negative.magnitude)*/
		private static BigInteger AndNotPositiveNegative(BigInteger positive, BigInteger negative) {
			// PRE: positive > 0 && negative < 0
			int iNeg = negative.FirstNonZeroDigit;
			int iPos = positive.FirstNonZeroDigit;

			if (iNeg >= positive.numberLength) {
				return positive;
			}

			int resLength = System.Math.Min(positive.numberLength, negative.numberLength);
			int[] resDigits = new int[resLength];

			// Always start from first non zero of positive
			int i = iPos;
			for (; i < iNeg; i++) {
				// resDigits[i] = positive.digits[i] & -1 (~0)
				resDigits[i] = positive.Digits[i];
			}
			if (i == iNeg) {
				resDigits[i] = positive.Digits[i] & (negative.Digits[i] - 1);
				i++;
			}
			for (; i < resLength; i++) {
				// resDigits[i] = positive.digits[i] & ~(~negative.digits[i]);
				resDigits[i] = positive.Digits[i] & negative.Digits[i];
			}

			BigInteger result = new BigInteger(1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/** @return sign = -1, magnitude = -(-negative.magnitude & ~positive.magnitude)*/
		private static BigInteger AndNotNegativePositive(BigInteger negative, BigInteger positive) {
			// PRE: negative < 0 && positive > 0
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
					// 1st case:  resDigits [i] = -(-negative.digits[i] & (~0))
					// otherwise: resDigits[i] = ~(~negative.digits[i] & ~0)  ;
					resDigits[i] = negative.Digits[i];
				}
				if (i == negative.numberLength) {
					for (i = iPos; i < positive.numberLength; i++) {
						// resDigits[i] = ~(~positive.digits[i] & -1);
						resDigits[i] = positive.Digits[i];
					}
				}
			} else {
				digit = -negative.Digits[i] & ~positive.Digits[i];
				if (digit == 0) {
					limit = System.Math.Min(positive.numberLength, negative.numberLength);
					for (i++; i < limit && (digit = ~(negative.Digits[i] | positive.Digits[i])) == 0; i++)
						; // digit = ~negative.digits[i] & ~positive.digits[i]
					if (digit == 0) {
						// the shorter has only the remaining virtual sign bits
						for (; i < positive.numberLength && (digit = ~positive.Digits[i]) == 0; i++)
							; // digit = -1 & ~positive.digits[i]
						for (; i < negative.numberLength && (digit = ~negative.Digits[i]) == 0; i++)
							; // digit = ~negative.digits[i] & ~0
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
				//resDigits[i] = ~(~negative.digits[i] & ~positive.digits[i]);
				resDigits[i] = negative.Digits[i] | positive.Digits[i];
			}
			// Actually one of the next two cycles will be executed
			for (; i < negative.numberLength; i++) {
				resDigits[i] = negative.Digits[i];
			}
			for (; i < positive.numberLength; i++) {
				resDigits[i] = positive.Digits[i];
			}

			BigInteger result = new BigInteger(-1, resLength, resDigits);
			return result;
		}

		/** @return sign = 1, magnitude = -val.magnitude & ~(-that.magnitude)*/
		private static BigInteger AndNotNegative(BigInteger val, BigInteger that) {
			// PRE: val < 0 && that < 0
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
				// resDigits[i] = -val.digits[i] & -1;
				resDigits[i] = -val.Digits[i];
				limit = System.Math.Min(val.numberLength, iThat);
				for (i++; i < limit; i++) {
					// resDigits[i] = ~val.digits[i] & -1;
					resDigits[i] = ~val.Digits[i];
				}
				if (i == val.numberLength) {
					for (; i < iThat; i++) {
						// resDigits[i] = -1 & -1;
						resDigits[i] = -1;
					}
					// resDigits[i] = -1 & ~-that.digits[i];
					resDigits[i] = that.Digits[i] - 1;
				} else {
					// resDigits[i] = ~val.digits[i] & ~-that.digits[i];
					resDigits[i] = ~val.Digits[i] & (that.Digits[i] - 1);
				}
			} else if (iThat < iVal) {
				// resDigits[i] = -val.digits[i] & ~~that.digits[i];
				resDigits[i] = -val.Digits[i] & that.Digits[i];
			} else {
				// resDigits[i] = -val.digits[i] & ~-that.digits[i];
				resDigits[i] = -val.Digits[i] & (that.Digits[i] - 1);
			}

			limit = System.Math.Min(val.numberLength, that.numberLength);
			for (i++; i < limit; i++) {
				// resDigits[i] = ~val.digits[i] & ~~that.digits[i];
				resDigits[i] = ~val.Digits[i] & that.Digits[i];
			}
			for (; i < that.numberLength; i++) {
				// resDigits[i] = -1 & ~~that.digits[i];
				resDigits[i] = that.Digits[i];
			}

			BigInteger result = new BigInteger(1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/** @see BigInteger#or(BigInteger) */

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

		/** @return sign = 1, magnitude = longer.magnitude | shorter.magnitude*/
		private static BigInteger OrPositive(BigInteger longer, BigInteger shorter) {
			// PRE: longer and shorter are positive;
			// PRE: longer has at least as many digits as shorter
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

		/** @return sign = -1, magnitude = -(-val.magnitude | -that.magnitude) */
		private static BigInteger OrNegative(BigInteger val, BigInteger that) {
			// PRE: val and that are negative;
			// PRE: val has at least as many trailing zeros digits as that
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

			//Looking for the first non-zero digit of the result
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

		/** @return sign = -1, magnitude = -(positive.magnitude | -negative.magnitude) */
		private static BigInteger OrDiffSigns(BigInteger positive, BigInteger negative) {
			// Jumping over the least significant zero bits
			int iNeg = negative.FirstNonZeroDigit;
			int iPos = positive.FirstNonZeroDigit;
			int i;
			int limit;

			// Look if the trailing zeros of the positive will "copy" all
			// the negative digits
			if (iPos >= negative.numberLength) {
				return negative;
			}
			int resLength = negative.numberLength;
			int[] resDigits = new int[resLength];

			if (iNeg < iPos) {
				// We know for sure that this will
				// be the first non zero digit in the result
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
					// resDigits[i] = ~(-negative.digits[i] | 0);
					resDigits[i] = negative.Digits[i] - 1;
				}
				i++;
			} else {// iNeg == iPos
				// Applying two complement to negative and to result
				i = iPos;
				resDigits[i] = -(-negative.Digits[i] | positive.Digits[i]);
				i++;
			}
			limit = System.Math.Min(negative.numberLength, positive.numberLength);
			for (; i < limit; i++) {
				// Applying two complement to negative and to result
				// resDigits[i] = ~(~negative.digits[i] | positive.digits[i] );
				resDigits[i] = negative.Digits[i] & ~positive.Digits[i];
			}
			for (; i < negative.numberLength; i++) {
				resDigits[i] = negative.Digits[i];
			}

			BigInteger result = new BigInteger(-1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/** @see BigInteger#xor(BigInteger) */

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

		/** @return sign = 0, magnitude = longer.magnitude | shorter.magnitude */
		private static BigInteger XorPositive(BigInteger longer, BigInteger shorter) {
			// PRE: longer and shorter are positive;
			// PRE: longer has at least as many digits as shorter
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

		/** @return sign = 0, magnitude = -val.magnitude ^ -that.magnitude */
		private static BigInteger XorNegative(BigInteger val, BigInteger that) {
			// PRE: val and that are negative
			// PRE: val has at least as many trailing zero digits as that
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
				// Remains digits in that?
				if (i == that.numberLength) {
					//Jumping over the remaining zero to the first non one
					for (; i < iVal; i++) {
						//resDigits[i] = 0 ^ -1;
						resDigits[i] = -1;
					}
					//resDigits[i] = -val.digits[i] ^ -1;
					resDigits[i] = val.Digits[i] - 1;
				} else {
					resDigits[i] = -val.Digits[i] ^ ~that.Digits[i];
				}
			}

			limit = System.Math.Min(val.numberLength, that.numberLength);
			//Perform ^ between that al val until that ends
			for (i++; i < limit; i++) {
				//resDigits[i] = ~val.digits[i] ^ ~that.digits[i];
				resDigits[i] = val.Digits[i] ^ that.Digits[i];
			}
			//Perform ^ between val digits and -1 until val ends
			for (; i < val.numberLength; i++) {
				//resDigits[i] = ~val.digits[i] ^ -1  ;
				resDigits[i] = val.Digits[i];
			}
			for (; i < that.numberLength; i++) {
				//resDigits[i] = -1 ^ ~that.digits[i] ;
				resDigits[i] = that.Digits[i];
			}

			BigInteger result = new BigInteger(1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/** @return sign = 1, magnitude = -(positive.magnitude ^ -negative.magnitude)*/
		private static BigInteger XorDiffSigns(BigInteger positive, BigInteger negative) {
			int resLength = System.Math.Max(negative.numberLength, positive.numberLength);
			int[] resDigits;
			int iNeg = negative.FirstNonZeroDigit;
			int iPos = positive.FirstNonZeroDigit;
			int i;
			int limit;

			//The first
			if (iNeg < iPos) {
				resDigits = new int[resLength];
				i = iNeg;
				//resDigits[i] = -(-negative.digits[i]);
				resDigits[i] = negative.Digits[i];
				limit = System.Math.Min(negative.numberLength, iPos);
				//Skip the positive digits while they are zeros
				for (i++; i < limit; i++) {
					//resDigits[i] = ~(~negative.digits[i]);
					resDigits[i] = negative.Digits[i];
				}
				//if the negative has no more elements, must fill the
				//result with the remaining digits of the positive
				if (i == negative.numberLength) {
					for (; i < positive.numberLength; i++) {
						//resDigits[i] = ~(positive.digits[i] ^ -1) -> ~(~positive.digits[i])
						resDigits[i] = positive.Digits[i];
					}
				}
			} else if (iPos < iNeg) {
				resDigits = new int[resLength];
				i = iPos;
				//Applying two complement to the first non-zero digit of the result
				resDigits[i] = -positive.Digits[i];
				limit = System.Math.Min(positive.numberLength, iNeg);
				for (i++; i < limit; i++) {
					//Continue applying two complement the result
					resDigits[i] = ~positive.Digits[i];
				}
				//When the first non-zero digit of the negative is reached, must apply
				//two complement (arithmetic negation) to it, and then operate
				if (i == iNeg) {
					resDigits[i] = ~(positive.Digits[i] ^ -negative.Digits[i]);
					i++;
				} else {
					//if the positive has no more elements must fill the remaining digits with
					//the negative ones
					for (; i < iNeg; i++) {
						// resDigits[i] = ~(0 ^ 0)
						resDigits[i] = -1;
					}
					for (; i < negative.numberLength; i++) {
						//resDigits[i] = ~(~negative.digits[i] ^ 0)
						resDigits[i] = negative.Digits[i];
					}
				}
			} else {
				int digit;
				//The first non-zero digit of the positive and negative are the same
				i = iNeg;
				digit = positive.Digits[i] ^ -negative.Digits[i];
				if (digit == 0) {
					limit = System.Math.Min(positive.numberLength, negative.numberLength);
					for (i++; i < limit && (digit = positive.Digits[i] ^ ~negative.Digits[i]) == 0; i++)
						;
					if (digit == 0) {
						// shorter has only the remaining virtual sign bits
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
				// resDigits[i] = ~(positive.digits[i] ^ -1)
				resDigits[i] = positive.Digits[i];
			}
			for (; i < negative.numberLength; i++) {
				// resDigits[i] = ~(0 ^ ~negative.digits[i])
				resDigits[i] = negative.Digits[i];
			}

			BigInteger result = new BigInteger(-1, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}
	}
}