using System;

namespace Deveel.Math {
	internal static class BitLevel {
		public static int BitLength(BigInteger val) {
			if (val.Sign == 0) {
				return 0;
			}
			int bLength = (val.numberLength << 5);
			int highDigit = val.Digits[val.numberLength - 1];

			if (val.Sign < 0) {
				int i = val.FirstNonZeroDigit;
				if (i == val.numberLength - 1) {
					highDigit--;
				}
			}
			bLength -= Utils.NumberOfLeadingZeros(highDigit);
			return bLength;
		}

		public static int BitCount(BigInteger val) {
			int bCount = 0;

			if (val.Sign == 0) {
				return 0;
			}

			int i = val.FirstNonZeroDigit;
			if (val.Sign > 0) {
				for (; i < val.numberLength; i++) {
					bCount += Utils.BitCount(val.Digits[i]);
				}
			} else {
				bCount += Utils.BitCount(-val.Digits[i]);
				for (i++; i < val.numberLength; i++) {
					bCount += Utils.BitCount(~val.Digits[i]);
				}
				bCount = (val.numberLength << 5) - bCount;
			}
			return bCount;
		}

		public static bool TestBit(BigInteger val, int n) {
			return ((val.Digits[n >> 5] & (1 << (n & 31))) != 0);
		}

		public static bool NonZeroDroppedBits(int numberOfBits, ReadOnlySpan<int> digits) {
			int intCount = numberOfBits >> 5;
			int bitCount = numberOfBits & 31;
			int i;

			for (i = 0; (i < intCount) && (digits[i] == 0); i++) {
			}
			return ((i != intCount) || (digits[i] << (32 - bitCount) != 0));
		}

		public static BigInteger ShiftLeft(BigInteger source, int count) {
			int intCount = count >> 5;
			count &= 31;
			int resLength = source.numberLength + intCount + ((count == 0) ? 0 : 1);
			int[] resDigits = new int[resLength];

			ShiftLeft(resDigits.AsSpan(), source.Digits.AsSpan(), intCount, count);
			var result = new BigInteger(source.Sign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		public static void InplaceShiftLeft(BigInteger val, int count) {
			int intCount = count >> 5;
			val.numberLength += intCount
								+ (Utils.NumberOfLeadingZeros(val.Digits[val.numberLength - 1])
								   - (count & 31) >= 0
										? 0
										: 1);
			ShiftLeft(val.Digits.AsSpan(), val.Digits.AsSpan(), intCount, count & 31);
			val.CutOffLeadingZeroes();
			val.UnCache();
		}

		public static void ShiftLeft(Span<int> result, ReadOnlySpan<int> source, int intCount, int count) {
			if (count == 0) {
				source.Slice(0, result.Length - intCount).CopyTo(result.Slice(intCount));
			} else {
				int rightShiftCount = 32 - count;

				result[result.Length - 1] = 0;
				for (int i = result.Length - 1; i > intCount; i--) {
					result[i] |= (int)((uint)source[i - intCount - 1] >> rightShiftCount);
					result[i - 1] = source[i - intCount - 1] << count;
				}
			}

			for (int i = 0; i < intCount; i++) {
				result[i] = 0;
			}
		}

		public static void ShiftLeftOneBit(Span<int> result, ReadOnlySpan<int> source, int srcLen) {
			int carry = 0;
			for (int i = 0; i < srcLen; i++) {
				int val = source[i];
				result[i] = (val << 1) | carry;
				carry = (int)((uint)val >> 31);
			}
			if (carry != 0) {
				result[srcLen] = carry;
			}
		}

		public static BigInteger ShiftLeftOneBit(BigInteger source) {
			int srcLen = source.numberLength;
			int resLen = srcLen + 1;
			int[] resDigits = new int[resLen];
			ShiftLeftOneBit(resDigits.AsSpan(), source.Digits.AsSpan(0, srcLen), srcLen);
			BigInteger result = new BigInteger(source.Sign, resLen, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		public static BigInteger ShiftRight(BigInteger source, int count) {
			int intCount = count >> 5;
			count &= 31;
			if (intCount >= source.numberLength) {
				return ((source.Sign < 0) ? BigInteger.MinusOne : BigInteger.Zero);
			}
			int i;
			int resLength = source.numberLength - intCount;
			int[] resDigits = new int[resLength + 1];

			ShiftRight(resDigits.AsSpan(), resLength, source.Digits.AsSpan(), intCount, count);
			if (source.Sign < 0) {
				for (i = 0; (i < intCount) && (source.Digits[i] == 0); i++) {
				}
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

		public static void InplaceShiftRight(BigInteger val, int count) {
			int sign = val.Sign;
			if (count == 0 || val.Sign == 0)
				return;
			int intCount = count >> 5;
			val.numberLength -= intCount;
			if (!ShiftRight(val.Digits.AsSpan(), val.numberLength, val.Digits.AsSpan(), intCount, count & 31)
				&& sign < 0) {
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

		public static bool ShiftRight(Span<int> result, int resultLen, ReadOnlySpan<int> source, int intCount, int count) {
			int i;
			bool allZero = true;
			for (i = 0; i < intCount; i++)
				allZero &= source[i] == 0;
			if (count == 0) {
				source.Slice(intCount, resultLen).CopyTo(result);
				i = resultLen;
			} else {
				int leftShiftCount = 32 - count;

				allZero &= (source[i] << leftShiftCount) == 0;
				for (i = 0; i < resultLen - 1; i++) {
					result[i] = (int)((uint)source[i + intCount] >> count) |
								(source[i + intCount + 1] << leftShiftCount);
				}
				result[i] = (int)((uint)source[i + intCount] >> count);
				i++;
			}

			return allZero;
		}

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
				resDigits[intCount] ^= bitNumber;
			}

			var result = new BigInteger(resSign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}
	}
}
