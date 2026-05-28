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
using System.Buffers;

namespace Deveel.Math {
	/// <summary>
	/// Static library that provides all operations related with division and modular
	/// arithmetic for <see cref="BigInteger"/>. Some methods are provided in both mutable
	/// and immutable way. The operations include:
	/// <list type="bullet">
	/// <item><description><b>Division</b> - <see cref="BigInteger"/> division and remainder by <see cref="BigInteger"/> and <c>int</c>.</description></item>
	/// <item><description>GCD between <see cref="BigInteger"/> numbers.</description></item>
	/// <item><description><b>Modular arithmetic</b> - Modular exponentiation and modular inverse.</description></item>
	/// </list>
	/// </summary>
	internal static class Division {
		private const int StackAllocMax = 256;
		/// <summary>
		/// Divides the array 'a' by the array 'b' and gets the quotient and the
		/// remainder. Implements the Knuth's division algorithm (see D. Knuth,
		/// The Art of Computer Programming, vol. 2, Steps D1-D8).
		/// </summary>
		/// <param name="quot">The quotient array (can be <c>null</c>).</param>
		/// <param name="quotLength">The quotient's length.</param>
		/// <param name="a">The dividend array.</param>
		/// <param name="aLength">The dividend's length.</param>
		/// <param name="b">The divisor array.</param>
		/// <param name="bLength">The divisor's length.</param>
		/// <returns>The remainder array.</returns>
		public static int[] Divide(Span<int> quot, int quotLength, ReadOnlySpan<int> a, int aLength, ReadOnlySpan<int> b, int bLength) {
			int totalLen = aLength + bLength + 2;
			int[]? poolArray = totalLen > StackAllocMax ? ArrayPool<int>.Shared.Rent(totalLen) : null;
			Span<int> workBuffer = totalLen <= StackAllocMax
				? stackalloc int[totalLen]
				: poolArray.AsSpan(0, totalLen);

			try {
				Span<int> normA = workBuffer.Slice(0, aLength + 1);
				Span<int> normB = workBuffer.Slice(aLength + 1, bLength + 1);
				int normBLength = bLength;
				int divisorShift = Utils.NumberOfLeadingZeros(b[bLength - 1]);
				if (divisorShift != 0) {
					BitLevel.ShiftLeft(normB, b, 0, divisorShift);
					BitLevel.ShiftLeft(normA, a, 0, divisorShift);
				} else {
					a.Slice(0, aLength).CopyTo(normA);
					b.Slice(0, bLength).CopyTo(normB);
				}
				int firstDivisorDigit = normB[normBLength - 1];
				int i = quotLength - 1;
				int j = aLength;

				while (i >= 0) {
					int guessDigit = 0;
					if (normA[j] == firstDivisorDigit) {
						guessDigit = -1;
					} else {
						long product = (((normA[j] & 0xffffffffL) << 32) + (normA[j - 1] & 0xffffffffL));
						long res = Division.DivideLongByInt(product, firstDivisorDigit);
						guessDigit = (int) res;
						int rem = (int) (res >> 32);
						if (guessDigit != 0) {
							long leftHand = 0;
							long rightHand = 0;
							bool rOverflowed = false;
							guessDigit++;
							do {
								guessDigit--;
								if (rOverflowed)
									break;
								leftHand = (guessDigit & 0xffffffffL)
								           *(normB[normBLength - 2] & 0xffffffffL);
								rightHand = ((long) rem << 32)
								            + (normA[j - 2] & 0xffffffffL);
								long longR = (rem & 0xffffffffL)
								             + (firstDivisorDigit & 0xffffffffL);
								if (Utils.NumberOfLeadingZeros((int) Utils.URShift(longR, 32)) < 32)
									rOverflowed = true;
								else
									rem = (int) longR;
							} while ((leftHand ^ Int64.MinValue) > (rightHand ^ Int64.MinValue));

						}
					}
					if (guessDigit != 0) {
						int borrow = Division.MultiplyAndSubtract(normA, j - normBLength, normB, normBLength, guessDigit);
						if (borrow != 0) {
							guessDigit--;
							long carry = 0;
							for (int k = 0; k < normBLength; k++) {
								carry += (normA[j - normBLength + k] & 0xffffffffL)
								         + (normB[k] & 0xffffffffL);
								normA[j - normBLength + k] = (int) carry;
								carry = Utils.URShift(carry, 32);
							}
						}
					}
					if (quot != null)
						quot[i] = guessDigit;
					j--;
					i--;
				}

				int[] result = new int[bLength];
				if (divisorShift != 0) {
					Span<int> tempRem = stackalloc int[bLength];
					BitLevel.ShiftRight(tempRem, bLength, normA, 0, divisorShift);
					tempRem.Slice(0, bLength).CopyTo(result);
				} else {
					normA.Slice(0, bLength).CopyTo(result);
				}
				return result;
			} finally {
				if (poolArray != null)
					ArrayPool<int>.Shared.Return(poolArray);
			}
		}

