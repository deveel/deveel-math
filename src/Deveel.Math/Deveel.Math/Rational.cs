// 
//  Copyright 2009-2014  Deveel
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
#if !PORTABLE
	[Serializable]
#endif
	public struct Rational : IComparable<Rational>, IComparable
#if !PORTABLE
		, IConvertible 
#endif
		{
		public static readonly BigInteger MaxInt32 = BigInteger.ValueOf(Int32.MaxValue);
		public static readonly BigInteger MinInt32 = BigInteger.ValueOf(Int32.MinValue);

		/// <summary>
		///     The constant value of 1
		/// </summary>
		public static readonly Rational One = new Rational(1, 1);

		/// <summary>
		///     The constant value of 0.
		/// </summary>
		public static readonly Rational Zero = new Rational(0, 0);

		/// <summary>
		///     The constant of 1/2.
		/// </summary>
		public static readonly Rational Half = new Rational(1, 2);

		public static readonly Rational None = new Rational(null, null);

		public Rational(BigInteger numerator)
			: this(numerator, BigInteger.ValueOf(1)) {
		}

		public Rational(BigInteger numerator, BigInteger denominator)
			: this() {
			if (numerator == null)
				throw new ArgumentNullException("numerator");
			if (denominator == null)
				throw new ArgumentNullException("denominator");

			Denominator = denominator;
			Numerator = numerator;

			Normalize();
		}

		public Rational(int numerator)
			: this(numerator, 1) {
		}

		public Rational(int numerator, int denominator)
			: this(BigInteger.ValueOf(numerator), BigInteger.ValueOf(denominator)) {
		}

		/// <summary>
		///     Gets the numerator of the reduced fraction.
		/// </summary>
		public BigInteger Numerator { get; private set; }

		/// <summary>
		///     Gets the denominator of the reduced fraction.
		/// </summary>
		public BigInteger Denominator { get; private set; }

		public int Sign {
			get { return (Denominator.Sign*Numerator.Sign); }
		}

		public bool IsInteger {
			get {
				if (!IsBigInteger)
					return false;

				return (Numerator.CompareTo(MaxInt32) <= 0 && Numerator.CompareTo(MinInt32) >= 0);
			}
		}

		public bool IsBigInteger {
			get { return (Denominator.Abs().CompareTo(BigInteger.One) == 0); }
		}

		public bool IsIntegerFraction {
			get {
				return (Numerator.CompareTo(MaxInt32) <= 0 &&
				        Numerator.CompareTo(MinInt32) >= 0 &&
				        Denominator.CompareTo(MaxInt32) <= 0 &&
				        Denominator.CompareTo(MinInt32) >= 0);
			}
		}

		public override bool Equals(object obj) {
			if (!(obj is Rational))
				return false;

			return CompareTo((Rational)obj) == 0;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public int CompareTo(object obj) {
			if (!(obj is Rational))
				throw new ArgumentException();

			return CompareTo((Rational) obj);
		}

		public int CompareTo(Rational other) {
			BigInteger left = Numerator.Multiply(other.Denominator);
			BigInteger right = other.Numerator.Multiply(Denominator);
			return left.CompareTo(right);
		}

		private void Normalize() {
			// compute greatest common divisor of numerator and denominator
			BigInteger g = Numerator.Gcd(Denominator);
			if (g.CompareTo(BigInteger.One) > 0) {
				Numerator = Numerator.Divide(g);
				Denominator = Denominator.Divide(g);
			}
			if (Denominator.CompareTo(BigInteger.Zero) == -1) {
				Numerator = Numerator.Negate();
				Denominator = Denominator.Negate();
			}
		}

		public Rational Add(Rational val) {
			BigInteger num = Numerator.Multiply(val.Denominator).Add(Denominator.Multiply(val.Numerator));
			BigInteger deno = Denominator.Multiply(val.Denominator);
			return (new Rational(num, deno));
		} /* Rational.add */

		public Rational Add(BigInteger val) {
			return Add(new Rational(val, BigInteger.One));
		}

		public Rational Add(int val) {
			BigInteger val2 = Numerator.Add(Denominator.Multiply(BigInteger.Parse("" + val)));
			return new Rational(val2, Denominator);
		}

		public Rational Subtract(Rational val) {
			Rational val2 = val.Negate();
			return Add(val2);
		}

		public Rational Subtract(BigInteger val) {
			return (Subtract(new Rational(val, BigInteger.One)));
		}

		public Rational Subtract(int val) {
			return Subtract(new Rational(val, 1));
		}

		public Rational Negate() {
			return (new Rational(Numerator.Negate(), Denominator));
		}

		public Rational Divide(Rational val) {
			if (val.CompareTo(Zero) == 0)
				throw new DivideByZeroException();

			BigInteger num = Numerator.Multiply(val.Denominator);
			BigInteger deno = Denominator.Multiply(val.Numerator);

			/* Reduction to a coprime format is done inside the ctor,
                * and not repeated here.
                */
			return (new Rational(num, deno));
		}

		public Rational Divide(BigInteger val) {
			if (val.CompareTo(BigInteger.Zero) == 0)
				throw new DivideByZeroException();

			return Divide(new Rational(val, BigInteger.One));
		}

		public Rational Divide(int val) {
			if (val == 0)
				throw new DivideByZeroException();

			return Divide(new Rational(val, 1));
		}

		public Rational Multiply(Rational val) {
			BigInteger num = Numerator.Multiply(val.Numerator);
			BigInteger deno = Denominator.Multiply(val.Denominator);
			/* Normalization to an coprime format will be done inside
                * the ctor() and is not duplicated here.
                */
			return new Rational(num, deno);
		}

		public Rational Multiply(BigInteger val) {
			return Multiply(new Rational(val, BigInteger.One));
		}

		public Rational Multiply(int val) {
			return Multiply(BigInteger.ValueOf(val));
		}

		public Rational Pow(int exponent) {
			if (exponent == 0)
				return new Rational(1, 1);

			BigInteger num = Numerator.Pow(System.Math.Abs(exponent));
			BigInteger deno = Denominator.Pow(System.Math.Abs(exponent));
			if (exponent > 0)
				return (new Rational(num, deno));

			return (new Rational(deno, num));
		}

		public Rational Pow(BigInteger exponent) {
			/* test for overflow */
			if (exponent.CompareTo(MaxInt32) == 1)
				throw new FormatException("Exponent " + exponent + " too large.");
			if (exponent.CompareTo(MinInt32) == -1)
				throw new FormatException("Exponent " + exponent + " too small.");

			/* promote to the simpler interface above */
			return Pow(exponent.ToInt32());
		}

		public Rational Pow(Rational exponent) {
			if (exponent.Numerator.CompareTo(BigInteger.Zero) == 0)
				return new Rational(1, 1);

			/* calculate (a/b)^(exponent.a/exponent.b) as ((a/b)^exponent.a)^(1/exponent.b)
                * = tmp^(1/exponent.b)
                */
			Rational tmp = Pow(exponent.Numerator);
			return tmp.Root(exponent.Denominator);
		}

		public Rational Root(BigInteger r) {
			/* test for overflow */
			if (r.CompareTo(MaxInt32) == 1)
				throw new FormatException("Root " + r + " too large.");
			if (r.CompareTo(MinInt32) == -1)
				throw new FormatException("Root " + r + " too small.");

			int rthroot = r.ToInt32();

			/* cannot pull root of a negative value with even-valued root */
			if (CompareTo(Zero) == -1 && (rthroot%2) == 0)
				throw new FormatException("Negative basis " + ToString() + " with odd root " + r);

			/* extract a sign such that we calculate |n|^(1/r), still r carrying any sign
                */
			bool flipsign = (CompareTo(Zero) == -1 && (rthroot%2) != 0);

			/* delegate the main work to ifactor#root()
                */
			var num = new IFactor(Numerator.Abs());
			var deno = new IFactor(Denominator);
			Rational resul = num.Root(rthroot).Divide(deno.Root(rthroot));
			return flipsign ? resul.Negate() : resul;
		}

		public Rational Abs() {
			return (new Rational(Numerator.Abs(), Denominator.Abs()));
		}

		public Rational Pochhammer(BigInteger n) {
			if (n.CompareTo(BigInteger.Zero) < 0)
				return None;
			if (n.CompareTo(BigInteger.Zero) == 0)
				return One;

			/* initialize results with the current value
                        */
			var res = new Rational(Numerator, Denominator);
			BigInteger i = BigInteger.One;
			for (; i.CompareTo(n) < 0; i = i.Add(BigInteger.One))
				res = res.Multiply(Add(i));
			return res;
		}

		public BigInteger Truncate() {
			return Denominator.CompareTo(BigInteger.One) == 0 ? Numerator : Numerator.Divide(Denominator);
		}

		public BigInteger Ceiling() {
			if (Denominator.CompareTo(BigInteger.One) == 0)
				return Numerator;
			if (Numerator.CompareTo(BigInteger.Zero) > 0)
				return Numerator.Divide(Denominator).Add(BigInteger.One);
			return Numerator.Divide(Denominator);
		}

		public BigInteger Floor() {
			/* is already integer: return the numerator
			*/
			if (Denominator.CompareTo(BigInteger.One) == 0)
				return Numerator;
			if (Numerator.CompareTo(BigInteger.Zero) > 0)
				return Numerator.Divide(Denominator);
			return Numerator.Divide(Denominator).Subtract(BigInteger.One);
		}

		public static Rational Binomial(Rational n, int m) {
			if (m == 0)
				return One;
			Rational bin = n;
			for (int i = 2; i <= m; i++)
				bin = bin.Multiply(n.Subtract(i - 1)).Divide(i);
			return bin;
		}

		public static Rational Binomial(Rational n, BigInteger m) {
			if (m.CompareTo(BigInteger.Zero) == 0)
				return One;
			Rational bin = n;
			for (BigInteger i = BigInteger.ValueOf(2); i.CompareTo(m) != 1; i = i.Add(BigInteger.One))
				bin = bin.Multiply(n.Subtract(i.Subtract(BigInteger.One))).Divide(i);
			return bin;
		}

		public static Rational HankelSymb(Rational n, int k) {
			if (k == 0)
				return One;
			if (k < 0)
				throw new ArithmeticException("Negative parameter " + k);

			Rational nkhalf = n.Subtract(k).Add(Half);
			nkhalf = nkhalf.Pochhammer(2*k);
			var f = new Factorial();
			return nkhalf.Divide(f[k]);
		}

		public Rational Pochhammer(int n) {
			return Pochhammer(BigInteger.ValueOf(n));
		}

		public Rational Min(Rational val) {
			return CompareTo(val) < 0 ? this : val;
		}

		public Rational Max(Rational val) {
			return CompareTo(val) > 0 ? this : val;
		}

		public static Rational Parse(string s, int radix) {
			Rational value;
			if (!TryParse(s, radix, out value))
				throw new FormatException("Unable to parse the given string into a Rational");

			return value;
		}

		public static bool TryParse(string s, int radix, out Rational value) {
			if (String.IsNullOrEmpty(s))
				throw new FormatException();

			try {
				BigInteger num;
				BigInteger denom;

				int index = s.IndexOf('/');
				if (index != -1) {
					string s1 = s.Substring(0, index).Trim();
					string s2 = s.Substring(index + 1).Trim();

					num = BigInteger.Parse(s1, radix);
					denom = BigInteger.Parse(s2, radix);
				} else {
					num = BigInteger.Parse(s, radix);
					denom = BigInteger.ValueOf(1);
				}

				value = new Rational(num, denom);
			} catch (FormatException) {
				// TODO: instead of trapping an exception, implement a TryParse on BigDecimal
				value = Zero;
				return false;
			}

			return true;
		}

		public float ToSingle() {
			BigDecimal adivb = (new BigDecimal(Numerator)).Divide(new BigDecimal(Denominator), MathContext.Decimal128);
			return adivb.ToSingle();
		}

		public double ToDouble() {
			/* To meet the risk of individual overflows of the exponents of
			* a separate invocation a.doubleValue() or Denominator.doubleValue(), we divide first
			* in a BigDecimal environment and convert the result.
			*/
			BigDecimal adivb = (new BigDecimal(Numerator)).Divide(new BigDecimal(Denominator), MathContext.Decimal128);
			return adivb.ToDouble();
		}

		public BigDecimal ToBigDecimal(MathContext mc) {
			/* numerator and denominator individually rephrased
			*/
			var n = new BigDecimal(Numerator);
			var d = new BigDecimal(Denominator);
			/* the problem with n.divide(d,mc) is that the apparent precision might be
			* smaller than what is set by mc if the value has a precise truncated representation.
			* 1/4 will appear as 0.25, independent of mc
			*/
			return BigMath.ScalePrecision(n.Divide(d, mc), mc);
		}

		public override string ToString() {
			return Denominator.CompareTo(BigInteger.One) != 0 ? Numerator + "/" + Denominator : Numerator.ToString();
		}

		public byte ToByte() {
			if (!IsInteger)
				throw new InvalidCastException();

			var i = Numerator.Abs().ToInt32();
			if (i > Byte.MaxValue ||
			    i < Byte.MinValue)
				throw new InvalidCastException();

			return (byte) i;
		}

		public sbyte ToSByte() {
			if (!IsInteger)
				throw new InvalidCastException();

			var i = Numerator.ToInt32();
			if (i > SByte.MaxValue ||
			    i < SByte.MinValue)
				throw new InvalidCastException();

			return (sbyte) i;
		}

		public short ToInt16() {
			var i = ToInt32();
			if (i > Int16.MaxValue ||
			    i < Int16.MinValue)
				throw new InvalidCastException();

			return (short) i;
		}

		public int ToInt32() {
			if (!IsInteger)
				throw new InvalidCastException();

			return Numerator.ToInt32();
		}

		public long ToInt64() {
			if (!IsInteger)
				throw new InvalidCastException();

			return Numerator.ToInt64();
		}

		public BigInteger ToBigInteger() {
			if (!IsBigInteger)
				throw new InvalidCastException();

			return Numerator;
		}

#if !PORTABLE
		TypeCode IConvertible.GetTypeCode() {
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		char IConvertible.ToChar(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider) {
			return ToSByte();
		}

		byte IConvertible.ToByte(IFormatProvider provider) {
			return ToByte();
		}

		short IConvertible.ToInt16(IFormatProvider provider) {
			return ToInt16();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider) {
			throw new NotImplementedException();
		}

		int IConvertible.ToInt32(IFormatProvider provider) {
			return ToInt32();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider) {
			throw new NotImplementedException();
		}

		long IConvertible.ToInt64(IFormatProvider provider) {
			return ToInt64();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider) {
			throw new NotImplementedException();
		}

		float IConvertible.ToSingle(IFormatProvider provider) {
			return ToSingle();
		}

		double IConvertible.ToDouble(IFormatProvider provider) {
			return ToDouble();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider) {
			throw new NotImplementedException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider) {
			throw new InvalidCastException();
		}

		string IConvertible.ToString(IFormatProvider provider) {
			return ToString();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
			if (conversionType == typeof (byte))
				return ToByte();
			if (conversionType == typeof (sbyte))
				return ToSByte();
			if (conversionType == typeof (short))
				return ToInt16();
			if (conversionType == typeof (int))
				return ToInt32();
			if (conversionType == typeof (long))
				return ToInt64();
			if (conversionType == typeof (float))
				return ToSingle();
			if (conversionType == typeof (double))
				return ToDouble();

			if (conversionType == typeof (BigInteger))
				return ToBigInteger();
			if (conversionType == typeof (BigDecimal))
				return ToBigDecimal(MathContext.Decimal128);

			if (conversionType == typeof (string))
				return ToString();

			throw new InvalidCastException();
		}
#endif

		public static Rational operator +(Rational a, Rational b) {
			return a.Add(b);
		}

		public static Rational operator +(Rational a, BigInteger b) {
			return a.Add(b);
		}

		public static Rational operator +(Rational a, int b) {
			return a.Add(b);
		}

		public static Rational operator -(Rational a, Rational b) {
			return a.Subtract(b);
		}

		public static Rational operator -(Rational a, BigInteger b) {
			return a.Subtract(b);
		}

		public static Rational operator -(Rational a, int b) {
			return a.Subtract(b);
		}

		public static Rational operator *(Rational a, Rational b) {
			return a.Multiply(b);
		}

		public static Rational operator *(Rational a, BigInteger b) {
			return a.Multiply(b);
		}

		public static Rational operator *(Rational a, int b) {
			return a.Multiply(b);
		}

		public static Rational operator /(Rational a, Rational b) {
			return a.Divide(b);
		}

		public static Rational operator /(Rational a, BigInteger b) {
			return a.Divide(b);
		}

		public static Rational operator /(Rational a, int b) {
			return a.Divide(b);
		}

		public static Rational operator -(Rational a) {
			return a.Negate();
		}

		public static bool operator ==(Rational a, Rational b) {
			return a.Equals(b);
		}

		public static bool operator !=(Rational a, Rational b) {
			return !(a == b);
		}

		public static bool operator >(Rational a, Rational b) {
			return a.CompareTo(b) < 0;
		}

		public static bool operator <(Rational a, Rational b) {
			return a.CompareTo(b) > 0;
		}

		public static bool operator >=(Rational a, Rational b) {
			var i = a.CompareTo(b);
			return i < 0 || i == 0;
		}

		public static bool operator <=(Rational a, Rational b) {
			var i = a.CompareTo(b);
			return i > 0 || i == 0;
		}

		public static Rational operator ++(Rational a) {
			return a.Add(BigInteger.One);
		}

		public static Rational operator --(Rational a) {
			return a.Subtract(BigInteger.One);
		}
	}
}