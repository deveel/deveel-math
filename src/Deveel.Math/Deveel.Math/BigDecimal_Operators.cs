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
	public sealed partial class BigDecimal {
		#region Arithmetic Operators

		public static BigDecimal operator +(BigDecimal a, BigDecimal b) {
			// In case of implicit operators apply the precision of the dividend
			return BigMath.Add(a, b);
		}

		public static BigDecimal operator -(BigDecimal a, BigDecimal b) {
			// In case of implicit operators apply the precision of the dividend
			return BigMath.Subtract(a, b);
		}

		public static BigDecimal operator /(BigDecimal a, BigDecimal b) {
			// In case of implicit operators apply the precision of the dividend
			return BigDecimalMath.Divide(a, b);
		}

		public static BigDecimal operator %(BigDecimal a, BigDecimal b) {
			// In case of implicit operators apply the precision of the dividend
			return BigMath.Remainder(a, b);
		}

		public static BigDecimal operator *(BigDecimal a, BigDecimal b) {
			// In case of implicit operators apply the precision of the dividend
			return BigMath.Multiply(a, b);
		}

		public static BigDecimal operator +(BigDecimal a) {
			return BigMath.Plus(a);
		}

		public static BigDecimal operator -(BigDecimal a) {
			return BigMath.Negate(a);
		}

		public static bool operator ==(BigDecimal a, BigDecimal b) {
			if ((object)a == null && (object)b == null)
				return true;
			if ((object)a == null)
				return false;
			return a.Equals(b);
		}

		public static bool operator !=(BigDecimal a, BigDecimal b) {
			return !(a == b);
		}

		public static bool operator >(BigDecimal a, BigDecimal b) {
			return a.CompareTo(b) < 0;
		}

		public static bool operator <(BigDecimal a, BigDecimal b) {
			return a.CompareTo(b) > 0;
		}

		public static bool operator >=(BigDecimal a, BigDecimal b) {
			return a == b || a > b;
		}

		public static bool operator <=(BigDecimal a, BigDecimal b) {
			return a == b || a < b;
		}

		public static BigDecimal operator >>(BigDecimal a, int b) {
			return BigMath.ShiftRight(a, b);
		}

		public static BigDecimal operator <<(BigDecimal a, int b) {
			return BigMath.ShiftLeft(a, b);
		}

		#endregion

		#region Implicit Operators

		public static implicit operator Int16(BigDecimal d) {
			return d.ToInt16Exact();
		}

		public static implicit operator Int32(BigDecimal d) {
			return d.ToInt32();
		}

		public static implicit operator Int64(BigDecimal d) {
			return d.ToInt64();
		}

		public static implicit operator Single(BigDecimal d) {
			return d.ToSingle();
		}

		public static implicit operator Double(BigDecimal d) {
			return d.ToDouble();
		}

		public static implicit operator BigInteger(BigDecimal d) {
			return d.ToBigInteger();
		}

		public static implicit operator String(BigDecimal d) {
			return d.ToString();
		}

		public static implicit operator BigDecimal(long value) {
			return new BigDecimal(value);
		}

		public static implicit operator BigDecimal(double value) {
			return new BigDecimal(value);
		}

		public static implicit operator BigDecimal(int value) {
			return new BigDecimal(value);
		}

		public static implicit operator BigDecimal(BigInteger value) {
			return new BigDecimal(value);
		}

		#endregion
	}
}