		/// <summary>
		/// Divides an array by an integer value. Implements the Knuth's division algorithm.
		/// </summary>
		/// <param name="dest">The quotient array.</param>
		/// <param name="src">The dividend array.</param>
		/// <param name="srcLength">The length of the dividend.</param>
		/// <param name="divisor">The divisor.</param>
		/// <returns>The remainder.</returns>
		public static int DivideArrayByInt(Span<int> dest, ReadOnlySpan<int> src, int srcLength, int divisor) {
			long rem = 0;
			long bLong = divisor & 0xffffffffL;

			for (int i = srcLength - 1; i >= 0; i--) {
				long temp = (rem << 32) | (src[i] & 0xffffffffL);
				long quot;
				if (temp >= 0) {
					quot = (temp/bLong);
					rem = (temp%bLong);
				} else {
					long aPos = Utils.URShift(temp, 1);
					long bPos = Utils.URShift(divisor, 1);
					quot = aPos/bPos;
					rem = aPos%bPos;
					rem = (rem << 1) + (temp & 1);
					if ((divisor & 1) != 0) {
						if (quot <= rem)
							rem -= quot;
						else {
							if (quot - rem <= bLong) {
								rem += bLong - quot;
								quot -= 1;
							} else {
								rem += (bLong << 1) - quot;
								quot -= 2;
							}
						}
					}
				}
				dest[i] = (int) (quot & 0xffffffffL);
			}
			return (int) rem;
		}

		/// <summary>
		/// Divides an array by an integer value and returns only the remainder.
		/// Implements the Knuth's division algorithm.
		/// </summary>
		/// <param name="src">The dividend array.</param>
		/// <param name="srcLength">The length of the dividend.</param>
		/// <param name="divisor">The divisor.</param>
		/// <returns>The remainder.</returns>
		public static int RemainderArrayByInt(ReadOnlySpan<int> src, int srcLength, int divisor) {
			long result = 0;

			for (int i = srcLength - 1; i >= 0; i--) {
				long temp = (result << 32) + (src[i] & 0xffffffffL);
				long res = DivideLongByInt(temp, divisor);
				result = (int) (res >> 32);
			}
			return (int) result;
		}

		/// <summary>
		/// Divides a <see cref="BigInteger"/> by a signed <c>int</c> and returns the remainder.
		/// </summary>
		/// <param name="dividend">The BigInteger to be divided (must be non-negative).</param>
		/// <param name="divisor">A signed int.</param>
		/// <returns><c>dividend % divisor</c>.</returns>
		public static int Remainder(BigInteger dividend, int divisor) {
			return RemainderArrayByInt(dividend.Digits, dividend.numberLength, divisor);
		}

		/// <summary>
		/// Divides an unsigned long <paramref name="a"/> by an unsigned int <paramref name="b"/>.
		/// It is supposed that the most significant bit of <paramref name="b"/> is set to 1, i.e. b &lt; 0.
		/// </summary>
		/// <param name="a">The dividend.</param>
		/// <param name="b">The divisor.</param>
		/// <returns>
		/// A long value containing the unsigned integer remainder in the left half
		/// and the unsigned integer quotient in the right half.
		/// </returns>
		private static long DivideLongByInt(long a, int b) {
			long quot;
			long rem;
			long bLong = b & 0xffffffffL;

			if (a >= 0) {
				quot = (a/bLong);
				rem = (a%bLong);
			} else {
				long aPos = Utils.URShift(a, 1);
				long bPos = Utils.URShift(b, 1);
				quot = aPos/bPos;
				rem = aPos%bPos;
				rem = (rem << 1) + (a & 1);
				if ((b & 1) != 0) {
					if (quot <= rem)
						rem -= quot;
					else {
						if (quot - rem <= bLong) {
							rem += bLong - quot;
							quot -= 1;
						} else {
							rem += (bLong << 1) - quot;
							quot -= 2;
						}
					}
				}
			}
			return (rem << 32) | (quot & 0xffffffffL);
		}

		/// <summary>
		/// Computes the quotient and the remainder after a division by an <c>int</c> number.
		/// </summary>
		/// <param name="val">The dividend.</param>
		/// <param name="divisor">The divisor.</param>
		/// <param name="divisorSign">The sign of the divisor.</param>
		/// <returns>An array of the form <c>[quotient, remainder]</c>.</returns>
		/// <example>
		/// <code>
		/// BigInteger value = new BigInteger(100);
		/// BigInteger[] result = Division.DivideAndRemainderByInteger(value, 7, 1);
		/// // result[0] == 14, result[1] == 2
		/// </code>
		/// </example>
		public static BigInteger[] DivideAndRemainderByInteger(BigInteger val, int divisor, int divisorSign) {
			ReadOnlySpan<int> valDigits = val.Digits.AsSpan(0, val.numberLength);
			int valLen = val.numberLength;
			int valSign = val.Sign;
			if (valLen == 1) {
				long a = (valDigits[0] & 0xffffffffL);
				long b = (divisor & 0xffffffffL);
				long quo = a/b;
				long rem = a%b;
				if (valSign != divisorSign)
					quo = -quo;
				if (valSign < 0)
					rem = -rem;
				return new BigInteger[] {
					BigInteger.FromInt64(quo),
					BigInteger.FromInt64(rem)
				};
			}
			int quotientLength = valLen;
			int quotientSign = ((valSign == divisorSign) ? 1 : -1);
			int[]? quotientArray = null;
			Span<int> quotientDigits = quotientLength <= StackAllocMax
				? stackalloc int[quotientLength]
				: (quotientArray = ArrayPool<int>.Shared.Rent(quotientLength));
			quotientDigits = quotientDigits.Slice(0, quotientLength);

			try {
				int remainder = Division.DivideArrayByInt(
					quotientDigits,
					valDigits,
					valLen,
					divisor);
				BigInteger result0 = new BigInteger(quotientSign,
					quotientLength,
					quotientDigits);
				BigInteger result1 = BigInteger.FromInt64(valSign * (long)remainder);
				result0 = result0.WithCutOffLeadingZeroes();
				return new BigInteger[] {result0, result1};
			} finally {
				if (quotientArray != null)
					ArrayPool<int>.Shared.Return(quotientArray);
			}
		}

