using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Math {
	static class BigRationalMath {
		private static BigRational Of(BigDecimal numerator, BigDecimal denominator) {
			if (numerator.Sign == 0 && denominator.Sign != 0) {
				return BigRational.Zero;
			}
			if (numerator.CompareTo(BigDecimal.One) == 0 &&
			    denominator.CompareTo(BigDecimal.One) == 0) {
				return BigRational.One;
			}

			return new BigRational(numerator, denominator);
		}

		public static BigRational Reduce(BigRational rational) {
			var n = rational.Numerator.ToBigInteger();
			var d = rational.Denominator.ToBigInteger();

			var gcd = BigMath.Gcd(n, d);
			n = BigMath.Divide(n, gcd);
			d = BigMath.Divide(d, gcd);

			return new BigRational(new BigDecimal(n), new BigDecimal(d));
		}

		public static BigRational IntegerPart(BigRational value) {
			return Of(BigMath.Subtract(value.Numerator, BigMath.Remainder(value.Numerator, value.Denominator)),
				value.Denominator);
		}

		public static BigRational FractionPart(BigRational value) {
			return Of(BigMath.Remainder(value.Numerator, value.Denominator), value.Denominator);
		}

		public static BigRational Negate(BigRational value) {
			if (value.IsZero) {
				return value;
			}

			return Of(BigMath.Negate(value.Numerator), value.Denominator);
		}

		public static BigRational Reciprocal(BigRational value) {
			return Of(value.Denominator, value.Numerator);
		}

		public static BigRational Increment(BigRational value) {
			return Of(BigMath.Add(value.Numerator, value.Denominator), value.Denominator);
		}

		public static BigRational Decrement(BigRational value) {
			return Of(BigMath.Subtract(value.Numerator, value.Denominator), value.Denominator);
		}

		public static BigRational Add(BigRational a, BigRational value) {
			if (a.Denominator.Equals(value.Denominator)) {
				return Of(BigMath.Add(a.Numerator, value.Numerator), a.Denominator);
			}

			var n = BigMath.Add(BigMath.Multiply(a.Numerator, value.Denominator),
				BigMath.Multiply(value.Numerator, a.Denominator));
			var d = BigMath.Multiply(a.Denominator, value.Denominator);
			return Of(n, d);
		}

		public static BigRational Add(BigRational a, BigDecimal value) {
			if (value == BigDecimal.Zero)
				return a;

			return Of(BigMath.Add(a.Numerator, BigMath.Multiply(value, a.Denominator)), a.Denominator);
		}

		public static BigRational Subtract(BigRational a, BigRational value) {
			if (a.Denominator.Equals(value.Denominator))
				return Of(BigMath.Subtract(a.Numerator, value.Numerator), a.Denominator);

			var n = BigMath.Subtract(BigMath.Multiply(a.Numerator, value.Denominator),
				BigMath.Multiply(value.Numerator, a.Denominator));
			var d = BigMath.Multiply(a.Denominator, value.Denominator);
			return Of(n, d);
		}

		public static BigRational Subtract(BigRational a, BigDecimal value) {
			if (value == BigDecimal.Zero)
				return a;

			return Of(BigMath.Subtract(a.Numerator, BigMath.Multiply(value, a.Denominator)), a.Denominator);
		}

		public static BigRational Multiply(BigRational a, BigRational value) {
			if (a.IsZero || value.IsZero)
				return BigRational.Zero;
			if (a.Equals(BigRational.One))
				return value;
			if (value.Equals(BigRational.One))
				return a;

			var n = BigMath.Multiply(a.Numerator, value.Numerator);
			var d = BigMath.Multiply(a.Denominator, value.Denominator);
			return Of(n, d);
		}

		public static BigRational Multiply(BigRational a, BigDecimal value) {
			var n = BigMath.Multiply(a.Numerator, value);
			var d = a.Denominator;
			return Of(n, d);
		}

		public static BigRational Divide(BigRational a, BigRational value) {
			if (value.Equals(BigRational.One)) {
				return a;
			}

			var n = BigMath.Multiply(a.Numerator, value.Denominator);
			var d = BigMath.Multiply(a.Denominator, value.Numerator);
			return Of(n, d);
		}

		public static BigRational Divide(BigRational a, BigDecimal value) {
			var n = a.Numerator;
			var d = BigMath.Multiply(a.Denominator, value);
			return Of(n, d);
		}

		public static BigRational Pow(BigRational a, int exponent) {
			if (exponent == 0)
				return BigRational.One;
			if (exponent == 1)
				return a;

			BigInteger n;
			BigInteger d;
			if (exponent > 0) {
				n = BigMath.Pow(a.Numerator.ToBigInteger(), exponent);
				d = BigMath.Pow(a.Denominator.ToBigInteger(), exponent);
			} else {
				n = BigMath.Pow(a.Denominator.ToBigInteger(), -exponent);
				d = BigMath.Pow(a.Numerator.ToBigInteger(), -exponent);
			}

			return new BigRational(n, d);
		}

		private static readonly List<BigRational> bernoulliCache = new List<BigRational>();

		public static BigRational Bernoulli(int n) {
			if (n == 1) {
				return new BigRational(-1, 2);
			} else if (n % 2 == 1) {
				return BigRational.Zero;
			}

			lock (bernoulliCache) {
				int index = n / 2;

				if (bernoulliCache.Count <= index) {
					for (int i = bernoulliCache.Count; i <= index; i++) {
						BigRational b = CalculateBernoulli(i * 2);
						bernoulliCache.Add(b);
					}
				}

				return bernoulliCache[index];
			}
		}

		private static BigRational CalculateBernoulli(int n) {
			return Enumerable.Range(0, n).AsParallel().Select(k => {
				BigRational jSum = BigRational.Zero;
				BigRational bin = BigRational.One;
				for (int j = 0; j <= k; j++) {
					BigRational jPowN = Pow((BigRational)j, n);
					if (j % 2 == 0) {
						jSum = Add(jSum, Multiply(bin, jPowN));
					} else {
						jSum = Subtract(jSum, Multiply(bin, jPowN));
					}

					bin = Divide(Multiply(bin, (BigRational)(k - j)), (BigRational)(j + 1));
				}
				return Divide(jSum, (BigRational)(k + 1));
			}).Aggregate(BigRational.Zero, Add);
		}
	}
}