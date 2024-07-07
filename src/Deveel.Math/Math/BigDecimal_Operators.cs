using System.Globalization;

namespace Deveel.Math
{
    public sealed partial class BigDecimal
    {
        public static BigDecimal operator +(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return BigMath.Add(a, b);
        }

        public static BigDecimal operator -(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return BigMath.Subtract(a, b);
        }

        public static BigDecimal operator /(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return BigDecimalMath.Divide(a, b);
        }

        public static BigDecimal operator %(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return BigMath.Remainder(a, b);
        }

        public static BigDecimal operator *(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return BigMath.Multiply(a, b);
        }

        public static BigDecimal operator +(BigDecimal a)
        {
            return BigMath.Plus(a);
        }

        public static BigDecimal operator -(BigDecimal a)
        {
            return BigMath.Negate(a);
        }

        public static bool operator ==(BigDecimal a, BigDecimal b)
        {
            if ((object)a == null && (object)b == null)
                return true;
            if ((object)a == null || (object)b == null)
                return false;
            return a.CompareTo(b) == 0;
        }

        public static bool operator !=(BigDecimal a, BigDecimal b)
        {
            return !(a == b);
        }

        public static bool operator >(BigDecimal a, BigDecimal b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <(BigDecimal a, BigDecimal b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >=(BigDecimal a, BigDecimal b)
        {
            return a == b || a > b;
        }

        public static bool operator <=(BigDecimal a, BigDecimal b)
        {
            return a == b || a < b;
        }

        public static BigDecimal operator >>(BigDecimal a, int b)
        {
            return BigMath.ShiftRight((BigInteger)a, b);
        }

        public static BigDecimal operator <<(BigDecimal a, int b)
        {
            return BigMath.ShiftLeft((BigInteger)a, b);
        }

        public static BigDecimal operator ++(BigDecimal a)
        {
            return a + One;
        }

        public static BigDecimal operator --(BigDecimal a)
        {
            return a - One;
        }

        public static explicit operator char(BigDecimal d)
        {
            return (char)d.ToInt32();
        }

        public static explicit operator sbyte(BigDecimal d)
        {
            return (sbyte)d.ToInt32();
        }

        public static explicit operator byte(BigDecimal d)
        {
            return (byte)d.ToInt32();
        }

        public static explicit operator short(BigDecimal d)
        {
            return (short)d.ToInt32();
        }

        public static explicit operator int(BigDecimal d)
        {
            return d.ToInt32();
        }

        public static explicit operator long(BigDecimal d)
        {
            return d.ToInt64();
        }

        public static implicit operator float(BigDecimal d)
        {
            return d.ToSingle();
        }

        public static implicit operator double(BigDecimal d)
        {
            return d.ToDouble();
        }

        public static implicit operator decimal(BigDecimal d)
        {
            return d.ToDecimal();
        }

        public static explicit operator BigInteger(BigDecimal d)
        {
            return d.ToBigInteger();
        }

        public static explicit operator BigDecimal(long value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(float value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(double value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(decimal value)
        {
            return Parse(value.ToString(CultureInfo.InvariantCulture));
        }

        public static explicit operator BigDecimal(int value)
        {
            return new BigDecimal(value);
        }

        public static implicit operator BigDecimal(BigInteger value)
        {
            return new BigDecimal(value);
        }

    }
}