		/// <summary>
		/// Multiplies an array by int and subtracts it from a subarray of another array.
		/// </summary>
		/// <param name="a">The array to subtract from.</param>
		/// <param name="start">The start element of the subarray of <paramref name="a"/>.</param>
		/// <param name="b">The array to be multiplied and subtracted.</param>
		/// <param name="bLen">The length of <paramref name="b"/>.</param>
		/// <param name="c">The multiplier of <paramref name="b"/>.</param>
		/// <returns>The carry element of subtraction.</returns>
		public static int MultiplyAndSubtract(Span<int> a, int start, ReadOnlySpan<int> b, int bLen, int c) {
			long carry0 = 0;
			long carry1 = 0;

			for (int i = 0; i < bLen; i++) {
				carry0 = Multiplication.UnsignedMultAddAdd(b[i], c, (int) carry0, 0);
				carry1 = (a[start + i] & 0xffffffffL) - (carry0 & 0xffffffffL) + carry1;
				a[start + i] = (int) carry1;
				carry1 >>= 32;
				carry0 = Utils.URShift(carry0, 32);
			}

			carry1 = (a[start + bLen] & 0xffffffffL) - carry0 + carry1;
			a[start + bLen] = (int) carry1;
			return (int) (carry1 >> 32);
		}

		/// <summary>
		/// Returns the greatest common divisor of <paramref name="op1"/> and <paramref name="op2"/>.
		/// Both must be greater than zero.
		/// </summary>
		/// <param name="op1">A positive <see cref="BigInteger"/>.</param>
		/// <param name="op2">A positive <see cref="BigInteger"/>.</param>
		/// <returns><c>GCD(op1, op2)</c>.</returns>
		/// <example>
		/// <code>
		/// BigInteger a = new BigInteger(48);
		/// BigInteger b = new BigInteger(18);
		/// BigInteger gcd = Division.GcdBinary(a, b);
		/// // gcd == 6
		/// </code>
		/// </example>
		public static BigInteger GcdBinary(BigInteger op1, BigInteger op2) {
			int lsb1 = op1.LowestSetBit;
			int lsb2 = op2.LowestSetBit;
			int pow2Count = System.Math.Min(lsb1, lsb2);

			op1 = BitLevel.InplaceShiftRight(op1, lsb1);
			op2 = BitLevel.InplaceShiftRight(op2, lsb2);

			BigInteger swap;
			if (op1.CompareTo(op2) == BigInteger.GREATER) {
				swap = op1;
				op1 = op2;
				op2 = swap;
			}

			do {
				if ((op2.numberLength == 1)
				    || ((op2.numberLength == 2) && (op2.Digits[1] > 0))) {
					op2 = BigInteger.FromInt64(Division.GcdBinary(op1.ToInt64(),
						op2.ToInt64()));
					break;
				}

				if (op2.numberLength > op1.numberLength*1.2) {
					op2 = BigMath.Remainder(op2, op1);
					if (op2.Sign != 0)
						op2 = BitLevel.InplaceShiftRight(op2, op2.LowestSetBit);
				} else {
					do {
						op2 = Elementary.inplaceSubtract(op2, op1);
						op2 = BitLevel.InplaceShiftRight(op2, op2.LowestSetBit);
					} while (op2.CompareTo(op1) >= BigInteger.EQUALS);
				}
				swap = op2;
				op2 = op1;
				op1 = swap;
			} while (op1.Sign != 0);
			return op2 << pow2Count;
		}

		/// <summary>
		/// Performs the same as <see cref="GcdBinary(BigInteger, BigInteger)"/> but
		/// with numbers of 63 bits, represented in positive values of <see cref="long"/> type.
		/// </summary>
		/// <param name="op1">A positive number.</param>
		/// <param name="op2">A positive number.</param>
		/// <returns><c>GCD(op1, op2)</c>.</returns>
		public static long GcdBinary(long op1, long op2) {
			int lsb1 = Utils.NumberOfTrailingZeros(op1);
			int lsb2 = Utils.NumberOfTrailingZeros(op2);
			int pow2Count = System.Math.Min(lsb1, lsb2);

			if (lsb1 != 0)
				op1 = Utils.URShift(op1, lsb1);
			if (lsb2 != 0)
				op2 = Utils.URShift(op2, lsb2);
			do {
				if (op1 >= op2) {
					op1 -= op2;
					op1 = Utils.URShift(op1, Utils.NumberOfTrailingZeros(op1));
				} else {
					op2 -= op1;
					op2 = Utils.URShift(op2, Utils.NumberOfTrailingZeros(op2));
				}
			} while (op1 != 0);
			return (op2 << pow2Count);
		}

		/// <summary>
		/// Calculates <c>a.modInverse(p)</c> based on Savas, E; Koc, C "The Montgomery
		/// Modular Inverse - Revised".
		/// </summary>
		/// <param name="a">The number to invert.</param>
		/// <param name="p">The modulus.</param>
		/// <returns><c>a^(-1) mod p</c>.</returns>
		public static BigInteger ModInverseMontgomery(BigInteger a, BigInteger p) {
			if (a.sign == 0) {
				throw new ArithmeticException(Messages.math19);
			}

			// Use extended Euclidean algorithm for correctness
			BigInteger oldR = p, r = a;
			BigInteger oldS = BigInteger.Zero, s = BigInteger.One;
			
			while (r.sign != 0) {
				BigInteger quotient = oldR / r;
				BigInteger temp = r;
				r = oldR - quotient * r;
				oldR = temp;
				
				temp = s;
				s = oldS - quotient * s;
				oldS = temp;
			}
			
			// oldR is the GCD, should be 1 for inverse to exist
			if (oldR.numberLength > 1 || oldR.digits[0] != 1) {
				throw new ArithmeticException(Messages.math19);
			}
			
			// oldS is the inverse, make it positive
			if (oldS.sign < 0) {
				oldS = oldS + p;
			}
			
			return oldS;
		}

