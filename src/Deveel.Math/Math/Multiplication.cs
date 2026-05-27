using System;
using System.Buffers;

namespace Deveel.Math {
	static class Multiplication {
		private const int WhenUseKaratsuba = 63;

		private const int StackAllocMax = 256;

		static readonly int[] TenPows = {
		1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000
	};

		static readonly int[] FivePows = {
		1, 5, 25, 125, 625, 3125, 15625, 78125, 390625,
		1953125, 9765625, 48828125, 244140625, 1220703125
	};

		public static readonly BigInteger[] BigTenPows = new BigInteger[32];
		public static readonly BigInteger[] BigFivePows = new BigInteger[32];

		static Multiplication() {
			int i;
			long fivePow = 1L;

			for (i = 0; i <= 18; i++) {
				BigFivePows[i] = BigInteger.FromInt64(fivePow);
				BigTenPows[i] = BigInteger.FromInt64(fivePow << i);
				fivePow *= 5;
			}
			for (; i < BigTenPows.Length; i++) {
				BigFivePows[i] = BigFivePows[i - 1] * BigFivePows[1];
				BigTenPows[i] = BigTenPows[i - 1] * BigInteger.Ten;
			}
		}

		public static BigInteger Multiply(BigInteger x, BigInteger y) {
			return Karatsuba(x, y);
		}

		private static BigInteger Karatsuba(BigInteger op1, BigInteger op2) {
			BigInteger temp;
			if (op2.numberLength > op1.numberLength) {
				temp = op1;
				op1 = op2;
				op2 = temp;
			}
			if (op2.numberLength < WhenUseKaratsuba) {
				return MultiplyPap(op1, op2);
			}
			int ndiv2 = (int)(op1.numberLength & 0xFFFFFFFE) << 4;
			BigInteger upperOp1 = op1 >> ndiv2;
			BigInteger upperOp2 = op2 >> ndiv2;
			BigInteger lowerOp1 = op1 - (upperOp1 << ndiv2);
			BigInteger lowerOp2 = op2 - (upperOp2 << ndiv2);

			BigInteger upper = Karatsuba(upperOp1, upperOp2);
			BigInteger lower = Karatsuba(lowerOp1, lowerOp2);
			BigInteger middle = Karatsuba(upperOp1 - lowerOp1,
					lowerOp2 - upperOp2);
			middle = (middle + upper + lower);
			middle = middle << ndiv2;
			upper = upper << (ndiv2 << 1);

			return (upper + middle + lower);
		}

		private static BigInteger MultiplyPap(BigInteger a, BigInteger b) {
			int aLen = a.numberLength;
			int bLen = b.numberLength;
			int resLength = aLen + bLen;
			int resSign = (a.Sign != b.Sign) ? -1 : 1;
			if (resLength == 2) {
				long val = UnsignedMultAddAdd(a.Digits[0], b.Digits[0], 0, 0);
				int valueLo = (int)val;
				int valueHi = (int)(val >> 32);
				return ((valueHi == 0)
				? new BigInteger(resSign, valueLo)
				: new BigInteger(resSign, 2, new int[] { valueLo, valueHi }));
			}
			ReadOnlySpan<int> aDigits = a.Digits;
			ReadOnlySpan<int> bDigits = b.Digits;

			int[]? resArray = null;
			Span<int> resDigits = resLength <= StackAllocMax
				? stackalloc int[resLength]
				: (resArray = ArrayPool<int>.Shared.Rent(resLength));
			resDigits = resDigits.Slice(0, resLength);

			try {
				MultArraysPap(aDigits, aLen, bDigits, bLen, resDigits);
				BigInteger result = new BigInteger(resSign, resLength, resDigits.ToArray());
				result.CutOffLeadingZeroes();
				return result;
			} finally {
				if (resArray != null)
					ArrayPool<int>.Shared.Return(resArray);
			}
		}

		public static void MultArraysPap(ReadOnlySpan<int> aDigits, int aLen, ReadOnlySpan<int> bDigits, int bLen, Span<int> resDigits) {
			if (aLen == 0 || bLen == 0) return;

			if (aLen == 1) {
				resDigits[bLen] = MultiplyByInt(resDigits, bDigits, bLen, aDigits[0]);
			} else if (bLen == 1) {
				resDigits[aLen] = MultiplyByInt(resDigits, aDigits, aLen, bDigits[0]);
			} else {
				MultPap(aDigits, bDigits, resDigits, aLen, bLen);
			}
		}

		private static void MultPap(ReadOnlySpan<int> a, ReadOnlySpan<int> b, Span<int> t, int aLen, int bLen) {
			if (a == b && aLen == bLen) {
				Square(a, aLen, t);
				return;
			}

			for (int i = 0; i < aLen; i++) {
				long carry = 0;
				int aI = a[i];
				ref int tBase = ref t[i];
				for (int j = 0; j < bLen; j++) {
					carry = UnsignedMultAddAdd(aI, b[j], UnsafeAdd(ref tBase, j), (int)carry);
					UnsafeAdd(ref tBase, j) = (int)carry;
					carry = (long)((ulong)carry >> 32);
				}
				t[i + bLen] = (int)carry;
			}
		}

		private static ref int UnsafeAdd(ref int source, int index) {
			return ref System.Runtime.CompilerServices.Unsafe.Add(ref source, index);
		}

		private static int MultiplyByInt(Span<int> res, ReadOnlySpan<int> a, int aSize, int factor) {
			long carry = 0;
			for (int i = 0; i < aSize; i++) {
				carry = UnsignedMultAddAdd(a[i], factor, (int)carry, 0);
				res[i] = (int)carry;
				carry = (long)((ulong)carry >> 32);
			}
			return (int)carry;
		}

