using System;

namespace Deveel.Math {
	public sealed partial class BigInteger {
		internal BigInteger ShiftLeftOneBit() {
			return (sign == 0) ? this : BitLevel.ShiftLeftOneBit(this);
		}

		/**
		 * Returns a new {@code BigInteger} whose value is {@code this & ~val}.
		 * Evaluating {@code x.andNot(val)} returns the same result as {@code
		 * x.and(val.not())}.
		 * <p>
		 * <b>Implementation Note:</b> Usage of this method is not recommended as
		 * the current implementation is not efficient.
		 *
		 * @param val
		 *            value to be not'ed and then and'ed with {@code this}.
		 * @return {@code this & ~val}.
		 * @throws NullPointerException
		 *             if {@code val == null}.
		 */
		public BigInteger AndNot(BigInteger val) {
			return Logical.AndNot(this, val);
		}


		public static BigInteger operator +(BigInteger a, BigInteger b) {
			return BigMath.Add(a, b);
		}

		public static BigInteger operator -(BigInteger a, BigInteger b) {
			return BigMath.Subtract(a, b);
		}

		public static BigInteger operator *(BigInteger a, BigInteger b) {
			return BigMath.Multiply(a, b);
		}

		public static BigInteger operator /(BigInteger a, BigInteger b) {
			return BigMath.Divide(a, b);
		}

		public static BigInteger operator %(BigInteger a, BigInteger b) {
			return BigMath.Mod(a, b);
		}

		public static BigInteger operator &(BigInteger a, BigInteger b) {
			return BigMath.And(a, b);
		}

		public static BigInteger operator |(BigInteger a, BigInteger b) {
			return BigMath.Or(a, b);
		}

		public static BigInteger operator ^(BigInteger a, BigInteger b) {
			return BigMath.XOr(a, b);
		}

		public static BigInteger operator ~(BigInteger a) {
			return BigMath.Not(a);
		}

		public static BigInteger operator -(BigInteger a) {
			return BigMath.Negate(a);
		}

		public static BigInteger operator >>(BigInteger a, int b) {
			return BigMath.ShiftRight(a, b);
		}

		public static BigInteger operator <<(BigInteger a, int b) {
			return BigMath.ShiftLeft(a, b);
		}

		public static bool operator >(BigInteger a, BigInteger b) {
			return a.CompareTo(b) > 0;
		}

		public static bool operator <(BigInteger a, BigInteger b) {
			return a.CompareTo(b) < 0;
		}

		public static bool operator ==(BigInteger a, BigInteger b) {
			if ((object)a == null && (object)b == null)
				return true;
			if ((object)a == null)
				return false;
			return a.Equals(b);
		}

		public static bool operator !=(BigInteger a, BigInteger b) {
			return !(a == b);
		}

		public static bool operator >=(BigInteger a, BigInteger b) {
			return a == b || a > b;
		}

		public static bool operator <=(BigInteger a, BigInteger b) {
			return a == b || a < b;
		}

		#region Implicit Operators

		public static implicit operator Int32(BigInteger i) {
			return i.ToInt32();
		}

		public static implicit operator Int64(BigInteger i) {
			return i.ToInt64();
		}

		public static implicit operator Single(BigInteger i) {
			return i.ToSingle();
		}

		public static implicit operator Double(BigInteger i) {
			return i.ToDouble();
		}

		public static implicit operator String(BigInteger i) {
			return i.ToString();
		}

		public static implicit operator BigInteger(int value) {
			return FromInt64(value);
		}

		public static implicit operator BigInteger(long value) {
			return FromInt64(value);
		}

		#endregion
	}
}