		/// <summary>
		/// Calculate the first digit of the inverse for Montgomery multiplication.
		/// </summary>
		/// <param name="a">The modulus.</param>
		/// <returns>The first digit of the inverse.</returns>
		private static int CalcN(BigInteger a) {
			long m0 = a.Digits[0] & 0xFFFFFFFFL;
			long n2 = 1L;
			long powerOfTwo = 2L;
			do {
				if (((m0*n2) & powerOfTwo) != 0)
					n2 |= powerOfTwo;
				powerOfTwo <<= 1;
			} while (powerOfTwo < 0x100000000L);
			n2 = -n2;
			return (int) (n2 & 0xFFFFFFFFL);
		}

		/// <summary>
		/// Checks whether <paramref name="bi"/> == abs(2^<paramref name="exp"/>).
		/// </summary>
		/// <param name="bi">The big integer to test.</param>
		/// <param name="exp">The exponent.</param>
		/// <returns><c>true</c> if <paramref name="bi"/> equals 2^<paramref name="exp"/>.</returns>
		public static bool IsPowerOfTwo(BigInteger bi, int exp) {
			bool result = false;
			result = (exp >> 5 == bi.numberLength - 1)
			         && (bi.Digits[bi.numberLength - 1] == 1 << (exp & 31));
			if (result) {
				for (int i = 0; result && i < bi.numberLength - 1; i++)
					result = bi.Digits[i] == 0;
			}
			return result;
		}

		/// <summary>
		/// Calculates how many iterations of Lorencz's algorithm would perform
		/// the same operation.
		/// </summary>
		/// <param name="bi">The big integer.</param>
		/// <param name="n">The bit length.</param>
		/// <returns>The number of iterations.</returns>
		private static int HowManyIterations(BigInteger bi, int n) {
			int i = n - 1;
			if (bi.Sign > 0) {
				while (!BigInteger.TestBit(bi, i))
					i--;
				return n - 1 - i;
			} else {
				while (BigInteger.TestBit(bi, i))
					i--;
				return n - 1 - System.Math.Max(i, bi.LowestSetBit);
			}
		}

		/// <summary>
		/// Computes the modular inverse using the Lórencz algorithm.
		/// Based on "New Algorithm for Classical Modular Inverse" Róbert Lórencz,
		/// LNCS 2523 (2002).
		/// </summary>
		/// <param name="a">The number to invert (coprime with <paramref name="modulo"/>).</param>
		/// <param name="modulo">The modulus.</param>
		/// <returns><c>a^(-1) mod m</c>.</returns>
		private static BigInteger ModInverseLorencz(BigInteger a, BigInteger modulo) {
			int max = System.Math.Max(a.numberLength, modulo.numberLength);
			int[] uDigits = new int[max + 1];
			int[] vDigits = new int[max + 1];
			Array.Copy(modulo.Digits, 0, uDigits, 0, modulo.numberLength);
			Array.Copy(a.Digits, 0, vDigits, 0, a.numberLength);
			BigInteger u = new BigInteger(modulo.Sign,
				modulo.numberLength,
				uDigits);
			BigInteger v = new BigInteger(a.Sign, a.numberLength, vDigits);

			BigInteger r = new BigInteger(0, 1, new int[max + 1]);
			BigInteger s = new BigInteger(1, 1, new int[max + 1]);
			s.Digits[0] = 1;

			int coefU = 0, coefV = 0;
			int n = modulo.BitLength;
			int k;
			while (!IsPowerOfTwo(u, coefU) && !IsPowerOfTwo(v, coefV)) {
				k = HowManyIterations(u, n);

				if (k != 0) {
					u = BitLevel.InplaceShiftLeft(u, k);
					if (coefU >= coefV)
						r = BitLevel.InplaceShiftLeft(r, k);
					else {
						s = BitLevel.InplaceShiftRight(s, System.Math.Min(coefV - coefU, k));
						if (k - (coefV - coefU) > 0)
							r = BitLevel.InplaceShiftLeft(r, k - coefV + coefU);
					}
					coefU += k;
				}

				k = HowManyIterations(v, n);
				if (k != 0) {
					v = BitLevel.InplaceShiftLeft(v, k);
					if (coefV >= coefU)
						s = BitLevel.InplaceShiftLeft(s, k);
					else {
						r = BitLevel.InplaceShiftRight(r, System.Math.Min(coefU - coefV, k));
						if (k - (coefU - coefV) > 0)
							s = BitLevel.InplaceShiftLeft(s, k - coefU + coefV);
					}
					coefV += k;
				}

				if (u.Sign == v.Sign) {
					if (coefU <= coefV) {
						u = Elementary.completeInPlaceSubtract(u, v);
						r = Elementary.completeInPlaceSubtract(r, s);
					} else {
						v = Elementary.completeInPlaceSubtract(v, u);
						s = Elementary.completeInPlaceSubtract(s, r);
					}
				} else {
					if (coefU <= coefV) {
						u = Elementary.completeInPlaceAdd(u, v);
						r = Elementary.completeInPlaceAdd(r, s);
					} else {
						v = Elementary.completeInPlaceAdd(v, u);
						s = Elementary.completeInPlaceAdd(s, r);
					}
				}
				if (v.Sign == 0 || u.Sign == 0) {
					throw new ArithmeticException(Messages.math19);
				}
			}

			if (IsPowerOfTwo(v, coefV)) {
				r = s;
				if (v.Sign != u.Sign)
					u = -u;
			}
			if (BigInteger.TestBit(u, n)) {
				if (r.Sign < 0)
					r = -r;
				else
					r = modulo - r;
			}
			if (r.Sign < 0)
				r += modulo;

			return r;
		}

