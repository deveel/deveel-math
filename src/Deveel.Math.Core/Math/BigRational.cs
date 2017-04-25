using System;

namespace Deveel.Math {
	public struct BigRational : IComparable<BigRational> {
		public static readonly BigRational Zero = new BigRational(0);
		public static readonly  BigRational One = new BigRational(1);
		public static readonly BigRational Two = new BigRational(2);
		public static readonly BigRational Ten = new BigRational(10);

		public BigRational(BigDecimal numerator, BigDecimal denominator)
			: this() {
			BigDecimal n = numerator;
			BigDecimal d = denominator;

			if (d.Sign == 0)
				throw new ArithmeticException("Divide by zero");

			if (d.Sign < 0) {
				n = -n;
				d = -d;
			}

			Numerator = n;
			Denominator = d;
		}

		private BigRational(int value)
			: this(new BigDecimal(value), BigDecimal.One) {
		}

		public BigDecimal Numerator { get; }

		public BigDecimal Denominator { get; }

		public bool IsZero {
			get { return Numerator.Sign == 0; }
		}

		public bool IsPositive {
			get { return Numerator.Sign > 0; }
		}

		private int Precision {
			get { return CountDigits(Numerator.ToBigInteger()) + CountDigits(Denominator.ToBigInteger()); }
		}

		private static int CountDigits(BigInteger number) {
			double factor = System.Math.Log(2) / System.Math.Log(10);
			int digitCount = (int)(factor * number.BitLength + 1);
			if (BigMath.Pow(BigInteger.Ten, digitCount - 1).CompareTo(number) > 0) {
				return digitCount - 1;
			}
			return digitCount;
		}

		public BigDecimal ToBigDecimal() {
			int precision = System.Math.Max(Precision, MathContext.Decimal128.Precision);
			return ToBigDecimal(new MathContext(precision));
		}

		public BigDecimal ToBigDecimal(MathContext context) {
			return BigMath.Divide(Numerator, Denominator, context);
		}

		public int CompareTo(BigRational other) {
			if (this == other)
				return 0;

			return BigMath.Multiply(Numerator, other.Denominator).CompareTo(BigMath.Multiply(Denominator, other.Numerator));
		}

		public override int GetHashCode() {
			if (IsZero)
				return 0;

			return Numerator.GetHashCode() + Denominator.GetHashCode();
		}

		public override bool Equals(object obj) {
			if (!(obj is BigRational))
				return false;

			var other = (BigRational)obj;
			if (!Numerator.Equals(other.Numerator))
				return false;

			return Denominator.Equals(other.Denominator);
		}

		#region Operators

		public static explicit operator BigRational(BigInteger value) {
			if (value == BigInteger.Zero)
				return Zero;
			if (value == BigInteger.One)
				return One;

			return new BigRational(new BigDecimal(value), BigDecimal.One);
		}

		public static explicit operator BigRational(double value) {
			if (value == 0.0)
				return Zero;
			if (value == 1.0)
				return One;
			if (Double.IsInfinity(value))
				throw new FormatException();
			if (Double.IsNaN(value))
				throw new FormatException();

			return (BigRational)(BigDecimal)value;
		}

		public static explicit operator BigRational(BigDecimal value) {
			if (value.CompareTo(BigDecimal.Zero) == 0)
				return Zero;
			if (value.CompareTo(BigDecimal.One) == 0)
				return One;

			int scale = value.Scale;
			if (scale == 0)
				return new BigRational(value, BigDecimal.One);

			if (scale < 0) {
				var n = BigMath.Multiply(new BigDecimal(value.UnscaledValue), BigMath.MovePointLeft(BigDecimal.One, value.Scale));
				return new BigRational(n, BigDecimal.One);
			} else {
				var n = new BigDecimal(value.UnscaledValue);
				var d = BigMath.MovePointRight(BigDecimal.One, value.Scale);
				return new BigRational(n, d);
			}
		}

		public static explicit operator BigDecimal(BigRational value) {
			return value.ToBigDecimal();
		}

		public static bool operator ==(BigRational a, BigRational b) {
			return a.Equals(b);
		}

		public static bool operator !=(BigRational a, BigRational b) {
			return !(a == b);
		}

		public static BigRational operator -(BigRational value) {
			return BigMath.Negate(value);
		}

		public static BigRational operator ++(BigRational value) {
			return BigMath.Increment(value);
		}

		public static BigRational operator --(BigRational value) {
			return BigMath.Decrement(value);
		}

		public static BigRational operator +(BigRational a, BigRational b) {
			return BigMath.Add(a, b);
		}

		public static BigRational operator +(BigRational a, BigDecimal b) {
			return BigMath.Add(a, b);
		}

		public static BigRational operator +(BigRational a, BigInteger b) {
			return BigMath.Add(a, b);
		}

		public static BigRational operator -(BigRational a, BigRational b) {
			return BigMath.Subtract(a, b);
		}

		public static BigRational operator -(BigRational a, BigDecimal b) {
			return BigMath.Subtract(a, b);
		}

		public static BigRational operator -(BigRational a, BigInteger b) {
			return BigMath.Subtract(a, b);
		}

		public static BigRational operator *(BigRational a, BigRational b) {
			return BigMath.Multiply(a, b);
		}

		public static BigRational operator *(BigRational a, BigDecimal b) {
			return BigMath.Multiply(a, b);
		}

		public static BigRational operator *(BigRational a, BigInteger b) {
			return BigMath.Multiply(a, b);
		}

		public static BigRational operator /(BigRational a, BigRational b) {
			return BigMath.Divide(a, b);
		}

		public static BigRational operator /(BigRational a, BigDecimal b) {
			return BigMath.Divide(a, b);
		}

		public static BigRational operator /(BigRational a, BigInteger b) {
			return BigMath.Divide(a, b);
		}

		#endregion
	}
}