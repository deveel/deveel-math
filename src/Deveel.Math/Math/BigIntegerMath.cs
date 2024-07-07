using System;

namespace Deveel.Math {
	static class BigIntegerMath {
		public static BigInteger ShiftRight(BigInteger value, int n) {
			if ((n == 0) || (value.Sign == 0)) {
				return value;
			}
			return ((n > 0)
				? BitLevel.ShiftRight(value, n)
				: BitLevel.ShiftLeft(
					value, -n));
		}

		public static BigInteger ShiftLeft(BigInteger value, int n) {
			if ((n == 0) || (value.Sign == 0)) {
				return value;
			}
			return ((n > 0) ? BitLevel.ShiftLeft(value, n) : BitLevel.ShiftRight(value, -n));
		}

		public static BigInteger Negate(BigInteger value) {
			return ((value.Sign == 0) ? value : new BigInteger(-value.Sign, value.numberLength, value.digits));
		}

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
			int[] resDigits = new int[resLength];
			int resSign = ((thisSign == divisorSign) ? 1 : -1);
			if (divisorLen == 1) {
				Division.DivideArrayByInt(resDigits, dividend.digits, thisLen,
					divisor.digits[0]);
			} else {
				Division.Divide(resDigits, resLength, dividend.digits, thisLen,
					divisor.digits, divisorLen);
			}
			BigInteger result = new BigInteger(resSign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

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
			int[] resDigits = new int[resLength];
			if (resLength == 1) {
				resDigits[0] = Division.RemainderArrayByInt(dividend.digits, thisLen,
					divisor.digits[0]);
			} else {
				int qLen = thisLen - divisorLen + 1;
				resDigits = Division.Divide(null, qLen, dividend.digits, thisLen,
					divisor.digits, divisorLen);
			}
			BigInteger result = new BigInteger(dividend.Sign, resLength, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

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
			int[] quotientDigits = new int[quotientLength];
			int[] remainderDigits = Division.Divide(quotientDigits, quotientLength,
				thisDigits, thisLen, divisorDigits, divisorLen);

			var quotient = new BigInteger(quotientSign, quotientLength, quotientDigits);
			remainder = new BigInteger(thisSign, remainderLength, remainderDigits);
			quotient.CutOffLeadingZeroes();
			remainder.CutOffLeadingZeroes();

			return quotient;
		}

		public static BigInteger Mod(BigInteger value, BigInteger m) {
			if (m.Sign <= 0) {
				// math.18=BigInteger: modulus not positive
				throw new ArithmeticException(Messages.math18); //$NON-NLS-1$
			}
			BigInteger rem = BigMath.Remainder(value, m);
			return ((rem.Sign < 0) ? rem + m : rem);
		}

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

		public static BigInteger Abs(BigInteger value) {
			return ((value.Sign < 0) ? new BigInteger(1, value.numberLength, value.digits) : value);
		}

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

			return Division.GcdBinary(val1.Copy(), val2.Copy());

		}
	}
}