		/// <summary>
		/// Performs modular exponentiation using the Montgomery Reduction.
		/// Requires that all parameters be positive and the modulus be odd.
		/// </summary>
		/// <param name="b">The base.</param>
		/// <param name="exponent">The exponent.</param>
		/// <param name="modulus">The modulus (must be odd).</param>
		/// <returns><c>b^exponent mod modulus</c>.</returns>
		/// <example>
		/// <code>
		/// BigInteger result = Division.OddModPow(
		///     new BigInteger(4),
		///     new BigInteger(3),
		///     new BigInteger(7));
		/// // result == 1  (4^3 mod 7 = 64 mod 7 = 1)
		/// </code>
		/// </example>
		public static BigInteger OddModPow(BigInteger b,
			BigInteger exponent,
			BigInteger modulus) {
			int n = modulus.numberLength;
			int k = n << 5;
			int n2 = CalcN(modulus);

			// Fast path for single-digit modulus
			if (n == 1) {
				int modDigit = modulus.digits[0];
				int bRem = Division.RemainderArrayByInt(b.digits, b.numberLength, modDigit);
				int a2 = (int)(((long)bRem << k) % modDigit);
				// x2 = 2^k % modulus, where k = 32 for n=1
				int x2 = (int)(0x100000000UL % (uint)modDigit);
				return SquareAndMultiplyBufSingle(x2, a2, exponent, modDigit, n2);
			}

			int resSize = n + 1;
			int workSize = (n << 1) + 1;

			int[]? a2Array = ArrayPool<int>.Shared.Rent(resSize);
			int[]? x2Array = ArrayPool<int>.Shared.Rent(resSize);
			int shiftedLen = n + b.numberLength;
			int[]? workArray = ArrayPool<int>.Shared.Rent(System.Math.Max(workSize, shiftedLen));
			int[]? quotArray = ArrayPool<int>.Shared.Rent(workSize);
			try {
				Span<int> a2 = a2Array.AsSpan(0, resSize);
				Span<int> x2 = x2Array.AsSpan(0, resSize);
				Span<int> work = workArray.AsSpan(0, workArray.Length);
				Span<int> quot = quotArray.AsSpan(0, workSize);
				Span<int> modDigits = modulus.digits.AsSpan(0, n);

				// a2 = (b << k) % modulus
				work.Slice(0, shiftedLen).Clear();
				b.digits.AsSpan(0, b.numberLength).CopyTo(work.Slice(n));
				Division.Divide(quot, shiftedLen - n + 1, work.Slice(0, shiftedLen), shiftedLen, modDigits, n);
				work.Slice(0, n).CopyTo(a2);
				a2[n] = 0;

				// x2 = 2^k % modulus
				work.Slice(0, n + 1).Clear();
				work[n] = 1;
				Division.Divide(quot, 2, work.Slice(0, n + 1), n + 1, modDigits, n);
				work.Slice(0, n).CopyTo(x2);
				x2[n] = 0;

				BigInteger res = SlidingWindowBuf(x2, a2, exponent, modulus, n2);
				return res;
			} finally {
				ArrayPool<int>.Shared.Return(a2Array);
				ArrayPool<int>.Shared.Return(x2Array);
				ArrayPool<int>.Shared.Return(workArray);
				ArrayPool<int>.Shared.Return(quotArray);
			}
		}

		private static BigInteger SquareAndMultiplyBufSingle(int x2, int a2, BigInteger exponent, int modulus, int n2) {
			int res = x2;
			for (int i = exponent.BitLength - 1; i >= 0; i--) {
				res = MonProSingle(res, res, modulus, n2);
				if (BitLevel.TestBit(exponent, i))
					res = MonProSingle(res, a2, modulus, n2);
			}
			// Final Montgomery conversion
			res = MonProSingle(res, 1, modulus, n2);
			return BigInteger.FromInt64(res);
		}

		private static int MonProSingle(int a, int b, int modulus, int n2) {
			ulong ua = (uint)a;
			ulong ub = (uint)b;
			ulong um = (uint)modulus;
			ulong un2 = (uint)n2;
			ulong product = ua * ub;
			ulong mVal = (product * un2) & 0xFFFFFFFFUL;
			ulong t = product + mVal * um;
			uint result = (uint)(t >> 32);
			if (result >= (uint)um) result -= (uint)um;
			return (int)result;
		}

