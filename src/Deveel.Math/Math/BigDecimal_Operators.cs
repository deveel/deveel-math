using System;
using System.Globalization;

namespace Deveel.Math
{
    public sealed partial class BigDecimal
    {
        /// <summary>
        /// Adds the specified <see cref="BigDecimal"/> to this instance.
        /// </summary>
        /// <param name="value">
        /// The <see cref="BigDecimal"/> to add to this instance.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the sum
        /// of this instance and the given value.
        /// </returns>
        /// <example>
        /// <code>
        /// var a = BigDecimal.Parse("123.45");
        /// var b = BigDecimal.Parse("67.89");
        /// var sum = a.Add(b);
        /// </code>
        /// </example>
        /// <seealso cref="operator +(BigDecimal, BigDecimal)"/>
        public BigDecimal Add(BigDecimal value)
        {
            return BigDecimalMath.Add(this, value);
        }

        /// <summary>
        /// Subtracts the specified <see cref="BigDecimal"/> from this instance.
        /// </summary>
        /// <param name="value">
        /// The <see cref="BigDecimal"/> to subtract from this instance.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the difference
        /// between this instance and the given value.
        /// </returns>
        /// <example>
        /// <code>
        /// var a = BigDecimal.Parse("123.45");
        /// var b = BigDecimal.Parse("67.89");
        /// var diff = a.Subtract(b);
        /// </code>
        /// </example>
        /// <seealso cref="operator -(BigDecimal, BigDecimal)"/>
        public BigDecimal Subtract(BigDecimal value)
        {
            return BigDecimalMath.Subtract(this, value);
        }

        /// <summary>
        /// Multiplies this instance by the specified <see cref="BigDecimal"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="BigDecimal"/> to multiply this instance by.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the product
        /// of this instance and the given value.
        /// </returns>
        /// <example>
        /// <code>
        /// var a = BigDecimal.Parse("123.45");
        /// var b = BigDecimal.Parse("67.89");
        /// var product = a.Multiply(b);
        /// </code>
        /// </example>
        /// <seealso cref="operator *(BigDecimal, BigDecimal)"/>
        public BigDecimal Multiply(BigDecimal value)
        {
            return BigDecimalMath.Multiply(this, value);
        }

        /// <summary>
        /// Divides this instance by the specified <see cref="BigDecimal"/>.
        /// </summary>
        /// <param name="dividend">
        /// The <see cref="BigDecimal"/> to divide this instance by.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the quotient
        /// of this instance divided by the given value.
        /// </returns>
        /// <example>
        /// <code>
        /// var a = BigDecimal.Parse("123.45");
        /// var b = BigDecimal.Parse("67.89");
        /// var quotient = a.Divide(b);
        /// </code>
        /// </example>
        /// <seealso cref="operator /(BigDecimal, BigDecimal)"/>
        public BigDecimal Divide(BigDecimal dividend)
        {
            return BigDecimalMath.Divide(this, dividend);
        }

        /// <summary>
        /// Divides this instance by the specified <see cref="BigDecimal"/>,
        /// rounding the result according to the given <see cref="RoundingMode"/>.
        /// </summary>
        /// <param name="dividend">
        /// The <see cref="BigDecimal"/> to divide this instance by.
        /// </param>
        /// <param name="roundingMode">
        /// The <see cref="RoundingMode"/> to apply to the result.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the quotient
        /// of this instance divided by the given value, rounded as specified.
        /// </returns>
        public BigDecimal Divide(BigDecimal dividend, RoundingMode roundingMode)
            => Divide(dividend, Scale, roundingMode);

        /// <summary>
        /// Divides this instance by the specified <see cref="BigDecimal"/>,
        /// rounding the result to the specified scale according to the
        /// given <see cref="RoundingMode"/>.
        /// </summary>
        /// <param name="dividend">
        /// The <see cref="BigDecimal"/> to divide this instance by.
        /// </param>
        /// <param name="scale">
        /// The scale of the quotient.
        /// </param>
        /// <param name="roundingMode">
        /// The <see cref="RoundingMode"/> to apply to the result.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the quotient
        /// of this instance divided by the given value, with the specified scale
        /// and rounding.
        /// </returns>
        public BigDecimal Divide(BigDecimal dividend, int scale, RoundingMode roundingMode)
        {
            return BigDecimalMath.Divide(this, dividend, scale, roundingMode);
        }

