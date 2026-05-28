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
	/// Provides static mathematical operations for <see cref="BigInteger"/> values,
	/// including shifting, negation, division, modular arithmetic, exponentiation,
	/// and greatest common divisor computation.
	/// </summary>
	static class BigIntegerMath {
		private const int StackAllocMax = 256;
		/// <summary>
		/// Shifts the bits of the specified <see cref="BigInteger"/> value to the right.
		/// </summary>
		/// <param name="value">The <see cref="BigInteger"/> value to shift.</param>
		/// <param name="n">The number of bit positions to shift. If negative, the value is shifted to the left instead.</param>
		/// <returns>A <see cref="BigInteger"/> whose bits have been shifted to the right by <paramref name="n"/> positions.</returns>
		public static BigInteger ShiftRight(BigInteger value, int n) {
			if ((n == 0) || (value.Sign == 0)) {
				return value;
			}
			return ((n > 0)
				? BitLevel.ShiftRight(value, n)
				: BitLevel.ShiftLeft(
					value, -n));
		}

		/// <summary>
		/// Shifts the bits of the specified <see cref="BigInteger"/> value to the left.
		/// </summary>
		/// <param name="value">The <see cref="BigInteger"/> value to shift.</param>
		/// <param name="n">The number of bit positions to shift. If negative, the value is shifted to the right instead.</param>
		/// <returns>A <see cref="BigInteger"/> whose bits have been shifted to the left by <paramref name="n"/> positions.</returns>
		public static BigInteger ShiftLeft(BigInteger value, int n) {
			if ((n == 0) || (value.Sign == 0)) {
				return value;
			}
			return ((n > 0) ? BitLevel.ShiftLeft(value, n) : BitLevel.ShiftRight(value, -n));
		}

		/// <summary>
		/// Negates the specified <see cref="BigInteger"/> value.
		/// </summary>
		/// <param name="value">The <see cref="BigInteger"/> value to negate.</param>
		/// <returns>A <see cref="BigInteger"/> representing the negation of <paramref name="value"/>.</returns>
		public static BigInteger Negate(BigInteger value) {
			return ((value.Sign == 0) ? value : new BigInteger(-value.Sign, value.numberLength, value.digits));
		}

		/// <summary>
		/// Divides one <see cref="BigInteger"/> by another, returning the integer quotient.
		/// </summary>
		/// <param name="dividend">The <see cref="BigInteger"/> value to be divided.</param>
		/// <param name="divisor">The <see cref="BigInteger"/> value to divide by.</param>
		/// <returns>The integer quotient of the division.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="divisor"/> is zero.</exception>
		/// <example>
		/// <code>
		/// BigInteger quotient = BigIntegerMath.Divide(
		///     BigInteger.Parse("100"),
		///     BigInteger.Parse("3"));
		/// // quotient is 33
		/// </code>
		/// </example>
		public static BigInteger Divide(BigInteger dividend, BigInteger divisor) {
			if (divisor.Sign == 0) {
				// math.17=BigInteger divide by zero
				throw new ArithmeticException(Messages.math17); //$NON-NLS-1$
			}
			int divisorSign = divisor.Sign;
			if (divisor.IsOne) {
				return ((divisor.Sign > 0) ? dividend : -dividend);
			}
			int thisSign = dividend.Sign;
			int thisLen = dividend.numberLength;
			int divisorLen = divisor.numberLength;
			if (thisLen + divisorLen == 2) {
				long val = (dividend.digits[0] & 0xFFFFFFFFL)
				           / (divisor.digits[0] & 0xFFFFFFFFL);
				if (thisSign != divisorSign) {
					val = -val;
				}
				return BigInteger.FromInt64(val);
			}
			int cmp = ((thisLen != divisorLen)
				? ((thisLen > divisorLen) ? 1 : -1)
				: Elementary.CompareArrays(dividend.digits, divisor.digits, thisLen));
			if (cmp == BigInteger.EQUALS) {
				return ((thisSign == divisorSign) ? BigInteger.One : BigInteger.MinusOne);
			}
			if (cmp == BigInteger.LESS) {
				return BigInteger.Zero;
			}
			int resLength = thisLen - divisorLen + 1;
			int[]? resArray = null;
			Span<int> resDigits = resLength <= StackAllocMax
				? stackalloc int[resLength]
				: (resArray = ArrayPool<int>.Shared.Rent(resLength));
			resDigits = resDigits.Slice(0, resLength);
			int resSign = ((thisSign == divisorSign) ? 1 : -1);
			if (divisorLen == 1) {
				Division.DivideArrayByInt(resDigits, dividend.digits, thisLen,
					divisor.digits[0]);
			} else {
				Division.Divide(resDigits, resLength, dividend.digits, thisLen,
					divisor.digits, divisorLen);
			}
			BigInteger result = new BigInteger(resSign, resLength, resDigits);
			return result.WithCutOffLeadingZeroes();

			if (resArray != null)
				ArrayPool<int>.Shared.Return(resArray);

			return result;
		}

		/// <summary>
		/// Computes the remainder when one <see cref="BigInteger"/> is divided by another.
		/// </summary>
		/// <param name="dividend">The <see cref="BigInteger"/> value to be divided.</param>
		/// <param name="divisor">The <see cref="BigInteger"/> value to divide by.</param>
		/// <returns>The remainder of the division.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="divisor"/> is zero.</exception>
		public static BigInteger Remainder(BigInteger dividend, BigInteger divisor) {
			if (divisor.Sign == 0) {
				// math.17=BigInteger divide by zero
				throw new ArithmeticException(Messages.math17); //$NON-NLS-1$
			}
			int thisLen = dividend.numberLength;
			int divisorLen = divisor.numberLength;
			if (((thisLen != divisorLen)
				    ? ((thisLen > divisorLen) ? 1 : -1)
				    : Elementary.CompareArrays(dividend.digits, divisor.digits, thisLen)) == BigInteger.LESS) {
				return dividend;
			}
			int resLength = divisorLen;
			if (resLength == 1) {
				int remainder = Division.RemainderArrayByInt(dividend.digits, thisLen,
					divisor.digits[0]);
				return BigInteger.FromInt64(dividend.Sign * (long)remainder);
			} else {
				int qLen = thisLen - divisorLen + 1;
				int[] resDigits = Division.Divide(null, qLen, dividend.digits, thisLen,
					divisor.digits, divisorLen);
				BigInteger result = new BigInteger(dividend.Sign, resLength, resDigits);
				return result.WithCutOffLeadingZeroes();
				return result;
			}
		}

		/// <summary>
		/// Divides one <see cref="BigInteger"/> by another, returning the quotient and the remainder.
		/// </summary>
		/// <param name="dividend">The <see cref="BigInteger"/> value to be divided.</param>
		/// <param name="divisor">The <see cref="BigInteger"/> value to divide by.</param>
		/// <param name="remainder">When this method returns, contains the remainder of the division.</param>
		/// <returns>The integer quotient of the division.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="divisor"/> is zero.</exception>
		/// <example>
		/// <code>
		/// BigInteger remainder;
		/// BigInteger quotient = BigIntegerMath.DivideAndRemainder(
		///     BigInteger.Parse("100"),
		///     BigInteger.Parse("3"),
		///     out remainder);
		/// // quotient is 33, remainder is 1
		/// </code>
		/// </example>
		public static BigInteger DivideAndRemainder(BigInteger dividend, BigInteger divisor, out BigInteger remainder) {
			int divisorSign = divisor.Sign;
			if (divisorSign == 0) {
				// math.17=BigInteger divide by zero
				throw new ArithmeticException(Messages.math17); //$NON-NLS-1$
			}
			int divisorLen = divisor.numberLength;
			int[] divisorDigits = divisor.digits;
			if (divisorLen == 1) {
				var values = Division.DivideAndRemainderByInteger(dividend, divisorDigits[0], divisorSign);
				remainder = values[1];
				return values[0];
			}

			int[] thisDigits = dividend.digits;
			int thisLen = dividend.numberLength;
			int cmp = (thisLen != divisorLen)
				? ((thisLen > divisorLen) ? 1 : -1)
				: Elementary.CompareArrays(thisDigits, divisorDigits, thisLen);
			if (cmp < 0) {
				remainder = dividend;
				return BigInteger.Zero;
			}
			int thisSign = dividend.Sign;
			int quotientLength = thisLen - divisorLen + 1;
			int remainderLength = divisorLen;
			int quotientSign = ((thisSign == divisorSign) ? 1 : -1);
			int[]? quotientArray = null;
			Span<int> quotientDigits = quotientLength <= StackAllocMax
				? stackalloc int[quotientLength]
				: (quotientArray = ArrayPool<int>.Shared.Rent(quotientLength));
			quotientDigits = quotientDigits.Slice(0, quotientLength);

			try {
				int[] remainderDigits = Division.Divide(quotientDigits, quotientLength,
					thisDigits, thisLen, divisorDigits, divisorLen);

				var quotient = new BigInteger(quotientSign, quotientLength, quotientDigits);
				remainder = new BigInteger(thisSign, remainderLength, remainderDigits);
				quotient = quotient.WithCutOffLeadingZeroes();
				remainder = remainder.WithCutOffLeadingZeroes();

				return quotient;
			} finally {
				if (quotientArray != null)
					ArrayPool<int>.Shared.Return(quotientArray);
			}
		}

		/// <summary>
		/// Computes the mathematical modulus (mod) of the specified <see cref="BigInteger"/> value.
		/// </summary>
		/// <param name="value">The <see cref="BigInteger"/> value.</param>
		/// <param name="m">The modulus, which must be positive.</param>
		/// <returns>The result of <paramref name="value"/> mod <paramref name="m"/>, always non-negative.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="m"/> is less than or equal to zero.</exception>
		public static BigInteger Mod(BigInteger value, BigInteger m) {
			if (m.Sign <= 0) {
				// math.18=BigInteger: modulus not positive
				throw new ArithmeticException(Messages.math18); //$NON-NLS-1$
			}
			BigInteger rem = BigMath.Remainder(value, m);
			return ((rem.Sign < 0) ? rem + m : rem);
		}

		/// <summary>
		/// Computes the modular multiplicative inverse of the specified <see cref="BigInteger"/>.
		/// </summary>
		/// <param name="value">The <see cref="BigInteger"/> value to compute the inverse for.</param>
		/// <param name="m">The modulus, which must be positive.</param>
		/// <returns>The modular multiplicative inverse of <paramref name="value"/> modulo <paramref name="m"/>.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="m"/> is less than or equal to zero, or when the modular inverse does not exist.</exception>
		public static BigInteger ModInverse(BigInteger value, BigInteger m) {
			if (m.Sign <= 0) {
				// math.18=BigInteger: modulus not positive
				throw new ArithmeticException(Messages.math18); //$NON-NLS-1$
			}
			// If both are even, no inverse exists
			if (!(BigInteger.TestBit(value, 0) ||
			      BigInteger.TestBit(m, 0))) {
				// math.19=BigInteger not invertible.
				throw new ArithmeticException(Messages.math19); //$NON-NLS-1$
			}
			if (m.IsOne) {
				return BigInteger.Zero;
			}

			// From now on: (m > 1)
			BigInteger res = Division.ModInverseMontgomery(Abs(value) % m, m);
			if (res.Sign == 0) {
				// math.19=BigInteger not invertible.
				throw new ArithmeticException(Messages.math19); //$NON-NLS-1$
			}

			res = ((value.Sign < 0) ? m - res : res);
			return res;
		}

		/// <summary>
		/// Returns the absolute value of the specified <see cref="BigInteger"/>.
		/// </summary>
		/// <param name="value">The <see cref="BigInteger"/> value.</param>
		/// <returns>The absolute value of <paramref name="value"/>.</returns>
		public static BigInteger Abs(BigInteger value) {
			return ((value.Sign < 0) ? new BigInteger(1, value.numberLength, value.digits) : value);
		}

		/// <summary>
		/// Computes the modular exponentiation (value^exponent mod m).
		/// </summary>
		/// <param name="value">The base <see cref="BigInteger"/> value.</param>
		/// <param name="exponent">The exponent <see cref="BigInteger"/>.</param>
		/// <param name="m">The modulus, which must be positive.</param>
		/// <returns>The result of <paramref name="value"/>^<paramref name="exponent"/> mod <paramref name="m"/>.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="m"/> is less than or equal to zero.</exception>
		/// <example>
		/// <code>
		/// BigInteger result = BigIntegerMath.ModPow(
		///     BigInteger.Parse("5"),
		///     BigInteger.Parse("3"),
		///     BigInteger.Parse("13"));
		/// // result is 8 (5^3 = 125, 125 mod 13 = 8)
		/// </code>
		/// </example>
		public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger m) {
			if (m.Sign <= 0) {
				// math.18=BigInteger: modulus not positive
				throw new ArithmeticException(Messages.math18); //$NON-NLS-1$
			}
			BigInteger b = value;

			if (m.IsOne | (exponent.Sign > 0 & b.Sign == 0)) {
				return BigInteger.Zero;
			}
			if (b.Sign == 0 && exponent.Sign == 0) {
				return BigInteger.One;
			}
			if (exponent.Sign < 0) {
				b = BigMath.ModInverse(value, m);
				exponent = -exponent;
			}
			// From now on: (m > 0) and (exponent >= 0)
			BigInteger res = (BigInteger.TestBit(m, 0))
				? Division.OddModPow(Abs(b),
					exponent, m)
				: Division.EvenModPow(Abs(b), exponent, m);
			if ((b.Sign < 0) && BigInteger.TestBit(exponent, 0)) {
				// -b^e mod m == ((-1 mod m) * (b^e mod m)) mod m
				res = ((m - BigInteger.One) * res) % m;
			}
			// else exponent is even, so base^exp is positive
			return res;
		}

		/// <summary>
		/// Raises the specified <see cref="BigInteger"/> to the specified power.
		/// </summary>
		/// <param name="value">The <see cref="BigInteger"/> base value.</param>
		/// <param name="exp">The exponent, which must be non-negative.</param>
		/// <returns>The result of <paramref name="value"/> raised to the power of <paramref name="exp"/>.</returns>
		/// <exception cref="ArithmeticException">Thrown when <paramref name="exp"/> is negative.</exception>
		public static BigInteger Pow(BigInteger value, int exp) {
			if (exp < 0) {
				// math.16=Negative exponent
				throw new ArithmeticException(Messages.math16); //$NON-NLS-1$
			}
			if (exp == 0) {
				return BigInteger.One;
			} else if (exp == 1 ||
			           value.Equals(BigInteger.One) || value.Equals(BigInteger.Zero)) {
				return value;
			}

			// if even take out 2^x factor which we can
			// calculate by shifting.
			if (!BigInteger.TestBit(value, 0)) {
				int x = 1;
				while (!BigInteger.TestBit(value, x)) {
					x++;
				}

				return BigInteger.GetPowerOfTwo(x * exp) * (Pow(value >> x, exp));
			}

			return Multiplication.Pow(value, exp);
		}

		/// <summary>
		/// Computes the greatest common divisor (GCD) of two <see cref="BigInteger"/> values.
		/// </summary>
		/// <param name="a">The first <see cref="BigInteger"/> value.</param>
		/// <param name="b">The second <see cref="BigInteger"/> value.</param>
		/// <returns>The greatest common divisor of <paramref name="a"/> and <paramref name="b"/>.</returns>
		/// <example>
		/// <code>
		/// BigInteger gcd = BigIntegerMath.Gcd(
		///     BigInteger.Parse("12"),
		///     BigInteger.Parse("18"));
		/// // gcd is 6
		/// </code>
		/// </example>
		public static BigInteger Gcd(BigInteger a, BigInteger b) {
			BigInteger val1 = Abs(a);
			BigInteger val2 = Abs(b);
			// To avoid a possible division by zero
			if (val1.Sign == 0) {
				return val2;
			} else if (val2.Sign == 0) {
				return val1;
			}

			// Optimization for small operands
			// (op2.bitLength() < 64) and (op1.bitLength() < 64)
			if (((val1.numberLength == 1) || ((val1.numberLength == 2) && (val1.digits[1] > 0)))
			    && (val2.numberLength == 1 || (val2.numberLength == 2 && val2.digits[1] > 0))) {
				return BigInteger.FromInt64(Division.GcdBinary(val1.ToInt64(), val2.ToInt64()));
			}

			return Division.GcdBinary(val1, val2);

		}
	}
}