		private static BigInteger SquareAndMultiplyBuf(ReadOnlySpan<int> x2, ReadOnlySpan<int> a2, BigInteger exponent, BigInteger modulus, int n2) {
			int n = modulus.numberLength;
			int resSize = n + 1;
			int workSize = (n << 1) + 1;

			int[][] buffers = new int[3][];
			buffers[0] = ArrayPool<int>.Shared.Rent(resSize);
			buffers[1] = ArrayPool<int>.Shared.Rent(resSize);
			buffers[2] = ArrayPool<int>.Shared.Rent(workSize);
			try {
				int resIdx = 0;
				Span<int> res = buffers[0].AsSpan(0, resSize);
				res.Clear();
				x2.Slice(0, n).CopyTo(res);

				Span<int> a2Buf = buffers[2].AsSpan(0, resSize);
				a2Buf.Clear();
				a2.Slice(0, n).CopyTo(a2Buf);

				Span<int> work = buffers[2].AsSpan(0, workSize);

				for (int i = exponent.BitLength - 1; i >= 0; i--) {
					// Square: res = res * res
					int nextIdx = 1 - resIdx;
					Span<int> next = buffers[nextIdx].AsSpan(0, resSize);
					Multiplication.MultArraysPap(res.Slice(0, n), n, res.Slice(0, n), n, work);
					work[workSize - 1] = 0;
					MonReduction(work, modulus, n2);
					FinalSubtractionInPlace(work.Slice(0, resSize), modulus);
					work.Slice(0, resSize).CopyTo(next);
					resIdx = nextIdx;
					res = buffers[resIdx].AsSpan(0, resSize);

					if (BitLevel.TestBit(exponent, i)) {
						// Multiply: res = res * a2
						nextIdx = 1 - resIdx;
						next = buffers[nextIdx].AsSpan(0, resSize);
						Multiplication.MultArraysPap(res.Slice(0, n), n, a2Buf.Slice(0, n), n, work);
						work[workSize - 1] = 0;
						MonReduction(work, modulus, n2);
						FinalSubtractionInPlace(work.Slice(0, resSize), modulus);
						work.Slice(0, resSize).CopyTo(next);
						resIdx = nextIdx;
						res = buffers[resIdx].AsSpan(0, resSize);
					}
				}

				// Final Montgomery conversion in-place
				work.Slice(0, workSize).Clear();
				res.CopyTo(work);
				MonReduction(work, modulus, n2);
				FinalSubtractionInPlace(work.Slice(0, resSize), modulus);

				// Compute actual length and handle zero
				int actualLen = resSize;
				while (actualLen > 1 && work[actualLen - 1] == 0) actualLen--;
				int finalSign = (actualLen == 1 && work[0] == 0) ? 0 : 1;
				int[] resultDigits = new int[actualLen];
				work.Slice(0, actualLen).CopyTo(resultDigits);
				return new BigInteger(finalSign, actualLen, resultDigits);
			} finally {
				ArrayPool<int>.Shared.Return(buffers[0]);
				ArrayPool<int>.Shared.Return(buffers[1]);
				ArrayPool<int>.Shared.Return(buffers[2]);
			}
		}

		private static BigInteger SlidingWindowBuf(ReadOnlySpan<int> x2, ReadOnlySpan<int> a2, BigInteger exponent, BigInteger modulus, int n2) {
			int n = modulus.numberLength;
			int resSize = n + 1;
			int workSize = (n << 1) + 1;

			int[][] powsArrays = new int[8][];
			for (int i = 0; i < 8; i++)
				powsArrays[i] = ArrayPool<int>.Shared.Rent(resSize);

			int[]? resArray = ArrayPool<int>.Shared.Rent(resSize);
			int[]? mainWorkArray = ArrayPool<int>.Shared.Rent(workSize);

			try {
				// pows[0] = a2
				a2.CopyTo(powsArrays[0]);

				// x3 = MonPro(a2, a2)
				int[]? x3Array = ArrayPool<int>.Shared.Rent(resSize);
				int[]? preWorkArray = ArrayPool<int>.Shared.Rent(workSize);
				Span<int> work = preWorkArray.AsSpan(0, workSize);

				Multiplication.MultArraysPap(powsArrays[0].AsSpan(0, n), n, powsArrays[0].AsSpan(0, n), n, work);
				work[workSize - 1] = 0;
				MonReduction(work, modulus, n2);
				FinalSubtractionInPlace(work.Slice(0, resSize), modulus);
				work.Slice(0, resSize).CopyTo(x3Array);

				// pows[i] = MonPro(pows[i-1], x3)
				Span<int> x3Span = x3Array.AsSpan(0, resSize);
				for (int i = 1; i <= 7; i++) {
					Multiplication.MultArraysPap(powsArrays[i - 1].AsSpan(0, n), n, x3Span.Slice(0, n), n, work);
					work[workSize - 1] = 0;
					MonReduction(work, modulus, n2);
					FinalSubtractionInPlace(work.Slice(0, resSize), modulus);
					work.Slice(0, resSize).CopyTo(powsArrays[i]);
				}

				ArrayPool<int>.Shared.Return(x3Array);
				ArrayPool<int>.Shared.Return(preWorkArray);

				// Main loop
				Span<int> res = resArray.AsSpan(0, resSize);
				Span<int> mainWork = mainWorkArray.AsSpan(0, workSize);

				res.Clear();
				x2.CopyTo(res);

				int lowexp;
				int acc3;

				for (int i = exponent.BitLength - 1; i >= 0; i--) {
					if (BitLevel.TestBit(exponent, i)) {
						lowexp = 1;
						acc3 = i;

						for (int j = System.Math.Max(i - 3, 0); j <= i - 1; j++) {
							if (BitLevel.TestBit(exponent, j)) {
								if (j < acc3) {
									acc3 = j;
									lowexp = (lowexp << (i - j)) ^ 1;
								} else
									lowexp = lowexp ^ (1 << (j - acc3));
							}
						}

						for (int j = acc3; j <= i; j++) {
							Multiplication.MultArraysPap(res.Slice(0, n), n, res.Slice(0, n), n, mainWork);
							mainWork[workSize - 1] = 0;
							MonReduction(mainWork, modulus, n2);
							FinalSubtractionInPlace(mainWork.Slice(0, resSize), modulus);
							mainWork.Slice(0, resSize).CopyTo(res);
						}

						Multiplication.MultArraysPap(powsArrays[(lowexp - 1) >> 1].AsSpan(0, n), n, res.Slice(0, n), n, mainWork);
						mainWork[workSize - 1] = 0;
						MonReduction(mainWork, modulus, n2);
						FinalSubtractionInPlace(mainWork.Slice(0, resSize), modulus);
						mainWork.Slice(0, resSize).CopyTo(res);
						i = acc3;
					} else {
						Multiplication.MultArraysPap(res.Slice(0, n), n, res.Slice(0, n), n, mainWork);
						mainWork[workSize - 1] = 0;
						MonReduction(mainWork, modulus, n2);
						FinalSubtractionInPlace(mainWork.Slice(0, resSize), modulus);
						mainWork.Slice(0, resSize).CopyTo(res);
					}
				}

				// Final Montgomery conversion in-place
				mainWork.Slice(0, workSize).Clear();
				res.CopyTo(mainWork);
				MonReduction(mainWork, modulus, n2);
				FinalSubtractionInPlace(mainWork.Slice(0, resSize), modulus);

				// Compute actual length and handle zero
				int actualLen = resSize;
				while (actualLen > 1 && mainWork[actualLen - 1] == 0) actualLen--;
				int finalSign = (actualLen == 1 && mainWork[0] == 0) ? 0 : 1;
				int[] resultDigits = new int[actualLen];
				mainWork.Slice(0, actualLen).CopyTo(resultDigits);
				return new BigInteger(finalSign, actualLen, resultDigits);
			} finally {
				ArrayPool<int>.Shared.Return(resArray);
				ArrayPool<int>.Shared.Return(mainWorkArray);
				for (int i = 0; i < 8; i++)
					ArrayPool<int>.Shared.Return(powsArrays[i]);
			}
		}

