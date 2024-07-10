using System;
using System.Globalization;

namespace Deveel.Math
{
    public sealed partial class BigDecimal
    {
        public BigDecimal Add(BigDecimal value)
        {
            return BigDecimalMath.Add(this, value);
        }

        public BigDecimal Subtract(BigDecimal value)
        {
            return BigDecimalMath.Subtract(this, value);
        }

        public BigDecimal Multiply(BigDecimal value)
        {
            return BigDecimalMath.Multiply(this, value);
        }

        public BigDecimal Divide(BigDecimal dividend)
        {
            return BigDecimalMath.Divide(this, dividend);
        }

        public BigDecimal Divide(BigDecimal dividend, RoundingMode roundingMode)
            => Divide(dividend, Scale, roundingMode);

        public BigDecimal Divide(BigDecimal dividend, int scale, RoundingMode roundingMode)
        {
            return BigDecimalMath.Divide(this, dividend, scale, roundingMode);
        }

        public BigDecimal Divide(BigDecimal dividend, MathContext mc)
            => BigDecimalMath.Divide(this, dividend, mc);

        /// <summary>
        /// Returns a new <see cref="BigDecimal"/> whose value is the negation
        /// of ths number, with the same scale as the original number.
        /// </summary>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that is the negation of the given number,
        /// with the same scale as the original number.
        /// </returns>
        public BigDecimal Negate()
        {
            if (BitLength < 63 || (BitLength == 63 && SmallValue != Int64.MinValue))
            {
                return BigDecimal.Create(-SmallValue, Scale);
            }

            return new BigDecimal(-UnscaledValue, Scale);
        }

        /// <summary>
        /// Returns a new <see cref="BigDecimal"/> whose value is the negation
        /// of the this number, rounded according to the specified precision
        /// scale and rounding mode.
        /// </summary>
        /// <param name="mc">
        /// The <see cref="MathContext"/> object that specifies the precision and
        /// rounding mode for the result of the operation.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that is the negation of the given number,
        /// rounded according to the specified precision scale and rounding mode.
        /// </returns>
        public BigDecimal Negate(MathContext mc)
        {
            return BigMath.Round(this, mc).Negate();
        }

        /// <summary>
        /// Adds two instances of <see cref="BigDecimal"/> together.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="BigDecimal"/> to add.
        /// </param>
        /// <param name="b">
        /// The second <see cref="BigDecimal"/> to add.
        /// </param>
        /// <remarks>
        /// <para>
        /// The precision of the result is the maximum of the precisions of 
        /// the two <see cref="BigDecimal"/> instances.
        /// </para>
        /// </remarks>
        /// <returns>
        /// Returns the sum of the two <see cref="BigDecimal"/> instances.
        /// </returns>
        /// <seealso cref="Add(BigDecimal)"/>
        public static BigDecimal operator +(BigDecimal a, BigDecimal b)
        {
            return a.Add(b);
        }

        /// <summary>
        /// Subtracts one <see cref="BigDecimal"/> instance from another.
        /// </summary>
        /// <param name="a">
        /// The <see cref="BigDecimal"/> instance to subtract from.
        /// </param>
        /// <param name="b">
        /// The <see cref="BigDecimal"/> instance to subtract.
        /// </param>
        /// <returns>
        /// Returns the result of subtracting the second <see cref="BigDecimal"/>
        /// operand from the first.
        /// </returns>
        public static BigDecimal operator -(BigDecimal a, BigDecimal b)
        {
            return a.Subtract(b);
        }

        public static BigDecimal operator /(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return a.Divide(b);
        }

        public static BigDecimal operator %(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return BigMath.Remainder(a, b);
        }

        public static BigDecimal operator *(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return a.Multiply(b);
        }

        public static BigDecimal operator +(BigDecimal a)
        {
            return BigMath.Plus(a);
        }

        public static BigDecimal operator -(BigDecimal a)
        {
            return a.Negate();
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
    }
}