		public static int MultiplyByInt(int[] a, int aSize, int factor) {
			return MultiplyByInt(a.AsSpan(), a.AsSpan(), aSize, factor);
		}

		public static BigInteger MultiplyByPositiveInt(BigInteger val, int factor) {
			int resSign = val.Sign;
			if (resSign == 0) {
				return BigInteger.Zero;
			}
			int aNumberLength = val.numberLength;
			ReadOnlySpan<int> aDigits = val.Digits;

			if (aNumberLength == 1) {
				long res = UnsignedMultAddAdd(aDigits[0], factor, 0, 0);
				int resLo = (int)res;
				int resHi = (int)(res >> 32);
				return ((resHi == 0)
				? new BigInteger(resSign, resLo)
				: new BigInteger(resSign, 2, new int[] { resLo, resHi }));
			}
			int resLength = aNumberLength + 1;

			int[]? resArray = null;
			Span<int> resDigits = resLength <= StackAllocMax
				? stackalloc int[resLength]
				: (resArray = ArrayPool<int>.Shared.Rent(resLength));
			resDigits = resDigits.Slice(0, resLength);

			try {
				resDigits[aNumberLength] = MultiplyByInt(resDigits, aDigits, aNumberLength, factor);
				BigInteger result = new BigInteger(resSign, resLength, resDigits.ToArray());
				result.CutOffLeadingZeroes();
				return result;
			} finally {
				if (resArray != null)
					ArrayPool<int>.Shared.Return(resArray);
			}
		}

		public static BigInteger Pow(BigInteger b, int exponent) {
			BigInteger res = BigInteger.One;
			BigInteger acc = b;

			for (; exponent > 1; exponent >>= 1) {
				if ((exponent & 1) != 0) {
					res = res * acc;
				}
				if (acc.numberLength == 1) {
					acc = acc * acc;
				} else {
					int newLen = acc.numberLength << 1;
					int[]? resArray = null;
					Span<int> squareDigits = newLen <= StackAllocMax
						? stackalloc int[newLen]
						: (resArray = ArrayPool<int>.Shared.Rent(newLen));
					squareDigits = squareDigits.Slice(0, newLen);

					try {
						Square(acc.Digits, acc.numberLength, squareDigits);
						acc = new BigInteger(1, newLen, squareDigits.ToArray());
					} finally {
						if (resArray != null)
							ArrayPool<int>.Shared.Return(resArray);
					}
				}
			}
			res = res * acc;
			return res;
		}

		private static void Square(ReadOnlySpan<int> a, int aLen, Span<int> res) {
			long carry;

			for (int i = 0; i < aLen; i++) {
				carry = 0;
				ref int resBase = ref res[i];
				for (int j = i + 1; j < aLen; j++) {
					carry = UnsignedMultAddAdd(a[i], a[j], UnsafeAdd(ref resBase, j), (int)carry);
					UnsafeAdd(ref resBase, j) = (int)carry;
					carry = (long)((ulong)carry >> 32);
				}
				res[i + aLen] = (int)carry;
			}

			BitLevel.ShiftLeftOneBit(res, res, aLen << 1);

			carry = 0;
			for (int i = 0, index = 0; i < aLen; i++, index++) {
				carry = UnsignedMultAddAdd(a[i], a[i], res[index], (int)carry);
				res[index] = (int)carry;
				carry = (long)((ulong)carry >> 32);
				index++;
				carry += (uint)res[index];
				res[index] = (int)carry;
				carry = (long)((ulong)carry >> 32);
			}
		}

		public static BigInteger MultiplyByTenPow(BigInteger val, long exp) {
			return ((exp < TenPows.Length)
			? MultiplyByPositiveInt(val, TenPows[(int)exp])
			: val * PowerOf10(exp));
		}

		public static BigInteger PowerOf10(long exp) {
			int intExp = (int)exp;
			if (exp < BigTenPows.Length) {
				return BigTenPows[intExp];
			} else if (exp <= 50) {
				return BigMath.Pow(BigInteger.Ten, intExp);
			} else if (exp <= 1000) {
				return BigMath.Pow(BigFivePows[1], intExp) << intExp;
			}
			var byteArraySize = 1 + (exp / 2.4082399653118496);

			if (byteArraySize > (128 * 1024))
				throw new ArithmeticException(Messages.math01);

			if (exp <= Int32.MaxValue) {
				return BigMath.Pow(BigFivePows[1], intExp) << intExp;
			}
			BigInteger powerOfFive = BigMath.Pow(BigFivePows[1], Int32.MaxValue);
			BigInteger res = powerOfFive;
			long longExp = exp - Int32.MaxValue;

			intExp = (int)(exp % Int32.MaxValue);
			while (longExp > Int32.MaxValue) {
				res = res * powerOfFive;
				longExp -= Int32.MaxValue;
			}
			res = res * BigMath.Pow(BigFivePows[1], intExp);
			res = res << Int32.MaxValue;
			longExp = exp - Int32.MaxValue;
			while (longExp > Int32.MaxValue) {
				res = res << Int32.MaxValue;
				longExp -= Int32.MaxValue;
			}
			res = res << intExp;
			return res;
		}

		public static BigInteger MultiplyByFivePow(BigInteger val, int exp) {
			if (exp < FivePows.Length) {
				return MultiplyByPositiveInt(val, FivePows[exp]);
			} else if (exp < BigFivePows.Length) {
				return val * BigFivePows[exp];
			} else {
				return val * BigMath.Pow(BigFivePows[1], exp);
			}
		}

		public static long UnsignedMultAddAdd(int a, int b, int c, int d) {
			return (a & 0xFFFFFFFFL) * (b & 0xFFFFFFFFL) + (c & 0xFFFFFFFFL) + (d & 0xFFFFFFFFL);
		}
	}
}