		/// <summary>
		/// Performs modular exponentiation using the Montgomery Reduction with an even modulus.
		/// Based on the square and multiply algorithm and Koc's "Montgomery Reduction with Even Modulus".
		/// </summary>
		/// <param name="b">The base.</param>
		/// <param name="exponent">The exponent.</param>
		/// <param name="modulus">The modulus (must be even).</param>
		/// <returns><c>b^exponent mod modulus</c>.</returns>
		public static BigInteger EvenModPow(BigInteger b,
			BigInteger exponent,
			BigInteger modulus) {
			int j = modulus.LowestSetBit;
			BigInteger q = modulus >> j;

			BigInteger x1 = OddModPow(b, exponent, q);

			BigInteger x2 = Pow2ModPow(b, exponent, j);

			BigInteger qInv = ModPow2Inverse(q, j);
			BigInteger y = (x2 - x1) * qInv;
			y = InplaceModPow2(y, j);
			if (y.Sign < 0)
				y += BigInteger.GetPowerOfTwo(j);
			return x1 + (q * y);
		}

		/// <summary>
		/// Computes <c>base^exponent mod (2^j)</c>.
		/// </summary>
		/// <param name="b">The base.</param>
		/// <param name="exponent">The exponent.</param>
		/// <param name="j">The exponent for the power-of-two modulus.</param>
		/// <returns><c>b^exponent mod (2^j)</c>.</returns>
		private static BigInteger Pow2ModPow(BigInteger b, BigInteger exponent, int j) {
			BigInteger res = BigInteger.One;
			BigInteger e = exponent;
			BigInteger baseMod2toN = b;
			BigInteger res2;
			if (BigInteger.TestBit(b, 0))
				e = InplaceModPow2(e, j - 1);
			baseMod2toN = InplaceModPow2(baseMod2toN, j);

			for (int i = e.BitLength - 1; i >= 0; i--) {
				res2 = res;
				res2 = InplaceModPow2(res2, j);
				res = res * res2;
				if (BitLevel.TestBit(e, i)) {
					res = res * baseMod2toN;
					res = InplaceModPow2(res, j);
				}
			}
			res = InplaceModPow2(res, j);
			return res;
		}

		private static void MonReduction(Span<int> res, BigInteger modulus, int n2) {
			int[] modulusDigits = modulus.Digits;
			int modulusLen = modulus.numberLength;
			long outerCarry = 0;

			for (int i = 0; i < modulusLen; i++) {
				long innnerCarry = 0;
				int m = (int) Multiplication.UnsignedMultAddAdd(res[i], n2, 0, 0);
				for (int j = 0; j < modulusLen; j++) {
					innnerCarry = Multiplication.UnsignedMultAddAdd(m, modulusDigits[j], res[i + j], (int) innnerCarry);
					res[i + j] = (int) innnerCarry;
					innnerCarry = Utils.URShift(innnerCarry, 32);
				}

				outerCarry += (res[i + modulusLen] & 0xFFFFFFFFL) + innnerCarry;
				res[i + modulusLen] = (int) outerCarry;
				outerCarry = Utils.URShift(outerCarry, 32);
			}

			res[modulusLen << 1] = (int) outerCarry;

			for (int j = 0; j < modulusLen + 1; j++)
				res[j] = res[j + modulusLen];
		}