        /// <summary>
        /// Divides this instance by the specified <see cref="BigDecimal"/>,
        /// rounding the result according to the specified <see cref="MathContext"/>.
        /// </summary>
        /// <param name="dividend">
        /// The <see cref="BigDecimal"/> to divide this instance by.
        /// </param>
        /// <param name="mc">
        /// The <see cref="MathContext"/> that specifies the precision and
        /// rounding mode for the result.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the quotient
        /// of this instance divided by the given value, rounded as specified.
        /// </returns>
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
        /// <example>
        /// <code>
        /// var a = BigDecimal.Parse("123.45");
        /// var b = BigDecimal.Parse("67.89");
        /// var sum = a + b;
        /// </code>
        /// </example>
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
        /// <example>
        /// <code>
        /// var a = BigDecimal.Parse("123.45");
        /// var b = BigDecimal.Parse("67.89");
        /// var diff = a - b;
        /// </code>
        /// </example>
        public static BigDecimal operator -(BigDecimal a, BigDecimal b)
        {
            return a.Subtract(b);
        }

        /// <summary>
        /// Divides two <see cref="BigDecimal"/> instances.
        /// </summary>
        /// <param name="a">
        /// The <see cref="BigDecimal"/> to divide.
        /// </param>
        /// <param name="b">
        /// The <see cref="BigDecimal"/> to divide by.
        /// </param>
        /// <returns>
        /// Returns the quotient of the two <see cref="BigDecimal"/> instances.
        /// </returns>
        /// <example>
        /// <code>
        /// var a = BigDecimal.Parse("123.45");
        /// var b = BigDecimal.Parse("67.89");
        /// var quotient = a / b;
        /// </code>
        /// </example>
        /// <seealso cref="Divide(BigDecimal)"/>
        public static BigDecimal operator /(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return a.Divide(b);
        }

        /// <summary>
        /// Returns the remainder resulting from dividing two <see cref="BigDecimal"/> instances.
        /// </summary>
        /// <param name="a">
        /// The <see cref="BigDecimal"/> dividend.
        /// </param>
        /// <param name="b">
        /// The <see cref="BigDecimal"/> divisor.
        /// </param>
        /// <returns>
        /// Returns the remainder of the division of the two <see cref="BigDecimal"/> instances.
        /// </returns>
        public static BigDecimal operator %(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return BigMath.Remainder(a, b);
        }

        /// <summary>
        /// Multiplies two <see cref="BigDecimal"/> instances.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="BigDecimal"/> to multiply.
        /// </param>
        /// <param name="b">
        /// The second <see cref="BigDecimal"/> to multiply.
        /// </param>
        /// <returns>
        /// Returns the product of the two <see cref="BigDecimal"/> instances.
        /// </returns>
        /// <example>
        /// <code>
        /// var a = BigDecimal.Parse("123.45");
        /// var b = BigDecimal.Parse("67.89");
        /// var product = a * b;
        /// </code>
        /// </example>
        /// <seealso cref="Multiply(BigDecimal)"/>
        public static BigDecimal operator *(BigDecimal a, BigDecimal b)
        {
            // In case of implicit operators apply the precision of the dividend
            return a.Multiply(b);
        }

        /// <summary>
        /// Returns the value of the specified <see cref="BigDecimal"/>.
        /// </summary>
        /// <param name="a">
        /// The <see cref="BigDecimal"/> operand.
        /// </param>
        /// <returns>
        /// Returns the same <see cref="BigDecimal"/> value.
        /// </returns>
        public static BigDecimal operator +(BigDecimal a)
        {
            return BigMath.Plus(a);
        }

