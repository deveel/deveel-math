using System.Globalization;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Deveel.Math
{
    public sealed partial class BigDecimal
    {
        /// <summary>
        /// Attempts to parse a string representation of a <see cref="BigDecimal"/> number.
        /// </summary>
        /// <param name="chars">
        /// The array of characters that contains the string representation of the number.
        /// </param>
        /// <param name="offset">
        /// The offset in the array where the number starts.
        /// </param>
        /// <param name="length">
        /// The length of the number in the array.
        /// </param>
        /// <param name="value">
        /// The parsed <see cref="BigDecimal"/> number, if the parsing is successful.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if the parsing is successful, otherwise <c>false</c>.
        /// </returns>
        /// <seealso cref="TryParse(char[], int, int, IFormatProvider, out BigDecimal)"/>
        public static bool TryParse(char[] chars, int offset, int length, [MaybeNullWhen(false)] out BigDecimal value) 
            => TryParse(chars, offset, length, NumberFormatInfo.InvariantInfo, out value);

        /// <summary>
        /// Attempts to parse a string representation of a <see cref="BigDecimal"/> number.
        /// </summary>
        /// <param name="chars">
        /// The array of characters that contains the string representation of the number.
        /// </param>
        /// <param name="offset">
        /// The offset in the array where the number starts.
        /// </param>
        /// <param name="length">
        /// The length of the number in the array.
        /// </param>
        /// <param name="provider">
        /// A <see cref="IFormatProvider"/> that provides culture-specific information about 
        /// the format of the number.
        /// </param>
        /// <param name="value">
        /// The parsed <see cref="BigDecimal"/> number, if the parsing is successful.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if the parsing is successful, otherwise <c>false</c>.
        /// </returns>
        /// <seealso cref="TryParse(char[], int, int, MathContext, IFormatProvider, out BigDecimal)"/>
        public static bool TryParse(char[] chars, int offset, int length, IFormatProvider provider, out BigDecimal value) 
            => TryParse(chars, offset, length, null, provider, out value);

        /// <summary>
        /// Attempts to parse a string representation of a <see cref="BigDecimal"/> number.
        /// </summary>
        /// <param name="chars">
        /// The array of characters that contains the string representation of the number.
        /// </param>
        /// <param name="offset">
        /// The offset in the array where the number starts.
        /// </param>
        /// <param name="length">
        /// The length of the number in the array.
        /// </param>
        /// <param name="context">
        /// The <see cref="MathContext"/> that defines the precision and rounding mode of the number.
        /// </param>
        /// <param name="value">
        /// The parsed <see cref="BigDecimal"/> number, if the parsing is successful.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if the parsing is successful, otherwise <c>false</c>.
        /// </returns>
        /// <seealso cref="TryParse(char[], int, int, MathContext, IFormatProvider, out BigDecimal)"/>
        public static bool TryParse(char[] chars, int offset, int length, MathContext context, out BigDecimal value)
        {
            return TryParse(chars, offset, length, context, null, out value);
        }

        /// <summary>
        /// Attempts to parse a string representation of a <see cref="BigDecimal"/> number.
        /// </summary>
        /// <param name="chars">
        /// The array of characters that contains the string representation of the number.
        /// </param>
        /// <param name="offset">
        /// The offset in the array where the number starts.
        /// </param>
        /// <param name="length">
        /// The length of the number in the array.
        /// </param>
        /// <param name="provider">
        /// A <see cref="IFormatProvider"/> that provides culture-specific information about 
        /// the format of the number.
        /// </param>
        /// <param name="context">
        /// The <see cref="MathContext"/> that defines the precision and rounding mode of the number.
        /// </param>
        /// <param name="value">
        /// The parsed <see cref="BigDecimal"/> number, if the parsing is successful.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if the parsing is successful, otherwise <c>false</c>.
        /// </returns>
        public static bool TryParse(char[] chars, int offset, int length, MathContext context, IFormatProvider provider,
            out BigDecimal value)
        {
            Exception error;
            if (!DecimalString.TryParse(chars, offset, length, provider, out value, out error))
                return false;

            if (context != null)
                value.InplaceRound(context);

            return true;
        }

        public static bool TryParse(char[] chars, out BigDecimal value)
        {
            return TryParse(chars, (MathContext)null, out value);
        }

        public static bool TryParse(char[] chars, MathContext context, out BigDecimal value)
        {
            return TryParse(chars, context, NumberFormatInfo.InvariantInfo, out value);
        }

        public static bool TryParse(char[] chars, IFormatProvider provider, out BigDecimal value)
        {
            return TryParse(chars, null, provider, out value);
        }

        public static bool TryParse(char[] chars, MathContext context, IFormatProvider provider, out BigDecimal value)
        {
            if (chars == null)
            {
                value = null;
                return false;
            }

            return TryParse(chars, 0, chars.Length, context, provider, out value);
        }

        public static BigDecimal Parse(char[] chars, int offset, int length, IFormatProvider provider)
        {
            return Parse(chars, offset, length, null, provider);
        }

        public static BigDecimal Parse(char[] chars, int offset, int length)
        {
            return Parse(chars, offset, length, (MathContext)null);
        }

        public static BigDecimal Parse(char[] chars, int offset, int length, MathContext context)
        {
            return Parse(chars, offset, length, context, NumberFormatInfo.InvariantInfo);
        }

        public static BigDecimal Parse(char[] chars, int offset, int length, MathContext context, IFormatProvider provider)
        {
            Exception error;
            BigDecimal value;
            if (!DecimalString.TryParse(chars, offset, length, provider, out value, out error))
                throw error;

            if (context != null)
                value.InplaceRound(context);

            return value;
        }

        public static BigDecimal Parse(char[] chars, IFormatProvider provider)
        {
            return Parse(chars, null, provider);
        }

        public static BigDecimal Parse(char[] chars)
        {
            return Parse(chars, (MathContext)null);
        }

        public static BigDecimal Parse(char[] chars, MathContext context)
        {
            return Parse(chars, context, NumberFormatInfo.InvariantInfo);
        }

        public static BigDecimal Parse(char[] chars, MathContext context, IFormatProvider provider)
        {
            if (chars == null)
                throw new ArgumentNullException("chars");

            return Parse(chars, 0, chars.Length, context, provider);
        }

        public static bool TryParse(string s, out BigDecimal value)
        {
            return TryParse(s, (MathContext)null, out value);
        }

        public static bool TryParse(string s, MathContext context, out BigDecimal value)
        {
            return TryParse(s, context, NumberFormatInfo.InvariantInfo, out value);
        }

        public static bool TryParse(string s, IFormatProvider provider, out BigDecimal value)
        {
            return TryParse(s, null, provider, out value);
        }

        public static bool TryParse(string s, MathContext context, IFormatProvider provider, out BigDecimal value)
        {
            if (String.IsNullOrEmpty(s))
            {
                value = null;
                return false;
            }

            var data = s.ToCharArray();

            Exception error;
            if (!DecimalString.TryParse(data, 0, data.Length, provider, out value, out error))
                return false;

            if (context != null)
                value.InplaceRound(context);

            return true;
        }

        public static BigDecimal Parse(string s, IFormatProvider provider)
        {
            return Parse(s, null, provider);
        }

        public static BigDecimal Parse(string s)
        {
            return Parse(s, (MathContext)null);
        }

        public static BigDecimal Parse(string s, MathContext context)
        {
            return Parse(s, context, NumberFormatInfo.InvariantInfo);
        }

        public static BigDecimal Parse(string s, MathContext context, IFormatProvider provider)
        {
            if (String.IsNullOrEmpty(s))
                throw new FormatException();

            var data = s.ToCharArray();

            Exception error;
            BigDecimal value;
            if (!DecimalString.TryParse(data, 0, data.Length, provider, out value, out error))
                throw error;

            if (context != null)
                value.InplaceRound(context);

            return value;
        }

    }
}