		/// <summary>
		/// Implements the Montgomery Product of two integers represented by
		/// <c>int</c> arrays in little-endian notation.
		/// </summary>
		/// <param name="a">The first factor.</param>
		/// <param name="b">The second factor.</param>
		/// <param name="modulus">The modulus.</param>
		/// <param name="n2">The digit modulus'[0].</param>
		/// <returns>The Montgomery product.</returns>
		private static BigInteger MonPro(BigInteger a, BigInteger b, BigInteger modulus, int n2) {
			int modulusLen = modulus.numberLength;
			int resLen = (modulusLen << 1) + 1;
			int[]? resArray = ArrayPool<int>.Shared.Rent(resLen);
			Span<int> res = resArray.AsSpan(0, resLen);
			try {
				Multiplication.MultArraysPap(a.Digits,
					System.Math.Min(modulusLen, a.numberLength),
					b.Digits,
					System.Math.Min(modulusLen, b.numberLength),
					res);
				MonReduction(res, modulus, n2);
				return FinalSubtraction(res, modulus);
			} finally {
				ArrayPool<int>.Shared.Return(resArray);
			}
		}

		/// <summary>
		/// Performs the final reduction of the Montgomery algorithm.
		/// </summary>
		/// <param name="res">The result array.</param>
		/// <param name="modulus">The modulus.</param>
		/// <returns>The reduced <see cref="BigInteger"/>.</returns>
		private static BigInteger FinalSubtraction(Span<int> res, BigInteger modulus) {
			int modulusLen = modulus.numberLength;
			bool doSub = res[modulusLen] != 0;
			if (!doSub) {
				int[] modulusDigits = modulus.Digits;
				doSub = true;
				for (int i = modulusLen - 1; i >= 0; i--) {
					if (res[i] != modulusDigits[i]) {
						doSub = (res[i] != 0) && ((res[i] & 0xFFFFFFFFL) > (modulusDigits[i] & 0xFFFFFFFFL));
						break;
					}
				}
			}

			if (doSub) {
				long borrow = 0;
				int[] modulusDigits = modulus.Digits;
				for (int i = 0; i < modulusLen; i++) {
					long diff = (res[i] & 0xFFFFFFFFL) - (modulusDigits[i] & 0xFFFFFFFFL) - borrow;
					res[i] = (int)diff;
					borrow = diff < 0 ? 1 : 0;
				}
			}
			res[modulusLen] = 0;

			// Compute actual length and handle zero
			int actualLen = modulusLen + 1;
			while (actualLen > 1 && res[actualLen - 1] == 0) actualLen--;
			int finalSign = (actualLen == 1 && res[0] == 0) ? 0 : 1;
			int[] resultDigits = new int[actualLen];
			res.Slice(0, actualLen).CopyTo(resultDigits);
			return new BigInteger(finalSign, actualLen, resultDigits);
		}

		private static void FinalSubtractionInPlace(Span<int> res, BigInteger modulus) {
			int modulusLen = modulus.numberLength;
			bool doSub = res[modulusLen] != 0;
			if (!doSub) {
				int[] modulusDigits = modulus.Digits;
				doSub = true;
				for (int i = modulusLen - 1; i >= 0; i--) {
					if (res[i] != modulusDigits[i]) {
						doSub = (res[i] != 0) && ((res[i] & 0xFFFFFFFFL) > (modulusDigits[i] & 0xFFFFFFFFL));
						break;
					}
				}
			}

			if (doSub) {
				long borrow = 0;
				int[] modulusDigits = modulus.Digits;
				for (int i = 0; i < modulusLen; i++) {
					long diff = (res[i] & 0xFFFFFFFFL) - (modulusDigits[i] & 0xFFFFFFFFL) - borrow;
					res[i] = (int)diff;
					borrow = diff < 0 ? 1 : 0;
				}
			}
			res[modulusLen] = 0;
		}

		/// <summary>
		/// Computes <c>x^(-1) mod (2^n)</c> for an odd positive number <paramref name="x"/>.
		/// </summary>
		/// <param name="x">An odd positive number.</param>
		/// <param name="n">The exponent by which 2 is raised.</param>
		/// <returns><c>x^(-1) mod (2^n)</c>.</returns>
		private static BigInteger ModPow2Inverse(BigInteger x, int n) {
			int[] d = new int[(n + 31) >> 5];
			d[0] = 1;
			BigInteger y = new BigInteger(1, 1, d);

			for (int i = 1; i < n; i++) {
				if (BitLevel.TestBit(x * y, i)) {
					d[i >> 5] |= (1 << (i & 31));
				}
			}
			return new BigInteger(1, d.Length, d);
		}

		/// <summary>
		/// Performs <c>x = x mod (2^n)</c>.
		/// </summary>
		/// <param name="x">A positive number.</param>
		/// <param name="n">A positive exponent of 2.</param>
		/// <returns>x mod (2^n).</returns>
		public static BigInteger InplaceModPow2(BigInteger x, int n) {
			int fd = n >> 5;
			int leadingZeros;

			if ((x.numberLength < fd) || (x.BitLength <= n))
				return x;
			leadingZeros = 32 - (n & 31);
			
			int[]? resArray = null;
			Span<int> resDigits = x.numberLength <= StackAllocMax
				? stackalloc int[x.numberLength]
				: (resArray = ArrayPool<int>.Shared.Rent(x.numberLength));
			resDigits = resDigits.Slice(0, x.numberLength);

			try {
				x.digits.AsSpan(0, x.numberLength).CopyTo(resDigits);
				resDigits[fd] &= (leadingZeros < 32) ? (Utils.URShift(-1, leadingZeros)) : 0;
				var result = new BigInteger(x.sign, fd + 1, resDigits);
				return result.WithCutOffLeadingZeroes();
			} finally {
				if (resArray != null)
					ArrayPool<int>.Shared.Return(resArray);
			}
		}
	}
}