        /// <summary>
        /// Negates the specified <see cref="BigDecimal"/>.
        /// </summary>
        /// <param name="a">
        /// The <see cref="BigDecimal"/> to negate.
        /// </param>
        /// <returns>
        /// Returns the negation of the specified <see cref="BigDecimal"/>.
        /// </returns>
        /// <seealso cref="Negate()"/>
        public static BigDecimal operator -(BigDecimal a)
        {
            return a.Negate();
        }

        /// <summary>
        /// Determines whether two <see cref="BigDecimal"/> instances are equal.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <param name="b">
        /// The second <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if the two <see cref="BigDecimal"/> instances
        /// are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(BigDecimal a, BigDecimal b)
        {
            if ((object)a == null && (object)b == null)
                return true;
            if ((object)a == null || (object)b == null)
                return false;

            return a.CompareTo(b) == 0;
        }

        /// <summary>
        /// Determines whether two <see cref="BigDecimal"/> instances are not equal.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <param name="b">
        /// The second <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if the two <see cref="BigDecimal"/> instances
        /// are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(BigDecimal a, BigDecimal b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether the first <see cref="BigDecimal"/> is greater
        /// than the second.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <param name="b">
        /// The second <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if <paramref name="a"/> is greater than
        /// <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(BigDecimal a, BigDecimal b)
        {
            return a.CompareTo(b) > 0;
        }

        /// <summary>
        /// Determines whether the first <see cref="BigDecimal"/> is less
        /// than the second.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <param name="b">
        /// The second <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if <paramref name="a"/> is less than
        /// <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(BigDecimal a, BigDecimal b)
        {
            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Determines whether the first <see cref="BigDecimal"/> is greater
        /// than or equal to the second.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <param name="b">
        /// The second <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if <paramref name="a"/> is greater than or equal
        /// to <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >=(BigDecimal a, BigDecimal b)
        {
            return a == b || a > b;
        }

        /// <summary>
        /// Determines whether the first <see cref="BigDecimal"/> is less
        /// than or equal to the second.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <param name="b">
        /// The second <see cref="BigDecimal"/> to compare.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if <paramref name="a"/> is less than or equal
        /// to <paramref name="b"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <=(BigDecimal a, BigDecimal b)
        {
            return a == b || a < b;
        }

        /// <summary>
        /// Shifts the bits of the <see cref="BigDecimal"/> value to the right.
        /// </summary>
        /// <param name="a">
        /// The <see cref="BigDecimal"/> value to shift.
        /// </param>
        /// <param name="b">
        /// The number of bits to shift.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the value
        /// after shifting right by the specified number of bits.
        /// </returns>
        public static BigDecimal operator >>(BigDecimal a, int b)
        {
            return BigMath.ShiftRight((BigInteger)a, b);
        }

        /// <summary>
        /// Shifts the bits of the <see cref="BigDecimal"/> value to the left.
        /// </summary>
        /// <param name="a">
        /// The <see cref="BigDecimal"/> value to shift.
        /// </param>
        /// <param name="b">
        /// The number of bits to shift.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the value
        /// after shifting left by the specified number of bits.
        /// </returns>
        public static BigDecimal operator <<(BigDecimal a, int b)
        {
            return BigMath.ShiftLeft((BigInteger)a, b);
        }

        /// <summary>
        /// Increments the <see cref="BigDecimal"/> value by one.
        /// </summary>
        /// <param name="a">
        /// The <see cref="BigDecimal"/> to increment.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the value
        /// after incrementing by one.
        /// </returns>
        public static BigDecimal operator ++(BigDecimal a)
        {
            return a + One;
        }

        /// <summary>
        /// Decrements the <see cref="BigDecimal"/> value by one.
        /// </summary>
        /// <param name="a">
        /// The <see cref="BigDecimal"/> to decrement.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="BigDecimal"/> that represents the value
        /// after decrementing by one.
        /// </returns>
        public static BigDecimal operator --(BigDecimal a)
        {
            return a - One;
        }
    }
}
